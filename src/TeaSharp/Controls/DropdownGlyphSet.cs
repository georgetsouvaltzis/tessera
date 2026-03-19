namespace TeaSharp.Controls;

/// <summary>
/// Defines glyphs used by drop-down controls such as <see cref="Choice"/> and <see cref="ComboBox"/>.
/// </summary>
public readonly record struct DropdownGlyphSet
{
    /// <summary>
    /// Gets the default glyph set used by drop-down controls.
    /// </summary>
    public static DropdownGlyphSet Default => new();

    /// <summary>
    /// Initializes a new glyph set with standard drop-down markers.
    /// </summary>
    public DropdownGlyphSet()
    {
        CollapsedIndicator = "▾";
        ExpandedIndicator = "▴";
        HighlightedOptionMarker = "▸";
        SelectedOptionMarker = "✓";
    }

    /// <summary>
    /// Initializes a new glyph set.
    /// </summary>
    /// <param name="collapsedIndicator">Indicator shown when the list is closed.</param>
    /// <param name="expandedIndicator">Indicator shown when the list is open.</param>
    /// <param name="highlightedOptionMarker">Marker shown for the highlighted option row.</param>
    /// <param name="selectedOptionMarker">Marker shown for the selected option row.</param>
    public DropdownGlyphSet(
        string collapsedIndicator,
        string expandedIndicator,
        string highlightedOptionMarker,
        string selectedOptionMarker)
    {
        CollapsedIndicator = collapsedIndicator ?? string.Empty;
        ExpandedIndicator = expandedIndicator ?? string.Empty;
        HighlightedOptionMarker = highlightedOptionMarker ?? string.Empty;
        SelectedOptionMarker = selectedOptionMarker ?? string.Empty;
    }

    /// <summary>
    /// Gets the indicator shown when the list is closed.
    /// </summary>
    public string CollapsedIndicator { get; init; }

    /// <summary>
    /// Gets the indicator shown when the list is open.
    /// </summary>
    public string ExpandedIndicator { get; init; }

    /// <summary>
    /// Gets the marker shown for the highlighted option row.
    /// </summary>
    public string HighlightedOptionMarker { get; init; }

    /// <summary>
    /// Gets the marker shown for the selected option row.
    /// </summary>
    public string SelectedOptionMarker { get; init; }
}
