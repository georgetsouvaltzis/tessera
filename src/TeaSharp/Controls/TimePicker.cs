using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a control for editing a time-of-day value.
/// </summary>
public sealed class TimePicker : Control
{
    private readonly WidgetStatePalette _fieldStatePalette = WidgetStatePalette.CreateDefault();
    private TimeField? _hoveredField;

    public event EventHandler<TimeValueChangedEventArgs>? ValueChanged;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Time Picker";

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

    public TimeOnly Value { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    public TimeOnly? LastCommittedTime { get; private set; }

    public TimeField ActiveField { get; private set; }

    public int HourStep
    {
        get;
        set;
    } = 1;

    public int MinuteStep
    {
        get;
        set;
    } = 1;

    public int SecondStep
    {
        get;
        set;
    } = 5;

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

    public void SetValue(TimeOnly time)
    {
        var previousValue = Value;
        Value = time;
        if (previousValue != Value)
        {
            ValueChanged?.Invoke(this, new TimeValueChangedEventArgs(previousValue, Value));
        }
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            ActiveField = TimePickerFields.Next(ActiveField);
            return true;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            ActiveField = TimePickerFields.Previous(ActiveField);
            return true;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            SetValue(TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, 1));
            return true;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            SetValue(TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, -1));
            return true;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            LastCommittedTime = Value;
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredField(null);
            }

            return changed || Handle(message);
        }

        var field = TimePickerFields.FieldFromPointer(content, pointer.X, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            changed |= SetHoveredField(field);
            return changed;
        }

        if (pointer.Kind is PointerEventKind.Press or PointerEventKind.Release)
        {
            changed |= SetHoveredField(field);
            if (pointer.Button == PointerButton.Left && field.HasValue && ActiveField != field.Value)
            {
                ActiveField = field.Value;
                changed = true;
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (field.HasValue && ActiveField != field.Value)
            {
                ActiveField = field.Value;
                changed = true;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                SetValue(TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, 1));
                changed = true;
            }
            else if (pointer.Button == PointerButton.WheelDown)
            {
                SetValue(TimePickerFields.Adjust(Value, ActiveField, HourStep, MinuteStep, SecondStep, -1));
                changed = true;
            }
        }

        return changed || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        TimePickerRenderer.Render(canvas, rect, Title, IsFocused, IsDisabled, IsReadOnly, Border, Padding, Value, ActiveField, _hoveredField, _fieldStatePalette);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, Title.Length + 4) + Padding.Horizontal;
        var height = Padding.Vertical + 1;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetHoveredField(TimeField? field)
    {
        if (_hoveredField == field)
        {
            return false;
        }

        _hoveredField = field;
        return true;
    }
}
