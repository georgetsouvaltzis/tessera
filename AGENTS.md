# Repository Guidelines

## Source Of Truth
- `docs/alpha-release-checklist.md` is the authoritative execution plan for Public Alpha.
- `docs/widget-roadmap.md` is the authoritative widget scope contract for M3.
- `docs/performance.md` is the authoritative performance strategy and release-gate policy.
- If a task conflicts with older docs, follow `alpha-release-checklist.md` and update outdated docs in the same change.
- Execute work in strict phase order from `alpha-release-checklist.md`: correctness -> API simplification -> visual polish -> expansion/perf/docs freeze.

## Project Structure
- `src/Tessera`: default public app-authoring API.
- `src/Tessera.Core`: advanced low-level runtime layer, supported but not onboarding-first.
- `tests/Tessera.Tests`: unit/contract/regression tests.
- `tests/Tessera.IntegrationTests`: runtime/integration flows.
- `examples/HelloWorld`, `examples/CounterForm`, `examples/WorkspaceApp`: canonical onboarding progression.
- `examples/DataWorkbench`, `examples/OpsWatch`, `examples/GitConsole`: flagship public evaluation path.
- `examples/IncidentDesk`, `examples/DownloadCenter`, `examples/TransitBoard`, `examples/MusicDeck`: supporting domain demos.

## Public API Boundaries
- Public app path is library-first and no-DI by default.
- Keep starter guidance in `Tessera`, `Tessera.Controls`, and `Tessera.Layout`.
- Do not leak `Tessera.Core.*` into onboarding examples/docs.
- Advanced hosting/runtime seams stay under `Tessera.Hosting`/advanced docs.

## Styling/Theming Direction
- Theme and style are first-class Public V1 concerns.
- Use semantic theme tokens and explicit override hierarchy (global -> control-type -> instance -> state).
- Hardcoded visual affordances (for example focus markers) should migrate to theme-driven behavior.
- For dropdown-style controls, prefer typed hooks (`DropdownGlyphSet`, `BorderStyleText`, `FocusedBorderStyleText`) over inline hardcoded glyph strings.

## Coordination Model
- One logical task per agent lane.
- Parallelize only when file ownership is disjoint.
- Use milestone checkpoints defined in `docs/alpha-release-checklist.md`.
- Keep notes short and update docs when behavior/API changes.
- When new controls ship, sync `docs/widget-roadmap.md`, `docs/prebuilt-widgets.md`, `docs/public-api-inventory.md`, `docs/spec.md`, and `docs/theme-system.md` in the same slice.

## Build/Test Commands
Use .NET 10 from `global.json`.

- `dotnet build Tessera.slnx`
- `dotnet build examples/Tessera.Examples.slnx`
- `dotnet test Tessera.slnx`
- `dotnet run --project examples/HelloWorld`
- `dotnet run --project examples/CounterForm`
- `dotnet run --project examples/WorkspaceApp`
- `dotnet run --project examples/DataWorkbench/DataWorkbench.csproj --no-build`
- `dotnet run --project examples/OpsWatch/OpsWatch.csproj --no-build`
- `dotnet run --project examples/GitConsole/GitConsole.csproj --no-build`

Before handoff, run full cycle (build/tests/examples/docs consistency) and report exact commands/results.
For performance-sensitive or release-track slices, run checks required by `docs/performance.md` and report outcomes.

## Coding Rules
- Follow C# conventions already in repo: 4-space indent, nullable enabled, file-scoped namespaces.
- Keep files under 500 LOC where practical; split when needed.
- Fix root causes, not temporary patches.
- Add regression tests when fixing bugs.
- For changed public APIs, add meaningful XML docs (`<summary>`, relevant `<param>`, `<returns>`, and `<remarks>` when behavior/ordering is non-obvious).

## Commit Rules
- Conventional commit prefixes: `feat|fix|refactor|build|chore|docs|perf|test`.
- Keep commits logically scoped and verifiable.
- Commit each completed logical slice before moving to the next slice.
- Include docs updates whenever public behavior/API changes.
