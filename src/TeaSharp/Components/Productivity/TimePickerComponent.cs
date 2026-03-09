using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class TimePickerComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private TimePickerField? _hoveredField;

    public string Title { get; set; } = "Time Picker";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public TimeOnly Value { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    public TimeOnly? LastCommittedTime { get; private set; }

    public TimePickerField ActiveField { get; private set; }

    public int HourStep { get; set; } = 1;

    public int MinuteStep { get; set; } = 1;

    public int SecondStep { get; set; } = 5;

    public KeyBinding NextFieldKey { get; set; } = new("right/l", "next field", "right", "l");

    public KeyBinding PreviousFieldKey { get; set; } = new("left/h", "previous field", "left", "h");

    public KeyBinding IncreaseKey { get; set; } = new("up/k", "increase", "up", "k");

    public KeyBinding DecreaseKey { get; set; } = new("down/j", "decrease", "down", "j");

    public KeyBinding CommitKey { get; set; } = new("enter/space", "commit time", "enter", "space");

    public WidgetStatePalette FieldStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile { get; set; } = WidgetInteractionProfile.Default.Clone();

    public void SetValue(TimeOnly time)
    {
        Value = time;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextFieldKey.Matches(key))
        {
            ActiveField = (TimePickerField)(((int)ActiveField + 1) % 3);
            return true;
        }

        if (PreviousFieldKey.Matches(key))
        {
            ActiveField = (TimePickerField)(((int)ActiveField + 2) % 3);
            return true;
        }

        if (IncreaseKey.Matches(key))
        {
            Adjust(1);
            return true;
        }

        if (DecreaseKey.Matches(key))
        {
            Adjust(-1);
            return true;
        }

        if (CommitKey.Matches(key))
        {
            LastCommittedTime = Value;
            return true;
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

        var inside = content.Contains(message.X, message.Y);
        var changed = false;
        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredField(null);
            }

            return changed;
        }

        var field = FieldFromPointer(content, message.X, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredField(field);
            return changed;
        }

        if (message is MouseClickMsg or MouseReleaseMsg)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredField(field);
            }

            if (message.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && field.HasValue)
            {
                if (ActiveField != field.Value)
                {
                    ActiveField = field.Value;
                    changed = true;
                }
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (field.HasValue && ActiveField != field.Value)
            {
                ActiveField = field.Value;
                changed = true;
            }

            if (wheel.Button == MouseButton.WheelUp)
            {
                Adjust(1);
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelDown)
            {
                Adjust(-1);
                changed = true;
            }
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

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var hour = RenderField(Value.Hour.ToString("D2"), TimePickerField.Hour);
        var minute = RenderField(Value.Minute.ToString("D2"), TimePickerField.Minute);
        var second = RenderField(Value.Second.ToString("D2"), TimePickerField.Second);
        canvas.WriteText(content.X, content.Y, $"{hour}:{minute}:{second}", content.Width);
    }

    private string RenderField(string value, TimePickerField field)
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

        if (field == ActiveField)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (_hoveredField.HasValue && _hoveredField.Value == field)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return FieldStatePalette.Render(value, states);
    }

    private void Adjust(int direction)
    {
        var delta = ActiveField switch
        {
            TimePickerField.Hour => TimeSpan.FromHours(HourStep * direction),
            TimePickerField.Minute => TimeSpan.FromMinutes(MinuteStep * direction),
            _ => TimeSpan.FromSeconds(SecondStep * direction),
        };
        Value = Value.Add(delta);
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
    }

    private static TimePickerField? FieldFromPointer(Rect content, int x, int y)
    {
        if (y < content.Y || y >= content.Bottom)
        {
            return null;
        }

        var index = x - content.X;
        if (index < 0)
        {
            return null;
        }

        return index switch
        {
            <= 2 => TimePickerField.Hour,
            <= 5 => TimePickerField.Minute,
            <= 8 => TimePickerField.Second,
            _ => null,
        };
    }

    private bool SetHoveredField(TimePickerField? field)
    {
        if (_hoveredField == field)
        {
            return false;
        }

        _hoveredField = field;
        return true;
    }
}

