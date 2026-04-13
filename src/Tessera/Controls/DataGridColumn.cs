namespace Tessera.Controls;

/// <summary>
///     Represents one column definition in a <see cref="DataGrid" />.
/// </summary>
public sealed class DataGridColumn
{
    /// <summary>
    ///     Initializes a new data-grid column.
    /// </summary>
    /// <param name="id">Stable column identifier.</param>
    /// <param name="header">Header text rendered for the column.</param>
    public DataGridColumn(string id, string header)
    {
        Id = id;
        Header = header;
    }

    /// <summary>
    ///     Gets the stable column identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     Gets or sets the header text rendered for the column.
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    ///     Gets or sets an optional fixed display width for this column.
    ///     Values less than or equal to zero are normalized to <see langword="null" />.
    /// </summary>
    public int? Width
    {
        get;
        set => field = value is > 0 ? value : null;
    }

    /// <summary>
    ///     Gets or sets whether the column can participate in sort requests.
    /// </summary>
    public bool IsSortable { get; set; }

    /// <summary>
    ///     Gets or sets the optional comparer used for built-in row sorting.
    /// </summary>
    public Comparison<string>? SortComparer { get; set; }
}
