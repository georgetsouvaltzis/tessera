namespace Tessera.Controls;

/// <summary>
/// Provides the newly selected time value.
/// </summary>
public sealed class TimeValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Executes time value changed event args.
    /// </summary>
    /// <param name="previousValue">The previous value value.</param>
    /// <param name="value">The value value.</param>
    /// <returns>The result of time value changed event args.</returns>
    public TimeValueChangedEventArgs(TimeOnly previousValue, TimeOnly value)
    {
        PreviousValue = previousValue;
        Value = value;
    }

    /// <summary>
    /// Gets the previous value.
    /// </summary>
    public TimeOnly PreviousValue { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public TimeOnly Value { get; }
}
