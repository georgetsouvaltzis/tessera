using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Styles;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Creates centered layouts without exposing manual geometry math.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class Center
{
    /// <summary>
    /// Centers an existing layout node.
    /// </summary>
    public static CenterLayout Item(LayoutNode content, int? width = null, int? height = null, Thickness margin = default) =>
        new(content, width, height, margin);

    /// <summary>
    /// Centers an existing TeaSharp component.
    /// </summary>
    public static CenterLayout Item(
        ICanvasComponent component,
        ScreenRegionKey? regionKey = null,
        int? width = null,
        int? height = null,
        Thickness margin = default,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = (int)ScreenLayer.Base,
        Action? onFocus = null) =>
        new(
            new ComponentLayout(component, regionKey, width, height, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            width,
            height,
            margin);

    /// <summary>
    /// Centers styled text using a text block measured by its intrinsic content size.
    /// </summary>
    public static CenterLayout Text(
        string text,
        TeaStyle style = default,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default) =>
        Item(
            new TextBlockComponent(new TextBlockOptions(
                Text: text,
                Title: title,
                Border: border,
                Padding: padding,
                TextStyle: style)),
            width: null,
            height: null,
            margin: margin);
}
