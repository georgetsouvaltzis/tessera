namespace Tessera.Controls;

/// <summary>
/// Represents one pivot value at the intersection of a row key and column key.
/// </summary>
public sealed class PivotTableCell
{
    /// <summary>
    /// Initializes a pivot cell payload.
    /// </summary>
    /// <param name="rowKey">Row key.</param>
    /// <param name="columnKey">Column key.</param>
    /// <param name="value">Display value.</param>
    public PivotTableCell(string rowKey, string columnKey, string value)
    {
        RowKey = rowKey ?? string.Empty;
        ColumnKey = columnKey ?? string.Empty;
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets row key.
    /// </summary>
    public string RowKey { get; set; }

    /// <summary>
    /// Gets or sets column key.
    /// </summary>
    public string ColumnKey { get; set; }

    /// <summary>
    /// Gets or sets display value.
    /// </summary>
    public string Value { get; set; }
}
