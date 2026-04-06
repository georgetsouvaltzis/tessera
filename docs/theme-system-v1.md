# TeaSharp Theme System V1

## Scope

This document defines the V1 theming/styling contract for `TeaSharp` without DI and without engine leakage.

Goals:

- one consistent styling model across built-in controls
- semantic tokens instead of hardcoded colors in control code
- global defaults plus granular override points
- first-class support for built-in palettes and custom palettes

Out of scope for V1:

- inline image rendering in controls (target: V1.1)

## Theme Tokens

Theme values should be semantic, not control-specific:

- `Text.Primary`, `Text.Secondary`, `Text.Muted`, `Text.Inverse`
- `Surface.Base`, `Surface.Panel`, `Surface.Overlay`
- `Border.Default`, `Border.Strong`, `Border.Focused`, `Border.Error`
- `State.Success`, `State.Warning`, `State.Error`, `State.Info`
- `Accent.Primary`, `Accent.Secondary`
- `Selection.Background`, `Selection.Foreground`
- `Focus.Ring`, `Focus.Title`, `Focus.Border`, `Focus.Marker`

All tokens map to `TeaStyle` values and are consumable by controls without raw ANSI strings in app code.
`Focus.Marker` is a first-class token and should drive focus-marker rendering for controls that expose marker hooks (`FocusMarker`/`ShowFocusMarker`), rather than hardcoded marker styling.

## Consumer Hook Matrix (Quick Lookup)

Use these naming patterns on the no-DI public path (`Tea.RunAsync(new App())` or `Tea.CreateBuilder().UseApp<TApp>()...`):

| Family | Focus | Selected | Hovered | Border |
|---|---|---|---|---|
| Inputs/query (`TextInput`, `TextArea`, `SearchBox`) | `FocusMarker`, `ShowFocusMarker`, `FocusedTitleStyle` | control-specific `Selected*` hooks | control-specific `Hovered*` hooks | `BorderStyleText`, `FocusedBorderStyleText` |
| Navigation/list (`ListView<T>`, `TreeView`, `Choice`, `ComboBox`) | title focus hooks + marker hooks | `SelectedIndex`/`SelectedItem` + `Selected*Style` | `Hovered*Style` | `BorderStyleText`, `FocusedBorderStyleText` |
| Data/forms (`Table`, `DataGrid`, `TreeTable`, `Form`, `FieldSet`, `DataForm<TModel>`, `ValidationSummary`) | focused title/marker hooks | `Selected*Style` and row/cell markers | `Hovered*Style` | `BorderStyleText`, `FocusedBorderStyleText` |

Extended family-by-family matrix lives in [public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md).

## Typography Emphasis Intent

TeaSharp supports lightweight typography intent through `TeaFontWeight` and `TeaStyle.WithFontWeight(...)`.
This maps to ANSI SGR emphasis flags only (`Normal`, `Bold`, `Dim`).

It does **not** control terminal font engines, real font families, font sizes, or ligature behavior.

Experimental terminal font request is exposed separately through `ScreenOptions.FontSpec`:

```csharp
runtime.Screen = new ScreenOptions
{
    FontSpec = "Iosevka Term 14",
};
```

`FontSpec` behavior in TeaSharp V1:

