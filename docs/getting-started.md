# Getting Started With Tessera

This page is the default onboarding map for Tessera public alpha.

If you are evaluating Tessera for a real product, follow this order:

1. read [overview.md](overview.md)
2. complete [install-and-prerequisites.md](install-and-prerequisites.md)
3. build [first-app.md](first-app.md)
4. run the starter ladder from [examples.md](examples.md)
5. open [showcase.md](showcase.md) for the flagship shells
6. use the concept pages when you need deeper understanding:
   - [app-model.md](app-model.md)
   - [layout-and-screen-composition.md](layout-and-screen-composition.md)
   - [runtime-and-screen-options.md](runtime-and-screen-options.md)
   - [architectural-review.md](architectural-review.md)
   - [controls-overview.md](controls-overview.md)
   - [recipes.md](recipes.md)

## Before You Begin

- `.NET 10.0.103` SDK
- a terminal with solid ANSI/CSI support
  - Ghostty
  - iTerm2
  - Windows Terminal
  - macOS Terminal

Tessera is a library-first framework. You do not need ASP.NET hosting, dependency injection, or Generic Host wiring for the normal app path.

## Fastest onboarding path

If you want the shortest path from zero to a running app:

1. create a `net10.0` console app
2. `dotnet add package Tessera`
3. paste the sample from [first-app.md](first-app.md)
4. run it
5. move into the starter examples

## Recommended docs path

### 1. Install and prerequisites

Start here if you have not added Tessera to a project yet:

- [install-and-prerequisites.md](install-and-prerequisites.md)

### 2. Your first app

Use this page for the first runnable app and the first explanation of the public app shape:

- [first-app.md](first-app.md)

### 3. Starter ladder

Run these examples in order:

- `HelloWorld`
- `CounterForm`
- `WorkspaceApp`

The full commands and “what to look for” checklist live in [examples.md](examples.md).

### 4. Flagship evaluation

Once the starter path feels coherent, move into:

- `GitConsole`
- `OpsWatch`
- `DataWorkbench`

Use [showcase.md](showcase.md) for the commands and evaluation goals.

## The public app model

Preferred imports:

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Layout;
```

The default story stays intentionally small:

1. derive from `TesseraApp`
2. build screens with `Screen.Build(...)`
3. use controls from `Tessera.Controls`
4. use layouts from `Tessera.Layout`
5. handle domain/runtime messages in `Update(Message)`
6. run with `TesseraApplication.RunAsync(...)` or `TesseraApplication.CreateBuilder()`

## Where To Go Next

- install and package setup: [install-and-prerequisites.md](install-and-prerequisites.md)
- first runnable app: [first-app.md](first-app.md)
- starter example catalog: [examples.md](examples.md)
- flagship and supporting demos: [showcase.md](showcase.md)
- lifecycle and message flow: [app-model.md](app-model.md)
- composition model: [layout-and-screen-composition.md](layout-and-screen-composition.md)
- runtime and screen knobs: [runtime-and-screen-options.md](runtime-and-screen-options.md)
- control families: [controls-overview.md](controls-overview.md)
- architecture map: [architectural-review.md](architectural-review.md)
- common recipes: [recipes.md](recipes.md)
- theme model: [theme-system.md](theme-system.md)
- API surface map: [api-reference.mdx](api-reference.mdx)
