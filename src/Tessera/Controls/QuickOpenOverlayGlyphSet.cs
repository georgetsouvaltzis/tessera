namespace Tessera.Controls;

/// <summary>
///     Defines glyphs used by <see cref="QuickOpenOverlay" />.
/// </summary>
public readonly record struct QuickOpenOverlayGlyphSet
{
    /// <summary>
    ///     Initializes a glyph set with built-in defaults.
    /// </summary>
    public QuickOpenOverlayGlyphSet()
    {
        QueryPrompt = ">";
        NormalRowMarker = " ";
        SelectedRowMarker = ">";
        HoveredRowMarker = "▸";
        MatchMarker = "~";
        MarkerSeparator = " ";
    }

    /// <summary>
    ///     Initializes a glyph set.
    /// </summary>
    /// <param name="queryPrompt">Prompt shown before query text.</param>
    /// <param name="normalRowMarker">Marker shown for non-hovered and non-selected rows.</param>
    /// <param name="selectedRowMarker">Marker shown for selected rows.</param>
    /// <param name="hoveredRowMarker">Marker shown for hovered rows.</param>
    /// <param name="matchMarker">Marker shown when the current query is non-empty.</param>
    /// <param name="markerSeparator">Separator placed between marker segments and row text.</param>
    public QuickOpenOverlayGlyphSet(
        string queryPrompt,
        string normalRowMarker,
        string selectedRowMarker,
        string hoveredRowMarker,
        string matchMarker,
        string markerSeparator)
    {
        QueryPrompt = queryPrompt;
        NormalRowMarker = normalRowMarker;
        SelectedRowMarker = selectedRowMarker;
        HoveredRowMarker = hoveredRowMarker;
        MatchMarker = matchMarker;
        MarkerSeparator = markerSeparator;
    }

    /// <summary>
    ///     Gets the built-in glyph set.
    /// </summary>
    public static QuickOpenOverlayGlyphSet Default => new();

    /// <summary>
    ///     Gets the prompt shown before query text.
    /// </summary>
    public string QueryPrompt { get; init; }

    /// <summary>
    ///     Gets the marker shown for non-hovered and non-selected rows.
    /// </summary>
    public string NormalRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker shown for selected rows.
    /// </summary>
    public string SelectedRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker shown for hovered rows.
    /// </summary>
    public string HoveredRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker shown when query matching is active.
    /// </summary>
    public string MatchMarker { get; init; }

    /// <summary>
    ///     Gets the separator placed between marker segments and row text.
    /// </summary>
    public string MarkerSeparator { get; init; }
}
