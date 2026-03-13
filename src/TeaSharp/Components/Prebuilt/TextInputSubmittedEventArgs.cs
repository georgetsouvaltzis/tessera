using System.ComponentModel;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Describes a submitted text-input value.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TextInputSubmittedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new event payload for a submitted text-input value.
    /// </summary>
    /// <param name="value">The submitted text value.</param>
    public TextInputSubmittedEventArgs(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the submitted text value.
    /// </summary>
    public string Value { get; }
}
