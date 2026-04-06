namespace Tessera.Controls;

/// <summary>
/// Identifies sort direction in a <see cref="DataGrid" /> request.
/// </summary>
public enum DataGridSortDirection
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
/// Provides details when a data-grid column sort is requested.
/// </summary>
public sealed class DataGridSortRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new sort-request payload.
    /// </summary>
    /// <param name="columnIndex">The requested column index.</param>
    /// <param name="column">The requested column definition.</param>
    /// <param name="direction">The requested sort direction.</param>
    public DataGridSortRequestedEventArgs(int columnIndex, DataGridColumn column, DataGridSortDirection direction)
    {
        ColumnIndex = columnIndex;
        Column = column ?? throw new ArgumentNullException(nameof(column));
        Direction = direction;
    }

    /// <summary>
    /// Gets the requested column index.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets the requested column definition.
    /// </summary>
    public DataGridColumn Column { get; }

    /// <summary>
    /// Gets the requested sort direction.
    /// </summary>
    public DataGridSortDirection Direction { get; }

    /// <summary>
    /// Gets or sets whether the sort request was handled externally.
    /// </summary>
    public bool Handled { get; set; }
}
