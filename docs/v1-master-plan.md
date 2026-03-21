# TeaSharp Public V1 Master Plan

This document is the authoritative execution plan for Public V1.
All agents must treat this file as the source of truth for scope, sequencing, ownership, and done criteria.
Release-candidate execution checklist: [public-v1-rc-checklist.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-v1-rc-checklist.md).

## Vision
- Ship a C#-first TUI library with a small default API, strong extensibility, and polished visuals.
- Keep `TeaSharp` as the default app-authoring surface.
- Keep `TeaSharp.Core` as low-level advanced product, supported but not onboarding-first.
- Ensure advanced teams can build complex apps without internal forks.

## Non-Goals
- No rewrite of the runtime loop or renderer internals unless required by correctness/perf defects.
- No dependency-injection-first public startup model for normal apps.
- No image rendering support in Public V1 (scheduled for V1.1).

## Scope Split
### Public V1 (in scope)
- Public API decoupling and boundary cleanup.
- No-DI startup path standardization.
- Theming/styling architecture and implementation for core controls.
- Widget expansion toward a broad built-in catalog.
- Regression tests, docs alignment, and baseline benchmarks.

### V1.1 (out of scope for V1)
- Image embedding (`kitty`, `iTerm2`, `wezterm`, `ghostty`) with capability fallback.
- Advanced image render modes (native, pixelated block fallback).

## Authoritative Execution Order (Do Not Reorder)
Correctness is a continuous gate across all phases: fix regressions at source and add regression tests for touched bug classes.

1. **(2) Terminal Compatibility Verification Matrix**
   - Verify Ghostty, iTerm2, WezTerm, Kitty, and Windows Terminal behavior.
   - Document support model for `ScreenOptions.FontSpec` (best-effort request, terminal-dependent).
   - Exit criteria: matrix complete + unsupported terminals verified as safe no-op (no crash, no broken frame output).
2. **(4) Widget Expansion**
   - Deliver planned built-in widget tranche with consistent theming/state hooks.
   - Exit criteria: widget tranche targets met per roadmap and covered by render/theme tests.
3. **(3) Visual Quality Full Polish Pass**
   - Execute full visual refinement after the widget tranche is implemented.
   - Keep only minimum visual contract enforcement during widget build-out (state hooks, override path, monochrome readability).
   - Exit criteria: polish checklist complete for expanded widget surface; visual regressions closed.
4. **(1) API Freeze + Cleanup**
   - Freeze public naming/shape, remove ambiguity, cleanup dead or duplicate paths.
   - Exit criteria: naming clarity gate pass + API commenting gate pass + cleanup diffs validated.
5. **(5) Performance Gate + Benchmarks**
   - Run V1 benchmark gate and finalize docs/release checklist.
   - Exit criteria: perf gates pass, benchmark evidence attached, docs/release coherence signoff complete.

## Current Progress
- **Status Legend (for this file)**
  - `Done` = implementation landed and merged.
  - `In progress` = active execution, not yet at phase exit criteria.
  - `Pending manual signoff` = release-gate evidence still required; RC is not closed.
  - `Blocked` = open issue prevents phase exit.
- **M1: Terminal Compatibility Verification Matrix** -> **Done**
  - cross-terminal font capability doc added (`xterm`, `iTerm2`, `Kitty`, `WezTerm`, `Ghostty`, Windows terminals)
  - verification evidence captured in [terminal-compatibility-evidence-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-compatibility-evidence-v1.md)
  - explicit environment note: iTerm2/WezTerm/Kitty/Windows Terminal binaries are not installed on this host; their verification uses deterministic tests + official specs; Ghostty has host evidence
  - capability-gated no-op fallback behavior is covered by deterministic tests for unsupported terminals
- **M2: Widget Expansion** -> **Done**
  - widget roadmap backlog is zero across Waves 1-4 in [widget-roadmap-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/widget-roadmap-v1.md)
  - Wave 1 app-shell/forms tranche integrated in docs+catalog/theme coverage (`Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`)
  - Wave 4 batch A + B integrated in docs+catalog/theme coverage (`DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`)
