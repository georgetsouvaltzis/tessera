using System.Text;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class JumpList
{
    private int ResolveSelectedIndex(string? selectedId)
    {
        if (!string.IsNullOrEmpty(selectedId))
        {
            for (var index = 0; index < _items.Count; index++)
            {
                if (string.Equals(_items[index].Id, selectedId, StringComparison.Ordinal))
                {
                    return index;
                }
            }
        }

        return _items.Count == 0 ? -1 : 0;
    }

    private int ResolveListTop(Rect content)
    {
        if (Border != BorderStyle.None)
        {
            return content.Y;
        }

        var title = RenderTitle();
        return string.IsNullOrEmpty(title) ? content.Y : content.Y + 1;
    }

    private int ResolveRowIndex(int pointerX, int pointerY, int listX, int listTop, int listWidth, int listHeight)
    {
        if (pointerX < listX || pointerX >= listX + listWidth)
        {
            return -1;
        }

        if (pointerY < listTop || pointerY >= listTop + listHeight)
        {
            return -1;
        }

        var row = pointerY - listTop;
        return row >= 0 && row < _items.Count ? row : -1;
    }

    private bool SetHoveredIndex(int index)
    {
        var normalized = index < 0 || index >= _items.Count ? -1 : index;
        if (_hoveredIndex == normalized)
        {
            return false;
        }

        _hoveredIndex = normalized;
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, JumpListItem? previousItem)
    {
        if (previousIndex == _selectedIndex)
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousItem);
    }

    private void RaiseSelectionChanged(int previousIndex, JumpListItem? previousItem)
    {
        SelectionChanged?.Invoke(
            this,
            new ListSelectionChangedEventArgs<JumpListItem>(
                previousIndex,
                _selectedIndex,
                previousItem,
                SelectedItem));
    }

    private string BuildLine(JumpListItem item, bool selected)
    {
        var marker = selected ? Glyphs.SelectedMarker : Glyphs.UnselectedMarker;
        var stateMarker = BuildStateMarker(item);

        var builder = new StringBuilder(marker.Length + stateMarker.Length + Glyphs.MarkerSeparator.Length + item.Label.Length + 2);
        builder.Append(marker);
        builder.Append(' ');
        if (!string.IsNullOrEmpty(stateMarker))
        {
            builder.Append(stateMarker);
            builder.Append(Glyphs.MarkerSeparator);
        }

        builder.Append(item.Label);
        return builder.ToString();
    }

    private string BuildStateMarker(JumpListItem item)
    {
        var hasPinned = item.IsPinned && !string.IsNullOrEmpty(Glyphs.PinnedMarker);
        var hasRecent = item.IsRecent && !string.IsNullOrEmpty(Glyphs.RecentMarker);
        if (!hasPinned && !hasRecent)
        {
            return string.Empty;
        }

        if (hasPinned && hasRecent)
        {
            var pinned = ApplyStyle(Glyphs.PinnedMarker, PinnedMarkerStyle);
            var recent = ApplyStyle(Glyphs.RecentMarker, RecentMarkerStyle);
            return pinned + recent;
        }

        return hasPinned
            ? ApplyStyle(Glyphs.PinnedMarker, PinnedMarkerStyle)
            : ApplyStyle(Glyphs.RecentMarker, RecentMarkerStyle);
    }

    private TeaStyle ResolveBorderStyle()
    {
        return IsFocused
            ? BorderStyleText.Merge(FocusedBorderStyleText)
            : BorderStyleText;
    }

    private TeaStyle ResolveRowStyle(JumpListItem item, int row)
    {
        var style = ItemStyle;
        if (row == _hoveredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (row == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedItemStyle);
            }
        }

        if (item.IsDisabled || IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var title = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrEmpty(FocusMarker))
        {
            title = string.IsNullOrEmpty(title)
                ? FocusMarker
                : $"{title} {FocusMarker}";
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(title, style);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
