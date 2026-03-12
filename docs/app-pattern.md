# TeaSharp App Pattern

Use this pattern for normal apps:

- derive from `TeaApp`
- handle terminal input through `Message`
- route controls through `HandleScreenInput(...)`
- return a `Screen` from `Build(ScreenContext)`
- run with `Tea.RunAsync(...)` or `TeaApplicationBuilder`
- keep low-level composition APIs as advanced-only escape hatches

## Default Shape

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
            EnableFocusReporting = true,
        };
    })
    .Build();

await app.RunAsync();

internal sealed class CounterApp : TeaApp
{
    private int _count;
    private readonly Button _increment = new() { Text = "Increment" };
    private readonly StatusBar _status = new();

    public override TeaEffect? Update(Message message)
    {
        if (HandleScreenInput(message))
        {
            if (_increment.TryConsumeActivation())
            {
                _count++;
            }

            return null;
        }

        return message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Count: {_count}";
        _status.RightText = $"Size {context.Width}x{context.Height}";

        return Screen.From(
            new DockLayout(
                bottom: new LayoutSlot(_status, LayoutLength.Fixed(1)),
                fill: new LayoutSlot(
                    new CenterLayout(_increment, width: 20, height: 3),
                    LayoutLength.Fill()),
                padding: Thickness.All(1)));
    }
}
```

## Application Model

`TeaApp` is the default app contract.

- `Initialize()` is optional startup work
- `Update(Message)` handles input, terminal events, and custom messages
- `Build(ScreenContext)` creates the current frame
- `DefaultScreenOptions` defines per-app terminal defaults
- `HandleScreenInput(Message)` routes keyboard and pointer input into the most recently built screen tree

`ScreenContext` already tracks terminal size and focus state, so normal apps do not need to manage `_width`, `_height`, or focus-reporting flags by hand.

## Composition Model

The default composition path uses explicit layout objects, not a nested static DSL.

Common layout types:

- `StackLayout`
- `SplitLayout`
- `DockLayout`
- `PanelLayout`
- `CenterLayout`
- `OverlayLayout`
- `LayoutSlot`

Common default controls:

- `Label`
- `Button`
- `TextInput`
- `TextArea`
- `Choice`
- `Dialog`
- `StatusBar`

These types compile down into the existing internal composition/runtime engine, but normal apps do not need to use `ScreenComposer` directly.

## Startup

Use `Tea.RunAsync(app, options)` for small apps.

```csharp
await Tea.RunAsync(
    new HelloApp(),
    new TeaRuntimeOptions
    {
        MaxFps = 30,
        Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Hello",
        },
    });
```

Use `TeaApplicationBuilder` when you want a reusable composition root.

- choose the app with `UseApp(...)`
- configure the runtime with `ConfigureRuntime(...)`
- call `Build()`
- run the resulting `TeaApplication`

## Messages And Effects

The default path is strongly typed.

- `KeyPressed`
- `KeyReleased`
- `WindowResized`
- `PointerInput`
- `FocusChanged`
- `Pasted`
- `Faulted`
- `ExternalMessage`

Use `TeaEffects` to return runtime actions such as:

- `TeaEffects.Quit`
- `TeaEffects.Interrupt`
- `TeaEffects.Emit(...)`
- `TeaEffects.Batch(...)`
- `TeaEffects.Sequence(...)`

## Runtime Options

`TeaRuntimeOptions` is the default runtime configuration object.

Common settings:

- `MaxFps`
- `AdaptiveFramePacing`
- `DisableRenderer`
- `DisableInput`
- `EnableResizeSignals`
- `Screen`

`ScreenOptions` owns terminal-facing frame options such as:

- `AltScreen`
- `WindowTitle`
- `EnableBracketedPaste`
- `EnableFocusReporting`
- `MouseTracking`

Advanced runtime seams like renderer, terminal adapter, event decoder, and capability probes remain available but are marked `EditorBrowsable(Advanced)`.

## Advanced Escape Hatch

The older composition stack remains available for now:

- `ScreenComposer`
- `InteractiveScreenModel`
- `InputRouter`
- `ScreenRegionKey`
- layout helper DSL types such as `Stack`, `Split`, `Panel`, `Dock`, `Overlay`, `Center`, and `Slot`
- shell helpers such as `MasterDetail`, `Dashboard`, and `Form`

Those APIs are now explicitly marked advanced. They are still usable for transitional or highly customized screens, but they are no longer the recommended starting point.
