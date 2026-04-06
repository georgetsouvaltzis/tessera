namespace Tessera.Controls;

/// <summary>
/// Provides the submitted text value from a text input control.
/// </summary>
public sealed class TextInputSubmittedEventArgs : EventArgs
{
    public TextInputSubmittedEventArgs(string value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }
}
