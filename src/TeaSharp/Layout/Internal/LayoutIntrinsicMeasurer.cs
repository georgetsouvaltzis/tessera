using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

internal static class LayoutIntrinsicMeasurer
{
    public static LayoutMeasurement Measure(ICanvasComponent component, in Rect availableBounds)
    {
        return new LayoutMeasurement(availableBounds.Width, availableBounds.Height);
    }
}
