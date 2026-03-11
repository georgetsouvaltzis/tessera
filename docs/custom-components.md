# TeaSharp Custom Components

For the recommended full-app shell, see `docs/app-pattern.md`.

TeaSharp custom components are built around three contracts:

- `ICanvasComponent`: render-only component.
- `IStatefulComponent`: render + local `Update(IMessage)` state transitions.
- `IMouseStatefulComponent`: optional bounds-aware mouse transitions via `UpdateMouse(MouseMsg, Rect)`.
- `IFocusableComponent`: explicit `IsFocused` state for keyboard-routing participation.
- `ComponentComposer`: deterministic slot composition and optional update routing for lower-level component subtrees.
  - default keyboard mode is focused-slot only
  - switch to `KeyboardRoutingMode.Broadcast` when a container should fan out input
  - use it for slot-based component trees inside a larger app shell
- `ScreenComposer`: named screen regions with frame snapshots, focus ownership, and mouse routing for larger app surfaces
  - prefer `ScreenRegionKey` fields over ad hoc string constants once a screen grows beyond a toy example
  - overlay helpers handle blocking modals/palettes and passive toast overlays without extra app-level hit-testing
- `InputRouter`: app-level key precedence across overlays, command bars, focused regions, and global shortcuts
  - typical scope order: `System` -> `Modal` -> `Palette` -> `CommandBar` -> `FocusedRegion` -> `Global`
  - use `CaptureWhileActive` for modal/palette/command scopes so lower handlers cannot accidentally run
  - use `blocksGlobalShortcuts` on text-entry scopes to suppress plain-character globals while editing
- `InteractiveScreenModel`: reusable screen shell for apps that compose a `ScreenComposer` + `InputRouter`
  - call `RouteKey(...)`, `RouteMouse(...)`, and `RenderScreen(...)` instead of hand-rolling `EnsureScreen` / `BeginFrame` / `CompleteFrame` glue

## Minimal Render-Only Component

```csharp
using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

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
using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

public sealed class CounterChip : IStatefulComponent, IFocusableComponent
{
    private int _count;

    public bool IsFocused { get; set; }

    public bool Update(IMessage message)
    {
        if (message is KeyPressMsg key && key.IsCharacter('+', KeyModifiers.None, ignoreCase: false))
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

## Compose In a Local Component Subtree

```csharp
var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
var composer = new ComponentComposer();

composer.Add(new ClockComponent(), new Rect(0, 0, 24, 3));
composer.Add(new CounterChip(), new Rect(24, 0, 24, 3));

// default keyboard routing targets the focused slot:
composer.Update(message);

composer.Render(canvas);
return canvas.Render();
```

## Practical Notes

- Keep components deterministic: all state transitions via `Update`.
- For mouse-aware widgets, keep bounds checks inside `UpdateMouse` and treat coordinates as canvas-space.
- Use `IFocusableComponent` instead of relying on naming conventions or reflection.
- Keep render pure: no side effects from `Render`.
- If you need full Unicode layout fidelity in component text, use `CanvasTextMode.GraphemeAware`.
- Use composer slots as an explicit layout graph; avoid hidden global state.
- For screen-scale apps, keep one owner per concern: `ScreenComposer` for regions/focus/mouse, `InputRouter` for key precedence.
- If your model is “one screen + some overlays + scoped shortcuts”, derive from `InteractiveScreenModel` and keep region keys as `static readonly ScreenRegionKey` fields.
- Treat `ComponentComposer` as a lower-level subtree helper, not the default top-level app shell.
