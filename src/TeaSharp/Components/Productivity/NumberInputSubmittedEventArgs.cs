namespace TeaSharp.Components.Productivity;

/// <summary>
/// Describes a submitted numeric value.
/// </summary>
public sealed class NumberInputSubmittedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new event payload for a submitted numeric value.
    /// </summary>
    /// <param name="value">The submitted numeric value.</param>
    public NumberInputSubmittedEventArgs(double value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the submitted numeric value.
    /// </summary>
    public double Value { get; }
}
