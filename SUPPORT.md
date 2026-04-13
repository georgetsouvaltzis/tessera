# Support

Tessera is in public alpha.

Support is best-effort, in public, and optimized for reproducible reports.

## Where To Ask

- bugs, regressions, crashes, and incorrect rendering:
  - open a GitHub Issue with the bug report template
- docs confusion, onboarding gaps, and example problems:
  - open a GitHub Issue
- feature requests and API ideas:
  - open a GitHub Issue with the feature request template when it fits
- security reports:
  - follow [SECURITY.md](SECURITY.md)

Tessera does not currently provide private product support or response-time guarantees.

## Before Opening An Issue

Please check these first:

- [README.md](README.md)
- [docs/getting-started.md](docs/getting-started.md)
- [docs/examples.md](docs/examples.md)
- [CHANGELOG.md](CHANGELOG.md)

If you can, verify against the current public baseline:

- `1.0.0-alpha.1`
- or the latest commit on the default branch

Useful local checks before filing:

```bash
dotnet tool restore
dotnet build Tessera.slnx
dotnet test Tessera.slnx
dotnet build examples/Tessera.Examples.slnx
dotnet jb inspectcode Tessera.slnx -e=HINT --build
dotnet run --project examples/DataWorkbench/DataWorkbench.csproj --no-build
dotnet run --project examples/OpsWatch/OpsWatch.csproj --no-build
dotnet run --project examples/GitConsole/GitConsole.csproj --no-build
```

## What To Include

Strong issues usually include:

- Tessera version or commit SHA
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

Tessera is already usable, but the public contract is still tightening.

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
