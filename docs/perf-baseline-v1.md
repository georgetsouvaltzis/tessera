# Perf Baseline V1 Smoke

Date: 2026-03-20

Environment:
- mode: `inProcess` BenchmarkDotNet toolchain
- host: `Darwin arm64`
- terminal: `xterm-ghostty`
- benchmark mode families: `render-only` + `render+materialize`

Latest measured snapshot (current head):
- command pattern (render-only): `scripts/run_benchmarks_v1.sh shortlist-render-only`
- command pattern (materialize): `scripts/run_benchmarks_v1.sh shortlist-materialize`
- command pattern (viewport supplemental): `scripts/run_benchmarks_v1.sh scenario "*Viewport*"`

| Scenario | Render-only mean | Render-only alloc | Materialize mean | Materialize alloc |
| --- | --- | --- | --- | --- |
| Startup | `12.60 us` | `30.52 KB` | `13.20 us` | `48.11 KB` |
| LogTail | `4.899 ms` | `80.1 KB` | `4.955 ms` | `106.61 KB` |
| LargeTable | `12.76 us` | `15.67 KB` | `14.11 us` | `46.88 KB` |
| OverlayStress | `429.7 us` | `61.13 KB` | `495.0 us` | `1.42 MB` |
| ResizeStorm | `299.5 us` | `59.11 KB` | `365.4 us` | `1.2 MB` |
| StyledHeavy | `55.53 us` | `93.23 KB` | `56.17 us` | `118.48 KB` |

Supplemental viewport scenario (optional):

| Scenario | Render-only mean | Render-only alloc | Materialize mean | Materialize alloc |
| --- | --- | --- | --- | --- |
| ViewportRenderBenchmarks | `51.82 us` | `5 KB` | `99.58 us` | `1701.5 KB` |

Notes:
- priority-setting warnings on this host (`Permission denied` / `Operation not permitted`) are non-fatal noise
- values above are from `inProcess` mode and represent single-host snapshots
- table captures both mode families for the same scenario set

## Regression Budget Check (2026-03-20)

Commits:
- accepted baseline (before): `d30df85076ee`
- candidate measured (after): `06cc6a8c59e3`

Commands:
- `scripts/run_benchmarks_v1.sh list`
- `scripts/run_benchmarks_v1.sh shortlist-render-only`
- `scripts/run_benchmarks_v1.sh shortlist-materialize`

Budget thresholds from `docs/perf-plan-v1.md`:
- time regression budget: `> 10%` -> fail
- allocation regression budget: `> 15%` -> fail

Measured deltas vs accepted baseline:

| Scenario | RO mean delta | RO alloc delta | MAT mean delta | MAT alloc delta | Gate |
| --- | --- | --- | --- | --- | --- |
| Startup | `+5.79%` | `+1.60%` | `-0.45%` | `+1.01%` | `pass` |
| LogTail | `-21.95%` | `-0.46%` | `-0.46%` | `-0.34%` | `pass` |
| LargeTable | `-1.31%` | `+0.00%` | `-1.05%` | `+0.00%` | `pass` |
| OverlayStress | `-12.22%` | `-47.17%` | `-12.76%` | `-3.40%` | `pass` |
| ResizeStorm | `-2.19%` | `-10.25%` | `-2.69%` | `+0.00%` | `pass` |
| StyledHeavy | `-0.14%` | `+0.00%` | `-3.36%` | `+0.00%` | `pass` |

Conclusion:
- measured benchmark budgets: `pass` (worst time regression `+5.79%`, worst allocation regression `+1.60%`)
- input latency p95 budget: `not measured` in current BenchmarkDotNet shortlist lane

## Iteration 3 Spotlight (2026-03-19)

Commit range:
- `6bad663` -> `d9d0d21` -> `6d93e67` -> `fb6b057` -> `d90110e`

Validation:
- latest HEAD build/tests status: green

Spotlight before/after deltas (compared to prior full snapshot in this doc):

| Scenario | Render-only (before -> after) | Render-only alloc | RO delta | Materialize (before -> after) | Materialize alloc | MAT delta |
| --- | --- | --- | --- | --- | --- | --- |
| LogTail | `7.112 ms -> 4.976 ms` | n/a in latest capture | `-30.03%` | `6.552 ms -> 5.005 ms` | `106.97 KB` | `-23.61%` |
| OverlayStress | `368.8 us -> 354.2 us` | `49.5 KB` | `-3.96%` | `438.6 us -> 410.2 us` | n/a in latest capture | `-6.48%` |
| ResizeStorm | `256.3 us -> 253.5 us` | `65.86 KB` | `-1.09%` | `319.8 us -> 305.7 us` | n/a in latest capture | `-4.41%` |

Iteration caveat:
- spotlight measurements came from targeted scenario runs; use a same-pass full sweep for final release gate decisions

## Overlay Optimization Spotlight (2026-03-20)

Commits:
- `9d2bc23`
- `3487356`

Command:
- `scripts/run_benchmarks_v1.sh scenario "*OverlayStress*"`

Targeted before/after evidence:

| Mode | Mean (before -> after) | Mean delta | Alloc (before -> after) | Alloc delta |
| --- | --- | --- | --- | --- |
| Render-only | `489.9 us -> 428.8 us` | `-12.47%` | `115.71 KB -> 61.13 KB` | `-47.17%` |
| Materialize | `578.7 us -> 495.4 us` | `-14.40%` | `1508.84 KB -> 1454.25 KB` | `-3.62%` |

Note:
- this is targeted scenario evidence for `OverlayStress`; it does not replace full-suite gate runs

## Iteration Log (Template)

Metadata:
- Date (UTC): `YYYY-MM-DD`
- Before commit: `<sha>`
- After commit: `<sha>`
- Host/terminal note: `<machine + terminal profile>`

Per-scenario table (one row per scenario, both modes captured in-row):

| Scenario | Render-only (before -> after) | Render-only alloc (before -> after) | Materialize (before -> after) | Materialize alloc (before -> after) | RO mean delta % | MAT mean delta % | Gate |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Startup | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| LogTail | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| LargeTable | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| OverlayStress | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| ResizeStorm | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |
| StyledHeavy | __ us -> __ us | __ KB -> __ KB | __ us -> __ us | __ KB -> __ KB | __% | __% | pass/fail |

Final note:
- Result summary + mitigation note if any gate is `fail`
