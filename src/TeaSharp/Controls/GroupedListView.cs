using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>Grouped list with selectable rows and collapsible sections.</summary>
/// <typeparam name="TGroup">Group key type.</typeparam>
/// <typeparam name="TItem">Item type.</typeparam>
public sealed class GroupedListView<TGroup, TItem> : Control
{
    private readonly Func<TGroup, string> _groupTextSelector;
    private readonly Func<TItem, string> _itemTextSelector;
    private readonly List<GroupedListViewGroup<TGroup, TItem>> _groups = [];
    private readonly List<RowEntry> _rows = [];
    private int _selectedRowIndex = -1;
    private int _hoveredRowIndex = -1;
    private int _scrollOffset;
    private int _viewportHeight = 1;

    /// <summary>Creates a grouped list with optional text selectors.</summary>
    public GroupedListView(Func<TGroup, string>? groupTextSelector = null, Func<TItem, string>? itemTextSelector = null)
    {
        _groupTextSelector = groupTextSelector ?? DefaultGroupText;
        _itemTextSelector = itemTextSelector ?? DefaultItemText;
    }

    /// <summary>Raised when selected row/item changes.</summary>
    public event EventHandler<GroupedListSelectionChangedEventArgs<TGroup, TItem>>? SelectionChanged;
    /// <summary>Control title.</summary>
    public string Title { get; set; } = "Grouped List";
    /// <summary>Focused title marker.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Whether to show <see cref="FocusMarker"/>.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Title style when unfocused.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Title style when focused.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Group-header row style.</summary>
    public TeaStyle GroupHeaderStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Item row style.</summary>
    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Hovered row style merge.</summary>
    public TeaStyle HoveredRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Selected row style merge.</summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Disabled row style merge.</summary>
    public TeaStyle DisabledRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Unfocused border style.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Focused border style merge.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Text rendered when no groups exist.</summary>
    public string EmptyText { get; set; } = "(empty)";
    /// <summary>Frame border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Inner padding.</summary>
    public Thickness Padding { get; set; }
    /// <summary>Current groups.</summary>
    public IReadOnlyList<GroupedListViewGroup<TGroup, TItem>> Groups => _groups;
    /// <summary>Visible row count (headers and expanded items).</summary>
    public int VisibleRowCount => _rows.Count;
    /// <summary>Selected visible row index or <c>-1</c>.</summary>
    public int SelectedRowIndex => _selectedRowIndex;
    /// <summary>Selected group index when available.</summary>
    public int? SelectedGroupIndex => TryGetRow(_selectedRowIndex, out var row) ? row.GroupIndex : null;
    /// <summary>Selected item index within group when item row is selected.</summary>
    public int? SelectedItemIndex => TryGetRow(_selectedRowIndex, out var row) && !row.IsHeader ? row.ItemIndex : null;
    /// <summary>Selected item when item row is selected.</summary>
    public TItem? SelectedItem => TryGetRow(_selectedRowIndex, out var row) && !row.IsHeader ? _groups[row.GroupIndex].Items[row.ItemIndex] : default;
    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces groups.</summary>
    public void SetGroups(IEnumerable<GroupedListViewGroup<TGroup, TItem>> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);
        var prev = CaptureSelection();
        _groups.Clear();
        foreach (var group in groups)
        {
            if (group is null) continue;
            _groups.Add(new GroupedListViewGroup<TGroup, TItem>(group.Group, group.Items) { IsCollapsed = group.IsCollapsed });
        }

