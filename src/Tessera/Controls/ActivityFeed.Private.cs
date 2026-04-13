using System.Globalization;
using System.Text;
using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class ActivityFeed
{
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width,
                ControlTextLayout.MeasureDisplayWidth(FormatLine(_items[index], index == SelectedIndex)) + 2);
        }

        var height = Math.Max(4, Math.Min(12, _items.Count + 2));
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private void TrimToMaxItems()
    {
        if (MaxItems <= 0 || _items.Count <= MaxItems)
        {
            return;
        }

        var remove = _items.Count - MaxItems;
        _items.RemoveRange(0, remove);
        SelectedIndex = SelectedIndex < 0 ? -1 : Math.Max(0, SelectedIndex - remove);
        _hoveredIndex = _hoveredIndex < 0 ? -1 : Math.Max(0, _hoveredIndex - remove);
        _scrollOffset = Math.Max(0, _scrollOffset - remove);
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_items.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }

        if (SelectedIndex < _scrollOffset)
        {
            _scrollOffset = SelectedIndex;
        }
        else if (SelectedIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = SelectedIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _items.Count - viewportRows));
    }

    private TesseraStyle ResolveItemStyle(int index)
    {
        var item = _items[index];
        var style = ResolveKindStyle(item.Kind);
        if (item.IsUnread)
        {
            style = style.Merge(UnreadItemStyle);
        }

        if (item.IsMuted)
        {
            style = style.Merge(MutedItemStyle);
        }

        if (item.HasError)
        {
            style = style.Merge(ErrorItemStyle);
        }

        if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (index == SelectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedItemStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private TesseraStyle ResolveKindStyle(ActivityFeedItemKind kind)
    {
        return kind switch
        {
            ActivityFeedItemKind.Success => SuccessItemStyle,
            ActivityFeedItemKind.Warning => WarningItemStyle,
            ActivityFeedItemKind.Error => ErrorItemStyle,
            _ => InfoItemStyle
        };
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledItemStyle) : style;
    }

    private string FormatLine(ActivityFeedItem item, bool selected)
    {
        var marker = UnselectedMarker;
        if (selected)
        {
            marker = SelectedMarker;
        }
        else if (item.IsUnread)
        {
            marker = UnreadMarker;
        }

        return string.Concat(marker, " ", BuildBody(item));
    }

    private string BuildBody(ActivityFeedItem item)
    {
        var builder = new StringBuilder(64);
        if (ShowTimestamp)
        {
            var stamp = item.Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);
            builder.Append(ApplyStyle(stamp, TimestampStyle));
            builder.Append(' ');
        }

        builder.Append(item.Actor);
        if (!string.IsNullOrWhiteSpace(item.Action))
        {
            builder.Append(' ');
            builder.Append(item.Action);
        }

        if (!string.IsNullOrWhiteSpace(item.Target))
        {
            builder.Append(' ');
            builder.Append(item.Target);
        }

        if (!string.IsNullOrWhiteSpace(item.Details))
        {
            builder.Append(" - ");
            builder.Append(item.Details);
        }

        return builder.ToString();
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
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title;
    }

    private static ActivityFeedItem Clone(ActivityFeedItem item)
    {
        return new ActivityFeedItem(item.Actor, item.Action, item.Target, item.Details, item.Kind, item.Timestamp)
        {
            IsUnread = item.IsUnread,
            IsMuted = item.IsMuted,
            HasError = item.HasError
        };
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
