# Tessera Widget Roadmap

This roadmap expands the widget catalog by **+34 additional widgets** in delivery waves, keeping the public C# API cohesive and theme-first.

## Target and Delivery Model

- Target: move toward **40-50 production-grade widgets** with consistent authoring patterns.
- Scope: **docs + implementation waves**, no DI-first requirement on default app path.
- Delivery: each wave must ship with tests, examples, and theme mapping hooks.

## Strict Theming Contract (applies to every new widget)

Each widget in this roadmap must implement the same minimum visual contract from day one:

- required state styles: `Default`, `Hover`, `Focus`, `Pressed/Active`, `Selected`, `Disabled`, `Error`, `ReadOnly` (if editable)
- hover/focus/selected behavior: visually distinct in both color and monochrome terminals; `Focus` must not be hidden by `Hover`
- glyph hooks: widget-specific glyph set properties (no hardcoded glyph literals in render path)
- override layers: global theme -> control type defaults -> instance overrides -> state overrides
- API shape: explicit C# properties/events; no `Tessera.Core.*` namespace leakage in default authoring path

## Delivery Waves (+34 Widgets)

Notation: all rows inherit the strict state-style contract above; `Glyph hooks` column lists required widget-specific glyph seams.

### Wave 1 (P0): App Shell + Forms (9)

| Widget | Priority | C# workflow rationale | Glyph hooks |
| --- | --- | --- | --- |
| `Form` | P0 | Standard CRUD/data-entry scaffolding | section divider, required marker, submit marker |
| `FieldSet` | P0 | Group reusable input blocks | border set, title markers |
| `ValidationSummary` | P0 | Centralized model validation output | severity markers, bullet markers |
| `DataForm<TModel>` | P0 | Strongly-typed model editing | field separators, validation markers |
| `Wizard` | P0 | Multi-step onboarding/config flows | step markers, connector glyphs |
| `SplitView` | P0 | Master/detail and inspector layouts | splitter glyph, collapse marker |
| `InspectorPanel` | P0 | Right-pane metadata/property editing | section expand/collapse glyphs |
| `EmptyState` | P0 | Predictable zero-data UX | icon/marker slot glyph |
| `SearchResultsView` | P0 | Common search result rendering | match markers, rank marker |

Wave 1 integration status (current lane):

- Shipped and integrated: `Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`, `EmptyState`, `ValidationSummary`, `SearchResultsView`.
- Theme extension coverage includes all listed controls (`ApplyTheme`, `ApplyThemeDefaults`, override overloads).
- Bordered parity + control-catalog + theme-override tests include bordered Wave 1 controls (`Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`).
- Remaining Wave 1 backlog: none.

### Wave 2 (P0/P1): Data, Planning, Query (9)

| Widget | Priority | C# workflow rationale | Glyph hooks |
| --- | --- | --- | --- |
| `VirtualizedListView<T>` | P0 | Large collections without UI lag | overflow, continuation markers |
| `GroupedListView<TGroup,TItem>` | P0 | Grouped domain lists | group expand/collapse markers |
| `PivotTable` | P1 | Analytical summaries in-console | sort marker, subtotal marker |
| `QueryBuilder` | P1 | Filter/query composition in tools | operator markers, join markers |
| `KanbanBoard` | P1 | Task-state workflows | lane separators, card markers |
| `CalendarMonthView` | P1 | Scheduling and planning UIs | day markers, current-day marker |
| `SchedulerTimeline` | P1 | Time-slice planning/editor tools | tick markers, range handles |
| `TagInput` | P1 | Label-driven domain modeling | add/remove tag markers |
| `RichTextView` | P1 | Structured text output with emphasis | heading/list/quote markers |

Wave 2 integration status (current lane):

- Implemented and wired: `VirtualizedListView<T>`, `GroupedListView<TGroup,TItem>`, `PivotTable`, `QueryBuilder`, `KanbanBoard`, `CalendarMonthView`, `SchedulerTimeline`, `TagInput`, `RichTextView`.
- Theme extension coverage added for all Wave 2 controls (`ApplyTheme`, `ApplyThemeDefaults`, override overloads).
- Bordered parity + control-catalog + theme-override tests now include all bordered Wave 2 controls.
- Remaining Wave 2 backlog: none.

### Wave 3 (P1): Dev/Ops Workflows (8)

