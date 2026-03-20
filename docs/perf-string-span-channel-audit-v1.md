# TeaSharp Full-Repo Perf String/Span/Channel Audit (V1)

Date (UTC): 2026-03-20  
Lane: Performance lead (analysis-first, orchestration notes)  
Change type: docs-only

## Skill usage (required order)
1. `dotnet_optimization_techniques`
2. `dotnet_strings_and_spans_best_practices`

Repo-profile checks (version safety):
- SDK: `10.0.103` (`global.json`)
- primary targets: `net10.0` (`src`, `tests`, `benchmarks`, `examples`)
- nullable/implicit usings: enabled
- implication: recommendations using `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>`, `ReadOnlyMemory<T>`, `string.Create`, and pooling APIs are version-safe for this repo.

## Scope and evidence

Audited scope (whole repo, where perf-relevant):
- product code: `src/TeaSharp`, `src/TeaSharp.Core`
- test code: `tests/*`
- benchmark harness: `benchmarks/*`
- sample apps: `examples/*`
- perf tooling/docs: `scripts/*`, `docs/*` (when affecting measurement fidelity/orchestration)

Inventory snapshot:
- total files scanned under scope: `468`
- `src`: `352`
- `tests`: `56`
- `benchmarks`: `11`
- `examples`: `22`
- `scripts`: `3`
- `docs`: `24`

Key evidence commands used:
- `rg -n "Channel<|Channel\\.Create|System\\.Threading\\.Channels|ArrayPool|..."`
- `rg -n "string\\.Concat|string\\.Join|StringBuilder|\\.ToString\\(|Substring\\(|Split\\(|Replace\\(|string\\.Create|AsSpan\\(|ReadOnlySpan<|Memory<|ReadOnlyMemory<"`
- focused file inspections with `nl -ba <file> | sed -n ...`
- benchmark references from current perf lane:
  - `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`

Current measured alloc pressure anchors (materialize mode):
- `OverlayStress ~1.42 MB`
- `ResizeStorm ~1.2 MB`
- `Viewport ~1701.5 KB`

## Severity-ranked findings

### F1 (Critical): full-frame string materialization in render path dominates allocations
Evidence:
- `src/TeaSharp/Components/Canvas/Canvas.cs:424`
- `src/TeaSharp/Components/Canvas/Canvas.cs:433`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:173`
- `src/TeaSharp/Internal/TeaSceneCompiler.cs:52`
- `benchmarks/TeaSharp.Benchmarks/LogTailStreamBenchmarks.cs:55`
- `benchmarks/TeaSharp.Benchmarks/OverlayStressBenchmarks.cs:112`
- `benchmarks/TeaSharp.Benchmarks/ResizeStormBenchmarks.cs:90`
- `benchmarks/TeaSharp.Benchmarks/ViewportRenderBenchmarks.cs:54`

Why it matters:
- every frame can allocate a full terminal-sized string plus intermediate builder state.

### F2 (High): grapheme/ANSI write path performs repeated transient string concatenations
Evidence:
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:78`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:83`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:106`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:127`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:154`
- `src/TeaSharp/Components/Canvas/Internal/CanvasAnsiScanner.cs:26`

Why it matters:
- heavy styled output and ANSI-rich rendering multiply allocation churn on hot paths.

### F3 (High): control rendering loops still use repeated normalize/split/format patterns
Evidence:
- `src/TeaSharp/Controls/DataGrid.Rendering.cs:351`
- `src/TeaSharp/Controls/DataGrid.Rendering.cs:352`
- `src/TeaSharp/Controls/TextArea.cs:199`
- `src/TeaSharp/Controls/TextArea.cs:227`
- `src/TeaSharp/Controls/MiniLog.cs:99`
- `src/TeaSharp/Controls/MiniLog.cs:101`
- `src/TeaSharp/Controls/FuzzyFinder.Rendering.cs:84`
- `src/TeaSharp/Controls/FuzzyFinder.Rendering.cs:87`

Why it matters:
- these paths run per render/update in benchmarked scenarios (`OverlayStress`, `ResizeStorm`, log-heavy flows).

### F4 (Medium-High): viewport wrap pipeline allocates per-segment substrings
Evidence:
- `src/TeaSharp/Widgets/Internal/ViewportVisualLineBuilder.cs:38`
- `src/TeaSharp/Widgets/ViewportModel.cs:167`

Why it matters:
- long wrapped lines create many short-lived allocations; pressure increases with scrolling + wrap.

