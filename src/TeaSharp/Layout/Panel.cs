using TeaSharp.Components.Primitives;
using TeaSharp.Components.Composition;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Creates grouped panel layouts with borders, padding, and nested content.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class Panel
{
    /// <summary>
    /// Wraps an existing layout node in panel chrome.
    /// </summary>
    public static PanelLayout Item(
        LayoutNode content,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default) =>
        new(content, title, border, padding, margin);

    /// <summary>
    /// Wraps an existing TeaSharp component in panel chrome.
    /// </summary>
    public static PanelLayout Item(
        ICanvasComponent component,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default,
        int? preferredWidth = null,
        int? preferredHeight = null,
        bool? focusable = null,
        bool focusOnClick = true,
        bool interceptsPointer = true,
        int layer = 0,
        Action? onFocus = null) =>
        Item(
            new ComponentLayout(component, preferredWidth, preferredHeight, focusable, focusOnClick, interceptsPointer, layer, onFocus),
            title,
            border,
            padding,
            margin);

    /// <summary>
    /// Creates a horizontal panel around a row layout.
    /// </summary>
    public static PanelLayout Row(
        IReadOnlyList<LayoutSlot> children,
        int gap = 0,
        Thickness innerPadding = default,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default) =>
        Item(new StackLayout(true, children, gap, innerPadding), title, border, padding, margin);

    /// <summary>
    /// Creates a vertical panel around a column layout.
    /// </summary>
    public static PanelLayout Column(
        IReadOnlyList<LayoutSlot> children,
        int gap = 0,
        Thickness innerPadding = default,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default) =>
        Item(new StackLayout(false, children, gap, innerPadding), title, border, padding, margin);

    /// <summary>
    /// Creates a horizontal panel around a row layout.
    /// </summary>
    public static PanelLayout Row(
        int gap = 0,
        Thickness innerPadding = default,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default,
        params LayoutSlot[] children) =>
        Row((IReadOnlyList<LayoutSlot>)children, gap, innerPadding, title, border, padding, margin);

    /// <summary>
    /// Creates a vertical panel around a column layout.
    /// </summary>
    public static PanelLayout Column(
        int gap = 0,
        Thickness innerPadding = default,
        string? title = null,
        BorderStyle border = BorderStyle.None,
        Thickness padding = default,
        Thickness margin = default,
        params LayoutSlot[] children) =>
        Column((IReadOnlyList<LayoutSlot>)children, gap, innerPadding, title, border, padding, margin);
}
