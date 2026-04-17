---
sidebar_label: Starter Examples
---

# Starter Examples

These are the three examples to run before you judge the flagship shells. They stay on the default public path and teach the app model in increasing density.

## Recommended Order

1. `HelloWorld`
2. `CounterForm`
3. `WorkspaceApp`

## `HelloWorld`

The smallest polished entry point.

- confirms your terminal baseline
- shows the basic `TesseraApp` loop
- keeps the screen intentionally sparse

```bash
dotnet run --project examples/HelloWorld/HelloWorld.csproj
```

## `CounterForm`

The first interactive form surface.

- introduces text input, numeric input, and choice
- keeps updates message-driven and easy to trace
- proves the default path can already feel like software, not a toy

```bash
dotnet run --project examples/CounterForm/CounterForm.csproj
```

## `WorkspaceApp`

The first denser starter shell.

- combines navigation, editing, preview, and actions
- shows how multiple panes still fit the same app model
- is the handoff point into the flagship demos

```bash
dotnet run --project examples/WorkspaceApp/WorkspaceApp.csproj
```

## Next Step

Once the starter ladder feels coherent, continue to [showcase.md](showcase.md) for `GitConsole`, `OpsWatch`, `DataWorkbench`, and the supporting demos.
