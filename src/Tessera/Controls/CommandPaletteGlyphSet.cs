namespace Tessera.Controls;

/// <summary>
/// Defines glyphs used by <see cref="CommandPalette"/> for query and result row rendering.
/// </summary>
public readonly record struct CommandPaletteGlyphSet
{
    /// <summary>
    /// Gets the default glyph set used by command palettes.
    /// </summary>
    public static CommandPaletteGlyphSet Default => new();

    /// <summary>
    /// Initializes a new glyph set with built-in command palette markers.
    /// </summary>
    public CommandPaletteGlyphSet()
    {
        QueryPrompt = ">";
        NormalRowMarker = " ";
        SelectedRowMarker = ">";
        HoveredRowMarker = "▸";
        MarkerSeparator = " ";
    }

    /// <summary>
    /// Initializes a new glyph set.
    /// </summary>
    /// <param name="queryPrompt">Prompt shown before the query input value.</param>
    /// <param name="normalRowMarker">Marker shown for unselected rows.</param>
    /// <param name="selectedRowMarker">Marker shown for selected rows.</param>
    /// <param name="hoveredRowMarker">Marker shown for hovered rows.</param>
    /// <param name="markerSeparator">Separator between markers and row text.</param>
    public CommandPaletteGlyphSet(
        string queryPrompt,
        string normalRowMarker,
        string selectedRowMarker,
        string hoveredRowMarker,
        string markerSeparator)
    {
        QueryPrompt = queryPrompt ?? string.Empty;
        NormalRowMarker = normalRowMarker ?? string.Empty;
        SelectedRowMarker = selectedRowMarker ?? string.Empty;
        HoveredRowMarker = hoveredRowMarker ?? string.Empty;
        MarkerSeparator = markerSeparator ?? string.Empty;
    }

    /// <summary>
    /// Gets the prompt shown before the query input value.
    /// </summary>
    public string QueryPrompt { get; init; }

    /// <summary>
    /// Gets the marker shown for unselected rows.
    /// </summary>
    public string NormalRowMarker { get; init; }

    /// <summary>
    /// Gets the marker shown for selected rows.
    /// </summary>
    public string SelectedRowMarker { get; init; }

    /// <summary>
    /// Gets the marker shown for hovered rows.
    /// </summary>
    public string HoveredRowMarker { get; init; }

    /// <summary>
    /// Gets the separator between markers and row text.
    /// </summary>
    public string MarkerSeparator { get; init; }
}
