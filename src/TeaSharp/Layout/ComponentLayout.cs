using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Represents a layout leaf backed by a TeaSharp canvas component.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class ComponentLayout : LayoutNode
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ComponentLayout(
        ICanvasComponent component,
        ScreenRegionKey? regionKey,
        int? preferredWidth,
        int? preferredHeight,
        bool? focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        Action? onFocus)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
        RegionKey = regionKey;
        PreferredWidth = preferredWidth;
        PreferredHeight = preferredHeight;
        Focusable = focusable;
        FocusOnClick = focusOnClick;
        InterceptsPointer = interceptsPointer;
        Layer = layer;
        OnFocus = onFocus;
    }

    public ComponentLayout(ICanvasComponent component)
        : this(component, null, null, null, null, focusOnClick: true, interceptsPointer: true, layer: (int)ScreenLayer.Base, onFocus: null)
    {
    }

    /// <summary>
    /// Gets the wrapped TeaSharp component.
    /// </summary>
    public ICanvasComponent Component { get; }

    /// <summary>
    /// Gets the optional stable region key used when the component participates in screen routing.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ScreenRegionKey? RegionKey { get; }

    /// <summary>
    /// Gets the preferred width used when the layout needs an intrinsic measurement.
    /// </summary>
    public int? PreferredWidth { get; }

    /// <summary>
    /// Gets the preferred height used when the layout needs an intrinsic measurement.
    /// </summary>
    public int? PreferredHeight { get; }

    /// <summary>
    /// Gets the explicit focusability override, if provided.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool? Focusable { get; }

    /// <summary>
    /// Gets a value indicating whether mouse clicks should move focus into the component.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool FocusOnClick { get; }

    /// <summary>
    /// Gets a value indicating whether the component should intercept pointer input.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool InterceptsPointer { get; }

    /// <summary>
    /// Gets the target screen layer used for composition.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Layer { get; }

    /// <summary>
    /// Gets the optional callback raised when the region receives focus.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Action? OnFocus { get; }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        if (PreferredWidth.HasValue || PreferredHeight.HasValue)
        {
            return new LayoutMeasurement(
                Math.Clamp(PreferredWidth ?? availableBounds.Width, 0, availableBounds.Width),
                Math.Clamp(PreferredHeight ?? availableBounds.Height, 0, availableBounds.Height));
        }

        return LayoutIntrinsicMeasurer.Measure(Component, availableBounds);
    }

    internal override void Compose(ScreenComposer screen, in Rect bounds, string path)
    {
        if (bounds.IsEmpty)
        {
            return;
        }

        var regionKey = RegionKey ?? LayoutRegionKeys.Generated(path, "component");
        screen.AddComponent(regionKey, bounds, Component, Focusable, FocusOnClick, InterceptsPointer, Layer, OnFocus);
    }
}