- null/empty: no sequence emitted
- non-empty: renderer emits OSC 50 when value changes
- sanitization: strips `BEL`, `ESC`, `\`, and control chars
- reset: no forced font restore sequence

Cross-terminal support and terminal-specific caveats:
`docs/terminal-font-capability-matrix.md`.

## ANSI Style Foundations

Low-level ANSI style composition lives in `TeaSharp.Styles`:

- `AnsiColor`
  - `Indexed(0..255)`
  - `Rgb(r, g, b)`
- `TeaStyle`
  - `WithBold`
  - `WithUnderline`
  - `WithForeground`
  - `WithBackground`
  - `WithItalic`
  - `WithDim`
  - `WithInverse`
  - `WithFontWeight(...)`
  - `Merge(...)`
  - `ToEscapeSequence()`
  - `Render(string text)`

Use `TeaStyle` as the primitive value type behind theme tokens and per-control overrides. Public docs should treat raw ANSI styling as the foundation layer, and theme tokens as the preferred application-facing layer.

## Public API Names (V1 Foundations)

Theme primitives use the following public types:

- `TeaTheme`
- `TeaThemeTextTokens`
- `TeaThemeSurfaceTokens`
- `TeaThemeBorderTokens`
- `TeaThemeStateTokens`
- `TeaThemeAccentTokens`
- `TeaThemeSelectionTokens`
- `TeaThemeFocusTokens`
- `TeaThemes.Catppuccin(CatppuccinVariant)`
- `TeaThemes.RosePine(RosePineVariant)`
- `TeaThemeOverrides`
- `TeaThemeVisualState`
- `TeaThemeControlExtensions.ApplyTheme(...)`
- `TeaRuntimeOptions.Theme`
- `ScreenOptions.FontSpec` (experimental best-effort terminal font request)
- `TeaFontWeight`
- `TeaStyle.WithFontWeight(TeaFontWeight)`

## Theme Cookbook

### Select Catppuccin

```csharp
using TeaSharp;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<MyApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = TeaThemes.Catppuccin(CatppuccinVariant.Mocha);
    })
    .Build();
```

### Select Rosé Pine

```csharp
using TeaSharp;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<MyApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = TeaThemes.RosePine(RosePineVariant.Main);
    })
    .Build();
```

### Set a Custom Theme

```csharp
using TeaSharp;
using TeaSharp.Styles;

var baseTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);
var customTheme = new TeaTheme
{
    Text = baseTheme.Text,
    Surface = baseTheme.Surface,
    Border = new TeaThemeBorderTokens
    {
        Default = baseTheme.Border.Default,
        Strong = baseTheme.Border.Strong,
        Focused = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.Rgb(255, 184, 108)),
        Error = baseTheme.Border.Error,
    },
    State = baseTheme.State,
    Accent = baseTheme.Accent,
    Selection = baseTheme.Selection,
    Focus = baseTheme.Focus,
};

var app = Tea.CreateBuilder()
    .UseApp<MyApp>()
    .ConfigureRuntime(runtime => runtime.Theme = customTheme)
    .Build();
```

### Per-Control Overrides

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var button = new Button
{
    LabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
    FocusedLabelStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightWhite),
    SurfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(36, 24, 30)),
    FocusedSurfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(54, 36, 44)),
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(108, 68, 84)),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.Rgb(255, 184, 108)),
};

`Button` treats label styles as text-only semantics.
Use `LabelStyle` / `FocusedLabelStyle` / `PressedLabelStyle` for foreground and emphasis.
Use `SurfaceStyle` / `FocusedSurfaceStyle` / `PressedSurfaceStyle` for button-body fill.
Background-like label facets are ignored so rounded pills stay a single shell with a single body.
For pill/button-style controls, prefer a single coherent body surface plus border-led focus treatment.
Avoid layering a second chip-like background behind the label, because it breaks the intended box-model read and makes padding visually disappear.
Default button focus should come primarily from the shell/ring treatment; body fill should remain stable unless an app explicitly opts into a stronger pressed/focused tint.
If an app needs a distinct rounded outline with a separately filled inner body, set `RoundedSurfaceMode = ButtonRoundedSurfaceMode.InsetBody`.
That mode reserves a taller rounded box so the border shell and the filled body remain visually separate.
When the app keeps the default button label chrome, `InsetBody` also suppresses the default `[` `]` bracket treatment and adds minimum inner X breathing room automatically.
`RoundedSurfaceMode = ButtonRoundedSurfaceMode.UnifiedShell` is the filled-pill mode and should reserve enough vertical space plus inset shoulder/cap rows so the shell reads as a rounded pill instead of collapsing into a 3-row cutout or clipped octagon.

var list = new ListView<string>()
{
    DefaultRowStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
    HoveredRowStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightCyan),
    SelectedRowStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
};

var input = new TextInput
{
    ValueTextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
    PlaceholderTextStyle = TeaStyle.Empty.WithDim().WithForeground(AnsiColor.BrightBlack),
    FocusedTitleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow),
};
```

### Overlay Glyph APIs (MenuBar, ContextMenu, CommandPalette)

