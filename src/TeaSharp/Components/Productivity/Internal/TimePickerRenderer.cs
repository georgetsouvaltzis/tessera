using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using System.Globalization;

namespace TeaSharp.Components.Productivity.Internal;

internal static class TimePickerRenderer
{
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
        TimePickerField activeField,
        TimePickerField? hoveredField,
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
            border == BorderStyle.None ? null : focused ? $"{title} *" : title,
            border,
            padding);

        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var hour = RenderField(value.Hour.ToString("D2", CultureInfo.InvariantCulture), TimePickerField.Hour, focused, disabled, readOnly, activeField, hoveredField, fieldStatePalette);
        var minute = RenderField(value.Minute.ToString("D2", CultureInfo.InvariantCulture), TimePickerField.Minute, focused, disabled, readOnly, activeField, hoveredField, fieldStatePalette);
        var second = RenderField(value.Second.ToString("D2", CultureInfo.InvariantCulture), TimePickerField.Second, focused, disabled, readOnly, activeField, hoveredField, fieldStatePalette);
        canvas.WriteText(content.X, content.Y, $"{hour}:{minute}:{second}", content.Width);
    }

    private static string RenderField(
        string value,
        TimePickerField field,
        bool focused,
        bool disabled,
        bool readOnly,
        TimePickerField activeField,
        TimePickerField? hoveredField,
        WidgetStatePalette fieldStatePalette)
    {
        var states = TimePickerStateResolver.ResolveFieldStates(focused, disabled, readOnly, activeField, hoveredField, field);
        return fieldStatePalette.Render(value, states);
    }
}
