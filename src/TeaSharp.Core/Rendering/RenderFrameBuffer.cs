namespace TeaSharp.Core.Rendering;

internal sealed class RenderFrameBuffer
{
    private readonly List<RenderRow> _rows;

    private RenderFrameBuffer(List<RenderRow> rows)
    {
        _rows = rows;
    }

    public static RenderFrameBuffer Empty { get; } = new([]);

    public int RowCount => _rows.Count;

    public static RenderFrameBuffer FromContent(string content, int width, int height)
    {
        var normalized = NormalizeLines(content);
        var wrapped = new List<DisplayLine>(normalized.Count);
        foreach (var line in normalized)
        {
            wrapped.AddRange(DisplayLine.WrapText(line, width));
        }

        if (height > 0 && wrapped.Count > height)
        {
            wrapped = wrapped.GetRange(wrapped.Count - height, height);
        }

        if (wrapped.Count == 0)
        {
            wrapped.Add(DisplayLine.FromText(string.Empty, width));
        }

        var rows = new List<RenderRow>(wrapped.Count);
        foreach (var line in wrapped)
        {
            rows.Add(RenderRow.FromDisplayLine(line, width));
        }

        return new RenderFrameBuffer(rows);
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
        if (row < 0 || row >= _rows.Count)
        {
            return 0;
        }

        return _rows[row].ColumnCount;
    }

    public string SignatureAt(int row, int column)
    {
        if (row < 0 || row >= _rows.Count)
        {
            return string.Empty;
        }

        return _rows[row].SignatureAt(column);
    }

    public string? CellAt(int row, int column)
    {
        if (row < 0 || row >= _rows.Count)
        {
            return null;
        }

        return _rows[row].CellAt(column);
    }

    public string StyleAt(int row, int column)
    {
        if (row < 0 || row >= _rows.Count)
        {
            return string.Empty;
        }

        return _rows[row].StyleAt(column);
    }

    public int CellWidthAt(int row, int column)
    {
        if (row < 0 || row >= _rows.Count)
        {
            return 1;
        }

        return _rows[row].CellWidthAt(column);
    }

    private static List<string> NormalizeLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return [string.Empty];
        }

        content = content.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        return [.. content.Split('\n')];
    }

    private readonly struct RenderCell(string? text, string style, bool continuation, int width)
    {
        public string? Text { get; } = text;
        public string Style { get; } = style;
        public bool Continuation { get; } = continuation;
        public int Width { get; } = width;
    }

    private sealed class RenderRow
    {
        private const string ContinuationMarker = "\u0000";
        private readonly RenderCell[] _cells;

        private RenderRow(RenderCell[] cells)
        {
            _cells = cells;
        }

        public int ColumnCount => _cells.Length;

        public static RenderRow FromDisplayLine(DisplayLine line, int maxWidth)
        {
            var columnCount = line.ColumnCount;
            if (maxWidth > 0 && columnCount > maxWidth)
            {
                columnCount = maxWidth;
            }

            if (columnCount == 0)
            {
                return new RenderRow([]);
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

            return new RenderRow(cells);
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

            return _cells[column].Continuation
                ? null
                : _cells[column].Text;
        }

        public string StyleAt(int column)
        {
            if (column < 0 || column >= _cells.Length)
            {
                return string.Empty;
            }

            return _cells[column].Continuation
                ? string.Empty
                : _cells[column].Style;
        }

        public int CellWidthAt(int column)
        {
            if (column < 0 || column >= _cells.Length)
            {
                return 1;
            }

            return _cells[column].Continuation
                ? 1
                : Math.Max(1, _cells[column].Width);
        }
    }
}
