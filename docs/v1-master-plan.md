# TeaSharp Public V1 Master Plan

This document is the authoritative execution plan for Public V1.
All agents must treat this file as the source of truth for scope, sequencing, ownership, and done criteria.

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
1. **Correctness + Bug-Fix Stabilization**
   - Fix regressions first; no feature polish before stability.
   - Add/extend regression tests for every production bug class touched.
   - Exit criteria: integration + unit gates green for touched areas; no known P0/P1 regressions open.
2. **API Simplification + Boundary Cleanup**
   - Simplify public entry points and recurring control patterns.
   - Keep advanced seams available but out of default onboarding path.
   - Exit criteria: API review pass + naming clarity gate pass + no new onboarding leaks from `TeaSharp.Core`.
3. **Visual Polish + Theming Consistency**
   - Improve default visuals after behavior/API stability.
   - Keep keyboard/mouse semantics unchanged unless explicitly scoped.
   - Exit criteria: snapshots/visual assertions updated; theme override behavior verified.
4. **Expansion + Perf Hardening + Docs Freeze**
   - Ship remaining V1 widget tranche, perf hardening, and final docs freeze in one release phase.
   - Exit criteria: widget tranche complete, perf gates pass, docs/commenting gates pass, release checklist complete.

## Current Progress
- **M1: Boundary Baseline** -> **Done**
  - no-DI startup policy established on docs path
  - canonical onboarding progression established (`HelloWorld` -> `CounterForm` -> `WorkspaceApp`)
  - boundary guardrails active
- **M2: Theme Contract** -> **Done**
  - `TeaThemeControlExtensions` split into domain partial files (`Basic`, `InputValue`, `Navigation`, `NavigationOverlay`, `NavigationPrimitives`, `DataAndFlow`, `ExplorerAndFeedback`, `RenderingTextUtilities`, `ModalAndCharts`)
  - direct token mappings landed for input/value controls (`TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`)
  - direct token mappings landed for navigation/overlay controls (`Choice`, `ComboBox`, `TreeView`, `MenuBar`, `ContextMenu`, `CommandPalette`, `Notifications`)
  - direct token mappings landed for navigation primitives (`Accordion`, `MultiSelect`, `RadioGroup`)
  - direct token mappings landed for rendering text utilities (`Badge`, `LogView`, `MarkdownView`, `MiniLog`)
  - direct token mappings landed for modal/chart summary controls (`Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`)
  - control-level style hooks and theme mappings are implemented across the shipped Public V1 control surface
- **WS-D benchmark harness status**
  - `BenchmarkSwitcher` discoverability is wired and `--list flat` lists all 6 required scenarios (`Startup`, `LogTail`, `LargeTable`, `OverlayStress`, `ResizeStorm`, `StyledHeavyOutput`)
  - supplemental viewport no-decoration benchmark coverage is wired (`ViewportRenderBenchmarks`) for render/materialize hot-path tracking

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

## API Simplification Execution Checklist (Phase 2 Gate)
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

## Beautiful UI Visual Customization Checklist (Phase 3 Gate)
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

## Open V1 Visual Parity Gaps (Current)
- Closed: cookbook examples for overlay glyph APIs, border overrides, dropdown/tree glyph sets, and data widget separator/marker hooks (`edf676c`, `dcdc51f`).
- Closed: visual edge-case regression assertions for parity-sensitive rendering paths (`7caa741`).
- Closed: overlay/input token-consistency enforcement in theme mappings (`74751e6`).
- Remaining: enforce the same parity rule for any new control added post-freeze (must ship with style hooks + token mapping + regression coverage in the same slice).

## Parallelization Constraints
- Parallel work is allowed only within the active phase in the authoritative execution order.
- Do not start phase N+1 before phase N exit criteria are satisfied.
- Within a phase:
  - parallelize only with disjoint file ownership.
  - central coordinator validates gate status before opening next checkpoint.

## Milestones and Acceptance Criteria
### M1: Correctness and Bug-Fix Stabilization
- Acceptance:
  - all known P0/P1 regressions in active scope are fixed or explicitly deferred with owner/date
  - regression tests added/updated for every fixed defect class
  - touched-area unit/integration filters pass consistently

### M2: API Simplification and Boundary Contract
- Acceptance:
  - API simplification contract is satisfied for touched public surfaces
  - naming clarity gate passes for all new/renamed public symbols
  - default onboarding path remains `TeaSharp`-first with no new `TeaSharp.Core` onboarding leakage

### M3: Visual Polish and Theme Consistency
- Acceptance:
  - default visuals are improved without semantic/input regressions
  - theme/default/override behavior is validated on touched controls
  - visual/text snapshot expectations are updated where intentional changes were made

### M4: Expansion + Performance + Docs Freeze (Release Candidate)
- Acceptance:
  - V1 widget tranche target from `docs/widget-roadmap-v1.md` is complete
  - perf gates from `docs/perf-plan-v1.md` pass with benchmark report attached
  - public API commenting gate passes for V1-touching APIs
  - docs/examples/release checklist are coherent and frozen for RC

## Dependency Graph and Critical Path
1. Correctness + bug-fix stabilization
2. API simplification + boundary cleanup
3. Visual polish + theming consistency
4. Expansion + perf hardening + docs freeze

Critical path: Phase 1 -> Phase 2 -> Phase 3 -> Phase 4 (strict order).

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
- **C0: Stabilization Ready**
  - Required: open regression inventory, failing tests triaged, lane ownership map confirmed.
  - Exit: correctness phase can begin.
- **C1: Correctness Exit**
  - Required: all active regressions closed or explicitly deferred with owner/date.
  - Required commands: touched-area unit/integration filters + smoke scenarios.
  - Exit: API simplification phase can begin.
- **C2: API Contract Exit**
  - Required: API simplification contract pass + naming clarity gate pass.
  - Required: public API diff reviewed and migration notes updated.
  - Exit: visual polish phase can begin.
- **C3: Visual Exit**
  - Required: visual defaults updated with behavior parity; theme override checks green.
  - Required: snapshot/text-render assertions updated where expected.
  - Exit: expansion/perf/docs-freeze phase can begin.
- **C4: Release Candidate Exit**
  - Required: widget tranche target met, perf gates pass per `docs/perf-plan-v1.md`, docs/commenting gate pass.
  - Required: final verification matrix run and archived in PR/release notes.
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
| Docs | public API + examples + migration guidance consistent | must pass review |

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
