namespace TeaSharp.Controls;

/// <summary>
/// Represents one matrix cell rendered by <see cref="Heatmap"/>.
/// </summary>
public readonly record struct HeatmapCell
{
    /// <summary>
    /// Initializes a new heatmap cell.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="value">Numeric value used for color/glyph intensity mapping.</param>
    /// <param name="label">Optional human-readable cell label.</param>
    public HeatmapCell(int row, int column, double value, string? label = null)
    {
        Row = row;
        Column = column;
        Value = value;
        Label = label ?? string.Empty;
    }

    /// <summary>
    /// Gets the zero-based row index.
    /// </summary>
    public int Row { get; init; }

    /// <summary>
    /// Gets the zero-based column index.
    /// </summary>
    public int Column { get; init; }

    /// <summary>
    /// Gets the numeric value used by rendering.
    /// </summary>
    public double Value { get; init; }

    /// <summary>
    /// Gets optional per-cell label text.
    /// </summary>
    public string Label { get; init; }
}