```csharp
using TeaSharp.Controls;

var menuBar = new MenuBar
{
    Border = BorderStyle.SingleLine,
    Glyphs = new MenuBarGlyphSet("[", "]", " ", " ", "{", "}", "<", ">"),
};

var contextMenu = new ContextMenu
{
    Border = BorderStyle.Rounded,
    ShowFocusMarker = true,
    Glyphs = new ContextMenuGlyphSet("·", ">", "▸", " "),
};

var commandPalette = new CommandPalette
{
    Border = BorderStyle.Rounded,
    ShowFocusMarker = true,
    Glyphs = new CommandPaletteGlyphSet("❯", " ", ">", "▸", " "),
};
```

### Border Override APIs (High-Use Controls)

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var input = new TextInput
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightCyan),
};

var choice = new Choice
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
};

var grid = new DataGrid
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow),
};
```

### Visual Polish Patterns (Choice/ComboBox/TreeView)

Use the same three-layer pattern for polished defaults and app-specific overrides:

1. border hook override (`BorderStyleText`, `FocusedBorderStyleText`)
2. typed glyph set override (`DropdownGlyphSet`, `TreeViewGlyphSet`)
3. text-state override (`TitleStyle`, `FocusedTitleStyle`, row/item styles)

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var choice = new Choice
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
    Glyphs = new DropdownGlyphSet("▾", "▴", ">", "✓"),
    TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
    FocusedTitleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightCyan),
};

var comboBox = new ComboBox
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow),
    Glyphs = new DropdownGlyphSet("▼", "▲", "•", "✓"),
};

var treeView = new TreeView
{
    BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
    FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightMagenta),
    Glyphs = new TreeViewGlyphSet("▼", "▶", "•"),
    TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
    FocusedTitleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightMagenta),
};
```

### Wave 1 App Shell + Forms Theme Hooks

`TeaThemeControlExtensions.FormsAndShell.cs` includes explicit mappings for:

- `Form`
- `FieldSet`
- `DataForm<TModel>`
- `Wizard`
- `SplitView`
- `InspectorPanel`

These controls map `BorderStyleText` and `FocusedBorderStyleText` to semantic border/focus tokens by default.

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var theme = TeaThemes.Catppuccin(CatppuccinVariant.Frappe);

var form = new Form().ApplyThemeDefaults(theme);
form.RequiredMarkerStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightRed);

var dataForm = new DataForm<object>().ApplyThemeDefaults(theme);
dataForm.ErrorStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightRed);

var wizard = new Wizard().ApplyThemeDefaults(theme);
wizard.ActiveStepStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightCyan);

var split = new SplitView().ApplyThemeDefaults(theme);
split.FocusedDividerStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow);
```

### Wave 2 Data/Planning/Query Theme Hooks

`TeaThemeControlExtensions` includes explicit mappings for:

- `VirtualizedListView<T>`
- `GroupedListView<TGroup,TItem>`
- `PivotTable`
- `QueryBuilder`
- `KanbanBoard`
- `TagInput`
- `CalendarMonthView`
- `SchedulerTimeline`
- `RichTextView`

Bordered controls in this set (`VirtualizedListView<T>`, `GroupedListView<TGroup,TItem>`, `PivotTable`, `QueryBuilder`, `KanbanBoard`, `TagInput`, `RichTextView`) map `BorderStyleText` and `FocusedBorderStyleText` to semantic border/focus tokens by default.

`TagInput` also maps `CaretStyle` to `theme.Focus.Ring` by default, while placeholder tint remains controllable through `PlaceholderTextStyle`.

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var theme = TeaThemes.Catppuccin(CatppuccinVariant.Mocha);

var kanban = new KanbanBoard().ApplyThemeDefaults(theme);
kanban.BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack);
kanban.FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow);

var scheduler = new SchedulerTimeline().ApplyThemeDefaults(theme);
scheduler.ConflictRowStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightRed);

var query = new QueryBuilder().ApplyThemeDefaults(theme);
query.ErrorRuleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightRed);

var richText = new RichTextView().ApplyThemeDefaults(theme);
richText.InlineCodeStyle = TeaStyle.Empty.WithBackground(AnsiColor.BrightBlack).WithForeground(AnsiColor.BrightWhite);
```

