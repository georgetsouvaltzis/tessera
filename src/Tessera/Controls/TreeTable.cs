using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a hierarchical table with expandable rows.
/// </summary>
public sealed partial class TreeTable : Control
{
    private readonly List<string> _columns = [];
    private readonly List<TreeTableNode> _roots = [];
    private readonly List<VisibleEntry> _visible = [];
    private int _hoveredVisibleIndex = -1;
    private int _scrollOffset;
    private int _selectedVisibleIndex;

    /// <summary>
    ///     Initializes a tree table with optional columns.
    /// </summary>
    /// <param name="columns">Column headers. The first column is the tree label column.</param>
    public TreeTable(IEnumerable<string>? columns = null)
    {
        SetColumns(columns ?? ["Name"]);
    }

    /// <summary>
    ///     Initializes a tree table with optional columns.
    /// </summary>
    /// <param name="columns">Column headers. The first column is the tree label column.</param>
    public TreeTable(params string[] columns)
        : this((IEnumerable<string>)columns)
    {
    }

    /// <summary>
    ///     Gets or sets the table title.
    /// </summary>
    public string Title { get; set; } = "Tree Table";

    /// <summary>
    ///     Gets or sets the marker shown in the title while focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether to render the focus marker when focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets title style when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets title style when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets column header style.
    /// </summary>
    public TesseraStyle HeaderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style for branch rows.
    /// </summary>
    public TesseraStyle BranchRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style for leaf rows.
    /// </summary>
    public TesseraStyle LeafRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into selected rows.
    /// </summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into hovered rows.
    /// </summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged when disabled.
    /// </summary>
    public TesseraStyle MutedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets text rendered between columns.
    /// </summary>
    public string ColumnSeparatorText { get; set; } = " | ";

    /// <summary>
    ///     Gets or sets marker text rendered for selected rows.
    /// </summary>
    public string SelectedRowMarker { get; set; } = ">";

    /// <summary>
    ///     Gets or sets marker text rendered for unselected rows.
    /// </summary>
    public string UnselectedRowMarker { get; set; } = " ";

    /// <summary>
    ///     Gets or sets marker text rendered for expanded branch rows.
    /// </summary>
    public string ExpandedBranchMarker { get; set; } = "-";

    /// <summary>
    ///     Gets or sets marker text rendered for collapsed branch rows.
    /// </summary>
    public string CollapsedBranchMarker { get; set; } = "+";

    /// <summary>
    ///     Gets or sets marker text rendered for leaf rows.
    /// </summary>
    public string LeafMarker { get; set; } = ".";

    /// <summary>
    ///     Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets configured column headers.
    /// </summary>
    public IReadOnlyList<string> Columns => _columns;

    /// <summary>
    ///     Gets root rows.
    /// </summary>
    public IReadOnlyList<TreeTableNode> RootItems => _roots;

    /// <summary>
    ///     Gets selected visible row index. Returns <c>-1</c> when no rows exist.
    /// </summary>
    public int SelectedIndex => _visible.Count == 0 ? -1 : _selectedVisibleIndex;

    /// <summary>
    ///     Gets selected row.
    /// </summary>
    public TreeTableNode? SelectedItem => _selectedVisibleIndex >= 0 && _selectedVisibleIndex < _visible.Count
        ? _visible[_selectedVisibleIndex].Item
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Occurs when the selected row changes.
    /// </summary>
    public event EventHandler<TreeTableSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Replaces column headers.
    /// </summary>
    /// <param name="columns">Column headers. Empty input falls back to a single <c>Name</c> column.</param>
    public void SetColumns(IEnumerable<string> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        _columns.Clear();
        foreach (var column in columns)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                continue;
            }

