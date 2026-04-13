namespace Tessera.Controls;

/// <summary>
///     Provides the last text value when a text input cancels editing.
/// </summary>
public sealed class TextInputCancelledEventArgs : EventArgs
{
    /// <summary>
    ///     Executes text input cancelled event args.
    /// </summary>
    /// <param name="value">The value value.</param>
    /// <returns>The result of text input cancelled event args.</returns>
    public TextInputCancelledEventArgs(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public string Value { get; }
}
