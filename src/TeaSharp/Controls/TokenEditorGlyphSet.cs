namespace TeaSharp.Controls;

/// <summary>
/// Defines glyphs used by <see cref="TokenEditor" />.
/// </summary>
public readonly record struct TokenEditorGlyphSet
{
    /// <summary>
    /// Gets the built-in glyph set.
    /// </summary>
    public static TokenEditorGlyphSet Default => new();

    /// <summary>
    /// Initializes a glyph set with built-in defaults.
    /// </summary>
    public TokenEditorGlyphSet()
    {
        SelectedMarker = "●";
        UnselectedMarker = "○";
        TokenPrefix = "[";
        TokenSuffix = "]";
        MarkerSeparator = " ";
        TokenSeparator = " ";
    }

    /// <summary>
    /// Initializes a glyph set.
    /// </summary>
    /// <param name="selectedMarker">Marker rendered for selected chips.</param>
    /// <param name="unselectedMarker">Marker rendered for non-selected chips.</param>
    /// <param name="tokenPrefix">Prefix rendered before token value.</param>
    /// <param name="tokenSuffix">Suffix rendered after token value.</param>
    /// <param name="markerSeparator">Separator between marker and chip content.</param>
    /// <param name="tokenSeparator">Separator between chips.</param>
    public TokenEditorGlyphSet(
        string selectedMarker,
        string unselectedMarker,
        string tokenPrefix,
        string tokenSuffix,
        string markerSeparator,
        string tokenSeparator)
    {
        SelectedMarker = selectedMarker ?? string.Empty;
        UnselectedMarker = unselectedMarker ?? string.Empty;
        TokenPrefix = tokenPrefix ?? string.Empty;
        TokenSuffix = tokenSuffix ?? string.Empty;
        MarkerSeparator = markerSeparator ?? string.Empty;
        TokenSeparator = tokenSeparator ?? string.Empty;
    }

    /// <summary>
    /// Gets the marker rendered for selected chips.
    /// </summary>
    public string SelectedMarker { get; init; }

    /// <summary>
    /// Gets the marker rendered for non-selected chips.
    /// </summary>
    public string UnselectedMarker { get; init; }

    /// <summary>
    /// Gets the prefix rendered before token value.
    /// </summary>
    public string TokenPrefix { get; init; }

    /// <summary>
    /// Gets the suffix rendered after token value.
    /// </summary>
    public string TokenSuffix { get; init; }

    /// <summary>
    /// Gets the separator rendered between marker and chip content.
    /// </summary>
    public string MarkerSeparator { get; init; }

    /// <summary>
    /// Gets the separator rendered between chips.
    /// </summary>
    public string TokenSeparator { get; init; }
}
