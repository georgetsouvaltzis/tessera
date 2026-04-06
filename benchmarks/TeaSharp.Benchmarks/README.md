# TeaSharp.Benchmarks

BenchmarkDotNet harness used by Public V1 perf gates.

## Modes

Two BenchmarkDotNet mode families are tracked:
- render-only: control render path only; excludes final `canvas.Render()` materialization
- render+materialize: includes `canvas.Render()` to measure full frame/output cost
- direct SLO gate mode: startup/input-latency p95 thresholds compared against a baseline file without invoking BenchmarkDotNet at gate time
- runtime e2e mode: public hosting path, runtime loop, input decode, renderer flush, and terminal output in one deterministic probe

Why both:
- render-only isolates renderer/layout regressions
- render+materialize reflects end-to-end user-visible frame and allocation cost

Gating:
- render-only gates renderer/layout regression budget
- render+materialize gates release-facing frame/allocation budgets
- direct SLO gate validates startup/input-latency thresholds with `2` warmups + `10` measured iterations
- runtime e2e is supplemental confidence evidence and is non-gating for V1
- V1 perf gate requires both mode families plus the SLO gate to stay within budget

## Deterministic Execution Commands

Use Release configuration for comparisons and gates.

```bash
# 1) List all discoverable benchmarks
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --list flat

# 2) Run all scenarios in Release
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*"

# 3) Run a single scenario (example: LargeTable)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*LargeTable*"

# 4) Run render-only mode slice (current suffix pattern)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Only"

# 5) Run render+materialize mode slice (current method names without `Only`)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Frame" --filter "*ScrollLogTail" --filter "*FirstFrameRender" --filter "*OverlayStressFrames" --filter "*ResizeStormFrames"
```

Direct gate path:

```bash
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --perf-gate --baseline docs/perf-baselines/v1-slo-gate-baseline.json --output docs/perf-baselines/latest-slo-gate-result.json
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --perf-gate --baseline docs/perf-baselines/v1-slo-gate-baseline.json --output docs/perf-baselines/latest-slo-gate-result.json --dry-run
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --runtime-e2e --output docs/perf-baselines/latest-runtime-e2e-result.json
```

## Before/After Reporting Workflow

Use the same host/terminal/configuration for both runs.

1. checkout baseline commit and run shortlist for the target mode
2. checkout candidate commit and run the same shortlist mode
3. copy results into the iteration log template from `docs/performance.md`

Mode guidance:
- `shortlist-render-only`: runs the six `*Only` methods (renderer/layout gate signals)
- `shortlist-materialize`: runs the six non-`Only` methods (end-to-end frame/allocation gate signals)
- SLO baseline gate:
  - baseline: `docs/perf-baselines/v1-slo-gate-baseline.json`
  - run: `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --perf-gate --baseline docs/perf-baselines/v1-slo-gate-baseline.json --output docs/perf-baselines/latest-slo-gate-result.json`
  - output: `docs/perf-baselines/latest-slo-gate-result.json`
- runtime e2e probe:
  - run: `dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --runtime-e2e --output docs/perf-baselines/latest-runtime-e2e-result.json`
  - output: `docs/perf-baselines/latest-runtime-e2e-result.json`

## Artifacts Location

BenchmarkDotNet writes reports/artifacts under:

- `benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/BenchmarkDotNet.Artifacts/`

Scenarios use fixed sizes and deterministic seeded data.
