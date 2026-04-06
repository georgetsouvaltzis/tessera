# Getting Started With TeaSharp

This guide is the default onboarding path for TeaSharp public alpha.

If you are evaluating TeaSharp for a product, follow this order:

1. read the quick-start snippet in [README.md](../README.md)
2. run `HelloWorld`
3. run `CounterForm`
4. run `WorkspaceApp`
5. read [theme-system-v1.md](theme-system-v1.md) if you care about look and feel
6. then open the flagship showcases
7. read [architecture-overview.md](architecture-overview.md) if you want to contribute or extend the framework

## Prerequisites

- `.NET 10.0.103` SDK
- a terminal with solid ANSI/CSI support
  - Ghostty
  - iTerm2
  - Windows Terminal
  - macOS Terminal

TeaSharp is a library-first framework. You do not need ASP.NET hosting, dependency injection, or Generic Host wiring for the normal app path.

## The Public App Model

The public path is intentionally small:

1. derive from `TeaApp`
2. build screens with `Screen.Build(...)`
3. use controls from `TeaSharp.Controls`
4. use layouts from `TeaSharp.Layout`
5. handle domain/runtime messages in `Update(Message)`
6. run with `Tea.RunAsync(...)` or `Tea.CreateBuilder()`

Preferred imports:

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
```

## Choose Your First Example

Recommended learning order:

1. `examples/HelloWorld`
   - smallest visual starter
   - teaches the basic `TeaApp` loop, centered layout, button events, and status text
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
- theme model: [theme-system-v1.md](theme-system-v1.md)
- public API boundaries: [public-api-guidelines.md](public-api-guidelines.md)
- API surface map: [public-api-inventory.md](public-api-inventory.md)
- custom controls: [custom-components.md](custom-components.md)
- contributing: [CONTRIBUTING.md](../CONTRIBUTING.md)
