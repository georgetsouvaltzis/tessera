# TeaSharp

TeaSharp is a message-driven terminal UI library for .NET.

The default app path is intentionally small:

- derive from `TeaApp`
- run apps with `Tea.RunAsync(...)` or `TeaApplicationBuilder`
- let built-in controls route automatically; `Update(...)` handles unhandled input plus runtime messages
- return `Screen` from `Build(ScreenContext)`
- assemble screens with `WindowLayout`, `RowLayout`, and `ColumnLayout`
- use first-class controls from `TeaSharp.Controls`
- configure runtime behavior with `TeaRuntimeOptions`
- keep `TeaSharp.Components.Composition` and `TeaSharp.Core.*` for advanced or transitional scenarios only

If you need custom runtime wiring, explicit region routing, or low-level component composition, those APIs still exist, but they are now marked `EditorBrowsable(Advanced)` and are no longer the starter path.

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
    private int _count;
    private readonly Button _increment = new()
    {
        Text = "Increment",
    };
    private readonly StatusBar _status = new();
    public CounterApp() => _increment.Activated += (_, _) => _count++;

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Count: {_count}";
        _status.RightText = "Enter increments   Ctrl+C quits";

        return Screen.From(new WindowLayout
        {
            Footer = LayoutSlot.Fixed(_status, 1),
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

## Docs

- app model and startup: [docs/app-pattern.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/app-pattern.md)
- custom widgets: [docs/custom-components.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/custom-components.md)
- public API tiers: [docs/public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- legacy-to-new map: [docs/migration-map.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/migration-map.md)
- canonical example app: [examples/Showcase/Program.cs](/Users/georgetsouvaltzis/Projects/playground/teasharp/examples/Showcase/Program.cs)
- engine and namespace notes: [docs/spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md), [docs/namespace-migration.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/namespace-migration.md)
- existing widget catalogs: [docs/prebuilt-widgets.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/prebuilt-widgets.md), [docs/widgets.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/widgets.md)

## Build

The solution files remain useful for IDE navigation:

- `TeaSharp.slnx`
- `TeaSharp.Examples.slnx`
