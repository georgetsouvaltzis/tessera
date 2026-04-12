using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Styles;

namespace Tessera.Controls.Internal;

internal static class ControlTextLayout
{
    public static string[] SplitLines(string text)
    {
        var value = text ?? string.Empty;
        if (value.IndexOf('\r', StringComparison.Ordinal) < 0)
        {
            return value.Split('\n');
        }

        return SplitLinesWithCarriageNormalization(value);
    }

    public static int MeasureDisplayWidth(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        if (IsPlainAscii(text))
        {
            return text.Length;
        }

        var width = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            if (CanvasAnsiScanner.TryReadEscape(element, 0, out _))
            {
                continue;
            }

            width += TextElementWidth.Measure(element);
        }

        return width;
    }

    public static void WriteCentered(Canvas canvas, Rect content, int y, string text)
    {
        if (y < content.Y || y > content.Bottom)
        {
            return;
        }

        var displayWidth = MeasureDisplayWidth(text);
        var x = content.X;
        var width = content.Width;
        if (displayWidth < content.Width)
        {
            var offset = (content.Width - displayWidth) / 2;
            x += offset;
            width -= offset;
        }

        canvas.WriteText(x, y, text, width);
    }

    public static string ApplyPressedStyle(string text)
    {
        return TesseraStyle.Empty.WithInverse().WithBold().Render(text);
    }

    private static bool IsPlainAscii(string text)
    {
        for (var index = 0; index < text.Length; index++)
        {
            var value = text[index];
            if (value < '\u0020' || value > '\u007e')
            {
                return false;
            }
        }

        return true;
    }

    private static string[] SplitLinesWithCarriageNormalization(string value)
    {
        var estimatedLines = 1;
        for (var scanIndex = 0; scanIndex < value.Length; scanIndex++)
        {
            var current = value[scanIndex];
            if (current is '\n' or '\r')
            {
                estimatedLines++;
            }
        }

        var lines = new List<string>(estimatedLines);
        var start = 0;
        var index = 0;
        while (index < value.Length)
        {
            var current = value[index];
            if (current is not ('\n' or '\r'))
            {
                index++;
                continue;
            }

            lines.Add(value[start..index]);
            if (current == '\r' && index + 1 < value.Length && value[index + 1] == '\n')
            {
                index++;
            }

            start = index + 1;
            index++;
        }

        lines.Add(value[start..]);
        return [.. lines];
    }
}
