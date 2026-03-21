# TeaSharp Control Catalog

TeaSharp now distinguishes between:

- root controls for the default app path
- advanced seams for specialized hosting and interop

Root controls are C#-first and no-DI by default.

## Root Controls

Preferred public catalog:

1. `Label`
2. `Button`
3. `TextInput`
4. `TextArea`
5. `Choice`
6. `ComboBox`
7. `Dialog`
8. `ProgressBar`
9. `LogView`
10. `Notifications`
11. `Toggle`
12. `Slider`
13. `Spinner`
14. `StatusBar`
15. `Tabs`
16. `ListView<T>`
17. `Table`
18. `TreeItem`
19. `TreeView`
20. `MenuBar`
21. `NumberInput`
22. `DatePicker`
23. `TimePicker`
24. `MarkdownView`
25. `MultiSelect`
26. `RadioGroup`
27. `CommandPalette`
28. `ContextMenu`
29. `Badge`
30. `Modal`
31. `Accordion`
32. `Gauge`
33. `MiniLog`
34. `StatsCard`
35. `BarChart`
36. `LineChart`
37. `Breadcrumb`
38. `Paginator`
39. `Toolbar`
40. `CommandBar`
41. `SearchBox`
42. `DiffView`
43. `PropertyGrid`
44. `FileExplorer`
45. `FuzzyFinder`
46. `ToastCenter`
47. `DataGrid`
48. `TreeTable`
49. `KeyValueList`
50. `Timeline`
51. `Stepper`
52. `SearchResultsView`
53. `SearchResultsGlyphSet`
54. `Sparkline`
55. `AreaPlot`
56. `ScatterPlot`
57. `ScatterPlotPoint`
58. `Histogram`
59. `HistogramBucket`
60. `LinePlot`
61. `LineSeries`
62. `LinePlotOptions`
63. `PlotPanel`
64. `PlotPanelOptions`
65. `VirtualizedListView<T>`
66. `VirtualizedListViewOptions`
67. `GroupedListView<TGroup,TItem>`
68. `GroupedListViewGroup<TGroup,TItem>`
69. `GroupedListSelectionChangedEventArgs<TGroup,TItem>`
70. `KanbanBoard`
71. `KanbanLane`
72. `KanbanCard`
73. `KanbanSelectionChangedEventArgs`
74. `TagInput`
75. `CalendarMonthView`
76. `CalendarDayCell`
77. `CalendarDateSelectedEventArgs`
78. `SchedulerTimeline`
79. `SchedulerEntry`
80. `SchedulerSelectionChangedEventArgs`
81. `PivotTable`
82. `PivotTableColumn`
83. `PivotTableCell`
84. `PivotSortDirection`
85. `PivotSortRequestedEventArgs`
86. `QueryBuilder`
87. `QueryGroup`
88. `QueryRule`
89. `QueryOperator`
90. `QueryChangedEventArgs`
91. `RichTextView`
92. `RichTextSegment`
93. `RichTextStyleKind`
94. `JsonTreeView`
95. `JsonTreeNode`
96. `JsonTreeNodeKind`
97. `JsonTreeSelectionChangedEventArgs`
98. `CommandOutput`
99. `CommandOutputLine`
100. `CommandOutputChannel`
101. `LogTailPanel`
102. `LogEntry`
103. `LogLevel`
104. `ActivityFeed`
105. `ActivityFeedItem`
106. `ActivityFeedItemKind`
107. `NotificationInbox`
108. `InboxItem`
109. `KeyBindingHelpDialog`
110. `KeyBindingItem`
111. `TraceViewer`
112. `TraceEntry`
113. `TraceSelectionChangedEventArgs`
114. `TaskRunnerPanel`
115. `TaskRunItem`
116. `TaskRunnerSelectionChangedEventArgs`

These live in `TeaSharp.Controls`.

### Dropdown Visual Defaults

- `Choice` and `ComboBox` now use richer dropdown glyph defaults for closed/open states.
- `Choice.Glyphs` and `ComboBox.Glyphs` use `DropdownGlyphSet` for explicit collapsed/expanded/highlighted/selected markers.
- Border text visuals are overrideable per control through `BorderStyleText` and `FocusedBorderStyleText`.
- Theme defaults map those border text hooks to semantic border/focus tokens.

