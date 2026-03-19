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

## Parallelization Map
- Run WS-A + WS-B + WS-C in parallel.
- WS-D runs continuously, with one hardening sprint at the end.
- Blocking dependencies:
  - WS-B token contract must stabilize before full WS-C styling adoption.
  - WS-A boundary decisions must land before final docs freeze.

## Milestones and Acceptance Criteria
### M1: Boundary Baseline
- Acceptance:
  - no-DI public startup policy implemented
  - starter examples compile with `TeaSharp` namespaces only
  - boundary tests green

### M2: Theme Contract
- Acceptance:
  - semantic theme model documented and implemented
  - built-in palettes: Catppuccin + Rosé Pine + custom
  - focused/selected/hovered/error visuals overrideable

### M3: Widget Tranche
- Acceptance:
  - 10-15 widgets from `docs/widget-roadmap-v1.md` V1 tranche are implemented
  - all new widgets expose consistent style/state override hooks
  - all new widgets are validated against global theme + per-widget overrides
  - widget docs/examples and control catalog updates are merged

### M4: Hardening and Release Candidate
- Acceptance:
  - full test suite green
  - example smoke green
  - benchmark suite produced with baseline report following `docs/perf-plan-v1.md`
  - regression budget gates from `docs/perf-plan-v1.md` pass
  - public docs complete and coherent

## Dependency Graph and Critical Path
1. M1 no-DI + boundary guardrails
2. M2 theme token contract
3. M3 widget delivery on top of M2 style contract
4. M4 hardening + perf + docs freeze

Critical path: M1 -> M2 -> M3 -> M4.

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