            _columns.Add(column.Trim());
        }

        if (_columns.Count == 0)
        {
            _columns.Add("Name");
        }
    }

    /// <summary>
    ///     Replaces root rows.
    /// </summary>
    /// <param name="items">Root rows.</param>
    public void SetItems(IEnumerable<TreeTableNode> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var selectedId = previousItem?.Id;

        _roots.Clear();
        foreach (var item in items)
        {
            _roots.Add(Clone(item));
        }

        RefreshVisible();
        if (!string.IsNullOrEmpty(selectedId))
        {
            var index = _visible.FindIndex(entry => string.Equals(entry.Item.Id, selectedId, StringComparison.Ordinal));
            if (index >= 0)
            {
                _selectedVisibleIndex = index;
            }
        }

        _selectedVisibleIndex = Math.Clamp(_selectedVisibleIndex, 0, Math.Max(0, _visible.Count - 1));
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    ///     Toggles expansion for the selected branch row.
    /// </summary>
    /// <returns><see langword="true" /> when expansion changed; otherwise <see langword="false" />.</returns>
    public bool ToggleSelectedExpanded()
    {
        var selected = SelectedItem;
        if (selected is null || !selected.IsBranch)
        {
            return false;
        }

        selected.IsExpanded = !selected.IsExpanded;
        RefreshVisible();
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _visible.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedVisibleIndex(Math.Min(_visible.Count - 1, _selectedVisibleIndex + 1));
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedVisibleIndex(Math.Max(0, _selectedVisibleIndex - 1));
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedVisibleIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedVisibleIndex(_visible.Count - 1);
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return ExpandOrMoveIntoChild();
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return CollapseOrMoveToParent();
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            return ToggleSelectedExpanded();
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty || _visible.Count == 0)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        var inside = !content.IsEmpty && content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredVisibleIndex(-1);
            }

            if (pointer.Kind is not PointerEventKind.Wheel)
            {
                return changed || Handle(message);
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return changed || SetSelectedVisibleIndex(Math.Min(_visible.Count - 1, _selectedVisibleIndex + 1));
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return changed || SetSelectedVisibleIndex(Math.Max(0, _selectedVisibleIndex - 1));
            }

            return false;
        }

        if (!inside)
        {
            return changed || Handle(message);
        }

        var hoveredVisibleIndex = RowToVisibleIndex(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredVisibleIndex(hoveredVisibleIndex);
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return Handle(message);
        }

        RequestFocus();
        changed |= SetHoveredVisibleIndex(hoveredVisibleIndex);
        if (hoveredVisibleIndex < 0)
        {
            return true;
        }

        var selectionChanged = SetSelectedVisibleIndex(hoveredVisibleIndex);
        return changed || selectionChanged;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
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
        if (content.IsEmpty || content.Height <= 0)
        {
            return;
        }

        canvas.WriteText(content.X, content.Y, ApplyStyle(RenderHeader(), HeaderStyle), content.Width);
        if (content.Height <= 1)
        {
            return;
        }

        if (_visible.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ApplyStyle("(empty)", MutedRowStyle), content.Width);
            return;
        }

        var viewportRows = content.Height - 1;
        EnsureSelectionVisible(viewportRows);
        var rows = Math.Min(viewportRows, _visible.Count - _scrollOffset);
        for (var row = 0; row < rows; row++)
        {
            var visibleIndex = _scrollOffset + row;
            var entry = _visible[visibleIndex];
            var marker = visibleIndex == _selectedVisibleIndex
                ? ResolveSelectedRowMarkerText()
                : ResolveUnselectedRowMarkerText();
            var indent = new string(' ', entry.Depth * 2);
            var glyph = ResolveRowGlyph(entry.Item);
            var firstColumn = $"{indent}{glyph} {entry.Item.Label}";
            var values = new List<string>(_columns.Count) { firstColumn };
            for (var column = 1; column < _columns.Count; column++)
            {
                var valueIndex = column - 1;
                values.Add(valueIndex < entry.Item.Values.Count ? entry.Item.Values[valueIndex] : string.Empty);
            }

            var line = $"{marker} {string.Join(ResolveColumnSeparatorText(), values)}";
            var style = entry.Item.IsBranch ? BranchRowStyle : LeafRowStyle;
            if (visibleIndex == _hoveredVisibleIndex)
            {
                style = style.Merge(HoveredRowStyle);
            }

            if (visibleIndex == _selectedVisibleIndex)
            {
                style = style.Merge(SelectedRowStyle);
            }

            if (IsDisabled)
            {
                style = style.Merge(MutedRowStyle);
            }

            canvas.WriteText(content.X, content.Y + 1 + row, ApplyStyle(line, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(RenderHeader()) + 2);
        for (var index = 0; index < _visible.Count; index++)
        {
            var entry = _visible[index];
            var firstColumn = $"{new string(' ', entry.Depth * 2)}{ResolveRowGlyph(entry.Item)} {entry.Item.Label}";
            var values = new List<string>(_columns.Count) { firstColumn };
            for (var column = 1; column < _columns.Count; column++)
            {
                var valueIndex = column - 1;
                values.Add(valueIndex < entry.Item.Values.Count ? entry.Item.Values[valueIndex] : string.Empty);
            }

            var rowText = $"{ResolveSelectedRowMarkerText()} {string.Join(ResolveColumnSeparatorText(), values)}";
            var rowWidth = ControlTextLayout.MeasureDisplayWidth(rowText);
            width = Math.Max(width, rowWidth);
        }

        var height = Math.Max(2, _visible.Count + 1);
        width += Padding.Horizontal;
        height += Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