| Widget | Priority | C# workflow rationale | Glyph hooks |
| --- | --- | --- | --- |
| `JsonTreeView` | P1 | API/debug payload inspection | node expand/collapse, type marker |
| `LogTailPanel` | P1 | Streaming logs with filters | level markers, follow marker |
| `TraceViewer` | P1 | Request/operation trace analysis | span markers, timing separators |
| `CommandOutput` | P1 | Deterministic process output panes | prompt marker, continuation marker |
| `TaskRunnerPanel` | P1 | Build/test/deploy dashboards | status markers, progress markers |
| `ActivityFeed` | P1 | Event/audit visualization | event type markers, timestamp marker |
| `NotificationInbox` | P1 | Persistent in-app notifications | unread marker, severity marker |
| `KeyBindingHelpDialog` | P1 | Discoverability for shortcuts | keycap separators, category markers |

Wave 3 integration status (current lane):

- Implemented and wired: `JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`.
- Theme extension coverage added for all Wave 3 controls (`ApplyTheme`, `ApplyThemeDefaults`, override overloads).
- Bordered parity + control-catalog + theme-override tests now include bordered Wave 3 controls (`JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`).
- Remaining Wave 3 backlog: none.

### Wave 4 (P2): Advanced Composition + Visual Data (8)

| Widget | Priority | C# workflow rationale | Glyph hooks |
| --- | --- | --- | --- |
| `DockWorkspace` | P2 | IDE-like pane composition | dock handle glyphs |
| `PaneTabs` | P2 | Multi-pane/multi-view workspaces | tab close/dirty markers |
| `PaletteEditor` | P2 | Theme/palette authoring tools | swatch markers, active marker |
| `Heatmap` | P2 | Dense matrix-based insights | cell markers, legend markers |
| `Sparkline` (baseline shipped) | P2 | Inline trend telemetry | min/max markers |
| `TreeMapChart` | P2 | Hierarchical metric distribution | hierarchy separators |
| `TerminalPanel` | P2 | Embedded subprocess sessions | prompt/stream markers |
| `ProcessListView` | P2 | Runtime process inspection | status markers, sort marker |

Wave 4 integration status (batch A + B):

- Shipped and integrated: `DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`.
- Theme extension domain file `TesseraThemeControlExtensions.Workspace.cs` is active for Wave 4 workspace/visual-data mappings.
- Remaining Wave 4 backlog: none (`Sparkline` already shipped in plotting baseline).

## Shipped Plotting Baseline (Current V1 Track)

These controls are already shipped on the public path and are not part of the +34 backlog count:

- `Sparkline`
- `TelemetryChart`
- `AreaPlot`
- `ScatterPlot`
- `Histogram`
- `LinePlot`
- `PlotPanel`

All shipped plotting controls follow the strict theming contract (state styles, glyph hooks where applicable, and override hierarchy).

### Plotting Dashboard Authoring Notes

Use the shipped plotting controls by intent:

- `Sparkline` for inline single-row trend hints
- `TelemetryChart` for tiny multi-row dashboard telemetry cards (braille-first compact coverage with block/area fallbacks)
- `AreaPlot` for bounded single-series filled plots when the plot region is larger than a telemetry card
- `LinePlot` for multi-series trend dashboards and larger/coarser single-series plots
- `ScatterPlot` for correlation
- `Histogram` for distribution buckets
- `PlotPanel` to compose multiple plotting controls into one screen region

Recommended implementation pattern:

- keep bounded buffers for stream inputs
- update existing controls/series in place (avoid rebuilding control trees)
- apply theme defaults, then instance-level visual overrides

Documentation and examples:

- canonical plotting/dashboard sample: `examples/PlottingDashboard` (add when available)
- current stopgap references: `examples/OpsWatch`, `examples/DownloadCenter`

## Expansion Backlog: +36 Dashboard-First Widgets (10 Landed)

This backlog is intentionally implementation-oriented for the next growth tranche beyond the current +34 wave completion. It keeps dashboard and operational TUI use-cases first.

Dependency keys:

- `D0`: existing V1 control contract (selection/focus/title/border hooks, theme override layers)
- `D1`: shared chart primitives (`Series`, scales, legends, bucket/axis formatters)
- `D2`: shared virtualization/windowing helpers for dense rows/cells
- `D3`: shared overlay stack and focus trapping contract
- `D4`: shared adaptive layout primitives (pane resize, snap, docking rules)
- `D5`: terminal capability-gated image/media path (V1.1 only)

### Data Viz (6)

