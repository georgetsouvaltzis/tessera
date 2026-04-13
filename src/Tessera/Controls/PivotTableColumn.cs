namespace Tessera.Controls;

/// <summary>
///     Defines one value column in a <see cref="PivotTable" />.
/// </summary>
public sealed class PivotTableColumn
{
    /// <summary>
    ///     Initializes a new pivot column.
    /// </summary>
    /// <param name="key">Stable column key.</param>
    /// <param name="header">Header text.</param>
    public PivotTableColumn(string key, string header)
    {
        Key = key;
        Header = header;
    }

    /// <summary>
    ///     Gets or sets stable column key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///     Gets or sets header text.
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    ///     Gets or sets whether this column supports sorting.
    /// </summary>
    public bool IsSortable { get; set; }

    /// <summary>
    ///     Gets or sets optional comparer used for built-in row sorting by this column.
    /// </summary>
    public Comparison<string>? SortComparer { get; set; }
}
