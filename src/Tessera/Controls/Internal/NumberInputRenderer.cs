using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Components.Styling;
using Tessera.Widgets;

namespace Tessera.Controls.Internal;

internal static class NumberInputRenderer
{
    private static string? ResolveTitle(BorderStyle border, bool focused, string title)
    {
        if (border == BorderStyle.None)
        {
            return null;
        }

        return focused ? $"{title} *" : title;
    }

    public static void Render(
        Canvas canvas,
        Rect rect,
        TextInputModel input,
        WidgetStatePalette statePalette,
        string title,
        bool focused,
        bool disabled,
        bool readOnly,
        BorderStyle border,
        Thickness padding,
        double value,
        double min,
        double max,
        int precision)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            ResolveTitle(border, focused, title),
            border,
            padding);

        if (content.IsEmpty)
        {
            return;
        }

        var states = NumberInputStateResolver.Resolve(focused, disabled, readOnly);
        var frame = input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, statePalette.Render(frame.Text, states), content.Width);
        if (content.Height > 1)
        {
            var summary = $"value={NumberInputFormatting.Format(value, precision)} range=[{NumberInputFormatting.Format(min, precision)}, {NumberInputFormatting.Format(max, precision)}]";
            canvas.WriteText(content.X, content.Y + 1, statePalette.Render(summary, states), content.Width);
        }
    }
}
