namespace Tessera.Controls;

/// <summary>
///     Defines row markers used by <see cref="ContextMenu" /> during rendering.
/// </summary>
public readonly record struct ContextMenuGlyphSet
{
    /// <summary>
    ///     Initializes a new glyph set with built-in context-menu markers.
    /// </summary>
    public ContextMenuGlyphSet()
    {
        NormalRowMarker = " ";
        SelectedRowMarker = ">";
        HoveredRowMarker = "▸";
        MarkerSeparator = " ";
    }

    /// <summary>
    ///     Initializes a new glyph set.
    /// </summary>
    /// <param name="normalRowMarker">Marker used for rows that are neither selected nor hovered.</param>
    /// <param name="selectedRowMarker">Marker used for the selected row.</param>
    /// <param name="hoveredRowMarker">Marker used for the hovered row.</param>
    /// <param name="markerSeparator">Separator inserted between the marker and item title.</param>
    public ContextMenuGlyphSet(
        string normalRowMarker,
        string selectedRowMarker,
        string hoveredRowMarker,
        string markerSeparator)
    {
        NormalRowMarker = normalRowMarker;
        SelectedRowMarker = selectedRowMarker;
        HoveredRowMarker = hoveredRowMarker;
        MarkerSeparator = markerSeparator;
    }

    /// <summary>
    ///     Gets the default glyph set used by context menus.
    /// </summary>
    public static ContextMenuGlyphSet Default => new();

    /// <summary>
    ///     Gets the marker used for rows that are neither selected nor hovered.
    /// </summary>
    public string NormalRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker used for the selected row.
    /// </summary>
    public string SelectedRowMarker { get; init; }

    /// <summary>
    ///     Gets the marker used for the hovered row.
    /// </summary>
    public string HoveredRowMarker { get; init; }

    /// <summary>
    ///     Gets the separator inserted between the marker and item title.
    /// </summary>
    public string MarkerSeparator { get; init; }
}
