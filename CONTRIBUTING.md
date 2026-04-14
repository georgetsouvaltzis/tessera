# Contributing To Tessera

Tessera is developed in public. Contributions are welcome, but this repo is still in alpha and we will prefer API clarity over compatibility when necessary.

Release-facing changes should also be reflected in [CHANGELOG.md](CHANGELOG.md).

## Before You Start

- read [docs/overview.md](docs/overview.md)
- read [docs/getting-started.md](docs/getting-started.md)
- read [docs/architecture-overview.md](docs/architecture-overview.md)
- read [docs/public-api-guidelines.md](docs/public-api-guidelines.md)
- read [SUPPORT.md](SUPPORT.md) for the public issue/support contract

## Development Prerequisites

- `.NET 10.0.103` SDK pinned in `global.json`
- a terminal with strong ANSI/CSI behavior for manual app checks
- repo-local tools via `dotnet tool restore`

## Repo Layout

- `src/Tessera`: default public API
- `src/Tessera.Core`: advanced runtime internals
- `tests/Tessera.Tests`: unit, regression, and contract tests
- `tests/Tessera.IntegrationTests`: integration/runtime coverage
- `examples`: public examples and showcase apps
- `docs`: product and contributor docs

## Required Verification

Before asking for review, run:

```bash
dotnet tool restore
dotnet build Tessera.slnx
dotnet build examples/Tessera.Examples.slnx
dotnet test Tessera.slnx
dotnet jb inspectcode Tessera.slnx -e=HINT --build
dotnet build Tessera.slnx -p:TesseraInspectCodeEnabled=true
dotnet build examples/Tessera.Examples.slnx -p:TesseraInspectCodeEnabled=true
dotnet run --project examples/DataWorkbench/DataWorkbench.csproj --no-build
dotnet run --project examples/OpsWatch/OpsWatch.csproj --no-build
dotnet run --project examples/GitConsole/GitConsole.csproj --no-build
```

When `TesseraInspectCodeEnabled=true`, the solution build runs `dotnet jb inspectcode` after compilation, writes a temporary report to `obj/inspectcode.xml`, fails if any findings are reported at the configured severity, and deletes the report automatically on a clean run.

If your change touches perf-sensitive code or release-track behavior, also follow [docs/performance.md](docs/performance.md).

## Contribution Rules

- fix root causes, not temporary patches
- add regression tests when fixing bugs
- keep the default public path in `Tessera`, `Tessera.Controls`, `Tessera.Layout`, and `Tessera.Styles`
- do not move onboarding toward `Tessera.Core`
- update docs when public behavior or public examples change
- use conventional commits when maintainers ask for commit-ready slices

## Public API Changes

If you change a public API or public behavior, update the same slice:

- [docs/overview.md](docs/overview.md)
- [CHANGELOG.md](CHANGELOG.md)
- [docs/spec.md](docs/spec.md)
- [docs/public-api-guidelines.md](docs/public-api-guidelines.md)
- [docs/public-api-inventory.md](docs/public-api-inventory.md)
- [docs/theme-system.md](docs/theme-system.md) when styling/theming behavior changes
- [docs/examples.md](docs/examples.md) when the example lineup changes

## Pull Request Expectations

Good PRs are:

- scoped to one logical problem
- easy to verify
- backed by tests when behavior changes
- explicit about user-facing impact
- explicit about breaking changes during alpha

Use the repository PR template when opening a pull request.

If you are unsure where something belongs, open the issue or draft PR with the smallest reproducible example you can provide.
