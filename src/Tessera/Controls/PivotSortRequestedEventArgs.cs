namespace Tessera.Controls;

/// <summary>
/// Identifies sort direction in <see cref="PivotTable" /> requests.
/// </summary>
public enum PivotSortDirection
{
    /// <summary>
    /// Sort ascending.
    /// </summary>
    Ascending = 0,

    /// <summary>
    /// Sort descending.
    /// </summary>
    Descending = 1,
}

/// <summary>
/// Provides details when <see cref="PivotTable" /> requests sorting for a column.
/// </summary>
public sealed class PivotSortRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a sort request payload.
    /// </summary>
    /// <param name="columnIndex">Column index.</param>
    /// <param name="column">Column metadata.</param>
    /// <param name="direction">Sort direction.</param>
    public PivotSortRequestedEventArgs(int columnIndex, PivotTableColumn column, PivotSortDirection direction)
    {
        ColumnIndex = columnIndex;
        Column = column ?? throw new ArgumentNullException(nameof(column));
        Direction = direction;
    }

    /// <summary>
    /// Gets requested column index.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets requested column metadata.
    /// </summary>
    public PivotTableColumn Column { get; }

    /// <summary>
    /// Gets requested sort direction.
    /// </summary>
    public PivotSortDirection Direction { get; }

    /// <summary>
    /// Gets or sets whether sorting was handled externally.
    /// </summary>
    public bool Handled { get; set; }
}
