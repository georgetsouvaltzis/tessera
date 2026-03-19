# TeaSharp

TeaSharp is a message-driven terminal UI library for .NET.

The default app path is intentionally small:

- derive from `TeaApp`
- run apps with `Tea.RunAsync(...)` for minimal startup, or `Tea.CreateBuilder()` for configured startup
- choose the app with `UseApp(...)`, then configure runtime with `ConfigureRuntime(...)`
- let built-in controls route automatically; `Update(...)` handles unhandled input plus runtime messages
- return `Screen` from `Build(ScreenContext)`
- assemble screens with `Screen.Build(...)` and shallow builder callbacks
- use first-class controls from `TeaSharp.Controls`
- configure runtime behavior with `TeaRuntimeOptions`
- keep low-level runtime wiring under `TeaSharp.Hosting` only when you truly need advanced seams

If you need custom runtime wiring, explicit region routing, or low-level component composition, those APIs still exist under `TeaSharp.Hosting` or advanced namespaces, but they are marked `EditorBrowsable(Advanced)` and are no longer the starter path.

## Quick Start

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
    private readonly CounterState _state = new();
    private readonly Button _increment = new()
    {
        Text = "Increment",
    };
    private readonly StatusBar _status = new();
    public CounterApp() => _increment.Activated += (_, _) => _state.Count++;

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Count: {_state.Count}";
        _status.RightText = "Enter increments   Ctrl+C quits";

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Footer(1, _status);
            window.Body(body => body.Center(_increment, width: 20, height: 3));
        });
    }
}

internal sealed class CounterState
{
    public int Count { get; set; }
}
```

For tiny apps, the short path also exists:

```csharp
await Tea.RunAsync(new HelloApp());

internal sealed class HelloApp : TeaApp
{
    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
        => Screen.From("Hello from TeaSharp");
}
```

## Example Progression

Follow examples in this order:

1. `examples/HelloWorld`: minimal startup with `Tea.RunAsync(new App())`.
2. `examples/CounterForm`: configured startup with `Tea.CreateBuilder()`, `UseApp(...)`, and `ConfigureRuntime(...)`.
3. `examples/WorkspaceApp`: stateful multi-pane app using app-level messages/effects for coordinated flows.
4. Advanced interaction lane: `examples/AdvancedWidgets` and `examples/WidgetGallery` for richer overlays, command surfaces, and advanced behavior.

Default onboarding should stay in `TeaSharp` namespaces. `TeaSharp.Core` is the low-level advanced lane.

## Docs

- app model and startup: [docs/app-pattern.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/app-pattern.md)
- C#-first public API policy: [docs/public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- custom widgets: [docs/custom-components.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/custom-components.md)
- public API tiers: [docs/public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- legacy-to-new map: [docs/migration-map.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/migration-map.md)
- canonical progression: [examples/HelloWorld/Program.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/examples/HelloWorld/Program.cs), [examples/CounterForm/Program.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/examples/CounterForm/Program.cs), [examples/WorkspaceApp/Program.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/examples/WorkspaceApp/Program.cs)
- engine and namespace notes: [docs/spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md), [docs/namespace-migration.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/namespace-migration.md)
- control catalog: [docs/prebuilt-widgets.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/prebuilt-widgets.md)

## Build

The solution files remain useful for IDE navigation:

- `TeaSharp.slnx`
- `TeaSharp.Examples.slnx`
