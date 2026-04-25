---
sidebar_label: Introduction
title: Introduction
---

Tessera is a C#-first terminal UI framework for teams building real product shells, not throwaway demos.

It keeps one public app-authoring path from first screen to dense multi-pane software:

- install one package
- derive from `TesseraApp`
- build screens with `Screen.Build(...)`
- compose with `Tessera.Controls` and `Tessera.Layout`
- scale into dashboards, workflows, and inspectors without switching framework model

## Install in one line

```bash
dotnet add package Tessera
```

Then continue with [Installation](/docs/install-and-prerequisites) and [Your First App](/docs/first-app).

## Why teams choose Tessera

- **Product-first app model**: explicit `Initialize`, `Update(Message)`, and `Build(ScreenContext)` lifecycle
- **TEA-inspired flow**: state + message update + view rebuild in a .NET-native terminal runtime
- **Single mental model**: starter apps and flagship showcases use the same public API lane
- **Widget depth**: forms, navigation, grids, inspectors, overlays, dashboards, and plot surfaces
- **Theme-first visuals**: semantic tokens, control defaults, and instance/state overrides
- **Runtime control when needed**: screen options and runtime policy without forcing host-heavy setup

## At a glance sample

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;

var app = TesseraApplication.CreateBuilder()
    .UseApp<OrdersApp>()
    .Build();

await app.RunAsync();

internal sealed class OrdersApp : TesseraApp
{
    public override TesseraEffect? Update(Message message) => null;

    public override Screen Build(ScreenContext context) =>
        Screen.Build(window => window.Body(body => body.Center(new Button { Text = "Open orders" }, 24, 3)));
}
```

## Recommended reading order

1. [Installation](/docs/install-and-prerequisites)
2. [Your First App](/docs/first-app)
3. [Starter Examples](/docs/examples)
4. [Flagship Evaluation](/docs/showcase)
5. [Widget Reference](/docs/widget-reference)
6. [Architectural Review](/docs/architectural-review)
