# Perf Baseline V1 Smoke

Date: 2026-03-19

Environment:
- mode: `inProcess` BenchmarkDotNet toolchain
- host: `Darwin arm64`
- terminal: `xterm-ghostty`

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
