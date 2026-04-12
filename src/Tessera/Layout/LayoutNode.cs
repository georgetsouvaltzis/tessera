using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Layout;

/// <summary>
/// Base type for public layout nodes composed through the Tessera layout facade.
/// </summary>
public abstract class LayoutNode
{
    /// <summary>
    /// Converts a control into layout content on the default path.
    /// </summary>
    /// <param name="control">The control to wrap as layout content.</param>
    public static implicit operator LayoutNode(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);
        return new ComponentLayout(control);
    }

    internal abstract LayoutMeasurement Measure(in Rect availableBounds);
}
