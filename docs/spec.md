# TeaSharp Design Spec

## Overview

TeaSharp is a `.NET 10` terminal UI framework for state-driven applications.

Current design center:

- small root API
- explicit C# object model
- TeaSharp-owned startup
- screen/layout/control composition
- stable custom control contract
- advanced runtime seams kept separate from the default path

TeaSharp is pre-public. Breaking changes are allowed when they simplify the long-term API.

## Goals

- Keep the default authoring model learnable in one sitting.
- Prefer explicit object models over nested mini-DSLs.
- Let normal apps build screens without region ids, input scopes, or manual terminal bookkeeping.
- Preserve strong extensibility for custom widgets and advanced hosting.
- Keep rendering deterministic and testable.

## Non-Goals

- Generic Host as the framework identity.
- Prompt-style console helpers.
- Reproducing Terminal.Gui or Spectre.Console API shape.
- Requiring application architecture patterns such as repository, MVVM, CQRS, or mediator.

## Public Architecture

### Root Surface

Default namespaces:

- `TeaSharp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`
- `TeaSharp.Styles`

Advanced namespace:

- `TeaSharp.Hosting`

The normal app path should not import `TeaSharp.Core.*`.

### Application Model

Primary app contract:

- `TeaApp`
- `Tea.RunAsync(...)`
- `Tea.CreateBuilder()`
- `TeaApplicationBuilder.UseApp<TApp>()`
- `TeaApplicationBuilder`
- `TeaApplication`
- `TeaRuntimeOptions`
- `Screen`
- `ScreenContext`
- `ScreenOptions`
- `Message`
- `TeaEffect`
- `TeaEffects`

App model shape:

1. `Initialize()` optionally returns the first effect.
2. `Update(Message)` handles typed input/runtime messages.
3. `Build(ScreenContext)` returns the next assembled screen.

Startup model:

- minimal path: `Tea.RunAsync(new App())`
- configured path: `Tea.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`

Canonical onboarding progression:

1. `examples/HelloWorld`: minimal startup path.
2. `examples/CounterForm`: configured startup path (`UseApp<TApp>()` + `ConfigureRuntime(...)`).
3. `examples/WorkspaceApp`: stateful multi-pane coordination with app-level messages/effects.
4. Advanced interaction lane: `examples/AdvancedWidgets` and `examples/WidgetGallery`.

Default onboarding remains in `TeaSharp`. `TeaSharp.Core` is a low-level advanced product lane.

### Theme Model

V1 theming is semantic-token based with override hierarchy:

