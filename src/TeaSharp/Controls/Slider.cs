using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a bounded slider control.
/// </summary>
public sealed class Slider : Control
{
    private bool _hovered;
    private bool _dragging;
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Slider";

    public double Min
    {
        get;
        set;
    }

    public double Max
    {
        get;
        set;
    } = 100.0;

    public double Step
    {
        get;
        set;
    } = 1.0;

    public double Value
    {
        get;
        private set;
    }

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

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    public void SetValue(double value) => Value = Clamp(value);

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('-'))
        {
            var previous = Value;
            Value = Clamp(Value - Step);
            return !AreClose(previous, Value);
        }

        if (key.Is(Key.Right) || key.IsCharacter('+'))
        {
            var previous = Value;
            Value = Clamp(Value + Step);
            return !AreClose(previous, Value);
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var changed = false;
        if (pointer is { Kind: PointerEventKind.Release, Button: PointerButton.Left })
        {
            var wasDragging = _dragging;
            _dragging = false;
            changed |= SetHovered(content.Contains(pointer.X, pointer.Y));
            return changed || wasDragging;
        }

        if (pointer is { Kind: PointerEventKind.Motion, Button: PointerButton.Left } && _dragging)
        {
            changed |= SetHovered(content.Contains(pointer.X, pointer.Y));
            changed |= SetValueFromPointer(pointer.X, content);
            return changed;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
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

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            var before = Value;
            if (pointer.Button == PointerButton.WheelUp)
            {
                Value = Clamp(Value + Step);
            }
            else if (pointer.Button == PointerButton.WheelDown)
            {
                Value = Clamp(Value - Step);
            }

            return changed || !AreClose(before, Value);
        }

        if (pointer is { Kind: PointerEventKind.Press, Button: PointerButton.Left } && IsPointerOnBarRow(content, pointer.Y))
        {
            _dragging = true;
            changed |= SetHovered(true);
            changed |= SetValueFromPointer(pointer.X, content);
            return changed;
        }

        return Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);

        if (content.IsEmpty)
        {
            return;
        }

        var states = new List<WidgetVisualState>(4);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (IsDisabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (IsReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        var label = $"{Value:0.##} / {Max:0.##}";
        canvas.WriteText(content.X, content.Y, _statePalette.Render(label, states), content.Width);
        if (content.Height > 1)
        {
            TeaSharp.Components.Primitives.Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y + 1, content.Width, 1), Normalize());
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, Title.Length + 8);
        var height = Border == BorderStyle.None ? 2 + Padding.Vertical : 4 + Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private double Normalize()
    {
        var range = Max - Min;
        if (range <= 0.0)
        {
            return 0.0;
        }

        return Math.Clamp((Value - Min) / range, 0.0, 1.0);
    }

    private double Clamp(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

    private static bool AreClose(double left, double right) => Math.Abs(left - right) <= 0.000001;

    private Rect ResolveContentRect(Rect bounds) => FrameLayout.ResolveContentRect(bounds, Border, Padding);

    private static bool IsPointerOnBarRow(Rect content, int y)
    {
        var barY = content.Height > 1
            ? content.Y + 1
            : content.Y;
        return y == barY;
    }

    private bool SetValueFromPointer(int x, Rect content)
    {
        if (Max <= Min)
        {
            return false;
        }

        var barX = content.X + 1;
        var barWidth = Math.Max(1, content.Width - 2);
        var clampedX = Math.Clamp(x, barX, barX + barWidth - 1);
        var normalized = barWidth == 1
            ? 1.0
            : (double)(clampedX - barX) / Math.Max(1, barWidth - 1);
        var before = Value;
        Value = Clamp(Min + ((Max - Min) * normalized));
        return !AreClose(before, Value);
    }

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
