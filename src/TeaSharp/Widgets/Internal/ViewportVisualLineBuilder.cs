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
                AppendLine(line, wrap, width, target, ref maxVisualWidth);
            }

            return;
        }

        foreach (var sourceLine in sourceLines)
        {
            AppendLine(sourceLine, wrap, width, target, ref maxVisualWidth);
        }
    }

    public static void AppendLine(string sourceLine, bool wrap, int width, List<string> target, ref int maxVisualWidth)
    {
        if (!wrap || width <= 0)
        {
            target.Add(sourceLine);
            maxVisualWidth = Math.Max(maxVisualWidth, sourceLine.Length);
            return;
        }

        if (sourceLine.Length == 0)
        {
            target.Add(string.Empty);
            return;
        }

        for (var index = 0; index < sourceLine.Length; index += width)
        {
            var length = Math.Min(width, sourceLine.Length - index);
            target.Add(sourceLine.Substring(index, length));
            maxVisualWidth = Math.Max(maxVisualWidth, length);
        }
    }
}
