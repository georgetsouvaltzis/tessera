using Tessera.Components.Primitives;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class SearchResultsView
{
    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private bool SetPressedIndex(int index)
    {
        if (_pressedIndex == index)
        {
            return false;
        }

        _pressedIndex = index;
        return true;
    }

    private int RowIndexAtPointer(int x, int y, Rect content)
    {
        if (!content.Contains(x, y))
        {
            return -1;
        }

        var row = y - content.Y;
        return row >= 0 && row < _items.Count ? row : -1;
    }

    private bool HasQueryMatch(string value)
    {
        return !string.IsNullOrWhiteSpace(Query)
            && value.Contains(Query, StringComparison.OrdinalIgnoreCase);
    }

    private string RenderTitle()
    {
        var title = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrEmpty(FocusMarker))
        {
            title = $"{title} {FocusMarker}";
        }

        if (TitleStyle.IsEmpty && FocusedTitleStyle.IsEmpty)
        {
            return title;
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return style.IsEmpty ? title : style.Render(title);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        return IsFocused
            ? BorderStyleText.Merge(FocusedBorderStyleText)
            : BorderStyleText;
    }

    private string ApplyRowStyle(string text, bool selected, bool hovered, bool pressed)
    {
        var style = DefaultRowStyle;
        if (hovered)
        {
            style = style.Merge(HoveredRowStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
        }

        if (selected && IsFocused)
        {
            style = style.Merge(FocusedSelectedRowStyle);
        }

        if (pressed)
        {
            style = style.Merge(PressedRowStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorRowStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledRowStyle);
        }

        return style.IsEmpty ? text : style.Render(text);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, string previousItem)
    {
        if (previousIndex == _selectedIndex)
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new SelectionChangedEventArgs(
                previousIndex,
                _selectedIndex,
                previousItem,
                SelectedItem));
    }
}
