namespace Tessera.Controls;

/// <summary>
/// Defines branch and leaf markers used by <see cref="TreeView"/> during rendering.
/// </summary>
public readonly record struct TreeViewGlyphSet
{
    /// <summary>
    /// Gets the default glyph set used by tree views.
    /// </summary>
    public static TreeViewGlyphSet Default => new();

    /// <summary>
    /// Initializes a new glyph set with the built-in tree markers.
    /// </summary>
    public TreeViewGlyphSet()
    {
        ExpandedBranchMarker = "▾";
        CollapsedBranchMarker = "▸";
        LeafMarker = "•";
    }

    /// <summary>
    /// Initializes a new glyph set.
    /// </summary>
    /// <param name="expandedBranchMarker">Marker used for expanded branch nodes.</param>
    /// <param name="collapsedBranchMarker">Marker used for collapsed branch nodes.</param>
    /// <param name="leafMarker">Marker used for leaf nodes.</param>
    public TreeViewGlyphSet(string expandedBranchMarker, string collapsedBranchMarker, string leafMarker)
    {
        ExpandedBranchMarker = expandedBranchMarker ?? string.Empty;
        CollapsedBranchMarker = collapsedBranchMarker ?? string.Empty;
        LeafMarker = leafMarker ?? string.Empty;
    }

    /// <summary>
    /// Gets the marker used for expanded branch nodes.
    /// </summary>
    public string ExpandedBranchMarker { get; init; }

    /// <summary>
    /// Gets the marker used for collapsed branch nodes.
    /// </summary>
    public string CollapsedBranchMarker { get; init; }

    /// <summary>
    /// Gets the marker used for leaf nodes.
    /// </summary>
    public string LeafMarker { get; init; }
}
