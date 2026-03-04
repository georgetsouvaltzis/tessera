# TeaSharp Custom Components

TeaSharp custom components are built around three contracts:

- `ICanvasComponent`: render-only component.
- `IStatefulComponent`: render + local `Update(IMessage)` state transitions.
- `ComponentComposer`: deterministic slot composition and optional update routing.

## Minimal Render-Only Component

```csharp
using TeaSharp.Components;

public sealed class ClockComponent : ICanvasComponent
{
    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, "Clock");
        var body = rect.Inset(1, 1);
        canvas.WriteText(body.X, body.Y, DateTimeOffset.Now.ToString("HH:mm:ss"), body.Width);
    }
}
```

## Stateful Component

```csharp
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

public sealed class CounterChip : IStatefulComponent
{
    private int _count;

    public bool Update(IMessage message)
    {
        if (message is KeyPressMsg { Text: "+" })
        {
            _count++;
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, "Chip");
        var body = rect.Inset(1, 1);
        canvas.WriteText(body.X, body.Y, $"count={_count}", body.Width);
    }
}
```

## Compose In a Model View

```csharp
var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
var composer = new ComponentComposer();

composer.Add(new ClockComponent(), new Rect(0, 0, 24, 3));
composer.Add(new CounterChip(), new Rect(24, 0, 24, 3));

// optional for stateful components:
composer.Update(message);

composer.Render(canvas);
return canvas.Render();
```

## Practical Notes

- Keep components deterministic: all state transitions via `Update`.
- Keep render pure: no side effects from `Render`.
- If you need full Unicode layout fidelity in component text, use `CanvasTextMode.GraphemeAware`.
- Use composer slots as an explicit layout graph; avoid hidden global state.
