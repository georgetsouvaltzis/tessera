# Public API Refactor Plan

## Goals
- Make `src/TeaSharp` the default app-building path for C# developers, with familiar .NET patterns and minimal ceremony.
- Keep `src/TeaSharp.Core` as a stable low-level product for advanced scenarios, not the default onboarding path.
- Align implementation and docs with [docs/spec.md](docs/spec.md), [docs/source-of-truth.md](docs/source-of-truth.md), [docs/public-api-inventory.md](docs/public-api-inventory.md), and [docs/public-api-guidelines.md](docs/public-api-guidelines.md).

## Non-goals
- Rewriting runtime internals that are already correct and performant.
- Removing `TeaSharp.Core` capabilities or blocking advanced extensibility.
- Introducing custom DSL-style APIs that diverge from idiomatic C#.

## Design Principles
- C#-first shape: typed options, builder patterns, async + cancellation conventions, explicit extension points.
- Layer clarity: app developers start in `TeaSharp`; `TeaSharp.Core` appears only in advanced docs and samples.
- Incremental migration: additive first, deprecate later with clear guidance and tests.

## Phased Plan
### Phase 1: Surface Baseline and Guardrails
1. Define the supported public surface in `TeaSharp` from inventory/spec docs.  
Acceptance: every exported root API has owner, purpose, and status in `docs/public-api-inventory.md`.
2. Add boundary tests preventing examples from importing `TeaSharp.Core.*` for common scenarios.  
Acceptance: new tests fail on unauthorized Core usage and pass on approved advanced examples.
3. Normalize startup path (`Tea.CreateBuilder(...)`, `RunAsync(...)`) across canonical examples.  
Acceptance: hello-world example compiles with only `TeaSharp` namespaces and no Core dependency.
4. Docs update pass for baseline terminology and namespace consistency.  
Acceptance: `docs/spec.md` and `docs/source-of-truth.md` match API names used by examples.

### Phase 2: Ergonomic API Consolidation
5. Move common configuration seams into typed options/builders in `TeaSharp`.  
Acceptance: common app setup needs no direct Core types.
6. Standardize public async APIs (`*Async`, optional `CancellationToken` last).  
Acceptance: API review shows no naming/signature exceptions without written justification.
7. Promote common interaction patterns (commands, keymaps, layout/state) as first-class `TeaSharp` APIs.  
Acceptance: at least two existing advanced examples remove Core-only plumbing.
8. Docs update for usage patterns and migration notes.  
Acceptance: `docs/public-api-guidelines.md` includes before/after snippets for each promoted pattern.

### Phase 3: Compatibility and Adoption
9. Add deprecation shims for legacy entry points and namespace transitions.  
Acceptance: legacy APIs compile with `[Obsolete]` warnings and link to migration docs.
10. Finalize canonical example progression (basic, stateful, advanced) and smoke it in CI.  
Acceptance: all three compile; at least one run-smoke per tier succeeds in CI.

## Compatibility Strategy
- Keep behavioral compatibility during refactor: additive APIs first, warning-only obsoletions next, removals only after documented window.
- Publish migration map updates in lockstep with releases, including old/new API tables and rationale.
- Maintain Core parity for advanced users while simplifying default paths for app teams.

## Verification Matrix
| Area | Check | Acceptance |
|---|---|---|
| Unit | `dotnet test tests/TeaSharp.Tests` | API boundary + behavior regressions covered |
| Integration | `dotnet test tests/TeaSharp.IntegrationTests` | Terminal/runtime flows unchanged |
| Examples | build + run smoke for basic/stateful/advanced samples | onboarding paths are executable and simple |
| Compatibility | obsolete warnings + migration docs | no silent breaking changes |
| Docs | spec/source-of-truth/inventory/guidelines updated per phase | docs reflect shipped behavior |
