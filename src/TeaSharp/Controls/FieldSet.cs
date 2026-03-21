using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a titled selectable section block for grouped data-entry content.
/// </summary>
public sealed class FieldSet : Control
{
    private readonly List<string> _items = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when selected item changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<string>>? SelectionChanged;

    /// <summary>
    /// Gets or sets field-set title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Field Set";

    /// <summary>
    /// Gets or sets prefix rendered before title text.
    /// </summary>
    public string SectionPrefix
    {
        get;
        set => field = value ?? string.Empty;
    } = "[";

    /// <summary>
    /// Gets or sets suffix rendered after title text.
    /// </summary>
    public string SectionSuffix
    {
        get;
        set => field = value ?? string.Empty;
    } = "]";

    /// <summary>
    /// Gets or sets focus marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focus marker is shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets selected-row marker.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = ">";

    /// <summary>
    /// Gets or sets unselected-row marker.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets empty-state text.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(empty section)";

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner content padding.
    /// </summary>
    public Thickness Padding { get; set; }

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
    /// Gets or sets base item style.
    /// </summary>
    public TeaStyle ItemStyle { get; set; } = TeaStyle.Empty;

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
    /// Gets or sets style merged while disabled.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets empty-state style.
    /// </summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets configured items.
    /// </summary>
    public IReadOnlyList<string> Items => _items;

    /// <summary>
    /// Gets selected item index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets selected item text, if any.
    /// </summary>
    public string? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces all items in the section.
    /// </summary>
    /// <param name="items">Item text values.</param>
    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        foreach (var item in items)
        {
            _items.Add(item ?? string.Empty);
        }

        _selectedIndex = _items.Count == 0 ? -1 : Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _items.Count - 1);
        _hoveredIndex = -1;
        _scrollOffset = 0;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + Math.Max(1, _lastViewportRows - 1));
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - Math.Max(1, _lastViewportRows - 1));
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_items.Count - 1);
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

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + 1);
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - 1);
            return false;
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1) || Handle(message);
            }

            return Handle(message);
        }

        EnsureSelectionVisible(content.Height);
        var itemIndex = _scrollOffset + (pointer.Y - content.Y);
        if (itemIndex < 0 || itemIndex >= _items.Count)
        {
            return SetHoveredIndex(-1) || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(itemIndex);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            return SetSelectedIndex(itemIndex);
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

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveEmptyStyle()), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(content.Height);
        var visibleRows = Math.Min(content.Height, _items.Count - _scrollOffset);
        for (var row = 0; row < visibleRows; row++)
        {
            var index = _scrollOffset + row;
            var marker = index == _selectedIndex ? SelectedMarker : UnselectedMarker;
            var text = $"{marker} {_items[index]}";
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(text, ResolveItemStyle(index)), content.Width);
        }
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var i = 0; i < _items.Count; i++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth($"{SelectedMarker} {_items[i]}"));
        }

        var height = Math.Max(3, _items.Count + 2);
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool SetSelectedIndex(int index)
    {
        if (_items.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _items.Count - 1);
        if (_selectedIndex == clamped)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<string>(previousIndex, _selectedIndex, previousItem, _items[_selectedIndex]));
        return true;
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

    private void EnsureSelectionVisible(int viewportHeight)
    {
        if (_selectedIndex < 0 || _items.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var viewport = Math.Max(1, viewportHeight);
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
            return;
        }

        if (_selectedIndex >= _scrollOffset + viewport)
        {
            _scrollOffset = _selectedIndex - viewport + 1;
        }
    }

    private TeaStyle ResolveItemStyle(int index)
    {
        var style = ItemStyle;
        if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedItemStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TeaStyle ResolveEmptyStyle() => IsDisabled ? EmptyStyle.Merge(DisabledStyle) : EmptyStyle;

    private string RenderTitle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return ApplyStyle(MeasureTitle(), style);
    }

    private string MeasureTitle()
    {
        var title = $"{SectionPrefix}{Title}{SectionSuffix}";
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{title} {FocusMarker}"
            : title;
    }

    private static string ApplyStyle(string value, TeaStyle style) => style.IsEmpty ? value : style.Render(value);
}
