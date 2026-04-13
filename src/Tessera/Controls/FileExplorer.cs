using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents an in-memory hierarchical explorer for directory/file-like nodes.
/// </summary>
public sealed partial class FileExplorer : Control
{
    private readonly List<FileExplorerItem> _roots = [];
    private readonly List<VisibleEntry> _visible = [];
    private int _hoveredVisibleIndex = -1;
    private int _scrollOffset;
    private int _selectedVisibleIndex;

    /// <summary>
    ///     Gets or sets the explorer title.
    /// </summary>
    public string Title { get; set; } = "File Explorer";

    /// <summary>
    ///     Gets or sets the marker appended to the title while focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether the focus marker should be rendered.
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
    ///     Gets or sets the style used for directory rows.
    /// </summary>
    public TesseraStyle DirectoryStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style used for file rows.
    /// </summary>
    public TesseraStyle FileStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into selected rows.
    /// </summary>
    public TesseraStyle SelectedStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into hovered rows.
    /// </summary>
    public TesseraStyle HoveredStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style used for muted/disabled output.
    /// </summary>
    public TesseraStyle MutedStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets configured root nodes.
    /// </summary>
    public IReadOnlyList<FileExplorerItem> RootItems => _roots;

    /// <summary>
    ///     Gets the selected visible-row index.
    ///     Returns <c>-1</c> when no rows exist.
    /// </summary>
    public int SelectedIndex => _visible.Count == 0 ? -1 : _selectedVisibleIndex;

    /// <summary>
    ///     Gets the selected node path, when any.
    /// </summary>
    public string? SelectedPath => SelectedItem?.Path;

    /// <summary>
    ///     Gets the selected node, when any.
    /// </summary>
    public FileExplorerItem? SelectedItem => _selectedVisibleIndex >= 0 && _selectedVisibleIndex < _visible.Count
        ? _visible[_selectedVisibleIndex].Item
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Occurs when the selected node changes.
    /// </summary>
    public event EventHandler<FileExplorerSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Replaces root explorer nodes.
    /// </summary>
    /// <param name="items">Root nodes.</param>
    public void SetItems(IEnumerable<FileExplorerItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previousPath = SelectedPath;
        var previousItem = SelectedItem;

        _roots.Clear();
        foreach (var item in items)
        {
            _roots.Add(Clone(item));
        }

        RefreshVisible();
        RaiseSelectionChangedIfNeeded(previousPath, previousItem);
    }

    /// <summary>
    ///     Selects a node by path, expanding ancestors when needed.
    /// </summary>
    /// <param name="path">The path to select.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SelectPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        if (!TryFindPath(path, out var chain))
        {
            return false;
        }

        for (var index = 0; index < chain.Count - 1; index++)
        {
            if (chain[index].IsDirectory)
            {
                chain[index].IsExpanded = true;
            }
        }