- semantic tokens for text/surface/border/state/focus/selection/accent
- `Focus.Marker` is first-class and wired across focus-marker controls (controls exposing `FocusMarker`/`ShowFocusMarker`)
- built-in palettes (Catppuccin, Rosé Pine) plus custom palette
- override precedence: global theme -> control type -> control instance -> state
- consumer hook quick-lookup matrix is documented in [public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- `TeaThemeControlExtensions` is split by domain (`Basic`, `InputValue`, `Navigation`, `NavigationOverlay`, `NavigationPrimitives`, `DataAndFlow`, `PlanningAndBoards`, `QueryAndRichText`, `ExplorerAndFeedback`, `RenderingTextUtilities`, `ModalAndCharts`, `Plotting`, `DevOpsAndWorkflows`, `Workspace`, `FormsAndShell`)
- mapped input/value controls include `TextInput`, `TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`
- mapped basic controls include `Label`, `Button`, `ListView<T>`, `StatusBar`, `TextInput`, `Table`, `Tabs`
- mapped navigation controls include `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`, `SearchResultsView`
- mapped navigation/overlay controls include `Choice`, `ComboBox`, `TreeView`, `MenuBar`, `ContextMenu`, `CommandPalette`, `Notifications`, `SearchBox`
- `Choice` and `ComboBox` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- dropdown marker customization is explicit through `DropdownGlyphSet` on `Choice` and `ComboBox`
- `Table` and `TreeView` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `TreeView` marker customization is explicit through `TreeViewGlyphSet`
- `TextArea`, `NumberInput`, `DatePicker`, and `TimePicker` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `Toggle`, `Slider`, `Spinner`, and `ProgressBar` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `MenuBar`, `ContextMenu`, and `CommandPalette` map border text hooks and expose typed glyph-set customization
- `Notifications`, `LogView`, and `MarkdownView` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, and `ToastCenter` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `ListView<T>` and `TreeView` expose `FocusMarker` + `ShowFocusMarker` for explicit focus-title rendering
- `TextInput` and `SearchBox` expose focus-title marker customization plus title/value/placeholder and border style hooks
- `SearchResultsView` exposes focus-title markers, row-state styles, border style hooks, and typed marker customization through `SearchResultsGlyphSet`
- `DataGrid` and `TreeTable` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `DataGrid` and `TreeTable` expose explicit text-based visual hooks for separators and selection/sort/tree markers
- `KeyValueList` and `Timeline` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- `ContextMenu` preserves focused title marker text when rendering bordered titles
- parity rule: any new bordered public control must ship border-style hooks, token mappings, and parity regression coverage in the same change
- mapped navigation primitive controls include `Accordion`, `MultiSelect`, `RadioGroup`
- mapped app-shell/forms controls include `Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`
- bordered app-shell/forms controls (`Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`) map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- mapped data/flow controls include `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`, `Stepper`
- mapped planning/boards controls include `VirtualizedListView<T>`, `GroupedListView<TGroup,TItem>`, `KanbanBoard`, `TagInput`, `CalendarMonthView`, `SchedulerTimeline`
- bordered planning/boards controls (`VirtualizedListView<T>`, `GroupedListView<TGroup,TItem>`, `KanbanBoard`, `TagInput`) map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- mapped query/analytics controls include `PivotTable`, `QueryBuilder`, `RichTextView`
- bordered query/analytics controls (`PivotTable`, `QueryBuilder`, `RichTextView`) map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- mapped explorer/feedback controls include `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`
- mapped dev/ops workflow controls include `JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`
- notification guidance: use `Notifications` as the default/onboarding notification feed; treat `NotificationInbox` as advanced/devops workflow surface
- selection naming guidance: treat `Selected*` as canonical; compatibility `Current*` members remain in V1 where present and follow [post-v1-selection-naming-migration.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/post-v1-selection-naming-migration.md)
- bordered dev/ops workflow controls (`JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`) map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default
- mapped workspace/visual-data controls include `DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`
- `TeaThemeControlExtensions.Workspace.cs` maps all workspace/visual-data controls in this set
- mapped rendering text utility controls include `Badge`, `LogView`, `MarkdownView`, `MiniLog`
- mapped modal/chart summary controls include `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`
- mapped plotting controls include `Sparkline`, `AreaPlot`, `ScatterPlot`, `Histogram`, `LinePlot`, `PlotPanel`
- expansion tranche controls (`DashboardGrid`, `QuickOpenOverlay`, `BulletChart`, `ResizablePaneGroup`, `SideNavRail`, `TokenEditor`, `HealthBoard`) are implemented with deterministic tests; theme extension defaults are currently wired for `BulletChart` and `DashboardGrid`, while the remaining controls still need typed `ApplyTheme`/`ApplyThemeDefaults` parity wiring
- `Dialog` and `Modal` map `BorderStyleText`/`FocusedBorderStyleText` to semantic border/focus tokens by default

Focus visuals must be theme-driven (for example focused border style/color), not limited to marker suffixes.

Visual polish override recipe for navigation inputs:

- set frame hooks first (`BorderStyleText`, `FocusedBorderStyleText`)
- set typed markers second (`DropdownGlyphSet` for `Choice`/`ComboBox`, `TreeViewGlyphSet` for `TreeView`)
- set title/row text state hooks third (`TitleStyle`, `FocusedTitleStyle`, selected/hovered styles)

Minimal C# pattern:

```csharp
var choice = new Choice
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
    Glyphs = new DropdownGlyphSet("▾", "▴", ">", "✓"),
};

var tree = new TreeView
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightMagenta),
    Glyphs = new TreeViewGlyphSet("▼", "▶", "•"),
};
```

Image rendering is planned for V1.1.

### Typography Portability

- Portable typography in TeaSharp is ANSI SGR emphasis intent (for example bold/dim) and should be treated as styling intent, not a real font engine contract.
- Terminal-facing font requests are best-effort and explicitly opt-in through `ScreenOptions`:
  - `FontSpec`: legacy/explicit raw request lane.
  - `FontFamily` + `FontSize`: structured request lane.
  - `Iterm2Profile`: iTerm2 profile-switch lane.
- Behavior is capability-gated by terminal detection flags:
  - `SupportsOsc50FontRequests`
  - `SupportsIterm2ProfileRequests`
- Preference rule: when `Iterm2Profile` is set and `SupportsIterm2ProfileRequests` is true, renderer prefers iTerm2 profile switching over OSC 50 font requests.
- Reset semantics remain conservative: TeaSharp does not force unknown font restore.
- Terminal-by-terminal guidance lives in [terminal-font-capability-matrix.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/terminal-font-capability-matrix.md).

Preferred C# usage:

```csharp
runtime.Screen = new ScreenOptions
{
    FontSpec = "JetBrains Mono 13", // legacy/raw OSC 50 request
    FontFamily = "JetBrains Mono",  // structured request
    FontSize = 13,
    Iterm2Profile = "TeaSharp",
};
```

### Composition Model

TeaSharp uses an object-based screen model.

Core default layout types:

- `WindowLayout`
- `RowLayout`
- `ColumnLayout`
- `PanelLayout`
- `CenterLayout`
- `LayoutSlot`
- `LayoutLength`

The default authoring model should read like explicit screen assembly, not nested layout-tree construction.

### Control Model

Root controls currently include:

- `Label`
- `Button`
- `Breadcrumb`
- `TextInput`
- `TextArea`
- `Choice`
- `ComboBox`
- `DropdownGlyphSet`
- `Dialog`
- `CommandPalette`
- `CommandPaletteGlyphSet`
- `ContextMenu`
- `ContextMenuGlyphSet`
- `Notifications`
- `Toggle`
- `Slider`
- `Spinner`
- `StatusBar`
- `Tabs`
- `ListView<T>`
- `VirtualizedListView<T>`
- `VirtualizedListViewOptions`
- `GroupedListView<TGroup,TItem>`
- `GroupedListViewGroup<TGroup,TItem>`
- `GroupedListSelectionChangedEventArgs<TGroup,TItem>`
- `Table`
- `KanbanBoard`
- `KanbanLane`
- `KanbanCard`
- `KanbanSelectionChangedEventArgs`
- `TagInput`
- `CalendarMonthView`
- `CalendarDayCell`
- `CalendarDateSelectedEventArgs`
- `SchedulerTimeline`
- `SchedulerEntry`
- `SchedulerSelectionChangedEventArgs`
- `PivotTable`
- `PivotTableColumn`
- `PivotTableCell`
- `PivotSortDirection`
- `PivotSortRequestedEventArgs`
- `QueryBuilder`
- `QueryGroup`
- `QueryRule`
- `QueryOperator`
- `QueryChangedEventArgs`
- `RichTextView`
- `RichTextSegment`
- `RichTextStyleKind`
- `JsonTreeView`
- `JsonTreeNode`
- `JsonTreeNodeKind`
- `JsonTreeSelectionChangedEventArgs`
- `TraceViewer`
- `TraceEntry`
- `TraceSelectionChangedEventArgs`
- `CommandOutput`
- `CommandOutputLine`
- `CommandOutputChannel`
- `LogTailPanel`
- `LogEntry`
- `LogLevel`
- `TaskRunnerPanel`
- `TaskRunItem`
- `TaskRunnerSelectionChangedEventArgs`
- `DockWorkspace`
- `DockPane`
- `DockPanePosition`
- `PaneTabs`
- `PaneTabItem`
- `PaneTabSelectionChangedEventArgs`
- `PaletteEditor`
- `PaletteSwatch`
- `PaletteSelectionChangedEventArgs`
- `Heatmap`
- `HeatmapCell`
- `HeatmapLegend`
- `TreeMapChart`
- `TreeMapNode`
- `TerminalPanel`
- `TerminalPanelLine`
- `TerminalPanelChannel`
- `ProcessListView`
- `ProcessListEntry`
- `ProcessListStatus`
- `ProcessListSelectionChangedEventArgs`
- `ActivityFeed`
- `ActivityFeedItem`
- `ActivityFeedItemKind`
- `NotificationInbox` (advanced dev/ops inbox surface)
- `InboxItem`
- `KeyBindingHelpDialog`
- `KeyBindingItem`
- `DataGrid`
- `TreeTable`
- `KeyValueList`
- `Timeline`
- `Stepper`
- `TreeItem`
- `TreeView`
- `TreeViewGlyphSet`
- `MenuBar`
- `MenuBarGlyphSet`
- `Toolbar`
- `CommandBar`
- `NumberInput`
- `DatePicker`
- `TimePicker`
- `MarkdownView`
- `MultiSelect`
- `Paginator`
- `SearchBox`
- `SearchResultsView`
- `SearchResultsGlyphSet`
- `Form`
- `FormField`
- `DataForm<TModel>`
- `DataFormField<TModel>`
- `DataFormSelectionChangedEventArgs<TModel>`
- `DataFormFieldCommittedEventArgs<TModel>`
- `FieldSet`
- `SplitView`
- `SplitViewOrientation`
- `InspectorPanel`
- `InspectorSection`
- `InspectorField`
- `Wizard`
- `WizardStep`
- `WizardStepChangedEventArgs`
- `DiffView`
- `PropertyGrid`
- `FileExplorer`
- `FuzzyFinder`
- `ToastCenter`
- `RadioGroup`
- `ProgressBar`
- `LogView`
- `Badge`
- `Accordion`
- `Modal`
- `Gauge`
- `MiniLog`
- `StatsCard`
- `BarChart`
- `LineChart`
- `Sparkline`
- `AreaPlot`
- `BulletChart`
- `BulletRange`
- `BulletRangeKind`
- `DashboardGrid`
- `DashboardTile`
- `QuickOpenOverlay`
- `QuickOpenItem`
- `QuickOpenOverlayGlyphSet`
- `QuickOpenOverlaySubmittedEventArgs`
- `ResizablePaneGroup`
- `PaneSpec`
- `SideNavRail`
- `NavItem`
- `SideNavRailGlyphSet`
- `SideNavRailSelectionChangedEventArgs`
- `SideNavRailActivatedEventArgs`
- `TokenEditor`
- `TokenItem`
- `TokenEditorGlyphSet`
- `TokenEditorSelectionChangedEventArgs`
- `HealthBoard`
- `HealthService`
- `HealthServiceSeverity`
- `HealthBoardGlyphSet`
- `ScatterPlot`
- `ScatterPlotPoint`
- `Histogram`
- `HistogramBucket`
- `LinePlot`
- `LineSeries`
- `LinePlotOptions`
- `PlotPanel`
- `PlotPanelOptions`

These types provide the default control vocabulary. Most promoted legacy `*Component` names are now internal bridges behind these controls.

### Notification Surface Guidance

- primary path: `Notifications`
- advanced path: `NotificationInbox`
- shared item model: `InboxItem`

`Notifications` primary API includes:

- `Items`
- `SelectedIndex`
- `SelectedItem`
- `SelectionChanged`
- `SetItems(IEnumerable<InboxItem>)`
- `Add(InboxItem)`
- `SetSelectedIndex(int)` / `Select(int)`
- `MarkAllRead()`
- `RemoveSelected()`
- `Push(...)` remains supported for append-by-message workflows

### Plotting and Dashboard Authoring Guidance

Recommended control selection:

- `Sparkline`: compact single metric, fast append path, bounded by constructor capacity.
- `AreaPlot`: single metric with fill semantics.
- `LinePlot` + `LineSeries`: multi-series trend dashboards.
- `ScatterPlot`: correlation analysis (X/Y points).
- `Histogram`: bucketed distributions.
- `PlotPanel`: container for composing multiple plot controls in grid-like dashboards.

Streaming guidance:

- Keep producers bounded. For `Sparkline`/`AreaPlot`, use capacity constructors and `Append`.
- For `LinePlot`, keep bounded external buffers per series and call `LineSeries.SetSamples(...)`.
- Reuse control instances and update data only; avoid rebuilding controls each frame.

Theming guidance:

- Apply semantic defaults first (`ApplyThemeDefaults`), then instance overrides.
- Bordered plotting controls expose `BorderStyleText` and `FocusedBorderStyleText`; use those hooks for focus emphasis instead of hardcoded symbols.

Reference example:

- `examples/PlottingDashboard` is the planned canonical plotting dashboard sample once available.

### Custom Control Model

Custom widgets extend `TeaSharp.Controls.Control`.

That contract gives:

- render hook through `Render(Canvas, Rect)`
- typed message hook through `Handle(Message)`
- optional pointer-aware hook through `Handle(Message, Rect)`
- automatic bridge into the current runtime/composition engine without exposing the legacy component interfaces on the default path

The legacy component contracts still exist for advanced interop, but they are intentionally marked advanced and are no longer part of the normal custom-widget story.

Design rule:

- users should be able to write custom widgets without understanding `ScreenComposer`, routing scopes, or terminal protocol details

## Internal Architecture

### Runtime

The current runtime still uses the original core engine:

- internal `TeaRuntimeLoop`
- terminal adapters
- decoder
- renderer
- effect scheduling

Those remain the execution backend while the new root API compiles into them.

### Screen Compilation

The root screen model compiles through:

1. layout tree normalization
2. scene graph compilation
3. focus/input routing
4. canvas rendering
5. terminal output emission

The old public composition engine has been removed. Remaining advanced bridges are internal implementation details, not part of the app contract.

### Interaction

Default app code uses:

- automatic control input dispatch before `TeaApp.Update(...)`
- `TeaApp.Update(...)` for unhandled input plus runtime messages
- `RequestEffect(...)` when a control event needs to trigger runtime work
- typed key messages such as `KeyPressed`
- typed pointer messages such as `PointerInput`
- `TeaEffects` for quit/tick/sequence/batch behavior

Normal apps should not manually configure `InputRouter`, `InputScope`, or screen region chains.

## Advanced Layer

Advanced/custom-host scenarios can still reach:

- low-level renderers
- terminal capability probes
- decoder seams
- raw canvas drawing
- legacy composition helpers
- legacy `*Component` types without root wrappers yet

Most promoted legacy `*Component` families are now internal bridges behind root `TeaSharp.Controls` wrappers. The remaining public advanced layer is mainly hosting/runtime seams plus a smaller set of explicit interop contracts.

## Repo Profile

- SDK pinned: `10.0.103`
- main solution: `TeaSharp.slnx`
- test projects:
  - `tests/TeaSharp.Tests`
  - `tests/TeaSharp.IntegrationTests`

## Design Rules

- One obvious startup path.
- One obvious composition path.
- One obvious root control catalog.
- No namespace/type collisions on the public path.
- No stringly-typed routing identifiers on the normal path.
- No bool-heavy public orchestration APIs when a stronger object model is available.
- Simplicity for common apps; power for advanced users through deliberate extension points.
