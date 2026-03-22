# TeaSharp Public V1 RC Checklist

Use this checklist to run Public V1 release-candidate validation without ambiguity.
Do not mark a checkbox complete unless command output/evidence is attached.

## RC Metadata (placeholder)

- [ ] RC tag/branch: `<fill>`
- [x] Date (UTC): `2026-03-22`
- [x] Commit SHA: `51d46a39c1a1` (pre-doc-sync head)
- [ ] Owner: `<fill>`

## Build, Test, Examples

- [x] Solution build passed
  - command: `dotnet build TeaSharp.slnx --no-restore --nologo`
  - evidence: `Build succeeded. 0 Warning(s), 0 Error(s).`
- [x] Full test suite passed
  - command: `dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal`
  - evidence: `Passed: TeaSharp.Tests 871/871, TeaSharp.IntegrationTests 10/10.`
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
  - evidence: `20 benchmark entries listed, including render/materialize gate scenarios, SLO latency gates, and input decoding benchmarks.`
- [x] Gate scenarios measured in both modes (`render-only`, `materialize`)
  - command template: `scripts/run_benchmarks_v1.sh shortlist-render-only`
  - command template: `scripts/run_benchmarks_v1.sh shortlist-materialize`
  - evidence: `Captured in [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md): dual-mode table for Startup, LogTail, LargeTable, OverlayStress, ResizeStorm, StyledHeavy (Date: 2026-03-20).`
- [ ] Regression budget check completed vs previous baseline
  - baseline doc: `docs/perf-baseline-v1.md` (`before: d30df85076ee`, `after: 842aaaf8ba64`)
  - latest accepted evidence: `PASS` (`worst time regression: +1.51%`, `worst alloc regression: +0.00%`, all six gate scenarios pass in both modes; input latency p95 budget remains not measured in that shortlist lane)
  - pending for RC closure: rerun and attach explicit budget verdict for final RC candidate SHA.

## Docs and Public API Commenting

- [x] Public API docs gate reviewed for changed symbols
  - references: `docs/public-api-inventory.md`, `docs/spec.md`, `docs/theme-system-v1.md`
  - evidence: `Reviewed in current docs sync lanes; inventories/spec/theme mappings aligned to shipped API surface.`
- [x] XML docs expectations validated for changed public API
  - command: `dotnet test tests/TeaSharp.Tests --no-restore --nologo --filter "PublicApiXmlDocs_"`
  - evidence: `Passed: 4/4, Failed: 0, Skipped: 0.`
- [x] Docs consistency pass completed for shipped behavior
  - evidence: `Current docs sync commits and references are coherent with v1 source-of-truth docs, including external-consumer friction closure updates for Table/ListView/ThemeScope (`51d46a3`, `a9f774f`, `8ff286d`).`
- [x] Terminal compatibility matrix evidence attached
  - reference: [terminal-compatibility-evidence-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-compatibility-evidence-v1.md)
  - evidence: `iTerm2/WezTerm/Kitty/Windows Terminal binaries not installed on this host; verification uses deterministic tests + official specs; Ghostty has host evidence.`

## Scope Guard (V1 vs V1.1)

- [ ] V1 scope verified complete (API simplification, theming, widget tranche, perf/docs gates)
  - reference: `docs/v1-master-plan.md`
  - evidence: `M4 remains in progress and M5 is pending manual signoff in the master plan; keep this gate open until closure evidence is attached.`
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

- `dotnet build TeaSharp.slnx --no-restore --nologo -v minimal` -> `Build succeeded. 0 Warning(s), 0 Error(s).`
- `dotnet test TeaSharp.slnx --no-restore --nologo --tl:off -v minimal` -> `TeaSharp.Tests: 871 passed; TeaSharp.IntegrationTests: 10 passed.`
- `dotnet build TeaSharp.Examples.slnx --no-restore --nologo -v minimal` -> `Build succeeded. 0 Warning(s), 0 Error(s).`
- `scripts/smoke_examples_v1.sh 4` -> `PASS HelloWorld`, `PASS CounterForm`, `PASS WorkspaceApp`, `SUMMARY pass=3 fail=0` (bounded startup probe; processes intentionally terminated after 4s).
- `dotnet run --project benchmarks/TeaSharp.Benchmarks/TeaSharp.Benchmarks.csproj --no-build -- --list flat` -> listed 20 benchmark entries (`render`, `render-only`, SLO, and input decoding suites).
- `scripts/perf_gate_v1.sh dry-run` -> intermittent wrapper stall reproduced: command prints the benchmark `dotnet build ...TeaSharp.Benchmarks.csproj...` line and may not progress.
- `dotnet benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/TeaSharp.Benchmarks.dll --perf-gate --baseline docs/perf-baselines/v1-slo-gate-baseline.json --output docs/perf-baselines/latest-slo-gate-result.json --dry-run` -> `Status: dry-run`, all SLO scenarios `Pass: true`; artifact updated at `2026-03-22T15:10:24.457564+00:00`.
- `dotnet benchmarks/TeaSharp.Benchmarks/bin/Release/net10.0/TeaSharp.Benchmarks.dll --inProcess --filter "*InputDecodingBenchmarks*"` -> osc clipboard `44.65 ns / 208 B`, osc color `60.15 ns / 136 B`, dcs capability `49.52 ns / 272 B`.
- regression budget decision reference (latest accepted baseline comparison): [perf-baseline-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-baseline-v1.md) remains `PASS` (`worst time regression: +1.51%`, `worst alloc regression: +0.00%`); final RC-candidate rerun still pending manual signoff.
