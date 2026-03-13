using System.ComponentModel;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Describes a time-value transition.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TimeValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new time-value change payload.
    /// </summary>
    /// <param name="previousValue">The previous selected time value.</param>
    /// <param name="value">The current selected time value.</param>
    public TimeValueChangedEventArgs(TimeOnly previousValue, TimeOnly value)
    {
        PreviousValue = previousValue;
        Value = value;
    }

    /// <summary>
    /// Gets the previous selected time value.
    /// </summary>
    public TimeOnly PreviousValue { get; }

    /// <summary>
    /// Gets the current selected time value.
    /// </summary>
    public TimeOnly Value { get; }
}
