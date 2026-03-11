using TeaSharp.Components.UiKit;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
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
        BorderStyle border,
        Thickness padding,
        int submitCount)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            border == BorderStyle.None ? null : focused ? $"{title} *" : title,
            border,
            padding);

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
