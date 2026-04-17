---
sidebar_label: App Model
---

# App Model

Tessera keeps the core app contract intentionally small:

- derive from `TesseraApp`
- optionally return an initial effect from `Initialize()`
- handle messages in `Update(Message)`
- return the next screen from `Build(ScreenContext)`

That is the public model you should learn first.

## Lifecycle order

The runtime calls your app in this order:

1. `Initialize()`
2. `Update(Message)` whenever runtime, input, or effect messages arrive
3. `Build(ScreenContext)` whenever the screen needs to render

## Input routing rules

Built-in controls participate in routing before your app sees messages:

- focused controls usually handle direct input first
- `KeyPressed` and `KeyReleased` still continue to `Update(...)` even when a control handles them
- handled pointer and paste messages can be swallowed by controls

That gives you a simple rule:

- use `Update(...)` for app-wide hotkeys and domain messages
- use control events for local UI interaction

## `Initialize()`

Use `Initialize()` when the app should start with an effect:

- schedule a periodic refresh
- emit a bootstrap message
- trigger a first background action

If you do not need startup work, return `null`.

## `Update(Message)`

Use `Update(...)` to:

- apply state transitions
- handle runtime messages such as `WindowResized` or `FocusChanged`
- translate app-wide input into effects
- respond to domain messages posted by controls

Typical global hotkey:

```csharp
public override TesseraEffect? Update(Message message)
{
    return message is KeyPressed key && key.IsCharacter('q', ModifierKeys.Ctrl)
        ? TesseraEffects.Quit
        : null;
}
```

## `Build(ScreenContext)`

Use `Build(...)` to compose the next screen from current state and runtime context.

`ScreenContext` exposes:

- `Width`
- `Height`
- `HasFocus`
- `Theme`
- `ThemeOverrides`
- `Bounds`

That is where responsive layout decisions and focus-aware hints usually belong.

## `Post(...)` and `RequestEffect(...)`

These two methods are the bridge from local control events back into app logic.

- `Post(message)`
  - queues a message to flow through `Update(...)` on the next pass
- `RequestEffect(effect)`
  - queues runtime work directly

Use `Post(...)` when you want control activity to become explicit app state transitions.

## Recommended event pattern

For tiny demos, direct state mutation from control events is acceptable.

For real apps, prefer this shape:

```csharp
private sealed record SaveRequested : Message;

public OrdersApp()
{
    _saveButton.Activated += (_, _) => Post(new SaveRequested());
}

public override TesseraEffect? Update(Message message)
{
    return message switch
    {
        SaveRequested => SaveOrder(),
        KeyPressed key when key.IsCharacter('q', ModifierKeys.Ctrl) => TesseraEffects.Quit,
        _ => null
    };
}
```

That keeps state changes visible in one place.

## When to stay simple

You do **not** need to force every tiny app into a message-heavy architecture. Keep the public path readable:

- direct event mutation for very small examples
- message-driven updates once the app has real domain state
- effects only when the runtime must do work outside normal state mutation

## Next step

- screen assembly: [layout-and-screen-composition.md](layout-and-screen-composition.md)
- runtime knobs: [runtime-and-screen-options.md](runtime-and-screen-options.md)
- starter examples: [examples.md](examples.md)
