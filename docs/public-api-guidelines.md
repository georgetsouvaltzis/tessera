# Tessera Public API Guidelines

This document is the implementation policy for a C#-first Tessera API.

## Audience And Layers

Tessera supports two intentional product layers:

- `Tessera`: primary app-authoring surface for most C# developers building TUIs.
- `Tessera.Core`: low-level product for expert/runtime-driven scenarios.

Advanced host seams (`Tessera.Hosting`) remain supported as an opt-in lane, but they are not the beginner path.

## C#-First Rules

Use familiar .NET patterns by default:

- explicit object models, object initializers, and strongly typed options
- `EventHandler` / `EventHandler<TEventArgs>` for control notifications
- async APIs with `Async` suffix and `CancellationToken` as the last optional parameter
- immutable message payloads (records) for application message flow
- `IAsyncDisposable` for runtime/terminal resources

Avoid framework-specific patterns when BCL conventions already solve the problem.

## Canonical Startup Pattern

Support two startup lanes:

- minimal: `await TesseraApplication.RunAsync(new App());`
- configured: `TesseraApplication.CreateBuilder().UseApp<TApp>().ConfigureRuntime(...).Build()`

Tessera is a library-first TUI framework. Default startup should not require DI containers or Generic Host wiring.

Minimal app shape for docs/examples:

1. derive from `TesseraApp`
2. handle app/runtime input through `Message`
3. let built-in controls route handled input before `Update(...)`
4. return a `Screen` from `Build(ScreenContext)`

Typical configured startup:

```csharp
var app = TesseraApplication.CreateBuilder()
    .UseApp<MyApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.MaxFps = 60;
        runtime.Theme = TesseraThemes.Catppuccin(CatppuccinVariant.Mocha);
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "MyApp",
            EnableFocusReporting = true,
        };
    })
    .Build();

await app.RunAsync();
```

## Canonical Learning Path

Public onboarding should teach one story:

1. start with [overview.md](overview.md)
2. complete [install-and-prerequisites.md](install-and-prerequisites.md)
3. build [first-app.md](first-app.md)
4. use the starter example order in [examples.md](examples.md): `HelloWorld` -> `CounterForm` -> `WorkspaceApp`
5. use the flagship evaluation path in [showcase.md](showcase.md) only after the starter ladder is clear

Keep examples in `Tessera` namespaces. `Tessera.Core` is the low-level advanced lane and should not appear in the starter or flagship public examples.

## Canonical Theme Pattern

Theming must be semantic-token based and overrideable:

- semantic tokens (`Text`, `Surface`, `Border`, `State`, `Accent`, `Selection`, `Focus`)
- built-in palettes (Catppuccin, Rosé Pine)
- custom user palette
- override hierarchy: global -> control type -> control instance -> state

Focus styling must be configurable beyond `"*"` markers. Border/title/focus visuals should be theme-driven.

## Canonical App Pattern

Default integration model for app code:

1. controls raise events
2. app posts domain messages with `Post(...)` when state changes should flow through the state machine
3. `Update(...)` applies state transitions and returns effects
4. `Build(...)` returns the next screen

For tiny demos, direct event mutation is acceptable. Production examples should prefer message-driven updates.

## Canonical Composition Pattern

Default composition path:

- `Screen.Build(...)` + `WindowBuilder`
- root controls from `Tessera.Controls`
- root layouts from `Tessera.Layout`
- drawing primitives only when needed: `Tessera.Components.Primitives`

Alternative composition surfaces may remain public for advanced scenarios, but docs and starter examples should teach the default path first.

Preferred default imports:

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;
```

## Boundary Rules

- Normal app examples should not import `Tessera.Core.*`.
- Public docs should use `Tessera.Styles` (not legacy namespace names).
- Runtime knobs for advanced hosting should live under `Tessera.Hosting` discoverability as opt-in APIs, not the default path.
- Images are V1.1 scope, not V1 scope.

## Review Checklist

Before merging API/docs/example changes:

- Is the beginner path still one obvious path?
- Does this follow idiomatic C# and .NET conventions?
- Does this introduce a second equal-status integration style?
- Does this leak low-level runtime vocabulary into default app guidance?
