using TeaSharp.Components.Primitives;

namespace TeaSharp.Components.Primitives.Internal;

internal static class FrameLayout
{
    public static Rect ResolveContentRect(Rect bounds, BorderStyle border, Thickness padding)
    {
        var content = border == BorderStyle.None
            ? bounds
            : bounds.Inset(1, 1);
        return content.Inset(padding);
    }

    public static Rect DrawFrameAndResolveContent(Canvas canvas, Rect bounds, string? title, BorderStyle border, Thickness padding)
    {
        if (border != BorderStyle.None)
        {
            canvas.DrawBox(bounds, title, border);
        }

        return ResolveContentRect(bounds, border, padding);
    }

    public static Rect DrawFrameAndResolveContent(
        Canvas canvas,
        Rect bounds,
        string? title,
        BorderStyle border,
        Thickness padding,
        TeaSharp.Styles.TeaStyle borderStyleText)
    {
        if (border != BorderStyle.None)
        {
            canvas.DrawBox(bounds, title, border, borderStyleText);
        }

        return ResolveContentRect(bounds, border, padding);
    }
}
