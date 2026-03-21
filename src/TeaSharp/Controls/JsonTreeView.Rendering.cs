using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class JsonTreeView
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_visible.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveRowStyle(null, selected: false, hovered: false)), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_visible.Count, start + content.Height);
        for (var row = 0; row < end - start; row++)
        {
            var visibleIndex = start + row;
            var entry = _visible[visibleIndex];
            var style = ResolveRowStyle(entry.Node, visibleIndex == _selectedIndex, visibleIndex == _hoveredIndex);
            var line = FormatLine(entry.Node, entry.Depth, visibleIndex == _selectedIndex);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 4);
        for (var index = 0; index < _visible.Count; index++)
        {
            var entry = _visible[index];
            var rowWidth = ControlTextLayout.MeasureDisplayWidth(FormatLine(entry.Node, entry.Depth, selected: false));
            width = Math.Max(width, rowWidth + 2);
        }

        var height = Math.Max(2, _visible.Count);
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatLine(JsonTreeNode node, int depth, bool selected)
    {
        var marker = ResolveNodeMarker(node);
        var cursor = selected ? ">" : " ";
        var indent = new string(' ', Math.Max(0, depth) * 2);
        var label = string.IsNullOrEmpty(node.Key)
            ? node.DisplayValue
            : $"{node.Key}: {node.DisplayValue}";
        return $"{cursor} {indent}{marker} {label}";
    }

    private string ResolveNodeMarker(JsonTreeNode node)
    {
        if (!node.IsContainer)
        {
            return ValueMarker;
        }

        return node.Expanded ? ExpandedMarker : CollapsedMarker;
    }

    private TeaStyle ResolveRowStyle(JsonTreeNode? node, bool selected, bool hovered)
    {
        var style = node is null || node.IsContainer
            ? ContainerStyle
            : ValueStyle;
        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedRowStyle);
            }
        }

        if (hovered)
        {
            style = style.Merge(HoveredRowStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string MeasureTitle()
    {
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private static string ApplyStyle(string value, TeaStyle style)
    {
        return style.IsEmpty ? value : style.Render(value);
    }
}
