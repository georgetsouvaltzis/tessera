---
sidebar_label: Quickstart (Existing App)
---

# Quickstart: Existing App

Use this path when you already have a .NET console app and want to add Tessera.

## Goal

Integrate Tessera with minimal disruption, confirm runtime behavior, and then expand safely.

## 1. From your existing project folder

```bash
dotnet add package Tessera
```

## 2. Update `Program.cs`

Start from this minimal integration template:

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
    private readonly Button _refresh = new() { Text = "Refresh" };
    private int _count = 12;

    public OrdersApp()
    {
        _refresh.Activated += (_, _) => _count++;
    }

    public override TesseraEffect? Update(Message message)
    {
        return message is KeyPressed key && key.IsCharacter('q', ModifierKeys.Ctrl)
            ? TesseraEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Footer(1, new StatusBar
            {
                LeftText = $"Orders: {_count}",
                RightText = "Enter refreshes   Ctrl+Q quits"
            });
            window.Body(body => body.Center(_refresh, width: 18, height: 3));
        });
    }
}
```

## 3. Run and validate

```bash
dotnet run
```

Verify:

- the UI opens in the terminal
- interaction works (`Enter`, navigation keys, `Ctrl+Q`)
- app exits cleanly and restores terminal state

## 4. Common migration mistakes

- keeping old console loop code active alongside Tessera startup
- mixing business mutation directly inside rendering code
- jumping to `Tessera.Core` too early

Keep normal app integration inside:

- `Tessera`
- `Tessera.Controls`
- `Tessera.Layout`
- `Tessera.Styles` (when theming starts)

## What to do next

1. Read [Your First App](/docs/first-app) for lifecycle details.
2. Use [App Model](/docs/app-model) for message/effect patterns.
3. Use [Widget Pages](/docs/widgets) to pick controls quickly.