- **M3: Visual Quality Full Polish Pass** -> **Done**
  - visual polish closure evidence landed:
    - `1c43dbe` -> WidgetGallery polished default theme/layout slice
    - `d3d7065` -> Choice/ComboBox/TreeView border+glyph+focus-marker token override slice
    - `9ea516e` + `8e778d1` -> cross-control focus-marker parity wiring follow-up
    - `1dba5bc` + `3731c50` -> visual regression/assertion updates and decoder leak fix
    - `cf3e8a1` -> focus-marker policy/regression test follow-up
  - runtime visual sanity artifacts exist under `.artifacts/screenshots/*`; current text-capture scans show no `<...;...M` leakage patterns
  - latest workspace verification on March 21, 2026 is green (`dotnet test TeaSharp.slnx --no-build`, `dotnet build TeaSharp.Examples.slnx --no-restore --nologo -v minimal`)
- **M4: API Freeze + Cleanup** -> **In progress**
  - checklist evidence updates from recent commits:
    - item `1` + item `6` evidence: `c712c2a` -> [examples/PublicApiDashboard/Program.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/examples/PublicApiDashboard/Program.cs), [docs/public-api-consumer-friction-log.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-consumer-friction-log.md), [examples/PublicApiDashboard/README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/examples/PublicApiDashboard/README.md)
    - item `4` focused-border naming/behavior consistency evidence: `1a4fab8` -> [src/TeaSharp/Controls/SearchResultsView.Rendering.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/src/TeaSharp/Controls/SearchResultsView.Rendering.cs), [tests/TeaSharp.Tests/SearchResultsViewControlTests.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/tests/TeaSharp.Tests/SearchResultsViewControlTests.cs)
    - item `3` + item `6` docs sync evidence: `ed4449d` -> [docs/public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md), [docs/theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md), [docs/spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md)
    - item `5` XML docs pass evidence: `4e3a240` -> [src/TeaSharp/Styles/TeaStyle.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/src/TeaSharp/Styles/TeaStyle.cs), [src/TeaSharp/Styles/TeaThemeControlExtensions.DevOpsAndWorkflows.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/src/TeaSharp/Styles/TeaThemeControlExtensions.DevOpsAndWorkflows.cs)
  - C4 remains open until all API freeze checklist items are marked complete
- **M5: Performance Gate + Benchmarks + Docs Freeze** -> **Pending manual signoff**
  - harness and scenarios are wired; final pass/fail evidence is still required at RC

## RC Closure Manual Signoffs (Unresolved Until Checked)
Public V1 RC must not be declared closed from this document alone. The gates below require explicit human signoff with evidence links.

| Gate | Required Evidence | Current Status |
|---|---|---|
| RC checklist closure | Completed entries in [public-v1-rc-checklist.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-v1-rc-checklist.md) with owner/date | Pending manual signoff |
| Verification matrix rerun on RC candidate SHA | Command output + pass/fail summary for unit, integration, full suite, and example smoke from the exact candidate SHA | Pending manual signoff |
| Performance gate approval | Benchmark report links and pass/fail verdict per [perf-plan-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-plan-v1.md) | Pending manual signoff |
| Docs freeze coherence approval | Final docs diff review confirming `v1-master-plan.md` + `source-of-truth.md` + API docs are aligned | Pending manual signoff |

Release approval rule: M5 is only complete when all four rows above are moved from `Pending manual signoff` to `Done` with owner/date evidence.

## Workstreams
### WS-A: Public API and Runtime Boundaries
- Remove DI-centric startup from the default app path.
- Keep startup library-first (`Tea.RunAsync(...)`, `Tea.CreateBuilder()`, `UseApp(...)`, runtime options).
- Standardize namespace boundaries and advanced lane guidance.
- Add API guardrails and compatibility checks.

### WS-B: Theming and Styling
- Define semantic theme tokens and palette model.
- Implement override hierarchy:
  1. global theme
  2. control-type defaults
  3. control instance overrides
  4. per-state overrides (`focused`, `selected`, `hovered`, `disabled`, `error`, etc.)
- Replace hardcoded focus affordances with theme-driven behavior.
- Cover top controls first, then broaden.

### WS-C: Widgets and UX Depth
- Expand built-in widgets (10-15 in V1 tranche).
- Ensure widget APIs follow consistent control/event/style patterns.
- Keep customization/theming first-class for all new widgets.
- Execution scope and inventory are tracked in [widget-roadmap-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/widget-roadmap-v1.md).

