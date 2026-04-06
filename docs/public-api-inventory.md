# TeaSharp Public API Inventory

## Purpose

This document tracks the public API tiers so the pre-release redesign stays deliberate.

## Freeze Status

The current working freeze line is:

- Tier 1 is the supported default consumer path
- Tier 2 is the supported advanced escape-hatch path
- anything outside those tiers is still a candidate for internalization or deletion before first public release

Examples, README guidance, and starter docs should teach Tier 1 first. Tier 2 may remain public, but it should not be the default onboarding story.

## Tier 1: Default Consumer Path

These are the types new applications should discover first.

- `Tea`
- `TeaApp`
- `TeaApplication`
- `TeaApplicationBuilder`
- `TeaRuntimeOptions`
- `TeaEffect`
- `TeaEffects`
- `TeaEffects.Periodic(...)` for auto-rescheduling interval updates
- `Message` and the typed message records in `TeaSharp`
- `TeaSharp.Styles.TeaStyle`
- `TeaSharp.Styles.AnsiColor`
- `TeaSharp.Styles.TeaFontWeight`
- `TeaSharp.Styles.TeaThemeOverrideBundle`
- `TeaSharp.Styles.TeaThemeOverrideBundleExtensions`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Screen.Build(...)`
- `TeaSharp.Layout.*` object-model types
- `TeaSharp.Controls.Button`
- `TeaSharp.Controls.Breadcrumb`
- `TeaSharp.Controls.Label`
- `TeaSharp.Controls.Badge`
- `TeaSharp.Controls.BadgeTone`
- `TeaSharp.Controls.Accordion`
- `TeaSharp.Controls.AccordionSection`
- `TeaSharp.Controls.TextInput`
- `TeaSharp.Controls.TextArea`
- `TeaSharp.Controls.Choice`
- `TeaSharp.Controls.ComboBox`
- `TeaSharp.Controls.DropdownGlyphSet`
- `TeaSharp.Controls.CommandPalette`
- `TeaSharp.Controls.CommandPaletteItem`
- `TeaSharp.Controls.CommandPaletteGlyphSet`
- `TeaSharp.Controls.Dialog`
- `TeaSharp.Controls.DialogResult`
- `TeaSharp.Controls.DialogClosedEventArgs`
- `TeaSharp.Controls.ContextMenu`
- `TeaSharp.Controls.ContextMenuItem`
- `TeaSharp.Controls.ContextMenuGlyphSet`
- `TeaSharp.Controls.ProgressBar`
- `TeaSharp.Controls.BarPoint`
- `TeaSharp.Controls.BarChart`
- `TeaSharp.Controls.LineChart`
- `TeaSharp.Controls.Sparkline`
- `TeaSharp.Controls.TelemetryChart`
- `TeaSharp.Controls.TelemetryChartOptions`
- `TeaSharp.Controls.TelemetryChartRenderMode`
- `TeaSharp.Controls.AreaPlot`
- `TeaSharp.Controls.ScatterPlotPoint`
- `TeaSharp.Controls.ScatterPlot`
- `TeaSharp.Controls.HistogramBucket`
- `TeaSharp.Controls.Histogram`
- `TeaSharp.Controls.LineSeries`
- `TeaSharp.Controls.LinePlot`
- `TeaSharp.Controls.PlotPanel`
- `TeaSharp.Controls.BulletChart`
- `TeaSharp.Controls.BulletRange`
- `TeaSharp.Controls.BulletRangeKind`
- `TeaSharp.Controls.DashboardGrid`
- `TeaSharp.Controls.DashboardTile`
- `TeaSharp.Controls.QuickOpenOverlay`
- `TeaSharp.Controls.QuickOpenItem`
- `TeaSharp.Controls.QuickOpenOverlayGlyphSet`
- `TeaSharp.Controls.QuickOpenOverlaySubmittedEventArgs`
- `TeaSharp.Controls.ResizablePaneGroup`
- `TeaSharp.Controls.PaneSpec`
- `TeaSharp.Controls.SideNavRail`
- `TeaSharp.Controls.NavItem`
- `TeaSharp.Controls.SideNavRailGlyphSet`
- `TeaSharp.Controls.SideNavRailSelectionChangedEventArgs`
- `TeaSharp.Controls.SideNavRailActivatedEventArgs`
- `TeaSharp.Controls.TokenEditor`
- `TeaSharp.Controls.TokenItem`
- `TeaSharp.Controls.TokenEditorGlyphSet`
- `TeaSharp.Controls.TokenEditorSelectionChangedEventArgs`
- `TeaSharp.Controls.HealthBoard`
- `TeaSharp.Controls.HealthService`
- `TeaSharp.Controls.HealthServiceSeverity`
- `TeaSharp.Controls.HealthBoardGlyphSet`
- `TeaSharp.Controls.JumpList`
- `TeaSharp.Controls.JumpListItem`
- `TeaSharp.Controls.JumpListGlyphSet`
- `TeaSharp.Controls.JumpListActivatedEventArgs`
- `TeaSharp.Controls.AutocompleteInput`
- `TeaSharp.Controls.AutocompleteInputGlyphSet`
- `TeaSharp.Controls.AutocompleteInputSuggestionCommittedEventArgs`
- `TeaSharp.Controls.BoxPlot`
- `TeaSharp.Controls.BoxPlotSeries`
- `TeaSharp.Controls.Gauge`
- `TeaSharp.Controls.MiniLog`
- `TeaSharp.Controls.StatItem`
- `TeaSharp.Controls.StatsCard`
- `TeaSharp.Controls.NumberInput`
- `TeaSharp.Controls.DatePicker`
- `TeaSharp.Controls.TimePicker`
- `TeaSharp.Controls.MarkdownView`
- `TeaSharp.Controls.MultiSelect`
- `TeaSharp.Controls.Paginator`
- `TeaSharp.Controls.RadioGroup`
- `TeaSharp.Controls.LogView`
- `TeaSharp.Controls.Modal`
- `TeaSharp.Controls.Notifications` (primary notification feed API)
- `TeaSharp.Controls.Toggle`
- `TeaSharp.Controls.Slider`
- `TeaSharp.Controls.Spinner`
- `TeaSharp.Controls.StatusBar`
- `TeaSharp.Controls.Tabs`
- `TeaSharp.Controls.ListView<T>`
- `TeaSharp.Controls.VirtualizedListView<T>`
- `TeaSharp.Controls.VirtualizedListViewOptions`
- `TeaSharp.Controls.GroupedListView<TGroup,TItem>`
- `TeaSharp.Controls.GroupedListViewGroup<TGroup,TItem>`
- `TeaSharp.Controls.GroupedListSelectionChangedEventArgs<TGroup,TItem>`
- `TeaSharp.Controls.Table`
- `TeaSharp.Controls.KanbanBoard`
- `TeaSharp.Controls.KanbanLane`
- `TeaSharp.Controls.KanbanCard`
- `TeaSharp.Controls.KanbanSelectionChangedEventArgs`
- `TeaSharp.Controls.TagInput`
- `TeaSharp.Controls.CalendarMonthView`
- `TeaSharp.Controls.CalendarDayCell`
- `TeaSharp.Controls.CalendarDateSelectedEventArgs`
- `TeaSharp.Controls.SchedulerTimeline`
- `TeaSharp.Controls.SchedulerEntry`
- `TeaSharp.Controls.SchedulerSelectionChangedEventArgs`
- `TeaSharp.Controls.PivotTable`
- `TeaSharp.Controls.PivotTableColumn`
- `TeaSharp.Controls.PivotTableCell`
- `TeaSharp.Controls.PivotSortDirection`
- `TeaSharp.Controls.PivotSortRequestedEventArgs`
- `TeaSharp.Controls.QueryBuilder`
- `TeaSharp.Controls.QueryGroup`
- `TeaSharp.Controls.QueryRule`
- `TeaSharp.Controls.QueryOperator`
- `TeaSharp.Controls.QueryChangedEventArgs`
- `TeaSharp.Controls.RichTextView`
- `TeaSharp.Controls.RichTextSegment`
- `TeaSharp.Controls.RichTextStyleKind`
- `TeaSharp.Controls.JsonTreeView`
- `TeaSharp.Controls.JsonTreeNode`
- `TeaSharp.Controls.JsonTreeNodeKind`
- `TeaSharp.Controls.JsonTreeSelectionChangedEventArgs`
- `TeaSharp.Controls.TraceViewer`
- `TeaSharp.Controls.TraceEntry`
- `TeaSharp.Controls.TraceSelectionChangedEventArgs`
- `TeaSharp.Controls.CommandOutput`
- `TeaSharp.Controls.CommandOutputLine`
- `TeaSharp.Controls.CommandOutputChannel`
- `TeaSharp.Controls.LogTailPanel`
- `TeaSharp.Controls.LogEntry`
- `TeaSharp.Controls.LogLevel`
- `TeaSharp.Controls.TaskRunnerPanel`
- `TeaSharp.Controls.TaskRunItem`
- `TeaSharp.Controls.TaskRunnerSelectionChangedEventArgs`
- `TeaSharp.Controls.DockWorkspace`
- `TeaSharp.Controls.DockPane`
- `TeaSharp.Controls.DockPanePosition`
- `TeaSharp.Controls.PaneTabs`
- `TeaSharp.Controls.PaneTabItem`
- `TeaSharp.Controls.PaneTabSelectionChangedEventArgs`
- `TeaSharp.Controls.PaletteEditor`
- `TeaSharp.Controls.PaletteSwatch`
- `TeaSharp.Controls.PaletteSelectionChangedEventArgs`
- `TeaSharp.Controls.Heatmap`
- `TeaSharp.Controls.HeatmapCell`
- `TeaSharp.Controls.HeatmapLegend`
- `TeaSharp.Controls.TreeMapChart`
- `TeaSharp.Controls.TreeMapNode`
- `TeaSharp.Controls.TerminalPanel`
- `TeaSharp.Controls.TerminalPanelLine`
- `TeaSharp.Controls.TerminalPanelChannel`
- `TeaSharp.Controls.ProcessListView`
- `TeaSharp.Controls.ProcessListEntry`
- `TeaSharp.Controls.ProcessListStatus`
- `TeaSharp.Controls.ProcessListSelectionChangedEventArgs`
- `TeaSharp.Controls.ActivityFeed`
- `TeaSharp.Controls.ActivityFeedItem`
- `TeaSharp.Controls.ActivityFeedItemKind`
- `TeaSharp.Controls.InboxItem`
- `TeaSharp.Controls.KeyBindingHelpDialog`
- `TeaSharp.Controls.KeyBindingItem`
- `TeaSharp.Controls.DataGrid`
- `TeaSharp.Controls.TreeTable`
- `TeaSharp.Controls.KeyValueList`
- `TeaSharp.Controls.Timeline`
- `TeaSharp.Controls.Stepper`
- `TeaSharp.Controls.TreeItem`
- `TeaSharp.Controls.TreeView` (native)
- `TeaSharp.Controls.TreeViewGlyphSet`
- `TeaSharp.Controls.MenuBar`
- `TeaSharp.Controls.MenuBarGlyphSet`
- `TeaSharp.Controls.Toolbar`
- `TeaSharp.Controls.CommandBar`
- `TeaSharp.Controls.SearchBox`
- `TeaSharp.Controls.SearchResultsView`
- `TeaSharp.Controls.SearchResultsGlyphSet`
- `TeaSharp.Controls.Form`
- `TeaSharp.Controls.FormField`
- `TeaSharp.Controls.DataForm<TModel>`
- `TeaSharp.Controls.DataFormField<TModel>`
- `TeaSharp.Controls.DataFormSelectionChangedEventArgs<TModel>`
- `TeaSharp.Controls.DataFormFieldCommittedEventArgs<TModel>`
- `TeaSharp.Controls.FieldSet`
- `TeaSharp.Controls.SplitView`
- `TeaSharp.Controls.SplitViewOrientation`
- `TeaSharp.Controls.InspectorPanel`
- `TeaSharp.Controls.InspectorSection`
- `TeaSharp.Controls.InspectorField`
- `TeaSharp.Controls.Wizard`
- `TeaSharp.Controls.WizardStep`
- `TeaSharp.Controls.WizardStepChangedEventArgs`
- `TeaSharp.Controls.EmptyState`
- `TeaSharp.Controls.ValidationSummary`
- `TeaSharp.Controls.DiffView`
- `TeaSharp.Controls.PropertyGrid`
- `TeaSharp.Controls.FileExplorer`
- `TeaSharp.Controls.FuzzyFinder`
- `TeaSharp.Controls.ToastCenter`
- `TeaSharp.Controls.MenuItem`
- `TeaSharp.Controls.Control`

The intended beginner path is:

- build an app by deriving from `TeaApp`
- run it with the minimal startup lane (`Tea.RunAsync(new App())`) or configured startup lane (`Tea.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`)
- rely on automatic control routing; `Update(...)` handles only unhandled input plus runtime messages
- return `Screen` from `Build(ScreenContext)`
- assemble screens with `Screen.Build(...)` and shallow builder callbacks
- keep configuration in `TeaRuntimeOptions` and `ScreenOptions`
- follow canonical onboarding examples in order: `examples/HelloWorld` -> `examples/CounterForm` -> `examples/WorkspaceApp`
- treat `TeaSharp.Core` as the low-level advanced lane, not default onboarding
- use semantic theme tokens and palette-driven styling on the default path

### Pointer Runtime Semantics (Tier 1)

- `PointerActivationPolicy` controls click activation gating:
  - `DoubleClick` (default): first press transfers focus, activation requires a qualifying second press.
  - `SingleClick`: first press focuses and activates.
- hover (`PointerEventKind.Motion`) is visual-only and should not be treated as click activation.
- pointer terminal transport is independent from policy:
  - policy controls activation semantics.
  - `ScreenOptions.MouseTracking` controls requested terminal mouse reporting mode.

Runtime input-path contract:

- runtime keeps terminal byte-stream decoding when capabilities advertise CSI input features (`MouseReporting`, `FocusReporting`, `BracketedPaste`, or `ModeReports`), including non-raw console mode.
- `Console.ReadKey` fallback is reserved for non-raw legacy terminals without CSI input features.

Terminal prerequisites and troubleshooting:

- CSI-capable terminals (Ghostty, iTerm2, Windows Terminal, macOS Terminal) should run through byte-stream decoding for pointer/focus/paste.
- verify app requests mouse reporting (`runtime.Screen.MouseTracking = CellMotion|AllMotion`).
- verify `DisableInput` is false.
- verify `TEASHARP_CAPS` is not disabling mouse (`mouse=0`).
- if using tmux, enable `set -g mouse on`.
- if terminal text selection appears instead of app pointer behavior, run through the checklist above first.

### Notification Surface Guidance (Tier 1)

- primary onboarding path: `TeaSharp.Controls.Notifications`
- advanced/devops path: `TeaSharp.Controls.NotificationInbox` (Tier 2)
- shared item model: `TeaSharp.Controls.InboxItem`

Current `Notifications` primary API surface:

- `Items`
- `SelectedIndex`
- `SelectedItem`
- `SelectionChanged`
- `SetItems(IEnumerable<InboxItem>)`
- `Add(InboxItem)`
- `SetSelectedIndex(int)`
- `Select(int)` (compatibility wrapper)
- `MarkAllRead()`
- `RemoveSelected()`
- `Push(...)` remains supported and forwards to `Add(...)`

### Selection Naming Policy (Tier 1)

Canonical selection naming for docs/examples is `Selected*`:

- `SelectedIndex`
- `SelectedItem` (or domain-specific `SelectedNode`, `SelectedProperty`, etc.)
- `SelectionChanged`

Compatibility names such as `Current*` remain supported in V1 where already present, but they are compatibility aliases only. New docs, examples, and additive APIs should use `Selected*` names.

### Selection Ergonomics Additions (Tier 1)

Additive selection APIs now available on the default path:

- `Table.SetSelectedIndex(int)`
- `Choice.SetSelectedIndex(int)`
- `Choice.TrySetSelectedItem(string)`
- `ComboBox.SetSelectedIndex(int)`
- `ComboBox.TrySetSelectedItem(string)`
- `DataForm<TModel>.SelectField(string key)`
- `DataForm<TModel>` uses explicit edit mode on the default path: row selection first, `Enter` to edit, `Enter` to commit, `Esc` to cancel, and failed commits render a dedicated validation line inside the widget
- `DataForm<TModel>.BeginEdit()`
- `DataForm<TModel>.CancelEdit()`
- `DataForm<TModel>.IsEditing`

These close the consumer proof-loop selection pressure without breaking existing APIs.

`DataForm<TModel>` now defaults to selection-first interaction: row selection does not mutate values, `Enter` enters edit mode, `Enter` commits, `Esc` cancels, and validation failure remains visible inside the control.

## Tier 2: Advanced But Supported

These APIs remain public because they still offer real value, but they should not dominate the default path.

- `TeaSharp.Hosting.TeaHostingOptions`
- `TeaSharp.Hosting.TeaHost.CreateApplication(...)`
- `TeaSharp.Hosting.TeaHost.RunAsync(...)`
- `TeaSharp.Hosting.IProgramRenderer`
- `TeaSharp.Hosting.RenderOutput`
- `TeaSharp.Hosting.AnsiRendererOptions`
- `TeaSharp.Hosting.AnsiDiffRenderer`
- `TeaSharp.Hosting.NullRenderer`
- `TeaSharp.Hosting.ITerminalAdapter`
- `TeaSharp.Hosting.TerminalSize`
- `TeaSharp.Hosting.TerminalCapabilityProfile`
- `TeaSharp.Hosting.TerminalColorProfile`
- `TeaSharp.Hosting.ConsoleTerminalAdapter`
- `TeaSharp.Hosting.TerminalCapabilityDetector`
- `TeaSharp.Hosting.TerminalColorProfileDetector`
- `TeaSharp.Hosting.IEventDecoder`
- `TeaSharp.Hosting.EventDecodeResult`
- `TeaSharp.Hosting.EventDecoder`
- `TeaSharp.Hosting.TerminalCursorStyle`
- `TeaSharp.Controls.NotificationInbox` (advanced dev/ops inbox workflow)
- `TeaSharp.Controls.BarChartOptions`
- `TeaSharp.Controls.LineChartOptions`
- `TeaSharp.Controls.LinePlotOptions`
- `TeaSharp.Controls.LinePlotRenderMode`
- `TeaSharp.Controls.PlotPanelOptions`
- `ICanvasComponent` as a render-only advanced seam
- renderer, terminal, and capability-probing seams

Most of these types are now marked `EditorBrowsable(Advanced)`.

## Tier 3: Candidates For Further Narrowing

These areas still expose more mechanism than the long-term public design should:

- low-level widget models leaking through component configuration
- runtime seams that most apps never need
- duplicate terminology between root app types and older core/runtime types
- the remaining advanced component namespaces that still expose an alternate engine-shaped control story
- lower-level runtime/input helpers that still live deeper than the preferred TeaSharp-owned hosting surface
- overlap between `TeaSharp` and `TeaSharp.Core` mental models when boundaries are not documented clearly

## Current Direction

TeaSharp is shifting from:

- `InteractiveScreenModel`
- `InputRouter`

to:

- `Tea.RunAsync(...)`
- `TeaApplicationBuilder`
- `TeaApp`
- `Screen`
- `ScreenContext`
- `TeaRuntimeOptions`
- `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`
- root `TeaSharp.Controls` wrappers

The old `TeaHost.CreateProgram(...)` / `TeaProgramOptions` / `IScreen` program-hosting path has been removed.

The old `ScreenComposer` composition bridge has been removed. The previous static layout helper DSL is internal-only, and root layouts now compile through the scene compiler/runtime loop.

The first root controls that already own their implementation directly are:

- `Label`
- `Button`
  - label chrome can be customized with `LabelPrefix` and `LabelSuffix`
  - padded button body can be styled via `SurfaceStyle`, `FocusedSurfaceStyle`, and `PressedSurfaceStyle`
  - rounded surface buttons can choose between `RoundedSurfaceMode = UnifiedShell` and `RoundedSurfaceMode = InsetBody`
  - `UnifiedShell` reserves a taller filled-pill silhouette with inset cap and shoulder rows so the shell reads as a rounded pill instead of collapsing to a 3-row cutout or clipped octagon; label-only pills use the taller 7-row contract while description-bearing action buttons stay on the tighter 5-row contract
  - `InsetBody` suppresses the default bracket label chrome and adds minimum inner X breathing room when apps keep the built-in button label defaults
  - compact filled rectangular buttons should prefer `BorderStyle.Heavy`; thin `SingleLine` borders plus full-cell surface fill can read like the body color escapes past the stroke on terminal grids
  - `BorderStyleText` colors button borders; `BorderStyle.Heavy` is the compact bordered-button affordance
  - label styles are text-only; body/background semantics belong to button surface styles
  - surface styling is expected to cover the whole inner button box, including padding, not only the post-padding content rect
  - rounded buttons are expected to read as one coherent pill/button surface with centered content, not nested visual layers
  - measurement must account for the widest rendered line across label and description
- `TextInput`
- `TextArea`
- `Breadcrumb`
- `Choice`
- `ComboBox`
- `ContextMenu`
- `CommandPalette`
- `Dialog`
- `ProgressBar`
- `LogView`
- `Badge`
- `Slider`
- `Spinner`
- `Toggle`
- `NumberInput`
- `DatePicker`
- `TimePicker`
- `MarkdownView`
- `Modal`
- `Accordion`
- `RadioGroup`
- `MultiSelect`
- `Gauge`
- `MiniLog`
- `StatsCard`
- `BarChart`
- `LineChart`
- `Sparkline`
- `AreaPlot`
- `ScatterPlot`
- `Histogram`
- `LinePlot`
- `PlotPanel`
- `ListView<T>`
- `VirtualizedListView<T>`
- `GroupedListView<TGroup,TItem>`
- `Tabs`
- `DataGrid`
- `TreeTable`
- `KanbanBoard`
- `TagInput`
- `CalendarMonthView`
- `SchedulerTimeline`
- `PivotTable`
- `QueryBuilder`
- `RichTextView`
- `KeyValueList`
- `Timeline`
- `Stepper`
- `DockWorkspace`
- `PaneTabs`
- `PaletteEditor`
- `Heatmap`
- `TreeMapChart`
- `TerminalPanel`
- `ProcessListView`
- `DashboardGrid`
- `QuickOpenOverlay`
- `BulletChart`
- `ResizablePaneGroup`
- `SideNavRail`
- `TokenEditor`
- `HealthBoard`
- `JumpList`
- `AutocompleteInput`
- `BoxPlot`
- `MenuBar`
- `Toolbar`
- `CommandBar`
- `StatusBar`
- `Paginator`
- `SearchBox`
- `SearchResultsView`
- `Form`
- `DataForm<TModel>`
- `FieldSet`
- `SplitView`
- `InspectorPanel`
- `Wizard`
- `DiffView`
- `PropertyGrid`
- `FileExplorer`
- `FuzzyFinder`
- `ToastCenter`

Their old `TeaSharp.Components.Prebuilt.*` counterparts have been removed instead of kept as compatibility wrappers.

## Design Constraints

- normal apps should stay in `TeaSharp`
- normal apps should not import `TeaSharp.Core.*`
- normal apps should not manage terminal size manually
- normal apps should not manage input scopes or region routing manually
- custom widgets should remain possible through a small stable contract

## Theme Mapping Status

Current shipped theme mapping is centralized in `TeaSharp.Styles.TeaThemeControlExtensions` and split into domain partial files (`Basic`, `InputValue`, `Navigation`, `NavigationOverlay`, `NavigationPrimitives`, `DataAndFlow`, `PlanningAndBoards`, `QueryAndRichText`, `ExplorerAndFeedback`, `RenderingTextUtilities`, `ModalAndCharts`, `Plotting`, `DevOpsAndWorkflows`, `Workspace`, `FormsAndShell`).
Overlay glyph cookbook snippets for `MenuBarGlyphSet`, `ContextMenuGlyphSet`, and `CommandPaletteGlyphSet` are documented in [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md).
Border override, dropdown/tree glyph-set, and data marker/separator cookbook snippets are documented in [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md).
Wave 2 query/analytics controls (`PivotTable`, `QueryBuilder`, `RichTextView`) are mapped with the same semantic-token and border-style contract.
Wave 3 dev/ops controls (`JsonTreeView`, `CommandOutput`, `LogTailPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`) are mapped with the same semantic-token contract. For onboarding/default app flows, use `Notifications`; treat `NotificationInbox` as advanced dev/ops surface.
Wave 4 batch A + B controls (`DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`) are integrated and mapped in `TeaThemeControlExtensions.Workspace.cs`.
Wave 1 app-shell/forms controls (`Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`) are integrated and mapped in `TeaThemeControlExtensions.FormsAndShell.cs`.
Expansion tranche controls (`DashboardGrid`, `QuickOpenOverlay`, `BulletChart`, `ResizablePaneGroup`, `SideNavRail`, `TokenEditor`, `HealthBoard`, `JumpList`, `AutocompleteInput`, `BoxPlot`) are implemented with dedicated tests; theme extension mapping is wired for all of them across `TeaThemeControlExtensions.DashboardMetrics.cs`, `TeaThemeControlExtensions.FormsAndShell.cs`, `TeaThemeControlExtensions.Navigation.cs`, `TeaThemeControlExtensions.NavigationOverlay.cs`, `TeaThemeControlExtensions.PlanningAndBoards.cs`, and `TeaThemeControlExtensions.Plotting.cs` (commit evidence: `4e005ed`, `1c1b748`, `03c7a43`, `db63e01`).
Reusable consumer-level override helpers are available via `TeaThemeOverrideBundle.CreateDashboardBundle(...)` and `ApplyThemeAndDashboardOverrides(...)` extensions for `ListView<T>`, `Table`, `Notifications`, `LogView`, `Button`, and `Dialog`.

## State-Style Naming Matrix (Tier 1 Consumer Quick Lookup)

Use this matrix to find the expected public hook names quickly when customizing focus/selection/hover/border behavior on the no-DI default path.

| Control family | Focus hooks | Selected hooks | Hovered hooks | Border hooks |
|---|---|---|---|---|
| Text and query inputs (`TextInput`, `TextArea`, `SearchBox`, `NumberInput`, `DatePicker`, `TimePicker`) | `FocusMarker`, `ShowFocusMarker`, `TitleStyle`, `FocusedTitleStyle` | `Selected*` where control-specific (for example result rows in `SearchResultsView`) | `Hovered*` where control-specific | `BorderStyleText`, `FocusedBorderStyleText` |
| List and tree navigation (`ListView<T>`, `TreeView`, `Choice`, `ComboBox`, `SearchResultsView`) | `FocusMarker`, `ShowFocusMarker`, `TitleStyle`, `FocusedTitleStyle` | `SelectedIndex`, `SelectedItem`, `Selected*Style` | `Hovered*Style` | `BorderStyleText`, `FocusedBorderStyleText` |
| Data surfaces (`Table`, `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`) | `FocusMarker`, `ShowFocusMarker`, focused title styles where supported | `Selected*Marker` and/or `Selected*Style` | `Hovered*Style` where supported | `BorderStyleText`, `FocusedBorderStyleText` |
| Forms and validation (`Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `ValidationSummary`) | `FocusMarker`, `ShowFocusMarker`, `FocusedTitleStyle` | `Selected*Style` or active-step styles | `Hovered*Style` where supported | `BorderStyleText`, `FocusedBorderStyleText` |
| Menus and overlays (`MenuBar`, `ContextMenu`, `CommandPalette`, `Notifications`) | `FocusMarker`, `ShowFocusMarker`, focused title styles where supported | selected-item style/marker hooks per control | hovered-item style/marker hooks per control | `BorderStyleText`, `FocusedBorderStyleText` |

