using Tessera.Components.Primitives;

namespace Tessera.Layout;

internal static class LayoutArrangement
{
    public static LayoutMeasurement MeasureStack(
        bool horizontal,
        IReadOnlyList<LayoutSlot> children,
        int gap,
        Thickness padding,
        in Rect availableBounds)
    {
        var totalPrimary = padding.Horizontal;
        var maxCross = 0;

        if (!horizontal)
        {
            totalPrimary = padding.Vertical;
        }

        for (var index = 0; index < children.Count; index++)
        {
            var child = children[index];
            var measured = child.Content.Measure(availableBounds);
            var primary = horizontal ? measured.Width : measured.Height;
            var cross = horizontal ? measured.Height : measured.Width;
            var marginPrimary = horizontal ? child.Margin.Horizontal : child.Margin.Vertical;
            var marginCross = horizontal ? child.Margin.Vertical : child.Margin.Horizontal;
            totalPrimary += primary + marginPrimary;
            maxCross = Math.Max(maxCross, cross + marginCross);
        }

        if (children.Count > 1)
        {
            totalPrimary += gap * (children.Count - 1);
        }

        var width = horizontal
            ? totalPrimary
            : maxCross + padding.Horizontal;
        var height = horizontal
            ? maxCross + padding.Vertical
            : totalPrimary;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    public static LayoutMeasurement MeasureDock(DockLayout layout, in Rect availableBounds)
    {
        var inner = Rect.Intersect(availableBounds.Inset(layout.Padding), availableBounds);
        var top = layout.Top?.Content.Measure(inner).Height ?? 0;
        var bottom = layout.Bottom?.Content.Measure(inner).Height ?? 0;
        var left = layout.Left?.Content.Measure(inner).Width ?? 0;
        var right = layout.Right?.Content.Measure(inner).Width ?? 0;
        var fill = layout.Fill?.Content.Measure(inner) ?? default;
        var width = Math.Max(fill.Width + left + right, Math.Max(
            layout.Top?.Content.Measure(inner).Width ?? 0,
            layout.Bottom?.Content.Measure(inner).Width ?? 0));
        var height = top + bottom + Math.Max(fill.Height, Math.Max(
            layout.Left?.Content.Measure(inner).Height ?? 0,
            layout.Right?.Content.Measure(inner).Height ?? 0));
        return new LayoutMeasurement(
            Math.Clamp(width + layout.Padding.Horizontal, 0, availableBounds.Width),
            Math.Clamp(height + layout.Padding.Vertical, 0, availableBounds.Height));
    }

    private static int ResolveSlotExtent(LayoutSlot slot, bool horizontal, in Rect availableBounds)
    {
        var marginPrimary = horizontal ? slot.Margin.Horizontal : slot.Margin.Vertical;
        var measured = slot.Content.Measure(availableBounds);
        var availablePrimary = horizontal ? availableBounds.Width : availableBounds.Height;
        var measuredPrimary = horizontal ? measured.Width : measured.Height;

        var content = slot.Length.Kind switch
        {
            LayoutLengthKind.Fixed => slot.Length.Value,
            LayoutLengthKind.Weighted => Math.Max(0, (availablePrimary - marginPrimary) * slot.Length.Value),
            LayoutLengthKind.Fill => Math.Max(0, availablePrimary - marginPrimary),
            _ => measuredPrimary,
        };

        return Math.Clamp(content + marginPrimary, 0, availablePrimary);
    }

}
