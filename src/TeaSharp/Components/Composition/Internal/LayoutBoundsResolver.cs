using TeaSharp.Layout;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Components.Composition.Internal;

internal static class LayoutBoundsResolver
{
    public static IReadOnlyDictionary<ScreenRegionKey, Rect> Resolve(LayoutNode layout, Rect bounds)
    {
        var screen = new ScreenComposer();
        screen.BeginFrame();
        screen.Compose(layout, bounds);
        screen.CompleteFrame();

        var resolved = new Dictionary<ScreenRegionKey, Rect>(screen.Regions.Count);
        foreach (var region in screen.Regions)
        {
            resolved[region.Id] = region.Bounds;
        }

        return resolved;
    }

    public static ICanvasComponent Placeholder() => PlaceholderComponent.Instance;

    private sealed class PlaceholderComponent : ICanvasComponent
    {
        public static readonly PlaceholderComponent Instance = new();

        public void Render(Canvas canvas, Rect rect)
        {
        }
    }
}
