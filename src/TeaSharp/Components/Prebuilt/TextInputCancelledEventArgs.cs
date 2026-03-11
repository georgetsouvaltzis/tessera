namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Describes a cancelled text-input value.
/// </summary>
public sealed class TextInputCancelledEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new event payload for a cancelled text-input value.
    /// </summary>
    /// <param name="value">The cancelled text value.</param>
    public TextInputCancelledEventArgs(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the cancelled text value.
    /// </summary>
    public string Value { get; }
}
