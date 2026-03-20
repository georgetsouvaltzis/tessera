# TeaSharp ANSI Styles

TeaSharp includes a lightweight ANSI styling API in `TeaSharp.Styles` for composing and rendering terminal text styles.

## API

- `AnsiColor`
  - `Indexed(0..255)`
  - `Rgb(r, g, b)`
  - built-in common colors (`BrightGreen`, `BrightCyan`, etc.)
- `TeaStyle`
  - chainable setters: `WithBold`, `WithUnderline`, `WithForeground`, `WithBackground`, `WithItalic`, `WithDim`, `WithInverse`
  - typography intent helper: `WithFontWeight(TeaFontWeight.Normal|Bold|Dim)`
  - `Merge(TeaStyle other)` for composition
  - `ToEscapeSequence()` for SGR prefix
  - `Render(string text)` for `prefix + text + reset`
- `TeaFontWeight`
  - `Normal` -> disables bold/dim SGR emphasis
  - `Bold` -> sets bold SGR emphasis
  - `Dim` -> sets dim SGR emphasis

## Typography Intent

`TeaFontWeight` and `TeaStyle.WithFontWeight(...)` are convenience APIs for **ANSI SGR emphasis intent only**.
They do not control actual terminal font family, font size, ligatures, or host font engines.

## Renderer Behavior

`AnsiDiffRenderer` now parses SGR (`CSI ... m`) in view content and keeps style metadata at cell level.  
Diff signatures include both glyph and style, so style-only changes produce minimal patches.

Covered by deterministic tests:

- style composition output
- styled frame rendering
- style-only diff patching
