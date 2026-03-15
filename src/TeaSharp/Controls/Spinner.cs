using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents an animated busy indicator.
/// </summary>
public sealed class Spinner : Control
{
    private readonly List<string> _frames = ["|", "/", "-", "\\"];
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();
    private bool _hovered;
    private int _frameIndex;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Spinner";

    public string Label
    {
        get;
        set => field = value ?? string.Empty;
    } = "loading";

    public bool Running
    {
        get;
        private set;
    } = true;

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    public void SetRunning(bool running) => Running = running;

    public void Advance()
    {
        if (_frames.Count == 0)
        {
            return;
        }

        _frameIndex = (_frameIndex + 1) % _frames.Count;
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Enter))
        {
            Running = !Running;
            return true;
        }

        if ((key.Is(Key.Right) || key.IsCharacter(' ')) && Running)
        {
            Advance();
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (_frames.Count == 0 || content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Wheel && Running && pointer.Button is PointerButton.WheelUp or PointerButton.WheelDown)
        {
            Advance();
            return true;
        }

        if (pointer is { Kind: PointerEventKind.Press, Button: PointerButton.Left })
        {
            changed |= SetHovered(true);
            Running = !Running;
            return true;
        }

        return Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _frames.Count == 0)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var states = new List<WidgetVisualState>(3);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        states.Add(Running ? WidgetVisualState.Loading : WidgetVisualState.ReadOnly);
        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        canvas.WriteText(content.X, content.Y, _statePalette.Render($"{_frames[_frameIndex]} {Label}", states), content.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, Title.Length + Label.Length + 6);
        var height = Border == BorderStyle.None ? 1 + Padding.Vertical : 3 + Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private Rect ResolveContentRect(Rect bounds) => FrameLayout.ResolveContentRect(bounds, Border, Padding);

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
    }
}
