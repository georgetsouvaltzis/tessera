namespace TeaSharp.Widgets.Internal;

internal static class ViewportRenderer
{
    public static IReadOnlyList<string> RenderLines(
        IReadOnlyList<string> visualLines,
        int width,
        int height,
        int xOffset,
        int yOffset,
        bool wrap,
        bool showLineNumbers,
        int? highlightVisualLine)
    {
        if (visualLines.Count == 0)
        {
            return [string.Empty];
        }

        var start = Math.Clamp(yOffset, 0, Math.Max(0, visualLines.Count - 1));
        var max = Math.Min(height, visualLines.Count - start);
        if (max <= 0)
        {
            return [string.Empty];
        }

        var rendered = new List<string>(max);
        var lineNumberWidth = ViewportLineFormatter.ComputeLineNumberWidth(showLineNumbers, visualLines.Count);
        for (var i = 0; i < max; i++)
        {
            var visualIndex = start + i;
            var line = visualLines[visualIndex];
            var clipped = ViewportLineFormatter.ClipLine(line, wrap, width, xOffset, showLineNumbers, lineNumberWidth);
            rendered.Add(ViewportLineFormatter.DecorateLine(clipped, showLineNumbers, highlightVisualLine, visualIndex, lineNumberWidth, width));
        }

        return rendered;
    }
}
