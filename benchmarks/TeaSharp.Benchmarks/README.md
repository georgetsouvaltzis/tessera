# TeaSharp.Benchmarks

BenchmarkDotNet harness used by Public V1 perf gates.

## Modes

Two BenchmarkDotNet mode families are tracked:
- render-only: control render path only; excludes final `canvas.Render()` materialization
- render+materialize: includes `canvas.Render()` to measure full frame/output cost

Why both:
- render-only isolates renderer/layout regressions
- render+materialize reflects end-to-end user-visible frame and allocation cost

Gating:
- render-only gates renderer/layout regression budget
- render+materialize gates release-facing frame/allocation budgets
- V1 perf gate requires both mode families to stay within budget

## Deterministic Execution Commands

Use Release configuration for comparisons and gates.

```bash
# 1) List all discoverable benchmarks
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --list flat

# 2) Run all scenarios in Release
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*"

# 3) Run a single scenario (example: LargeTable)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*LargeTable*"

# 4) Run render-only mode slice (dual-mode instrumentation)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*RenderOnly*"

# 5) Run render+materialize mode slice (dual-mode instrumentation)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --filter "*Materialize*"
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
