# Visual State & Style Audit (V1)

## Scope
- Target: background colors, hover/selected/focused overrides, theme token wiring.
- Focused areas: `TeaTheme`, `TeaThemeOverrides`, `TeaThemeControlExtensions.*`, dropdown-like controls (`Choice`, `ComboBox`), and representative nav/data/overlay/input controls.
- This is analysis-only. No product code changes.
- Synced against current repository HEAD after lane commits `79d4658`, `03e17e0`, `842aaaf`.

## Final Sync Findings

### 1) Theme hierarchy exists and is deterministic
- Override precedence is explicit: global -> global-state -> control-type -> control-type-state -> control-instance -> control-instance-state (`src/TeaSharp/Styles/TeaThemeOverrides.cs:7`, `src/TeaSharp/Styles/TeaThemeOverrides.cs:99`).
- Visual-state keys exist (`Default`, `Focused`, `Hovered`, `Selected`, `Disabled`, `Error`, `Active`) (`src/TeaSharp/Styles/TeaThemeVisualState.cs:6`).

### 2) Background color capability exists, but token usage is uneven
- `TeaStyle` supports foreground and background ANSI color (`src/TeaSharp/Styles/TeaStyle.cs:27`, `src/TeaSharp/Styles/TeaStyle.cs:43`).
- Built-in palettes populate background tokens (`Surface.*`, `Selection.Background`) (`src/TeaSharp/Styles/TeaThemes.cs:162`, `src/TeaSharp/Styles/TeaThemes.cs:187`).
- Surface tokens are barely wired into controls (notably `StatusBar.FillStyle`) (`src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:143`).

### 3) Runtime now applies override hierarchy during scene compilation
- `ScreenContext` now carries `ThemeOverrides` (`src/TeaSharp/ScreenContext.cs:29`) and runtime options expose it (`src/TeaSharp/TeaRuntimeOptions.cs:78`).
- Runtime wiring propagates overrides through app context (`src/TeaSharp/TeaApp.cs:89`, `src/TeaSharp/TeaApp.cs:171`).
- Scene compiler now resolves per-control visual state and applies `ApplyThemeDefaults(overrides, baseTheme, state)` (`src/TeaSharp/Internal/TeaSceneCompiler.cs:37`, `src/TeaSharp/Internal/TeaSceneCompiler.cs:130`, `src/TeaSharp/Internal/TeaSceneCompiler.cs:252`, `src/TeaSharp/Internal/TeaSceneCompiler.cs:364`).
- Runtime coverage exists via dedicated tests (`tests/TeaSharp.Tests/ThemeOverridesRuntimeWiringTests.cs:12`, `:35`, `:83`).

### 4) Typography support is split between portable SGR emphasis and terminal-specific font requests
- Portable emphasis intent is exposed through `TeaStyle.WithFontWeight(TeaFontWeight)` and maps to ANSI SGR bold/dim behavior only (`src/TeaSharp/Styles/TeaStyle.cs:39`, `src/TeaSharp/Styles/TeaFontWeight.cs:9`).
- `ScreenOptions.FontSpec` provides an experimental terminal font request path (`src/TeaSharp/ScreenOptions.cs:64`, `src/TeaSharp/ScreenOptions.cs:70`).
- The renderer emits OSC 50 when `FontSpec` changes (`src/TeaSharp.Core/Rendering/AnsiDiffRenderer.cs:171`, `:176`) and sanitizes control characters before output (`:431`).
- Caveat for V1 docs: custom family/size remains best-effort via OSC 50 and is not guaranteed across terminals (`src/TeaSharp/ScreenOptions.cs:67`, `:68`).

## Support Matrix (Final)

