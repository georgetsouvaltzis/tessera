using Tessera.Components.Primitives;
namespace Tessera.Controls.Internal;

internal static class OptionListViewport
{
    public static int ComputeWindowStart(int highlightedIndex, int rows, int count)
    {
        if (count <= rows)
        {
            return 0;
        }

        var half = rows / 2;
        var start = highlightedIndex - half;
        if (start < 0)
        {
            return 0;
        }

        var maxStart = count - rows;
        if (start > maxStart)
        {
            return maxStart;
        }

        return start;
    }

    public static int RowToVisibleIndex(Rect content, int y, int maxVisibleItems, int visibleCount, int highlightedVisibleIndex)
    {
        if (content.Height <= 1 || visibleCount == 0)
        {
            return -1;
        }

        var visibleRows = Math.Min(Math.Max(1, maxVisibleItems), content.Height - 1);
        var row = y - (content.Y + 1);
        if (row < 0 || row >= visibleRows)
        {
            return -1;
        }

        var start = ComputeWindowStart(highlightedVisibleIndex, visibleRows, visibleCount);
        var visibleIndex = start + row;
        return visibleIndex >= 0 && visibleIndex < visibleCount
            ? visibleIndex
            : -1;
    }
}