### F5 (Medium-High): decoder/parser paths still use split/string materialization where span parsing fits
Evidence:
- `src/TeaSharp.Core/Input/Decoding/DecoderCommon.cs:105`
- `src/TeaSharp.Core/Input/Decoding/DecoderCommon.cs:106`
- `src/TeaSharp.Core/Input/Decoding/OscDcsSequenceDecoder.cs:46`
- `src/TeaSharp.Core/Input/Decoding/OscDcsSequenceDecoder.cs:71`
- `src/TeaSharp.Core/Input/Decoding/OscDcsSequenceDecoder.cs:150`
- `src/TeaSharp.Core/Terminal/Capabilities/TerminalCapabilityDetector.cs:150`
- `src/TeaSharp.Core/Terminal/Capabilities/TerminalCapabilityDetector.cs:189`

Why it matters:
- input decode and capability parsing run frequently in interactive loops and startup probes.

### F6 (Medium): runtime channels can be tuned for deterministic throughput/backpressure behavior
Evidence:
- `src/TeaSharp.Core/Application/Internal/TeaRuntimeLoop.cs:34`
- `src/TeaSharp.Core/Application/Internal/TeaRuntimeLoop.cs:35`
- `src/TeaSharp.Core/Application/Internal/TeaResizeMonitor.cs:23`
- `src/TeaSharp.Core/Application/Internal/TeaResizeMonitor.cs:48`

Why it matters:
- unbounded/default channel semantics can allow noisy signal accumulation and less predictable scheduling behavior.

### F7 (Medium): command/effect formatting has avoidable LINQ/string pipeline allocations
Evidence:
- `src/TeaSharp.Core/Commands/Effects.cs:92`
- `src/TeaSharp.Core/Application/Internal/TeaCapabilityProbe.cs:230`
- `src/TeaSharp.Core/Application/Internal/TeaCapabilityProbe.cs:240`
- `src/TeaSharp.Core/Rendering/Internal/SgrStyleState.cs:85`
- `src/TeaSharp.Core/Rendering/Internal/SgrStyleState.cs:100`

Why it matters:
- effect/capability operations happen across startup and runtime orchestration.

### F8 (Low, non-product runtime): examples/tests have string-heavy operations that affect CI + demo smoothness
Evidence:
- `examples/WorkspaceApp/Program.cs:117`
- `examples/WidgetGallery/Program.cs:274`
- `tests/TeaSharp.IntegrationTests/TmuxSmokeIntegrationTests.cs:124`
- `tests/TeaSharp.IntegrationTests/TmuxSmokeIntegrationTests.cs:125`
- `tests/TeaSharp.IntegrationTests/TmuxSmokeIntegrationTests.cs:126`
- repeated `canvas.Render()` usage across tests (expected, but high CI allocation footprint): e.g. `tests/TeaSharp.Tests/PrebuiltWidgetTests.cs:121` and many peers.

Why it matters:
- not release-critical, but impacts dev-loop and CI runtime/cost.

## Prioritized backlog and ownership slices

Priority legend:
- `P0`: immediate perf gate impact
- `P1`: high return, medium complexity
- `P2`: medium return or narrower impact
- `P3`: hygiene/CI improvements

### Slice S0 (P0) - Render pipeline non-materializing path
Owner: Core Rendering lane  
Scope: `src/TeaSharp/Components/Canvas/*`, `src/TeaSharp/Internal/TeaSceneCompiler.cs`, benchmark call sites  
Tasks:
1. Add non-materializing API path (`IBufferWriter<char>` or equivalent) while preserving existing `Render()` behavior.
2. Reuse pooled buffers for frame assembly to avoid per-frame large string churn.
3. Keep ANSI/styling output byte-for-byte compatible in existing tests.
Measurement:
- `scripts/run_benchmarks_v1.sh shortlist-materialize`
- `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`
Success criteria:
- `Viewport` materialize alloc: >= 10% reduction
- `OverlayStress` materialize alloc: >= 8% reduction
- no > 3% mean regression in paired scenarios.

### Slice S1 (P0) - Grapheme/ANSI accumulation refactor
Owner: Rendering Text lane  
Scope: `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs`, `CanvasAnsiScanner.cs`  
Tasks:
1. Remove repeated concatenation on pending ANSI/zero-width append paths.
2. Adopt staged span-first accumulation and single materialization per committed cell.
3. Preserve truncation + reset semantics.
Measurement:
- `scripts/run_benchmarks_v1.sh scenario "*StyledHeavy*"`
- `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`
Success criteria:
- Styled-heavy materialize alloc: >= 5% reduction
- no snapshot/render parity regression.

