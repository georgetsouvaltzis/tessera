# Getting Started With Tessera

This guide is the default onboarding path for Tessera public alpha.

If you are evaluating Tessera for a product, follow this order:

1. read the framework overview in [overview.md](overview.md)
2. run `HelloWorld`
3. run `CounterForm`
4. run `WorkspaceApp`
5. read [theme-system.md](theme-system.md) if you care about look and feel
6. then open the flagship showcases
7. read [architecture-overview.md](architecture-overview.md) if you want to contribute or extend the framework

## Prerequisites

- `.NET 10.0.103` SDK
- a terminal with solid ANSI/CSI support
  - Ghostty
  - iTerm2
  - Windows Terminal
  - macOS Terminal

Tessera is a library-first framework. You do not need ASP.NET hosting, dependency injection, or Generic Host wiring for the normal app path.

## The Public App Model

The public path is intentionally small:

1. derive from `TesseraApp`
2. build screens with `Screen.Build(...)`
3. use controls from `Tessera.Controls`
4. use layouts from `Tessera.Layout`
5. handle domain/runtime messages in `Update(Message)`
6. run with `TesseraApplication.RunAsync(...)` or `TesseraApplication.CreateBuilder()`

Preferred imports:

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;
```

## Choose Your First Example

Recommended learning order:

1. `examples/HelloWorld`
   - smallest visual starter
   - teaches the basic `TesseraApp` loop, centered layout, button events, and status text
2. `examples/CounterForm`
   - first interactive app
   - teaches text input, numeric input, choice, progress, and message-driven updates
3. `examples/WorkspaceApp`
   - first multi-pane starter
   - teaches navigation, editing, preview, and action flow inside one centered shell
4. `examples/GitConsole`
   - first larger workflow app
5. `examples/OpsWatch`
   - first larger dashboard app
6. `examples/DataWorkbench`
   - first richer workbench shell

Supporting demos such as `DownloadCenter`, `IncidentDesk`, `MusicDeck`, and `TransitBoard` are useful after the flagship path.

## Run The Examples

```bash
dotnet run --project examples/HelloWorld/HelloWorld.csproj
dotnet run --project examples/CounterForm/CounterForm.csproj
dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
dotnet run --project examples/GitConsole/GitConsole.csproj
dotnet run --project examples/OpsWatch/OpsWatch.csproj
dotnet run --project examples/DataWorkbench/DataWorkbench.csproj
```

## Where To Go Next

- product contract: [spec.md](spec.md)
- theme model: [theme-system.md](theme-system.md)
- public API boundaries: [public-api-guidelines.md](public-api-guidelines.md)
- API surface map: [public-api-inventory.md](public-api-inventory.md)
- custom controls: [custom-components.md](custom-components.md)
- contributing: [CONTRIBUTING.md](/contributing)
