using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a selectable search-results list with query markers and ranked rows.
/// </summary>
public sealed partial class SearchResultsView : Control
{
    private readonly List<string> _items = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _pressedIndex = -1;

    /// <summary>
    /// Occurs when the selected row changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets the control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Search Results";

    /// <summary>
    /// Gets or sets marker text appended to <see cref="Title"/> when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether <see cref="FocusMarker"/> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets the query used to mark matching rows.
    /// </summary>
    public string Query
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets text shown when no results are available.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no results)";

    /// <summary>
    /// Gets or sets whether rank prefixes are rendered for rows.
    /// </summary>
    public bool ShowRankMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets whether error style should be merged into rendered rows.
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// Gets or sets the glyph set used to render row markers and match indicators.
    /// </summary>
    public SearchResultsGlyphSet Glyphs { get; set; } = SearchResultsGlyphSet.Default;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style for normal rows.
    /// </summary>
    public TesseraStyle DefaultRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TesseraStyle HoveredRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows when focused.
    /// </summary>
    public TesseraStyle FocusedSelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while rows are pressed.
    /// </summary>
    public TesseraStyle PressedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while disabled.
    /// </summary>
    public TesseraStyle DisabledRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while <see cref="HasError"/> is <see langword="true"/>.
    /// </summary>
    public TesseraStyle ErrorRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets style applied to border glyphs when not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets all rows currently shown by the control.
    /// </summary>
    public IReadOnlyList<string> Items => _items;

    /// <summary>
    /// Gets the current selected row index, or <c>-1</c> when there are no rows.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets the currently selected row text.
    /// </summary>
    public string SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count
        ? _items[_selectedIndex]
        : string.Empty;

    /// <summary>
    /// Gets number of rows currently shown.
    /// </summary>
    public int Count => _items.Count;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces rows shown by the control.
    /// </summary>
    /// <param name="items">Rows to render.</param>
    public void SetItems(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;

        _items.Clear();
        foreach (var item in items)
        {
            _items.Add(item ?? string.Empty);
        }

        if (_items.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _pressedIndex = -1;
        }
        else
        {
            _selectedIndex = _selectedIndex < 0 ? 0 : Math.Clamp(_selectedIndex, 0, _items.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _items.Count - 1);
            _pressedIndex = Math.Clamp(_pressedIndex, -1, _items.Count - 1);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Clears rows and resets selection state.
    /// </summary>
    public void ClearItems()
    {
        if (_items.Count == 0 && _selectedIndex < 0)
        {
            return;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _items.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _pressedIndex = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Attempts to set selected row index.
    /// </summary>
    /// <param name="index">Index to select.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
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
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _items.Count == 0 || message is not KeyPressed key)
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

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (IsDisabled || IsReadOnly || _items.Count == 0)
        {
            return false;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(RowIndexAtPointer(pointer.X, pointer.Y, content));
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }

            return false;
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            var row = RowIndexAtPointer(pointer.X, pointer.Y, content);
            if (row < 0)
            {
                return SetPressedIndex(-1);
            }

            _ = SetHoveredIndex(row);
            _ = SetPressedIndex(row);
            return SetSelectedIndex(row) || true;
        }

        if (pointer.Kind == PointerEventKind.Release && pointer.Button == PointerButton.Left)
        {
            return SetPressedIndex(-1);
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
            ResolveBorderStyleText());
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyRowStyle(EmptyText, selected: false, hovered: false, pressed: false), content.Width);
            return;
        }

        for (var row = 0; row < content.Height && row < _items.Count; row++)
        {
            var selected = row == _selectedIndex;
            var hovered = row == _hoveredIndex;
            var pressed = row == _pressedIndex;
            var marker = selected
                ? Glyphs.SelectedRowMarker
                : hovered ? Glyphs.HoveredRowMarker : Glyphs.DefaultRowMarker;
            var rank = ShowRankMarker ? $"{row + 1}{Glyphs.RankSeparator} " : string.Empty;
            var match = HasQueryMatch(_items[row]) ? $"{Glyphs.MatchMarker} " : string.Empty;
            var text = $"{marker} {rank}{match}{_items[row]}";
            canvas.WriteText(content.X, content.Y + row, ApplyRowStyle(text, selected, hovered, pressed), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(EmptyText);
        for (var index = 0; index < _items.Count; index++)
        {
            var rank = ShowRankMarker ? $"{index + 1}{Glyphs.RankSeparator} " : string.Empty;
            var itemWidth = ControlTextLayout.MeasureDisplayWidth($"{Glyphs.SelectedRowMarker} {rank}{Glyphs.MatchMarker} {_items[index]}");
            width = Math.Max(width, itemWidth);
        }

        if (Border != BorderStyle.None)
        {
            width = Math.Max(width + 2, Title.Length + 4);
        }

        var height = Math.Max(1, _items.Count);
        if (Border != BorderStyle.None)
        {
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

}
