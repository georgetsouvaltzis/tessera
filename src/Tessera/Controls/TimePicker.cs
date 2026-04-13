using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Components.Styling;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a control for editing a time-of-day value.
/// </summary>
public sealed class TimePicker : Control
{
    private readonly WidgetStatePalette _fieldStatePalette = WidgetStatePalette.CreateDefault();
    private TimeField? _hoveredField;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Time Picker";

    /// <summary>
    ///     Represents focus marker.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Represents show focus marker.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents value text style.
    /// </summary>
    public TesseraStyle ValueTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents active field style.
    /// </summary>
    public TesseraStyle ActiveFieldStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents hovered field style.
    /// </summary>
    public TesseraStyle HoveredFieldStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents disabled value style.
    /// </summary>
    public TesseraStyle DisabledValueStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents separator style.
    /// </summary>
    public TesseraStyle SeparatorStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    ///     Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    public TimeOnly Value { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    ///     Gets or sets the last committed time.
    /// </summary>
    public TimeOnly? LastCommittedTime { get; private set; }

    /// <summary>
    ///     Gets or sets the active field.
    /// </summary>
    public TimeField ActiveField { get; private set; }

    /// <summary>
    ///     Represents hour step.
    /// </summary>
    public int HourStep
    {
        get;
        set;
    } = 1;

    /// <summary>
    ///     Represents minute step.
    /// </summary>
    public int MinuteStep
    {
        get;
        set;
    } = 1;

    /// <summary>
    ///     Represents second step.
    /// </summary>
    public int SecondStep
    {
        get;
        set;
    } = 5;

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents value changed.
    /// </summary>
    public event EventHandler<TimeValueChangedEventArgs>? ValueChanged;

    /// <summary>
    ///     Executes set value.
    /// </summary>
    /// <param name="time">The time value.</param>
    public void SetValue(TimeOnly time)
    {
        var previousValue = Value;
        Value = time;
        if (previousValue != Value)
        {
            ValueChanged?.Invoke(this, new TimeValueChangedEventArgs(previousValue, Value));
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
        var width = Math.Max(8, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4) +
                    Padding.Horizontal;
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
        var states =
            TimePickerStateResolver.ResolveFieldStates(IsFocused, IsDisabled, IsReadOnly, ActiveField, _hoveredField,
                field);
        var rendered = _fieldStatePalette.Render(value, states);
        var style = ResolveFieldStyle(field);
        if (!style.IsEmpty)
        {
            rendered = style.Render(rendered);
        }

        return rendered;
    }

    private TesseraStyle ResolveFieldStyle(TimeField field)
    {
        TesseraStyle style;
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

    private TesseraStyle ResolveBorderStyleText()
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
