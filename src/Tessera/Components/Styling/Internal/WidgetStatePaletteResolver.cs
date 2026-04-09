using Tessera.Styles;

namespace Tessera.Components.Styling.Internal;

internal static class WidgetStatePaletteResolver
{
    public static bool TryResolveAppearance(
        WidgetVisualState state,
        IReadOnlyList<WidgetStatePalette> hierarchy,
        Func<WidgetStatePalette, WidgetVisualState, WidgetStateAppearance?> resolver,
        out WidgetStateAppearance appearance)
    {
        var found = false;
        var style = TesseraStyle.Empty;
        var upper = false;
        var prefix = string.Empty;
        var suffix = string.Empty;

        for (var i = 0; i < hierarchy.Count; i++)
        {
            var local = resolver(hierarchy[i], state);
            if (local is null)
            {
                continue;
            }

            found = true;
            style = style.Merge(local.TextStyle);
            upper |= local.Uppercase;
            if (!string.IsNullOrEmpty(local.Prefix))
            {
                prefix = local.Prefix;
            }

            if (!string.IsNullOrEmpty(local.Suffix))
            {
                suffix = local.Suffix;
            }
        }

        if (!found)
        {
            appearance = null!;
            return false;
        }

        appearance = new WidgetStateAppearance
        {
            TextStyle = style,
            Uppercase = upper,
            Prefix = prefix,
            Suffix = suffix,
        };
        return true;
    }

    public static string Render(
        string text,
        IEnumerable<WidgetVisualState> activeStates,
        IReadOnlyList<WidgetVisualState> priority,
        IReadOnlyList<WidgetStatePalette> hierarchy,
        Func<WidgetStatePalette, WidgetVisualState, WidgetStateAppearance?> resolver)
    {
        var source = text ?? string.Empty;
        var active = new HashSet<WidgetVisualState>(activeStates);

        var style = TesseraStyle.Empty;
        var upper = false;
        var prefix = string.Empty;
        var suffix = string.Empty;

        if (TryResolveAppearance(WidgetVisualState.Default, hierarchy, resolver, out var defaults))
        {
            style = style.Merge(defaults.TextStyle);
            upper |= defaults.Uppercase;
            if (!string.IsNullOrEmpty(defaults.Prefix))
            {
                prefix = defaults.Prefix;
            }

            if (!string.IsNullOrEmpty(defaults.Suffix))
            {
                suffix = defaults.Suffix;
            }
        }

        foreach (var state in priority)
        {
            if (!active.Contains(state))
            {
                continue;
            }

            if (!TryResolveAppearance(state, hierarchy, resolver, out var appearance))
            {
                continue;
            }

            style = style.Merge(appearance.TextStyle);
            upper |= appearance.Uppercase;
            if (string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(appearance.Prefix))
            {
                prefix = appearance.Prefix;
            }

            if (string.IsNullOrEmpty(suffix) && !string.IsNullOrEmpty(appearance.Suffix))
            {
                suffix = appearance.Suffix;
            }
        }

        if (upper)
        {
            source = source.ToUpperInvariant();
        }

        return style.Render($"{prefix}{source}{suffix}");
    }
}