| Widget | Target | Dashboard use-case | Depends on |
| --- | --- | --- | --- |
| `BulletChart` | V1 | KPI vs target and qualitative ranges | `D0`, `D1` |
| `BoxPlot` | V1 | percentile/distribution inspection | `D0`, `D1` |
| `CandlestickChart` | V1 | market/service interval trend views | `D0`, `D1` |
| `RadarChart` | V1.1 | multivariate profile comparison | `D0`, `D1` |
| `GanttChart` | V1 | timeline execution planning | `D0`, `D1`, `D2` |
| `FunnelChart` | V1 | conversion/drop-off pipelines | `D0`, `D1` |

### Layout and Composition (6)

| Widget | Target | Dashboard use-case | Depends on |
| --- | --- | --- | --- |
| `DashboardGrid` | V1 | drag/reflow card-based metric boards | `D0`, `D4` |
| `CardDeck` | V1 | card stacks for grouped summaries | `D0`, `D4` |
| `ResizablePaneGroup` | V1 | multi-pane operator workspaces | `D0`, `D4` |
| `TileLayoutPanel` | V1 | dense tiled monitoring surfaces | `D0`, `D4` |
| `DrilldownStack` | V1 | master->detail history navigation | `D0`, `D4` |
| `FloatingPanelHost` | V1.1 | detachable inspector/transient panes | `D0`, `D3`, `D4` |

### Inputs and Editors (6)

| Widget | Target | Dashboard use-case | Depends on |
| --- | --- | --- | --- |
| `AutocompleteInput` | V1 | command/filter authoring with suggestions | `D0`, `D3` |
| `TokenEditor` | V1 | structured labels/tags/owners editing | `D0` |
| `PathPicker` | V1 | file/log/config source selection | `D0`, `D3` |
| `CronExpressionInput` | V1 | schedule authoring in automation tools | `D0` |
| `NumericRangeInput` | V1 | bounded thresholds and filter ranges | `D0` |
| `JsonEditor` | V1.1 | structured JSON edit/validate panes | `D0`, `D2` |

### Overlays and Workflow (6)

| Widget | Target | Dashboard use-case | Depends on |
| --- | --- | --- | --- |
| `QuickOpenOverlay` | V1 | fuzzy jump across resources/views | `D0`, `D3` |
| `ActionSheet` | V1 | context-safe command execution menus | `D0`, `D3` |
| `TooltipOverlay` | V1 | dense-help hints for complex UIs | `D0`, `D3` |
| `SpotlightOverlay` | V1 | focused walkthrough/highlight mode | `D0`, `D3` |
| `CommandHistoryOverlay` | V1 | replay/reuse previous commands | `D0`, `D3` |
| `ImagePreviewOverlay` | V1.1 | capability-gated image preview panes | `D0`, `D3`, `D5` |

### Status and Ops (6)

| Widget | Target | Dashboard use-case | Depends on |
| --- | --- | --- | --- |
| `HealthBoard` | V1 | aggregated service health surface | `D0`, `D1` |
| `IncidentTimelinePanel` | V1 | incident events and milestone review | `D0`, `D1` |
| `DeploymentPipelineView` | V1 | stage-gated release visibility | `D0` |
| `AlertRuleTable` | V1 | rule status and suppression controls | `D0`, `D2` |
| `SlaBurnRatePanel` | V1 | SLO/error-budget burn monitoring | `D0`, `D1` |
| `ResourceQuotaPanel` | V1 | quota/limit and headroom tracking | `D0`, `D1` |

### Navigation and Discovery (6)

| Widget | Target | Dashboard use-case | Depends on |
| --- | --- | --- | --- |
| `SideNavRail` | V1 | persistent section navigation | `D0` |
| `WorkspaceSwitcher` | V1 | fast context switching between workspaces | `D0`, `D3` |
| `OutlineNavigator` | V1 | tree-like structural navigation | `D0`, `D2` |
| `JumpList` | V1 | MRU and pinned target navigation | `D0` |
| `RecentItemsNavigator` | V1 | recency-based productivity flow | `D0` |
| `KeymapCheatSheetPanel` | V1 | always-available shortcut discoverability | `D0` |

### Expansion Tranche Status (March 22, 2026)

Landed controls from the expansion tranche:

