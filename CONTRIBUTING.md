# Contributing To TeaSharp

TeaSharp is developed in public. Contributions are welcome, but this repo is still in alpha and we will prefer API clarity over compatibility when necessary.

Release-facing changes should also be reflected in [CHANGELOG.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CHANGELOG.md).

## Before You Start

- read [README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/README.md)
- read [docs/getting-started.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/getting-started.md)
- read [docs/architecture-overview.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/architecture-overview.md)
- read [docs/public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- read [SUPPORT.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/SUPPORT.md) for the public issue/support contract

## Development Prerequisites

- `.NET 10.0.103` SDK from [global.json](/Users/georgetsouvaltzis/Projects/playground/teasharp/global.json)
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
scripts/smoke_examples_v1.sh 4
```

If your change touches perf-sensitive code or release-track behavior, also follow [docs/perf-plan-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-plan-v1.md).

## Contribution Rules

- fix root causes, not temporary patches
- add regression tests when fixing bugs
- keep the default public path in `TeaSharp`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`
- do not move onboarding toward `TeaSharp.Core`
- update docs when public behavior or public examples change
- use conventional commits when maintainers ask for commit-ready slices

## Public API Changes

If you change a public API or public behavior, update the same slice:

- [README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/README.md)
- [CHANGELOG.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CHANGELOG.md)
- [docs/spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md)
- [docs/public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- [docs/public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- [docs/theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md) when styling/theming behavior changes
- [docs/examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md) when the example lineup changes

## Pull Request Expectations

Good PRs are:

- scoped to one logical problem
- easy to verify
- backed by tests when behavior changes
- explicit about user-facing impact
- explicit about breaking changes during alpha

If you are unsure where something belongs, open the issue or draft PR with the smallest reproducible example you can provide.
