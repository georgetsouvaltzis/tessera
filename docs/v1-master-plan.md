# TeaSharp Public V1 Remaining Work

This document tracks only the work still required before TeaSharp can be called Public V1.
Anything already implemented and accepted should live in the product docs, tests, and code, not here.

## Current State

Implemented in code already:

- terminal compatibility model
- widget expansion tranche
- visual polish pass
- API cleanup and naming convergence
- benchmark harness and perf gate foundation
- Docusaurus site bootstrap

Public V1 is still not closed because release evidence and signoff are incomplete.

## Remaining Release Gates

### 1. Choose The Candidate

Still needed:

- final candidate branch or tag
- final candidate commit SHA
- release owner

Without those, none of the remaining evidence is final.

### 2. Reconcile Repo Consistency Blockers

Current repo blockers still referenced by tests and release flow:

- `examples/HelloWorld/HelloWorld.csproj`
- `examples/CounterForm/CounterForm.csproj`
- `examples/WorkspaceApp/WorkspaceApp.csproj`
- `TeaSharp.Examples.slnx`

These must either:

- exist again and be kept current
- or the tests, docs, and release flow must be updated to stop treating them as canonical

Until that is resolved, V1 verification remains noisy and future agents will keep getting mixed signals.

### 3. Rerun Verification On The Exact Candidate SHA

Required commands on the chosen candidate:

- `dotnet build TeaSharp.slnx`
- `dotnet test TeaSharp.slnx`
- `dotnet build TeaSharp.Examples.slnx`
- `scripts/smoke_examples_v1.sh 4`

Goal:

- one final verified evidence set, all from the exact release candidate SHA

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

Before V1 signoff, do one final docs pass confirming that these files agree:

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

V1 is not closed until those are written against the final candidate SHA.

## Non-Gating Follow-Up

Useful, but not V1-blocking:

- `ScatterPlot` frame-style parity follow-up
- `LogView.AppendLine` discoverability alias
- theme-token naming crosswalk clarity
- additional native-host manual verification on terminals not installed on this machine

## Release Rule

TeaSharp Public V1 is ready only when:

1. candidate metadata is filled
2. repo consistency blockers are resolved
3. build, test, examples, and smoke are rerun on the exact candidate SHA
4. perf gate is rerun and accepted on that same SHA
5. docs coherence pass is done
6. engineering and product signoff are recorded
