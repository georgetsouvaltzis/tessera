using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a compact single-line breadcrumb navigator.
/// </summary>
public sealed class Breadcrumb : Control
{
    private readonly List<BreadcrumbItem> _items = [];
    private int _selectedIndex;

    /// <summary>
    /// Occurs when the selected breadcrumb item changes.
    /// </summary>
    public event EventHandler<BreadcrumbSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets the configured breadcrumb items.
    /// </summary>
    public IReadOnlyList<BreadcrumbItem> Items => _items;

    /// <summary>
    /// Gets the current selected index.
    /// Returns <c>-1</c> when no items are configured.
    /// </summary>
    public int SelectedIndex => _items.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets the current selected item.
    /// </summary>
    public BreadcrumbItem? SelectedItem => _items.Count == 0 ? null : _items[_selectedIndex];

    /// <summary>
    /// Gets or sets the optional title shown before breadcrumb items.
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
    /// Gets or sets a value indicating whether the title focus marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the style applied to the title when not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to the title when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the separator rendered between breadcrumb items.
    /// </summary>
    public string Separator
    {
        get;
        set => field = value ?? string.Empty;
    } = "/";

    /// <summary>
    /// Gets or sets the marker rendered in front of the selected item.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "›";

    /// <summary>
    /// Gets or sets the base style used for breadcrumb item labels.
    /// </summary>
    public TesseraStyle ItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged on top of <see cref="ItemStyle"/> for the selected item.
    /// </summary>
    public TesseraStyle SelectedItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style used for separator rendering.
    /// </summary>
    public TesseraStyle SeparatorStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

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
    /// Replaces the breadcrumb item list.
    /// </summary>
    /// <param name="items">The items to display in order.</param>
    public void SetItems(IEnumerable<BreadcrumbItem> items)
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

        if (key.Is(Key.Right))
        {
            return SetSelectedIndex((_selectedIndex + 1) % _items.Count);
        }

        if (key.Is(Key.Left))
        {
            return SetSelectedIndex((_selectedIndex + _items.Count - 1) % _items.Count);
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

        var hit = HitTestItemIndex(pointer.X, bounds);
        return hit >= 0 && SetSelectedIndex(hit);
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
                var separator = FormatSeparator();
                canvas.WriteText(x, clipped.Y, RenderSeparator(separator), clipped.Right - x);
                x += ControlTextLayout.MeasureDisplayWidth(separator);
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
                width += ControlTextLayout.MeasureDisplayWidth(FormatSeparator());
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
            new BreadcrumbSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, _items[_selectedIndex]));
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, BreadcrumbItem? previousItem)
    {
        var selectedIndex = SelectedIndex;
        var selectedItem = SelectedItem;
        if (previousIndex == selectedIndex
            && EqualityComparer<BreadcrumbItem?>.Default.Equals(previousItem, selectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new BreadcrumbSelectionChangedEventArgs(previousIndex, selectedIndex, previousItem, selectedItem));
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

    private string FormatSeparator()
    {
        return $" {Separator} ";
    }

    private string RenderSeparator(string separator)
    {
        return RenderStyled(separator, SeparatorStyle);
    }

    private string FormatItemLabel(int index)
    {
        var label = _items[index].Label ?? string.Empty;
        if (index == _selectedIndex && !string.IsNullOrWhiteSpace(SelectedMarker))
        {
            return $"{SelectedMarker} {label}";
        }

        return label;
    }

    private string RenderItem(int index, string label)
    {
        var style = ItemStyle;
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        return RenderStyled(label, style);
    }

    private static string RenderStyled(string text, TesseraStyle style)
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
                cursor += ControlTextLayout.MeasureDisplayWidth(FormatSeparator());
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
