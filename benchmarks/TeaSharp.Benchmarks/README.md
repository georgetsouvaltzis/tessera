# TeaSharp.Benchmarks

BenchmarkDotNet harness used by Public V1 perf gates.

## Modes

Two BenchmarkDotNet mode families are tracked:
- render-only: control render path only; excludes final `canvas.Render()` materialization
- render+materialize: includes `canvas.Render()` to measure full frame/output cost
- SLO gate mode: startup/input-latency p95 thresholds compared against a baseline file

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
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*"

# 3) Run a single scenario (example: LargeTable)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*LargeTable*"

# 4) Run render-only mode slice (current suffix pattern)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Only"

# 5) Run render+materialize mode slice (current method names without `Only`)
dotnet run --project benchmarks/TeaSharp.Benchmarks --configuration Release -- --inProcess --filter "*Frame" --filter "*ScrollLogTail" --filter "*FirstFrameRender" --filter "*OverlayStressFrames" --filter "*ResizeStormFrames"
```

Optional helper:

```bash
scripts/run_benchmarks_v1.sh list
scripts/run_benchmarks_v1.sh all
scripts/run_benchmarks_v1.sh scenario "*Overlay*"
scripts/run_benchmarks_v1.sh shortlist
scripts/run_benchmarks_v1.sh shortlist-render-only
scripts/run_benchmarks_v1.sh shortlist-materialize
scripts/run_benchmarks_v1.sh iteration-template
scripts/perf_gate_v1.sh run
scripts/perf_gate_v1.sh dry-run
```

## Before/After Reporting Workflow

Use the same host/terminal/configuration for both runs.

1. checkout baseline commit and run shortlist for the target mode
2. checkout candidate commit and run the same shortlist mode
3. copy results into the iteration log template from `docs/perf-baseline-v1.md`

Mode guidance:
- `shortlist-render-only`: runs the six `*Only` methods (renderer/layout gate signals)
- `shortlist-materialize`: runs the six non-`Only` methods (end-to-end frame/allocation gate signals)
- helper execution modes (`all|scenario|shortlist*`) use `--inProcess` by default
- helper uses lazy build; it builds only when benchmark output is missing
- SLO baseline gate:
  - baseline: `docs/perf-baselines/v1-slo-gate-baseline.json`
  - run: `scripts/perf_gate_v1.sh run`
  - output: `docs/perf-baselines/latest-slo-gate-result.json`

## Artifacts Location

BenchmarkDotNet writes reports/artifacts under:

- `benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/BenchmarkDotNet.Artifacts/`

Scenarios use fixed sizes and deterministic seeded data.
