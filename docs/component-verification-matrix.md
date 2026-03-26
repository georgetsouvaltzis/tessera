# Component Verification Matrix

This document is the source of truth for component-by-component manual verification, API ergonomics review, engineering follow-up, and final approval.

Status legend:
- `Not Started`
- `In Progress`
- `Ready To Test`
- `Needs Fix`
- `Approved`

Review gates:
- `Implementator`: external-consumer pass complete
- `Engineering`: required API/widget/test work complete
- `User Review`: manual visual/interaction review complete
- `Final Status`: component is accepted for the current V1 target

## Workflow

For each component or tightly-related component family:

1. Implementator builds a minimal lab app using only the public API.
   - one widget per app
   - lab path format: `examples/<WidgetName>`
2. Implementator reports what was easy, hard, awkward, or ambiguous.
3. Implementator provides exact manual test steps for the user.
4. Engineering fixes justified issues and adds tests as needed.
5. Implementator retests against the updated API/widget behavior.
6. User reviews the lab manually.
7. Component is marked `Approved` only after all three lanes agree.

## Coordinator Lanes

- `Human Lane`: builds or audits minimal labs under `examples/<WidgetName>` using only `Tea`, `TeaSharp.Controls`, `TeaSharp.Layout`, and `TeaSharp.Styles`; reports easiest path, hardest path, and any ambiguous API.
- `Owner Lane A - Input + Selection`: `TextInput`, `SearchBox`, `Choice`, `ComboBox`, `TagInput`, `TokenEditor`, `AutocompleteInput`, `CommandPalette`, `QuickOpenOverlay`, `JumpList`.
- `Owner Lane B - Data + Explorer`: `ListView<T>`, `Table`, `NotificationInbox`, `CalendarMonthView`, `SchedulerTimeline`, `KanbanBoard`, `QueryBuilder`, `JsonTreeView`, `SearchResultsView`, `ProcessListView`, `TraceViewer`, `LogTailPanel`.
- `Owner Lane C - Shell + Workflow + Visuals`: `Notifications`, `DataForm<T>`, `ValidationSummary`, `Form`, `FieldSet`, `Wizard`, `Stepper`, `Dialog`, `InspectorPanel`, plotting widgets, KPI widgets, matrix/data-viz widgets, workspace layout widgets, and navigation/layout composition widgets.

## Lab Layout Rule

- Final lab path shape is `examples/<WidgetName>`.
- One widget per lab app. If the backlog still shows grouped placeholder rows, split them before implementation starts.
- Implementator handoff is incomplete without a short manual test checklist for the user.

## Current Loop

- Active component: `DataForm<T>`
- Human lab target: `examples/DataForm`
- Owner lane: `Owner Lane C - Shell + Workflow + Visuals`
- Round goal: land the strict widget-only `DataForm` lab, verify public-api ergonomics, and prepare manual test steps for visual review

## Matrix

