# TeaSharp App Pattern

Use this pattern for normal apps:

- derive from `TeaApp`
- handle terminal input through `Message`
- let controls route automatically; `Update(...)` handles unhandled input plus runtime messages
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

    public CounterApp() => _increment.Activated += (_, _) => _count++;

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Count: {_count}";
        _status.RightText = $"Size {context.Width}x{context.Height}";

        return Screen.From(new WindowLayout
        {
            Footer = new LayoutSlot
            {
                Content = _status,
                Length = 1,
            },
            Body = new CenterLayout
            {
                Content = _increment,
                Width = 20,
                Height = 3,
            },
            Padding = Thickness.All(1),
        });
    }
}
```

## Application Model

`TeaApp` is the default app contract.

- `Initialize()` is optional startup work
- `Update(Message)` handles unhandled input, terminal events, and custom messages
- `Build(ScreenContext)` creates the current frame
- `DefaultScreenOptions` defines per-app terminal defaults
- built-in controls route keyboard, pointer, and paste input before `Update(Message)`
- when a control event should re-enter the app state machine, call `Post(...)`
- `Post(...)` does not call `Update(...)` immediately; it queues a follow-up message for the next runtime pass
- `RequestEffect(...)` is available when a control event needs to trigger runtime work such as `TeaEffects.Quit`

`ScreenContext` already tracks terminal size and focus state, so normal apps do not need to manage `_width`, `_height`, or focus-reporting flags by hand.

If a control should claim focus programmatically, call `RequestFocus()`. The request is one-shot: it applies to the next composition pass only. When multiple controls request focus during the same build pass, the most recent request wins.

## Composition Model

The default composition path uses explicit layout objects, not a nested static DSL.

Common default layout types:

- `WindowLayout`
- `RowLayout`
- `ColumnLayout`
- `PanelLayout`
- `CenterLayout`
- `LayoutSlot`

Common default controls:

- `Label`
- `Button`
- `TextInput`
- `TextArea`
- `Choice`
- `ComboBox`
- `Dialog`
- `ProgressBar`
- `LogView`
- `Notifications`
- `Toggle`
- `Slider`
- `Spinner`
- `StatusBar`
- `Tabs`
- `ListView<T>`
- `Table`
- `TreeItem`
- `TreeView`
- `MenuBar`
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

`TeaSharp.Hosting.TeaHostingOptions` owns the advanced hosting seams such as renderer, terminal adapter, event decoder, capability probes, and runtime interception hooks.

`ScreenOptions` owns terminal-facing frame options such as:

- `AltScreen`
- `WindowTitle`
- `EnableBracketedPaste`
- `EnableFocusReporting`
- `MouseTracking`

Advanced runtime seams remain available, but they now sit under `TeaSharp.Hosting` and are marked `EditorBrowsable(Advanced)`.

## Advanced Escape Hatch

Most of the older composition stack is now internal-only.

Advanced interop still exists through hosting/runtime seams and advanced component/layout overloads, but explicit region-key composition is no longer part of the public path.
