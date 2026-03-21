namespace TeaSharp.Controls;

/// <summary>
/// Carries typed closure information for <see cref="Dialog.Closed"/>.
/// </summary>
public sealed class DialogClosedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes closure payload.
    /// </summary>
    /// <param name="result">Applied dialog result.</param>
    public DialogClosedEventArgs(DialogResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the applied dialog result.
    /// </summary>
    public DialogResult Result { get; }
}