| Component | Owner Lane | Minimal Lab Idea | Lab App | Implementator | Engineering | User Review | Final Status | Notes |
|---|---|---|---|---|---|---|---|---|
| `TextInput` | `Owner Lane A` | Single-field command box with submit/cancel and validation hint. | `examples/TextInput` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `SearchBox` | `Owner Lane A` | Search bar driving a tiny result count/status panel. | `examples/SearchBox` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Choice` | `Owner Lane A` | Environment selector with keyboard + pointer open/select flow. | `examples/Choice` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `ComboBox` | `Owner Lane A` | Region picker with type-to-filter and explicit selection summary. | `examples/ComboBox` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `TagInput` | `Owner Lane A` | Incident label editor: add, dedupe, cap, remove, pointer select, style toggle. | `examples/TagInput` | `Approved` | `Approved` | `Not Started` | `Ready To Test` | Footer steps now explicitly prove `SetTags`/`AddTag`/`RemoveTagAt`, wrapped vertical growth, and pointer selection on wrapped rows. |
| `TokenEditor` | `Owner Lane A` | Assignee/chip editor with selection and token mutation feed. | `examples/TokenEditor` | `Approved` | `Approved` | `Not Started` | `Ready To Test` | Widget-only lab landed; footer proves typing, selection, delete, glyph/style seam, disabled-token styling, and `SetTokens`/`AddToken`/`RemoveSelectedToken`. |
| `AutocompleteInput` | `Owner Lane A` | Command/search suggestion bar with commit and fallback typing. | `examples/AutocompleteInput` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `ListView<T>` | `Owner Lane B` | Simple queue list with programmatic reselect and detail pane. | `examples/ListView` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Table` | `Owner Lane B` | Service table with row selection, mutation, and drilldown summary. | `examples/Table` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Notifications` | `Owner Lane C` | Live toast/feed center with selection and remove/clear actions. | `examples/Notifications` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `NotificationInbox` | `Owner Lane B` | Persistent inbox with unread/severity state and selection review. | `examples/NotificationInbox` | `Approved` | `Not Started` | `Not Started` | `In Progress` | Widget-only first pass landed; footer proves select/read/pin/delete flows and `SetItems`/`Add`/`Select`/`MarkAllRead`/`Clear`. |
| `DataForm<T>` | `Owner Lane C` | Small settings editor with keyed field selection and validation state. | `examples/DataForm` | `Approved` | `Not Started` | `Not Started` | `In Progress` | Widget-only first pass landed; footer proves bind/clear model, keyed field selection, validation failure, commit, and read-only field behavior. |
| `ValidationSummary` | `Owner Lane C` | Compact error stack fed by a fake save attempt. | `examples/ValidationSummary` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Form` / `FieldSet` | `Owner Lane C` | Bordered account form with grouped sections and submit footer. | `examples/ComponentLabs/FormFieldSetLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Wizard` / `Stepper` | `Owner Lane C` | Three-step setup flow with next/back and status recap. | `examples/ComponentLabs/WizardStepperLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Dialog` | `Owner Lane C` | Confirm/cancel destructive action with typed close result readback. | `examples/ComponentLabs/DialogLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `CommandPalette` / `QuickOpenOverlay` | `Owner Lane A` | Quick-jump launcher over 8-10 fake resources with query filter. | `examples/ComponentLabs/OverlayLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `InspectorPanel` | `Owner Lane C` | Read-only entity inspector with collapsible sections. | `examples/ComponentLabs/InspectorPanelLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `LinePlot` / `AreaPlot` / `ScatterPlot` / `Histogram` / `Sparkline` | `Owner Lane C` | Tiny telemetry dashboard with bounded sample retention and scale labels. | `examples/ComponentLabs/PlottingLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `StatsCard` / `Gauge` / `BulletChart` / `BoxPlot` | `Owner Lane C` | KPI board with target vs actual and distribution snapshot. | `examples/ComponentLabs/KpiLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Heatmap` / `TreeMapChart` | `Owner Lane C` | Dense utilization board with legend and focused cell/node summary. | `examples/ComponentLabs/MatrixVizLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `SplitView` / `ResizablePaneGroup` / `PaneTabs` / `DockWorkspace` | `Owner Lane C` | Mini operator workspace with nav/detail/log panes and resize flow. | `examples/ComponentLabs/WorkspaceLayoutLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `SideNavRail` / navigation primitives | `Owner Lane C` | Left-nav shell switching between 3 small content panels. | `examples/ComponentLabs/NavigationLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `CalendarMonthView` / `SchedulerTimeline` / planning widgets | `Owner Lane B` | On-call planner with date select and timeline range summary. | `examples/ComponentLabs/PlanningLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `KanbanBoard` / `QueryBuilder` / workflow boards | `Owner Lane B` | Ticket triage board with filter rule builder. | `examples/ComponentLabs/WorkflowBoardLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `JsonTreeView` / `SearchResultsView` / `ProcessListView` / `TraceViewer` / `LogTailPanel` | `Owner Lane B` | Ops explorer reading fake process/log/trace payloads side by side. | `examples/ComponentLabs/ExplorerOpsLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |

## Current Order

Current priority order for lab creation:

1. `TagInput`
2. `TokenEditor`
3. `NotificationInbox`
4. `DataForm<T>`
5. `Choice`
6. `ComboBox`
7. `Table`

This order should evolve based on implementator feedback and user review outcomes.
