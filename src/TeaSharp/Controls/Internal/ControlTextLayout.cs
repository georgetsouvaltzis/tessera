using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Styles;

namespace TeaSharp.Controls.Internal;

internal static class ControlTextLayout
{
    public static string[] SplitLines(string text)
    {
        return (text ?? string.Empty)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');
    }

    public static int MeasureDisplayWidth(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        var width = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            if (CanvasAnsiScanner.TryReadEscape(element, 0, out _, out _))
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
}
