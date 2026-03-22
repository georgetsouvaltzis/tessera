namespace TeaSharp.Core.Rendering;

internal static class RenderFrameContent
{
    public static List<string> NormalizeLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return [string.Empty];
        }

        var lines = new List<string>(EstimateLineCount(content));
        var start = 0;
        for (var index = 0; index < content.Length; index++)
        {
            var current = content[index];
            if (current is not ('\r' or '\n'))
            {
                continue;
            }

            lines.Add(content[start..index]);
            if (current == '\r' && index + 1 < content.Length && content[index + 1] == '\n')
            {
                index++;
            }

            start = index + 1;
        }

        lines.Add(content[start..]);
        return lines;
    }

    public static List<DisplayLine> WrapLines(IReadOnlyList<string> normalized, int width)
    {
        var wrapped = new List<DisplayLine>(normalized.Count);
        foreach (var line in normalized)
        {
            wrapped.AddRange(DisplayLine.WrapText(line, width));
        }

        if (wrapped.Count == 0)
        {
            wrapped.Add(DisplayLine.FromText(string.Empty, width));
        }

        return wrapped;
    }

    public static List<RenderFrameRow> ToRows(IReadOnlyList<DisplayLine> wrapped, int width)
    {
        var rows = new List<RenderFrameRow>(wrapped.Count);
        foreach (var line in wrapped)
        {
            rows.Add(RenderFrameRow.FromDisplayLine(line, width));
        }

        return rows;
    }

    private static int EstimateLineCount(string content)
    {
        var lines = 1;
        for (var index = 0; index < content.Length; index++)
        {
            var current = content[index];
            if (current is '\r' or '\n')
            {
                lines++;
            }
        }

        return lines;
    }
}
