# TeaSharp Public V1 RC Checklist

Use this checklist to run Public V1 release-candidate validation without ambiguity.
Do not mark a checkbox complete unless command output/evidence is attached.

## RC Metadata (placeholder)

- [ ] RC tag/branch: `<fill>`
- [ ] Date (UTC): `<fill>`
- [ ] Commit SHA: `<fill>`
- [ ] Owner: `<fill>`

## Build, Test, Examples

- [ ] Solution build passed
  - command: `dotnet build TeaSharp.slnx --no-restore --nologo`
  - evidence: `<fill>`
- [ ] Full test suite passed
  - command: `dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal`
  - evidence: `<fill>`
- [ ] Canonical examples build passed
  - command: `dotnet build TeaSharp.Examples.slnx --no-restore --nologo`
  - evidence: `<fill>`
- [ ] Canonical examples smoke run validated
  - commands:
    - `dotnet run --project examples/HelloWorld --no-build`
    - `dotnet run --project examples/CounterForm --no-build`
    - `dotnet run --project examples/WorkspaceApp --no-build`
  - evidence: `<fill>`

## Benchmark Evidence and Regression Budgets

- [ ] Benchmark inventory listed
  - command: `dotnet run --project benchmarks/TeaSharp.Benchmarks --no-build -- --list flat`
  - evidence: `<fill>`
- [ ] Gate scenarios measured in both modes (`render-only`, `materialize`)
  - command template: `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - command template: `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - evidence: `<fill>`
- [ ] Regression budget check completed vs previous baseline
  - baseline doc: `docs/perf-baseline-v1.md`
  - pass/fail + deltas: `<fill>`

## Docs and Public API Commenting

- [ ] Public API docs gate reviewed for changed symbols
  - references: `docs/public-api-inventory.md`, `docs/spec.md`, `docs/theme-system-v1.md`
  - evidence: `<fill>`
- [ ] XML docs expectations validated for changed public API
  - command: `dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "PublicApiXmlDocs_"`
  - evidence: `<fill>`
- [ ] Docs consistency pass completed for shipped behavior
  - evidence: `<fill>`

## Scope Guard (V1 vs V1.1)

- [ ] V1 scope verified complete (API simplification, theming, widget tranche, perf/docs gates)
  - reference: `docs/v1-master-plan.md`
  - evidence: `<fill>`
- [ ] V1.1-only features remain out of V1 release scope
  - image rendering remains V1.1 (`kitty`, `iTerm2`, `wezterm`, `ghostty`)
  - evidence: `<fill>`
- [ ] No DI-first onboarding leakage introduced
  - check: default path remains `Tea.RunAsync(...)` / `Tea.CreateBuilder().UseApp<TApp>()`
  - evidence: `<fill>`

## Sign-off (placeholder)

- [ ] Engineering sign-off: `<fill>`
- [ ] Product/release sign-off: `<fill>`
- [ ] RC approved for publish: `<fill>`
