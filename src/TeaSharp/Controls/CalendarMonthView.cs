using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a month-grid calendar control for planning workflows.
/// </summary>
public sealed class CalendarMonthView : Control
{
    private const int CalendarColumns = 7;
    private const int CalendarRows = 6;
    private const int DefaultCellWidth = 4;
    private DateOnly? _hoveredDate;

    /// <summary>
    /// Occurs when the selected date changes.
    /// </summary>
    public event EventHandler<CalendarDateSelectedEventArgs>? DateSelected;

    /// <summary>
    /// Gets or sets control title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Calendar";

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker { get; set; } = true;
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle MonthHeaderStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle WeekdayHeaderStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle OutsideMonthDayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle TodayDayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SelectedDayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle HoveredDayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DisabledDayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;
    public Thickness Padding { get; set; }
    public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Monday;
    public bool ShowAdjacentMonthDays { get; set; } = true;
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
    public DateOnly Today { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    public DateOnly SelectedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    public DateOnly DisplayMonth { get; private set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    public override bool IsFocused { get; set; }
    public override bool IsDisabled { get; set; }
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Selects a date and updates the displayed month to match.
    /// </summary>
    /// <param name="date">Date to select.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise, <see langword="false" />.</returns>
    public bool SelectDate(DateOnly date)
    {
        if (!IsDateSelectable(date))
        {
            return false;
        }

        var previous = SelectedDate;
        SelectedDate = date;
        DisplayMonth = new DateOnly(date.Year, date.Month, 1);
        _hoveredDate = null;
        if (previous != date)
        {
            DateSelected?.Invoke(this, new CalendarDateSelectedEventArgs(previous, date));
        }

        return previous != date;
    }

    /// <summary>
    /// Sets the displayed month while keeping selected date unchanged.
    /// </summary>
    /// <param name="month">Any date within the target month.</param>
    public void SetDisplayedMonth(DateOnly month)
    {
        DisplayMonth = new DateOnly(month.Year, month.Month, 1);
    }

    /// <summary>
    /// Returns the 6x7 day-cell grid used for current rendering state.
    /// </summary>
    /// <returns>Calendar day cells in row-major order.</returns>
    public IReadOnlyList<CalendarDayCell> GetVisibleCells()
    {
        var result = new CalendarDayCell[CalendarColumns * CalendarRows];
        BuildCells(result);
        return result;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return SelectDate(SelectedDate.AddDays(-1));
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return SelectDate(SelectedDate.AddDays(1));
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SelectDate(SelectedDate.AddDays(-7));
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SelectDate(SelectedDate.AddDays(7));
        }

        if (key.Is(Key.PageUp))
        {
            return SelectDate(SelectedDate.AddMonths(-1));
        }

        if (key.Is(Key.PageDown))
        {
            return SelectDate(SelectedDate.AddMonths(1));
        }

        if (key.Is(Key.Home))
        {
            return SelectDate(new DateOnly(DisplayMonth.Year, DisplayMonth.Month, 1));
        }

        if (key.Is(Key.End))
        {
            var last = DateTime.DaysInMonth(DisplayMonth.Year, DisplayMonth.Month);
            return SelectDate(new DateOnly(DisplayMonth.Year, DisplayMonth.Month, last));
        }

        return key.Is(Key.Enter) || key.IsCharacter(' ');
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = bounds.Inset(Padding);
        if (!TryResolveGridLayout(content, out var layout))
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelUp)
            {
                SetDisplayedMonth(DisplayMonth.AddMonths(-1));
                return true;
            }

            if (pointer.Button == PointerButton.WheelDown)
            {
                SetDisplayedMonth(DisplayMonth.AddMonths(1));
                return true;
            }
        }

