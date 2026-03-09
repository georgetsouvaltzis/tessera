namespace TeaSharp.Core.Rendering;

internal static class RenderFrameContent
{
    public static List<string> NormalizeLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return [string.Empty];
        }

        content = content.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        return [.. content.Split('\n')];
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
}
