namespace TeaSharp.Components.Productivity;

/// <summary>
/// Describes a date selection transition.
/// </summary>
public sealed class DateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new date-change payload.
    /// </summary>
    /// <param name="previousDate">The previous selected date.</param>
    /// <param name="selectedDate">The current selected date.</param>
    public DateChangedEventArgs(DateOnly previousDate, DateOnly selectedDate)
    {
        PreviousDate = previousDate;
        SelectedDate = selectedDate;
    }

    /// <summary>
    /// Gets the previous selected date.
    /// </summary>
    public DateOnly PreviousDate { get; }

    /// <summary>
    /// Gets the current selected date.
    /// </summary>
    public DateOnly SelectedDate { get; }
}
