namespace TeaSharp.Widgets;

internal static class ViewportVisualLineBuilder
{
    public static void Build(IReadOnlyList<string> sourceLines, bool wrap, int width, List<string> target, out int maxVisualWidth)
    {
        target.Clear();
        maxVisualWidth = 0;

        if (sourceLines.Count == 0)
        {
            target.Add(string.Empty);
            return;
        }

        if (!wrap || width <= 0)
        {
            foreach (var line in sourceLines)
            {
                target.Add(line);
                maxVisualWidth = Math.Max(maxVisualWidth, line.Length);
            }

            return;
        }

        foreach (var sourceLine in sourceLines)
        {
            if (sourceLine.Length == 0)
            {
                target.Add(string.Empty);
                continue;
            }

            for (var i = 0; i < sourceLine.Length; i += width)
            {
                var length = Math.Min(width, sourceLine.Length - i);
                target.Add(sourceLine.Substring(i, length));
                maxVisualWidth = Math.Max(maxVisualWidth, length);
            }
        }
    }
}
