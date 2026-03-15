using TeaSharp.Components.Primitives;
using TeaSharp.Components.Composition;
using TeaSharp.Controls;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a layout leaf backed by a TeaSharp canvas component.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ComponentLayout : LayoutNode
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ComponentLayout(
        ICanvasComponent component,
        int? preferredWidth = null,
        int? preferredHeight = null)
    {
        CanvasComponent = component ?? throw new ArgumentNullException(nameof(component));
        PreferredWidth = preferredWidth;
        PreferredHeight = preferredHeight;
    }

    public ComponentLayout(Control control)
    {
        Control = control ?? throw new ArgumentNullException(nameof(control));
    }

    /// <summary>
    /// Gets the wrapped TeaSharp component.
    /// </summary>
    public ICanvasComponent? CanvasComponent { get; }

    internal Control? Control { get; }

    /// <summary>
    /// Gets the preferred width used when the layout needs an intrinsic measurement.
    /// </summary>
    public int? PreferredWidth { get; }

    /// <summary>
    /// Gets the preferred height used when the layout needs an intrinsic measurement.
    /// </summary>
    public int? PreferredHeight { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        if (PreferredWidth.HasValue || PreferredHeight.HasValue)
        {
            return new LayoutMeasurement(
                Math.Clamp(PreferredWidth ?? availableBounds.Width, 0, availableBounds.Width),
                Math.Clamp(PreferredHeight ?? availableBounds.Height, 0, availableBounds.Height));
        }

        return Control is not null
            ? Control.Measure(availableBounds)
            : LayoutIntrinsicMeasurer.Measure(CanvasComponent!, availableBounds);
    }
}
