# TeaSharp Public V1 RC Checklist

Use this checklist to run Public V1 release-candidate validation without ambiguity.
Do not mark a checkbox complete unless command output/evidence is attached.

## RC Metadata (placeholder)

- [ ] RC tag/branch: `<fill>`
- [x] Date (UTC): `2026-03-20`
- [x] Commit SHA: `b132c8a1cfa8`
- [ ] Owner: `<fill>`

## Build, Test, Examples

- [x] Solution build passed
  - command: `dotnet build TeaSharp.slnx --no-restore --nologo`
  - evidence: `Build succeeded. 0 Warning(s), 0 Error(s).`
- [x] Full test suite passed
  - command: `dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal`
  - evidence: `Passed: TeaSharp.Tests 523/523, TeaSharp.IntegrationTests 10/10.`
- [x] Canonical examples build passed
  - command: `dotnet build TeaSharp.Examples.slnx --no-restore --nologo`
  - evidence: `Build succeeded. 0 Warning(s), 0 Error(s).`
- [x] Canonical examples smoke run validated
  - commands:
    - `dotnet run --project examples/HelloWorld --no-build`
    - `dotnet run --project examples/CounterForm --no-build`
    - `dotnet run --project examples/WorkspaceApp --no-build`
    - `scripts/smoke_examples_v1.sh 4`
  - evidence: `scripts/smoke_examples_v1.sh 4` -> `PASS HelloWorld startup alive >=4s`, `PASS CounterForm startup alive >=4s`, `PASS WorkspaceApp startup alive >=4s`, `SUMMARY pass=3 fail=0`.

## Benchmark Evidence and Regression Budgets

- [x] Benchmark inventory listed
  - command: `dotnet run --project benchmarks/TeaSharp.Benchmarks --no-build -- --list flat`
  - evidence: `14 benchmark entries listed, including both render and render-only variants for gate scenarios.`
- [x] Gate scenarios measured in both modes (`render-only`, `materialize`)
  - command template: `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - command template: `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - evidence: `Captured in [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md): dual-mode table for Startup, LogTail, LargeTable, OverlayStress, ResizeStorm, StyledHeavy (Date: 2026-03-20).`
- [x] Regression budget check completed vs previous baseline
  - baseline doc: `docs/perf-baseline-v1.md` (`before: d30df85076ee`, `after: 06cc6a8c59e3`)
  - pass/fail + deltas: `PASS` (`worst time regression: +5.79%`, `worst alloc regression: +1.60%`, all six gate scenarios pass in both modes; input latency p95 budget remains not measured in current BenchmarkDotNet shortlist lane).

## Docs and Public API Commenting

- [x] Public API docs gate reviewed for changed symbols
  - references: `docs/public-api-inventory.md`, `docs/spec.md`, `docs/theme-system-v1.md`
  - evidence: `Reviewed in current docs sync lanes; inventories/spec/theme mappings aligned to shipped API surface.`
- [x] XML docs expectations validated for changed public API
  - command: `dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "PublicApiXmlDocs_"`
  - evidence: `Passed: 4/4, Failed: 0, Skipped: 0.`
- [x] Docs consistency pass completed for shipped behavior
  - evidence: `Current docs sync commits and references are coherent with v1 source-of-truth docs.`

## Scope Guard (V1 vs V1.1)

- [x] V1 scope verified complete (API simplification, theming, widget tranche, perf/docs gates)
  - reference: `docs/v1-master-plan.md`
  - evidence: `Current Progress + gate sections reflect completion state with only forward-only parity policy guard remaining.`
- [x] V1.1-only features remain out of V1 release scope
  - image rendering remains V1.1 (`kitty`, `iTerm2`, `wezterm`, `ghostty`)
  - evidence: `v1-master-plan and source-of-truth keep image rendering explicitly in V1.1 scope.`
- [x] No DI-first onboarding leakage introduced
  - check: default path remains `Tea.RunAsync(...)` / `Tea.CreateBuilder().UseApp<TApp>()`
  - evidence: `Docs continue to enforce no-DI default onboarding path.`

## Sign-off (placeholder)

- [ ] Engineering sign-off: `<fill>`
- [ ] Product/release sign-off: `<fill>`
- [ ] RC approved for publish: `<fill>`

## Evidence Snapshot (this run)

- `dotnet build TeaSharp.slnx --no-restore --nologo` -> `Build succeeded. 0 Warning(s), 0 Error(s).`
- `dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal` -> `TeaSharp.Tests: 523 passed; TeaSharp.IntegrationTests: 10 passed.`
- `dotnet build TeaSharp.Examples.slnx --no-restore --nologo` -> `Build succeeded. 0 Warning(s), 0 Error(s).`
- `scripts/smoke_examples_v1.sh 4` -> `PASS HelloWorld`, `PASS CounterForm`, `PASS WorkspaceApp`, `SUMMARY pass=3 fail=0` (bounded startup probe; processes intentionally terminated after 4s).
- `dotnet run --project benchmarks/TeaSharp.Benchmarks --no-build -- --list flat` -> listed gate scenarios in both `render` and `render-only` forms.
- `scripts/run_benchmarks_v1.sh shortlist-render-only` -> Startup `12.60 us / 30.52 KB`, LogTail `4.899 ms / 80.1 KB`, LargeTable `12.76 us / 15.67 KB`, OverlayStress `429.7 us / 61.13 KB`, ResizeStorm `299.5 us / 59.11 KB`, StyledHeavy `55.53 us / 93.23 KB`.
- `scripts/run_benchmarks_v1.sh shortlist-materialize` -> Startup `13.20 us / 48.11 KB`, LogTail `4.955 ms / 106.61 KB`, LargeTable `14.11 us / 46.88 KB`, OverlayStress `495.0 us / 1.42 MB`, ResizeStorm `365.4 us / 1.2 MB`, StyledHeavy `56.17 us / 118.48 KB`.
- regression budget decision vs baseline `d30df85076ee`: `PASS` (worst time regression `+5.79%`; worst alloc regression `+1.60%`; both under plan thresholds `10%` and `15%`).
- `dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "PublicApiXmlDocs_"` -> `Passed: 4, Failed: 0, Skipped: 0.`
- dual-mode benchmark snapshot reference: [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md) (`Date: 2026-03-20`, `inProcess`, six gate scenarios).
- overlay optimization spotlight reference: [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md) (`Overlay Optimization Spotlight`, added in commit `d30df85` after perf commits `9d2bc23`, `3487356`).
