# TeaSharp Public Alpha Release Checklist

This document tracks the remaining work required before TeaSharp can be treated as a public alpha release on GitHub.
Implemented features belong in the product docs, code, and tests.
This file tracks only the release-closing work.

## Current State

Implemented in code already:

- terminal compatibility model
- widget expansion tranche
- visual polish pass
- API cleanup and naming convergence
- benchmark harness and perf gate foundation
- Docusaurus site bootstrap

TeaSharp is close to public alpha, but it is not release-closed until the repo contract, verification evidence, and signoff are all tied to the same candidate SHA.

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

README, onboarding docs, API docs, smoke scripts, and tests must agree on that contract and must not reference removed example paths or removed solution files.

### 3. Rerun Verification On The Exact Candidate SHA

Required commands on the chosen candidate:

- `dotnet build TeaSharp.slnx`
- `dotnet test TeaSharp.slnx`
- `scripts/smoke_examples_v1.sh 4`

Goal:

- one final verified evidence set, all from the exact public alpha candidate SHA

### 4. Final Performance Approval

Required:

- rerun the perf gate on the exact candidate SHA
- attach explicit regression-budget verdict against the accepted baseline
- keep direct runner evidence as the primary release artifact

Reference:

- [perf-plan-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-plan-v1.md)
- `docs/perf-baselines/latest-slo-gate-result.json`
- `docs/perf-baselines/latest-runtime-e2e-result.json`

Open caveat:

- `scripts/perf_gate_v1.sh` has had wrapper flakiness; the direct benchmark DLL path remains the trusted release path

### 5. Docs Freeze Coherence

Before public alpha signoff, do one final docs pass confirming that these files agree:

- [README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/README.md)
- [getting-started.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/getting-started.md)
- [examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md)
- [architecture-overview.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/architecture-overview.md)
- [CONTRIBUTING.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CONTRIBUTING.md)
- [CODE_OF_CONDUCT.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CODE_OF_CONDUCT.md)
- [SECURITY.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/SECURITY.md)

- [spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md)
- [public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- [public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md)
- [widget-roadmap-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/widget-roadmap-v1.md)
- [perf-plan-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-plan-v1.md)

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

TeaSharp public alpha is ready only when:

1. candidate metadata is filled
2. repo consistency blockers are resolved
3. build, test, examples, and smoke are rerun on the exact candidate SHA
4. perf gate is rerun and accepted on that same SHA
5. docs coherence pass is done
6. engineering and product signoff are recorded
