---
sidebar_label: Beginner Track
---

# Beginner Track

Use this track if this is your first production TUI with Tessera.

## Goal

Ship one small but real app without learning every API first.

## What to install

```bash
dotnet add package Tessera
```

That is enough for the normal public path.

## Which project to start with

Start with a new `net10.0` console app, then move through this order:

1. [Installation](/docs/install-and-prerequisites)
2. [Quickstart (New App)](/docs/quickstart-new-app) or [Quickstart (Existing App)](/docs/quickstart-existing-app)
3. [Your First App](/docs/first-app)
4. [Starter Examples](/docs/examples)
5. [Flagship Evaluation](/docs/showcase)

## Learning order by problem

If your team is building:

- forms and data entry: read [Inputs & Forms](/docs/widgets-inputs-and-forms)
- navigation-heavy shells: read [Navigation & Workflow](/docs/widgets-navigation-and-workflow)
- record-heavy investigations: read [Data & Inspection](/docs/widgets-data-and-inspection)
- dashboard/monitoring screens: read [Dashboards & Plots](/docs/widgets-dashboards-and-plots)

If you want one page per control with usage + properties + events, use [Widget Pages](/docs/widgets).

## Minimal app shape to remember

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
        Screen.Build(window => window.Body(body => body.Center(new Button { Text = "Open" }, 16, 3)));
}
```

## What beginners usually miss

- keep app state in your `TesseraApp`
- use messages/effects for state transitions, not hidden side effects
- keep runtime configuration in builder setup
- do not start with `Tessera.Core` unless you need advanced seams

## Next step

Once this track feels stable, continue with [Advanced Track](/docs/advanced-track).
