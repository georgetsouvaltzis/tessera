using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Styles;

namespace TeaSharp.Controls.Internal;

internal static class ControlTextLayout
{
    public static string[] SplitLines(string text)
    {
        var value = text ?? string.Empty;
        if (value.IndexOf('\r', StringComparison.Ordinal) < 0)
        {
            return value.Split('\n');
        }

        return NormalizeCarriageReturns(value).Split('\n');
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
        return TeaStyle.Empty.WithInverse().WithBold().Render(text);
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

    private static string NormalizeCarriageReturns(string text)
    {
        var builder = new System.Text.StringBuilder(text.Length);
        for (var index = 0; index < text.Length; index++)
        {
            var value = text[index];
            if (value == '\r')
            {
                if (index + 1 < text.Length && text[index + 1] == '\n')
                {
                    continue;
                }

                builder.Append('\n');
                continue;
            }

            builder.Append(value);
        }

        return builder.ToString();
    }
}