### Focus Marker and Title Hooks

- `ListView<T>` and `TreeView` expose `FocusMarker` and `ShowFocusMarker` for title focus rendering.
- `TextInput` and `SearchBox` expose `FocusMarker` and `ShowFocusMarker` with `TitleStyle`/`FocusedTitleStyle`.
- `TextInput` and `SearchBox` style hooks include title/value/placeholder plus border text hooks (`BorderStyleText`/`FocusedBorderStyleText`).
- `TextArea`, `NumberInput`, `DatePicker`, and `TimePicker` also expose border text hooks for focused/unfocused frame rendering.

### Navigation Overlay Glyph and Border Hooks

- `MenuBar`, `ContextMenu`, and `CommandPalette` support `BorderStyleText`/`FocusedBorderStyleText`.
- Glyph customization is typed through `MenuBarGlyphSet`, `ContextMenuGlyphSet`, and `CommandPaletteGlyphSet`.
- Quick cookbook setup:

```csharp
var menuBar = new MenuBar { Glyphs = new MenuBarGlyphSet("[", "]", " ", " ", "{", "}", "(", ")") };
var contextMenu = new ContextMenu { ShowFocusMarker = true, Glyphs = new ContextMenuGlyphSet("·", ">", "▸", " ") };
var commandPalette = new CommandPalette { ShowFocusMarker = true, Glyphs = new CommandPaletteGlyphSet("❯", " ", ">", "▸", " ") };
```

### Search Results Hooks

- `SearchResultsView` exposes title/focus markers, bordered frame hooks, and full row-state style hooks.
- Row markers are customizable through `SearchResultsGlyphSet` (`DefaultRowMarker`, `HoveredRowMarker`, `SelectedRowMarker`, `MatchMarker`, `RankSeparator`).
- Query matching can display explicit match markers with rank prefixes for dense result lists.

### Table and TreeView Visual Hooks

- `Table` supports `BorderStyleText` and `FocusedBorderStyleText` for focused/unfocused frame glyph rendering.
- `TreeView` supports the same border text hooks and typed marker customization via `TreeViewGlyphSet`.

### DataGrid and TreeTable Visual Hooks

- `DataGrid` supports `BorderStyleText`/`FocusedBorderStyleText`, `ColumnSeparatorText`, `SortAscendingMarker`, and `SortDescendingMarker`.
- `TreeTable` supports `BorderStyleText`/`FocusedBorderStyleText`, `ColumnSeparatorText`, row markers (`SelectedRowMarker`/`UnselectedRowMarker`), and tree markers (`ExpandedBranchMarker`/`CollapsedBranchMarker`/`LeafMarker`).
- Quick cookbook setup:

```csharp
var combo = new ComboBox { Glyphs = new DropdownGlyphSet("v", "^", ">", "+") };
var tree = new TreeView { Glyphs = new TreeViewGlyphSet("▼", "▶", "•") };
var dataGrid = new DataGrid { ColumnSeparatorText = " │ ", SortAscendingMarker = " ↑", SortDescendingMarker = " ↓" };
var treeTable = new TreeTable("Name", "Value") { ColumnSeparatorText = " │ ", SelectedRowMarker = ">", UnselectedRowMarker = " ", ExpandedBranchMarker = "▼", CollapsedBranchMarker = "▶", LeafMarker = "•" };
```

### Rendering and Explorer Border Hooks

