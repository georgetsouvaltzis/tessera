using TeaSharp.Components.Primitives;
using System.ComponentModel;
using TeaSharp.Components.Composition;

namespace TeaSharp.Layout;

/// <summary>
/// Creates deterministic layout slots without exposing geometry math to app code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class Slot
{
    /// <summary>
    /// Creates an auto-sized slot from an existing layout node.
    /// </summary>
    public static LayoutSlot Auto(LayoutNode content, Thickness margin = default) =>
        new(content, LayoutLength.Auto(), margin);

    /// <summary>
    /// Creates an auto-sized slot from a TeaSharp component.
    /// </summary>
    public static LayoutSlot Auto(
        ICanvasComponent component,
        int? preferredWidth = null,
        int? preferredHeight = null,
        Thickness margin = default,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        new(
            new ComponentLayout(component, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            LayoutLength.Auto(),
            margin);

    /// <summary>
    /// Creates a fixed-size slot from an existing layout node.
    /// </summary>
    public static LayoutSlot Fixed(int size, LayoutNode content, Thickness margin = default) =>
        new(content, LayoutLength.Fixed(size), margin);

    /// <summary>
    /// Creates a fixed-size slot from a TeaSharp component.
    /// </summary>
    public static LayoutSlot Fixed(
        int size,
        ICanvasComponent component,
        int? preferredWidth = null,
        int? preferredHeight = null,
        Thickness margin = default,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        new(
            new ComponentLayout(component, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            LayoutLength.Fixed(size),
            margin);

    /// <summary>
    /// Creates a fill slot from an existing layout node.
    /// </summary>
    public static LayoutSlot Fill(LayoutNode content, Thickness margin = default) =>
        new(content, LayoutLength.Fill(), margin);

    /// <summary>
    /// Creates a fill slot from a TeaSharp component.
    /// </summary>
    public static LayoutSlot Fill(
        ICanvasComponent component,
        int? preferredWidth = null,
        int? preferredHeight = null,
        Thickness margin = default,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        new(
            new ComponentLayout(component, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            LayoutLength.Fill(),
            margin);

    /// <summary>
    /// Creates a weighted slot from an existing layout node.
    /// </summary>
    public static LayoutSlot Weighted(int weight, LayoutNode content, Thickness margin = default) =>
        new(content, LayoutLength.Weighted(weight), margin);

    /// <summary>
    /// Creates a weighted slot from a TeaSharp component.
    /// </summary>
    public static LayoutSlot Weighted(
        int weight,
        ICanvasComponent component,
        int? preferredWidth = null,
        int? preferredHeight = null,
        Thickness margin = default,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        new(
            new ComponentLayout(component, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            LayoutLength.Weighted(weight),
            margin);
}
