namespace Tessera.Controls;

/// <summary>
/// Defines glyphs used by <see cref="AutocompleteInput" />.
/// </summary>
public readonly record struct AutocompleteInputGlyphSet
{
    /// <summary>
    /// Gets the default glyph set.
    /// </summary>
    public static AutocompleteInputGlyphSet Default => new();

    /// <summary>
    /// Initializes a glyph set with built-in defaults.
    /// </summary>
    public AutocompleteInputGlyphSet()
    {
        SuggestionMarker = ">";
        CommitMarker = "↵";
        MarkerSeparator = " ";
    }

    /// <summary>
    /// Initializes a glyph set.
    /// </summary>
    /// <param name="suggestionMarker">Marker shown for the selected suggestion row.</param>
    /// <param name="commitMarker">Marker shown as a commit hint while suggestions are visible.</param>
    /// <param name="markerSeparator">Separator placed after <paramref name="suggestionMarker" />.</param>
    public AutocompleteInputGlyphSet(string suggestionMarker, string commitMarker, string markerSeparator)
    {
        SuggestionMarker = suggestionMarker ?? string.Empty;
        CommitMarker = commitMarker ?? string.Empty;
        MarkerSeparator = markerSeparator ?? string.Empty;
    }

    /// <summary>
    /// Gets the marker shown for the selected suggestion row.
    /// </summary>
    public string SuggestionMarker { get; init; }

    /// <summary>
    /// Gets the marker shown as a commit hint while suggestions are visible.
    /// </summary>
    public string CommitMarker { get; init; }

    /// <summary>
    /// Gets the separator placed after <see cref="SuggestionMarker" />.
    /// </summary>
    public string MarkerSeparator { get; init; }
}
