namespace Tessera.Controls;

/// <summary>
/// Provides the submitted numeric value from a <see cref="NumberInput"/>.
/// </summary>
public sealed class NumberInputSubmittedEventArgs : EventArgs
{
    public NumberInputSubmittedEventArgs(double value)
    {
        Value = value;
    }

    public double Value { get; }
}
