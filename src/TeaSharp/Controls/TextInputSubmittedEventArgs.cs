namespace TeaSharp.Controls;

public sealed class TextInputSubmittedEventArgs : EventArgs
{
    public TextInputSubmittedEventArgs(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }
}
