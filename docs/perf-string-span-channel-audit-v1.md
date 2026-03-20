# TeaSharp Perf String/Span/Channel Audit (V1)

Date (UTC): 2026-03-20  
Owner lane: Performance

Skills used (requested order):
1. `dotnet_optimization_techniques`
2. `dotnet_strings_and_spans_best_practices`

Repo-profile checks (version safety):
- SDK: `10.0.103` (`global.json`)
- primary TFM for audited projects: `net10.0`
- nullable: `enable`
- implicit usings: `enable`
- implication: `Span<T>/ReadOnlySpan<T>/Memory<T>/ArrayPool<T>/string.Create` are safe recommendations.

Scope:
- `src/TeaSharp`
- `src/TeaSharp.Core`
- `benchmarks/TeaSharp.Benchmarks`

Evidence sources:
- static code inspection with line references below
- benchmark evidence already captured in this lane:
  - `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`
  - key alloc signals (materialize mode): `OverlayStress ~1.42 MB`, `ResizeStorm ~1.2 MB`, `Viewport ~1701.5 KB`.

## Severity-ranked findings

### F1 (High): frame materialization allocates full-frame strings every render
Impact:
- Dominant allocator in materialize benchmarks; directly tied to frame size and FPS.

Evidence:
- `benchmarks/TeaSharp.Benchmarks/LogTailStreamBenchmarks.cs:55`
- `benchmarks/TeaSharp.Benchmarks/OverlayStressBenchmarks.cs:111`
- `benchmarks/TeaSharp.Benchmarks/ResizeStormBenchmarks.cs:89`
- `benchmarks/TeaSharp.Benchmarks/ViewportRenderBenchmarks.cs:53`
- `src/TeaSharp/Components/Canvas/Canvas.cs:424`
- `src/TeaSharp/Components/Canvas/Canvas.cs:433`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:173`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:175`

Recommendation:
- Introduce a non-materializing render path for hot loops: `RenderTo(IBufferWriter<char>)` (or equivalent) and keep `Render()` as compatibility wrapper.
- Reuse backing buffers (`ArrayPool<char>` or owned reusable buffer per canvas) to avoid per-frame `StringBuilder` + final `string` churn.

Risk:
- Medium (core render path/API seam touch), but measurable and high return.

### F2 (High): ANSI/grapheme write path concatenates strings in tight loops
Impact:
- Repeated `string` concatenation while scanning text/ANSI sequences can amplify allocations under styled content.

Evidence:
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:78`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:83`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:106`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:111`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:127`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:154`
- `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:168`

Recommendation:
- Replace transient concatenation with staged accumulation (`ValueStringBuilder`-style or pooled `StringBuilder`) and flush once per written cell.
- Keep ANSI parsing zero-copy where possible (track offsets/ranges, materialize only at final write boundary).

Risk:
- Medium (text correctness + ANSI reset behavior must stay exact).

### F3 (Medium-High): DataGrid cell normalization does repeated replace/pad per visible cell
Impact:
- Per-cell `Replace`/`PadRight`/slice work occurs inside row/column render loops; shows up in overlay/resize scenarios that render `DataGrid` every frame.

Evidence:
- `benchmarks/TeaSharp.Benchmarks/OverlayStressBenchmarks.cs:95`
- `benchmarks/TeaSharp.Benchmarks/ResizeStormBenchmarks.cs:84`
- `src/TeaSharp/Controls/DataGrid.Rendering.cs:241`
- `src/TeaSharp/Controls/DataGrid.Rendering.cs:351`
- `src/TeaSharp/Controls/DataGrid.Rendering.cs:394`

Recommendation:
- Fast path in `PadToWidth`: scan first for CR/LF; if absent, skip `Replace`.
- Consider span-based normalization/truncation via `string.Create` to avoid intermediate strings when style path is active.

Risk:
- Low/medium (localized change, good candidate for first low-risk patch).

### F4 (Medium): viewport wrap mode allocates substrings per visual segment
Impact:
- `Substring` per wrapped segment can spike allocations for long lines/log streams with wrap enabled.

Evidence:
- `src/TeaSharp/Widgets/Internal/ViewportVisualLineBuilder.cs:35`
- `src/TeaSharp/Widgets/Internal/ViewportVisualLineBuilder.cs:38`
- `src/TeaSharp/Widgets/ViewportModel.cs:167`