| Control | Landed commit(s) | Current verification signal |
| --- | --- | --- |
| `DashboardGrid` | `8593562`, `4e005ed` | implemented; deterministic render/interaction tests present in `DashboardGridControlTests`; included in passing targeted suite |
| `QuickOpenOverlay` | `d91c934`, `db63e01` | implemented; keyboard/pointer/submit + deterministic render tests present in `QuickOpenOverlayControlTests`; typed theme/parity wiring landed; included in passing targeted suite |
| `BulletChart` | `dfd4221`, `c681b64`, `4e005ed` | implemented; style-focused test stabilization landed; included in passing targeted suite |
| `ResizablePaneGroup` | `77cc95d`, `1c1b748` | implemented; selection/resize/style + deterministic render tests present; theme/parity wiring restored; included in passing targeted suite |
| `SideNavRail` | `d236de2`, `1c1b748` | implemented; keyboard/pointer/activation/style tests present; theme/parity wiring restored; included in passing targeted suite |
| `TokenEditor` | `7fbf7be`, `1c1b748` | implemented; add/remove/navigation/style tests present; theme/parity wiring restored; currently passing in targeted suite reruns |
| `HealthBoard` | `18adc16`, `1c1b748` | implemented; severity/selection/ack/style tests present; theme/parity wiring restored; included in passing targeted suite |
| `JumpList` | `187468c`, `03c7a43` | implemented; activation/navigation/style tests present in `JumpListControlTests`; included in passing targeted suite |
| `AutocompleteInput` | `50212de`, `be23de7`, `03c7a43` | implemented; suggestion ranking/commit/style behavior stabilized; included in passing targeted suite |
| `BoxPlot` | `6ae3c5b`, `03c7a43` | implemented; deterministic render/style tests present in `BoxPlotControlTests`; included in passing targeted suite |

Targeted verification command (current host):  
`dotnet test tests/Tessera.Tests --no-restore --nologo --filter "DashboardGrid|QuickOpenOverlay|BulletChart|ResizablePaneGroup|SideNavRail|TokenEditor|HealthBoard|JumpList|AutocompleteInput|BoxPlot"` -> pass (48/48).

Outstanding for this tranche (do not mark done yet):

- Theme extension parity is confirmed for all currently landed tranche controls (`4e005ed`, `1c1b748`, `03c7a43`, `db63e01`).
- Remaining tranche scope is unimplemented backlog controls from the expansion list (not parity gaps on landed controls).

## Dependency Graph (Build Order)

1. `D0` and `D4` first (API/style parity + layout substrate). This unlocks `DashboardGrid`, `ResizablePaneGroup`, `TileLayoutPanel`, `SideNavRail`.
2. `D3` second (overlay host contracts). This unlocks `QuickOpenOverlay`, `ActionSheet`, `WorkspaceSwitcher`, `AutocompleteInput`.
3. `D1` third (shared chart primitives). This unlocks `BulletChart`, `BoxPlot`, `FunnelChart`, `HealthBoard`, `SlaBurnRatePanel`.
4. `D2` fourth (dense virtualization helpers). This unlocks `GanttChart`, `AlertRuleTable`, `OutlineNavigator`, and V1.1 `JsonEditor`.
5. `D5` last and V1.1-only. This unlocks `ImagePreviewOverlay` and any future image-capable widgets.

## Top 12 Widgets: Minimal Public API Sketch + Style Contract