### Wave 3 Dev/Ops Theme Hooks

`TeaThemeControlExtensions` includes explicit mappings for:

- `JsonTreeView`
- `TraceViewer`
- `CommandOutput`
- `LogTailPanel`
- `TaskRunnerPanel`
- `ActivityFeed`
- `NotificationInbox`
- `KeyBindingHelpDialog`

Bordered controls in this set (`JsonTreeView`, `TraceViewer`, `CommandOutput`, `LogTailPanel`, `TaskRunnerPanel`, `ActivityFeed`) map `BorderStyleText` and `FocusedBorderStyleText` to semantic border/focus tokens by default.

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var theme = TeaThemes.RosePine(RosePineVariant.Main);

var logs = new LogTailPanel().ApplyThemeDefaults(theme);
logs.BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack);
logs.FocusedBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow);

var trace = new TraceViewer().ApplyThemeDefaults(theme);
trace.WarningRowStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow);

var tasks = new TaskRunnerPanel().ApplyThemeDefaults(theme);
tasks.FailedStatusStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightRed);

var inbox = new NotificationInbox().ApplyThemeDefaults(theme);
inbox.PinnedItemStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightCyan);

var help = new KeyBindingHelpDialog().ApplyThemeDefaults(theme);
help.KeysStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightWhite);
```

### Wave 4 Workspace + Visual Data Theme Hooks (Batch A + B)

`TeaThemeControlExtensions.Workspace.cs` includes explicit mappings for:

- `DockWorkspace`
- `PaneTabs`
- `PaletteEditor`
- `Heatmap`
- `TreeMapChart`
- `TerminalPanel`
- `ProcessListView`

Bordered controls in this set (`DockWorkspace`, `PaneTabs`, `Heatmap`, `TreeMapChart`, `ProcessListView`) map `BorderStyleText` and `FocusedBorderStyleText` to semantic border/focus tokens by default.

```csharp
using TeaSharp.Controls;
using TeaSharp.Styles;

var theme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

var workspace = new DockWorkspace().ApplyThemeDefaults(theme);
workspace.FocusedPaneBorderStyleText = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow);

var tabs = new PaneTabs().ApplyThemeDefaults(theme);
tabs.HoveredTabStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightCyan);

var heatmap = new Heatmap().ApplyThemeDefaults(theme);
heatmap.PeakCellStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen);

var processList = new ProcessListView().ApplyThemeDefaults(theme);
processList.HeaderStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightWhite);

var terminal = new TerminalPanel().ApplyThemeDefaults(theme);
terminal.StandardErrorStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightRed);
```

### Dropdown and Tree Glyph Sets

```csharp
using TeaSharp.Controls;

var combo = new ComboBox
{
    Glyphs = new DropdownGlyphSet("v", "^", ">", "+"),
};

var tree = new TreeView
{
    Glyphs = new TreeViewGlyphSet("▼", "▶", "•"),
};
```

### Search Results Glyph Set

```csharp
using TeaSharp.Controls;

var results = new SearchResultsView
{
    Glyphs = new SearchResultsGlyphSet("·", "▸", "▶", "~", "."),
    ShowRankMarker = true,
};
```

### Data Widget Marker and Separator APIs

```csharp
using TeaSharp.Controls;

var dataGrid = new DataGrid
{
    ColumnSeparatorText = " │ ",
    SortAscendingMarker = " ↑",
    SortDescendingMarker = " ↓",
};

