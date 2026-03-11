using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Creates deterministic two-pane splits.
/// </summary>
public static class Split
{
    /// <summary>
    /// Splits the available bounds into left and right slots.
    /// </summary>
    public static SplitLayout Columns(LayoutSlot left, LayoutSlot right, int gap = 0, Thickness padding = default) =>
        new(true, left, right, gap, padding);

    /// <summary>
    /// Splits the available bounds into top and bottom slots.
    /// </summary>
    public static SplitLayout Rows(LayoutSlot top, LayoutSlot bottom, int gap = 0, Thickness padding = default) =>
        new(false, top, bottom, gap, padding);
}
