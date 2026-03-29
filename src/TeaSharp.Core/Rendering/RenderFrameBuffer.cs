namespace TeaSharp.Core.Rendering;

internal sealed class RenderFrameBuffer
{
    private readonly List<RenderFrameRow> _rows;

    private RenderFrameBuffer(List<RenderFrameRow> rows)
    {
        _rows = rows;
    }

    public static RenderFrameBuffer Empty { get; } = new([]);

    public int RowCount => _rows.Count;

    public static RenderFrameBuffer FromContent(string content, int width, int height)
    {
        return new RenderFrameBuffer(RenderFrameContent.BuildRows(content, width, height));
    }

    public bool RowEquals(RenderFrameBuffer previous, int row)
    {
        var maxColumns = Math.Max(previous.ColumnCountAt(row), ColumnCountAt(row));
        for (var column = 0; column < maxColumns; column++)
        {
            if (!CellEquals(previous, row, column))
            {
                return false;
            }
        }

        return true;
    }

    public int ColumnCountAt(int row)
    {
        return row < 0 || row >= _rows.Count ? 0 : _rows[row].ColumnCount;
    }

    public string SignatureAt(int row, int column)
    {
        return row < 0 || row >= _rows.Count ? string.Empty : _rows[row].SignatureAt(column);
    }

    public bool CellEquals(RenderFrameBuffer other, int row, int column)
    {
        var hasThis = row >= 0 && row < _rows.Count;
        var hasOther = row >= 0 && row < other._rows.Count;
        if (!hasThis || !hasOther)
        {
            return !hasThis && !hasOther;
        }

        return _rows[row].CellEquals(other._rows[row], column);
    }

    public string? CellAt(int row, int column)
    {
        return row < 0 || row >= _rows.Count ? null : _rows[row].CellAt(column);
    }

    public string StyleAt(int row, int column)
    {
        return row < 0 || row >= _rows.Count ? string.Empty : _rows[row].StyleAt(column);
    }

    public int CellWidthAt(int row, int column)
    {
        return row < 0 || row >= _rows.Count ? 1 : _rows[row].CellWidthAt(column);
    }
}
