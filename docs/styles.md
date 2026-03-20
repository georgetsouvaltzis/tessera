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

## Terminal Font Request (Experimental)

For explicit, terminal-dependent font requests, use `ScreenOptions.FontSpec`.
This is an opt-in request lane (renderer emits OSC 50 when changed), not a guaranteed contract.

```csharp
var app = Tea.CreateBuilder()
    .UseApp<MyApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            FontSpec = "Iosevka Term 14",
        };
    })
    .Build();
```

Support matrix (TeaSharp V1 behavior):

- `FontSpec == null`: no font sequence emitted (no-op).
- `FontSpec != null`: renderer emits sanitized `OSC 50` set-font request (`BEL`, `ESC`, `\` removed).
- capability probe: none (best-effort only; terminal may ignore).
- reset behavior: no font restore sequence (TeaSharp avoids unsafe restore assumptions).

## Renderer Behavior

`AnsiDiffRenderer` now parses SGR (`CSI ... m`) in view content and keeps style metadata at cell level.  
Diff signatures include both glyph and style, so style-only changes produce minimal patches.

Covered by deterministic tests:

- style composition output
- styled frame rendering
- style-only diff patching
