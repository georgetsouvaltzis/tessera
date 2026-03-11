using System.Globalization;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;

namespace TeaSharp.Layout;

internal static class LayoutIntrinsicMeasurer
{
    public static LayoutMeasurement Measure(ICanvasComponent component, in Rect availableBounds)
    {
        return component switch
        {
            TextBlockComponent textBlock => MeasureTextBlock(textBlock, availableBounds),
            _ => new LayoutMeasurement(availableBounds.Width, availableBounds.Height),
        };
    }

    private static LayoutMeasurement MeasureTextBlock(TextBlockComponent textBlock, in Rect availableBounds)
    {
        var lines = SplitLines(textBlock.Text);
        var width = 0;
        for (var index = 0; index < lines.Length; index++)
        {
            width = Math.Max(width, MeasureDisplayWidth(lines[index]));
        }

        width += textBlock.Padding.Horizontal;
        var height = lines.Length + textBlock.Padding.Vertical;

        if (textBlock.Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            if (!string.IsNullOrWhiteSpace(textBlock.Title))
            {
                width = Math.Max(width, textBlock.Title!.Length + 4);
            }
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private static string[] SplitLines(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');
    }

    private static int MeasureDisplayWidth(string text)
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
}
