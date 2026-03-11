namespace TeaSharp.Components.UiKit.Internal;

internal static class SortableTableRenderStateBuilder
{
    public static SortableTableRenderState Build(
        IReadOnlyList<IReadOnlyList<string>> rows,
        IReadOnlyList<string> headers,
        string title,
        int sortColumn,
        bool sortDescending,
        int pageSize,
        int pageIndex,
        bool enableVirtualization,
        int virtualStartIndex,
        int virtualWindowSize)
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
        if (enableVirtualization)
        {
            var virtualOffset = Math.Clamp(virtualStartIndex, 0, Math.Max(0, sorted.Count - 1));
            var safeWindow = Math.Max(1, virtualWindowSize);
            visibleRows = sorted.Skip(virtualOffset).Take(safeWindow).ToList();
        }

        return new SortableTableRenderState(
            visibleRows,
            BuildTitle(title, headers, sortColumn, sortDescending, enableVirtualization, virtualStartIndex, virtualWindowSize, page, pageCount),
            visibleRows.Count);
    }

    private static string ValueAt(IReadOnlyList<string> row, int column)
    {
        if (column < 0 || column >= row.Count)
        {
            return string.Empty;
        }

        return row[column];
    }

    private static string BuildTitle(
        string title,
        IReadOnlyList<string> headers,
        int sortColumn,
        bool sortDescending,
        bool enableVirtualization,
        int virtualStartIndex,
        int virtualWindowSize,
        int page,
        int pageCount)
    {
        var sortLabel = headers[Math.Min(sortColumn, headers.Count - 1)];
        return enableVirtualization
            ? $"{title} v{virtualStartIndex + 1}+{Math.Max(1, virtualWindowSize)} sort:{sortLabel} {(sortDescending ? "desc" : "asc")}"
            : $"{title} p{page + 1}/{pageCount} sort:{sortLabel} {(sortDescending ? "desc" : "asc")}";
    }
}
