using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable jump-list control with activation support.
/// </summary>
public sealed partial class JumpList : Control
{
    private readonly List<JumpListItem> _items = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<JumpListItem>>? SelectionChanged;

    /// <summary>
    /// Occurs when an item is activated.
    /// </summary>
    public event EventHandler<JumpListActivatedEventArgs>? Activated;

    /// <summary>
    /// Gets or sets control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Jump List";

    /// <summary>
    /// Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focus marker should be rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets glyphs used for row markers.
    /// </summary>
    public JumpListGlyphSet Glyphs { get; set; } = JumpListGlyphSet.Default;

    /// <summary>
    /// Gets or sets title style while not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style while not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style while focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets base row style.
    /// </summary>
    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected row.
    /// </summary>
    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected row while focused.
    /// </summary>
    public TeaStyle FocusedSelectedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered row.
    /// </summary>
    public TeaStyle HoveredItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into disabled rows.
    /// </summary>
    public TeaStyle DisabledItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to pinned marker text.
    /// </summary>
    public TeaStyle PinnedMarkerStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to recent marker text.
    /// </summary>
    public TeaStyle RecentMarkerStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets text shown when no items are available.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no items)";

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets control padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets configured items.
    /// </summary>
    public IReadOnlyList<JumpListItem> Items => _items;

    /// <summary>
    /// Gets selected index, or <c>-1</c> when no items are available.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected item, or <see langword="null" /> when no items are available.
    /// </summary>
    public JumpListItem? SelectedItem =>
        _selectedIndex >= 0 && _selectedIndex < _items.Count
            ? _items[_selectedIndex]
            : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces jump-list items.
    /// </summary>
    /// <param name="items">Items in visual order.</param>
    public void SetItems(IEnumerable<JumpListItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        var selectedId = previousItem?.Id;

        _items.Clear();
        foreach (var item in items)
        {
            if (item is not null)
            {
                _items.Add(item with { });
            }
        }

        if (_items.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
        }
        else
        {
            _selectedIndex = ResolveSelectedIndex(selectedId);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Sets selected index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested selected index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _items.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = clamped;
        RaiseSelectionChanged(previousIndex, previousItem);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_items.Count - 1);
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            return ActivateSelected();
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        if (IsDisabled || _items.Count == 0)
        {
            return false;
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return false;
        }

        var listTop = ResolveListTop(content);
        var listHeight = Math.Max(0, content.Bottom - listTop);
        if (listHeight <= 0)
        {
            return false;
        }

        var hovered = ResolveRowIndex(pointer.X, pointer.Y, content.X, listTop, content.Width, listHeight);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            if (hovered < 0)
            {
                return false;
            }

            RequestFocus();
            var changed = SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            if (!IsReadOnly)
            {
                changed |= ActivateSelected();
            }

            return changed;
        }

        return false;
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
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (Border == BorderStyle.None)
        {
            var inlineTitle = RenderTitle();
            if (!string.IsNullOrEmpty(inlineTitle))
            {
                canvas.WriteText(content.X, content.Y, inlineTitle, content.Width);
            }
        }

        var listTop = ResolveListTop(content);
        var listHeight = Math.Max(0, content.Bottom - listTop);
        if (_items.Count == 0 || listHeight == 0)
        {
            if (listHeight > 0)
            {
                canvas.WriteText(content.X, listTop, EmptyText, content.Width);
            }

            return;
        }

        var visible = Math.Min(listHeight, _items.Count);
        for (var row = 0; row < visible; row++)
        {
            var item = _items[row];
            var line = BuildLine(item, row == _selectedIndex);
            var style = ResolveRowStyle(item, row);
            canvas.WriteText(content.X, listTop + row, ApplyStyle(line, style), content.Width);
        }
    }

    private bool ActivateSelected()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _items.Count || IsReadOnly)
        {
            return false;
        }

        var selected = _items[_selectedIndex];
        if (selected.IsDisabled)
        {
            return false;
        }

        Activated?.Invoke(this, new JumpListActivatedEventArgs(_selectedIndex, selected));
        return true;
    }

}
