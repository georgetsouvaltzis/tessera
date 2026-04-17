---
sidebar_label: Runtime & Screen Options
---

# Runtime And Screen Options

Tessera separates application-loop configuration from terminal-facing screen configuration.

That split is easy to miss in the current docs, so treat it as a core concept:

- `TesseraRuntimeOptions`
  - controls the runtime loop, input behavior, resize handling, and themes
- `ScreenOptions`
  - controls terminal-facing behavior such as alternate screen, paste, mouse tracking, and title

## Typical configured startup

```csharp
var app = TesseraApplication.CreateBuilder()
    .UseApp<MyApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.MaxFps = 60;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Theme = TesseraThemes.Catppuccin(CatppuccinVariant.Mocha);
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "MyApp",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();
```

## Use `TesseraRuntimeOptions` for

- `MaxFps`
- `AdaptiveFramePacing`
- `DisableRenderer`
- `DisableInput`
- `UseConsoleKeyEvents`
- `CatchEffectExceptions`
- `EscapeTimeout`
- `PointerActivationPolicy`
- `DoubleClickTimeout`
- `DoubleClickSlop`
- `EnableResizeSignals`
- `ResizePollInterval`
- `MinResizePollInterval`
- `Theme`
- `ThemeOverrides`
- `Screen`

## Use `ScreenOptions` for

- `AltScreen`
- `EnableBracketedPaste`
- `EnableFocusReporting`
- `EnableSynchronizedUpdates`
- `MouseTracking`
- `CursorColor`
- `ForegroundColor`
- `BackgroundColor`
- `WindowTitle`
- `FontSpec`
- `FontFamily`
- `FontSize`
- `Iterm2Profile`

## Defaults worth knowing

- `PointerActivationPolicy` defaults to `DoubleClick`
  - the first click still transfers focus
  - set `SingleClick` for pointer-friendly product apps
- `MaxFps` defaults to `60`
- `AdaptiveFramePacing` defaults to `true`
- resize monitoring is enabled by default

## Recommended starting profile

Most product-like apps should begin with:

- `PointerActivationPolicy.SingleClick`
- `AltScreen = true`
- `WindowTitle` set
- `EnableFocusReporting = true`
- `EnableBracketedPaste = true`
- `MouseTracking = MouseTrackingMode.AllMotion`

That is the profile used by the starter examples.

## Theme configuration lives here

Global theme selection belongs on `TesseraRuntimeOptions`:

```csharp
runtime.Theme = TesseraThemes.Catppuccin(CatppuccinVariant.Mocha);
runtime.ThemeOverrides = myOverrides;
```

Use [theme-system.md](theme-system.md) for the token and override model itself.

## Common mistakes

- setting pointer behavior on `ScreenOptions`
  - pointer activation policy belongs on `TesseraRuntimeOptions`
- expecting `ScreenOptions` to control app state or message flow
  - it only controls terminal/screen behavior
- skipping `AltScreen` and then thinking Tessera is “breaking” the terminal
  - decide explicitly whether you want alternate-screen behavior

## Next step

- theming and overrides: [theme-system.md](theme-system.md)
- terminal caveats: [terminal-font-capability-matrix.md](terminal-font-capability-matrix.md)
- problem symptoms and fixes: [troubleshooting.md](troubleshooting.md)
