namespace TeaSharp.Controls;

/// <summary>
/// Defines row markers used by <see cref="ListView{T}"/> during rendering.
/// </summary>
public readonly record struct ListViewMarkerSet
{
    /// <summary>
    /// Gets the default marker set used by list views.
    /// </summary>
    public static ListViewMarkerSet Default => new();

    /// <summary>
    /// Initializes a new marker set with the built-in list markers.
    /// </summary>
    public ListViewMarkerSet()
    {
        DefaultRowMarker = " ";
        HoveredRowMarker = "▸";
        SelectedRowMarker = "›";
    }

    /// <summary>
    /// Initializes a new marker set.
    /// </summary>
    /// <param name="defaultRowMarker">Marker used for rows that are neither hovered nor selected.</param>
    /// <param name="hoveredRowMarker">Marker used for hovered rows.</param>
    /// <param name="selectedRowMarker">Marker used for selected rows.</param>
    public ListViewMarkerSet(string defaultRowMarker, string hoveredRowMarker, string selectedRowMarker)
    {
        DefaultRowMarker = defaultRowMarker ?? string.Empty;
        HoveredRowMarker = hoveredRowMarker ?? string.Empty;
        SelectedRowMarker = selectedRowMarker ?? string.Empty;
    }

    /// <summary>
    /// Gets the marker used for rows that are neither hovered nor selected.
    /// </summary>
    public string DefaultRowMarker { get; init; }

    /// <summary>
    /// Gets the marker used for hovered rows.
    /// </summary>
    public string HoveredRowMarker { get; init; }

    /// <summary>
    /// Gets the marker used for selected rows.
    /// </summary>
    public string SelectedRowMarker { get; init; }
}
