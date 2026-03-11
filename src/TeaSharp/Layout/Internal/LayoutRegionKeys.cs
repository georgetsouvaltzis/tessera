using TeaSharp.Components.Composition;

namespace TeaSharp.Layout;

internal static class LayoutRegionKeys
{
    public static ScreenRegionKey Generated(string path, string suffix)
    {
        var safePath = path
            .Replace("/", ":", StringComparison.Ordinal)
            .Replace(" ", "-", StringComparison.Ordinal);
        return new ScreenRegionKey($"layout:{safePath}:{suffix}");
    }
}
