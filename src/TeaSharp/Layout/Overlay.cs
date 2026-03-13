using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Creates overlay compositions using back-to-front layout items.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class Overlay
{
    /// <summary>
    /// Creates an overlay from the provided items.
    /// </summary>
    public static OverlayLayout Items(params LayoutNode[] items) => new(items);
}
