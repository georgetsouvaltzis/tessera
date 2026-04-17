---
sidebar_label: Controls Overview
---

# Controls Overview

Tessera ships a broad control surface, but end-users should learn it by problem domain instead of by one giant inventory list.

Use this page as the bridge between the starter examples and the full [public-api-inventory.md](public-api-inventory.md).

## Start with the controls you will actually use first

For most apps, learn these families in roughly this order:

1. buttons, labels, status bars, tabs
2. text input, choice, combo box, sliders, toggles
3. list and tree navigation
4. table/grid data surfaces
5. overlays and command surfaces
6. charts and dashboard widgets

## Inputs and forms

Use these when the app collects or edits data:

- `TextInput`
- `TextArea`
- `NumberInput`
- `Choice`
- `ComboBox`
- `DatePicker`
- `TimePicker`
- `RadioGroup`
- `Toggle`
- `Slider`
- `DataForm<TModel>`
- `Form`
- `FieldSet`
- `ValidationSummary`

Best example to see them in context:

- [examples.md](examples.md) -> `CounterForm`

## Navigation and workflow

Use these when the user moves through commands, sections, or records:

- `Button`
- `Tabs`
- `Breadcrumb`
- `SideNavRail`
- `JumpList`
- `CommandPalette`
- `QuickOpenOverlay`
- `MenuBar`
- `ContextMenu`
- `SearchBox`
- `SearchResultsView`
- `TreeView`
- `ListView<T>`

Best examples:

- `WorkspaceApp`
- `GitConsole`

## Data surfaces

Use these when the UI is record-heavy or inspection-heavy:

- `Table`
- `DataGrid`
- `TreeTable`
- `KeyValueList`
- `Timeline`
- `MarkdownView`
- `LogView`
- `LogTailPanel`
- `TraceViewer`
- `CommandOutput`
- `ActivityFeed`

Best examples:

- `DataWorkbench`
- `GitConsole`
- `OpsWatch`

## Dashboards and plotting

Use these when the surface needs metrics, telemetry, or compact visuals:

- `StatsCard`
- `Gauge`
- `ProgressBar`
- `BarChart`
- `LineChart`
- `Sparkline`
- `TelemetryChart`
- `Histogram`
- `LinePlot`
- `PlotPanel`
- `Heatmap`
- `TreeMapChart`
- `HealthBoard`

Best examples:

- `OpsWatch`
- `DataWorkbench`

## Composition and overlays

Use these when the app is turning into a shell or workspace:

- `SplitView`
- `ResizablePaneGroup`
- `DockWorkspace`
- `PaneTabs`
- `InspectorPanel`
- `Dialog`
- `Modal`
- `Notifications`

Best examples:

- `WorkspaceApp`
- `DataWorkbench`
- `GitConsole`

## How to choose a control

Ask these questions in order:

1. Is the user editing, navigating, or inspecting?
2. Is the surface single-pane or multi-pane?
3. Does the control need to be compact, data-dense, or high-visibility?
4. Will it need theme overrides or state styling?

If you still need the exact type name after that, open [public-api-inventory.md](public-api-inventory.md).

## What this page does not replace

This page is for discovery. Use the reference pages when you need:

- exact public type names
- naming policy
- terminal-specific caveats
- theming hook details

For those, use:

- [api-reference.mdx](api-reference.mdx)
- [public-api-guidelines.md](public-api-guidelines.md)
- [public-api-inventory.md](public-api-inventory.md)
- [theme-system.md](theme-system.md)
