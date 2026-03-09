using TeaSharp.Widgets.Internal;

namespace TeaSharp.Widgets;

public static class HelpView
{
    public static string RenderColumns(
        IEnumerable<KeyBinding> bindings,
        int maxWidth,
        int minColumnWidth = 24,
        int columnGap = 3)
    {
        return HelpViewLayout.RenderColumns(ToChunks(bindings), maxWidth, minColumnWidth, columnGap);
    }

    public static string RenderCompact(IEnumerable<KeyBinding> bindings, int maxWidth = 0)
    {
        return HelpViewLayout.RenderCompact(ToChunks(bindings), maxWidth);
    }

    private static IReadOnlyList<string> ToChunks(IEnumerable<KeyBinding> bindings)
    {
        return bindings.Select(static binding => $"{binding.Keys} {binding.Description}").ToArray();
    }
}
