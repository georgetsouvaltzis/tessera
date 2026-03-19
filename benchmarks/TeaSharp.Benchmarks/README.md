# TeaSharp.Benchmarks

BenchmarkDotNet harness used by Public V1 perf gates.

## Deterministic Execution Commands

Use Release configuration for comparisons and gates.

```bash
# 1) List all discoverable benchmarks
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --list flat

# 2) Run all scenarios in Release
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*"

# 3) Run a single scenario (example: LargeTable)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*LargeTable*"
```

Optional helper:

```bash
scripts/run_benchmarks_v1.sh list
scripts/run_benchmarks_v1.sh all
scripts/run_benchmarks_v1.sh scenario "*Overlay*"
```

## Artifacts Location

BenchmarkDotNet writes reports/artifacts under:

- `benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/BenchmarkDotNet.Artifacts/`

Scenarios use fixed sizes and deterministic seeded data.