### WS-D: Quality, Performance, and Docs
- Full-cycle validation: unit, integration, example smoke, docs sync.
- Cleanup pass: remove dead/legacy paths, simplify logic, optimize hot allocations.
- Benchmark harness and baseline comparisons per [perf-plan-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/perf-plan-v1.md).
- Public API docs and examples as release gate.

## API Simplification Contract
- Keep default app-authoring surface in `TeaSharp`, `TeaSharp.Controls`, `TeaSharp.Layout`.
- Generalize recurring control patterns:
  - consistent `Title`, `FocusMarker`, `ShowFocusMarker`, `Border`, `Padding`, and style hook naming.
  - consistent event payload conventions (`SelectionChanged`, submit/cancel, activate/execute).
  - consistent state naming (`IsOpen`, `IsFocused`, `IsDisabled`, `IsReadOnly`) and semantics.
- Hide complex internals from default path:
  - avoid exposing low-level runtime/compiler/renderer details in onboarding docs/examples.
  - keep advanced APIs in explicit advanced namespaces/docs.
- No breaking API reshapes in V1 unless they remove ambiguity and are migration-documented in the same change.

## API Freeze/Cleanup Checklist (Phase 4 Gate)
1. Default path remains no-DI: `Tea.RunAsync(new App())` and `Tea.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...)`.
2. Advanced seams (`TeaSharp.Hosting`, renderer/terminal adapters) stay documented as opt-in only.
3. Public control APIs follow C# conventions:
   - noun properties for state/config (`Title`, `IsOpen`, `SelectedIndex`)
   - verb methods for actions (`SetItems`, `ApplyTheme`, `Clear`)
   - `Try*` only for non-throwing probe/consume patterns
4. Cross-control naming is normalized:
   - focus/title: `FocusMarker`, `ShowFocusMarker`, `TitleStyle`, `FocusedTitleStyle`
   - frame styling: `BorderStyleText`, `FocusedBorderStyleText` where border glyph styling exists
   - selection semantics: `SelectedIndex`, `SelectedItem`, `SelectionChanged`
5. Public XML docs match runtime behavior for any changed API before merge.
6. Starter docs/examples do not import `TeaSharp.Core.*`.

M4 checklist status snapshot (March 21, 2026):

- item `1`: **In progress** (consumer-path evidence landed via `c712c2a`)
- item `2`: **In progress** (no new closure evidence in this slice)
- item `3`: **In progress** (friction-log and inventory/spec/theme sync evidence landed via `c712c2a`, `ed4449d`)
- item `4`: **In progress** (SearchResultsView focused-border merge fix landed via `1a4fab8`)
- item `5`: **In progress** (XML docs coverage updates landed via `4e3a240`)
- item `6`: **In progress** (PublicApiDashboard + docs sync indicate no `TeaSharp.Core` onboarding leakage in touched assets)

## Naming Clarity Gate
- Public names must be unambiguous to C# developers without reading internals.
- Gate checklist for every new/renamed public symbol:
  - Name matches behavior and scope (no overloaded meaning like "Manager", "Helper", "Context" without domain prefix).
  - Method intent is obvious from signature (`Set*`, `Apply*`, `Try*`, `Create*`, `Render*` used consistently).
  - Option/state names avoid double negatives and hidden side effects.
  - Similar controls use the same term for the same concept (for example `SelectedItem` vs `CurrentItem` must be standardized).
  - Ambiguous names require rename before merge or explicit documented exception.

## Public API Commenting Gate
- All public APIs changed for V1 must have meaningful XML docs.
- Minimum bar:
  - `<summary>` explains behavior and intent, not type restatement.
  - `<param>` documents constraints/default behavior where non-obvious.
  - `<returns>` for non-void methods with semantic meaning.
  - `<remarks>` where side effects, lifecycle, or ordering matter.
  - `<exception>` where caller-actionable exceptions are expected.
- Gate enforcement:
  - PR fails if new/changed public APIs are undocumented or copy-template comments.
  - Docs must match actual runtime behavior in examples/tests.

