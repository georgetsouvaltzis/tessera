# Visual State & Style Audit (V1)

## Scope
- Target: background colors, hover/selected/focused overrides, theme token wiring.
- Focused areas: `TeaTheme`, `TeaThemeOverrides`, `TeaThemeControlExtensions.*`, dropdown-like controls (`Choice`, `ComboBox`), and representative nav/data/overlay/input controls.
- This is analysis-only. No product code changes.

## Baseline Findings

### 1) Theme hierarchy exists and is deterministic
- Override precedence is explicit: global -> global-state -> control-type -> control-type-state -> control-instance -> control-instance-state (`src/TeaSharp/Styles/TeaThemeOverrides.cs:7`, `src/TeaSharp/Styles/TeaThemeOverrides.cs:99`).
- Visual-state keys exist (`Default`, `Focused`, `Hovered`, `Selected`, `Disabled`, `Error`, `Active`) (`src/TeaSharp/Styles/TeaThemeVisualState.cs:6`).

### 2) Background color capability exists, but token usage is uneven
- `TeaStyle` supports foreground and background ANSI color (`src/TeaSharp/Styles/TeaStyle.cs:27`, `src/TeaSharp/Styles/TeaStyle.cs:43`).
- Built-in palettes populate background tokens (`Surface.*`, `Selection.Background`) (`src/TeaSharp/Styles/TeaThemes.cs:162`, `src/TeaSharp/Styles/TeaThemes.cs:187`).
- Surface tokens are barely wired into controls (notably `StatusBar.FillStyle`) (`src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:143`).

### 3) Runtime applies base theme defaults, not override hierarchy
- Scene compiler applies only `context.Theme` defaults (`src/TeaSharp/Internal/TeaSceneCompiler.cs:33`).
- `ScreenContext` carries `Theme` but no `ThemeOverrides` (`src/TeaSharp/ScreenContext.cs:24`).
- Net: `TeaThemeOverrides` is available API but not automatically integrated into render pipeline.

## Support Matrix (Current)

| Control | Hover hook | Selected hook | Focused hook | Background-ready hook | Theme token wiring |
|---|---|---|---|---|---|
| Choice | Yes (`HoveredOptionStyle`) | Yes (`SelectedOptionStyle`) | Yes (title/border) | Yes (style supports BG) | Yes (`NavigationOverlay`) |
| ComboBox | Yes (`HoveredOptionStyle`) | Yes (`SelectedOptionStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| ContextMenu | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| CommandPalette | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| MenuBar | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (`FocusedItemStyle` + border) | Yes | Yes (`NavigationOverlay`) |
| ListView | Yes (`HoveredRowStyle`) | Yes (`SelectedRowStyle`) | Yes (title/border) | Yes | Yes (`Basic`) |
| DataGrid | No explicit hover style | Yes (`SelectedRowStyle`, `SelectedCellStyle`) | Yes (title/border) | Yes | Yes (`DataAndFlow`) |
| TreeTable | No explicit hover style | Yes (`SelectedRowStyle`) | Yes (title/border) | Yes | Yes (`DataAndFlow`) |
| FileExplorer | No explicit hover style | Yes (`SelectedStyle`) | Yes (title/border) | Yes | Yes (`ExplorerAndFeedback`) |
| FuzzyFinder | No hover style hook | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Partial (no hover token map) |
| DatePicker | Yes (`HoveredDayStyle`) | Yes (`SelectedDayStyle`) | Yes (title/border) | Yes | Yes (`InputValue`) |
| TimePicker | Yes (`HoveredFieldStyle`) | Yes (`ActiveFieldStyle`/selected semantics) | Yes (title/border) | Yes | Yes (`InputValue`) |
| Table | Internal hover/selection only | Internal marker only | Yes (title/border) | Limited row styling | Minimal (`Basic` title/border only) |

## Concrete Gaps (with references)

1. **Overrides not auto-applied at runtime**
- `TeaThemeOverrides` resolution exists, but compiler path does not consume it (`src/TeaSharp/Styles/TeaThemeOverrides.cs:99`, `src/TeaSharp/Internal/TeaSceneCompiler.cs:33`, `src/TeaSharp/ScreenContext.cs:24`).

2. **Surface/background theme tokens underutilized**
- `Surface.Base/Panel/Overlay` are defined, but control mappings rarely consume them (`src/TeaSharp/Styles/TeaThemes.cs:162`, `src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:143`).

3. **Border style hooks are mode-sensitive**
- Styled border glyphs are ignored in `CanvasTextMode.Fast`; only active in `GraphemeAware` (`src/TeaSharp/Components/Canvas/Canvas.cs:369`, `src/TeaSharp/Components/Canvas/Canvas.cs:379`).

4. **Dropdown field-hover state tracked but not rendered**
- `Choice` and `ComboBox` track `_fieldHovered`, but field style resolver ignores it (`src/TeaSharp/Controls/Choice.cs:15`, `src/TeaSharp/Controls/Choice.cs:453`, `src/TeaSharp/Controls/ComboBox.cs:20`, `src/TeaSharp/Controls/ComboBox.cs:471`).

5. **Table row visual state not style-hook driven**
- Table keeps hovered/selected row indices, but rendering delegates to `Widgets.DrawTable` with hardcoded selected marker behavior and no row style hooks (`src/TeaSharp/Controls/Table.cs:15`, `src/TeaSharp/Controls/Table.cs:273`, `src/TeaSharp/Components/Canvas/Widgets.cs:205`, `src/TeaSharp/Components/Canvas/Widgets.cs:235`).

6. **FuzzyFinder has no hover visual override path**
- Render only differentiates selected vs non-selected rows; no hovered row state/style hook (`src/TeaSharp/Controls/FuzzyFinder.Rendering.cs:68`).
- Theme mapping for FuzzyFinder similarly has no hover token target (`src/TeaSharp/Styles/TeaThemeControlExtensions.ExplorerAndFeedback.cs:195`).

7. **Theme-override state coverage in tests is narrow**
- Existing override tests heavily exercise focused state; hovered/selected/disabled/error/active state layering is not broadly asserted (`tests/TeaSharp.Tests/ThemeOverridesTests.Foundation.cs:39`, `tests/TeaSharp.Tests/ThemeOverridesTests.InputValueWidgets.cs:235`).

## Suggested Priority Order
1. Runtime integration of `TeaThemeOverrides` (without breaking explicit control-level styles).
2. Surface/background token adoption policy (panel/fill semantics per control family).
3. Visual-state parity: add hover hooks where state already exists (DataGrid, TreeTable, FileExplorer, FuzzyFinder, Table).
4. Expand theme-override state tests to `Hovered`, `Selected`, `Disabled`, `Error`, `Active`.
