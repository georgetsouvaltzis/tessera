namespace TeaSharp.Widgets;

internal static class ViewportLineFormatter
{
    public static int ComputeLineNumberWidth(bool showLineNumbers, int visualLineCount)
    {
        return showLineNumbers
            ? Math.Max(2, (visualLineCount + 1).ToString(System.Globalization.CultureInfo.InvariantCulture).Length)
            : 0;
    }

    public static string ClipLine(string line, bool wrap, int width, int xOffset, bool showLineNumbers, int lineNumberWidth)
    {
        var availableWidth = showLineNumbers
            ? Math.Max(0, width - (lineNumberWidth + 2))
            : width;
        if (availableWidth <= 0)
        {
            return string.Empty;
        }

        if (wrap)
        {
            return line.Length <= availableWidth ? line : line[..availableWidth];
        }

        if (xOffset >= line.Length)
        {
            return string.Empty;
        }

        if (xOffset == 0 && line.Length <= availableWidth)
        {
            return line;
        }

        var remaining = line.Length - xOffset;
        var length = Math.Min(availableWidth, remaining);
        return line.Substring(xOffset, length);
    }

    public static string DecorateLine(string line, bool showLineNumbers, int? highlightVisualLine, int visualIndex, int lineNumberWidth, int width)
    {
        if (!showLineNumbers && highlightVisualLine != visualIndex)
        {
            return line;
        }

        if (!showLineNumbers)
        {
            return highlightVisualLine == visualIndex ? $"> {line}" : $"  {line}";
        }

        var lineNumber = (visualIndex + 1).ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(lineNumberWidth);
        var marker = highlightVisualLine == visualIndex ? ">" : " ";
        var prefix = $"{lineNumber}{marker} ";
        if (prefix.Length >= width)
        {
            return prefix[..width];
        }

        var available = width - prefix.Length;
        var clipped = line.Length <= available ? line : line[..available];
        return prefix + clipped;
    }

    public static List<string> NormalizeContentLines(string content)
    {
        var normalized = content
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        return [.. normalized.Split('\n')];
    }
}
