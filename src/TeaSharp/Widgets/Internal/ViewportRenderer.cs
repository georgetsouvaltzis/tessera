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
        int? highlightVisualLine,
        List<string>? target = null)
    {
        var rendered = target ?? [];
        rendered.Clear();

        if (visualLines.Count == 0)
        {
            rendered.Add(string.Empty);
            return rendered;
        }

        var start = Math.Clamp(yOffset, 0, Math.Max(0, visualLines.Count - 1));
        var max = Math.Min(height, visualLines.Count - start);
        if (max <= 0)
        {
            rendered.Add(string.Empty);
            return rendered;
        }

        if (rendered.Capacity < max)
        {
            rendered.Capacity = max;
        }

        var lineNumberWidth = ViewportLineFormatter.ComputeLineNumberWidth(showLineNumbers, visualLines.Count);
        var canBypassDecoration = !showLineNumbers && !highlightVisualLine.HasValue && xOffset == 0;
        for (var i = 0; i < max; i++)
        {
            var visualIndex = start + i;
            var line = visualLines[visualIndex];
            if (canBypassDecoration && (wrap || line.Length <= width))
            {
                rendered.Add(line);
                continue;
            }

            rendered.Add(
                ViewportLineFormatter.FormatLine(
                    line,
                    wrap,
                    width,
                    xOffset,
                    showLineNumbers,
                    highlightVisualLine,
                    visualIndex,
                    lineNumberWidth));
        }

        return rendered;
    }
}
