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
2. Implementator reports what was easy, hard, awkward, or ambiguous.
3. Engineering fixes justified issues and adds tests as needed.
4. Implementator retests against the updated API/widget behavior.
5. User reviews the lab manually.
6. Component is marked `Approved` only after all three lanes agree.

## Matrix

| Component | Lab App | Implementator | Engineering | User Review | Final Status | Notes |
|---|---|---|---|---|---|---|
| `TextInput` | `examples/ComponentLabs/TextInputLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `SearchBox` | `examples/ComponentLabs/SearchBoxLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Choice` | `examples/ComponentLabs/ChoiceLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `ComboBox` | `examples/ComponentLabs/ComboBoxLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `TagInput` | `examples/ComponentLabs/TagInputLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `TokenEditor` | `examples/ComponentLabs/TokenEditorLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `AutocompleteInput` | `examples/ComponentLabs/AutocompleteInputLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `ListView<T>` | `examples/ComponentLabs/ListViewLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Table` | `examples/ComponentLabs/TableLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Notifications` | `examples/ComponentLabs/NotificationsLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `NotificationInbox` | `examples/ComponentLabs/NotificationInboxLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `DataForm<T>` | `examples/ComponentLabs/DataFormLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `ValidationSummary` | `examples/ComponentLabs/ValidationSummaryLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Form` / `FieldSet` | `examples/ComponentLabs/FormFieldSetLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Wizard` / `Stepper` | `examples/ComponentLabs/WizardStepperLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Dialog` | `examples/ComponentLabs/DialogLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `CommandPalette` / `QuickOpenOverlay` | `examples/ComponentLabs/OverlayLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `InspectorPanel` | `examples/ComponentLabs/InspectorPanelLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `LinePlot` / `AreaPlot` / `ScatterPlot` / `Histogram` / `Sparkline` | `examples/ComponentLabs/PlottingLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `StatsCard` / `Gauge` / `BulletChart` / `BoxPlot` | `examples/ComponentLabs/KpiLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `Heatmap` / `TreeMapChart` | `examples/ComponentLabs/MatrixVizLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `SplitView` / `ResizablePaneGroup` / `PaneTabs` / `DockWorkspace` | `examples/ComponentLabs/WorkspaceLayoutLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `SideNavRail` / navigation primitives | `examples/ComponentLabs/NavigationLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `CalendarMonthView` / `SchedulerTimeline` / planning widgets | `examples/ComponentLabs/PlanningLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `KanbanBoard` / `QueryBuilder` / workflow boards | `examples/ComponentLabs/WorkflowBoardLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |
| `JsonTreeView` / `SearchResultsView` / `ProcessListView` / `TraceViewer` / `LogTailPanel` | `examples/ComponentLabs/ExplorerOpsLab` | `Not Started` | `Not Started` | `Not Started` | `Not Started` | |

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
