using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable timeline-style feed for operational activity events.
/// </summary>
public sealed partial class ActivityFeed : Control
{
    private readonly List<ActivityFeedItem> _items = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<ActivityFeedItem>>? SelectionChanged;

    /// <summary>
    /// Gets or sets the control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Activity Feed";

    /// <summary>
    /// Gets or sets the marker appended to <see cref="Title"/> while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker"/> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

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
    /// Gets or sets style for info items.
    /// </summary>
    public TeaStyle InfoItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for success items.
    /// </summary>
    public TeaStyle SuccessItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for warning items.
    /// </summary>
    public TeaStyle WarningItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for error items.
    /// </summary>
    public TeaStyle ErrorItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TeaStyle HoveredItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TeaStyle FocusedSelectedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into unread items.
    /// </summary>
    public TeaStyle UnreadItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into muted items.
    /// </summary>
    public TeaStyle MutedItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rows while disabled.
    /// </summary>
    public TeaStyle DisabledItemStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for timestamp text.
    /// </summary>
    public TeaStyle TimestampStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for empty-state text.
    /// </summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether timestamps are rendered.
    /// </summary>
    public bool ShowTimestamp { get; set; } = true;

    /// <summary>
    /// Gets or sets the timestamp format string.
    /// </summary>
    public string TimestampFormat
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? "HH:mm:ss" : value;
    } = "HH:mm:ss";

    /// <summary>
    /// Gets or sets a value indicating whether appending auto-selects the latest item.
    /// </summary>
    public bool AutoFollow { get; set; } = true;

    /// <summary>
    /// Gets or sets maximum retained items. Use <c>0</c> for unlimited.
    /// </summary>
    public int MaxItems { get; set; } = 2000;

    /// <summary>
    /// Gets or sets marker rendered for the selected row.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▶";

    /// <summary>
    /// Gets or sets marker rendered for non-selected read rows.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "·";

    /// <summary>
    /// Gets or sets marker rendered for non-selected unread rows.
    /// </summary>
    public string UnreadMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "●";

    /// <summary>
    /// Gets or sets text rendered when no items are present.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no activity)";

    /// <summary>
    /// Gets the current feed items.
    /// </summary>
    public IReadOnlyList<ActivityFeedItem> Items => _items;

    /// <summary>
    /// Gets selected index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected item, if any.
    /// </summary>
    public ActivityFeedItem? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces all feed items.
    /// </summary>
    /// <param name="items">Items to render.</param>
    public void SetItems(IEnumerable<ActivityFeedItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        foreach (var item in items)
        {
            if (item is not null)
            {
                _items.Add(Clone(item));
            }
        }

        if (_items.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _items.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, _items.Count - 1);
    }

    /// <summary>
    /// Appends one feed item.
    /// </summary>
    /// <param name="item">Item to append.</param>
    public void Append(ActivityFeedItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(Clone(item));
        TrimToMaxItems();
        if (AutoFollow)
        {
            _ = SetSelectedIndex(_items.Count - 1);
            EnsureSelectionVisible(_lastViewportRows);
            return;
        }

        if (_selectedIndex < 0 && _items.Count > 0)
        {
            _selectedIndex = 0;
        }
    }

    /// <summary>
    /// Appends one feed item from primitive values.
    /// </summary>
    /// <param name="actor">Actor identifier.</param>
    /// <param name="action">Action text.</param>
    /// <param name="target">Optional target text.</param>
    /// <param name="details">Optional detail text.</param>
    /// <param name="kind">Item kind for row styling.</param>
    /// <param name="timestamp">Optional timestamp.</param>
    public void Append(
        string actor,
        string action,
        string? target = null,
        string? details = null,
        ActivityFeedItemKind kind = ActivityFeedItemKind.Info,
        DateTimeOffset? timestamp = null)
    {
        Append(new ActivityFeedItem(actor, action, target, details, kind, timestamp));
    }

    /// <summary>
    /// Clears all feed items.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollOffset = 0;
    }

    /// <summary>
    /// Sets selected row index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
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
        SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<ActivityFeedItem>(previousIndex, _selectedIndex, previousItem, SelectedItem));
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var page = Math.Max(1, _lastViewportRows > 0 ? _lastViewportRows : 8);
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_items.Count - 1);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + page);
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - page);
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHoveredIndex(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _items.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + 1) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - 1) || changed;
        }

        if (!inside)
        {
            return changed;
        }

        EnsureSelectionVisible(content.Height);
        var hovered = _scrollOffset + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _items.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            return changed;
        }

        return changed;
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

        if (_items.Count == 0)
        {
            var emptyStyle = IsDisabled ? EmptyStyle.Merge(DisabledItemStyle) : EmptyStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, emptyStyle), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var visible = Math.Min(content.Height, _items.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var itemIndex = _scrollOffset + row;
            var line = FormatLine(_items[itemIndex], itemIndex == _selectedIndex);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveItemStyle(itemIndex)), content.Width);
        }
    }

}
