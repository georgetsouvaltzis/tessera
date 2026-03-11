using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity.Internal;

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
        bool showBorder,
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
