using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class SideNavRail
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 3 || clipped.Height < 1)
        {
            return;
        }

        var content = Border == BorderStyle.None
            ? clipped.Inset(Padding)
            : FrameLayout.DrawFrameAndResolveContent(
                canvas,
                clipped,
                RenderTitle(),
                Border,
                Padding,
                ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        RenderHeader(canvas, content);
        RenderItems(canvas, content);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var contentHeight = _items.Count + 1;
        var contentWidth = ControlTextLayout.MeasureDisplayWidth(FormatHeaderText());
        for (var index = 0; index < _items.Count; index++)
        {
            contentWidth = Math.Max(contentWidth, ControlTextLayout.MeasureDisplayWidth(FormatItemLine(index, hovered: false, selected: false)));
        }

        var width = Math.Max(8, contentWidth) + Padding.Horizontal;
        var height = Math.Max(1, contentHeight) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderHeader(Canvas canvas, Rect content)
    {
        var header = FormatHeaderText();
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        canvas.WriteText(content.X, content.Y, ApplyStyle(header, style), content.Width);
    }

    private void RenderItems(Canvas canvas, Rect content)
    {
        if (content.Height < 2)
        {
            return;
        }

        var maxRows = Math.Min(_items.Count, content.Height - 1);
        for (var row = 0; row < maxRows; row++)
        {
            var selected = row == _selectedIndex;
            var hovered = row == _hoveredIndex;
            var line = FormatItemLine(row, hovered, selected);
            var style = ResolveItemStyle(row, hovered, selected);
            canvas.WriteText(content.X, content.Y + 1 + row, ApplyStyle(line, style), content.Width);
        }
    }

    private string FormatHeaderText()
    {
        var marker = IsCollapsed ? Glyphs.CollapsedMarker : Glyphs.ExpandedMarker;
        var title = FormatTitleText();
        return string.IsNullOrEmpty(title)
            ? marker
            : string.Concat(marker, " ", title);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatItemLine(int index, bool hovered, bool selected)
    {
        var item = _items[index];
        var marker = selected
            ? Glyphs.SelectedItemMarker
            : hovered
                ? Glyphs.HoveredItemMarker
                : Glyphs.NormalItemMarker;
        return string.Concat(marker, Glyphs.ItemMarkerSeparator, FormatItemBody(item));
    }

    private string FormatItemBody(NavItem item)
    {
        var label = IsCollapsed ? FormatCollapsedLabel(item) : FormatExpandedLabel(item);
        if (string.IsNullOrEmpty(item.Badge))
        {
            return label;
        }

        return string.Concat(label, Glyphs.BadgeSeparator, Glyphs.BadgePrefix, item.Badge, Glyphs.BadgeSuffix);
    }

    private static string FormatCollapsedLabel(NavItem item)
    {
        if (!string.IsNullOrEmpty(item.Icon))
        {
            return item.Icon;
        }

        if (string.IsNullOrEmpty(item.Label))
        {
            return "?";
        }

        return item.Label[0].ToString();
    }

    private static string FormatExpandedLabel(NavItem item)
    {
        if (string.IsNullOrEmpty(item.Icon))
        {
            return item.Label;
        }

        if (string.IsNullOrEmpty(item.Label))
        {
            return item.Icon;
        }

        return string.Concat(item.Icon, " ", item.Label);
    }

    private TeaStyle ResolveItemStyle(int index, bool hovered, bool selected)
    {
        var style = ItemStyle;
        if (hovered)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedItemStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedItemStyle);
            }
        }

        if (IsDisabled || _items[index].IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