        RebuildRows(prev);
    }

    /// <summary>Clears groups, rows, and selection.</summary>
    public void Clear()
    {
        var prev = CaptureSelection();
        _groups.Clear();
        _rows.Clear();
        _selectedRowIndex = -1;
        _hoveredRowIndex = -1;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(prev);
    }

    /// <summary>Sets selected visible row index with bounds clamping.</summary>
    public bool SetSelectedRowIndex(int rowIndex)
    {
        if (_rows.Count == 0) return false;
        var next = Math.Clamp(rowIndex, 0, _rows.Count - 1);
        if (next == _selectedRowIndex) return false;
        var prev = CaptureSelection();
        _selectedRowIndex = next;
        EnsureSelectionVisible(_viewportHeight);
        RaiseSelectionChangedIfNeeded(prev);
        return true;
    }

    /// <summary>Sets selection to one item by group/item indexes.</summary>
    public bool SetSelectedItem(int groupIndex, int itemIndex)
    {
        var rowIndex = FindRowIndex(groupIndex, itemIndex);
        return rowIndex >= 0 && SetSelectedRowIndex(rowIndex);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _rows.Count == 0 || message is not KeyPressed key) return false;
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedRowIndex(_selectedRowIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedRowIndex(_selectedRowIndex - 1);
        if (key.Is(Key.PageDown)) return SetSelectedRowIndex(_selectedRowIndex + Math.Max(1, _viewportHeight));
        if (key.Is(Key.PageUp)) return SetSelectedRowIndex(_selectedRowIndex - Math.Max(1, _viewportHeight));
        if (key.Is(Key.Home)) return SetSelectedRowIndex(0);
        if (key.Is(Key.End)) return SetSelectedRowIndex(_rows.Count - 1);
        if (key.Is(Key.Left)) return SetSelectedGroupCollapse(collapse: true);
        if (key.Is(Key.Right)) return SetSelectedGroupCollapse(collapse: false);
        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            if (TryGetRow(_selectedRowIndex, out var row) && row.IsHeader) return ToggleGroup(row.GroupIndex);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty) return Handle(message);
        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty) return Handle(message);

        _viewportHeight = Math.Max(1, content.Height);
        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press) changed |= SetHovered(-1);
        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedRowIndex(_selectedRowIndex + 1) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedRowIndex(_selectedRowIndex - 1) || changed;
            return changed;
        }

        if (!inside || _rows.Count == 0) return changed;
        var rowIndex = ResolveRowIndex(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion) return SetHovered(rowIndex);
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && rowIndex >= 0)
        {
            RequestFocus();
            changed |= SetHovered(rowIndex);
            changed |= SetSelectedRowIndex(rowIndex);
            if (TryGetRow(rowIndex, out var row) && row.IsHeader)
            {
                var markerClick = pointer.X <= content.X + 2;
                if (markerClick)
                {
                    changed |= ToggleGroup(row.GroupIndex);
                }
                else if (!_groups[row.GroupIndex].IsCollapsed && _groups[row.GroupIndex].Items.Count > 0)
                {
                    changed |= SetSelectedItem(row.GroupIndex, 0);
                }
            }
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty) return;
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty) return;

        _viewportHeight = Math.Max(1, content.Height);
        if (_rows.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveRowStyle(TeaStyle.Empty, selected: false, hovered: false)), content.Width);
            return;
        }

        EnsureSelectionVisible(content.Height);
        var rows = Math.Min(content.Height, _rows.Count - _scrollOffset);
        for (var row = 0; row < rows; row++)
        {
            var index = _scrollOffset + row;
            var entry = _rows[index];
            var selected = index == _selectedRowIndex;
            var hovered = index == _hoveredRowIndex;
            var baseStyle = entry.IsHeader ? GroupHeaderStyle : ItemStyle;
            var text = entry.IsHeader ? BuildHeaderText(entry, selected, hovered) : BuildItemText(entry, selected, hovered);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(text, ResolveRowStyle(baseStyle, selected, hovered)), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 4) + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = 8 + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetSelectedGroupCollapse(bool collapse)
    {
        if (!TryGetRow(_selectedRowIndex, out var row)) return false;
        var groupIndex = row.GroupIndex;
        if (groupIndex < 0 || groupIndex >= _groups.Count || _groups[groupIndex].IsCollapsed == collapse) return false;
        var prev = CaptureSelection();
        _groups[groupIndex].IsCollapsed = collapse;
        RebuildRows(prev);
        return true;
    }

    private bool ToggleGroup(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= _groups.Count) return false;
        var prev = CaptureSelection();
        _groups[groupIndex].IsCollapsed = !_groups[groupIndex].IsCollapsed;
        RebuildRows(prev);
        return true;
    }

    private void RebuildRows(SelectionSnapshot previous)
    {
        _rows.Clear();
        for (var g = 0; g < _groups.Count; g++)
        {
            _rows.Add(new RowEntry(g, -1, IsHeader: true));
            if (_groups[g].IsCollapsed) continue;
            for (var i = 0; i < _groups[g].Items.Count; i++) _rows.Add(new RowEntry(g, i, IsHeader: false));
        }

        if (_rows.Count == 0)
        {
            _selectedRowIndex = -1;
            _hoveredRowIndex = -1;
            _scrollOffset = 0;
            RaiseSelectionChangedIfNeeded(previous);
            return;
        }

        _selectedRowIndex = ResolveRestoredRow(previous);
        _hoveredRowIndex = Math.Clamp(_hoveredRowIndex, -1, _rows.Count - 1);
        EnsureSelectionVisible(Math.Max(1, _viewportHeight));
        RaiseSelectionChangedIfNeeded(previous);
    }

    private int ResolveRestoredRow(SelectionSnapshot previous)
    {
        if (previous.GroupIndex.HasValue)
        {
            if (previous.ItemIndex.HasValue)
            {
                var itemRow = FindRowIndex(previous.GroupIndex.Value, previous.ItemIndex.Value);
                if (itemRow >= 0) return itemRow;
            }

            var headerRow = FindRowIndex(previous.GroupIndex.Value, -1);
            if (headerRow >= 0) return headerRow;
        }

        return _selectedRowIndex >= 0 ? Math.Clamp(_selectedRowIndex, 0, _rows.Count - 1) : 0;
    }

    private int FindRowIndex(int groupIndex, int itemIndex)
    {
        for (var i = 0; i < _rows.Count; i++)
        {
            var row = _rows[i];
            if (row.GroupIndex == groupIndex && row.ItemIndex == itemIndex) return i;
        }

        return -1;
    }

    private void EnsureSelectionVisible(int viewportHeight)
    {
        if (_rows.Count == 0 || viewportHeight <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedRowIndex < _scrollOffset) _scrollOffset = _selectedRowIndex;
        else if (_selectedRowIndex >= _scrollOffset + viewportHeight) _scrollOffset = _selectedRowIndex - viewportHeight + 1;
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _rows.Count - viewportHeight));
    }

    private int ResolveRowIndex(Rect content, int y)
    {
        if (y < content.Y || y >= content.Bottom) return -1;
        var index = _scrollOffset + (y - content.Y);
        return index >= 0 && index < _rows.Count ? index : -1;
    }

    private bool SetHovered(int index)
    {
        if (_hoveredRowIndex == index) return false;
        _hoveredRowIndex = index;
        return true;
    }

    private string BuildHeaderText(RowEntry row, bool selected, bool hovered)
    {
        var prefix = selected ? "> " : hovered ? "~ " : "  ";
        var marker = _groups[row.GroupIndex].IsCollapsed ? "▶ " : "▼ ";
        return string.Concat(prefix, marker, _groupTextSelector(_groups[row.GroupIndex].Group));
    }

    private string BuildItemText(RowEntry row, bool selected, bool hovered)
    {
        var prefix = selected ? "> " : hovered ? "~ " : "  ";
        return string.Concat(prefix, "  ", _itemTextSelector(_groups[row.GroupIndex].Items[row.ItemIndex]));
    }

    private bool TryGetRow(int index, out RowEntry row)
    {
        if (index < 0 || index >= _rows.Count)
        {
            row = default;
            return false;
        }

        row = _rows[index];
        return true;
    }

    private SelectionSnapshot CaptureSelection()
    {
        if (!TryGetRow(_selectedRowIndex, out var row)) return new SelectionSnapshot(-1, null, null, default);
        var item = row.IsHeader ? default : _groups[row.GroupIndex].Items[row.ItemIndex];
        return new SelectionSnapshot(_selectedRowIndex, row.GroupIndex, row.IsHeader ? null : row.ItemIndex, item);
    }

    private void RaiseSelectionChangedIfNeeded(SelectionSnapshot previous)
    {
        var current = CaptureSelection();
        if (previous.Equals(current)) return;
        SelectionChanged?.Invoke(
            this,
            new GroupedListSelectionChangedEventArgs<TGroup, TItem>(
                previous.RowIndex,
                current.RowIndex,
                previous.GroupIndex,
                current.GroupIndex,
                previous.ItemIndex,
                current.ItemIndex,
                previous.Item,
                current.Item));
    }

    private TeaStyle ResolveRowStyle(TeaStyle baseStyle, bool selected, bool hovered)
    {
        var style = baseStyle;
        if (hovered) style = style.Merge(HoveredRowStyle);
        if (selected) style = style.Merge(SelectedRowStyle);
        if (IsDisabled) style = style.Merge(DisabledRowStyle);
        return style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledRowStyle) : style;
    }

    private string RenderTitle() => ApplyStyle(CurrentTitle(), IsFocused ? FocusedTitleStyle : TitleStyle);

    private string CurrentTitle() => IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title ?? string.Empty;

    private string MeasureTitle() => ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title ?? string.Empty;

    private static string ApplyStyle(string text, TeaStyle style) => string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);

    private static string DefaultGroupText(TGroup value) => value?.ToString() ?? string.Empty;

    private static string DefaultItemText(TItem value) => value?.ToString() ?? string.Empty;

    private readonly record struct RowEntry(int GroupIndex, int ItemIndex, bool IsHeader);

    private readonly record struct SelectionSnapshot(int RowIndex, int? GroupIndex, int? ItemIndex, TItem? Item);
}
