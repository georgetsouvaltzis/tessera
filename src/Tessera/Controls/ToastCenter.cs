using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a transient toast-notification center with queueing, selection, and dismissal.
/// </summary>
public sealed partial class ToastCenter : Control
{
    private readonly List<ToastItem> _items = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    /// <summary>Gets or sets the title rendered in the frame header when a border is enabled.</summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Toasts";

    /// <summary>Gets or sets the marker shown in the title when focused.</summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>Gets or sets a value indicating whether the title focus marker should be rendered.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets the border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets the inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets the maximum number of queued toasts retained in memory.</summary>
    public int MaxItems { get; set; } = 128;

    /// <summary>Gets or sets the maximum number of visible rows rendered for toasts.</summary>
    public int VisibleCapacity { get; set; } = 5;

    /// <summary>
    /// Gets or sets the default timeout metadata assigned by
    /// <see cref="Push(string, NotificationLevel, string?, TimeSpan?)" />.
    /// Set to <see langword="null" /> for non-expiring toasts.
    /// </summary>
    public TimeSpan? DefaultTimeout { get; set; } = TimeSpan.FromSeconds(8);

    /// <summary>Gets or sets a value indicating whether expired toasts should be removed on each render pass.</summary>
    public bool AutoDismissExpired { get; set; } = true;

    /// <summary>Gets or sets the title style applied when not focused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the title style applied when focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style applied to border glyphs when the control is not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style applied to border glyphs when the control is focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the base style used for toast rows.</summary>
    public TesseraStyle ItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for hovered rows.</summary>
    public TesseraStyle HoveredItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for the selected row.</summary>
    public TesseraStyle SelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for muted rows.</summary>
    public TesseraStyle MutedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for info-level rows.</summary>
    public TesseraStyle InfoItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for success-level rows.</summary>
    public TesseraStyle SuccessItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for warning-level rows.</summary>
    public TesseraStyle WarningItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged for error-level rows.</summary>
    public TesseraStyle ErrorItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets queued toast items in insertion order.</summary>
    public IReadOnlyList<ToastItem> Items => _items;

    /// <summary>Gets the queued toast count.</summary>
    public int Count => _items.Count;

    /// <summary>Gets the selected toast index. Returns <c>-1</c> when the queue is empty.</summary>
    public int SelectedIndex => _items.Count == 0 ? -1 : _selectedIndex;

