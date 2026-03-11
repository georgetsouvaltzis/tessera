using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Base type for public layout nodes composed through the TeaSharp layout facade.
/// </summary>
public abstract class LayoutNode
{
    internal abstract LayoutMeasurement Measure(in Rect availableBounds);

    internal abstract void Compose(ScreenComposer screen, in Rect bounds, string path);
}
