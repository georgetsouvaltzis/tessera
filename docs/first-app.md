---
sidebar_label: Your First App
---

# Your First App

This page shows the smallest real Tessera app worth building. It uses the configured startup path because that is the public lane most teams will actually keep.

## Paste this into `Program.cs`

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;

var app = TesseraApplication.CreateBuilder()
    .UseApp<OrdersApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Orders",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();

await app.RunAsync();

internal sealed class OrdersApp : TesseraApp
{
    private readonly StatusBar _status = new();
    private readonly Button _refreshButton = new() { Text = "Refresh orders" };
    private int _orderCount = 12;

    public OrdersApp()
    {
        _refreshButton.Activated += (_, _) => _orderCount++;
    }

    public override TesseraEffect? Update(Message message)
    {
        return message is KeyPressed key && key.IsCharacter('q', ModifierKeys.Ctrl)
            ? TesseraEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Orders: {_orderCount}";
        _status.RightText = "Enter refreshes   Ctrl+Q quits";

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Footer(1, _status);
            window.Body(body => body.Center(_refreshButton, width: 24, height: 3));
        });
    }
}
```

## Run it

```bash
dotnet run
```

## What each part does

- `TesseraApplication.CreateBuilder()`
  - starts the configuration-first startup path
- `.UseApp<OrdersApp>()`
  - tells the builder which `TesseraApp` to create
- `.ConfigureRuntime(...)`
  - applies runtime-loop and screen options such as pointer policy and alternate screen behavior
- `OrdersApp : TesseraApp`
  - is the application contract you implement
- `Update(Message)`
  - handles global messages and returns effects such as `TesseraEffects.Quit`
- `Build(ScreenContext)`
  - returns the next screen tree
- `Screen.Build(...)`
  - uses the default imperative window builder for shell-style screens

## Minimal startup still exists

If you want the absolute smallest entry point, Tessera still supports:

```csharp
await TesseraApplication.RunAsync(new OrdersApp());
```

That is fine for tiny demos. The builder path is usually better once you need runtime configuration.

## What to do next

1. Run the starter ladder in [examples](/docs/examples).
2. Read [app-model](/docs/app-model) to understand `Initialize`, `Update`, `Build`, `Post`, and effects.
3. Read [layout-and-screen-composition](/docs/layout-and-screen-composition) to understand `Screen.Build(...)`.