var treeTable = new TreeTable("Name", "Value")
{
    ColumnSeparatorText = " │ ",
    SelectedRowMarker = ">",
    UnselectedRowMarker = " ",
    ExpandedBranchMarker = "▼",
    CollapsedBranchMarker = "▶",
    LeafMarker = "•",
};
```

## Palette Model

V1 ships with:

- Catppuccin variants (`Latte`, `Frappe`, `Macchiato`, `Mocha`)
- Rosé Pine variants (`Main`, `Moon`, `Dawn`)
- custom user palette from strongly typed theme objects

Palette selection is runtime-configurable and does not require app architecture patterns beyond `TeaApp` + `TeaRuntimeOptions`.

## Override Hierarchy

Style resolution order (lowest to highest precedence):

1. framework default theme
2. selected palette
3. global app theme overrides
4. control-type theme overrides (for example all `Button`)
5. control instance overrides
6. state overrides (`Focused`, `Hovered`, `Selected`, `Disabled`, `Error`, `Active`)

This hierarchy allows global consistency with explicit local escape hatches.

## Visual State Policy

Default policy for all controls:

- focus is visualized by themeable border/title style (not only `"*"` markers)
- selected and hovered states are visually distinct in monochrome-safe and color-capable terminals
- error/warning/success states are token-driven and accessible by contrast
- disabled/read-only states are clearly lower emphasis, still readable

## Implemented Extension Layout

`TeaThemeControlExtensions` is split into domain partial files:

- `TeaThemeControlExtensions.Basic.cs`
- `TeaThemeControlExtensions.InputValue.cs`
- `TeaThemeControlExtensions.Navigation.cs`
- `TeaThemeControlExtensions.NavigationOverlay.cs`
- `TeaThemeControlExtensions.NavigationPrimitives.cs`
- `TeaThemeControlExtensions.DataAndFlow.cs`
- `TeaThemeControlExtensions.ExplorerAndFeedback.cs`
- `TeaThemeControlExtensions.RenderingTextUtilities.cs`
- `TeaThemeControlExtensions.ModalAndCharts.cs`
- `TeaThemeControlExtensions.Plotting.cs`
- `TeaThemeControlExtensions.DevOpsAndWorkflows.cs`
- `TeaThemeControlExtensions.Workspace.cs`
- `TeaThemeControlExtensions.FormsAndShell.cs`

Mapped controls expose:

- `ApplyTheme(TeaTheme theme)`
- `ApplyThemeDefaults(TeaTheme theme)`
- plotting mappings: `Sparkline`, `TelemetryChart`, `AreaPlot`, `ScatterPlot`, `Histogram`, `LinePlot`, `PlotPanel`
- workspace mappings: `DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`
- app-shell/forms mappings: `Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`
- overloads taking `TeaThemeOverrides`, `baseTheme`, and `TeaThemeVisualState`

Input/value mapping coverage includes:

- `TextInput`, `TextArea`, `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `NumberInput`, `DatePicker`, `TimePicker`
- `TextInput` maps value/placeholder/focused-title styles; title marker remains configurable via `FocusMarker` + `ShowFocusMarker`
- `TextInput` maps `BorderStyleText` -> `theme.Border.Default` and `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`
- `Toggle`, `Slider`, `Spinner`, and `ProgressBar` map `BorderStyleText` -> `theme.Border.Default` and `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`
- `TextArea`, `NumberInput`, `DatePicker`, and `TimePicker` map `BorderStyleText` -> `theme.Border.Default` and `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`

Basic mapping coverage includes:

- `Label`, `Button`, `ListView<T>`, `StatusBar`, `TextInput`, `Table`, `Tabs`
- `Label` and `Button` map `BorderStyleText` -> `theme.Border.Default` and `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`
- `Button` also maps `SurfaceStyle` -> `theme.Surface.Panel`, `FocusedSurfaceStyle` -> `theme.Surface.Panel.Merge(theme.Focus.Border)`, and `PressedSurfaceStyle` -> `theme.Selection.Background`
- `Button` label styles are text-only; body/background comes from button surface styles
- `Table` maps `BorderStyleText` -> `theme.Border.Default` and `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`

Navigation mapping coverage includes:

- `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`, `SearchResultsView`
- `SearchBox` maps title/value/placeholder/match/navigation styles plus border text hooks
- `SearchResultsView` maps title, row-state, and border text hooks; row markers are customizable through `SearchResultsGlyphSet`

Navigation/overlay mapping coverage includes:

