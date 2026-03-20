# Perf String/Span Audit (Second Opinion)

## Scope
- Skills used: `dotnet_strings_and_spans_best_practices`, `dotnet_optimization_techniques`.
- Audit target: string-heavy render loops + parser-like flows under `src/TeaSharp`.
- Lane rule: analysis only, no behavior changes.

## Lead-Finding Validation
- Lead overlay cache work is valid and high impact.
- Local benchmark check (`scripts/run_benchmarks_v1.sh scenario "*OverlayStress*"`):
  - Overlay materialize: `577.2 us -> 500.8 us` (~`-13.2%`), alloc `1508.84 KB -> 1454.25 KB`.
  - Overlay render-only: `504.3 us -> 432.8 us` (~`-14.2%`), alloc `115.71 KB -> 61.13 KB` (~`-47.2%`).
- Challenge point: biggest remaining churn moved into shared canvas/styled border and text normalization paths.

## Top 5 Recommendations
1. **Cache styled border glyph strings inside `Canvas.DrawBox` hot path**
   - Evidence: `src/TeaSharp/Components/Canvas/Canvas.cs:398-413`, `:447-455`.
   - Issue: per-cell `value.ToString()` + `TeaStyle.Render(...)` in border loops.
   - Recommendation: compute 6 styled glyph strings once per call (or per style+border tuple), reuse in loops.
   - Risk: low. Output semantics unchanged if glyph mapping stays identical.

2. **Eliminate repeated `Replace+Split` normalization in `TextArea` render/measure path**
   - Evidence: `src/TeaSharp/Controls/TextArea.cs:199`, `:227`.
   - Issue: alloc-heavy normalization called repeatedly during layout/render.
   - Recommendation: keep a dirty cached normalized-lines snapshot updated only when input value changes.
   - Risk: medium. Must keep cursor/viewport sync correctness.

3. **Add non-alloc line enumeration API to replace `ControlTextLayout.SplitLines` in hot controls**
   - Evidence: `src/TeaSharp/Controls/Internal/ControlTextLayout.cs:10-15`.
   - Issue: global helper always allocates new strings array.
   - Recommendation: span-based line iterator (`ReadOnlySpan<char>` slices / callback enumerator), migrate hottest controls first.
   - Risk: medium-high. Broad call-site touch if done globally.

4. **Reduce ANSI parser allocations in grapheme-aware write path**
   - Evidence: `src/TeaSharp/Components/Canvas/Internal/CanvasAnsiScanner.cs:5-27`, `src/TeaSharp/Components/Canvas/Internal/CanvasGraphemeBuffer.cs:70-84`, `:127`, `:163-169`.
   - Issue: `Substring` + repeated string concatenation per escape/element in parser loop.
   - Recommendation: scanner returns consumed length only; compose fragments via pooled buffer (`ArrayPool<char>`/`ValueStringBuilder`) and append once.
   - Risk: medium-high. ANSI/reset correctness regression risk; needs strong regression coverage.

5. **Cache `MenuBar` formatted labels/widths for render + hit-test**
   - Evidence: `src/TeaSharp/Controls/MenuBar.cs:295-300`, `:323-335`, `:337-344`.
   - Issue: repeated `FormatLabel` concat and width measurement in render and pointer hit-testing.
   - Recommendation: maintain per-item cached label variants + display widths; invalidate on items/glyph/selection/hover changes.
   - Risk: medium. Cache invalidation bugs possible, but pattern already proven in overlay controls.

## Notes on ArrayPool/Channel Reuse
- `ArrayPool`: good fit for ANSI parsing/temporary composition buffers (recommendation #4).
- `Channel` reuse: no clear high-frequency channel-based pipeline found in audited paths; no immediate recommendation.

## Suggested Measurement Plan
- Benchmark targets: `OverlayStress`, `ResizeStorm`, `StyledHeavy`, plus control-focused microbenchmarks for `TextArea` and `MenuBar`.
- Success gates:
  - Mean latency improvement >= 5% on target scenario.
  - Managed allocation reduction >= 15% on affected benchmark.
  - No render snapshot or glyph-style regression.
