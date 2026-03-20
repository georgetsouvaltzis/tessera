namespace TeaSharp.Controls;

/// <summary>
/// Represents one rendered day cell in a <see cref="CalendarMonthView" /> grid.
/// </summary>
public readonly record struct CalendarDayCell(
    DateOnly Date,
    bool IsCurrentMonth,
    bool IsToday,
    bool IsSelected,
    bool IsDisabled);