| Control | Hover hook | Selected hook | Focused hook | Background-ready hook | Theme token wiring |
|---|---|---|---|---|---|
| Choice | Yes (`HoveredOptionStyle`) | Yes (`SelectedOptionStyle`) | Yes (title/border) | Yes (style supports BG) | Yes (`NavigationOverlay`) |
| ComboBox | Yes (`HoveredOptionStyle`) | Yes (`SelectedOptionStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| ContextMenu | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| CommandPalette | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| MenuBar | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (`FocusedItemStyle` + border) | Yes | Yes (`NavigationOverlay`) |
| ListView | Yes (`HoveredRowStyle`) | Yes (`SelectedRowStyle`) | Yes (title/border) | Yes | Yes (`Basic`) |
| DataGrid | Yes (`HoveredRowStyle`, `HoveredCellStyle`) | Yes (`SelectedRowStyle`, `SelectedCellStyle`) | Yes (title/border) | Yes | Partial (hover hooks not theme-mapped) |
| TreeTable | Yes (`HoveredRowStyle`) | Yes (`SelectedRowStyle`) | Yes (title/border) | Yes | Yes (`DataAndFlow`) |
| TreeView | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Yes (`NavigationOverlay`) |
| FileExplorer | Yes (`HoveredStyle`) | Yes (`SelectedStyle`) | Yes (title/border) | Yes | Yes (`ExplorerAndFeedback`) |
| FuzzyFinder | Yes (`HoveredItemStyle`) | Yes (`SelectedItemStyle`) | Yes (title/border) | Yes | Yes (`ExplorerAndFeedback`) |
| DatePicker | Yes (`HoveredDayStyle`) | Yes (`SelectedDayStyle`) | Yes (title/border) | Yes | Yes (`InputValue`) |
| TimePicker | Yes (`HoveredFieldStyle`) | Yes (`ActiveFieldStyle`/selected semantics) | Yes (title/border) | Yes | Yes (`InputValue`) |
| Table | Yes (`HoveredRowStyle`) | Yes (`SelectedRowStyle`) | Yes (title/border) | Yes | Yes (`Basic`) |

## Concrete Gaps (with references)

1. **DataGrid hover hooks are not mapped from theme tokens**
- Runtime/render now supports hovered row/cell styles (`src/TeaSharp/Controls/DataGrid.cs:85`, `src/TeaSharp/Controls/DataGrid.cs:90`, `src/TeaSharp/Controls/DataGrid.Rendering.cs:268`), but `ApplyTheme`/`ApplyThemeDefaults` do not set hover tokens (`src/TeaSharp/Styles/TeaThemeControlExtensions.DataAndFlow.cs:15`, `src/TeaSharp/Styles/TeaThemeControlExtensions.DataAndFlow.cs:49`).

2. **Surface/background theme tokens remain underutilized**
- `Surface.Base/Panel/Overlay` are defined, but broad control mappings still rarely consume them (`src/TeaSharp/Styles/TeaThemes.cs:162`, `src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:143`).

3. **Border style hooks are mode-sensitive**
- Styled border glyphs are ignored in `CanvasTextMode.Fast`; only active in `GraphemeAware` (`src/TeaSharp/Components/Canvas/Canvas.cs:369`, `src/TeaSharp/Components/Canvas/Canvas.cs:379`).

4. **Theme-override state coverage in tests is still incomplete**
- Runtime wiring + focused-state behavior are now covered (`tests/TeaSharp.Tests/ThemeOverridesRuntimeWiringTests.cs:35`, `:83`), and hover parity tests exist for nav/explorer controls (`tests/TeaSharp.Tests/ThemeStateParity_NavigationExplorerTests.cs:21`), but broad `Selected/Disabled/Error/Active` override layering assertions are still limited.

5. **Terminal typography requests are non-portable by design**
- `FontSpec` is explicitly documented as terminal-dependent and optional (`src/TeaSharp/ScreenOptions.cs:67`, `:68`), so style/theme guidance must treat custom family/size as an opt-in best-effort path, not a guaranteed visual contract.

## Final Sync Summary

- Full visual-state/style pass (`hover` + `selected` + `focused` hooks and token wiring): `13/14` controls (`Choice`, `ComboBox`, `ContextMenu`, `CommandPalette`, `MenuBar`, `ListView`, `TreeTable`, `TreeView`, `FileExplorer`, `FuzzyFinder`, `DatePicker`, `TimePicker`, `Table`).
- Partial pass: `1/14` (`DataGrid`, pending hover token mapping in theme extensions).
- Runtime override integration is now landed; remaining gaps are token adoption + residual parity/test breadth.

## Suggested Priority Order
1. Add DataGrid hover token mapping in `TeaThemeControlExtensions.DataAndFlow`.
2. Expand surface/background token adoption policy (panel/fill semantics per control family).
3. Expand theme-override state tests for `Selected`, `Disabled`, `Error`, and `Active` layering.
4. Decide whether fast-mode border-style behavior is intentional long-term or should have opt-in parity.
