using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class SliderComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private bool _hovered;
    private bool _dragging;

    public string Title { get; set; } = "Slider";

    public double Min { get; set; } = 0.0;

    public double Max { get; set; } = 100.0;

    public double Value { get; private set; }

    public double Step { get; set; } = 1.0;

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public KeyBinding DecreaseKey { get; set; } = new("left/-", "decrease", "left", "-");

    public KeyBinding IncreaseKey { get; set; } = new("right/+", "increase", "right", "+");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetValue(double value)
    {
        Value = Clamp(value);
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (DecreaseKey.Matches(key))
        {
            var previous = Value;
            Value = Clamp(Value - Step);
            return !AreClose(previous, Value);
        }

        if (IncreaseKey.Matches(key))
        {
            var previous = Value;
            Value = Clamp(Value + Step);
            return !AreClose(previous, Value);
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly)
        {
            return false;
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return false;
        }

        var changed = false;
        if (message is MouseReleaseMsg { Button: MouseButton.Left })
        {
            var wasDragging = _dragging;
            _dragging = false;
            changed |= SetHovered(content.Contains(message.X, message.Y));
            return changed || wasDragging;
        }

        if (message is MouseMotionMsg motion && _dragging && motion.Button == MouseButton.Left)
        {
            changed |= SetHovered(content.Contains(motion.X, motion.Y));
            changed |= SetValueFromPointer(motion.X, content);
            return changed;
        }

        var inside = content.Contains(message.X, message.Y);
        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (message is MouseClickMsg && InteractionProfile.HoverOnClick)
        {
            changed |= SetHovered(true);
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            var before = Value;
            if (wheel.Button == MouseButton.WheelUp)
            {
                Value = Clamp(Value + Step);
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                Value = Clamp(Value - Step);
            }

            changed |= !AreClose(before, Value);
        }

        if (message is MouseClickMsg { Button: MouseButton.Left } click
            && InteractionProfile.ActivateOnClick
            && IsPointerOnBarRow(content, click.Y))
        {
            _dragging = true;
            changed |= SetValueFromPointer(click.X, content);
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        var states = ResolveStates();
        var label = $"{Value:0.##} / {Max:0.##}";
        canvas.WriteText(content.X, content.Y, StatePalette.Render(label, states), content.Width);
        if (content.Height > 1)
        {
            var normalized = Normalize();
            Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y + 1, content.Width, 1), normalized);
        }
    }

    private List<WidgetVisualState> ResolveStates()
    {
        var states = new List<WidgetVisualState>(4);
        if (Focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (Disabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (ReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
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

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

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

