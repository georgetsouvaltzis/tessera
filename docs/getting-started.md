# Getting Started With TeaSharp

This guide is the default onboarding path for TeaSharp public alpha.

If you are evaluating TeaSharp for a product, follow this order:

1. read the quick-start snippet in [README.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/README.md)
2. run one flagship example from [examples.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/examples.md)
3. read [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md) if you care about look and feel
4. read [architecture-overview.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/architecture-overview.md) if you want to contribute or extend the framework

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

1. `examples/GitConsole`
   - best first real app
   - teaches command workflows, navigation, selection, and action handling
2. `examples/OpsWatch`
   - best first dashboard app
   - teaches telemetry surfaces, status-heavy layouts, and action rails
3. `examples/DataWorkbench`
   - best full-shell example
   - teaches multi-pane composition, richer state orchestration, and pointer-ready runtime configuration

Supporting demos such as `DownloadCenter`, `IncidentDesk`, `MusicDeck`, and `TransitBoard` are useful after the flagship path.

## Run The Examples

```bash
dotnet run --project examples/GitConsole/GitConsole.csproj
dotnet run --project examples/OpsWatch/OpsWatch.csproj
dotnet run --project examples/DataWorkbench/DataWorkbench.csproj
```

## Where To Go Next

- product contract: [spec.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/spec.md)
- theme model: [theme-system-v1.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/theme-system-v1.md)
- public API boundaries: [public-api-guidelines.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-guidelines.md)
- API surface map: [public-api-inventory.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/public-api-inventory.md)
- custom controls: [custom-components.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/docs/custom-components.md)
- contributing: [CONTRIBUTING.md](/Users/georgetsouvaltzis/Projects/playground/teasharp/CONTRIBUTING.md)
