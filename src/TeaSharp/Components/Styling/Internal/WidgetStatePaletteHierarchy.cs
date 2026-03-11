namespace TeaSharp.Components.Styling.Internal;

internal static class WidgetStatePaletteHierarchy
{
    public static IReadOnlyList<WidgetStatePalette> BuildRootFirst(WidgetStatePalette palette)
    {
        var chain = new List<WidgetStatePalette>(4);
        var seen = new HashSet<WidgetStatePalette>();

        var current = palette;
        while (current is not null)
        {
            if (!seen.Add(current))
            {
                throw new InvalidOperationException("WidgetStatePalette parent cycle detected.");
            }

            chain.Add(current);
            current = current.Parent;
        }

        chain.Reverse();
        return chain;
    }

    public static void EnsureNoCycle(WidgetStatePalette owner, WidgetStatePalette? candidateParent)
    {
        var current = candidateParent;
        while (current is not null)
        {
            if (ReferenceEquals(current, owner))
            {
                throw new InvalidOperationException("WidgetStatePalette cannot inherit from itself (cycle detected).");
            }

            current = current.Parent;
        }
    }
}
