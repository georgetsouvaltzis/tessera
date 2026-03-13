using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

/// <summary>
/// Represents a common screen shell split into header, body, and footer regions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct ScreenFrameLayout(Rect Bounds, Rect Header, Rect Body, Rect Footer)
{
    /// <summary>
    /// Gets a value indicating whether the frame contains a header region.
    /// </summary>
    public bool HasHeader => !Header.IsEmpty;

    /// <summary>
    /// Gets a value indicating whether the frame contains a footer region.
    /// </summary>
    public bool HasFooter => !Footer.IsEmpty;

    /// <summary>
    /// Splits the body into left and right columns.
    /// </summary>
    public (Rect Left, Rect Right) SplitBodyColumns(int leftWidth, int minLeft = 16, int minRight = 16) =>
        SplitVertical(Body, leftWidth, minLeft, minRight);

    /// <summary>
    /// Splits the body into top and bottom rows.
    /// </summary>
    public (Rect Top, Rect Bottom) SplitBodyRows(int topHeight, int minTop = 4, int minBottom = 4) =>
        SplitHorizontal(Body, topHeight, minTop, minBottom);

    private static (Rect Left, Rect Right) SplitVertical(Rect rect, int leftWidth, int minLeft, int minRight)
    {
        var clippedWidth = Math.Max(0, rect.Width);
        if (clippedWidth == 0)
        {
            return (new Rect(rect.X, rect.Y, 0, rect.Height), new Rect(rect.X, rect.Y, 0, rect.Height));
        }

        var safeMinLeft = Math.Clamp(minLeft, 0, clippedWidth);
        var maxRight = Math.Max(0, clippedWidth - safeMinLeft);
        var safeMinRight = Math.Clamp(minRight, 0, maxRight);
        var safeLeft = Math.Clamp(leftWidth, safeMinLeft, clippedWidth - safeMinRight);
        var left = new Rect(rect.X, rect.Y, safeLeft, rect.Height);
        var right = new Rect(rect.X + safeLeft, rect.Y, Math.Max(0, rect.Width - safeLeft), rect.Height);
        return (left, right);
    }

    private static (Rect Top, Rect Bottom) SplitHorizontal(Rect rect, int topHeight, int minTop, int minBottom)
    {
        var clippedHeight = Math.Max(0, rect.Height);
        if (clippedHeight == 0)
        {
            return (new Rect(rect.X, rect.Y, rect.Width, 0), new Rect(rect.X, rect.Y, rect.Width, 0));
        }

        var safeMinTop = Math.Clamp(minTop, 0, clippedHeight);
        var maxBottom = Math.Max(0, clippedHeight - safeMinTop);
        var safeMinBottom = Math.Clamp(minBottom, 0, maxBottom);
        var safeTop = Math.Clamp(topHeight, safeMinTop, clippedHeight - safeMinBottom);
        var top = new Rect(rect.X, rect.Y, rect.Width, safeTop);
        var bottom = new Rect(rect.X, rect.Y + safeTop, rect.Width, Math.Max(0, rect.Height - safeTop));
        return (top, bottom);
    }
}