Marker/glyph note:

- symbolic affordances remain explicit and typed (`DropdownGlyphSet`, `TreeViewGlyphSet`, `MenuBarGlyphSet`, `ContextMenuGlyphSet`, `CommandPaletteGlyphSet`, `SearchResultsGlyphSet`)
- `Focus.Marker` token is first-class in theme docs and should be mapped through marker-capable controls instead of hardcoded marker styling

## Plotting Authoring Guidance (Tier 1)

Plotting/dashboard controls on the default app path:

- single-metric trend: `Sparkline` or `AreaPlot`
- multi-metric trend: `LinePlot` + `LineSeries`
- correlation analysis: `ScatterPlot`
- distribution analysis: `Histogram`
- dashboard composition: `PlotPanel`
- dense telemetry cards: `LinePlotOptions.RenderMode = Compact`

Operational pattern:

- stream with bounded buffers
- reuse long-lived controls/series and mutate data in place
- apply theme defaults first, then control-instance overrides for title, border, legend, stats, axis, and data emphasis

Planned reference sample: `examples/PlottingDashboard` (once available).

## Typography Capability Status

- Portable typography lane: `TeaStyle.WithFontWeight(TeaFontWeight)` for ANSI SGR emphasis intent (normal/bold/dim), not real font engine control.
- Terminal request lanes:
  - `ScreenOptions.FontSpec` (legacy/explicit raw request).
  - `ScreenOptions.FontFamily` + `ScreenOptions.FontSize` (structured request).
  - `ScreenOptions.Iterm2Profile` (iTerm2 profile switch request).
