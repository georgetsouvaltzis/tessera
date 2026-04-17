---
sidebar_label: FAQ
---

# FAQ

## Do I need dependency injection or Generic Host?

No. The normal public path is library-first.

Use:

- `await TesseraApplication.RunAsync(new MyApp())`
- or `TesseraApplication.CreateBuilder().UseApp<TApp>()...`

## Should I use `RunAsync(...)` or `CreateBuilder()`?

Use:

- `RunAsync(...)`
  - for the smallest possible startup
- `CreateBuilder()`
  - when you need runtime configuration, theme selection, screen options, or a built application instance

Most real apps will prefer the builder.

## Do I need `Tessera.Core` to build a normal app?

No. Keep normal apps in:

- `Tessera`
- `Tessera.Controls`
- `Tessera.Layout`
- `Tessera.Styles`

## What is the fastest way to learn Tessera?

1. [install-and-prerequisites.md](install-and-prerequisites.md)
2. [first-app.md](first-app.md)
3. [examples.md](examples.md)
4. [showcase.md](showcase.md)

## How do I handle app state?

Use `TesseraApp`:

- `Initialize()` for first effect
- `Update(Message)` for state transitions and effects
- `Build(ScreenContext)` for rendering

For small demos, direct event mutation is fine. For real apps, prefer control events that `Post(...)` domain messages back into `Update(...)`.

## How do I theme the app?

Set theme at runtime:

```csharp
runtime.Theme = TesseraThemes.Catppuccin(CatppuccinVariant.Mocha);
runtime.ThemeOverrides = myOverrides;
```

Then use [theme-system.md](theme-system.md) for the token and override model.

## What terminals are recommended?

Start with:

- Ghostty
- iTerm2
- Windows Terminal
- macOS Terminal

For caveats and capability details, use [terminal-font-capability-matrix.md](terminal-font-capability-matrix.md).

## Can I build custom controls?

Yes. Use `Tessera.Controls.Control` for the normal custom widget path. That is the supported custom control contract for interactive widgets.

See [custom-components.md](custom-components.md).

## Where do I find the full control list?

Use:

- [controls-overview.md](controls-overview.md) for grouped discovery
- [public-api-inventory.md](public-api-inventory.md) for the exact public list

## Are images part of V1?

No. The current design contract treats inline image rendering as V1.1 scope, not V1 scope.

See [spec.md](spec.md) for the product boundary.

## What examples should I run first?

Starter ladder:

1. `HelloWorld`
2. `CounterForm`
3. `WorkspaceApp`

Then flagship evaluation:

1. `GitConsole`
2. `OpsWatch`
3. `DataWorkbench`

Use [examples.md](examples.md) and [showcase.md](showcase.md).
