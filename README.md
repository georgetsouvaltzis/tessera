# TeaSharp

TeaSharp is a message-driven terminal UI library for .NET.

The default app path is intentionally small:

- derive from `TeaApp`
- run apps with `Tea.RunAsync(...)` or `TeaApplicationBuilder`
- return `Screen` from `Build(ScreenContext)`
- configure runtime behavior with `TeaRuntimeOptions`
- keep `TeaSharp.Components.Composition` and `TeaSharp.Core.*` for advanced or transitional scenarios only

If you need custom runtime wiring, explicit region routing, or low-level component composition, those APIs still exist, but they are now marked `EditorBrowsable(Advanced)` and are no longer the starter path.

## Quick Start

```csharp
using TeaSharp;

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

    public override TeaEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.Is(Key.Up))
        {
            _count++;
            return null;
        }

        if (key.Is(Key.Down))
        {
            _count--;
            return null;
        }

        return key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        return Screen.From(
            $"""
            Counter

            Size: {context.Width} x {context.Height}
            Count: {_count}

            Up / Down: change
            Ctrl+C: quit
            """);
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
- engine and namespace notes: [docs/spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md), [docs/namespace-migration.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/namespace-migration.md)
- existing widget catalogs: [docs/prebuilt-widgets.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/prebuilt-widgets.md), [docs/widgets.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/widgets.md)

## Build

The solution files remain useful for IDE navigation:

- `TeaSharp.slnx`
- `TeaSharp.Examples.slnx`
