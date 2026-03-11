using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt.Internal;

internal static class TextInputRenderer
{
    public static void Render(
        Canvas canvas,
        Rect rect,
        TextInputModel input,
        string title,
        bool focused,
        bool showBorder,
        int submitCount)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (showBorder)
        {
            canvas.DrawBox(clipped, focused ? $"{title} *" : title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        var frame = input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, frame.Text, content.Width);
        if (content.Height > 1)
        {
            var submitted = submitCount == 0
                ? "submit: -"
                : $"submit: {submitCount}";
            canvas.WriteText(content.X, content.Y + 1, submitted, content.Width);
        }
    }
}