Recommendation:
- Store wrapped slices as ranges (`start,length`) or `ReadOnlyMemory<char>` and defer materialization until final render.

Risk:
- Medium (data model adjustment for viewport visual cache).

### F5 (Medium): duplicated normalize/split patterns create avoidable string arrays
Impact:
- Multiple normalize helpers allocate replacement strings + split arrays; repetitive on content updates and input paths.

Evidence:
- `src/TeaSharp.Core/Rendering/Internal/RenderFrameContent.cs:12`
- `src/TeaSharp.Core/Rendering/Internal/RenderFrameContent.cs:13`
- `src/TeaSharp/Widgets/Internal/ViewportLineFormatter.cs:125`
- `src/TeaSharp/Widgets/Internal/ViewportLineFormatter.cs:128`
- `src/TeaSharp/Controls/Internal/ControlTextLayout.cs:12`
- `src/TeaSharp/Controls/Internal/ControlTextLayout.cs:15`

Recommendation:
- Introduce shared line-splitting helper over `ReadOnlySpan<char>` that emits slices/ranges first; materialize only when required by public API.

Risk:
- Medium (cross-cutting utility, many call sites).

### F6 (Medium): channel configuration can be tightened for runtime hot loops
Impact:
- Default unbounded channel options leave throughput/continuation behavior untuned; resize signal channel can accumulate noisy writes.

Evidence:
- `src/TeaSharp.Core/Application/Internal/TeaRuntimeLoop.cs:34`
- `src/TeaSharp.Core/Application/Internal/TeaRuntimeLoop.cs:35`
- `src/TeaSharp.Core/Application/Internal/TeaResizeMonitor.cs:23`
- `src/TeaSharp.Core/Application/Internal/TeaResizeMonitor.cs:48`

Recommendation:
- Explicit channel options:
  - runtime channels: set `SingleReader = true`; set writer cardinality explicitly; set `AllowSynchronousContinuations = false` for predictable scheduling.
  - resize signal channel: use bounded capacity `1` with coalescing semantics to prevent unbounded signal backlog.

Risk:
- Medium (ordering/backpressure semantics must be verified with integration tests).

## Existing good patterns (do not regress)

- `src/TeaSharp/Widgets/Internal/ViewportRenderer.cs:5` uses thread-static list reuse.
- `src/TeaSharp/Widgets/Internal/ViewportLineFormatter.cs:83` and `src/TeaSharp/Widgets/Internal/ViewportLineFormatter.cs:104` already use `string.Create`.
- `src/TeaSharp.Core/Input/TerminalReader.cs:147` uses an internal reusable byte buffer with span-based append/consume.

## Measurement plan (evidence-first)

1. Baseline capture
- Commands:
  - `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`
- Record: mean + allocated bytes for Startup/LogTail/LargeTable/OverlayStress/ResizeStorm/StyledHeavy/Viewport.

2. Patch validation order (smallest risk first)
- Step A: F3 (`DataGrid.Rendering` fast path)
  - Measure with `scripts/run_benchmarks_v1.sh scenario "*OverlayStress*"` and `scripts/run_benchmarks_v1.sh scenario "*ResizeStorm*"`
  - Success threshold: allocation improvement >= 3% in at least one scenario, no >2% time regression.
- Step B: F2 (`CanvasGraphemeBuffer` concat reduction)
  - Measure with `scripts/run_benchmarks_v1.sh scenario "*StyledHeavy*"` and `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`
  - Success threshold: allocation improvement >= 5%, no correctness drift in ANSI/reset rendering.
- Step C: F6 (channel tuning/coalescing)
  - Measure with end-to-end integration stress and `ResizeStorm` scenario.
  - Success threshold: no dropped logical resize outcomes, equal/better frame metrics.

3. Regression gates
- Keep current perf gate criteria in `docs/perf-plan-v1.md`.
- Rollback criteria per patch:
  - any correctness regression in rendering/input behavior
  - >10% time regression or >15% allocation regression in gate scenarios.

## Optional patch status for this audit

- No code patch applied in this slice.
- Reason: this report is evidence-gathering + ranked plan; next slice should take F3 first (localized, low-risk, measurable) and attach benchmark deltas.