- Capability gating:
  - OSC 50 requests are emitted only when `SupportsOsc50FontRequests` is true.
  - iTerm2 profile requests are emitted only when `SupportsIterm2ProfileRequests` is true.
- Preference rule: if iTerm2 profile switching is supported and `Iterm2Profile` is set, renderer prefers profile switching over OSC 50 font requests.
- Explicit caveat: all font requests are best-effort and terminal-dependent.
- Terminal matrix: [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md).

Usage guidance (default path):

```csharp
runtime.Screen = new ScreenOptions
{
    FontSpec = "JetBrains Mono 13", // legacy/raw
    FontFamily = "JetBrains Mono",  // structured
    FontSize = 13,
    Iterm2Profile = "TeaSharp",
};
```

Support matrix (TeaSharp V1 contract):

- sequence emission: capability-gated (`SupportsOsc50FontRequests` / `SupportsIterm2ProfileRequests`)
- sanitization: `BEL`, `ESC`, `\`, and control chars are stripped before emission
- preference: iTerm2 profile lane wins when supported and requested
- reset/restore previous font: no (intentionally avoids unsafe assumptions)

Bordered control parity policy:

- any public control with bordered frame rendering must expose `BorderStyleText` and `FocusedBorderStyleText`
- each bordered control must have `ApplyTheme`/`ApplyThemeDefaults` token mapping in the corresponding `TeaThemeControlExtensions` domain file
- parity is enforced through theme mapping tests plus visual edge-case assertions before merge

Basic controls with direct token mappings:

- `Label`, `Button`, `ListView<T>`, `StatusBar`, `TextInput`, `Table`, `Tabs`
- `Label` and `Button` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `TextInput` and `Table` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Input/value controls with direct token mappings:

- `TextInput`, `TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`
- `TextInput` maps value/placeholder/focused-title styling; focus title marker is configurable through `FocusMarker` + `ShowFocusMarker`
- `Spinner` exposes `Frames` and `SetFrames(...)` for runtime spinner-family swaps without replacing the control instance
- `Toggle`, `Slider`, `Spinner`, and `ProgressBar` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `TextArea`, `NumberInput`, `DatePicker`, and `TimePicker` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Navigation controls with direct token mappings:

- `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`, `SearchResultsView`
- `SearchBox` maps title/value/placeholder/match/navigation styles plus border text hooks; title focus marker is configurable through `FocusMarker` + `ShowFocusMarker`
- `SearchResultsView` maps title, row-state, and border text hooks; marker customization is explicit through `SearchResultsGlyphSet`

Form/shell controls with direct token mappings:

- `Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`
- these controls map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Navigation/overlay controls with direct token mappings:

- `Choice`, `ComboBox`, `TreeView`, `MenuBar`, `ContextMenu`, `CommandPalette`, `Notifications`, `SearchBox`
- `Choice`/`ComboBox` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `Choice`/`ComboBox` keep glyph customization explicit through `DropdownGlyphSet`
- `TreeView` maps `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `TreeView` keeps title focus marker rendering configurable through `FocusMarker` + `ShowFocusMarker` and glyph customization through `TreeViewGlyphSet`
- `MenuBar`, `ContextMenu`, and `CommandPalette` map border text hooks to border/focus tokens
- `MenuBar`, `ContextMenu`, and `CommandPalette` expose typed glyph configuration through `MenuBarGlyphSet`, `ContextMenuGlyphSet`, and `CommandPaletteGlyphSet`
- `Notifications` maps `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `ContextMenu` preserves focused title markers in bordered rendering width calculations

Navigation primitive controls with direct token mappings:

- `Accordion`, `MultiSelect`, `RadioGroup`

Data/flow controls with direct token mappings:

- `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`, `Stepper`
- `DataGrid` maps `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `DataGrid` provides API-level text hooks for `ColumnSeparatorText`, `SortAscendingMarker`, and `SortDescendingMarker`
- `TreeTable` maps `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`
- `TreeTable` provides API-level text hooks for `ColumnSeparatorText`, row markers, and branch/leaf markers
- `KeyValueList` and `Timeline` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Explorer/feedback controls with direct token mappings:

