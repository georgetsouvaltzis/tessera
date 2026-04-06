namespace Tessera.Core.Rendering;

internal sealed class DisplayLine
{
    private const string ContinuationMarker = "\u0000";
    private readonly string?[] _cells;
    private readonly string?[] _styles;

    public DisplayLine(string?[] cells, string?[] styles)
    {
        _cells = cells;
        _styles = styles;
    }

    public int ColumnCount => _cells.Length;

    public static IReadOnlyList<DisplayLine> WrapText(string text, int maxColumns)
    {
        return DisplayLineBuilder.WrapText(text, maxColumns);
    }

    public static DisplayLine FromText(string text, int maxColumns)
    {
        return DisplayLineBuilder.FromText(text, maxColumns);
    }

    public string SignatureAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return string.Empty;
        }

        if (_cells[column] is null)
        {
            return ContinuationMarker;
        }

        var style = _styles[column];
        return string.IsNullOrEmpty(style)
            ? _cells[column]!
            : $"{style}\u001f{_cells[column]}";
    }

    public string? CellAt(int column)
    {
        return column < 0 || column >= _cells.Length ? null : _cells[column];
    }

    public int CellWidthAt(int column)
    {
        if (column < 0 || column >= _cells.Length || _cells[column] is null)
        {
            return 1;
        }

        return column + 1 < _cells.Length && _cells[column + 1] is null ? 2 : 1;
    }

    public string StyleAt(int column)
    {
        if (column < 0 || column >= _styles.Length || _cells[column] is null)
        {
            return string.Empty;
        }

        return _styles[column] ?? string.Empty;
    }
}
