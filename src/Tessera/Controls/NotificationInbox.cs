using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a persistent notification inbox with keyboard and pointer navigation.
/// </summary>
public sealed class NotificationInbox : Control
{
    private readonly List<InboxItem> _items = [];
    private int _hoveredIndex = -1;
    private int _lastViewportRows = 8;
    private int _scrollOffset;
    private int _selectedIndex;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Notification Inbox";

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
    ///     Gets or sets the item style.
    /// </summary>
    public TesseraStyle ItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the selected item style.
    /// </summary>
    public TesseraStyle SelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the hovered item style.
    /// </summary>
    public TesseraStyle HoveredItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the unread item style.
    /// </summary>
    public TesseraStyle UnreadItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the muted item style.
    /// </summary>
    public TesseraStyle MutedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the info item style.
    /// </summary>
    public TesseraStyle InfoItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the success item style.
    /// </summary>
    public TesseraStyle SuccessItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the warning item style.
    /// </summary>
    public TesseraStyle WarningItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the error item style.
    /// </summary>
    public TesseraStyle ErrorItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the pinned item style.
    /// </summary>
    public TesseraStyle PinnedItemStyle { get; set; } = TesseraStyle.Empty;

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
    ///     Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 8;

    /// <summary>
    ///     Gets or sets the max items.
    /// </summary>
    public int MaxItems { get; set; } = 256;

    /// <summary>
    ///     Gets or sets whether show timestamp.
    /// </summary>
    public bool ShowTimestamp { get; set; } = true;

    /// <summary>
    ///     Gets or sets whether show source.
    /// </summary>
    public bool ShowSource { get; set; } = true;

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
    public string EmptyText { get; set; } = "(no notifications)";

    /// <summary>
    ///     Represents items.
    /// </summary>
    public IReadOnlyList<InboxItem> Items => _items;

