using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;
using System.ComponentModel;

namespace TeaSharp.Components.UiKit;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class Layout
{
    public static ViewportClass Classify(int width)
    {
        return width switch
        {
            < 80 => ViewportClass.Xs,
            < 110 => ViewportClass.Sm,
            < 150 => ViewportClass.Md,
            < 190 => ViewportClass.Lg,
            _ => ViewportClass.Xl,
        };
    }

    public static (Rect First, Rect Second) SplitVertical(Rect rect, int firstWidth, int minFirst = 8, int minSecond = 8)
    {
        var clippedWidth = Math.Max(0, rect.Width);
        if (clippedWidth == 0)
        {
            return (new Rect(rect.X, rect.Y, 0, rect.Height), new Rect(rect.X, rect.Y, 0, rect.Height));
        }

        var safeMinFirst = Math.Clamp(minFirst, 0, clippedWidth);
        var maxSecond = Math.Max(0, clippedWidth - safeMinFirst);
        var safeMinSecond = Math.Clamp(minSecond, 0, maxSecond);
        var safeFirst = Math.Clamp(firstWidth, safeMinFirst, clippedWidth - safeMinSecond);
        var first = new Rect(rect.X, rect.Y, safeFirst, rect.Height);
        var second = new Rect(rect.X + safeFirst, rect.Y, Math.Max(0, rect.Width - safeFirst), rect.Height);
        return (first, second);
    }

    public static (Rect First, Rect Second) SplitHorizontal(Rect rect, int firstHeight, int minFirst = 4, int minSecond = 4)
    {
        var clippedHeight = Math.Max(0, rect.Height);
        if (clippedHeight == 0)
        {
            return (new Rect(rect.X, rect.Y, rect.Width, 0), new Rect(rect.X, rect.Y, rect.Width, 0));
        }

        var safeMinFirst = Math.Clamp(minFirst, 0, clippedHeight);
        var maxSecond = Math.Max(0, clippedHeight - safeMinFirst);
        var safeMinSecond = Math.Clamp(minSecond, 0, maxSecond);
        var safeFirst = Math.Clamp(firstHeight, safeMinFirst, clippedHeight - safeMinSecond);
        var first = new Rect(rect.X, rect.Y, rect.Width, safeFirst);
        var second = new Rect(rect.X, rect.Y + safeFirst, rect.Width, Math.Max(0, rect.Height - safeFirst));
        return (first, second);
    }

    public static Rect[] Grid(Rect rect, int rows, int columns)
    {
        var safeRows = Math.Max(1, rows);
        var safeCols = Math.Max(1, columns);
        var result = new Rect[safeRows * safeCols];
        var totalWidth = Math.Max(0, rect.Width);
        var totalHeight = Math.Max(0, rect.Height);

        var baseWidth = totalWidth / safeCols;
        var widthRemainder = totalWidth % safeCols;
        var baseHeight = totalHeight / safeRows;
        var heightRemainder = totalHeight % safeRows;

        var y = rect.Y;
        for (var row = 0; row < safeRows; row++)
        {
            var h = baseHeight + (row < heightRemainder ? 1 : 0);
            var x = rect.X;
            for (var col = 0; col < safeCols; col++)
            {
                var w = baseWidth + (col < widthRemainder ? 1 : 0);
                result[(row * safeCols) + col] = new Rect(x, y, w, h);
                x += w;
            }

            y += h;
        }

        return result;
    }
}