    /// <summary>Gets the selected toast item.</summary>
    public ToastItem? SelectedItem => _items.Count == 0 ? null : _items[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Pushes a new toast into the queue and selects it.
    /// </summary>
    /// <param name="message">Toast message text.</param>
    /// <param name="level">Toast severity level.</param>
    /// <param name="id">Optional stable identifier. A generated id is used when omitted.</param>
    /// <param name="timeout">Optional timeout metadata. Falls back to <see cref="DefaultTimeout" /> when omitted.</param>
    /// <returns>The queued toast item.</returns>
    public ToastItem Push(
        string message,
        NotificationLevel level = NotificationLevel.Info,
        string? id = null,
        TimeSpan? timeout = null)
    {
        var item = new ToastItem(
            id ?? Guid.NewGuid().ToString("n"),
            message ?? string.Empty,
            level,
            DateTimeOffset.UtcNow,
            timeout ?? DefaultTimeout);
        _items.Add(item);
        TrimToMaxItems();
        _selectedIndex = Math.Max(0, _items.Count - 1);
        _hoveredIndex = -1;
        return item;
    }

    /// <summary>Removes all queued toasts.</summary>
    public void Clear()
    {
        _items.Clear();
        _selectedIndex = 0;
        _hoveredIndex = -1;
    }

    /// <summary>Dismisses the currently selected toast.</summary>
    /// <returns><see langword="true" /> when a toast was dismissed; otherwise, <see langword="false" />.</returns>
    public bool DismissSelected()
    {
        return _items.Count > 0 && DismissAt(_selectedIndex);
    }

    /// <summary>Dismisses a toast by id.</summary>
    /// <param name="id">The toast id to remove.</param>
    /// <returns><see langword="true" /> when a toast was dismissed; otherwise, <see langword="false" />.</returns>
    public bool Dismiss(string id)
    {
        if (string.IsNullOrEmpty(id) || _items.Count == 0)
        {
            return false;
        }

        for (var index = 0; index < _items.Count; index++)
        {
            if (string.Equals(_items[index].Id, id, StringComparison.Ordinal))
            {
                return DismissAt(index);
            }
        }

        return false;
    }

    /// <summary>Removes expired toasts according to timeout metadata.</summary>
    /// <param name="utcNow">Optional UTC timestamp used for expiration comparison.</param>
    /// <returns>The number of removed toasts.</returns>
    public int DismissExpired(DateTimeOffset? utcNow = null)
    {
        if (_items.Count == 0)
        {
            return 0;
        }

        var now = utcNow ?? DateTimeOffset.UtcNow;
        var removed = 0;
        for (var index = _items.Count - 1; index >= 0; index--)
        {
            if (!IsExpired(_items[index], now))
            {
                continue;
            }

            _items.RemoveAt(index);
            removed++;
            if (_selectedIndex > index)
            {
                _selectedIndex--;
            }

            if (_hoveredIndex == index)
            {
                _hoveredIndex = -1;
            }
            else if (_hoveredIndex > index)
            {
                _hoveredIndex--;
            }
        }

        if (_items.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex, 0, _items.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
        }

        return removed;
    }

    /// <summary>Sets muted state for a toast by id.</summary>
    /// <param name="id">The toast id.</param>
    /// <param name="isMuted"><see langword="true" /> to mute; otherwise, <see langword="false" />.</param>
    /// <returns><see langword="true" /> when state changed; otherwise, <see langword="false" />.</returns>
    public bool SetMuted(string id, bool isMuted = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        for (var index = 0; index < _items.Count; index++)
        {
            if (string.Equals(_items[index].Id, id, StringComparison.Ordinal))
            {
                return SetMutedAt(index, isMuted);
            }
        }

        return false;
    }

    /// <summary>Sets muted state for the selected toast.</summary>
    /// <param name="isMuted"><see langword="true" /> to mute; otherwise, <see langword="false" />.</param>
    /// <returns><see langword="true" /> when state changed; otherwise, <see langword="false" />.</returns>
    public bool SetMutedSelected(bool isMuted = true)
    {
        return _items.Count > 0 && SetMutedAt(_selectedIndex, isMuted);
    }

    /// <summary>Toggles muted state for the selected toast.</summary>
    /// <returns><see langword="true" /> when state changed; otherwise, <see langword="false" />.</returns>
    public bool ToggleMutedSelected()
    {
        return _items.Count > 0 && SetMutedAt(_selectedIndex, !_items[_selectedIndex].IsMuted);
    }

    private static bool IsExpired(ToastItem item, DateTimeOffset now)
    {
        if (!item.Timeout.HasValue)
        {
            return false;
        }

        if (item.Timeout.Value <= TimeSpan.Zero)
        {
            return true;
        }

        return item.CreatedAtUtc + item.Timeout.Value <= now;
    }

    private bool DismissAt(int index)
    {
        if (index < 0 || index >= _items.Count)
        {
            return false;
        }

        _items.RemoveAt(index);
        if (_items.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            return true;
        }

        if (_selectedIndex > index)
        {
            _selectedIndex--;
        }
        else if (_selectedIndex == _items.Count)
        {
            _selectedIndex = _items.Count - 1;
        }

        if (_hoveredIndex == index)
        {
            _hoveredIndex = -1;
        }
        else if (_hoveredIndex > index)
        {
            _hoveredIndex--;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _items.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
        return true;
    }

    private bool SetMutedAt(int index, bool isMuted)
    {
        if (index < 0 || index >= _items.Count || _items[index].IsMuted == isMuted)
        {
            return false;
        }

        _items[index] = _items[index] with { IsMuted = isMuted };
        return true;
    }

    private void TrimToMaxItems()
    {
        var maxItems = Math.Max(1, MaxItems);
        if (_items.Count <= maxItems)
        {
            return;
        }

        var overflow = _items.Count - maxItems;
        _items.RemoveRange(0, overflow);
        _selectedIndex = Math.Clamp(_selectedIndex - overflow, 0, Math.Max(0, _items.Count - 1));
        _hoveredIndex = Math.Clamp(_hoveredIndex - overflow, -1, Math.Max(-1, _items.Count - 1));
    }
}
