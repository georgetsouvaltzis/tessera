using Tessera.Components.Primitives;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a left-rail navigation control with keyboard and pointer selection.
/// </summary>
public sealed partial class SideNavRail : Control
{
    private readonly List<NavItem> _items = [];
    private SideNavRailGlyphSet _glyphs = SideNavRailGlyphSet.Default;
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Occurs when selection changes.
    /// </summary>
    public event EventHandler<SideNavRailSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs when the selected item is activated by keyboard or pointer interaction.
    /// </summary>
    public event EventHandler<SideNavRailActivatedEventArgs>? Activated;

    /// <summary>
    /// Gets or sets rail title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Navigation";

    /// <summary>
    /// Gets or sets marker appended to title while focused and <see cref="ShowFocusMarker" /> is enabled.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets style used by title while not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style used by title while focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets base item style.
    /// </summary>
    public TesseraStyle ItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged for hovered rows.
    /// </summary>
    public TesseraStyle HoveredItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged for selected rows.
    /// </summary>
    public TesseraStyle SelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged for selected rows while focused.
    /// </summary>
    public TesseraStyle FocusedSelectedItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when the control or row is disabled.
    /// </summary>
    public TesseraStyle DisabledItemStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style glyph styling while not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style glyph styling while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets rail border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether labels should be collapsed to icon/initial form.
    /// </summary>
    public bool IsCollapsed { get; set; }

    /// <summary>
    /// Gets or sets glyphs used by title and item rendering.
    /// </summary>
    public SideNavRailGlyphSet Glyphs
    {
        get => _glyphs;
        set => _glyphs = value;
    }

    /// <summary>
    /// Gets currently configured navigation items.
    /// </summary>
    public IReadOnlyList<NavItem> Items => _items;

    /// <summary>
    /// Gets currently selected index, or <c>-1</c> when no selectable item exists.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected item, or <see langword="null"/> when no item is selected.
    /// </summary>
    public NavItem? SelectedItem => TryGetItem(_selectedIndex, out var item) ? item : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces the navigation items shown by the rail.
    /// </summary>
    /// <param name="items">Items to render.</param>
    public void SetItems(IEnumerable<NavItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        var previousId = previousItem?.Id;

        _items.Clear();
        foreach (var item in items)
        {
            if (item is not null)
            {
                _items.Add(item);
            }
        }

        _hoveredIndex = -1;
        _selectedIndex = ResolveInitialSelection(previousId);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Sets the selected index using bounds clamping.
    /// </summary>
    /// <param name="index">Target index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _items.Count - 1);
        if (_items[clamped].IsDisabled)
        {
            return false;
        }

        return TrySetSelectedIndex(clamped, raiseEvent: true);
    }

    private bool SetCollapsed(bool collapsed)
    {
        if (IsCollapsed == collapsed)
        {
            return false;
        }

        IsCollapsed = collapsed;
        return true;
    }

    private bool MoveSelection(int delta)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        if (FindFirstEnabledIndex() < 0)
        {
            return false;
        }

        var candidate = _selectedIndex;
        if (candidate < 0)
        {
            candidate = delta > 0 ? -1 : _items.Count;
        }

        for (var attempt = 0; attempt < _items.Count; attempt++)
        {
            candidate += delta;
            if (candidate < 0)
            {
                candidate = _items.Count - 1;
            }
            else if (candidate >= _items.Count)
            {
                candidate = 0;
            }

            if (!_items[candidate].IsDisabled)
            {
                return TrySetSelectedIndex(candidate, raiseEvent: true);
            }
        }

        return false;
    }

    private bool SelectBoundary(bool forward)
    {
        var candidate = forward ? FindFirstEnabledIndex() : FindLastEnabledIndex();
        return candidate >= 0 && TrySetSelectedIndex(candidate, raiseEvent: true);
    }

    private bool TrySetSelectedIndex(int index, bool raiseEvent)
    {
        if (index < 0 || index >= _items.Count || _selectedIndex == index)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = index;
        if (raiseEvent)
        {
            SelectionChanged?.Invoke(this, new SideNavRailSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, SelectedItem));
        }

        return true;
    }

    private bool ActivateSelection()
    {
        var index = _selectedIndex;
        if (index < 0 || index >= _items.Count)
        {
            return false;
        }

        var item = _items[index];
        if (item.IsDisabled)
        {
            return false;
        }

        Activated?.Invoke(this, new SideNavRailActivatedEventArgs(index, item));
        return true;
    }

    private int ResolveInitialSelection(string? previousId)
    {
        if (_items.Count == 0)
        {
            return -1;
        }

        if (!string.IsNullOrEmpty(previousId))
        {
            for (var index = 0; index < _items.Count; index++)
            {
                if (!_items[index].IsDisabled
                    && string.Equals(_items[index].Id, previousId, StringComparison.Ordinal))
                {
                    return index;
                }
            }
        }

        return FindFirstEnabledIndex();
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, NavItem? previousItem)
    {
        if (previousIndex != _selectedIndex)
        {
            SelectionChanged?.Invoke(this, new SideNavRailSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, SelectedItem));
        }
    }

    private int FindFirstEnabledIndex()
    {
        for (var index = 0; index < _items.Count; index++)
        {
            if (!_items[index].IsDisabled)
            {
                return index;
            }
        }

        return -1;
    }

    private int FindLastEnabledIndex()
    {
        for (var index = _items.Count - 1; index >= 0; index--)
        {
            if (!_items[index].IsDisabled)
            {
                return index;
            }
        }

        return -1;
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

    private int ResolveItemIndexByPointer(Rect content, int pointerY)
    {
        if (_items.Count == 0 || content.Height < 2)
        {
            return -1;
        }

        var row = pointerY - (content.Y + 1);
        if (row < 0 || row >= Math.Min(_items.Count, content.Height - 1))
        {
            return -1;
        }

        return row;
    }

    private bool TryGetItem(int index, out NavItem? item)
    {
        if (index >= 0 && index < _items.Count)
        {
            item = _items[index];
            return true;
        }

        item = null;
        return false;
    }

}
