using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Components.Styling;
using Tessera.Layout;
using Tessera.Styles;
using System.Globalization;

namespace Tessera.Controls;

/// <summary>
/// Represents a control for selecting a calendar date.
/// </summary>
public sealed class DatePicker : Control
{
    private readonly WidgetStatePalette _dayStatePalette = WidgetStatePalette.CreateDefault();
    private DateOnly? _hoveredDate;

    public event EventHandler<DateChangedEventArgs>? DateChanged;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Date Picker";

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

    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle MonthHeaderStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle WeekdayHeaderStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle DayStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle SelectedDayStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle HoveredDayStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    public TesseraStyle DisabledDayStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

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

    public DateOnly SelectedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    public DateOnly CurrentMonth { get; private set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

    public DateOnly? LastCommittedDate { get; private set; }

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

    public void SetDate(DateOnly date)
    {
        var previousDate = SelectedDate;
        SelectedDate = date;
        CurrentMonth = new DateOnly(date.Year, date.Month, 1);
        if (previousDate != SelectedDate)
        {
            DateChanged?.Invoke(this, new DateChangedEventArgs(previousDate, SelectedDate));
        }
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            SetDate(SelectedDate.AddDays(-1));
            return true;
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            SetDate(SelectedDate.AddDays(1));
            return true;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            SetDate(SelectedDate.AddDays(-7));
            return true;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            SetDate(SelectedDate.AddDays(7));
            return true;
        }

        if (key.Is(Key.PageUp))
        {
            SetDate(SelectedDate.AddMonths(-1));
            return true;
        }

        if (key.Is(Key.PageDown))
        {
            SetDate(SelectedDate.AddMonths(1));
            return true;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            LastCommittedDate = SelectedDate;
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

        var content = DatePickerCalendar.ResolveContentRect(bounds, Border, Padding);
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
                changed |= SetHoveredDate(null);
            }

            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelUp)
            {
                SetDate(SelectedDate.AddMonths(-1));
                changed = true;
            }
            else if (pointer.Button == PointerButton.WheelDown)
            {
                SetDate(SelectedDate.AddMonths(1));
                changed = true;
            }
        }

        if (!DatePickerCalendar.TryGetDateAtPointer(CurrentMonth, content, pointer.X, pointer.Y, out var hovered))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredDate(null);
            }

            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            changed |= SetHoveredDate(hovered);
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Press)
        {
            changed |= SetHoveredDate(hovered);
            if (pointer.Button == PointerButton.Left && hovered != SelectedDate)
            {
                SetDate(hovered);
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
        if (content.IsEmpty || content.Height < 3)
        {
            return;
        }

        var monthText = $"{CurrentMonth:yyyy-MM}";
        if (!MonthHeaderStyle.IsEmpty)
        {
            monthText = MonthHeaderStyle.Render(monthText);
        }

        canvas.WriteText(content.X, content.Y, monthText, content.Width);
        if (content.Height == 1)
        {
            return;
        }

        var weekdays = "Mo Tu We Th Fr Sa Su";
        if (!WeekdayHeaderStyle.IsEmpty)
        {
            weekdays = WeekdayHeaderStyle.Render(weekdays);
        }

        canvas.WriteText(content.X, content.Y + 1, weekdays, content.Width);
        if (content.Height < 3)
        {
            return;
        }

        var first = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var day = 1;
        for (var row = 0; row < 6 && (content.Y + 2 + row) < content.Bottom; row++)
        {
            for (var col = 0; col < 7; col++)
            {
                var cell = row * 7 + col;
                if (cell < startOffset || day > daysInMonth)
                {
                    continue;
                }

                var x = content.X + (col * 3);
                if (x + 1 >= content.Right)
                {
                    continue;
                }

                var date = new DateOnly(CurrentMonth.Year, CurrentMonth.Month, day);
                var states = DatePickerStateResolver.ResolveDayStates(IsFocused, SelectedDate, _hoveredDate, date);
                var dayText = _dayStatePalette.Render(day.ToString(CultureInfo.InvariantCulture).PadLeft(2, ' '), states);
                var dayStyle = ResolveDayStyle(date);
                if (!dayStyle.IsEmpty)
                {
                    dayText = dayStyle.Render(dayText);
                }

                canvas.WriteText(
                    x,
                    content.Y + 2 + row,
                    dayText,
                    Math.Min(2, content.Right - x));
                day++;
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(20, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4) + Padding.Horizontal;
        var height = 8 + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetHoveredDate(DateOnly? date)
    {
        if (_hoveredDate == date)
        {
            return false;
        }

        _hoveredDate = date;
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

    private TesseraStyle ResolveDayStyle(DateOnly date)
    {
        TesseraStyle style;
        if (date == SelectedDate && !SelectedDayStyle.IsEmpty)
        {
            style = SelectedDayStyle;
        }
        else if (_hoveredDate.HasValue && _hoveredDate.Value == date && !HoveredDayStyle.IsEmpty)
        {
            style = HoveredDayStyle;
        }
        else
        {
            style = DayStyle;
        }

        if ((IsDisabled || IsReadOnly) && !DisabledDayStyle.IsEmpty)
        {
            style = style.IsEmpty
                ? DisabledDayStyle
                : style.Merge(DisabledDayStyle);
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
            style = style.Merge(DisabledDayStyle);
        }

        return style;
    }
}
