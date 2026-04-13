using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;

namespace Tessera.Controls.Internal;

internal sealed record TableRenderState(
    IReadOnlyList<IReadOnlyList<string>> VisibleRows,
    string Title,
    int VisibleRowCount);

internal static class TableViewState
{
    public static TableRenderState Build(
        IReadOnlyList<IReadOnlyList<string>> rows,
        IReadOnlyList<string> headers,
        string title,
        int sortColumn,
        bool sortDescending,
        int pageSize,
        int pageIndex)
    {
        var sorted = rows
            .OrderBy(row => ValueAt(row, sortColumn), StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (sortDescending)
        {
            sorted.Reverse();
        }

        var safePageSize = Math.Max(1, pageSize);
        var pageCount = Math.Max(1, (sorted.Count + safePageSize - 1) / safePageSize);
        var page = Math.Clamp(pageIndex, 0, pageCount - 1);
        var offset = page * safePageSize;
        var visibleRows = sorted.Skip(offset).Take(safePageSize).ToList();
        var sortLabel = headers.Count == 0
            ? string.Empty
            : headers[Math.Clamp(sortColumn, 0, headers.Count - 1)];
        return new TableRenderState(
            visibleRows,
            $"{title} p{page + 1}/{pageCount} sort:{sortLabel} {(sortDescending ? "desc" : "asc")}",
            visibleRows.Count);
    }

    public static Rect ResolveContentRect(Rect bounds, BorderStyle border, Thickness padding, string title)
    {
        if (border != BorderStyle.None)
        {
            return FrameLayout.ResolveContentRect(bounds, border, padding);
        }

        var content = bounds.Inset(padding);
        if (!string.IsNullOrWhiteSpace(title))
        {
            content = new Rect(content.X, content.Y + 1, content.Width, Math.Max(0, content.Height - 1));
        }

        return content;
    }

    public static int HeaderColumnFromPointer(int x, Rect content, int columnCount)
    {
        var separatorCount = columnCount - 1;
        var availableWidth = Math.Max(columnCount, content.Width - separatorCount);
        var widths = ComputeColumnWidths(availableWidth, columnCount);
        var cursor = content.X;
        for (var i = 0; i < widths.Length; i++)
        {
            var end = cursor + widths[i];
            if (x >= cursor && x < end)
            {
                return i;
            }

            cursor = end;
            if (i < widths.Length - 1)
            {
                cursor++;
            }
        }

        return -1;
    }

    public static int RowFromPointer(Rect content, int y, int visibleRows)
    {
        var row = y - (content.Y + 2);
        return row < 0 || row >= visibleRows ? -1 : row;
    }

    public static (int Hovered, int Selected) NormalizeVisibleRowPointers(int hovered, int selected, int visibleRows)
    {
        if (visibleRows <= 0)
        {
            return (-1, -1);
        }

        if (hovered >= visibleRows)
        {
            hovered = visibleRows - 1;
        }

        if (selected >= visibleRows)
        {
            selected = visibleRows - 1;
        }

        return (hovered, selected);
    }

    private static string ValueAt(IReadOnlyList<string> row, int column)
    {
        if (column < 0 || column >= row.Count)
        {
            return string.Empty;
        }

        return row[column];
    }

    private static int[] ComputeColumnWidths(int width, int columns)
    {
        var widths = new int[columns];
        var baseWidth = width / columns;
        var remainder = width % columns;
        for (var i = 0; i < columns; i++)
        {
            widths[i] = baseWidth + (i < remainder ? 1 : 0);
        }

        return widths;
    }
}
