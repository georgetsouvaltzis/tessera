namespace Tessera.Widgets;

internal static class ListModelWindowing
{
    public static void EnsureSelectionVisible(int selectedIndex, int count, int pageSize, ref int offset)
    {
        if (selectedIndex < 0 || selectedIndex >= count)
        {
            offset = 0;
            return;
        }

        var page = Math.Max(1, pageSize);
        if (selectedIndex < offset)
        {
            offset = selectedIndex;
        }
        else if (selectedIndex >= offset + page)
        {
            offset = selectedIndex - page + 1;
        }

        var maxOffset = Math.Max(0, count - page);
        offset = Math.Clamp(offset, 0, maxOffset);
    }

    public static List<ListRow<T>> VisibleRows<T>(IReadOnlyList<T> allItems, IReadOnlyList<int> filteredIndexes, int offset, int pageSize, int selectedIndex)
    {
        var rows = new List<ListRow<T>>(Math.Max(1, pageSize));
        if (filteredIndexes.Count == 0 || pageSize <= 0)
        {
            return rows;
        }

        var start = Math.Clamp(offset, 0, Math.Max(0, filteredIndexes.Count - 1));
        var max = Math.Min(pageSize, filteredIndexes.Count - start);
        for (var i = 0; i < max; i++)
        {
            var filteredIndex = start + i;
            var sourceIndex = filteredIndexes[filteredIndex];
            rows.Add(new ListRow<T>(allItems[sourceIndex], filteredIndex, filteredIndex == selectedIndex));
        }

        return rows;
    }
}
