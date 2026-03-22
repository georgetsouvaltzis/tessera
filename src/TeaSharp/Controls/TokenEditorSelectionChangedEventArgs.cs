namespace TeaSharp.Controls;

/// <summary>
/// Provides token-selection transition data for <see cref="TokenEditor.SelectionChanged" />.
/// </summary>
public sealed class TokenEditorSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes selection change arguments.
    /// </summary>
    /// <param name="previousIndex">Previously selected token index, or <c>-1</c>.</param>
    /// <param name="selectedIndex">Current selected token index, or <c>-1</c>.</param>
    /// <param name="previousToken">Previously selected token, if any.</param>
    /// <param name="selectedToken">Current selected token, if any.</param>
    public TokenEditorSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        TokenItem? previousToken,
        TokenItem? selectedToken)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousToken = previousToken;
        SelectedToken = selectedToken;
    }

    /// <summary>
    /// Gets previously selected token index, or <c>-1</c> when none.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets current selected token index, or <c>-1</c> when none.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets previously selected token, if any.
    /// </summary>
    public TokenItem? PreviousToken { get; }

    /// <summary>
    /// Gets current selected token, if any.
    /// </summary>
    public TokenItem? SelectedToken { get; }
}
