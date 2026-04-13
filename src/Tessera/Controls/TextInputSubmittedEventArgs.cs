namespace Tessera.Controls;

/// <summary>
///     Provides the submitted text value from a text input control.
/// </summary>
public sealed class TextInputSubmittedEventArgs : EventArgs
{
    /// <summary>
    ///     Executes text input submitted event args.
    /// </summary>
    /// <param name="value">The value value.</param>
    /// <returns>The result of text input submitted event args.</returns>
    public TextInputSubmittedEventArgs(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public string Value { get; }
}
