using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Components.Styling;

namespace Tessera.Controls.Internal;

internal static class TimePickerRenderer
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
        string title,
        bool focused,
        bool disabled,
        bool readOnly,
        BorderStyle border,
        Thickness padding,
        TimeOnly value,
        TimeField activeField,
        TimeField? hoveredField,
        WidgetStatePalette fieldStatePalette)
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

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var hour = RenderField(value.Hour.ToString("D2", CultureInfo.InvariantCulture), TimeField.Hour, focused,
            disabled, readOnly, activeField, hoveredField, fieldStatePalette);
        var minute = RenderField(value.Minute.ToString("D2", CultureInfo.InvariantCulture), TimeField.Minute, focused,
            disabled, readOnly, activeField, hoveredField, fieldStatePalette);
        var second = RenderField(value.Second.ToString("D2", CultureInfo.InvariantCulture), TimeField.Second, focused,
            disabled, readOnly, activeField, hoveredField, fieldStatePalette);
        canvas.WriteText(content.X, content.Y, $"{hour}:{minute}:{second}", content.Width);
    }

    private static string RenderField(
        string value,
        TimeField field,
        bool focused,
        bool disabled,
        bool readOnly,
        TimeField activeField,
        TimeField? hoveredField,
        WidgetStatePalette fieldStatePalette)
    {
        var states =
            TimePickerStateResolver.ResolveFieldStates(focused, disabled, readOnly, activeField, hoveredField, field);
        return fieldStatePalette.Render(value, states);
    }
}
