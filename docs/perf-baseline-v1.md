# Perf Baseline V1 Smoke

Date: 2026-03-19

Environment:
- mode: `inProcess` BenchmarkDotNet toolchain
- host: `Darwin arm64`
- terminal: `xterm-ghostty`
- benchmark mode family: `render+materialize` (current benchmark methods call `canvas.Render()`)

Commands and measured outputs:
1. `dotnet run --project benchmarks/TeaSharp.Benchmarks -c Release --no-build -- --inProcess --filter "*Startup*"`
   - Mean: `15.67 us`
   - Allocated: `50.17 KB`
2. `dotnet run --project benchmarks/TeaSharp.Benchmarks -c Release --no-build -- --inProcess --filter "*LargeTable*"`
   - Mean: `23.61 us`
   - Allocated: `78.38 KB`
3. `dotnet run --project benchmarks/TeaSharp.Benchmarks -c Release --no-build -- --inProcess --filter "*StyledHeavy*"`
   - Mean: `68.31 us`
   - Allocated: `311.02 KB`

Notes:
- priority-setting warnings on this host (`Permission denied` / `Operation not permitted`) are non-fatal noise
- runs complete and report benchmark summaries in `inProcess` mode
- this baseline is for `render+materialize`; render-only baselines should be tracked separately once captured

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