## Visual Quality Baseline Contract (Applied During Phase 2 Widget Build-Out)
1. Override hierarchy remains enforced and documented:
   - global theme -> control-type defaults -> control instance -> state.
2. Focus visuals are fully overrideable (marker + title + border), not marker-only.
3. Border/text styling hooks are wired for shipped interactive controls:
   - `Choice`, `ComboBox`, `TextInput`, `SearchBox`, `Table`, `TreeView`.
4. Glyph customization hooks are available where symbolic affordances are core UX:
   - `DropdownGlyphSet` for `Choice`/`ComboBox`
   - `TreeViewGlyphSet` for `TreeView`.
5. Navigation/listing controls expose coherent title-focus options:
   - `ListView<T>`, `Table`, `TreeView`, `TextInput`, `SearchBox`.
6. Theme mappings cover current V1 control groups:
   - `Basic`, `InputValue`, `Navigation`, `NavigationOverlay`, `NavigationPrimitives`, `DataAndFlow`, `ExplorerAndFeedback`, `RenderingTextUtilities`, `ModalAndCharts`.
7. Monochrome rendering remains readable when style hooks are empty.

## Visual Quality Full Polish Checklist (Phase 3 Gate)
1. Expanded widget set receives spacing/contrast/passive-state polish review.
2. Focus, hover, selected, and error visuals are consistent across new widgets.
3. Default visuals feel cohesive across built-ins with no hardcoded focus marker assumptions.
4. WidgetGallery/Showcase outputs are updated to demonstrate polished defaults.
5. Regression snapshots/text assertions are updated for intentional visual deltas only.

## Open V1 Visual Parity Gaps (Current)
- Closed: cookbook examples for overlay glyph APIs, border overrides, dropdown/tree glyph sets, and data widget separator/marker hooks (`edf676c`, `dcdc51f`).
- Closed: visual edge-case regression assertions for parity-sensitive rendering paths (`7caa741`).
- Closed: overlay/input token-consistency enforcement in theme mappings (`74751e6`).
- Closed: bordered-control parity rollout for existing shipped controls (including Group1/Group2 additions) (`52e7574`, `91f07b9`, `33b22d5`, `4a3a103`, `135065e`, `98a6d7d`).
- Remaining (forward-only): keep parity policy enforced for any new bordered control added post-freeze (must ship with `BorderStyleText`/`FocusedBorderStyleText`, token mapping, and regression coverage in the same slice).

## Parallelization Constraints
- Parallel work is allowed only within the active phase in the authoritative execution order.
- Current active phase: **M4 API Freeze + Cleanup** (M3 closed).
- Do not start phase N+1 before phase N exit criteria are satisfied.
- Within a phase:
  - parallelize only with disjoint file ownership.
  - central coordinator validates gate status before opening next checkpoint.

## Milestones and Acceptance Criteria
### M1: Terminal Compatibility Verification Matrix
- Acceptance:
  - matrix coverage complete for Ghostty, iTerm2, WezTerm, Kitty, and Windows Terminal
  - `ScreenOptions.FontSpec` support model is explicit (best-effort, terminal-dependent)
  - unsupported terminals are validated as no-op fallback for font requests (safe output; no frame corruption)
  - touched-area unit/integration checks pass

### M2: Widget Expansion
- Acceptance:
  - V1 widget tranche target from `docs/widget-roadmap-v1.md` is complete
  - new widgets follow existing API/style naming and theme-token conventions
  - minimum visual baseline contract is enforced for every added widget

### M3: Visual Quality Full Polish Pass
- Acceptance:
  - full polish checklist is complete for expanded widget surface
  - theme/default/override behavior is validated across new widgets
  - intentional visual expectation updates are captured in tests

### M4: API Freeze + Cleanup
- Acceptance:
  - API freeze/cleanup checklist is satisfied for touched public surfaces
  - naming clarity gate passes for all new/renamed public symbols
  - default onboarding path remains `TeaSharp`-first with no new `TeaSharp.Core` leakage
  - public API commenting gate passes for V1-touching APIs

### M5: Performance Gate + Benchmarks + Docs Freeze (Release Candidate)
- Acceptance:
  - perf gates from `docs/perf-plan-v1.md` pass with benchmark report attached
  - docs/examples/release checklist are coherent and frozen for RC
  - RC manual signoff rows are all moved to `Done` with owner/date evidence