- `Choice`, `ComboBox`, `TreeView`, `MenuBar`, `ContextMenu`, `CommandPalette`, `Notifications`, `SearchBox`
- `Choice`/`ComboBox` map `BorderStyleText` -> `theme.Border.Default`
- `Choice`/`ComboBox` map `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`
- `Choice`/`ComboBox` expose `Glyphs` via `DropdownGlyphSet` for closed/open/highlight/selected markers
- `TreeView` maps border text hooks and title focus marker; branch/leaf markers are configurable through `TreeViewGlyphSet`
- `MenuBar`, `ContextMenu`, and `CommandPalette` map `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens
- `MenuBar`, `ContextMenu`, and `CommandPalette` expose typed glyph configuration (`MenuBarGlyphSet`, `ContextMenuGlyphSet`, `CommandPaletteGlyphSet`)
- `Notifications` maps `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens
- `ContextMenu` preserves focused title markers in bordered title rendering width calculations
- `SearchBox` maps title/value/placeholder/match/navigation styles plus border text hooks; title marker is configurable through `FocusMarker` + `ShowFocusMarker`

Navigation primitive mapping coverage includes:

- `Accordion`, `MultiSelect`, `RadioGroup`

Rendering text utility mapping coverage includes:

- `Badge`, `LogView`, `MarkdownView`, `MiniLog`
- `LogView` and `MarkdownView` map `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens

Modal/chart summary mapping coverage includes:

- `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`
- `Dialog` and `Modal` map border text hooks to semantic border/focus tokens

Data/flow mapping coverage includes:

- `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`, `Stepper`
- `DataGrid` maps border text hooks and exposes `ColumnSeparatorText` plus sort marker text APIs
- `TreeTable` maps border text hooks and exposes `ColumnSeparatorText`, row marker text APIs, and branch/leaf marker text APIs
- `KeyValueList` and `Timeline` map border text hooks to semantic border/focus tokens

Explorer/feedback mapping coverage includes:

- `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`
- `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, and `ToastCenter` map `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens

Dev/ops workflow mapping coverage includes:

- `JsonTreeView`, `CommandOutput`, `LogTailPanel`, `ActivityFeed`, `NotificationInbox`, `KeyBindingHelpDialog`
- bordered controls in this set map `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens

Workspace/visual-data mapping coverage includes:

- `DockWorkspace`, `PaneTabs`, `PaletteEditor`, `Heatmap`, `TreeMapChart`, `TerminalPanel`, `ProcessListView`
- bordered controls in this set map `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens

App-shell/forms mapping coverage includes:

- `Form`, `FieldSet`, `DataForm<TModel>`, `Wizard`, `SplitView`, `InspectorPanel`
- bordered controls in this set map `BorderStyleText` + `FocusedBorderStyleText` to border/focus tokens

Bordered control parity enforcement:

- new bordered controls must expose `BorderStyleText` and `FocusedBorderStyleText`
- new bordered controls must map those hooks to `theme.Border.Default` and `theme.Border.Focused.Merge(theme.Focus.Border)`
- test enforcement is maintained through `ThemeOverridesTests.*` token-mapping suites, `VisualParityTests` edge-case coverage, and `BorderedControlParityPolicyTests.cs`
- bordered-control parity rollout is complete for current shipped controls; policy remains forward-only for newly added controls

## V1 Rollout

Phase 1 (baseline controls):

- `Label`, `StatusBar`, `Button`, `TextInput`, `ListView<T>`, `Table`, `Tabs`, `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`, `Dialog`, `ContextMenu`, `CommandPalette`

Phase 2 (data and advanced controls):

- `TreeView`, `DataGrid`, `TreeTable`, `KeyValueList`, `Timeline`, `Stepper`, `MenuBar`, `Notifications`, `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`, `Toggle`, `Slider`, `Spinner`, `Accordion`, `MultiSelect`, `RadioGroup`, `Badge`, `LogView`, `MarkdownView`, `MiniLog`, `Dialog`, `Modal`, `BarChart`, `LineChart`, `Gauge`, `StatsCard`

Acceptance criteria:

- top controls expose style/theme override points with consistent naming
- focus, hover, selection, disabled, and error styles are configurable
- Catppuccin + Rosé Pine + custom palette can be applied without control-level rewiring
- docs and examples demonstrate global theme selection and per-control override
- `TeaSharp.Core.*` remains out of starter theming docs

## V1.1 Note

Image rendering is planned for V1.1 with capability-based backends and graceful fallback modes.
