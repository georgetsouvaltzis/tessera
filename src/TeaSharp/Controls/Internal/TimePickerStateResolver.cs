using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
namespace TeaSharp.Controls.Internal;

internal static class TimePickerStateResolver
{
    public static IReadOnlyCollection<WidgetVisualState> ResolveFieldStates(
        bool focused,
        bool disabled,
        bool readOnly,
        TimeField activeField,
        TimeField? hoveredField,
        TimeField field)
    {
        var states = new List<WidgetVisualState>(5);
        if (focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (disabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (readOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (field == activeField)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (hoveredField.HasValue && hoveredField.Value == field)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }
}
