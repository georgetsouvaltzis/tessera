---
sidebar_label: Starter Examples
---

# Starter Examples

These are the three examples to run before you judge the flagship shells. They stay on the default public path and teach the app model in increasing density.

## Recommended Order

1. `HelloWorld`
2. `CounterForm`
3. `WorkspaceApp`

## What the starter ladder proves

- the basic app loop is easy to understand
- the same public path holds as layout density increases
- you can stay in `Tessera`, `Tessera.Controls`, and `Tessera.Layout`
- the framework reads like ordinary C#, not like a hidden DSL

## Quick reference

| Example | Run it when you want to learn | Command |
| --- | --- | --- |
| `HelloWorld` | the smallest polished entry point | `dotnet run --project examples/HelloWorld/HelloWorld.csproj` |
| `CounterForm` | the first interactive form surface | `dotnet run --project examples/CounterForm/CounterForm.csproj` |
| `WorkspaceApp` | the first denser starter shell | `dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj` |

## `HelloWorld`

Use this first to confirm the terminal, theme, and button/message loop all read clearly.

```bash
dotnet run --project examples/HelloWorld/HelloWorld.csproj
```

Look for:

- centered layout and clean default spacing
- a direct `TesseraApp` -> `Screen.Build(...)` loop
- button activation and status text without extra host setup

## `CounterForm`

Use this second for the first real form surface.

```bash
dotnet run --project examples/CounterForm/CounterForm.csproj
```

Look for:

- text input, numeric input, and choice controls
- a small message-driven app shell
- form interaction without extra host configuration

## `WorkspaceApp`

Use this third when you want the first denser layout.

```bash
dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
```

Look for:

- multi-pane composition on the default public path
- preview, editing, and action flow in one screen
- the same mental model as the smaller starters

## Questions to ask before moving on

- Does the startup story still feel small?
- Does `Build(...)` stay readable as the screen gets denser?
- Do you understand where state changes happen?
- Do the controls feel like product surfaces rather than isolated widgets?

## Next step

Once the starter ladder feels coherent, continue to [showcase](/docs/showcase) for `GitConsole`, `OpsWatch`, `DataWorkbench`, and the supporting demos.
