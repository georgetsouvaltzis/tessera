using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class ToastCenter
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        if (AutoDismissExpired)
        {
            DismissExpired();
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, RenderStyled("(empty)", MutedItemStyle), content.Width);
            return;
        }

        var rowCapacity = ResolveRowCapacity(content.Height);
        var start = ComputeWindowStart(rowCapacity);
        var end = Math.Min(_items.Count, start + rowCapacity);
        for (var row = 0; row < end - start; row++)
        {
            var index = start + row;
            var item = _items[index];
            var line = FormatLine(item, index == _selectedIndex);
            var style = ResolveRowStyle(item, index == _selectedIndex, index == _hoveredIndex);
            canvas.WriteText(content.X, content.Y + row, RenderStyled(line, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var rowCapacity = _items.Count == 0 ? 1 : Math.Min(_items.Count, ResolveVisibleCapacity());
        var width = _items.Count == 0
            ? 10
            : _items.Max(static item => ControlTextLayout.MeasureDisplayWidth(FormatLine(item, false)));
        width = Math.Max(width, 10);

        var title = FormatTitleText();
        if (Border != BorderStyle.None && !string.IsNullOrEmpty(title))
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(title) + 4);
        }

        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = rowCapacity + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string RenderTitle()
    {
        var text = FormatTitleText();
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return RenderStyled(text, style);
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

    private TesseraStyle ResolveRowStyle(ToastItem item, bool selected, bool hovered)
    {
        var style = ItemStyle.Merge(ResolveLevelStyle(item.Level));
        if (item.IsMuted)
        {
            style = style.Merge(MutedItemStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedItemStyle);
        }

        return style;
    }

    private TesseraStyle ResolveLevelStyle(NotificationLevel level)
    {
        return level switch
        {
            NotificationLevel.Success => SuccessItemStyle,
            NotificationLevel.Warning => WarningItemStyle,
            NotificationLevel.Error => ErrorItemStyle,
            _ => InfoItemStyle
        };
    }

    private static string FormatLine(ToastItem item, bool selected)
    {
        var cursor = selected ? ">" : " ";
        var level = item.Level switch
        {
            NotificationLevel.Success => "[+]",
            NotificationLevel.Warning => "[!]",
            NotificationLevel.Error => "[x]",
            _ => "[i]"
        };
        var muted = item.IsMuted ? "~ " : string.Empty;
        return $"{cursor}{level} {muted}{item.Message}";
    }

    private static string RenderStyled(string text, TesseraStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(MutedItemStyle);
        }

        return style;
    }
}
