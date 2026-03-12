using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Layout;

/// <summary>
/// Creates docked layouts for common shell compositions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class Dock
{
    /// <summary>
    /// Creates a dock layout around optional top, bottom, left, right, and fill content.
    /// </summary>
    public static DockLayout Layout(
        LayoutSlot? top = null,
        LayoutSlot? bottom = null,
        LayoutSlot? left = null,
        LayoutSlot? right = null,
        LayoutSlot? fill = null,
        int gap = 0,
        Thickness padding = default) =>
        new(top, bottom, left, right, fill, gap, padding);
}
