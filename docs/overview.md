# Overview

Tessera is a C#-first terminal UI framework for teams that want deliberate application structure, product-grade visuals, and built-in controls without committing to a host-heavy stack.

## Product-first terminal UI

The public path stays intentionally compact:

- derive from `TesseraApp`
- compose screens with `Screen.Build(...)`
- use controls from `Tessera.Controls`
- keep runtime seams optional until you actually need them

Three ideas define the product posture:

- `C# object model`: explicit screens, controls, layouts, and messages instead of a nested layout DSL
- `Theme-driven visuals`: tokens, control defaults, instance overrides, and state styling are part of the public contract
- `Grow when you need to`: `Tessera.Hosting` and `Tessera.Core` stay available for deeper runtime seams

## Public app model

Normal apps should live in `Tessera`, `Tessera.Controls`, `Tessera.Layout`, and `Tessera.Styles`. The framework should read like explicit screen composition in ordinary C#.

1. Build the app shell with `TesseraApp` and return screens from `Build(ScreenContext)`.
2. Handle messages in `Update(Message)` and return effects only when needed.
3. Keep the path shallow with `TesseraApplication.RunAsync(...)` or `CreateBuilder()` until the app truly needs deeper runtime seams.

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
    public override Screen Build(ScreenContext context)
        => Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(body => body.Center(
                new Button { Text = "Open orders" },
                width: 20,
                height: 3));
        });
}
```

## Why teams evaluate it

Tessera is not trying to be the smallest widget sandbox. It is trying to be the best default path for real terminal software in C#.

- `Polished starter ladder`: `HelloWorld`, `CounterForm`, and `WorkspaceApp` teach the framework without dumping the whole catalog at once
- `Flagship evaluation apps`: `GitConsole`, `OpsWatch`, and `DataWorkbench` show how the public path scales into denser surfaces
- `Contributor-friendly shape`: the repo is organized around public API boundaries, examples, tests, docs, and release checklists instead of hidden internal magic
