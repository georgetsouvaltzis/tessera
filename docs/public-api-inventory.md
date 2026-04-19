# Tessera Public API Inventory

## Purpose

This document tracks the public API tiers so the public alpha surface stays deliberate.

## Freeze Status

The current working freeze line is:

- Tier 1 is the supported default consumer path
- Tier 2 is the supported advanced escape-hatch path
- anything outside those tiers is still a candidate for internalization or deletion before stable release

Examples, README guidance, and starter docs should teach Tier 1 first. Tier 2 may remain public, but it should not be the default onboarding story.

## Tier 1: Default Consumer Path

These are the types new applications should discover first.

- `Tessera`
- `TesseraApp`
- `TesseraApplication`
- `TesseraApplicationBuilder`
- `TesseraRuntimeOptions`
- `TesseraEffect`
- `TesseraEffects`
- `TesseraEffects.Periodic(...)` for auto-rescheduling interval updates
- `Message` and the typed message records in `Tessera`
- `Tessera.Styles.TesseraStyle`
- `Tessera.Styles.AnsiColor`
- `Tessera.Styles.TesseraFontWeight`
- `Tessera.Styles.TesseraThemeOverrideBundle`
- `Tessera.Styles.TesseraThemeOverrideBundleExtensions`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Screen.Build(...)`
- `Tessera.Layout.*` object-model types
- `Tessera.Controls.Button`
- `Tessera.Controls.Breadcrumb`
- `Tessera.Controls.Label`
- `Tessera.Controls.Badge`
- `Tessera.Controls.BadgeTone`
- `Tessera.Controls.Accordion`
- `Tessera.Controls.AccordionSection`
- `Tessera.Controls.TextInput`
- `Tessera.Controls.TextArea`
- `Tessera.Controls.Choice`
- `Tessera.Controls.ComboBox`
- `Tessera.Controls.DropdownGlyphSet`
- `Tessera.Controls.CommandPalette`
- `Tessera.Controls.CommandPaletteItem`
- `Tessera.Controls.CommandPaletteGlyphSet`
- `Tessera.Controls.Dialog`
- `Tessera.Controls.DialogResult`
- `Tessera.Controls.DialogClosedEventArgs`
- `Tessera.Controls.ContextMenu`
- `Tessera.Controls.ContextMenuItem`
- `Tessera.Controls.ContextMenuGlyphSet`
- `Tessera.Controls.ProgressBar`
- `Tessera.Controls.BarPoint`
- `Tessera.Controls.BarChart`
- `Tessera.Controls.LineChart`
- `Tessera.Controls.Sparkline`
- `Tessera.Controls.TelemetryChart`
- `Tessera.Controls.TelemetryChartOptions`
- `Tessera.Controls.TelemetryChartRenderMode`
- `Tessera.Controls.AreaPlot`
- `Tessera.Controls.ScatterPlotPoint`
- `Tessera.Controls.ScatterPlot`
- `Tessera.Controls.HistogramBucket`
- `Tessera.Controls.Histogram`
- `Tessera.Controls.LineSeries`
- `Tessera.Controls.LinePlot`
- `Tessera.Controls.PlotPanel`
- `Tessera.Controls.BulletChart`
- `Tessera.Controls.BulletRange`
- `Tessera.Controls.BulletRangeKind`
- `Tessera.Controls.DashboardGrid`
- `Tessera.Controls.DashboardTile`
- `Tessera.Controls.QuickOpenOverlay`
- `Tessera.Controls.QuickOpenItem`
- `Tessera.Controls.QuickOpenOverlayGlyphSet`
- `Tessera.Controls.QuickOpenOverlaySubmittedEventArgs`
- `Tessera.Controls.ResizablePaneGroup`
- `Tessera.Controls.PaneSpec`
- `Tessera.Controls.SideNavRail`
- `Tessera.Controls.NavItem`
- `Tessera.Controls.SideNavRailGlyphSet`
- `Tessera.Controls.SideNavRailSelectionChangedEventArgs`
- `Tessera.Controls.SideNavRailActivatedEventArgs`
- `Tessera.Controls.TokenEditor`
- `Tessera.Controls.TokenItem`
- `Tessera.Controls.TokenEditorGlyphSet`
- `Tessera.Controls.TokenEditorSelectionChangedEventArgs`
- `Tessera.Controls.HealthBoard`
- `Tessera.Controls.HealthService`
- `Tessera.Controls.HealthServiceSeverity`
- `Tessera.Controls.HealthBoardGlyphSet`
- `Tessera.Controls.JumpList`
- `Tessera.Controls.JumpListItem`
- `Tessera.Controls.JumpListGlyphSet`
- `Tessera.Controls.JumpListActivatedEventArgs`
- `Tessera.Controls.AutocompleteInput`
- `Tessera.Controls.AutocompleteInputGlyphSet`
- `Tessera.Controls.AutocompleteInputSuggestionCommittedEventArgs`
- `Tessera.Controls.BoxPlot`
- `Tessera.Controls.BoxPlotSeries`
- `Tessera.Controls.Gauge`
- `Tessera.Controls.MiniLog`
- `Tessera.Controls.StatItem`
- `Tessera.Controls.StatsCard`
- `Tessera.Controls.NumberInput`
- `Tessera.Controls.DatePicker`
- `Tessera.Controls.TimePicker`
- `Tessera.Controls.MarkdownView`
- `Tessera.Controls.MultiSelect`
- `Tessera.Controls.Paginator`
- `Tessera.Controls.RadioGroup`
- `Tessera.Controls.LogView`
- `Tessera.Controls.Modal`
- `Tessera.Controls.Notifications` (primary notification feed API)
- `Tessera.Controls.Toggle`
- `Tessera.Controls.Slider`
- `Tessera.Controls.Spinner`
- `Tessera.Controls.StatusBar`
- `Tessera.Controls.Tabs`
- `Tessera.Controls.ListView<T>`
- `Tessera.Controls.VirtualizedListView<T>`
- `Tessera.Controls.VirtualizedListViewOptions`
- `Tessera.Controls.GroupedListView<TGroup,TItem>`
- `Tessera.Controls.GroupedListViewGroup<TGroup,TItem>`
- `Tessera.Controls.GroupedListSelectionChangedEventArgs<TGroup,TItem>`
- `Tessera.Controls.Table`
- `Tessera.Controls.KanbanBoard`
- `Tessera.Controls.KanbanLane`
- `Tessera.Controls.KanbanCard`
- `Tessera.Controls.KanbanSelectionChangedEventArgs`
- `Tessera.Controls.TagInput`
- `Tessera.Controls.CalendarMonthView`
- `Tessera.Controls.CalendarDayCell`
- `Tessera.Controls.CalendarDateSelectedEventArgs`
- `Tessera.Controls.SchedulerTimeline`
- `Tessera.Controls.SchedulerEntry`
- `Tessera.Controls.SchedulerSelectionChangedEventArgs`
- `Tessera.Controls.PivotTable`
- `Tessera.Controls.PivotTableColumn`
- `Tessera.Controls.PivotTableCell`
- `Tessera.Controls.PivotSortDirection`
- `Tessera.Controls.PivotSortRequestedEventArgs`
- `Tessera.Controls.QueryBuilder`
- `Tessera.Controls.QueryGroup`
- `Tessera.Controls.QueryRule`
- `Tessera.Controls.QueryOperator`
- `Tessera.Controls.QueryChangedEventArgs`
- `Tessera.Controls.RichTextView`
- `Tessera.Controls.RichTextSegment`
- `Tessera.Controls.RichTextStyleKind`
- `Tessera.Controls.JsonTreeView`
- `Tessera.Controls.JsonTreeNode`
- `Tessera.Controls.JsonTreeNodeKind`
- `Tessera.Controls.JsonTreeSelectionChangedEventArgs`
- `Tessera.Controls.TraceViewer`
- `Tessera.Controls.TraceEntry`
- `Tessera.Controls.TraceSelectionChangedEventArgs`
- `Tessera.Controls.CommandOutput`
- `Tessera.Controls.CommandOutputLine`
- `Tessera.Controls.CommandOutputChannel`
- `Tessera.Controls.LogTailPanel`
- `Tessera.Controls.LogEntry`
- `Tessera.Controls.LogLevel`
- `Tessera.Controls.TaskRunnerPanel`
- `Tessera.Controls.TaskRunItem`
- `Tessera.Controls.TaskRunnerSelectionChangedEventArgs`
- `Tessera.Controls.DockWorkspace`
- `Tessera.Controls.DockPane`
- `Tessera.Controls.DockPanePosition`
- `Tessera.Controls.PaneTabs`
- `Tessera.Controls.PaneTabItem`
- `Tessera.Controls.PaneTabSelectionChangedEventArgs`
- `Tessera.Controls.PaletteEditor`
- `Tessera.Controls.PaletteSwatch`
- `Tessera.Controls.PaletteSelectionChangedEventArgs`
- `Tessera.Controls.Heatmap`
- `Tessera.Controls.HeatmapCell`
- `Tessera.Controls.HeatmapLegend`
- `Tessera.Controls.TreeMapChart`
- `Tessera.Controls.TreeMapNode`
- `Tessera.Controls.TerminalPanel`
- `Tessera.Controls.TerminalPanelLine`
- `Tessera.Controls.TerminalPanelChannel`
- `Tessera.Controls.ProcessListView`
- `Tessera.Controls.ProcessListEntry`
- `Tessera.Controls.ProcessListStatus`
- `Tessera.Controls.ProcessListSelectionChangedEventArgs`
- `Tessera.Controls.ActivityFeed`
- `Tessera.Controls.ActivityFeedItem`
- `Tessera.Controls.ActivityFeedItemKind`
- `Tessera.Controls.InboxItem`
- `Tessera.Controls.KeyBindingHelpDialog`
- `Tessera.Controls.KeyBindingItem`
- `Tessera.Controls.DataGrid`
- `Tessera.Controls.TreeTable`
- `Tessera.Controls.KeyValueList`
- `Tessera.Controls.Timeline`
- `Tessera.Controls.Stepper`
- `Tessera.Controls.TreeItem`
- `Tessera.Controls.TreeView` (native)
- `Tessera.Controls.TreeViewGlyphSet`
- `Tessera.Controls.MenuBar`
- `Tessera.Controls.MenuBarGlyphSet`
- `Tessera.Controls.Toolbar`
- `Tessera.Controls.CommandBar`
- `Tessera.Controls.SearchBox`
- `Tessera.Controls.SearchResultsView`
- `Tessera.Controls.SearchResultsGlyphSet`
- `Tessera.Controls.Form`
- `Tessera.Controls.FormField`
- `Tessera.Controls.DataForm<TModel>`
- `Tessera.Controls.DataFormField<TModel>`
- `Tessera.Controls.DataFormSelectionChangedEventArgs<TModel>`
- `Tessera.Controls.DataFormFieldCommittedEventArgs<TModel>`
- `Tessera.Controls.FieldSet`
- `Tessera.Controls.SplitView`
- `Tessera.Controls.SplitViewOrientation`
- `Tessera.Controls.InspectorPanel`
- `Tessera.Controls.InspectorSection`
- `Tessera.Controls.InspectorField`
- `Tessera.Controls.Wizard`
- `Tessera.Controls.WizardStep`
- `Tessera.Controls.WizardStepChangedEventArgs`
- `Tessera.Controls.EmptyState`
- `Tessera.Controls.ValidationSummary`
- `Tessera.Controls.DiffView`
- `Tessera.Controls.PropertyGrid`
- `Tessera.Controls.FileExplorer`
- `Tessera.Controls.FuzzyFinder`
- `Tessera.Controls.ToastCenter`
- `Tessera.Controls.MenuItem`
- `Tessera.Controls.Control`

The intended beginner path is:

- build an app by deriving from `TesseraApp`
- run it with the minimal startup lane (`TesseraApplication.RunAsync(new App())`) or configured startup lane (`TesseraApplication.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`)
- rely on automatic control routing; `Update(...)` handles only unhandled input plus runtime messages
- return `Screen` from `Build(ScreenContext)`
- assemble screens with `Screen.Build(...)` and shallow builder callbacks
- keep configuration in `TesseraRuntimeOptions` and `ScreenOptions`
- follow the public onboarding path in [getting-started](/docs/getting-started), starting with `HelloWorld`, `CounterForm`, and `WorkspaceApp` in [examples](/docs/examples) before the flagship showcases in [showcase](/docs/showcase)
- treat `Tessera.Core` as the low-level advanced lane, not default onboarding
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
- verify `TESSERA_CAPS` is not disabling mouse (`mouse=0`).
- if using tmux, enable `set -g mouse on`.
- if terminal text selection appears instead of app pointer behavior, run through the checklist above first.

### Notification Surface Guidance (Tier 1)

- primary onboarding path: `Tessera.Controls.Notifications`
- advanced/devops path: `Tessera.Controls.NotificationInbox` (Tier 2)
- shared item model: `Tessera.Controls.InboxItem`

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

- `Tessera.Hosting.TesseraHostingOptions`
- `Tessera.Hosting.TesseraHost.CreateApplication(...)`
- `Tessera.Hosting.TesseraHost.RunAsync(...)`
- `Tessera.Hosting.IProgramRenderer`
- `Tessera.Hosting.RenderOutput`
- `Tessera.Hosting.AnsiRendererOptions`
- `Tessera.Hosting.AnsiDiffRenderer`
- `Tessera.Hosting.NullRenderer`
- `Tessera.Hosting.ITerminalAdapter`
- `Tessera.Hosting.TerminalSize`
- `Tessera.Hosting.TerminalCapabilityProfile`
- `Tessera.Hosting.TerminalColorProfile`
- `Tessera.Hosting.ConsoleTerminalAdapter`
- `Tessera.Hosting.TerminalCapabilityDetector`
- `Tessera.Hosting.TerminalColorProfileDetector`
- `Tessera.Hosting.IEventDecoder`
- `Tessera.Hosting.EventDecodeResult`
- `Tessera.Hosting.EventDecoder`
- `Tessera.Hosting.TerminalCursorStyle`
- `Tessera.Controls.NotificationInbox` (advanced dev/ops inbox workflow)
- `Tessera.Controls.BarChartOptions`
- `Tessera.Controls.LineChartOptions`
- `Tessera.Controls.LinePlotOptions`
- `Tessera.Controls.LinePlotRenderMode`
- `Tessera.Controls.PlotPanelOptions`
- `ICanvasComponent` as a render-only advanced seam
- renderer, terminal, and capability-probing seams

Most of these types are now marked `EditorBrowsable(Advanced)`.

## Tier 3: Candidates For Further Narrowing

These areas still expose more mechanism than the long-term public design should:

- low-level widget models leaking through component configuration
- runtime seams that most apps never need
- duplicate terminology between root app types and older core/runtime types
- the remaining advanced component namespaces that still expose an alternate engine-shaped control story
- lower-level runtime/input helpers that still live deeper than the preferred Tessera-owned hosting surface
- overlap between `Tessera` and `Tessera.Core` mental models when boundaries are not documented clearly

## Current Direction

Tessera is shifting from:

- `InteractiveScreenModel`
- `InputRouter`

to:

- `TesseraApplication.RunAsync(...)`
- `TesseraApplicationBuilder`
- `TesseraApp`
- `Screen`
- `ScreenContext`
- `TesseraRuntimeOptions`
- `WindowLayout`, `RowLayout`, `ColumnLayout`, `PanelLayout`, `CenterLayout`, `LayoutSlot`
- root `Tessera.Controls` wrappers

The old `TesseraHost.CreateProgram(...)` / `TesseraProgramOptions` / `IScreen` program-hosting path has been removed.

The old `ScreenComposer` composition bridge has been removed. The previous static layout helper DSL is internal-only, and root layouts now compile through the scene compiler/runtime loop.

The first root controls that already own their implementation directly are:

- `Label`
- `Button`
  - label chrome can be customized with `LabelPrefix` and `LabelSuffix`
  - defaults to plain label chrome plus built-in symmetric horizontal breathing room
  - padded button body can be styled via `SurfaceStyle`, `FocusedSurfaceStyle`, and `PressedSurfaceStyle`
  - label styles are text-only; body/background semantics belong to button surface styles
  - surface styling is expected to cover the whole allocated button box, including padding, not only the post-padding content rect
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

Their old `Tessera.Components.Prebuilt.*` counterparts have been removed instead of kept as compatibility wrappers.

## Design Constraints

- normal apps should stay in `Tessera`
- normal apps should not import `Tessera.Core.*`
- normal apps should not manage terminal size manually
- normal apps should not manage input scopes or region routing manually
- custom widgets should remain possible through a small stable contract

## Theme Mapping Status

Current shipped theme mapping is centralized in `Tessera.Styles.TesseraThemeControlExtensions` and split into domain partial files (`Basic`, `InputValue`, `Navigation`, `NavigationOverlay`, `NavigationPrimitives`, `DataAndFlow`, `PlanningAndBoards`, `QueryAndRichText`, `ExplorerAndFeedback`, `RenderingTextUtilities`, `ModalAndCharts`, `Plotting`, `DevOpsAndWorkflows`, `Workspace`, `FormsAndShell`).
Overlay glyph cookbook snippets for `MenuBarGlyphSet`, `ContextMenuGlyphSet`, and `CommandPaletteGlyphSet` are documented in [theme-system](/docs/theme-system).
Border override, dropdown/tree glyph-set, and data marker/separator cookbook snippets are documented in [theme-system](/docs/theme-system).
Wave 2 query/analytics controls (`PivotTable`, `QueryBuilder`, `RichTextView`) are mapped with the same semantic-token and border-style contract.
Wave 3 dev/ops controls (`JsonTreeView`, `CommandOutput`, `LogTailPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`) are mapped with the same semantic-token contract. For onboarding/default app flows, use `Notifications`; treat `NotificationInbox` as advanced dev/ops surface.
Wave 4 batch A + B controls (`DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`) are integrated and mapped in `TesseraThemeControlExtensions.Workspace.cs`.
Wave 1 app-shell/forms controls (`Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`) are integrated and mapped in `TesseraThemeControlExtensions.FormsAndShell.cs`.
Expansion tranche controls (`DashboardGrid`, `QuickOpenOverlay`, `BulletChart`, `ResizablePaneGroup`, `SideNavRail`, `TokenEditor`, `HealthBoard`, `JumpList`, `AutocompleteInput`, `BoxPlot`) are implemented with dedicated tests; theme extension mapping is wired for all of them across `TesseraThemeControlExtensions.DashboardMetrics.cs`, `TesseraThemeControlExtensions.FormsAndShell.cs`, `TesseraThemeControlExtensions.Navigation.cs`, `TesseraThemeControlExtensions.NavigationOverlay.cs`, `TesseraThemeControlExtensions.PlanningAndBoards.cs`, and `TesseraThemeControlExtensions.Plotting.cs` (commit evidence: `4e005ed`, `1c1b748`, `03c7a43`, `db63e01`).
Reusable consumer-level override helpers are available via `TesseraThemeOverrideBundle.CreateDashboardBundle(...)` and `ApplyThemeAndDashboardOverrides(...)` extensions for `ListView<T>`, `Table`, `Notifications`, `LogView`, `Button`, and `Dialog`.

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

- Portable typography lane: `TesseraStyle.WithFontWeight(TesseraFontWeight)` for ANSI SGR emphasis intent (normal/bold/dim), not real font engine control.
- Terminal request lanes:
  - `ScreenOptions.FontSpec` (legacy/explicit raw request).
  - `ScreenOptions.FontFamily` + `ScreenOptions.FontSize` (structured request).
  - `ScreenOptions.Iterm2Profile` (iTerm2 profile switch request).
- Capability gating:
  - OSC 50 requests are emitted only when `SupportsOsc50FontRequests` is true.
  - iTerm2 profile requests are emitted only when `SupportsIterm2ProfileRequests` is true.
- Preference rule: if iTerm2 profile switching is supported and `Iterm2Profile` is set, renderer prefers profile switching over OSC 50 font requests.
- Explicit caveat: all font requests are best-effort and terminal-dependent.
- Terminal matrix: [terminal-font-capability-matrix](/docs/terminal-font-capability-matrix).

Usage guidance (default path):

```csharp
runtime.Screen = new ScreenOptions
{
    FontSpec = "JetBrains Mono 13", // legacy/raw
    FontFamily = "JetBrains Mono",  // structured
    FontSize = 13,
    Iterm2Profile = "Tessera",
};
```

Support matrix (Tessera V1 contract):

- sequence emission: capability-gated (`SupportsOsc50FontRequests` / `SupportsIterm2ProfileRequests`)
- sanitization: `BEL`, `ESC`, `\`, and control chars are stripped before emission
- preference: iTerm2 profile lane wins when supported and requested
- reset/restore previous font: no (intentionally avoids unsafe assumptions)

Bordered control parity policy:

- any public control with bordered frame rendering must expose `BorderStyleText` and `FocusedBorderStyleText`
- each bordered control must have `ApplyTheme`/`ApplyThemeDefaults` token mapping in the corresponding `TesseraThemeControlExtensions` domain file
- parity is enforced through theme mapping tests plus visual edge-case assertions before merge

Basic controls with direct token mappings:

- `Label`, `Button`, `ListView<T>`, `StatusBar`, `TextInput`, `Table`, `Tabs`
- `Button` maps `SurfaceStyle` -> `Surface.Panel`, `FocusedSurfaceStyle` -> `Surface.Panel`, and `PressedSurfaceStyle` -> `Selection.Background`
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
3. keep `Tessera.Core` as the intentional low-level product and keep docs/examples explicit about when app authors should prefer `Tessera` instead
4. keep custom widget extensibility stable while internal runtime details continue to shrink and stay behind Tessera-owned internal adapters
5. preserve discoverability and parity policy tests (for example `BorderedControlParityPolicyTests.cs`) so new bordered controls cannot drift from required hook/mapping coverage
6. keep V1 image scope out of the V1 default path docs (image rendering planned for V1.1)
