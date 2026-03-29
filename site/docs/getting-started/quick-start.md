---
title: Quick Start
---

This is the shortest practical TeaSharp app: one app type, one button, one status bar.

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<CounterApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.MaxFps = 60;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Counter",
        };
    })
    .Build();

await app.RunAsync();

internal sealed class CounterApp : TeaApp
{
    private int _count;
    private readonly Button _increment = new() { Text = "Increment" };
    private readonly StatusBar _status = new();

    public CounterApp() => _increment.Activated += (_, _) => _count++;

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Count: {_count}";
        _status.RightText = "Ctrl+C quits";

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Footer(1, _status);
            window.Body(body => body.Center(_increment, width: 20, height: 3));
        });
    }
}
```

## What this shows

- `Tea.CreateBuilder()` for configured startup
- `TeaApp` as the default app model
- `Update(...)` for hotkeys and runtime messages
- `Screen.Build(...)` for layout
- `TeaSharp.Controls` for built-in UI

Next: read the app model and then apply theming.
