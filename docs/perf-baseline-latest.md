# Perf Baseline Latest (V1)

Date: 2026-03-21

Environment:
- OS: `macOS 26.1 (Darwin 25.1.0) arm64`
- SDK: `.NET SDK 10.0.103`
- Runtime: `.NET 10.0.3`
- BenchmarkDotNet: `0.14.0` (`InProcessEmitToolchain`)

Commands executed:
- `scripts/run_benchmarks_v1.sh list`
- `scripts/run_benchmarks_v1.sh all`

## Current Snapshot (absolute, .NET 10)

| Scenario | Render-only mean | Render-only alloc | Materialize mean | Materialize alloc |
| --- | --- | --- | --- | --- |
| Startup | `10.07 us` | `29.02 KB` | `10.84 us` | `46.61 KB` |
| LogTail | `5.338 ms` | `80.11 KB` | `5.348 ms` | `106.62 KB` |
| LargeTable | `11.49 us` | `15.67 KB` | `12.90 us` | `46.88 KB` |
| OverlayStress | `369.8 us` | `51 KB` | `425.3 us` | `1444.13 KB` |
| ResizeStorm | `268.3 us` | `59.11 KB` | `320.5 us` | `1227.05 KB` |
| StyledHeavy | `50.06 us` | `93.23 KB` | `50.99 us` | `118.48 KB` |

Supplemental viewport:

| Scenario | Render-only mean | Render-only alloc | Materialize mean | Materialize alloc |
| --- | --- | --- | --- | --- |
| ViewportRender | `49.25 us` | `5 KB` | `101.78 us` | `1701.5 KB` |

## Before/After Reference And Budget Verdict

Reference source for `before` values:
- `docs/perf-baseline-v1.md` snapshot dated `2026-03-20` (git artifact reference available in history)

Regression budget policy (`docs/perf-plan-v1.md`):
- time regression fail if `> +10%`
- allocation regression fail if `> +15%`

| Scenario | RO mean delta | RO alloc delta | MAT mean delta | MAT alloc delta | Gate |
| --- | --- | --- | --- | --- | --- |
| Startup | `-5.09%` | `+0.00%` | `-7.59%` | `+0.00%` | `pass` |
| LogTail | `+6.50%` | `+0.01%` | `+5.84%` | `+0.01%` | `pass` |
| LargeTable | `-1.88%` | `+0.00%` | `-4.23%` | `+0.00%` | `pass` |
| OverlayStress | `-1.60%` | `+0.00%` | `-3.25%` | `+0.02%` | `pass` |
| ResizeStorm | `-4.72%` | `+0.00%` | `-5.04%` | `-0.14%` | `pass` |
| StyledHeavy | `+2.31%` | `+0.00%` | `+1.51%` | `+0.00%` | `pass` |

Viewport supplemental (not a release gate row, tracked for trend):
- RO mean delta: `-10.70%`
- MAT mean delta: `-3.80%`
- allocations: unchanged in rounded snapshot values

Conclusion:
- Regression budget verdict vs last accepted snapshot: `PASS`
- Worst time regression: `+6.50%` (`LogTail` render-only), within `+10%` budget
- Worst allocation regression: `+0.02%` (normalized `MB -> KB` conversion in materialize rows), within `+15%` budget

## Findings For Recent String/Core Optimizations

- Allocation behavior remains stable; no scenario exceeded `+0.02%` allocation delta.
- Throughput is improved in 4/6 gate scenarios for both mode families (`Startup`, `LargeTable`, `OverlayStress`, `ResizeStorm`).
- Mild throughput regressions appear in `LogTail` and `StyledHeavy`, but both remain below the release-gate threshold.

## Next Reproducible Baseline Method

If a runnable historical baseline is required (instead of docs artifact comparison), use same-host A/B replay:

1. Checkout accepted baseline commit and run:
   - `scripts/run_benchmarks_v1.sh all`
2. Checkout candidate commit and rerun:
   - `scripts/run_benchmarks_v1.sh all`
3. Compare generated CSV outputs under `BenchmarkDotNet.Artifacts/results/*.csv` with unit-normalized (`ms/us`, `MB/KB`) deltas.
