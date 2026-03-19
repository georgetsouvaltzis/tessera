using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Layout;
using TeaSharp.Styles;
using System.Globalization;

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

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle ValueTextStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle ActiveFieldStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle HoveredFieldStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle DisabledValueStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle SeparatorStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

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
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : FormatTitle();
        if (!string.IsNullOrEmpty(title))
        {
            var titleStyle = IsFocused ? FocusedTitleStyle : TitleStyle;
            if (!titleStyle.IsEmpty)
            {
                title = titleStyle.Render(title);
            }
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var hour = RenderField(
            Value.Hour.ToString("D2", CultureInfo.InvariantCulture),
            TimeField.Hour);
        var minute = RenderField(
            Value.Minute.ToString("D2", CultureInfo.InvariantCulture),
            TimeField.Minute);
        var second = RenderField(
            Value.Second.ToString("D2", CultureInfo.InvariantCulture),
            TimeField.Second);

        var separator = ":";
        if (!SeparatorStyle.IsEmpty)
        {
            separator = SeparatorStyle.Render(separator);
        }

        canvas.WriteText(content.X, content.Y, $"{hour}{separator}{minute}{separator}{second}", content.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4) + Padding.Horizontal;
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

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderField(string value, TimeField field)
    {
        var states = TimePickerStateResolver.ResolveFieldStates(IsFocused, IsDisabled, IsReadOnly, ActiveField, _hoveredField, field);
        var rendered = _fieldStatePalette.Render(value, states);
        var style = ResolveFieldStyle(field);
        if (!style.IsEmpty)
        {
            rendered = style.Render(rendered);
        }

        return rendered;
    }

    private TeaStyle ResolveFieldStyle(TimeField field)
    {
        TeaStyle style;
        if (ActiveField == field && !ActiveFieldStyle.IsEmpty)
        {
            style = ActiveFieldStyle;
        }
        else if (_hoveredField.HasValue && _hoveredField.Value == field && !HoveredFieldStyle.IsEmpty)
        {
            style = HoveredFieldStyle;
        }
        else
        {
            style = ValueTextStyle;
        }

        if ((IsDisabled || IsReadOnly) && !DisabledValueStyle.IsEmpty)
        {
            style = style.IsEmpty
                ? DisabledValueStyle
                : style.Merge(DisabledValueStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled || IsReadOnly)
        {
            style = style.Merge(DisabledValueStyle);
        }

        return style;
    }
}
