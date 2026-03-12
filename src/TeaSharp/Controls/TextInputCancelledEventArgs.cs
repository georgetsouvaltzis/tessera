namespace TeaSharp.Controls;

public sealed class TextInputCancelledEventArgs : EventArgs
{
    public TextInputCancelledEventArgs(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }
}