- `Notifications`, `LogView`, and `MarkdownView` support `BorderStyleText`/`FocusedBorderStyleText` with theme token mapping.
- `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, and `ToastCenter` support the same border-style hooks with theme token mapping.

### Latest Border Hook Rollout (Group1 and Group2)

- `Button`, `Label`, `ProgressBar`, `Toggle`, `Slider`, and `Spinner` expose `BorderStyleText`/`FocusedBorderStyleText`.
- `Dialog`, `Modal`, `KeyValueList`, and `Timeline` expose `BorderStyleText`/`FocusedBorderStyleText`.
- Theme defaults map all of the above to semantic border/focus tokens.

### ContextMenu Bordered Title Behavior

- Bordered `ContextMenu` titles now preserve focused `FocusMarker` output by reserving width for the rendered title marker text.

### Plotting and Dashboard Authoring

Use plotting controls by data shape:

- `Sparkline`: compact single-metric trend in one row (high-frequency status bars).
- `AreaPlot`: single series trend where fill emphasizes magnitude.
- `LinePlot`: multi-series time-aligned trend comparisons (`LineSeries` per metric).
- `ScatterPlot`: X/Y correlation or non-time samples.
- `Histogram`: distribution/bucket views (latency buckets, error counts).
- `PlotPanel`: compose multiple plot controls into one bordered dashboard surface.

Recommended streaming patterns:

- For `Sparkline` and `AreaPlot`, prefer constructor capacity (`new Sparkline(capacity: 240)`) and append (`Append(value)`).
- For `LinePlot`, keep bounded external buffers per metric, then refresh each `LineSeries` via `SetSamples(...)`.
- Reuse controls and series instances across frames; mutate data only in `Update(...)`.

```csharp
private static void PushBounded(Queue<double> buffer, double value, int capacity)
{
    if (buffer.Count == capacity) buffer.Dequeue();
    buffer.Enqueue(value);
}
```

Theme and override pattern:

- Start with semantic defaults (`ApplyThemeDefaults(theme)`), then set instance overrides (`LegendStyle`, `StatsStyle`, `AxisStyle`, `BorderStyleText`, `FocusedBorderStyleText`).
- Bordered plotting controls (`Sparkline`, `AreaPlot`, `LinePlot`, `PlotPanel`) should use border style hooks instead of hardcoded emphasis markers.

### Wave 2 Data/Planning Controls

- `VirtualizedListView<T>`, `GroupedListView<TGroup,TItem>`, `KanbanBoard`, and `TagInput` are bordered controls with `BorderStyleText`/`FocusedBorderStyleText`.
- `CalendarMonthView` and `SchedulerTimeline` are unbordered planning views with full title/row/day style hooks.
- `PivotTable`, `QueryBuilder`, and `RichTextView` are also bordered controls with semantic style hooks and border token mapping.
- Theme helpers now cover all nine Wave 2 controls (`ApplyTheme` + `ApplyThemeDefaults` + override overloads).
- Recommended usage: call `ApplyThemeDefaults(runtime.Theme)` first, then instance overrides per screen state.

### Wave 3 Dev/Ops Controls

- Implemented controls: `JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`.
- `JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, and `ActivityFeed` are bordered controls with `BorderStyleText`/`FocusedBorderStyleText`.
- Theme helpers now cover all eight controls (`ApplyTheme` + `ApplyThemeDefaults` + override overloads).
- Recommended usage: `ApplyThemeDefaults(runtime.Theme)` first, then per-screen marker/style overrides.

### Beautiful UI Checklist (Current Phase)

- Apply semantic theme first (`TeaRuntimeOptions.Theme`), then control-type, instance, and state overrides.
- Use explicit focus/title hooks (`FocusMarker`, `ShowFocusMarker`, `TitleStyle`, `FocusedTitleStyle`) on interactive controls.
- Use border text hooks where supported (`BorderStyleText`, `FocusedBorderStyleText`) to avoid hardcoded frame emphasis.
- Use typed glyph sets for symbolic affordances (`DropdownGlyphSet`, `TreeViewGlyphSet`) instead of inline string literals.
- For any new bordered control, ship border hooks + theme token mapping + parity tests in the same slice.
- Parity policy drift is guarded by `BorderedControlParityPolicyTests.cs`.
- Keep monochrome-safe defaults when style hooks are left empty.

## Theme Mapping Snapshot

Current shipped `TeaThemeControlExtensions` mappings include:

