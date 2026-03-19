namespace TeaSharp.Widgets.Internal;

internal static class ViewportRenderer
{
    [ThreadStatic]
    private static List<string>? s_threadRenderBuffer;

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
        var rendered = target ?? (s_threadRenderBuffer ??= []);
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

        var noDecoration = !showLineNumbers && !highlightVisualLine.HasValue;
        if (noDecoration)
        {
            RenderNoDecorationLines(visualLines, start, max, wrap, width, xOffset, rendered);
            return rendered;
        }

        var lineNumberWidth = showLineNumbers
            ? ViewportLineFormatter.ComputeLineNumberWidth(showLineNumbers: true, visualLineCount: visualLines.Count)
            : 0;
        for (var i = 0; i < max; i++)
        {
            var visualIndex = start + i;
            var line = visualLines[visualIndex];

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

    private static void RenderNoDecorationLines(
        IReadOnlyList<string> visualLines,
        int start,
        int max,
        bool wrap,
        int width,
        int xOffset,
        List<string> rendered)
    {
        if (width <= 0)
        {
            for (var i = 0; i < max; i++)
            {
                rendered.Add(string.Empty);
            }

            return;
        }

        if (xOffset == 0 || wrap)
        {
            for (var i = 0; i < max; i++)
            {
                var line = visualLines[start + i];
                rendered.Add(line.Length <= width ? line : line[..width]);
            }

            return;
        }

        for (var i = 0; i < max; i++)
        {
            rendered.Add(ViewportLineFormatter.FormatNoDecoration(visualLines[start + i], wrap: false, width, xOffset));
        }
    }
}
