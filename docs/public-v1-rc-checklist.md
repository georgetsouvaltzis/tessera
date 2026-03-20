# TeaSharp Public V1 RC Checklist

Use this checklist to run Public V1 release-candidate validation without ambiguity.
Do not mark a checkbox complete unless command output/evidence is attached.

## RC Metadata (placeholder)

- [ ] RC tag/branch: `<fill>`
- [x] Date (UTC): `2026-03-20`
- [x] Commit SHA: `d30df85076ee`
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
- [ ] Canonical examples smoke run validated
  - commands:
    - `dotnet run --project examples/HelloWorld --no-build`
    - `dotnet run --project examples/CounterForm --no-build`
    - `dotnet run --project examples/WorkspaceApp --no-build`
  - evidence: `Not executed in this run (interactive/manual lane).`

## Benchmark Evidence and Regression Budgets

- [x] Benchmark inventory listed
  - command: `dotnet run --project benchmarks/TeaSharp.Benchmarks --no-build -- --list flat`
  - evidence: `14 benchmark entries listed, including both render and render-only variants for gate scenarios.`
- [x] Gate scenarios measured in both modes (`render-only`, `materialize`)
  - command template: `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - command template: `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - evidence: `Captured in [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md): dual-mode table for Startup, LogTail, LargeTable, OverlayStress, ResizeStorm, StyledHeavy (Date: 2026-03-20).`
- [ ] Regression budget check completed vs previous baseline
  - baseline doc: `docs/perf-baseline-v1.md`
  - pass/fail + deltas: `Not explicitly computed in this run.`

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
- `dotnet run --project benchmarks/TeaSharp.Benchmarks --no-build -- --list flat` -> listed gate scenarios in both `render` and `render-only` forms.
- `dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "PublicApiXmlDocs_"` -> `Passed: 4, Failed: 0, Skipped: 0.`
- dual-mode benchmark snapshot reference: [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md) (`Date: 2026-03-20`, `inProcess`, six gate scenarios).
- overlay optimization spotlight reference: [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md) (`Overlay Optimization Spotlight`, added in commit `d30df85` after perf commits `9d2bc23`, `3487356`).
