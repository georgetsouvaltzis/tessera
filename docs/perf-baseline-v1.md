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
| Startup | `11.91 us` | `30.04 KB` | `13.26 us` | `47.63 KB` |
| LogTail | `6.277 ms` | `80.47 KB` | `4.978 ms` | `106.97 KB` |
| LargeTable | `12.93 us` | `15.67 KB` | `14.26 us` | `46.88 KB` |
| OverlayStress | `489.5 us` | `115.71 KB` | `567.4 us` | `1.47 MB` |
| ResizeStorm | `306.2 us` | `65.86 KB` | `375.5 us` | `1.2 MB` |
| StyledHeavy | `55.61 us` | `93.23 KB` | `58.12 us` | `118.48 KB` |

Supplemental viewport scenario (optional):

| Scenario | Render-only mean | Render-only alloc | Materialize mean | Materialize alloc |
| --- | --- | --- | --- | --- |
| ViewportRenderBenchmarks | `44.76 us` | `28.5 KB` | `102.66 us` | `1725 KB` |

Notes:
- priority-setting warnings on this host (`Permission denied` / `Operation not permitted`) are non-fatal noise
- values above are from `inProcess` mode and represent single-host snapshots
- table captures both mode families for the same scenario set

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
