using System.ComponentModel;
using Tessera.Widgets.Internal;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class HelpView
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

    private static string[] ToChunks(IEnumerable<KeyBinding> bindings)
    {
        var chunks = bindings is ICollection<KeyBinding> collection
            ? new List<string>(collection.Count)
            : [];

        foreach (var binding in bindings)
        {
            chunks.Add(string.Concat(binding.Keys, " ", binding.Description));
        }

        return [.. chunks];
    }
}
