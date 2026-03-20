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
- `Focus.Ring`, `Focus.Title`, `Focus.Border`

All tokens map to `TeaStyle` values and are consumable by controls without raw ANSI strings in app code.

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
    FocusedLabelStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightYellow),
};

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

Mapped controls expose:

- `ApplyTheme(TeaTheme theme)`
- `ApplyThemeDefaults(TeaTheme theme)`
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
- `Table` maps `BorderStyleText` -> `theme.Border.Default` and `FocusedBorderStyleText` -> `theme.Border.Focused.Merge(theme.Focus.Border)`

Navigation mapping coverage includes:

- `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`
- `SearchBox` maps title/value/placeholder/match/navigation styles plus border text hooks

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
