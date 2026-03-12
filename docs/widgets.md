# TeaSharp Widgets

`TeaSharp.Widgets` is the low-level stateful widget layer.

It remains public for advanced composition and custom controls, but it is no longer the default app path.

## When To Use It

Use `TeaSharp.Widgets` when you are:

- building custom controls directly on widget models
- reusing the low-level text input, viewport, or list behavior
- implementing advanced component wrappers
- writing experiments below the root control catalog

Do not start normal apps here. Start with:

- `TeaApp`
- `TeaSharp.Controls`
- `TeaSharp.Layout`

## Widget Models

Current advanced widget models include:

- `ViewportModel`
- `TextInputModel`
- `ListModel<T>`

These types provide lower-level behavior such as:

- scrolling
- filtering
- paging
- cursor movement
- text editing
- viewport rendering

## Keymaps

Advanced widget keymaps remain available:

- `ViewportKeyMap`
- `TextInputKeyMap`
- `ListKeyMap`
- `KeyBinding`

These are intentionally not the main beginner-facing interaction model anymore.

## Design Position

The root controls should cover the common app path.
The widget layer exists so TeaSharp stays adaptable.

That means:

- root controls optimize for usability
- widget models optimize for extensibility

## Example

```csharp
using TeaSharp.Components.Primitives;
using TeaSharp.Widgets;

var viewport = new ViewportModel();
viewport.SetLines(["alpha", "beta", "gamma"]);
viewport.Resize(20, 3);

foreach (var line in viewport.RenderLines())
{
    Console.WriteLine(line);
}
```

For most apps, prefer the wrapped controls:

- `TextInput` over `TextInputModel`
- `ListView<T>` over `ListModel<T>`

See also:

- `docs/components.md`
- `docs/custom-components.md`
