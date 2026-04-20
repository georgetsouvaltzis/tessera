---
sidebar_label: Quickstart (New App)
---

# Quickstart: New App

Use this path when you are starting from an empty folder.

## Goal

In 10 minutes, run a minimal Tessera app and verify your terminal/runtime are healthy.

## 1. Create the project

```bash
dotnet new console -n MyTesseraApp -f net10.0
cd MyTesseraApp
```

## 2. Install Tessera

```bash
dotnet add package Tessera
```

## 3. Replace `Program.cs`

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;

var app = TesseraApplication.CreateBuilder()
    .UseApp<HelloApp>()
    .Build();

await app.RunAsync();

internal sealed class HelloApp : TesseraApp
{
    private readonly Button _button = new() { Text = "Press Enter" };
    private int _pressCount;

    public HelloApp()
    {
        _button.Activated += (_, _) => _pressCount++;
    }

    public override TesseraEffect? Update(Message message)
    {
        return message is KeyPressed key && key.IsCharacter('q', ModifierKeys.Ctrl)
            ? TesseraEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        var status = new StatusBar
        {
            LeftText = $"Button presses: {_pressCount}",
            RightText = "Enter activates   Ctrl+Q quits"
        };

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Footer(1, status);
            window.Body(body => body.Center(_button, width: 20, height: 3));
        });
    }
}
```

## 4. Run

```bash
dotnet run
```

## 5. Success checklist

- app opens in your terminal without exceptions
- pressing `Enter` on the button increments the counter
- pressing `Ctrl+Q` exits cleanly

If one of these fails, go to [Troubleshooting](/docs/troubleshooting).

## What to do next

1. Read [Your First App](/docs/first-app) for detailed explanation.
2. Run [Starter Examples](/docs/examples) in order.
3. Use [Widget Pages](/docs/widgets) when you need control-by-control usage.
