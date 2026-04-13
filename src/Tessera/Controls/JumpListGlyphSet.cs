namespace Tessera.Controls;

/// <summary>
///     Defines glyphs used by <see cref="JumpList" />.
/// </summary>
public sealed record JumpListGlyphSet
{
    /// <summary>
    ///     Gets default jump-list glyphs.
    /// </summary>
    public static JumpListGlyphSet Default { get; } = new();

    /// <summary>
    ///     Gets or sets marker rendered before selected items.
    /// </summary>
    public string SelectedMarker { get; init; } = ">";

    /// <summary>
    ///     Gets or sets marker rendered before unselected items.
    /// </summary>
    public string UnselectedMarker { get; init; } = " ";

    /// <summary>
    ///     Gets or sets pinned marker glyph.
    /// </summary>
    public string PinnedMarker { get; init; } = "[P]";

    /// <summary>
    ///     Gets or sets recent marker glyph.
    /// </summary>
    public string RecentMarker { get; init; } = "[R]";

    /// <summary>
    ///     Gets or sets separator rendered between state markers and label.
    /// </summary>
    public string MarkerSeparator { get; init; } = " ";
}
