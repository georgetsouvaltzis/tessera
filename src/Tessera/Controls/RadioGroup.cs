using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a single-choice group of radio options.
/// </summary>
public sealed class RadioGroup : Control
{
    private readonly List<string> _items = [];
    private int _hoveredIndex = -1;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Radio";

    /// <summary>
    ///     Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets a value indicating whether the title focus marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Gets or sets the title style applied when not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the title style applied when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the base style applied to item rows.
    /// </summary>
    public TesseraStyle ItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into selected rows.
    /// </summary>
    public TesseraStyle SelectedItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into hovered rows.
    /// </summary>
    public TesseraStyle HoveredItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged when the control is disabled.
    /// </summary>
    public TesseraStyle DisabledItemStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the marker shown for selected rows.
    /// </summary>
    public string SelectedMarker { get; set; } = "(•)";

    /// <summary>
    ///     Gets or sets the marker shown for unselected rows.
    /// </summary>
    public string UnselectedMarker { get; set; } = "( )";

    /// <summary>
    ///     Gets or sets the selected index.
    /// </summary>
    public int SelectedIndex { get; private set; }

    /// <summary>
    ///     Represents selected item.
    /// </summary>
    public string SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _items.Count
            ? _items[SelectedIndex]
            : string.Empty;

    /// <summary>
    ///     Represents selection changed.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Executes set items.
    /// </summary>
    /// <param name="items">The items value.</param>
    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }

        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        var changed = false;
        if (_items.Count > 0 && message is KeyPressed key)
        {
            if (key.Is(Key.Down) || key.Is(Key.Right))
            {
                SelectedIndex = (SelectedIndex + 1) % _items.Count;
                changed = true;
            }
            else if (key.Is(Key.Up) || key.Is(Key.Left))
            {
                SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
                changed = true;
            }
        }

        if (changed && previousIndex != SelectedIndex)
        {
            SelectionChanged?.Invoke(this,
                new SelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
        }

        return changed;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || _items.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = bounds.Inset(1, 1);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1);
            }

            return false;
        }

        var hovered = ResolveHoveredIndex(pointer.Y, content);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left || hovered < 0)
        {
            return false;
        }

        RequestFocus();
        var changed = SetHoveredIndex(hovered);
        if (SelectedIndex == hovered)
        {
            return changed;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        SelectedIndex = hovered;
        SelectionChanged?.Invoke(this,
            new SelectionChangedEventArgs(previousIndex, SelectedIndex, previousItem, SelectedItem));
        return true;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, RenderTitle());
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var row = 0; row < rows; row++)
        {
            var marker = row == SelectedIndex ? SelectedMarker : UnselectedMarker;
            var line = $"{marker} {_items[row]}";
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveItemStyle(row, row == _hoveredIndex)),
                content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(FormatTitleText(true)) + 4;
        var markerWidth = Math.Max(
            ControlTextLayout.MeasureDisplayWidth(SelectedMarker),
            ControlTextLayout.MeasureDisplayWidth(UnselectedMarker));
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, markerWidth + 1 + ControlTextLayout.MeasureDisplayWidth(_items[index]) + 2);
        }

        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatTitleText(bool includeFocusMarkerWhenUnfocused = false)
    {
        if ((IsFocused || includeFocusMarkerWhenUnfocused) && ShowFocusMarker &&
            !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TesseraStyle ResolveItemStyle(int row, bool hovered)
    {
        var style = ItemStyle;
        if (row == SelectedIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (hovered)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledItemStyle);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }

    private int ResolveHoveredIndex(int pointerY, Rect content)
    {
        var row = pointerY - content.Y;
        if (row < 0 || row >= content.Height || row >= _items.Count)
        {
            return -1;
        }

        return row;
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
}
