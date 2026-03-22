namespace TeaSharp.Controls;

/// <summary>
/// Describes a tile rendered by <see cref="DashboardGrid" />.
/// </summary>
public sealed record DashboardTile
{
    /// <summary>
    /// Initializes a new tile definition.
    /// </summary>
    /// <param name="id">Stable tile identifier.</param>
    /// <param name="title">Tile title rendered in the tile frame.</param>
    /// <param name="column">Zero-based tile column.</param>
    /// <param name="row">Zero-based tile row.</param>
    /// <param name="columnSpan">Tile column span. Must be greater than zero.</param>
    /// <param name="rowSpan">Tile row span. Must be greater than zero.</param>
    /// <param name="subtitle">Optional subtitle rendered inside the tile.</param>
    /// <exception cref="ArgumentException"><paramref name="id" /> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="column" />, <paramref name="row" />, <paramref name="columnSpan" />, or
    /// <paramref name="rowSpan" /> is out of range.
    /// </exception>
    public DashboardTile(
        string id,
        string title,
        int column,
        int row,
        int columnSpan = 1,
        int rowSpan = 1,
        string? subtitle = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Tile id must be non-empty.", nameof(id));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfLessThan(columnSpan, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowSpan, 1);

        Id = id;
        Title = title ?? string.Empty;
        Column = column;
        Row = row;
        ColumnSpan = columnSpan;
        RowSpan = rowSpan;
        Subtitle = subtitle ?? string.Empty;
    }

    /// <summary>
    /// Gets tile identifier.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Gets tile title.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Gets tile column.
    /// </summary>
    public int Column { get; init; }

    /// <summary>
    /// Gets tile row.
    /// </summary>
    public int Row { get; init; }

    /// <summary>
    /// Gets tile column span.
    /// </summary>
    public int ColumnSpan { get; init; }

    /// <summary>
    /// Gets tile row span.
    /// </summary>
    public int RowSpan { get; init; }

    /// <summary>
    /// Gets optional subtitle rendered under the tile id.
    /// </summary>
    public string Subtitle { get; init; }
}
