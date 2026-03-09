namespace TeaSharp.Widgets;

internal static class ListModelFilter
{
    public static void Apply<T>(
        IReadOnlyList<T> allItems,
        Func<T, string> toText,
        string filter,
        StringComparison comparison,
        Comparison<T>? sortComparison,
        List<int> filteredIndexes)
    {
        filteredIndexes.Clear();
        for (var i = 0; i < allItems.Count; i++)
        {
            var label = toText(allItems[i]);
            if (filter.Length == 0 || label.Contains(filter, comparison))
            {
                filteredIndexes.Add(i);
            }
        }

        if (sortComparison is not null && filteredIndexes.Count > 1)
        {
            filteredIndexes.Sort((left, right) => sortComparison(allItems[left], allItems[right]));
        }
    }
}
