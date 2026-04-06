using Tessera.Components.Primitives;
using Tessera.Components.Styling;
namespace Tessera.Controls.Internal;

internal static class DatePickerStateResolver
{
    public static IReadOnlyCollection<WidgetVisualState> ResolveDayStates(bool focused, DateOnly selectedDate, DateOnly? hoveredDate, DateOnly date)
    {
        var states = new List<WidgetVisualState>(5);
        if (date == selectedDate)
        {
            states.Add(WidgetVisualState.Selected);
            states.Add(WidgetVisualState.Cursor);
        }

        if (focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (hoveredDate.HasValue && hoveredDate.Value == date)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }
}