        if (!TryGetDateAtPointer(layout, pointer.X, pointer.Y, out var date))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                var changed = _hoveredDate.HasValue;
                _hoveredDate = null;
                return changed || Handle(message);
            }

            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            var changed = _hoveredDate != date;
            _hoveredDate = date;
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            _hoveredDate = date;
            return SelectDate(date);
        }

        return Handle(message);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = clipped.Inset(Padding);
        if (!TryResolveGridLayout(content, out var layout))
        {
            return;
        }

        RenderHeaderRow(canvas, layout);
        RenderWeekdayHeaders(canvas, layout);
        RenderDayCells(canvas, layout);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var monthWidth = "September 2099".Length;
        var titleWidth = ControlTextLayout.MeasureDisplayWidth(FormatTitle());
        var width = Math.Max(CalendarColumns * DefaultCellWidth, titleWidth + monthWidth + 2);
        var height = 2 + CalendarRows;

        width += Padding.Horizontal;
        height += Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderHeaderRow(Canvas canvas, CalendarGridLayout layout)
    {
        var titleText = FormatTitle();
        var titleStyle = ResolveStyle(IsFocused ? FocusedTitleStyle : TitleStyle);
        canvas.WriteText(layout.Content.X, layout.TopY, ApplyStyle(titleText, titleStyle), layout.Content.Width);

        var monthText = DisplayMonth.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        var monthX = Math.Max(layout.Content.X, layout.Content.Right - monthText.Length);
        canvas.WriteText(monthX, layout.TopY, ApplyStyle(monthText, ResolveStyle(MonthHeaderStyle)), layout.Content.Right - monthX);
    }

    private void RenderWeekdayHeaders(Canvas canvas, CalendarGridLayout layout)
    {
        for (var column = 0; column < CalendarColumns; column++)
        {
            var dayOfWeek = (DayOfWeek)(((int)FirstDayOfWeek + column) % CalendarColumns);
            var label = GetDayLabel(dayOfWeek);
            var x = layout.GridX + (column * layout.CellWidth);
            var text = layout.CellWidth <= 2
                ? label[..1]
                : label.PadRight(layout.CellWidth, ' ');
            canvas.WriteText(x, layout.WeekdayY, ApplyStyle(text, ResolveStyle(WeekdayHeaderStyle)), layout.CellWidth);
        }
    }

    private void RenderDayCells(Canvas canvas, CalendarGridLayout layout)
    {
        Span<CalendarDayCell> cells = stackalloc CalendarDayCell[CalendarColumns * CalendarRows];
        BuildCells(cells);

        for (var row = 0; row < CalendarRows; row++)
        {
            for (var column = 0; column < CalendarColumns; column++)
            {
                var index = (row * CalendarColumns) + column;
                var cell = cells[index];
                var x = layout.GridX + (column * layout.CellWidth);
                var y = layout.GridStartY + row;
                var hover = _hoveredDate.HasValue && _hoveredDate.Value == cell.Date;
                var text = ResolveCellText(cell, hover, layout.CellWidth);
                var style = ResolveCellStyle(cell, hover);
                canvas.WriteText(x, y, ApplyStyle(text, style), layout.CellWidth);
            }
        }
    }

    private bool TryResolveGridLayout(Rect content, out CalendarGridLayout layout)
    {
        if (content.Width < CalendarColumns * 2 || content.Height < 2 + CalendarRows)
        {
            layout = default;
            return false;
        }

        var cellWidth = Math.Max(2, Math.Min(DefaultCellWidth, content.Width / CalendarColumns));
        var gridX = content.X;
        layout = new CalendarGridLayout(content, gridX, cellWidth, content.Y, content.Y + 1, content.Y + 2);
        return true;
    }

    private bool TryGetDateAtPointer(in CalendarGridLayout layout, int x, int y, out DateOnly date)
    {
        date = default;
        var row = y - layout.GridStartY;
        if (row is < 0 or >= CalendarRows)
        {
            return false;
        }

        var relativeX = x - layout.GridX;
        if (relativeX < 0)
        {
            return false;
        }

        var column = relativeX / layout.CellWidth;
        if (column is < 0 or >= CalendarColumns)
        {
            return false;
        }

        var first = new DateOnly(DisplayMonth.Year, DisplayMonth.Month, 1);
        var offset = GetDayOffset(first.DayOfWeek, FirstDayOfWeek);
        var start = first.AddDays(-offset);
        date = start.AddDays((row * CalendarColumns) + column);
        return true;
    }

    private void BuildCells(Span<CalendarDayCell> cells)
    {
        var first = new DateOnly(DisplayMonth.Year, DisplayMonth.Month, 1);
        var offset = GetDayOffset(first.DayOfWeek, FirstDayOfWeek);
        var start = first.AddDays(-offset);
        for (var i = 0; i < cells.Length; i++)
        {
            var date = start.AddDays(i);
            var disabled = !IsDateSelectable(date);
            cells[i] = new CalendarDayCell(
                date,
                date.Month == DisplayMonth.Month && date.Year == DisplayMonth.Year,
                date == Today,
                date == SelectedDate,
                disabled);
        }
    }

    private TeaStyle ResolveCellStyle(in CalendarDayCell cell, bool hovered)
    {
        var style = cell.IsCurrentMonth ? DayStyle : OutsideMonthDayStyle;
        if (cell.IsToday)
        {
            style = style.Merge(TodayDayStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredDayStyle);
        }

        if (cell.IsSelected)
        {
            style = style.Merge(SelectedDayStyle);
        }

        if (cell.IsDisabled)
        {
            style = style.Merge(DisabledDayStyle);
        }

        return ResolveStyle(style);
    }

    private string ResolveCellText(in CalendarDayCell cell, bool hovered, int width)
    {
        if (!ShowAdjacentMonthDays && !cell.IsCurrentMonth)
        {
            return string.Empty.PadRight(width, ' ');
        }

        var day = cell.Date.Day.ToString("00", CultureInfo.InvariantCulture);
        var text = day;
        if (cell.IsSelected && width >= 4)
        {
            text = $"[{day}]";
        }
        else if (hovered && width >= 4)
        {
            text = $"({day})";
        }

        if (text.Length > width)
        {
            return text[..width];
        }

        return text.PadLeft(Math.Min(text.Length + 1, width), ' ').PadRight(width, ' ');
    }

    private bool IsDateSelectable(DateOnly date)
    {
        if (MinDate.HasValue && date < MinDate.Value)
        {
            return false;
        }

        if (MaxDate.HasValue && date > MaxDate.Value)
        {
            return false;
        }

        return true;
    }

    private string FormatTitle()
    {
        if (!IsFocused || !ShowFocusMarker || string.IsNullOrWhiteSpace(FocusMarker) || string.IsNullOrEmpty(Title))
        {
            return Title;
        }

        return $"{Title} {FocusMarker}";
    }

    private TeaStyle ResolveStyle(TeaStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }

    private static int GetDayOffset(DayOfWeek day, DayOfWeek firstDayOfWeek)
    {
        var dayIndex = (int)day;
        var firstIndex = (int)firstDayOfWeek;
        var offset = dayIndex - firstIndex;
        return offset < 0 ? offset + CalendarColumns : offset;
    }

    private static string GetDayLabel(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "Mo",
            DayOfWeek.Tuesday => "Tu",
            DayOfWeek.Wednesday => "We",
            DayOfWeek.Thursday => "Th",
            DayOfWeek.Friday => "Fr",
            DayOfWeek.Saturday => "Sa",
            _ => "Su",
        };
    }

    private readonly record struct CalendarGridLayout(
        Rect Content,
        int GridX,
        int CellWidth,
        int TopY,
        int WeekdayY,
        int GridStartY);
}