## Dependency Graph and Critical Path
1. Terminal compatibility verification matrix
2. Widget expansion
3. Visual quality full polish pass
4. API freeze + cleanup
5. Performance gate + benchmarks + docs freeze

Critical path: Phase 1 -> Phase 2 -> Phase 3 -> Phase 4 -> Phase 5 (strict order).

## Agent Ownership Matrix (3-Lane Model)
| Lane | Primary Responsibility | Typical Files |
|---|---|---|
| Lane A | API boundaries, startup model, runtime-facing public surface | `src/TeaSharp/*`, startup examples, API tests |
| Lane B | Theme/styling architecture and rollout | `src/TeaSharp/Styles*`, control rendering paths, style docs |
| Lane C | Master plan, docs coherence, guardrails, release checklist | `docs/*`, `AGENTS.md`, boundary tests |

Coordination rules:
- One lane per logical task.
- Parallel where file ownership is disjoint.
- Merge checkpoints at each milestone.

## Agent Coordination Checkpoints
Current active checkpoint: **C4: API Freeze Exit**.

- **C0: Matrix Ready**
  - Required: lane ownership map confirmed and terminal verification inputs agreed.
  - Exit: terminal compatibility phase can begin.
- **C1: Terminal Matrix Exit**
  - Required: compatibility matrix complete for Ghostty/iTerm2/WezTerm/Kitty/Windows Terminal.
  - Required: `FontSpec` unsupported paths verified as safe no-op.
  - Exit: widget expansion phase can begin.
- **C2: Widget Expansion Exit**
  - Required: widget tranche target met with render/theme coverage.
  - Required: full polish backlog for expanded widgets captured.
  - Exit: visual full-polish phase can begin.
- **C3: Visual Full-Polish Exit**
  - Required: full visual polish checklist pass for expanded widgets.
  - Required: visual regression assertions updated for intentional changes.
  - Exit: API freeze/cleanup phase can begin.
- **C4: API Freeze Exit**
  - Required: API freeze/cleanup checklist pass + naming clarity gate pass + XML docs pass.
  - Required: public API diff reviewed and migration notes updated.
  - Exit: perf/docs-freeze phase can begin.
- **C5: Release Candidate Exit**
  - Required: perf gates pass per `docs/perf-plan-v1.md` and benchmark evidence attached.
  - Required: final verification matrix run and RC manual signoff rows closed.
  - Exit: Public V1 release approval.

## Risk Register
| Risk | Impact | Mitigation |
|---|---|---|
| Theme model churn | delayed widget rollout | freeze token contract before broad widget updates |
| Public API drift | onboarding confusion | boundary tests + docs source-of-truth updates per merge |
| Performance regressions from styling | runtime latency | benchmark before/after on styled high-churn scenarios |
| Widget inconsistency | poor DX | enforce widget API/style checklist in PR review |
| Scope creep | delayed V1 | strict V1 vs V1.1 scope gate (images deferred) |

## Verification Matrix
| Area | Command/Check | Gate |
|---|---|---|
| Unit tests | `dotnet test tests/TeaSharp.Tests --no-restore` | must pass |
| Integration | `dotnet test tests/TeaSharp.IntegrationTests --no-restore` | must pass |
| Full suite | `dotnet test TeaSharp.slnx --no-restore` | must pass |
| Example smoke | build/run canonical + advanced examples | must pass |
| Benchmarks | benchmark suite report committed to docs/artifacts | must exist |
| Docs | public API + examples + migration guidance consistent; RC checklist completed | must pass review |

## Weekly Checkpoint Format
- Week label:
- Milestone target:
- Completed:
- In progress:
- Blockers:
- Risks changed:
- Verification run summary:
- Next week commitments:

## Definition of Done: Public V1
- V1 scope complete, V1.1 scope excluded.
- No-DI default public startup path finalized.
- Theme system stable with built-in and custom palettes.
- Core widget tranche delivered and theme-aware.
- Boundary tests, full tests, and example smoke all green.
- Benchmark report produced and reviewed.
- Public docs/API guidance coherent and release-ready.
