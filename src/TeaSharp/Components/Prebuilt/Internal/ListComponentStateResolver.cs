using TeaSharp.Components.UiKit;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt.Internal;

internal static class ListComponentStateResolver
{
    public static IReadOnlyCollection<WidgetVisualState> ResolveBaseStates(bool focused, bool disabled, bool readOnly, bool isEmpty = false)
    {
        var states = new List<WidgetVisualState>(4);
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

        if (isEmpty)
        {
            states.Add(WidgetVisualState.Empty);
        }

        return states;
    }

    public static IReadOnlyCollection<WidgetVisualState> ResolveRowStates<T>(
        ListRow<T> visible,
        bool focused,
        bool disabled,
        bool readOnly,
        int? hoveredFilteredIndex,
        Func<T, IReadOnlyCollection<WidgetVisualState>?>? itemStateResolver)
    {
        var states = new List<WidgetVisualState>(6);
        states.AddRange(ResolveBaseStates(focused, disabled, readOnly));
        if (visible.Selected)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (hoveredFilteredIndex == visible.Index)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (itemStateResolver?.Invoke(visible.Item) is { } custom)
        {
            states.AddRange(custom);
        }

        return states;
    }
}
