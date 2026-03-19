# Perf Baseline V1 Smoke

Date: 2026-03-19

Environment:
- mode: `inProcess` BenchmarkDotNet toolchain
- host: `Darwin arm64`
- terminal: `xterm-ghostty`
- benchmark mode families: `render-only` + `render+materialize`

Latest measured snapshot (current head):
- command pattern: `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release --no-build -- --inProcess --filter "*<ScenarioClass>*"`

| Scenario | Render-only mean | Render-only alloc | Materialize mean | Materialize alloc |
| --- | --- | --- | --- | --- |
| Startup | `11.45 us` | `29.91 KB` | `12.31 us` | `47.49 KB` |
| LogTail | `7.112 ms` | `80.47 KB` | `6.552 ms` | `106.97 KB` |
| LargeTable | `10.97 us` | `15.67 KB` | `12.67 us` | `46.88 KB` |
| OverlayStress | `368.8 us` | `70.32 KB` | `438.6 us` | `1463.45 KB` |
| ResizeStorm | `256.3 us` | `88.97 KB` | `319.8 us` | `1256.91 KB` |
| StyledHeavy | `50.35 us` | `93.23 KB` | `52.12 us` | `118.48 KB` |

Notes:
- priority-setting warnings on this host (`Permission denied` / `Operation not permitted`) are non-fatal noise
- runs complete and report benchmark summaries in `inProcess` mode
- table captures both mode families for the same scenario set

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
