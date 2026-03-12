namespace TeaSharp.Controls;

public sealed class NumberInputSubmittedEventArgs : EventArgs
{
    public NumberInputSubmittedEventArgs(double value)
    {
        Value = value;
    }

    public double Value { get; }
}
