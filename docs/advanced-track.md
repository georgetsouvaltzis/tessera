---
sidebar_label: Advanced Track
---

# Advanced Track

Use this track once the starter path is clear and your app needs denser runtime control.

## Goal

Adopt Tessera for a long-lived product shell: tuned runtime, robust theming, custom components, and predictable performance.

## Recommended order

1. [App Model](/docs/app-model)
2. [Screen & Layout](/docs/layout-and-screen-composition)
3. [Runtime & Screen Options](/docs/runtime-and-screen-options)
4. [Theme System](/docs/theme-system)
5. [Custom Components](/docs/custom-components)
6. [Architectural Review](/docs/architectural-review)
7. [Performance](/docs/performance)

## Builder configuration example

```csharp
using Tessera;
using Tessera.Styles;

var app = TesseraApplication.CreateBuilder()
    .UseApp<OpsApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Ops",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .ConfigureTheme(static theme =>
    {
        theme.SetControlDefaults<Button>(style =>
            style.BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.BrightBlack));
    })
    .Build();

await app.RunAsync();
```

## Advanced adoption checklist

- runtime options explicit in builder
- keyboard and pointer behavior tested in your target terminal set
- theme overrides applied by token/hook hierarchy
- widget selection based on product tasks, not API novelty
- one flagship example chosen as a reference architecture

## Use `Tessera.Core` only when needed

Stay on the public `Tessera` lane for most apps. Move into lower-level seams only when the default runtime contract is not enough for your product constraints.

## Next step

When implementing feature surfaces, pick from [Widget Reference](/docs/widget-reference) first, then use [Recipes](/docs/recipes) for practical assembly patterns.