- basic controls: `Label`, `Button`, `ListView<T>`, `StatusBar`, `TextInput`, `Table`, `Tabs`
- input/value controls: `TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`
- navigation controls: `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`
- navigation controls: `SearchResultsView` (row-state + border token mapping)
- navigation primitives: `Accordion`, `MultiSelect`, `RadioGroup`
- navigation overlay details: `Choice`/`ComboBox` include border text token mapping plus `DropdownGlyphSet` marker customization
- navigation overlay details: `TreeView` includes border text token mapping plus `TreeViewGlyphSet` marker customization
- navigation overlay details: `MenuBar`/`ContextMenu`/`CommandPalette` include border text token mapping plus typed glyph-set customization
- navigation overlay details: `Notifications` includes border text token mapping
- data/flow controls: `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`, `Stepper`
- data/flow details: `DataGrid` and `TreeTable` include border text token mapping plus explicit separator/marker text APIs
- data/flow details: `KeyValueList` and `Timeline` include border text token mapping
- planning/boards controls: `VirtualizedListView<T>`, `GroupedListView<TGroup,TItem>`, `KanbanBoard`, `TagInput`, `CalendarMonthView`, `SchedulerTimeline`
- planning/boards details: bordered controls in this set map border tokens; scheduler/calendar map semantic row/day/title tokens
- query/analytics controls: `PivotTable`, `QueryBuilder`, `RichTextView`
- query/analytics details: all three map semantic text/selection/focus/state tokens and bordered controls map border tokens by default
- explorer/feedback controls: `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`
- rendering text utilities: `Badge`, `LogView`, `MarkdownView`, `MiniLog`
- modal/chart summary controls: `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`
- modal/chart summary details: `Dialog` and `Modal` include border text token mapping
- plotting controls: `Sparkline`, `AreaPlot`, `ScatterPlot`, `Histogram`, `LinePlot`, `PlotPanel`
- plotting details: bordered plotting controls (`Sparkline`, `AreaPlot`, `LinePlot`, `PlotPanel`) include border text token mapping; `ScatterPlot`/`Histogram` map point or bar + axis + legend tokens
- dev/ops workflow controls: `JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`
- dev/ops details: bordered controls in this set map border text hooks to semantic border/focus tokens; inbox/help-dialog map semantic title/row/state tokens

## Advanced Seams

Use advanced seams only when your app needs runtime or rendering control beyond normal app authoring.

Common advanced seams:

- `TeaSharp.Hosting.TeaHost`
- `TeaSharp.Hosting.TeaHostingOptions`
- `TeaSharp.Hosting.IProgramRenderer`
- `TeaSharp.Hosting.ITerminalAdapter`
- `TeaSharp.Hosting.IEventDecoder`
- `Screen.From(LayoutNode)` and `Screen.From(ICanvasComponent)` for explicit advanced composition interop
- `TryConsume...` control methods for transitional advanced polling scenarios

## How They Fit Together

Most applications should stay on:

- `TeaApp`
- `Screen.Build(...)`
- `TeaSharp.Controls`
- `TeaSharp.Layout`

Advanced seams are supported, but they should be opt-in and uncommon for regular app teams.

## Example

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

internal sealed class ComboApp : TeaApp
{
    private readonly ComboBox _combo = new()
    {
        Title = "Environment",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    public ComboApp()
    {
        _combo.SetItems(["alpha", "beta", "gamma"]);
    }

    public override TeaEffect? Update(Message message) => null;

    public override Screen Build(ScreenContext context) =>
        Screen.Build(window =>
        {
            window.Body(new CenterLayout
            {
                Content = _combo,
                Width = 48,
                Height = 8,
            });
        });
}
```

## Example Apps

Current example projects:

- `examples/Showcase`
- `examples/Dropdown`
- `examples/ComboBox`
- `examples/ProductivityWidgets`
- `examples/AdvancedWidgets`
- `examples/Kanban`
- `examples/WidgetGallery`

All of these now run on the new `TeaApp` startup/composition path, even when they demonstrate advanced seams.
`examples/PlottingDashboard` will be the dedicated plotting/dashboard reference once it lands; until then use `WidgetGallery` and `AdvancedWidgets` for plotting composition patterns.

## Migration Guidance

- Prefer root control names when available.
- Treat older `*Component` names as transitional and avoid using them in new app code.
- Keep `TeaSharp.Controls` and `TeaSharp.Layout` as the main app imports.
- Reach for advanced seams only when the root catalog and runtime options cannot cover the scenario.

See also:

- `docs/migration-map.md`
- `docs/namespace-migration.md`
