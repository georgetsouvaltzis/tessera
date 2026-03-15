using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Controls.Internal;

internal static class NumberInputRenderer
{
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
            border == BorderStyle.None ? null : focused ? $"{title} *" : title,
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
