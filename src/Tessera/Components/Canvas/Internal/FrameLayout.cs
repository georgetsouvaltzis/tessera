using Tessera.Components.Primitives;

namespace Tessera.Components.Primitives.Internal;

internal static class FrameLayout
{
    public static Rect ResolveInnerRect(Rect bounds, BorderStyle border)
    {
        return border == BorderStyle.None
            ? bounds
            : bounds.Inset(1, 1);
    }

    public static Rect ResolveContentRect(Rect bounds, BorderStyle border, Thickness padding)
    {
        return ResolveInnerRect(bounds, border).Inset(padding);
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
        global::Tessera.Styles.TesseraStyle borderStyleText)
    {
        if (border != BorderStyle.None)
        {
            canvas.DrawBox(bounds, title, border, borderStyleText);
        }

        return ResolveContentRect(bounds, border, padding);
    }
}
