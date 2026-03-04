# TeaSharp ANSI Styles

TeaSharp includes a lightweight ANSI styling API in `TeaSharp.Styles` for composing and rendering terminal text styles.

## API

- `AnsiColor`
  - `Indexed(0..255)`
  - `Rgb(r, g, b)`
  - built-in common colors (`BrightGreen`, `BrightCyan`, etc.)
- `TeaStyle`
  - chainable setters: `WithBold`, `WithUnderline`, `WithForeground`, `WithBackground`, `WithItalic`, `WithDim`, `WithInverse`
  - `Merge(TeaStyle other)` for composition
  - `ToEscapeSequence()` for SGR prefix
  - `Render(string text)` for `prefix + text + reset`

## Renderer Behavior

`AnsiDiffRenderer` now parses SGR (`CSI ... m`) in view content and keeps style metadata at cell level.  
Diff signatures include both glyph and style, so style-only changes produce minimal patches.

Covered by deterministic tests:

- style composition output
- styled frame rendering
- style-only diff patching
