namespace Tessera.Controls;

/// <summary>
/// Provides suggestion commit details for <see cref="AutocompleteInput.SuggestionCommitted" />.
/// </summary>
public sealed class AutocompleteInputSuggestionCommittedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes event args.
    /// </summary>
    /// <param name="text">Committed suggestion text.</param>
    /// <param name="suggestionIndex">Committed suggestion index in the configured suggestion source list.</param>
    /// <param name="previousText">Text before commit.</param>
    public AutocompleteInputSuggestionCommittedEventArgs(string text, int suggestionIndex, string previousText)
    {
        Text = text ?? string.Empty;
        SuggestionIndex = suggestionIndex;
        PreviousText = previousText ?? string.Empty;
    }

    /// <summary>
    /// Gets committed suggestion text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets committed suggestion index in the configured suggestion source list.
    /// </summary>
    public int SuggestionIndex { get; }

    /// <summary>
    /// Gets the previous input text before commit.
    /// </summary>
    public string PreviousText { get; }
}