| Widget | Minimal C# API sketch | Required style/theming hooks |
| --- | --- | --- |
| `DashboardGrid` | `SetTiles(IEnumerable<DashboardTile>)`, `MoveTile(string tileId, int row, int column)`, `ResizeTile(string tileId, int rowSpan, int columnSpan)`, `SelectedTileId`, `SelectionChanged` | tile border/title style hooks, selected tile style, drag-preview style, focus marker support |
| `ResizablePaneGroup` | `SetPanes(IEnumerable<PaneSpec>)`, `SetSplitRatio(int paneIndex, double ratio)`, `SelectedPaneIndex`, `SelectionChanged` | splitter style text, focused splitter style, pane title/focus marker hooks |
| `SideNavRail` | `SetItems(IEnumerable<NavItem>)`, `SetSelectedIndex(int index)`, `SelectedItem`, `SelectionChanged`, `Activated` | selected/hover/focus item styles, collapse marker glyphs, badge marker glyph |
| `WorkspaceSwitcher` | `SetWorkspaces(IEnumerable<WorkspaceItem>)`, `Open()`, `Close()`, `SetSelectedIndex(int index)`, `Submitted` | overlay border hooks, selected row style, search-hit marker glyph, dim-backdrop style |
| `QuickOpenOverlay` | `SetItems(IEnumerable<QuickOpenItem>)`, `SetQuery(string query)`, `SetSelectedIndex(int index)`, `Submitted`, `Cancelled` | query input style hooks, row state styles, match marker glyphs, focused border hooks |
| `AutocompleteInput` | `Text`, `SetSuggestions(IEnumerable<string>)`, `SetSelectedSuggestionIndex(int index)`, `SuggestionCommitted` | input/title/focus styles, popup border hooks, highlighted suggestion style, suggestion marker glyph |
| `TokenEditor` | `SetTokens(IEnumerable<TokenItem>)`, `AddToken(string text)`, `RemoveSelectedToken()`, `SelectedTokenIndex`, `SelectionChanged` | token chip default/selected/error styles, add/remove marker glyphs, focus marker support |
| `BulletChart` | `Title`, `SetRanges(IEnumerable<BulletRange>)`, `SetValue(double value)`, `SetTarget(double target)` | range-segment styles, value bar style, target marker glyph/style, threshold warning styles |
| `BoxPlot` | `Title`, `SetSeries(IEnumerable<BoxPlotSeries>)`, `SetSelectedSeries(int index)`, `SelectionChanged` | quartile/median/whisker styles, selected series style, axis/legend text hooks |
| `GanttChart` | `SetTasks(IEnumerable<GanttTask>)`, `SetViewport(DateOnly start, DateOnly end)`, `SetSelectedIndex(int index)`, `SelectionChanged` | bar styles by status, dependency marker glyphs, today marker style, focused border hooks |
| `HealthBoard` | `SetServices(IEnumerable<HealthService>)`, `SetSelectedIndex(int index)`, `SelectionChanged`, `Acknowledge(string serviceId)` | status-severity styles, degraded/outage glyph hooks, selected row style, muted acknowledged style |
| `DeploymentPipelineView` | `SetStages(IEnumerable<PipelineStage>)`, `SetSelectedStageIndex(int index)`, `SelectionChanged`, `RetrySelected()` | stage state styles, connector glyph hooks, selected stage style, running animation marker text |

Global style contract for every widget above:

- must support state styles: `Default`, `Hover`, `Focus`, `Selected`, `Disabled`
- bordered controls must expose `BorderStyleText` + `FocusedBorderStyleText`
- title-bearing controls must expose `FocusMarker` + `ShowFocusMarker`
- all marker literals must be typed glyph-set properties (no hardcoded render literals)

## Implementation Sequencing (Parallel Agent Lanes)

Phase 1 (foundation, 2 weeks):

- Lane A (layout/nav): `DashboardGrid`, `ResizablePaneGroup`, `SideNavRail`, `JumpList`
- Lane B (overlays/input): `QuickOpenOverlay`, `AutocompleteInput`, `TokenEditor`
- Lane C (data viz/ops): `BulletChart`, `BoxPlot`, `HealthBoard`

Phase 2 (breadth, 2-3 weeks):

- Lane A: `TileLayoutPanel`, `DrilldownStack`, `WorkspaceSwitcher`, `OutlineNavigator`
- Lane B: `ActionSheet`, `TooltipOverlay`, `CommandHistoryOverlay`, `PathPicker`, `NumericRangeInput`, `CronExpressionInput`
- Lane C: `FunnelChart`, `GanttChart`, `IncidentTimelinePanel`, `DeploymentPipelineView`, `AlertRuleTable`, `SlaBurnRatePanel`, `ResourceQuotaPanel`

Phase 3 (V1.1 capability tranche):

- Lane A: `FloatingPanelHost`
- Lane B: `JsonEditor`, `ImagePreviewOverlay`
- Lane C: `RadarChart` and other higher-density chart variants gated by terminal capability + perf budget

Execution rules:

- each widget slice must land with tests + theme mapping + deterministic interaction coverage
- each phase ends with benchmark checks for render-only and render+materialize hot paths
- image-capable widgets remain strictly V1.1 and capability-gated

## Visual Pass Timing

- **Phase A (implementation-first):** each widget ships with strict minimal visual contract (state styles + glyph hooks + override hierarchy) and monochrome-safe rendering.
- **Phase B (full visual polish):** after functionality stabilizes, run a dedicated visual pass for spacing, animation cadence, token tuning, and showcase examples.
- Release rule: no widget can skip Phase A; Phase B is scheduled after wave completion and before RC promotion.

## Coordination Notes

- Source-of-truth milestone mapping stays in [alpha-release-checklist](/docs/alpha-release-checklist).
- Public API and theme consistency remain aligned with [public-api-inventory](/docs/public-api-inventory) and [theme-system](/docs/theme-system).
