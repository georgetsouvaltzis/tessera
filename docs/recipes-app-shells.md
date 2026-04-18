---
sidebar_label: "Recipes: App Shells"
---

# Recipes: App Shells

Use these patterns when you want a minimal shell that still looks like real app code.

## Recipe: builder startup with safe defaults

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
    private readonly Button _refresh = new() { Text = "Refresh orders" };
    private readonly StatusBar _status = new();
    private int _count = 12;

    public OrdersApp()
    {
        _refresh.Activated += (_, _) => _count++;
    }

    public override TesseraEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('q', ModifierKeys.Ctrl)
            ? TesseraEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Orders: {_count}";
        _status.RightText = "Enter refreshes   Ctrl+Q quits";

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(body => body.Center(_refresh, width: 24, height: 3));
            window.Footer(1, _status);
        });
    }
}
```

Why this works:

- runtime defaults stay in one place
- the app contract stays small
- shell chrome is obvious
- `Ctrl+Q` is app-wide, so it belongs in `Update(...)`

## Recipe: smallest possible startup

When you do not need runtime configuration yet:

```csharp
await TesseraApplication.RunAsync(new OrdersApp());
```

Use this only for tiny demos or first experiments. Move to `CreateBuilder()` once theme/runtime options start to matter.

## Recipe: split shell regions before the app gets dense

When the screen starts growing, keep one screen-level `Build(...)` and split regions:

```csharp
public override Screen Build(ScreenContext context)
{
    return Screen.Build(window =>
    {
        window.Padding(1);
        window.Header(3, BuildHeader());
        window.Left(24, BuildRail());
        window.Body(body => body.Use(BuildBody()));
        window.Footer(1, _status);
    });
}
```

That keeps the top-level screen readable without inventing a second framework layer.

## Related pages

- lifecycle and messages: [app-model.md](app-model.md)
- composition details: [layout-and-screen-composition.md](layout-and-screen-composition.md)
- runtime options: [runtime-and-screen-options.md](runtime-and-screen-options.md)
