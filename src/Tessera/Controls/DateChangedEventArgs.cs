namespace Tessera.Controls;

/// <summary>
/// Provides the newly selected date value.
/// </summary>
public sealed class DateChangedEventArgs : EventArgs
{
    public DateChangedEventArgs(DateOnly previousDate, DateOnly selectedDate)
    {
        PreviousDate = previousDate;
        SelectedDate = selectedDate;
    }

    public DateOnly PreviousDate { get; }

    public DateOnly SelectedDate { get; }
}
