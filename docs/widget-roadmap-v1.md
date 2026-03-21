# TeaSharp Widget Roadmap V1

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
- API shape: explicit C# properties/events; no `TeaSharp.Core.*` leakage in default authoring path

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

- Shipped and integrated: `Form`, `FieldSet`, `Wizard`, `SplitView`, `InspectorPanel`, `EmptyState`, `ValidationSummary`, `SearchResultsView`.
- Theme extension coverage includes all listed controls (`ApplyTheme`, `ApplyThemeDefaults`, override overloads).
- Bordered parity + control-catalog + theme-override tests include bordered Wave 1 controls (`Form`, `FieldSet`, `Wizard`, `SplitView`, `InspectorPanel`).
- Remaining Wave 1 backlog: `DataForm<TModel>`.

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
- Theme extension domain file `TeaThemeControlExtensions.Workspace.cs` is active for Wave 4 workspace/visual-data mappings.
- Remaining Wave 4 backlog: none (`Sparkline` already shipped in plotting baseline).

## Shipped Plotting Baseline (Current V1 Track)

These controls are already shipped on the public path and are not part of the +34 backlog count:

- `Sparkline`
- `AreaPlot`
- `ScatterPlot`
- `Histogram`
- `LinePlot`
- `PlotPanel`

All shipped plotting controls follow the strict theming contract (state styles, glyph hooks where applicable, and override hierarchy).

### Plotting Dashboard Authoring Notes

Use the shipped plotting controls by intent:

- `Sparkline`/`AreaPlot` for bounded streaming single-series telemetry
- `LinePlot` for multi-series trend dashboards
- `ScatterPlot` for correlation
- `Histogram` for distribution buckets
- `PlotPanel` to compose multiple plotting controls into one screen region

Recommended implementation pattern:

- keep bounded buffers for stream inputs
- update existing controls/series in place (avoid rebuilding control trees)
- apply theme defaults, then instance-level visual overrides

Documentation and examples:

- canonical plotting/dashboard sample: `examples/PlottingDashboard` (add when available)
- current stopgap references: `examples/WidgetGallery`, `examples/AdvancedWidgets`

## Visual Pass Timing

- **Phase A (implementation-first):** each widget ships with strict minimal visual contract (state styles + glyph hooks + override hierarchy) and monochrome-safe rendering.
- **Phase B (full visual polish):** after functionality stabilizes, run a dedicated visual pass for spacing, animation cadence, token tuning, and showcase examples.
- Release rule: no widget can skip Phase A; Phase B is scheduled after wave completion and before RC promotion.

## Coordination Notes

- Source-of-truth milestone mapping stays in [v1-master-plan.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/v1-master-plan.md).
- Public API and theme consistency remain aligned with [public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md) and [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md).
