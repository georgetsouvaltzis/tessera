namespace TeaSharp.Controls;

public sealed class TimeValueChangedEventArgs : EventArgs
{
    public TimeValueChangedEventArgs(TimeOnly previousValue, TimeOnly value)
    {
        PreviousValue = previousValue;
        Value = value;
    }

    public TimeOnly PreviousValue { get; }

    public TimeOnly Value { get; }
}
