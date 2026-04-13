using System.Text;

namespace Tessera.Widgets.Internal;

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
            var builder = new StringBuilder();
            for (var index = 0; index < chunks.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append('\n');
                }

                var chunk = chunks[index];
                if (chunk.Length <= maxWidth)
                {
                    builder.Append(chunk);
                }
                else
                {
                    builder.Append(chunk, 0, maxWidth);
                }
            }

            return builder.ToString();
        }

        var rows = (int)Math.Ceiling(chunks.Count / (double)columns);
        var lines = new List<string>(rows);
        for (var row = 0; row < rows; row++)
        {
            var line = new StringBuilder(maxWidth);
            for (var column = 0; column < columns; column++)
            {
                var index = row + column * rows;
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

        const string separator = "  |  ";
        var lines = new List<string>();
        var current = new StringBuilder(maxWidth);
        foreach (var chunk in chunks)
        {
            if (current.Length == 0)
            {
                if (chunk.Length <= maxWidth)
                {
                    current.Append(chunk);
                }
                else
                {
                    lines.Add(chunk[..maxWidth]);
                }

                continue;
            }

            var candidateLength = current.Length + separator.Length + chunk.Length;
            if (candidateLength <= maxWidth)
            {
                current.Append(separator);
                current.Append(chunk);
                continue;
            }

            lines.Add(current.ToString());
            current.Clear();
            if (chunk.Length <= maxWidth)
            {
                current.Append(chunk);
            }
            else
            {
                lines.Add(chunk[..maxWidth]);
            }
        }

        if (current.Length != 0)
        {
            lines.Add(current.ToString());
        }

        return string.Join('\n', lines);
    }
}
