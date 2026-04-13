namespace Tessera.Controls;

/// <summary>
///     Provides the submitted numeric value from a <see cref="NumberInput" />.
/// </summary>
public sealed class NumberInputSubmittedEventArgs : EventArgs
{
    /// <summary>
    ///     Executes number input submitted event args.
    /// </summary>
    /// <param name="value">The value value.</param>
    /// <returns>The result of number input submitted event args.</returns>
    public NumberInputSubmittedEventArgs(double value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public double Value { get; }
}
