using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>Virtualized single-selection list for large datasets.</summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class VirtualizedListView<T> : Control
{
    private static readonly IReadOnlyList<T> Empty = Array.Empty<T>();
    private readonly Func<T, string> _textSelector;
    private IReadOnlyList<T> _items = Empty;
    private Func<int, T>? _resolver;
    private int _count;
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _viewportHeight = 1;

    /// <summary>Creates a list with an optional text selector.</summary>
    public VirtualizedListView(Func<T, string>? textSelector = null) => _textSelector = textSelector ?? DefaultText;

    /// <summary>Raised when selection changes.</summary>
    public event EventHandler<ListSelectionChangedEventArgs<T>>? SelectionChanged;
    /// <summary>List title.</summary>
    public string Title { get; set; } = "Virtualized List";
    /// <summary>Focused title marker.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Whether to show <see cref="FocusMarker"/>.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Title style when unfocused.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Title style when focused.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Base row style.</summary>
    public TeaStyle DefaultRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Hovered row style merge.</summary>
    public TeaStyle HoveredRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Selected row style merge.</summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Disabled row style merge.</summary>
    public TeaStyle DisabledRowStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Unfocused border style.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Focused border style merge.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Text rendered when no rows exist.</summary>
    public string EmptyText { get; set; } = "(empty)";
    /// <summary>Frame border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Inner content padding.</summary>
    public Thickness Padding { get; set; }
    /// <summary>Advanced behavior options.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public VirtualizedListViewOptions Options { get; set; } = new();
    /// <summary>Total item count.</summary>
    public int Count => _count;
    /// <summary>Selected index or <c>-1</c>.</summary>
    public int SelectedIndex => _selectedIndex;
    /// <summary>Selected item when available.</summary>
    public T? SelectedItem => TryResolve(_selectedIndex, out var item) ? item : default;
    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Sets in-memory items.</summary>
    public void SetItems(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var prev = (_selectedIndex, CaptureSelectedItemForEvent());
        _resolver = null;
        _items = items;
        _count = items.Count;
        NormalizeAfterSourceChange();
        RaiseSelectionChangedIfNeeded(prev.Item1, prev.Item2);
    }

    /// <summary>Sets items from enumerable source.</summary>
    public void SetItems(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        SetItems(items is IReadOnlyList<T> list ? list : items.ToList());
    }

    /// <summary>Sets a virtualized index-based source.</summary>
    public void SetDataSource(int count, Func<int, T> itemResolver)
    {
        ArgumentNullException.ThrowIfNull(itemResolver);
        var prev = (_selectedIndex, CaptureSelectedItemForEvent());
        _items = Empty;
        _resolver = itemResolver;
        _count = Math.Max(0, count);
        NormalizeAfterSourceChange();
        RaiseSelectionChangedIfNeeded(prev.Item1, prev.Item2);
    }

    /// <summary>Clears data and selection.</summary>
    public void Clear()
    {
        var prev = (_selectedIndex, CaptureSelectedItemForEvent());
        _items = Empty;
        _resolver = null;
        _count = 0;
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(prev.Item1, prev.Item2);
    }

    /// <summary>Sets selected index with bounds clamping.</summary>
    public bool SetSelectedIndex(int index)
    {
        if (_count <= 0)
        {
            return false;
        }

        var next = Math.Clamp(index, 0, _count - 1);
        if (next == _selectedIndex)
        {
            return false;
        }

        var prev = (_selectedIndex, CaptureSelectedItemForEvent());
        _selectedIndex = next;
        EnsureSelectionVisible(_viewportHeight);
        RaiseSelectionChangedIfNeeded(prev.Item1, prev.Item2);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + Math.Max(1, _viewportHeight));
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - Math.Max(1, _viewportHeight));
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_count - 1);
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty) return Handle(message);
        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty) return Handle(message);

        _viewportHeight = Math.Max(1, content.Height);
        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHovered(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            var step = Math.Clamp(Options.WheelStep, 1, 32);
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + step) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - step) || changed;
            return changed;
        }

        if (!inside || _count == 0) return changed;
        var hovered = ResolveIndexFromPointer(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion) return SetHovered(hovered);
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            changed |= SetHovered(hovered);
            if (hovered >= 0) changed |= SetSelectedIndex(hovered);
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty) return;
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty) return;

        _viewportHeight = Math.Max(1, content.Height);
        if (_count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveRowStyle(selected: false, hovered: false)), content.Width);
            return;
        }

        EnsureSelectionVisible(content.Height);
        var visibleRows = Math.Min(content.Height, _count - _scrollOffset);
        for (var row = 0; row < visibleRows; row++)
        {
            var index = _scrollOffset + row;
            if (!TryResolve(index, out var item)) continue;
            var text = _textSelector(item);
            var prefix = index == _selectedIndex ? "> " : index == _hoveredIndex ? "~ " : "  ";
            canvas.WriteText(
                content.X,
                content.Y + row,
                ApplyStyle(string.Concat(prefix, text), ResolveRowStyle(index == _selectedIndex, index == _hoveredIndex)),
                content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 4);
        var height = 6 + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool TryResolve(int index, out T item)
    {
        if (index < 0 || index >= _count)
        {
            item = default!;
            return false;
        }

        if (_resolver is not null)
        {
            item = _resolver(index);
            return true;
        }

        item = _items[index];
        return true;
    }

    private void NormalizeAfterSourceChange()
    {
        if (_count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _count - 1);
        EnsureSelectionVisible(Math.Max(1, _viewportHeight));
    }

    private void EnsureSelectionVisible(int viewportHeight)
    {
        if (_count == 0 || viewportHeight <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < 0) _selectedIndex = 0;
        if (Options.KeepSelectionCentered && viewportHeight > 1)
        {
            var centered = _selectedIndex - (viewportHeight / 2);
            _scrollOffset = Math.Clamp(centered, 0, Math.Max(0, _count - viewportHeight));
            return;
        }

        if (_selectedIndex < _scrollOffset) _scrollOffset = _selectedIndex;
        else if (_selectedIndex >= _scrollOffset + viewportHeight) _scrollOffset = _selectedIndex - viewportHeight + 1;
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _count - viewportHeight));
    }

    private int ResolveIndexFromPointer(Rect content, int y)
    {
        if (y < content.Y || y >= content.Bottom) return -1;
        var index = _scrollOffset + (y - content.Y);
        return index >= 0 && index < _count ? index : -1;
    }

    private bool SetHovered(int index)
    {
        if (_hoveredIndex == index) return false;
        _hoveredIndex = index;
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, T? previousItem)
    {
        if (SelectionChanged is null) return;
        var currentItem = SelectedItem;
        if (previousIndex == _selectedIndex && ReferenceEquals(previousItem, currentItem)) return;
        SelectionChanged.Invoke(this, new ListSelectionChangedEventArgs<T>(previousIndex, _selectedIndex, previousItem, currentItem));
    }

    private TeaStyle ResolveRowStyle(bool selected, bool hovered)
    {
        var style = DefaultRowStyle;
        if (hovered) style = style.Merge(HoveredRowStyle);
        if (selected) style = style.Merge(SelectedRowStyle);
        if (IsDisabled) style = style.Merge(DisabledRowStyle);
        return style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledRowStyle) : style;
    }

    private string RenderTitle() => ApplyStyle(CurrentTitle(), IsFocused ? FocusedTitleStyle : TitleStyle);

    private string CurrentTitle() => IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title ?? string.Empty;

    private string MeasureTitle() => ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title ?? string.Empty;

    private static string ApplyStyle(string text, TeaStyle style) => string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);

    private static string DefaultText(T item) => item?.ToString() ?? string.Empty;

    private T? CaptureSelectedItemForEvent() => SelectionChanged is null ? default : SelectedItem;
}
