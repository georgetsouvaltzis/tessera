namespace TeaSharp.Components.Productivity.Internal;

internal static class NumberInputStateResolver
{
    public static IReadOnlyCollection<WidgetVisualState> Resolve(bool focused, bool disabled, bool readOnly)
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

        return states;
    }
}
