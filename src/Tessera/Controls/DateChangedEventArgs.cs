namespace Tessera.Controls;

/// <summary>
/// Provides the newly selected date value.
/// </summary>
public sealed class DateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Executes date changed event args.
    /// </summary>
    /// <param name="previousDate">The previous date value.</param>
    /// <param name="selectedDate">The selected date value.</param>
    /// <returns>The result of date changed event args.</returns>
    public DateChangedEventArgs(DateOnly previousDate, DateOnly selectedDate)
    {
        PreviousDate = previousDate;
        SelectedDate = selectedDate;
    }

    /// <summary>
    /// Gets the previous date.
    /// </summary>
    public DateOnly PreviousDate { get; }

    /// <summary>
    /// Gets the selected date.
    /// </summary>
    public DateOnly SelectedDate { get; }
}
