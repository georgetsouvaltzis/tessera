using Tessera.Components.Composition;
using Tessera.Components.Primitives;

namespace Tessera.Layout;

internal static class LayoutIntrinsicMeasurer
{
    public static LayoutMeasurement Measure(ICanvasComponent component, in Rect availableBounds)
    {
        return new LayoutMeasurement(availableBounds.Width, availableBounds.Height);
    }
}
