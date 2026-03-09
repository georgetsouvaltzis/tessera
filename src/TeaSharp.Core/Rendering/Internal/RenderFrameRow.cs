namespace TeaSharp.Core.Rendering;

internal sealed class RenderFrameRow
{
    private const string ContinuationMarker = "\u0000";
    private readonly RenderCell[] _cells;

    private readonly struct RenderCell(string? text, string style, bool continuation, int width)
    {
        public string? Text { get; } = text;
        public string Style { get; } = style;
        public bool Continuation { get; } = continuation;
        public int Width { get; } = width;
    }

    private RenderFrameRow(RenderCell[] cells)
    {
        _cells = cells;
    }

    public int ColumnCount => _cells.Length;

    public static RenderFrameRow FromDisplayLine(DisplayLine line, int maxWidth)
    {
        var columnCount = line.ColumnCount;
        if (maxWidth > 0 && columnCount > maxWidth)
        {
            columnCount = maxWidth;
        }

        if (columnCount == 0)
        {
            return new RenderFrameRow([]);
        }

        var cells = new RenderCell[columnCount];
        for (var column = 0; column < columnCount; column++)
        {
            var text = line.CellAt(column);
            var continuation = text is null;
            var style = continuation ? string.Empty : line.StyleAt(column);
            var width = continuation ? 1 : line.CellWidthAt(column);
            cells[column] = new RenderCell(text, style, continuation, width);
        }

        return new RenderFrameRow(cells);
    }

    public string SignatureAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return string.Empty;
        }

        var cell = _cells[column];
        if (cell.Continuation)
        {
            return ContinuationMarker;
        }

        if (cell.Text is null)
        {
            return string.Empty;
        }

        return string.IsNullOrEmpty(cell.Style)
            ? cell.Text
            : $"{cell.Style}\u001f{cell.Text}";
    }

    public string? CellAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return null;
        }

        return _cells[column].Continuation ? null : _cells[column].Text;
    }

    public string StyleAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return string.Empty;
        }

        return _cells[column].Continuation ? string.Empty : _cells[column].Style;
    }

    public int CellWidthAt(int column)
    {
        if (column < 0 || column >= _cells.Length)
        {
            return 1;
        }

        return _cells[column].Continuation ? 1 : Math.Max(1, _cells[column].Width);
    }
}
