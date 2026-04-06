# Contributing To TeaSharp

TeaSharp is developed in public. Contributions are welcome, but this repo is still in alpha and we will prefer API clarity over compatibility when necessary.

Release-facing changes should also be reflected in [CHANGELOG.md](CHANGELOG.md).

## Before You Start

- read [README.md](README.md)
- read [docs/getting-started.md](docs/getting-started.md)
- read [docs/architecture-overview.md](docs/architecture-overview.md)
- read [docs/public-api-guidelines.md](docs/public-api-guidelines.md)
- read [SUPPORT.md](SUPPORT.md) for the public issue/support contract

## Development Prerequisites

- `.NET 10.0.103` SDK from [global.json](global.json)
- a terminal with strong ANSI/CSI behavior for manual app checks

## Repo Layout

- `src/TeaSharp`: default public API
- `src/TeaSharp.Core`: advanced runtime internals
- `tests/TeaSharp.Tests`: unit, regression, and contract tests
- `tests/TeaSharp.IntegrationTests`: integration/runtime coverage
- `examples`: public examples and showcase apps
- `docs`: product and contributor docs

## Required Verification

Before asking for review, run:

```bash
dotnet build TeaSharp.slnx
dotnet build examples/TeaSharp.Examples.slnx
dotnet test TeaSharp.slnx
scripts/smoke_examples.sh 4
```

If your change touches perf-sensitive code or release-track behavior, also follow [docs/performance.md](docs/performance.md).

## Contribution Rules

- fix root causes, not temporary patches
- add regression tests when fixing bugs
- keep the default public path in `TeaSharp`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`
- do not move onboarding toward `TeaSharp.Core`
- update docs when public behavior or public examples change
- use conventional commits when maintainers ask for commit-ready slices

## Public API Changes

If you change a public API or public behavior, update the same slice:

- [README.md](README.md)
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

If you are unsure where something belongs, open the issue or draft PR with the smallest reproducible example you can provide.
