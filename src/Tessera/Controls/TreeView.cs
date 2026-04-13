using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a hierarchical tree viewer.
/// </summary>
public sealed class TreeView : Control
{
    private readonly List<TreeItem> _roots = [];
    private readonly List<(TreeItem Node, int Depth, int? ParentVisibleIndex)> _visible = [];
    private int _hoveredIndex = -1;
    private int _selectedIndex;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Tree";

    /// <summary>
    ///     Represents focus marker.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether show focus marker.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets the title style.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the branch style.
    /// </summary>
    public TesseraStyle BranchStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the leaf style.
    /// </summary>
    public TesseraStyle LeafStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the selected item style.
    /// </summary>
    public TesseraStyle SelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the hovered item style.
    /// </summary>
    public TesseraStyle HoveredItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the muted style.
    /// </summary>
    public TesseraStyle MutedStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the disabled style.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets glyphs used for branch and leaf markers.
    /// </summary>
    public TreeViewGlyphSet Glyphs { get; set; } = TreeViewGlyphSet.Default;

    /// <summary>
    ///     Represents selected id.
    /// </summary>
    public string? SelectedId => _selectedIndex >= 0 && _selectedIndex < _visible.Count
        ? _visible[_selectedIndex].Node.Id
        : null;

    /// <summary>
    ///     Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    ///     Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    ///     Executes set items.
    /// </summary>
    /// <param name="items">The items value.</param>
    public void SetItems(IEnumerable<TreeItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _roots.Clear();
        foreach (var item in items)
        {
            _roots.Add(Clone(item));
        }

        RefreshVisible();
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key || _visible.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return MoveSelection(+1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return MoveSelection(-1);
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
            var node = _visible[_selectedIndex].Node;
            if (node.Children.Count == 0)
            {
                return false;
            }

            node.Expanded = !node.Expanded;
            RefreshVisible();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || _visible.Count == 0)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            if (pointer.Kind is not PointerEventKind.Wheel)
            {
                return changed || Handle(message);
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            return pointer.Button switch
            {
                PointerButton.WheelDown => MoveSelection(+1) || changed,
                PointerButton.WheelUp => MoveSelection(-1) || changed,
                _ => changed || Handle(message)
            };
        }

        if (!inside)
        {
            return changed || Handle(message);
        }

        var hovered = ComputeWindowStart(content.Height) + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _visible.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer is not { Kind: PointerEventKind.Press, Button: PointerButton.Left } || hovered < 0)
        {
            return changed || Handle(message);
        }

        changed |= SetHoveredIndex(hovered);
        if (_selectedIndex == hovered)
        {
            return changed;
        }

        _selectedIndex = hovered;
        return true;
    }

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

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_visible.Count, start + content.Height);
        for (var row = 0; row < end - start; row++)
        {
            var index = start + row;
            var (node, depth, _) = _visible[index];
            var indent = new string(' ', Math.Max(0, depth) * 2);
            var marker = ResolveNodeMarker(node);
            var cursor = index == _selectedIndex ? ">" : " ";
            canvas.WriteText(
                content.X,
                content.Y + row,
                ApplyStyle($"{cursor} {indent}{marker} {node.Label}",
                    ResolveRowStyle(node, index == _selectedIndex, index == _hoveredIndex)),
                content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, Title.Length + 4);
        if (_visible.Count > 0)
        {
            foreach (var entry in _visible)
            {
                var rowWidth = entry.Depth * 2
                               + ResolveTreePrefixWidth(entry.Node)
                               + ControlTextLayout.MeasureDisplayWidth(entry.Node.Label);
                width = Math.Max(width, rowWidth);
            }
        }

        var height = Math.Max(1, _visible.Count);
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool MoveSelection(int delta)
    {
        var next = delta > 0
            ? Math.Min(_visible.Count - 1, _selectedIndex + delta)
            : Math.Max(0, _selectedIndex + delta);
        if (next == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = next;
        return true;
    }

    private bool ExpandOrMoveIntoChild()
    {
        var node = _visible[_selectedIndex].Node;
        if (node.Children.Count == 0)
        {
            return false;
        }

        if (!node.Expanded)
        {
            node.Expanded = true;
            RefreshVisible();
            return true;
        }

        if (_selectedIndex + 1 >= _visible.Count || _visible[_selectedIndex + 1].Depth <= _visible[_selectedIndex].Depth)
        {
            return false;
        }

        _selectedIndex++;
        return true;
    }

    private bool CollapseOrMoveToParent()
    {
        var entry = _visible[_selectedIndex];
        if (entry.Node.Expanded && entry.Node.Children.Count > 0)
        {
            entry.Node.Expanded = false;
            RefreshVisible();
            return true;
        }

        if (entry.ParentVisibleIndex is not { } parent)
        {
            return false;
        }

        _selectedIndex = parent;
        return true;
    }

    private void RefreshVisible()
    {
        _visible.Clear();
        foreach (var root in _roots)
        {
            AppendVisible(root, 0, null);
        }

        if (_visible.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _visible.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _visible.Count - 1);
    }

    private void AppendVisible(TreeItem node, int depth, int? parentVisibleIndex)
    {
        var visibleIndex = _visible.Count;
        _visible.Add((node, depth, parentVisibleIndex));
        if (!node.Expanded || node.Children.Count == 0)
        {
            return;
        }

        foreach (var child in node.Children)
        {
            AppendVisible(child, depth + 1, visibleIndex);
        }
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return FrameLayout.ResolveContentRect(bounds, Border, Padding);
    }

    private int ComputeWindowStart(int contentHeight)
    {
        return Math.Clamp(_selectedIndex - contentHeight / 2, 0, Math.Max(0, _visible.Count - contentHeight));
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

    private static TreeItem Clone(TreeItem item)
    {
        var clone = new TreeItem(item.Id, item.Label, item.Children.Select(Clone)) { Expanded = item.Expanded };
        return clone;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && FocusMarker.Length > 0
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TesseraStyle ResolveRowStyle(TreeItem node, bool selected, bool hovered)
    {
        var style = node.Children.Count == 0 ? LeafStyle : BranchStyle;
        if (selected)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private string ResolveNodeMarker(TreeItem node)
    {
        if (node.Children.Count == 0)
        {
            return Glyphs.LeafMarker;
        }

        return node.Expanded
            ? Glyphs.ExpandedBranchMarker
            : Glyphs.CollapsedBranchMarker;
    }

    private int ResolveTreePrefixWidth(TreeItem node)
    {
        return 3 + ControlTextLayout.MeasureDisplayWidth(ResolveNodeMarker(node));
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = IsFocused
            ? BorderStyleText.Merge(FocusedBorderStyleText)
            : BorderStyleText;

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
