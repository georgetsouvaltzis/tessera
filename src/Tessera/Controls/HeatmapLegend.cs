using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents one legend band used by <see cref="Heatmap"/>.
/// </summary>
public readonly record struct HeatmapLegend
{
    /// <summary>
    /// Initializes a new legend band.
    /// </summary>
    /// <param name="label">Legend label shown in the footer row.</param>
    /// <param name="minInclusive">Minimum value in range, inclusive.</param>
    /// <param name="maxInclusive">Maximum value in range, inclusive.</param>
    /// <param name="glyph">Glyph rendered for cells inside the band.</param>
    /// <param name="style">Style merged for cells and legend text in this band.</param>
    public HeatmapLegend(
        string? label,
        double minInclusive,
        double maxInclusive,
        char glyph = '█',
        TesseraStyle style = default)
    {
        Label = label ?? string.Empty;
        MinInclusive = minInclusive;
        MaxInclusive = maxInclusive;
        Glyph = glyph;
        Style = style;
    }

    /// <summary>
    /// Gets legend label text.
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    /// Gets minimum value in range, inclusive.
    /// </summary>
    public double MinInclusive { get; init; }

    /// <summary>
    /// Gets maximum value in range, inclusive.
    /// </summary>
    public double MaxInclusive { get; init; }

    /// <summary>
    /// Gets glyph rendered for the range.
    /// </summary>
    public char Glyph { get; init; }

    /// <summary>
    /// Gets style merged for this legend band.
    /// </summary>
    public TesseraStyle Style { get; init; }
}
