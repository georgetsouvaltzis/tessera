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
        var normalized = RenderFrameContent.NormalizeLines(content);
        var wrapped = RenderFrameContent.WrapLines(normalized, width);
        if (height > 0 && wrapped.Count > height)
        {
            wrapped = wrapped.GetRange(wrapped.Count - height, height);
        }

        return new RenderFrameBuffer(RenderFrameContent.ToRows(wrapped, width));
    }

    public bool RowEquals(RenderFrameBuffer previous, int row)
    {
        var maxColumns = Math.Max(previous.ColumnCountAt(row), ColumnCountAt(row));
        for (var column = 0; column < maxColumns; column++)
        {
            if (!string.Equals(previous.SignatureAt(row, column), SignatureAt(row, column), StringComparison.Ordinal))
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
