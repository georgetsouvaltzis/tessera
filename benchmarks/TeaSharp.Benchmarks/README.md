# TeaSharp.Benchmarks

Minimal BenchmarkDotNet harness for V1 WS-D perf gates.

Scenarios included:
- startup-ish first-frame render baseline
- large table/data render workload
- styled-heavy output render workload

Run commands:

```bash
dotnet build benchmarks/TeaSharp.Benchmarks/TeaSharp.Benchmarks.csproj --nologo -v minimal
dotnet run --project benchmarks/TeaSharp.Benchmarks -- --list flat
dotnet run --project benchmarks/TeaSharp.Benchmarks -- --filter "*"
```

Notes:
- scenarios use fixed sizes and deterministic seeded data.
- use Release mode for gate/profiling comparisons.
