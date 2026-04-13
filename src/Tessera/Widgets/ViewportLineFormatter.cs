using System.Globalization;

namespace Tessera.Widgets;

internal static class ViewportLineFormatter
{
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    public static int ComputeLineNumberWidth(bool showLineNumbers, int visualLineCount)
    {
        if (!showLineNumbers)
        {
            return 0;
        }

        var value = Math.Max(1, visualLineCount + 1);
        var digits = 1;
        while (value >= 10)
        {
            digits++;
            value /= 10;
        }

        return Math.Max(2, digits);
    }

    public static string ClipLine(string line, bool wrap, int width, int xOffset, bool showLineNumbers,
        int lineNumberWidth)
    {
        var availableWidth = showLineNumbers
            ? Math.Max(0, width - (lineNumberWidth + 2))
            : width;
        return FormatNoDecoration(line, wrap, availableWidth, xOffset);
    }

    public static string DecorateLine(string line, bool showLineNumbers, int? highlightVisualLine, int visualIndex,
        int lineNumberWidth, int width)
    {
        if (!showLineNumbers && highlightVisualLine != visualIndex)
        {
            return line;
        }

        if (!showLineNumbers)
        {
            return highlightVisualLine == visualIndex ? $"> {line}" : $"  {line}";
        }

        var lineNumber = (visualIndex + 1).ToString(CultureInfo.InvariantCulture).PadLeft(lineNumberWidth);
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

    public static string FormatLine(
        string line,
        bool wrap,
        int width,
        int xOffset,
        bool showLineNumbers,
        int? highlightVisualLine,
        int visualIndex,
        int lineNumberWidth)
    {
        var availableWidth = showLineNumbers
            ? Math.Max(0, width - (lineNumberWidth + 2))
            : width;
        var (sliceStart, sliceLength) = ComputeSlice(line, wrap, availableWidth, xOffset);
        var isHighlighted = highlightVisualLine == visualIndex;

        if (!showLineNumbers)
        {
            if (!isHighlighted)
            {
                return Slice(line, sliceStart, sliceLength);
            }

            return string.Create(
                2 + sliceLength,
                (line, sliceStart, sliceLength),
                static (destination, state) =>
                {
                    destination[0] = '>';
                    destination[1] = ' ';
                    if (state.sliceLength > 0)
                    {
                        state.line.AsSpan(state.sliceStart, state.sliceLength).CopyTo(destination[2..]);
                    }
                });
        }

        var prefixLength = lineNumberWidth + 2;
        if (prefixLength >= width)
        {
            return BuildPrefixOnly(visualIndex, lineNumberWidth, isHighlighted, width);
        }

        var contentLength = Math.Min(sliceLength, width - prefixLength);
        return string.Create(
            prefixLength + contentLength,
            (line, sliceStart, contentLength, visualIndex, lineNumberWidth, isHighlighted),
            static (destination, state) =>
            {
                WritePrefix(destination, state.visualIndex, state.lineNumberWidth, state.isHighlighted);
                if (state.contentLength > 0)
                {
                    state.line.AsSpan(state.sliceStart, state.contentLength)
                        .CopyTo(destination[(state.lineNumberWidth + 2)..]);
                }
            });
    }

    public static string FormatNoDecoration(string line, bool wrap, int width, int xOffset)
    {
        var (sliceStart, sliceLength) = ComputeSlice(line, wrap, width, xOffset);
        return Slice(line, sliceStart, sliceLength);
    }

    public static List<string> NormalizeContentLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return [string.Empty];
        }

        var lines = new List<string>();
        var start = 0;
        var index = 0;
        while (index < content.Length)
        {
            var current = content[index];
            if (current != '\n' && current != '\r')
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

    public static string NormalizeInlineLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return string.Empty;
        }

        var source = line.AsSpan();
        var firstBreak = source.IndexOfAny('\r', '\n');
        if (firstBreak < 0)
        {
            return line;
        }

        return string.Create(
            source.Length,
            line,
            static (destination, state) =>
            {
                for (var index = 0; index < state.Length; index++)
                {
                    var current = state[index];
                    destination[index] = current is '\r' or '\n' ? ' ' : current;
                }
            });
    }

    private static string Slice(string line, int start, int length)
    {
        if (length <= 0)
        {
            return string.Empty;
        }

        if (start == 0 && length == line.Length)
        {
            return line;
        }

        return string.Create(
            length,
            (line, start, length),
            static (destination, state) => state.line.AsSpan(state.start, state.length).CopyTo(destination));
    }

    private static (int Start, int Length) ComputeSlice(string line, bool wrap, int availableWidth, int xOffset)
    {
        if (availableWidth <= 0)
        {
            return (0, 0);
        }

        if (wrap)
        {
            return (0, Math.Min(line.Length, availableWidth));
        }

        if (xOffset >= line.Length)
        {
            return (0, 0);
        }

        if (xOffset == 0 && line.Length <= availableWidth)
        {
            return (0, line.Length);
        }

        var remaining = line.Length - xOffset;
        var length = Math.Min(availableWidth, remaining);
        return (xOffset, length);
    }

    private static string BuildPrefixOnly(int visualIndex, int lineNumberWidth, bool highlighted, int width)
    {
        if (width <= 0)
        {
            return string.Empty;
        }

        Span<char> prefix = stackalloc char[lineNumberWidth + 2];
        WritePrefix(prefix, visualIndex, lineNumberWidth, highlighted);
        return new string(prefix[..Math.Min(width, prefix.Length)]);
    }

    private static void WritePrefix(Span<char> destination, int visualIndex, int lineNumberWidth, bool highlighted)
    {
        destination.Fill(' ');
        var numeric = destination[..Math.Min(lineNumberWidth, destination.Length)];
        if (!numeric.IsEmpty)
        {
            WriteLineNumber(numeric, visualIndex + 1);
        }

        if (lineNumberWidth < destination.Length)
        {
            destination[lineNumberWidth] = highlighted ? '>' : ' ';
        }

        if (lineNumberWidth + 1 < destination.Length)
        {
            destination[lineNumberWidth + 1] = ' ';
        }
    }

    private static void WriteLineNumber(Span<char> destination, int oneBasedValue)
    {
        destination.Fill(' ');
        Span<char> digits = stackalloc char[11];
        oneBasedValue.TryFormat(digits, out var digitsWritten, provider: InvariantCulture);
        digits[..digitsWritten].CopyTo(destination[(destination.Length - digitsWritten)..]);
    }
}
