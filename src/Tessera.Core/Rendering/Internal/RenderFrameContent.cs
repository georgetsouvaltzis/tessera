namespace Tessera.Core.Rendering.Internal;

internal static class RenderFrameContent
{
    public static List<RenderFrameRow> BuildRows(string content, int width, int height)
    {
        List<RenderFrameRow>? rows = height > 0
            ? null
            : new List<RenderFrameRow>(EstimateLineCount(content));
        Queue<RenderFrameRow>? clippedRows = height > 0
            ? new Queue<RenderFrameRow>(height)
            : null;

        var start = 0;
        var index = 0;
        while (index < content.Length)
        {
            var current = content[index];
            if (current is not ('\r' or '\n'))
            {
                index++;
                continue;
            }

            AppendWrappedRows(content[start..index], width, height, rows, clippedRows);
            if (current == '\r' && index + 1 < content.Length && content[index + 1] == '\n')
            {
                index++;
            }

            start = index + 1;
            index++;
        }

        AppendWrappedRows(content[start..], width, height, rows, clippedRows);

        if (rows is not null)
        {
            if (rows.Count == 0)
            {
                rows.Add(RenderFrameRow.Empty);
            }

            return rows;
        }

        if (clippedRows is null || clippedRows.Count == 0)
        {
            return [RenderFrameRow.Empty];
        }

        return [.. clippedRows];
    }

    public static List<string> NormalizeLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return [string.Empty];
        }

        var lines = new List<string>(EstimateLineCount(content));
        var start = 0;
        var index = 0;
        while (index < content.Length)
        {
            var current = content[index];
            if (current is not ('\r' or '\n'))
            {
                index++;
                continue;
            }

            lines.Add(content[start..index]);
            if (current == '\r' && index + 1 < content.Length && content[index + 1] == '\n')
            {
                index++;
            }

            start = index + 1;
            index++;
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

    private static void AppendWrappedRows(
        string line,
        int width,
        int height,
        List<RenderFrameRow>? rows,
        Queue<RenderFrameRow>? clippedRows)
    {
        foreach (var wrappedLine in DisplayLine.WrapText(line, width))
        {
            var row = RenderFrameRow.FromDisplayLine(wrappedLine, width);
            if (rows is not null)
            {
                rows.Add(row);
                continue;
            }

            if (clippedRows is null)
            {
                continue;
            }

            if (height > 0 && clippedRows.Count == height)
            {
                _ = clippedRows.Dequeue();
            }

            clippedRows.Enqueue(row);
        }
    }
}
