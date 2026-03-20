namespace TeaSharp.Controls;

/// <summary>
/// Defines glyphs used by <see cref="SearchResultsView"/>.
/// </summary>
public readonly record struct SearchResultsGlyphSet
{
    /// <summary>
    /// Gets the default glyph set used by <see cref="SearchResultsView"/>.
    /// </summary>
    public static SearchResultsGlyphSet Default => new();

    /// <summary>
    /// Initializes a new glyph set with default markers.
    /// </summary>
    public SearchResultsGlyphSet()
    {
        DefaultRowMarker = " ";
        HoveredRowMarker = "▸";
        SelectedRowMarker = "▶";
        MatchMarker = "•";
        RankSeparator = ".";
    }

    /// <summary>
    /// Initializes a new glyph set.
    /// </summary>
    /// <param name="defaultRowMarker">Marker shown for normal rows.</param>
    /// <param name="hoveredRowMarker">Marker shown for hovered rows.</param>
    /// <param name="selectedRowMarker">Marker shown for selected rows.</param>
    /// <param name="matchMarker">Marker shown for rows matching the current query.</param>
    /// <param name="rankSeparator">Separator between row rank and content.</param>
    public SearchResultsGlyphSet(
        string defaultRowMarker,
        string hoveredRowMarker,
        string selectedRowMarker,
        string matchMarker,
        string rankSeparator)
    {
        DefaultRowMarker = defaultRowMarker ?? string.Empty;
        HoveredRowMarker = hoveredRowMarker ?? string.Empty;
        SelectedRowMarker = selectedRowMarker ?? string.Empty;
        MatchMarker = matchMarker ?? string.Empty;
        RankSeparator = rankSeparator ?? string.Empty;
    }

    /// <summary>
    /// Gets the marker shown for normal rows.
    /// </summary>
    public string DefaultRowMarker { get; init; }

    /// <summary>
    /// Gets the marker shown for hovered rows.
    /// </summary>
    public string HoveredRowMarker { get; init; }

    /// <summary>
    /// Gets the marker shown for selected rows.
    /// </summary>
    public string SelectedRowMarker { get; init; }

    /// <summary>
    /// Gets the marker shown for rows matching the active query.
    /// </summary>
    public string MatchMarker { get; init; }

    /// <summary>
    /// Gets the separator between row rank and content.
    /// </summary>
    public string RankSeparator { get; init; }
}
