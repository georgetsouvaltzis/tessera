using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

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

    public static void ComposeStack(
        ScreenComposer screen,
        bool horizontal,
        IReadOnlyList<LayoutSlot> children,
        int gap,
        Thickness padding,
        in Rect bounds,
        string path)
    {
        var inner = Rect.Intersect(bounds.Inset(padding), bounds);
        if (inner.IsEmpty || children.Count == 0)
        {
            return;
        }

        var primaryAvailable = horizontal ? inner.Width : inner.Height;
        var crossAvailable = horizontal ? inner.Height : inner.Width;
        var gapTotal = children.Count > 1 ? gap * (children.Count - 1) : 0;
        var marginTotal = 0;
        var primarySizes = new int[children.Count];
        var flexibleWeights = new int[children.Count];
        var remaining = primaryAvailable - gapTotal;

        for (var index = 0; index < children.Count; index++)
        {
            var child = children[index];
            var marginPrimary = horizontal ? child.Margin.Horizontal : child.Margin.Vertical;
            marginTotal += marginPrimary;
        }

        remaining = Math.Max(0, remaining - marginTotal);

        for (var index = 0; index < children.Count; index++)
        {
            var child = children[index];
            switch (child.Length.Kind)
            {
                case LayoutLengthKind.Fixed:
                    primarySizes[index] = Math.Clamp(child.Length.Value, 0, remaining);
                    remaining = Math.Max(0, remaining - primarySizes[index]);
                    break;
                case LayoutLengthKind.Auto:
                    var measured = child.Content.Measure(inner);
                    var autoSize = horizontal ? measured.Width : measured.Height;
                    primarySizes[index] = Math.Clamp(autoSize, 0, remaining);
                    remaining = Math.Max(0, remaining - primarySizes[index]);
                    break;
                case LayoutLengthKind.Fill:
                    flexibleWeights[index] = 1;
                    break;
                case LayoutLengthKind.Weighted:
                    flexibleWeights[index] = Math.Max(1, child.Length.Value);
                    break;
            }
        }

        var totalWeight = flexibleWeights.Sum();
        if (totalWeight > 0 && remaining > 0)
        {
            var assigned = 0;
            for (var index = 0; index < children.Count; index++)
            {
                var weight = flexibleWeights[index];
                if (weight <= 0)
                {
                    continue;
                }

                var share = (remaining * weight) / totalWeight;
                primarySizes[index] = share;
                assigned += share;
            }

            var leftover = remaining - assigned;
            for (var index = 0; index < children.Count && leftover > 0; index++)
            {
                if (flexibleWeights[index] <= 0)
                {
                    continue;
                }

                primarySizes[index]++;
                leftover--;
            }
        }

        var cursorX = inner.X;
        var cursorY = inner.Y;
        for (var index = 0; index < children.Count; index++)
        {
            var child = children[index];
            var margin = child.Margin;
            var contentPrimary = primarySizes[index];
            var totalPrimary = contentPrimary + (horizontal ? margin.Horizontal : margin.Vertical);
            var cross = Math.Max(0, crossAvailable - (horizontal ? margin.Vertical : margin.Horizontal));

            Rect childOuter;
            Rect childBounds;
            if (horizontal)
            {
                childOuter = new Rect(cursorX, inner.Y, totalPrimary, inner.Height);
                childBounds = new Rect(
                    childOuter.X + margin.Left,
                    childOuter.Y + margin.Top,
                    Math.Max(0, contentPrimary),
                    Math.Max(0, cross));
                cursorX += totalPrimary + gap;
            }
            else
            {
                childOuter = new Rect(inner.X, cursorY, inner.Width, totalPrimary);
                childBounds = new Rect(
                    childOuter.X + margin.Left,
                    childOuter.Y + margin.Top,
                    Math.Max(0, cross),
                    Math.Max(0, contentPrimary));
                cursorY += totalPrimary + gap;
            }

            if (!childBounds.IsEmpty)
            {
                child.Content.Compose(screen, childBounds, $"{path}/slot:{index}");
            }
        }
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

    public static void ComposeDock(ScreenComposer screen, DockLayout layout, in Rect bounds, string path)
    {
        var inner = Rect.Intersect(bounds.Inset(layout.Padding), bounds);
        if (inner.IsEmpty)
        {
            return;
        }

        var working = inner;

        if (layout.Top is LayoutSlot top)
        {
            var measured = ResolveSlotExtent(top, horizontal: false, working);
            var outer = new Rect(working.X, working.Y, working.Width, measured);
            ComposeDockSlot(screen, top, outer, horizontal: false, $"{path}/top");
            working = new Rect(working.X, working.Y + measured + layout.Gap, working.Width, Math.Max(0, working.Height - measured - layout.Gap));
        }

        if (layout.Bottom is LayoutSlot bottom && !working.IsEmpty)
        {
            var measured = ResolveSlotExtent(bottom, horizontal: false, working);
            var outer = new Rect(working.X, Math.Max(working.Y, working.Bottom - measured), working.Width, measured);
            ComposeDockSlot(screen, bottom, outer, horizontal: false, $"{path}/bottom");
            working = new Rect(working.X, working.Y, working.Width, Math.Max(0, working.Height - measured - layout.Gap));
        }

        if (layout.Left is LayoutSlot left && !working.IsEmpty)
        {
            var measured = ResolveSlotExtent(left, horizontal: true, working);
            var outer = new Rect(working.X, working.Y, measured, working.Height);
            ComposeDockSlot(screen, left, outer, horizontal: true, $"{path}/left");
            working = new Rect(working.X + measured + layout.Gap, working.Y, Math.Max(0, working.Width - measured - layout.Gap), working.Height);
        }

        if (layout.Right is LayoutSlot right && !working.IsEmpty)
        {
            var measured = ResolveSlotExtent(right, horizontal: true, working);
            var outer = new Rect(Math.Max(working.X, working.Right - measured), working.Y, measured, working.Height);
            ComposeDockSlot(screen, right, outer, horizontal: true, $"{path}/right");
            working = new Rect(working.X, working.Y, Math.Max(0, working.Width - measured - layout.Gap), working.Height);
        }

        if (layout.Fill is LayoutSlot fill && !working.IsEmpty)
        {
            ComposeDockSlot(screen, fill, working, horizontal: true, $"{path}/fill");
        }
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

    private static void ComposeDockSlot(ScreenComposer screen, LayoutSlot slot, in Rect outerBounds, bool horizontal, string path)
    {
        var margin = slot.Margin;
        var bounds = new Rect(
            outerBounds.X + margin.Left,
            outerBounds.Y + margin.Top,
            Math.Max(0, outerBounds.Width - margin.Horizontal),
            Math.Max(0, outerBounds.Height - margin.Vertical));
        if (!bounds.IsEmpty)
        {
            slot.Content.Compose(screen, bounds, path);
        }
    }
}
