using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact single-line toolbar with keyboard and pointer selection.
/// </summary>
public sealed class Toolbar : Control
{
    private readonly List<ToolbarItem> _items = [];
    private int _selectedIndex;

    /// <summary>
    /// Occurs when the selected toolbar item changes.
    /// </summary>
    public event EventHandler<ToolbarSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets the configured toolbar items.
    /// </summary>
    public IReadOnlyList<ToolbarItem> Items => _items;

    /// <summary>
    /// Gets the currently selected index.
    /// Returns <c>-1</c> when no items are configured.
    /// </summary>
    public int SelectedIndex => _items.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public ToolbarItem? SelectedItem => _items.Count == 0 ? null : _items[_selectedIndex];

    /// <summary>
    /// Gets or sets the optional title shown before item labels.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be rendered when focused.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style applied when not focused.
    /// </summary>
    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style applied when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the separator text rendered between items.
    /// </summary>
    public string Separator
    {
        get;
        set => field = value ?? string.Empty;
    } = " | ";

    /// <summary>
    /// Gets or sets the base style applied to item labels.
    /// </summary>
    public TeaStyle ItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the selected item label.
    /// </summary>
    public TeaStyle SelectedItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the selected item when the toolbar has focus.
    /// </summary>
    public TeaStyle FocusedItemStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to separator text.
    /// </summary>
    public TeaStyle SeparatorStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

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
    /// Replaces the toolbar items.
    /// </summary>
    /// <param name="items">The items to display in visual order.</param>
    public void SetItems(IEnumerable<ToolbarItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;

        _items.Clear();
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            _items.Add(item);
        }

        _selectedIndex = _items.Count == 0 ? 0 : Math.Clamp(_selectedIndex, 0, _items.Count - 1);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || _items.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Right))
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

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || _items.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return Handle(message);
        }

        if (!bounds.Contains(pointer.X, pointer.Y) || pointer.Y != bounds.Y)
        {
            return Handle(message);
        }

        var index = HitTestItemIndex(pointer.X, bounds);
        return index >= 0 && SetSelectedIndex(index);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var x = clipped.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            canvas.WriteText(x, clipped.Y, RenderTitle(title), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        for (var index = 0; index < _items.Count && x < clipped.Right; index++)
        {
            if (index > 0)
            {
                canvas.WriteText(x, clipped.Y, RenderSeparator(Separator), clipped.Right - x);
                x += ControlTextLayout.MeasureDisplayWidth(Separator);
            }

            if (x >= clipped.Right)
            {
                break;
            }

            var label = FormatItemLabel(index);
            canvas.WriteText(x, clipped.Y, RenderItem(index, label), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(label);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            width += ControlTextLayout.MeasureDisplayWidth(title) + (_items.Count > 0 ? 1 : 0);
        }

        for (var index = 0; index < _items.Count; index++)
        {
            if (index > 0)
            {
                width += ControlTextLayout.MeasureDisplayWidth(Separator);
            }

            width += ControlTextLayout.MeasureDisplayWidth(FormatItemLabel(index));
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(width == 0 ? 0 : 1, 0, availableBounds.Height));
    }

    private bool SetSelectedIndex(int index)
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
        var previousItem = _items[previousIndex];
        _selectedIndex = clamped;
        SelectionChanged?.Invoke(
            this,
            new ToolbarSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, _items[_selectedIndex]));
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, ToolbarItem? previousItem)
    {
        var selectedIndex = SelectedIndex;
        var selectedItem = SelectedItem;
        if (previousIndex == selectedIndex
            && EqualityComparer<ToolbarItem?>.Default.Equals(previousItem, selectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new ToolbarSelectionChangedEventArgs(previousIndex, selectedIndex, previousItem, selectedItem));
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle(string title)
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return RenderStyled(title, style);
    }

    private string FormatItemLabel(int index)
    {
        var label = _items[index].Label ?? string.Empty;
        return index == _selectedIndex
            ? $"[{label}]"
            : label;
    }

    private string RenderItem(int index, string label)
    {
        var style = ItemStyle;
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedItemStyle);
            }
        }

        return RenderStyled(label, style);
    }

    private string RenderSeparator(string separator) => RenderStyled(separator, SeparatorStyle);

    private static string RenderStyled(string text, TeaStyle style)
    {
        if (style.IsEmpty || string.IsNullOrEmpty(text))
        {
            return text;
        }

        return style.Render(text);
    }

    private int HitTestItemIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            cursor += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        for (var index = 0; index < _items.Count && cursor < bounds.Right; index++)
        {
            if (index > 0)
            {
                cursor += ControlTextLayout.MeasureDisplayWidth(Separator);
            }

            var label = FormatItemLabel(index);
            var width = ControlTextLayout.MeasureDisplayWidth(label);
            var end = cursor + width;
            if (x >= cursor && x < end)
            {
                return index;
            }

            cursor = end;
        }

        return -1;
    }
}