### Slice S2 (P1) - Control text normalization fast paths
Owner: Controls lane  
Scope: `DataGrid.Rendering`, `TextArea`, `MiniLog`, `ControlTextLayout`, `MarkdownLineRenderer`, `ViewportLineFormatter`  
Tasks:
1. Add CR/LF detection fast paths before `Replace`/`Split`.
2. Replace per-cell temporary string ops with span-based slicing where feasible.
3. Reuse line buffers in controls with repeated normalization.
Measurement:
- `scripts/run_benchmarks_v1.sh scenario "*OverlayStress*"`
- `scripts/run_benchmarks_v1.sh scenario "*ResizeStorm*"`
- control-focused unit regression tests.
Success criteria:
- >= 3% allocation reduction in at least one of overlay/resize scenarios
- no > 2% mean regression.

### Slice S3 (P1) - Decoder/capability parser spanization
Owner: Input + Terminal lane  
Scope: `DecoderCommon`, `OscDcsSequenceDecoder`, `TerminalCapabilityDetector`, `AnsiColorNormalizer`  
Tasks:
1. Remove `Split`-heavy parsing in decoder hot paths.
2. Parse byte/char spans directly for integer lists and color channels.
3. Minimize intermediate `string` allocations in capability parsing.
Measurement:
- add targeted microbenchmarks under `benchmarks/TeaSharp.Benchmarks` for decoder/capability parse loops.
- `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Decoder*|*Capability*"`
Success criteria:
- >= 15% allocation reduction in added parser microbenchmarks
- neutral functional behavior in decoder tests.

### Slice S4 (P1) - Channel topology tuning and resize coalescing
Owner: Runtime lane  
Scope: `TeaRuntimeLoop`, `TeaResizeMonitor`, `TeaEffectScheduler`  
Tasks:
1. Make channel options explicit for single-reader/multi-writer assumptions.
2. Evaluate bounded size-1 coalescing for resize signal ticks.
3. Validate no starvation or ordering regression under burst conditions.
Measurement:
- existing runtime loop tests + resize-focused integration tests
- `scripts/run_benchmarks_v1.sh scenario "*ResizeStorm*"`
Success criteria:
- no dropped logical resize updates
- same or better mean/alloc in `ResizeStorm`.

### Slice S5 (P2) - Effect/style formatting micro-alloc reductions
Owner: Core Utilities lane  
Scope: `Effects.RequestCapability`, `TeaCapabilityProbe.SendQueriesAsync`, `SgrStyleState.ToEscapeSequence`  
Tasks:
1. Replace LINQ-driven hex payload build with span/stackalloc approach.
2. Reduce temporary list/string-join in SGR serialization.
3. Keep readability and correctness first.
Measurement:
- add microbenchmarks (`*Effects*`, `*Sgr*`) and compare alloc/mean.
Success criteria:
- >= 20% alloc reduction in microbenchmarks without readability collapse.

### Slice S6 (P3) - Example and CI perf hygiene
Owner: Examples + QA tooling lane  
Scope: `examples/*`, `tests/*`, `scripts/run_benchmarks_v1.sh`, perf docs  
Tasks:
1. Avoid per-frame string joins in examples where easy (`WorkspaceApp` activity aggregation).
2. Keep integration-test normalization helpers efficient but maintain clarity.
3. Extend benchmark script/report template for per-slice before/after storage.
Measurement:
- example smoke runs + CI timing deltas on test suites.
Success criteria:
- no behavior changes in examples
- observable CI/runtime improvement or reduced noise.

## Orchestration notes (implementation sequence)

Recommended order:
1. `S0` and `S1` first (largest direct effect on benchmark gate metrics).
2. `S2` in parallel with `S4` (different owners/files, low overlap).
3. `S3` after `S0/S1` baseline refresh (isolated parser microbench lane).
4. `S5`, then `S6` as polish/hygiene.

Parallelization guidance:
- Safe parallel lanes:
  - Rendering: `S0/S1`
  - Controls: `S2`
  - Runtime channels: `S4`
  - Decoder/terminal parser: `S3`
- Avoid simultaneous edits to:
  - `Canvas` + `TeaSceneCompiler` by multiple lanes
  - shared perf docs/checklist files without pre-assigned owner.

Required evidence per slice PR:
- before/after benchmark command lines
- exact mean + alloc deltas
- gate statement (`pass/fail/not measured`) aligned to `docs/perf-plan-v1.md`
- explicit rollback condition.

## Implementation status in this lane

- Product code changes: none (as requested).
- This document is analysis + execution orchestration only.
