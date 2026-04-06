namespace Tessera.Controls;

/// <summary>
/// Defines glyphs used by <see cref="SideNavRail" />.
/// </summary>
public readonly record struct SideNavRailGlyphSet
{
    /// <summary>
    /// Gets the default glyph set.
    /// </summary>
    public static SideNavRailGlyphSet Default => new();

    /// <summary>
    /// Initializes a glyph set with built-in defaults.
    /// </summary>
    public SideNavRailGlyphSet()
    {
        ExpandedMarker = "▼";
        CollapsedMarker = "▶";
        NormalItemMarker = " ";
        HoveredItemMarker = "▸";
        SelectedItemMarker = "●";
        ItemMarkerSeparator = " ";
        BadgePrefix = "[";
        BadgeSuffix = "]";
        BadgeSeparator = " ";
    }

    /// <summary>
    /// Initializes a glyph set.
    /// </summary>
    /// <param name="expandedMarker">Marker used when the rail is expanded.</param>
    /// <param name="collapsedMarker">Marker used when the rail is collapsed.</param>
    /// <param name="normalItemMarker">Marker used for non-selected and non-hovered items.</param>
    /// <param name="hoveredItemMarker">Marker used for hovered items.</param>
    /// <param name="selectedItemMarker">Marker used for selected items.</param>
    /// <param name="itemMarkerSeparator">Separator placed after the item marker.</param>
    /// <param name="badgePrefix">Prefix used before badge text.</param>
    /// <param name="badgeSuffix">Suffix used after badge text.</param>
    /// <param name="badgeSeparator">Separator placed between label and badge.</param>
    public SideNavRailGlyphSet(
        string expandedMarker,
        string collapsedMarker,
        string normalItemMarker,
        string hoveredItemMarker,
        string selectedItemMarker,
        string itemMarkerSeparator,
        string badgePrefix,
        string badgeSuffix,
        string badgeSeparator)
    {
        ExpandedMarker = expandedMarker ?? string.Empty;
        CollapsedMarker = collapsedMarker ?? string.Empty;
        NormalItemMarker = normalItemMarker ?? string.Empty;
        HoveredItemMarker = hoveredItemMarker ?? string.Empty;
        SelectedItemMarker = selectedItemMarker ?? string.Empty;
        ItemMarkerSeparator = itemMarkerSeparator ?? string.Empty;
        BadgePrefix = badgePrefix ?? string.Empty;
        BadgeSuffix = badgeSuffix ?? string.Empty;
        BadgeSeparator = badgeSeparator ?? string.Empty;
    }

    /// <summary>
    /// Gets the marker used when the rail is expanded.
    /// </summary>
    public string ExpandedMarker { get; init; }

    /// <summary>
    /// Gets the marker used when the rail is collapsed.
    /// </summary>
    public string CollapsedMarker { get; init; }

    /// <summary>
    /// Gets the marker used for non-selected and non-hovered items.
    /// </summary>
    public string NormalItemMarker { get; init; }

    /// <summary>
    /// Gets the marker used for hovered items.
    /// </summary>
    public string HoveredItemMarker { get; init; }

    /// <summary>
    /// Gets the marker used for selected items.
    /// </summary>
    public string SelectedItemMarker { get; init; }

    /// <summary>
    /// Gets the separator placed between marker and label text.
    /// </summary>
    public string ItemMarkerSeparator { get; init; }

    /// <summary>
    /// Gets the prefix used before badge text.
    /// </summary>
    public string BadgePrefix { get; init; }

    /// <summary>
    /// Gets the suffix used after badge text.
    /// </summary>
    public string BadgeSuffix { get; init; }

    /// <summary>
    /// Gets the separator placed between label and badge text.
    /// </summary>
    public string BadgeSeparator { get; init; }
}
