using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Layout;

/// <summary>
/// Base type for public layout nodes composed through the TeaSharp layout facade.
/// </summary>
public abstract class LayoutNode
{
    /// <summary>
    /// Converts a control into layout content on the default path.
    /// </summary>
    /// <param name="control">The control to wrap as layout content.</param>
    public static implicit operator LayoutNode(Control control)
        => new ComponentLayout(control ?? throw new ArgumentNullException(nameof(control)));

    internal abstract LayoutMeasurement Measure(in Rect availableBounds);

    internal abstract void Compose(ScreenComposer screen, in Rect bounds, string path);
}