    /// <summary>
    ///     Represents selected index.
    /// </summary>
    public int SelectedIndex => _items.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    ///     Represents selected item.
    /// </summary>
    public InboxItem? SelectedItem => _items.Count == 0 ? null : _items[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Occurs when the selected inbox item changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<InboxItem>>? SelectionChanged;

    /// <summary>
    ///     Executes set items.
    /// </summary>
    /// <param name="items">The items value.</param>
    public void SetItems(IEnumerable<InboxItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _items.Clear();
        foreach (var item in items)
        {
            _items.Add(Clone(item));
        }

        TrimToMaxItems();
        _selectedIndex = _items.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _items.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _items.Count - 1));
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, Math.Max(-1, _items.Count - 1));
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    ///     Executes add.
    /// </summary>
    /// <param name="message">The message value.</param>
    /// <param name="level">The level value.</param>
    /// <param name="source">The source value.</param>
    /// <param name="id">The id value.</param>
    /// <param name="createdAt">The created at value.</param>
    public void Add(
        string message,
        NotificationLevel level = NotificationLevel.Info,
        string? source = null,
        string? id = null,
        DateTimeOffset? createdAt = null)
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _items.Add(new InboxItem(
            id ?? Guid.NewGuid().ToString("n"),
            message,
            level,
            createdAt ?? DateTimeOffset.UtcNow,
            source));

        TrimToMaxItems();
        _selectedIndex = Math.Max(0, _items.Count - 1);
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    ///     Executes mark all read.
    /// </summary>
    public void MarkAllRead()
    {
        foreach (var item in _items)
        {
            item.IsRead = true;
        }
    }

    /// <summary>
    ///     Executes clear.
    /// </summary>
    public void Clear()
    {
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        _items.Clear();
        _selectedIndex = 0;
        _scrollOffset = 0;
        _hoveredIndex = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    ///     Executes select.
    /// </summary>
    /// <param name="index">The index value.</param>
    /// <returns><see langword="true" /> when select succeeds.</returns>
    public bool Select(int index)
    {
        return SetSelectedIndex(index);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || message is not KeyPressed key)
        {
            return false;
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

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            return MarkSelectedRead();
        }

        if (key.IsCharacter('r'))
        {
            _items[_selectedIndex].IsRead = !_items[_selectedIndex].IsRead;
            return true;
        }

        if (key.IsCharacter('p'))
        {
            _items[_selectedIndex].IsPinned = !_items[_selectedIndex].IsPinned;
            return true;
        }

        if (key.Is(Key.Delete) || key.IsCharacter('d'))
        {
            return RemoveSelected();
        }

        return key.IsCharacter('a')
            ? MarkAllReadAndHandle()
            : key.IsCharacter('c') && ClearAndHandle();
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = bounds.Inset(Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var headerRows = HasTitle() ? 1 : 0;
        var rowY = content.Y + headerRows;
        var rowsHeight = Math.Max(0, content.Height - headerRows);
        _lastViewportRows = Math.Max(1, rowsHeight);

        var inside = content.Contains(pointer.X, pointer.Y);
        if (!inside)
        {
            if (pointer.Kind is not PointerEventKind.Motion and not PointerEventKind.Press)
            {
                return Handle(message);
            }

            var changed = _hoveredIndex >= 0;
            _hoveredIndex = -1;
            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _items.Count > 0)
        {
            return HandleWheel(pointer.Button);
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

        return pointer.Kind switch
        {
            PointerEventKind.Motion => SetHoveredIndex(hovered),
            PointerEventKind.Press when pointer is { Button: PointerButton.Left } && hovered >= 0
                => HandlePress(hovered),
            _ => Handle(message)
        };
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
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
            var titleStyle = ResolveStyle(IsFocused ? FocusedTitleStyle : TitleStyle);
            canvas.WriteText(content.X, y, ApplyStyle(FormatTitle(), titleStyle), content.Width);
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
        var rowCount = Math.Min(rowsHeight, _items.Count - _scrollOffset);
        for (var row = 0; row < rowCount; row++)
        {
            var index = _scrollOffset + row;
            var selected = index == _selectedIndex;
            var hovered = index == _hoveredIndex;
            var item = _items[index];
            var style = ResolveLineStyle(item, selected, hovered);
            canvas.WriteText(content.X, y + row, ApplyStyle(BuildLine(item, selected), style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(FormatTitle()) + 2);
        width = _items.Count == 0
            ? width
            : Math.Max(width, _items.Max(item => ControlTextLayout.MeasureDisplayWidth(BuildLine(item, false))));

        var height = (HasTitle() ? 1 : 0) + Math.Max(1, Math.Min(PageSize, Math.Max(_items.Count, 1)));
        width += Padding.Horizontal;
        height += Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string BuildLine(InboxItem item, bool selected)
    {
        var marker = selected ? SelectedMarker : UnselectedMarker;
        var pin = item.IsPinned ? "*" : " ";
        var read = item.IsRead ? " " : "•";
        var stamp = ShowTimestamp
            ? $"{item.CreatedAt.ToUniversalTime().ToString("HH:mm", CultureInfo.InvariantCulture)} "
            : string.Empty;
        var source = ShowSource && !string.IsNullOrWhiteSpace(item.Source)
            ? $"[{item.Source.Trim()}] "
            : string.Empty;
        return $"{marker}{pin}{read} {stamp}{source}{item.Message}";
    }

    private TesseraStyle ResolveLineStyle(InboxItem item, bool selected, bool hovered)
    {
        var style = ItemStyle;
        if (selected)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredItemStyle);
        }

        style = style.Merge(item.IsRead ? MutedItemStyle : UnreadItemStyle);
        if (item.IsPinned)
        {
            style = style.Merge(PinnedItemStyle);
        }

        style = style.Merge(item.Level switch
        {
            NotificationLevel.Success => SuccessItemStyle,
            NotificationLevel.Warning => WarningItemStyle,
            NotificationLevel.Error => ErrorItemStyle,
            _ => InfoItemStyle
        });

        return ResolveStyle(style);
    }

    private bool RemoveSelected()
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = _items[_selectedIndex];
        _items.RemoveAt(_selectedIndex);
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, Math.Max(-1, _items.Count - 1));
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    private bool HandleWheel(PointerButton button)
    {
        return button switch
        {
            PointerButton.WheelDown => SetSelectedIndex(_selectedIndex + 1),
            PointerButton.WheelUp => SetSelectedIndex(_selectedIndex - 1),
            _ => false
        };
    }

    private bool HandlePress(int hovered)
    {
        RequestFocus();
        var changed = SetSelectedIndex(hovered);
        changed |= SetHoveredIndex(hovered);
        changed |= MarkSelectedRead();
        return changed;
    }

    private bool SetHoveredIndex(int hovered)
    {
        var changed = _hoveredIndex != hovered;
        _hoveredIndex = hovered;
        return changed;
    }

    private bool MarkSelectedRead()
    {
        if (_items[_selectedIndex].IsRead)
        {
            return false;
        }

        _items[_selectedIndex].IsRead = true;
        return true;
    }

    private bool ClearAndHandle()
    {
        Clear();
        return true;
    }

    private bool MarkAllReadAndHandle()
    {
        MarkAllRead();
        return true;
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

        var previousIndex = _selectedIndex;
        var previousItem = _items[_selectedIndex];
        _selectedIndex = next;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChanged(previousIndex, previousItem, _selectedIndex, _items[_selectedIndex]);
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

    private void TrimToMaxItems()
    {
        while (_items.Count > MaxItems)
        {
            _items.RemoveAt(0);
        }
    }

    private bool HasTitle()
    {
        return Title.Length > 0;
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

    private TesseraStyle ResolveStyle(TesseraStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private static InboxItem Clone(InboxItem item)
    {
        return new InboxItem(item.Id, item.Message, item.Level, item.CreatedAt, item.Source, item.IsRead,
            item.IsPinned);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, InboxItem? previousItem)
    {
        var selectedIndex = SelectedIndex;
        var selectedItem = SelectedItem;
        if (previousIndex == selectedIndex && IsSameItem(previousItem, selectedItem))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousItem, selectedIndex, selectedItem);
    }

    private void RaiseSelectionChanged(int previousIndex, InboxItem? previousItem, int selectedIndex,
        InboxItem? selectedItem)
    {
        SelectionChanged?.Invoke(
            this,
            new ListSelectionChangedEventArgs<InboxItem>(previousIndex, selectedIndex, previousItem, selectedItem));
    }

    private static bool IsSameItem(InboxItem? left, InboxItem? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return string.Equals(left.Id, right.Id, StringComparison.Ordinal);
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
