using Tessera.Components.Primitives;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class TreeTable
{
    private string RenderHeader()
    {
        return string.Join(ResolveColumnSeparatorText(), _columns);
    }

    private string RenderTitle()
    {
        var value = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(value, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private bool ExpandOrMoveIntoChild()
    {
        var selected = SelectedItem;
        if (selected is null || !selected.IsBranch)
        {
            return false;
        }

        if (!selected.IsExpanded)
        {
            selected.IsExpanded = true;
            RefreshVisible();
            return true;
        }

        if (_selectedVisibleIndex + 1 < _visible.Count
            && _visible[_selectedVisibleIndex + 1].Depth > _visible[_selectedVisibleIndex].Depth)
        {
            return SetSelectedVisibleIndex(_selectedVisibleIndex + 1);
        }

        return false;
    }

    private bool CollapseOrMoveToParent()
    {
        if (_selectedVisibleIndex < 0 || _selectedVisibleIndex >= _visible.Count)
        {
            return false;
        }

        var selected = _visible[_selectedVisibleIndex];
        if (selected.Item.IsBranch && selected.Item.IsExpanded)
        {
            selected.Item.IsExpanded = false;
            RefreshVisible();
            return true;
        }

        return selected.ParentVisibleIndex is { } parentIndex && SetSelectedVisibleIndex(parentIndex);
    }

    private bool SetSelectedVisibleIndex(int index)
    {
        if (_visible.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _visible.Count - 1);
        if (clamped == _selectedVisibleIndex)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _selectedVisibleIndex = clamped;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    private void RefreshVisible()
    {
        _visible.Clear();
        for (var index = 0; index < _roots.Count; index++)
        {
            AppendVisible(_roots[index], 0, null);
        }

        if (_visible.Count == 0)
        {
            _selectedVisibleIndex = 0;
            _scrollOffset = 0;
            _hoveredVisibleIndex = -1;
            return;
        }

        _selectedVisibleIndex = Math.Clamp(_selectedVisibleIndex, 0, _visible.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _visible.Count - 1));
        _hoveredVisibleIndex = _hoveredVisibleIndex >= 0 && _hoveredVisibleIndex < _visible.Count
            ? _hoveredVisibleIndex
            : -1;
    }

    private void AppendVisible(TreeTableNode item, int depth, int? parentVisibleIndex)
    {
        var visibleIndex = _visible.Count;
        _visible.Add(new VisibleEntry(item, depth, parentVisibleIndex));
        if (!item.IsBranch || !item.IsExpanded)
        {
            return;
        }

        for (var index = 0; index < item.Children.Count; index++)
        {
            AppendVisible(item.Children[index], depth + 1, visibleIndex);
        }
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (viewportRows <= 0 || _visible.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedVisibleIndex < _scrollOffset)
        {
            _scrollOffset = _selectedVisibleIndex;
        }
        else if (_selectedVisibleIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _selectedVisibleIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _visible.Count - viewportRows));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, TreeTableNode? previousItem)
    {
        if (previousIndex == SelectedIndex && ReferenceEquals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new TreeTableSelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
    }

    private static TreeTableNode Clone(TreeTableNode source)
    {
        var clone = new TreeTableNode(source.Id, source.Label, source.Values) { IsExpanded = source.IsExpanded };
        for (var index = 0; index < source.Children.Count; index++)
        {
            clone.AddChild(Clone(source.Children[index]));
        }

        return clone;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
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
            style = style.Merge(MutedRowStyle);
        }

        return style;
    }

    private string ResolveColumnSeparatorText()
    {
        return ColumnSeparatorText;
    }

    private string ResolveSelectedRowMarkerText()
    {
        return SelectedRowMarker;
    }

    private string ResolveUnselectedRowMarkerText()
    {
        return UnselectedRowMarker;
    }

    private int RowToVisibleIndex(in Rect content, int y)
    {
        var row = y - content.Y;
        if (row <= 0)
        {
            return -1;
        }

        var viewportRows = Math.Max(0, content.Height - 1);
        EnsureSelectionVisible(viewportRows);
        var visibleIndex = _scrollOffset + row - 1;
        return visibleIndex >= 0 && visibleIndex < _visible.Count
            ? visibleIndex
            : -1;
    }

    private bool SetHoveredVisibleIndex(int index)
    {
        var normalized = index >= 0 && index < _visible.Count
            ? index
            : -1;
        if (_hoveredVisibleIndex == normalized)
        {
            return false;
        }

        _hoveredVisibleIndex = normalized;
        return true;
    }

    private string ResolveRowGlyph(TreeTableNode item)
    {
        if (!item.IsBranch)
        {
            return LeafMarker;
        }

        return item.IsExpanded
            ? ExpandedBranchMarker
            : CollapsedBranchMarker;
    }

    private readonly record struct VisibleEntry(TreeTableNode Item, int Depth, int? ParentVisibleIndex);
}
