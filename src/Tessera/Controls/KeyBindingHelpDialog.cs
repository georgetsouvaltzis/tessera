using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a keyboard-shortcut help dialog for discoverability.
/// </summary>
public sealed class KeyBindingHelpDialog : Control
{
    private readonly List<KeyBindingItem> _items = [];
    private int _hoveredIndex = -1;
    private int _lastViewportRows = 8;
    private int _scrollOffset;
    private int _selectedIndex;

    /// <summary>
    ///     Gets or sets dialog title text.
    /// </summary>
    public string Title { get; set; } = "Keyboard Shortcuts";

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
    ///     Gets or sets the group style.
    /// </summary>
    public TesseraStyle GroupStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the keys style.
    /// </summary>
    public TesseraStyle KeysStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the description style.
    /// </summary>
    public TesseraStyle DescriptionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the selected row style.
    /// </summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the hovered row style.
    /// </summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the global binding style.
    /// </summary>
    public TesseraStyle GlobalBindingStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the disabled style.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the empty text style.
    /// </summary>
    public TesseraStyle EmptyTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets or sets whether show groups.
    /// </summary>
    public bool ShowGroups { get; set; } = true;

    /// <summary>
    ///     Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 8;

    /// <summary>
    ///     Gets or sets the key column width.
    /// </summary>
    public int KeyColumnWidth { get; set; } = 14;

    /// <summary>
    ///     Represents selected marker.
    /// </summary>
    public string SelectedMarker { get; set; } = ">";

    /// <summary>
    ///     Represents unselected marker.
    /// </summary>
    public string UnselectedMarker { get; set; } = " ";

    /// <summary>
    ///     Represents empty text.
    /// </summary>
    public string EmptyText { get; set; } = "(no key bindings)";

    /// <summary>
    ///     Gets or sets whether dialog is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    ///     Gets current key-binding rows.
    /// </summary>
    public IReadOnlyList<KeyBindingItem> Items => _items;