        RefreshVisible();
        var targetIndex = _visible.FindIndex(entry => string.Equals(entry.Item.Path, path, StringComparison.Ordinal));
        return targetIndex >= 0 && SetSelectedVisibleIndex(targetIndex);
    }

    /// <summary>
    ///     Expands a directory by path.
    /// </summary>
    /// <param name="path">The directory path to expand.</param>
    /// <returns><see langword="true" /> when expansion changed state; otherwise <see langword="false" />.</returns>
    public bool ExpandPath(string path)
    {
        return SetExpandedState(path, true);
    }

    /// <summary>
    ///     Collapses a directory by path.
    /// </summary>
    /// <param name="path">The directory path to collapse.</param>
    /// <returns><see langword="true" /> when collapse changed state; otherwise <see langword="false" />.</returns>
    public bool CollapsePath(string path)
    {
        return SetExpandedState(path, false);
    }

    /// <summary>
    ///     Toggles expansion of the selected node if it is a directory.
    /// </summary>
    /// <returns><see langword="true" /> when expansion changed state; otherwise <see langword="false" />.</returns>
    public bool ToggleSelectedExpanded()
    {
        var selected = SelectedItem;
        if (selected is null || !selected.IsDirectory || selected.Children.Count == 0)
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
        if (content.IsEmpty || !content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                var hoverChanged = SetHoveredVisibleIndex(-1);
                if (pointer.Kind is not PointerEventKind.Wheel)
                {
                    return hoverChanged || Handle(message);
                }
            }

            return Handle(message);
        }

        var changed = false;

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

        EnsureSelectionVisible(content.Height);
        var row = pointer.Y - content.Y;
        var hoveredVisibleIndex = RowToVisibleIndex(row);
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
        if (content.IsEmpty)
        {
            return;
        }

        if (_visible.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(empty)", MutedStyle), content.Width);
            return;
        }

        EnsureSelectionVisible(content.Height);
        var rows = Math.Min(content.Height, _visible.Count - _scrollOffset);
        for (var row = 0; row < rows; row++)
        {
            var index = _scrollOffset + row;
            var entry = _visible[index];
            var marker = index == _selectedVisibleIndex ? ">" : " ";
            var indent = new string(' ', entry.Depth * 2);
            var glyph = "•";
            if (entry.Item.IsDirectory)
            {
                glyph = entry.Item.IsExpanded ? "▾" : "▸";
            }

            var style = entry.Item.IsDirectory ? DirectoryStyle : FileStyle;
            if (index == _hoveredVisibleIndex)
            {
                style = style.Merge(HoveredStyle);
            }

            if (index == _selectedVisibleIndex)
            {
                style = style.Merge(SelectedStyle);
            }

            if (IsDisabled)
            {
                style = style.Merge(MutedStyle);
            }

            var line = $"{marker} {indent}{glyph} {entry.Item.Name}";
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, Title?.Length + 4 ?? 12);
        for (var index = 0; index < _visible.Count; index++)
        {
            var entry = _visible[index];
            var lineWidth = 4 + entry.Depth * 2 + ControlTextLayout.MeasureDisplayWidth(entry.Item.Name);
            width = Math.Max(width, lineWidth);
        }

        var height = Math.Max(1, _visible.Count);
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

    private bool SetExpandedState(string path, bool expanded)
    {
        if (string.IsNullOrWhiteSpace(path) || !TryFindNode(path, out var node) || !node.IsDirectory)
        {
            return false;
        }

        if (node.IsExpanded == expanded)
        {
            return false;
        }

        node.IsExpanded = expanded;
        RefreshVisible();
        return true;
    }

    private bool ExpandOrMoveIntoChild()
    {
        var selected = SelectedItem;
        if (selected is null || !selected.IsDirectory || selected.Children.Count == 0)
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
        if (selected.Item.IsDirectory && selected.Item.IsExpanded && selected.Item.Children.Count > 0)
        {
            selected.Item.IsExpanded = false;
            RefreshVisible();
            return true;
        }

        return selected.ParentVisibleIndex is { } parent && SetSelectedVisibleIndex(parent);
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

        var previousPath = SelectedPath;
        var previousItem = SelectedItem;
        _selectedVisibleIndex = clamped;
        RaiseSelectionChangedIfNeeded(previousPath, previousItem);
        return true;
    }

    private string RenderTitle()
    {
        var value = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(value, IsFocused ? FocusedTitleStyle : TitleStyle);
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
            style = style.Merge(MutedStyle);
        }

        return style;
    }

    private void RefreshVisible()
    {
        var previousPath = SelectedPath;
        var previousItem = SelectedItem;

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
            RaiseSelectionChangedIfNeeded(previousPath, previousItem);
            return;
        }

        var preferredIndex = previousPath is null
            ? _selectedVisibleIndex
            : _visible.FindIndex(entry => string.Equals(entry.Item.Path, previousPath, StringComparison.Ordinal));
        _selectedVisibleIndex = Math.Clamp(preferredIndex < 0 ? 0 : preferredIndex, 0, _visible.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _visible.Count - 1));
        _hoveredVisibleIndex = _hoveredVisibleIndex >= 0 && _hoveredVisibleIndex < _visible.Count
            ? _hoveredVisibleIndex
            : -1;
        RaiseSelectionChangedIfNeeded(previousPath, previousItem);
    }

    private void AppendVisible(FileExplorerItem item, int depth, int? parentVisibleIndex)
    {
        var visibleIndex = _visible.Count;
        _visible.Add(new VisibleEntry(item, depth, parentVisibleIndex));
        if (!item.IsDirectory || !item.IsExpanded || item.Children.Count == 0)
        {
            return;
        }

        for (var i = 0; i < item.Children.Count; i++)
        {
            AppendVisible(item.Children[i], depth + 1, visibleIndex);
        }
    }

    private void EnsureSelectionVisible(int viewportHeight)
    {
        if (viewportHeight <= 0 || _visible.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedVisibleIndex < _scrollOffset)
        {
            _scrollOffset = _selectedVisibleIndex;
        }
        else if (_selectedVisibleIndex >= _scrollOffset + viewportHeight)
        {
            _scrollOffset = _selectedVisibleIndex - viewportHeight + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _visible.Count - viewportHeight));
    }

    private int RowToVisibleIndex(int row)
    {
        if (row < 0)
        {
            return -1;
        }

        var visibleIndex = _scrollOffset + row;
        return visibleIndex >= 0 && visibleIndex < _visible.Count
            ? visibleIndex
            : -1;
    }

    private bool SetHoveredVisibleIndex(int visibleIndex)
    {
        var normalized = visibleIndex >= 0 && visibleIndex < _visible.Count
            ? visibleIndex
            : -1;
        if (_hoveredVisibleIndex == normalized)
        {
            return false;
        }

        _hoveredVisibleIndex = normalized;
        return true;
    }
}
