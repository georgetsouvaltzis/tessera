namespace Tessera.Controls;

/// <summary>
/// Provides old/new state when <see cref="CalendarMonthView" /> selection changes.
/// </summary>
public sealed class CalendarDateSelectedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a calendar selection-change payload.
    /// </summary>
    /// <param name="previousDate">The selected date before the change.</param>
    /// <param name="selectedDate">The selected date after the change.</param>
    public CalendarDateSelectedEventArgs(DateOnly previousDate, DateOnly selectedDate)
    {
        PreviousDate = previousDate;
        SelectedDate = selectedDate;
    }

    /// <summary>
    /// Gets the selected date before the change.
    /// </summary>
    public DateOnly PreviousDate { get; }

    /// <summary>
    /// Gets the selected date after the change.
    /// </summary>
    public DateOnly SelectedDate { get; }
}
