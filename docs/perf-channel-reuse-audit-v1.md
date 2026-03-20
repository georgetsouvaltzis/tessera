# Perf Channel/Reuse Audit v1 (2026-03-20)

## Scope
- `src/TeaSharp`
- `src/TeaSharp.Core`
- Focus: `Channel<T>` lifecycle/reuse, backpressure behavior, bounded vs unbounded queues, and `Memory<T>`/`IMemoryOwner<T>`/`ArrayPool<T>` opportunities in high-frequency loops.

## Findings Summary
- `Channel<T>` usage is concentrated in `TeaSharp.Core` runtime internals:
  - `TeaRuntimeLoop` creates two unbounded channels (`_messages`, `_effects`) and uses `TryWrite` for ingress.
  - `TeaResizeMonitor` creates an unbounded signal channel (`Channel<bool>`) even though it later coalesces signals.
- `src/TeaSharp` (UI/control layer) currently has no direct `Channel<T>` usage.
- High-frequency allocation pressure is more visible in runtime/support loops than in the channel APIs themselves:
  - Resize monitor loop builds tasks repeatedly (`Task.Delay`, `WaitToReadAsync(...).AsTask()`).
  - Terminal input buffering expands with new arrays when needed (no pooling).

## Ranked Recommendations

### 1) Make resize signal channel bounded/coalescing (low risk, high confidence)
**Evidence**
- `TeaResizeMonitor` uses `Channel.CreateUnbounded<bool>(...)` and then immediately drains all pending items each loop.
- Files: `src/TeaSharp.Core/Application/Internal/TeaResizeMonitor.cs`.

**Minimal-diff change**
- Replace unbounded channel with bounded capacity `1` and drop policy (`DropOldest` or `DropWrite`) to preserve coalescing semantics explicitly.
- Keep `TryWrite(true)` signaling style.

**Benefit**
- Prevents unbounded queue growth during resize storms.
- Clarifies intent: only “at least one pending resize” is needed.

**Risk**
- Very low; behavior already coalesces via drain loop.

---

### 2) Add explicit channel options in runtime loop (low risk, moderate benefit)
**Evidence**
- `TeaRuntimeLoop` uses `Channel.CreateUnbounded<IMessage>()` and `Channel.CreateUnbounded<Effect>()` with default options.
- Files: `src/TeaSharp.Core/Application/Internal/TeaRuntimeLoop.cs`.

**Minimal-diff change**
- Use `new UnboundedChannelOptions { SingleReader = true, SingleWriter = false }` for `_messages`.
- Use `new UnboundedChannelOptions { SingleReader = true, SingleWriter = true }` for `_effects` (single producer path in current design).
- Keep unbounded behavior for now; only tune metadata/locking assumptions.

**Benefit**
- Lower synchronization overhead in hot paths.
- Better documents thread/ownership model.

**Risk**
- Low; requires validating `_effects` truly remains single-writer as architecture evolves.

---

### 3) Introduce targeted backpressure for effects queue (medium risk, potentially high benefit under bursts)
**Evidence**
- `_effects` is unbounded and accepts writes from message processing loop (`WriteAsync`), while execution may be slower when effects are expensive.
- Files: `src/TeaSharp.Core/Application/Internal/TeaRuntimeLoop.cs`, `src/TeaSharp.Core/Application/Internal/TeaEffectScheduler.cs`.

**Minimal-diff change**
- Option A: bounded `_effects` channel with configurable capacity (for example, `MaxConcurrentEffects * N`).
- Option B: keep unbounded by default but add optional bounded mode via `TeaRuntimeLoopOptions`.

**Benefit**
- Prevents memory growth in effect-heavy workloads.
- Converts burst pressure to producer throttling.

**Risk**
- Medium; may change responsiveness/order under extreme load. Needs explicit policy choice.

---

### 4) Pool terminal reader buffers (medium risk, moderate benefit for long sessions/repeated startups)
**Evidence**
- `TerminalReader` allocates fixed `readBuffer` plus expandable pending arrays; growth allocates new arrays and copies.
- Files: `src/TeaSharp.Core/Input/TerminalReader.cs`.

**Minimal-diff change**
- Rent `readBuffer` from `ArrayPool<byte>.Shared` and return in `finally`.
- Convert pending buffer internal storage to pooled array ownership with explicit return.
- Keep API surface unchanged.

**Benefit**
- Reduces LOH/Gen0 churn during heavy input bursts and repeated lifecycle runs.
- Better memory locality for sustained input streams.

**Risk**
- Medium; requires strict return/clear discipline to avoid leaks/data retention.

---

### 5) Reduce per-iteration task allocations in resize monitor loop (low-medium risk, moderate benefit)
**Evidence**
- Each loop creates `Task.Delay(...)` and `WaitToReadAsync(...).AsTask()`; both allocate repeatedly.
- File: `src/TeaSharp.Core/Application/Internal/TeaResizeMonitor.cs`.

**Minimal-diff change**
- Replace dual-task `WhenAny` pattern with `PeriodicTimer` plus non-blocking channel drain checks, or a single await strategy with cancellation-aware timeout handling.

**Benefit**
- Cuts steady-state allocator pressure in idle/resize polling path.

**Risk**
- Low-medium; timer semantics and shutdown cancellation need careful parity checks.

## Measurement Plan (before/after for each accepted change)

### A) Benchmark-level allocation signal
1. `dotnet build benchmarks/TeaSharp.Benchmarks/TeaSharp.Benchmarks.csproj -c Release --no-restore`
2. `dotnet run --project benchmarks/TeaSharp.Benchmarks -c Release -- --filter "*ResizeStorm*" --warmupCount 1 --minIterationCount 8 --maxIterationCount 12`
3. Compare:
   - `Allocated` / `Gen0` / `Gen1`
   - Mean for `RenderResizeStormFramesOnly` (primary) and `RenderResizeStormFrames` (secondary)

### B) Runtime channel stress signal
1. Build release:
   - `dotnet build TeaSharp.slnx -c Release --no-restore`
2. Run a message/effect flood scenario (existing runtime tests or dedicated harness).
3. Collect counters:
   - `dotnet-counters monitor --process-id <pid> System.Runtime`
4. Track:
   - `GC Heap Size`
   - `Gen 0/1/2 GC Count`
   - `Allocation Rate`
   - `ThreadPool Queue Length`

### C) Trace validation for queue growth behavior
1. `dotnet-trace collect --process-id <pid> --providers Microsoft-DotNETCore-SampleProfiler`
2. Validate:
   - No unbounded queue growth under resize/effect bursts.
   - No regression in shutdown cancellation behavior.

## Recommended rollout order
1. Recommendation 1 + 2 (safe and minimal).
2. Recommendation 5.
3. Recommendation 4 (with careful ownership tests).
4. Recommendation 3 only behind option flag until load behavior is confirmed.
