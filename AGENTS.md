# Repository Guidelines

## Source Of Truth
- `docs/v1-master-plan.md` is the authoritative execution plan for Public V1.
- `docs/widget-roadmap-v1.md` is the authoritative widget scope contract for M3.
- If a task conflicts with older docs, follow `v1-master-plan.md` and update outdated docs in the same change.

## Project Structure
- `src/TeaSharp`: default public app-authoring API.
- `src/TeaSharp.Core`: advanced low-level runtime layer, supported but not onboarding-first.
- `tests/TeaSharp.Tests`: unit/contract/regression tests.
- `tests/TeaSharp.IntegrationTests`: runtime/integration flows.
- `examples/HelloWorld`, `examples/CounterForm`, `examples/WorkspaceApp`: canonical onboarding progression.
- `examples/AdvancedWidgets`, `examples/WidgetGallery`: advanced interaction lane.

## Public API Boundaries
- Public app path is library-first and no-DI by default.
- Keep starter guidance in `TeaSharp`, `TeaSharp.Controls`, and `TeaSharp.Layout`.
- Do not leak `TeaSharp.Core.*` into onboarding examples/docs.
- Advanced hosting/runtime seams stay under `TeaSharp.Hosting`/advanced docs.

## Styling/Theming Direction
- Theme and style are first-class Public V1 concerns.
- Use semantic theme tokens and explicit override hierarchy (global -> control-type -> instance -> state).
- Hardcoded visual affordances (for example focus markers) should migrate to theme-driven behavior.

## Coordination Model
- One logical task per agent lane.
- Parallelize only when file ownership is disjoint.
- Use milestone checkpoints defined in `docs/v1-master-plan.md`.
- Keep notes short and update docs when behavior/API changes.

## Build/Test Commands
Use .NET 10 from `global.json`.

- `dotnet build TeaSharp.slnx`
- `dotnet test TeaSharp.slnx`
- `dotnet build TeaSharp.Examples.slnx`
- `dotnet run --project examples/HelloWorld`
- `dotnet run --project examples/CounterForm`
- `dotnet run --project examples/WorkspaceApp`

Before handoff, run full cycle (build/tests/examples/docs consistency) and report exact commands/results.

## Coding Rules
- Follow C# conventions already in repo: 4-space indent, nullable enabled, file-scoped namespaces.
- Keep files under 500 LOC where practical; split when needed.
- Fix root causes, not temporary patches.
- Add regression tests when fixing bugs.

## Commit Rules
- Conventional commit prefixes: `feat|fix|refactor|build|chore|docs|perf|test`.
- Keep commits logically scoped and verifiable.
- Commit each completed logical slice before moving to the next slice.
- Include docs updates whenever public behavior/API changes.