- `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`
- `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, and `ToastCenter` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Dev/ops workflow controls with direct token mappings:

- `JsonTreeView`, `CommandOutput`, `LogTailPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`
- `JsonTreeView`, `CommandOutput`, `LogTailPanel`, and `ActivityFeed` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Workspace/visual-data controls with direct token mappings:

- `DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`
- bordered controls in this set (`DockWorkspace`, `PaneTabs`, `Heatmap`, `TreeMapChart`, `ProcessListView`) map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Rendering text utility controls with direct token mappings:

- `Badge`, `LogView`, `MarkdownView`, `MiniLog`
- `LogView` and `MarkdownView` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

Modal/chart summary controls with direct token mappings:

- `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`
- `Dialog` and `Modal` map `BorderStyleText` -> `Border.Default` and `FocusedBorderStyleText` -> `Border.Focused + Focus.Border`

## Follow-up Targets

1. keep moving control authoring toward a single obvious configuration style
2. review Tier 2 periodically and internalize anything that is public only by inertia
3. keep `TeaSharp.Core` as the intentional low-level product and keep docs/examples explicit about when app authors should prefer `TeaSharp` instead
4. keep custom widget extensibility stable while internal runtime details continue to shrink and stay behind TeaSharp-owned internal adapters
5. preserve discoverability and parity policy tests (for example `BorderedControlParityPolicyTests.cs`) so new bordered controls cannot drift from required hook/mapping coverage
6. keep V1 image scope out of the V1 default path docs (image rendering planned for V1.1)
