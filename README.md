# TeaSharp

TeaSharp is a C#-first terminal UI framework for building state-driven terminal applications on `.NET 10`.

TeaSharp is now in public alpha. The repository is ready for evaluation, experimentation, and contribution, but API cleanup is still allowed when it improves the long-term public authoring path.

## Why TeaSharp

- small default app model
- explicit C# object model instead of a nested DSL
- no DI container or Generic Host required for the normal path
- built-in controls, layouts, themes, and runtime options
- advanced hosting/runtime seams available, but not required

## Start Here

1. Read [docs/getting-started.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/getting-started.md).
2. Run the examples listed in [docs/examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md).
3. Use [docs/theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md) for theming and [docs/custom-components.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/custom-components.md) for custom controls.
4. If you want to contribute, read [CONTRIBUTING.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CONTRIBUTING.md) and [docs/architecture-overview.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/architecture-overview.md).

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

For the minimal path, `await Tea.RunAsync(new App());` remains supported.

## Example Lineup

Flagship examples:

- `examples/GitConsole`: workflow-heavy command surface with editing, diff review, and action history
- `examples/OpsWatch`: dashboard-first operations surface with alerts, telemetry, and action rails
- `examples/DataWorkbench`: multi-pane workbench shell with pointer-ready runtime configuration

Supporting demos:

- `examples/DownloadCenter`
- `examples/IncidentDesk`
- `examples/MusicDeck`
- `examples/TransitBoard`

The full example guide lives in [docs/examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md).

## Docs

- onboarding guide: [docs/getting-started.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/getting-started.md)
- example guide: [docs/examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md)
- architecture overview: [docs/architecture-overview.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/architecture-overview.md)
- design contract: [docs/spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md)
- public API guidelines: [docs/public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- public API inventory: [docs/public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- theme system: [docs/theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md)
- custom controls: [docs/custom-components.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/custom-components.md)
- contributor guide: [CONTRIBUTING.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CONTRIBUTING.md)
- code of conduct: [CODE_OF_CONDUCT.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CODE_OF_CONDUCT.md)
- security policy: [SECURITY.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/SECURITY.md)

## Build And Verify

TeaSharp uses the `.NET 10.0.103` SDK pinned in [global.json](/Users/georgetsouvaltzis/Projects/playground/teasharp/global.json).

Primary repo verification commands:

```bash
dotnet build TeaSharp.slnx
dotnet test TeaSharp.slnx
scripts/smoke_examples_v1.sh 4
```

Example-specific commands:

```bash
dotnet run --project examples/GitConsole/GitConsole.csproj
dotnet run --project examples/OpsWatch/OpsWatch.csproj
dotnet run --project examples/DataWorkbench/DataWorkbench.csproj
```

## Repo Layout

- `src/TeaSharp`: default public app-authoring API
- `src/TeaSharp.Core`: advanced low-level runtime layer
- `tests/TeaSharp.Tests`: unit, contract, and regression tests
- `tests/TeaSharp.IntegrationTests`: integration coverage
- `examples`: public examples and showcase apps
- `docs`: product, architecture, release, and contributor docs

## Contributing

TeaSharp is being shaped in public. If you want to contribute, start with [CONTRIBUTING.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CONTRIBUTING.md).
