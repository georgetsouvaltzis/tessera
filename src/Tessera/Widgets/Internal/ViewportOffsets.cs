namespace Tessera.Widgets.Internal;

internal static class ViewportOffsets
{
    public static int ClampY(int requested, int visualLineCount, int height)
    {
        var maxY = Math.Max(0, visualLineCount - height);
        return Math.Clamp(requested, 0, maxY);
    }

    public static int ClampX(bool wrap, bool showLineNumbers, int requested, int width, int visualLineCount,
        int maxVisualWidth)
    {
        if (wrap)
        {
            return 0;
        }

        var lineNumberWidth = ViewportLineFormatter.ComputeLineNumberWidth(showLineNumbers, visualLineCount);
        var visibleWidth = showLineNumbers ? Math.Max(0, width - (lineNumberWidth + 2)) : width;
        var maxX = Math.Max(0, maxVisualWidth - visibleWidth);
        return Math.Clamp(requested, 0, maxX);
    }
}
