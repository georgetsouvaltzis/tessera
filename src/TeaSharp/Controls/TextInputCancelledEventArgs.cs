namespace TeaSharp.Controls;

/// <summary>
/// Provides the last text value when a text input cancels editing.
/// </summary>
public sealed class TextInputCancelledEventArgs : EventArgs
{
    public TextInputCancelledEventArgs(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }
}
