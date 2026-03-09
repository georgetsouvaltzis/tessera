using System.Text;

namespace TeaSharp.Widgets.Internal;

internal static class HelpViewLayout
{
    public static string RenderColumns(IReadOnlyList<string> chunks, int maxWidth, int minColumnWidth, int columnGap)
    {
        if (chunks.Count == 0)
        {
            return string.Empty;
        }

        if (maxWidth <= 0)
        {
            return string.Join('\n', chunks);
        }

        var contentWidth = Math.Max(minColumnWidth, chunks.Max(static chunk => chunk.Length));
        var gap = Math.Max(1, columnGap);
        var perColumn = contentWidth + gap;
        var columns = Math.Max(1, (maxWidth + gap) / perColumn);
        if (columns <= 1)
        {
            return string.Join('\n', chunks.Select(chunk => chunk.Length <= maxWidth ? chunk : chunk[..maxWidth]));
        }

        var rows = (int)Math.Ceiling(chunks.Count / (double)columns);
        var lines = new List<string>(rows);
        for (var row = 0; row < rows; row++)
        {
            var line = new StringBuilder(maxWidth);
            for (var column = 0; column < columns; column++)
            {
                var index = row + (column * rows);
                if (index >= chunks.Count)
                {
                    continue;
                }

                var rendered = chunks[index].Length <= contentWidth
                    ? chunks[index]
                    : chunks[index][..contentWidth];

                if (line.Length > 0)
                {
                    line.Append(' ', gap);
                }

                line.Append(column == columns - 1 ? rendered : rendered.PadRight(contentWidth));
            }

            lines.Add(line.ToString().TrimEnd());
        }

        return string.Join('\n', lines);
    }

    public static string RenderCompact(IReadOnlyList<string> chunks, int maxWidth)
    {
        if (chunks.Count == 0)
        {
            return string.Empty;
        }

        if (maxWidth <= 0)
        {
            return string.Join("  |  ", chunks);
        }

        var lines = new List<string>();
        var current = string.Empty;
        foreach (var chunk in chunks)
        {
            var candidate = current.Length == 0 ? chunk : $"{current}  |  {chunk}";
            if (candidate.Length <= maxWidth)
            {
                current = candidate;
                continue;
            }

            if (current.Length > 0)
            {
                lines.Add(current);
            }

            current = chunk.Length <= maxWidth ? chunk : chunk[..maxWidth];
        }

        if (current.Length > 0)
        {
            lines.Add(current);
        }

        return string.Join('\n', lines);
    }
}