    /// <summary>
    ///     Gets selected row index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _items.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    ///     Gets selected row, or <see langword="null" /> when empty.
    /// </summary>
    public KeyBindingItem? SelectedItem => _items.Count == 0 ? null : _items[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Replaces key-binding rows.
    /// </summary>
    /// <param name="items">Rows to show.</param>
    public void SetItems(IEnumerable<KeyBindingItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        foreach (var item in items)
        {
            _items.Add(Clone(item));
        }

        _selectedIndex = _items.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _items.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, Math.Max(-1, _items.Count - 1));
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _items.Count - 1));
    }

    /// <summary>
    ///     Clears all key-binding rows.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        _selectedIndex = 0;
        _hoveredIndex = -1;
        _scrollOffset = 0;
    }

    /// <summary>
    ///     Shows the dialog and requests focus.
    /// </summary>
    public void Show()
    {
        IsVisible = true;
        RequestFocus();
    }

    /// <summary>
    ///     Hides the dialog.
    /// </summary>
    public void Hide()
    {
        IsVisible = false;
    }

    /// <summary>
    ///     Selects a row by index.
    /// </summary>
    /// <param name="index">Requested row index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool Select(int index)
    {
        return SetSelectedIndex(index);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsVisible || IsDisabled || IsReadOnly || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Escape))
        {
            Hide();
            return true;
        }

        if (_items.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_items.Count - 1);
        }

        var page = Math.Max(1, _lastViewportRows > 0 ? _lastViewportRows : PageSize);
        if (key.Is(Key.PageUp))
        {
            return SetSelectedIndex(_selectedIndex - page);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedIndex(_selectedIndex + page);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (!IsVisible || IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = bounds.Inset(Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var headerRows = HasTitle() ? 1 : 0;
        var rowY = content.Y + headerRows;
        var rowsHeight = Math.Max(0, content.Height - headerRows);
        _lastViewportRows = Math.Max(1, rowsHeight);

        if (!inside)
        {
            if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
            {
                Hide();
                return true;
            }

            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                var changed = _hoveredIndex >= 0;
                _hoveredIndex = -1;
                return changed;
            }

            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _items.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }
        }

        if (_items.Count == 0 || pointer.Y < rowY)
        {
            return Handle(message);
        }

        EnsureSelectionVisible(_lastViewportRows);
        var hovered = _scrollOffset + (pointer.Y - rowY);
        if (hovered < 0 || hovered >= _items.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            var changed = _hoveredIndex != hovered;
            _hoveredIndex = hovered;
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            var changed = SetSelectedIndex(hovered);
            _hoveredIndex = hovered;
            return changed;
        }

        return Handle(message);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = clipped.Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        var y = content.Y;
        if (HasTitle())
        {
            var style = ResolveStyle(IsFocused ? FocusedTitleStyle : TitleStyle);
            canvas.WriteText(content.X, y, ApplyStyle(FormatTitle(), style), content.Width);
            y++;
        }

        var rowsHeight = Math.Max(0, content.Bottom - y);
        _lastViewportRows = Math.Max(1, rowsHeight);
        if (_items.Count == 0 || rowsHeight <= 0)
        {
            if (rowsHeight > 0)
            {
                canvas.WriteText(content.X, y, ApplyStyle(EmptyText, ResolveStyle(EmptyTextStyle)), content.Width);
            }

            return;
        }

        EnsureSelectionVisible(_lastViewportRows);
        var keyWidth = ResolveKeyColumnWidth(content.Width);
        var rowCount = Math.Min(rowsHeight, _items.Count - _scrollOffset);
        for (var row = 0; row < rowCount; row++)
        {
            var index = _scrollOffset + row;
            var item = _items[index];
            var selected = index == _selectedIndex;
            var hovered = index == _hoveredIndex;
            var style = ResolveRowStyle(item, selected, hovered);
            canvas.WriteText(content.X, y + row, ApplyStyle(BuildLine(item, selected, keyWidth), style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(32, ControlTextLayout.MeasureDisplayWidth(FormatTitle()) + 2);
        var keyWidth = Math.Max(6, KeyColumnWidth);
        for (var i = 0; i < _items.Count; i++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(BuildLine(_items[i], false, keyWidth)));
        }

        var height = (HasTitle() ? 1 : 0) + Math.Max(1, Math.Min(PageSize, Math.Max(_items.Count, 1)));
        width += Padding.Horizontal;
        height += Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string BuildLine(KeyBindingItem item, bool selected, int keyWidth)
    {
        var marker = selected ? SelectedMarker : UnselectedMarker;
        var group = ShowGroups && !string.IsNullOrWhiteSpace(item.Group)
            ? $"[{item.Group.Trim()}] "
            : string.Empty;
        var keys = item.Keys.Length > keyWidth ? item.Keys[..keyWidth] : item.Keys.PadRight(keyWidth, ' ');
        var global = item.IsGlobal ? " (global)" : string.Empty;
        return $"{marker} {group}{keys}  {item.Description}{global}";
    }

    private TesseraStyle ResolveRowStyle(KeyBindingItem item, bool selected, bool hovered)
    {
        var style = KeysStyle.Merge(DescriptionStyle);
        if (ShowGroups && !string.IsNullOrWhiteSpace(item.Group))
        {
            style = style.Merge(GroupStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredRowStyle);
        }

        if (item.IsGlobal)
        {
            style = style.Merge(GlobalBindingStyle);
        }

        return ResolveStyle(style);
    }

    private bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var next = Math.Clamp(index, 0, _items.Count - 1);
        if (next == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = next;
        EnsureSelectionVisible(_lastViewportRows);
        return true;
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_items.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var rows = Math.Max(1, viewportRows);
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
            return;
        }

        if (_selectedIndex >= _scrollOffset + rows)
        {
            _scrollOffset = _selectedIndex - rows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _items.Count - rows));
    }

    private int ResolveKeyColumnWidth(int contentWidth)
    {
        var width = Math.Max(6, KeyColumnWidth);
        return Math.Min(width, Math.Max(6, contentWidth / 2 - 2));
    }

    private string FormatTitle()
    {
        if (!IsFocused || !ShowFocusMarker || string.IsNullOrWhiteSpace(FocusMarker) ||
            string.IsNullOrEmpty(Title))
        {
            return Title;
        }

        return $"{Title} {FocusMarker}";
    }

    private bool HasTitle()
    {
        return !string.IsNullOrEmpty(Title);
    }

    private TesseraStyle ResolveStyle(TesseraStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private static KeyBindingItem Clone(KeyBindingItem item)
    {
        return new KeyBindingItem(item.Keys, item.Description, item.Group, item.IsGlobal);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }
}
