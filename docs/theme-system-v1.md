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
- `TeaRuntimeOptions.Theme`

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

- `Label`, `StatusBar`, `Button`, `TextInput`, `ListView<T>`, `Table`, `Tabs`, `Dialog`, `ContextMenu`, `CommandPalette`

Phase 2 (data and advanced controls):

- `TreeView`, `MenuBar`, `Notifications`, `Toggle`, `Slider`, `Spinner`, chart and log controls

Acceptance criteria:

- top controls expose style/theme override points with consistent naming
- focus, hover, selection, disabled, and error styles are configurable
- Catppuccin + Rosé Pine + custom palette can be applied without control-level rewiring
- docs and examples demonstrate global theme selection and per-control override
- `TeaSharp.Core.*` remains out of starter theming docs

## V1.1 Note

Image rendering is planned for V1.1 with capability-based backends and graceful fallback modes.
