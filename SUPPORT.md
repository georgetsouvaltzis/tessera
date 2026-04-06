# Support

TeaSharp is in public alpha.

Support is best-effort, in public, and optimized for reproducible reports.

## Where To Ask

- bugs, regressions, crashes, and incorrect rendering:
  - open a GitHub Issue
- docs confusion, onboarding gaps, and example problems:
  - open a GitHub Issue
- feature requests and API ideas:
  - open a GitHub Issue
- security reports:
  - follow [SECURITY.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/SECURITY.md)

TeaSharp does not currently provide private product support or response-time guarantees.

## Before Opening An Issue

Please check these first:

- [README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/README.md)
- [docs/getting-started.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/getting-started.md)
- [docs/examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md)
- [CHANGELOG.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CHANGELOG.md)

If you can, verify against the current public baseline:

- `1.0.0-alpha.1`
- or the latest commit on the default branch

Useful local checks before filing:

```bash
dotnet build TeaSharp.slnx
dotnet test TeaSharp.slnx
dotnet build examples/TeaSharp.Examples.slnx
scripts/smoke_examples_v1.sh 4
```

## What To Include

Strong issues usually include:

- TeaSharp version or commit SHA
- .NET SDK version
- operating system and terminal emulator
- exact example or project path involved
- expected result
- actual result
- minimal reproduction steps
- screenshots or captured output when the issue is visual

For example-related reports, say whether the problem is in:

- starter ladder: `HelloWorld`, `CounterForm`, `WorkspaceApp`
- flagship showcases: `GitConsole`, `OpsWatch`, `DataWorkbench`
- supporting demos

## Alpha Expectations

TeaSharp is already usable, but the public contract is still tightening.

That means:

- breaking changes can still happen before `1.0.0`
- examples and docs should move together
- reports with precise repro steps will get the fastest traction

## Maintainer Triage Intent

Maintainers will usually prioritize:

1. crashes and rendering regressions
2. starter-ladder and docs breakage
3. public API clarity issues
4. showcase/example polish
5. broader expansion ideas
