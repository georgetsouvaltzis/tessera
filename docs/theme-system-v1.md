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

## V1 Rollout

Phase 1 (baseline controls):

- `Label`, `StatusBar`, `Button`, `TextInput`, `ListView<T>`, `Table`, `Tabs`, `Breadcrumb`, `Paginator`, `Toolbar`, `CommandBar`, `SearchBox`, `Dialog`, `ContextMenu`, `CommandPalette`

Phase 2 (data and advanced controls):

- `TreeView`, `DataGrid`, `TreeTable`, `KeyValueList`, `MenuBar`, `Notifications`, `DiffView`, `PropertyGrid`, `FileExplorer`, `FuzzyFinder`, `ToastCenter`, `Toggle`, `Slider`, `Spinner`, chart and log controls

Acceptance criteria:

- top controls expose style/theme override points with consistent naming
- focus, hover, selection, disabled, and error styles are configurable
- Catppuccin + Rosé Pine + custom palette can be applied without control-level rewiring
- docs and examples demonstrate global theme selection and per-control override
- `TeaSharp.Core.*` remains out of starter theming docs

## V1.1 Note

Image rendering is planned for V1.1 with capability-based backends and graceful fallback modes.
