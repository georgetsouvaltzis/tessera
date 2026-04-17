# Getting Started With Tessera

This guide is the default onboarding path for Tessera public alpha.

If you are evaluating Tessera for a product, follow this order:

1. read [overview.md](overview.md)
2. run `HelloWorld`
3. run `CounterForm`
4. run `WorkspaceApp`
5. open [showcase.md](showcase.md) for the flagship shells
6. read [theme-system.md](theme-system.md) when visuals and overrides start to matter

## Before You Begin

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

## Run The Starter Ladder

### `HelloWorld`

Use this first to confirm the terminal, theme, and button/message loop all read clearly.

```bash
dotnet run --project examples/HelloWorld/HelloWorld.csproj
```

Look for:

- centered layout and clean default spacing
- a direct `TesseraApp` -> `Screen.Build(...)` loop
- button activation and status text without extra host setup

### `CounterForm`

Use this second for the first real form surface.

```bash
dotnet run --project examples/CounterForm/CounterForm.csproj
```

Look for:

- text input, numeric input, and choice controls
- message-driven state updates instead of hidden framework magic
- a small but realistic app shell

### `WorkspaceApp`

Use this third when you want the first denser layout.

```bash
dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
```

Look for:

- multi-pane composition on the default public path
- preview, editing, and action flow in one screen
- the same mental model as the smaller starters

## Where To Go Next

- starter example catalog: [examples.md](examples.md)
- flagship and supporting demos: [showcase.md](showcase.md)
- product contract: [spec.md](spec.md)
- theme model: [theme-system.md](theme-system.md)
- API surface map: [api-reference.mdx](api-reference.mdx)
- contributor path: [architecture-overview.md](architecture-overview.md)
