# Tessera Public Alpha Release Checklist

This document tracks the remaining work required before Tessera can be treated as a public alpha release on GitHub.
Implemented features belong in the product docs, code, and tests.
This file tracks only the release-closing work.

## Current State

Implemented in code already:

- terminal compatibility model
- widget expansion tranche
- visual polish pass
- API cleanup and naming convergence
- benchmark harness and perf gate foundation
- Docusaurus public docs site scaffold

Tessera is close to public alpha, but it is not release-closed until the repo contract, verification evidence, and signoff are all tied to the same candidate SHA.

## Remaining Alpha Gates

### 1. Choose The Candidate

Still needed:

- final candidate branch or tag
- final candidate commit SHA
- release owner

Without those, none of the remaining evidence is final.

### 2. Reconcile Public Repo Contract

Current public example contract:

- onboarding ladder: `examples/HelloWorld/HelloWorld.csproj`
- onboarding ladder: `examples/CounterForm/CounterForm.csproj`
- onboarding ladder: `examples/WorkspaceApp/WorkspaceApp.csproj`

- flagship examples: `examples/DataWorkbench/DataWorkbench.csproj`
- flagship examples: `examples/OpsWatch/OpsWatch.csproj`
- flagship examples: `examples/GitConsole/GitConsole.csproj`
- supporting domain demos remain documented but are not part of the primary smoke gate

README, onboarding docs, API docs, the examples solution, and tests must agree on that contract and must not reference removed example paths or removed solution files.

### 3. Rerun Verification On The Exact Candidate SHA

Required commands on the chosen candidate:

- `dotnet build Tessera.slnx`
- `dotnet build examples/Tessera.Examples.slnx`
- `dotnet test Tessera.slnx`
- `dotnet run --project examples/DataWorkbench/DataWorkbench.csproj --no-build`
- `dotnet run --project examples/OpsWatch/OpsWatch.csproj --no-build`
- `dotnet run --project examples/GitConsole/GitConsole.csproj --no-build`

Goal:

- one final verified evidence set, all from the exact public alpha candidate SHA

### 4. Final Performance Approval

Required:

- rerun the perf gate on the exact candidate SHA
- attach explicit regression-budget verdict against the accepted baseline
- keep direct runner evidence as the primary release artifact

Reference:

- [performance.md](performance.md)
- `docs/perf-baselines/latest-slo-gate-result.json`
- `docs/perf-baselines/latest-runtime-e2e-result.json`

Open caveat:

- the direct benchmark host path is the trusted release path; do not hide release evidence behind repo-local wrapper scripts

### 5. Docs Freeze Coherence

Before public alpha signoff, do one final docs pass confirming that these files agree:

- [overview.md](overview.md)
- [CHANGELOG.md](/changelog)
- [SUPPORT.md](/support)
- [getting-started.md](getting-started.md)
- [examples.md](examples.md)
- [architecture-overview.md](architecture-overview.md)
- [CONTRIBUTING.md](/contributing)
- [CODE_OF_CONDUCT.md](/code-of-conduct)
- [SECURITY.md](/security)

- [spec.md](spec.md)
- [public-api-guidelines.md](public-api-guidelines.md)
- [public-api-inventory.md](public-api-inventory.md)
- [theme-system.md](theme-system.md)
- [widget-roadmap.md](widget-roadmap.md)
- [performance.md](performance.md)

Goal:

- no stale canonical paths
- no references to removed docs
- no mismatch between product contract and release contract

### 6. Final Manual Signoff

Still required:

- engineering signoff
- product or release signoff
- explicit approval for publish

Public alpha is not closed until those are written against the final candidate SHA.

## Non-Gating Follow-Up

Useful, but not alpha-blocking:

- `ScatterPlot` frame-style parity follow-up
- `LogView.AppendLine` discoverability alias
- theme-token naming crosswalk clarity
- additional native-host manual verification on terminals not installed on this machine

## Release Rule

Tessera public alpha is ready only when:

1. candidate metadata is filled
2. repo consistency blockers are resolved
3. build, test, examples, and smoke are rerun on the exact candidate SHA
4. perf gate is rerun and accepted on that same SHA
5. docs coherence pass is done
6. engineering and product signoff are recorded
