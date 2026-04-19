# Tessera Custom Components

For the default app shell, startup pattern, and default composition path, see [public-api-guidelines](/docs/public-api-guidelines).

Tessera keeps custom widgets available, but through a smaller contract than the full runtime engine.

## Default Contract

Use `Tessera.Controls.Control` when you want a reusable interactive widget.

`Control` already bridges into the existing component/runtime pipeline and gives you:

- `Render(Canvas, Rect)`
- `Handle(Message)`
- `Handle(Message, Rect)` for bounds-aware pointer work
- `IsFocused`
- `IsDisabled`
- `IsReadOnly`

This keeps custom widgets independent from `InputRouter`, `ScreenRegionKey`, and other screen-scale routing types.
The bridge into the older component/runtime pipeline is now internal, so custom widget authors do not need to implement the legacy component interfaces directly.

## Minimal Custom Control

```csharp
using Tessera;
using Tessera.Controls;
using Tessera.Components.Primitives;

public sealed class CounterBadge : Control
{
    private int _count;

    public override bool Handle(Message message)
    {
        if (message is KeyPressed key && key.IsCharacter('+', ignoreCase: false))
        {
            _count++;
            return true;
        }

        return false;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, "Counter");
        var body = rect.Inset(1, 1);
        canvas.WriteText(body.X, body.Y, $"count={_count}", body.Width);
    }
}
```

## When To Use The Advanced Contracts

The remaining raw component contract is:

- `ICanvasComponent`

The old screen-scale composition engine is now internal-only. Use `ICanvasComponent` only when you need:

- render-only interop with an existing low-level canvas primitive
- a raw drawing surface without control-level input/focus behavior

## Design Rules For Custom Widgets

- keep rendering pure; no side effects from `Render`
- keep state transitions in `Handle`
- prefer typed `Message` handling over raw core messages
- keep focus local to the control
- only drop to advanced component contracts when the widget truly needs low-level interop

The intent is simple: custom controls should be easy to write without forcing authors to learn the runtime engine.
