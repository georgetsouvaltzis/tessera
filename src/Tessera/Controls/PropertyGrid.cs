using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a read-focused key/value property table with optional category grouping.
/// </summary>
public sealed class PropertyGrid : Control
{
    private readonly List<PropertyGridProperty> _properties = [];
    private int _selectedIndex = -1;
    private int _scrollOffset;

    /// <summary>Occurs when <see cref="SelectedIndex"/> changes.</summary>
    public event EventHandler<PropertyGridSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>Gets or sets the grid title.</summary>
    public string Title { get; set; } = "Property Grid";

    /// <summary>Gets or sets the marker shown in the title when focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether the focus marker should be rendered.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets the title style when not focused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the title style when focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style applied to border glyphs when the control is not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style applied to border glyphs when the control is focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets inner content padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets the header text for the key column.</summary>
    public string HeaderKeyText { get; set; } = "Property";

    /// <summary>Gets or sets the header text for the value column.</summary>
    public string HeaderValueText { get; set; } = "Value";

    /// <summary>Gets or sets the preferred width of the key column.</summary>
    public int PreferredKeyColumnWidth { get; set; } = 22;

    /// <summary>Gets or sets whether category headers should be rendered.</summary>
    public bool ShowCategoryHeaders { get; set; } = true;

    /// <summary>Gets or sets the marker shown before the selected row.</summary>
    public string SelectedMarker { get; set; } = ">";

    /// <summary>Gets or sets the marker shown before unselected rows.</summary>
    public string UnselectedMarker { get; set; } = " ";

    /// <summary>Gets or sets the style applied to header rows.</summary>
    public TesseraStyle HeaderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style applied to key text.</summary>
    public TesseraStyle KeyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style applied to value text.</summary>
    public TesseraStyle ValueStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the style merged into selected row text.</summary>
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets the current property rows.</summary>
    public IReadOnlyList<PropertyGridProperty> Properties => _properties;

    /// <summary>Gets the selected row index, or <c>-1</c> when the grid is empty.</summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>Gets the selected property row, when any.</summary>
    public PropertyGridProperty? SelectedProperty => _selectedIndex >= 0 && _selectedIndex < _properties.Count
        ? _properties[_selectedIndex]
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces all property rows.</summary>
    /// <param name="properties">The properties to show.</param>
    public void SetProperties(IEnumerable<PropertyGridProperty> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        var previousIndex = _selectedIndex;
        var previousProperty = SelectedProperty;

        _properties.Clear();
        foreach (var property in properties.Where(static property => property is not null))
        {
            _properties.Add(property);
        }

        if (_properties.Count == 0)
        {
            _selectedIndex = -1;
        }
        else
        {
            var seedIndex = _selectedIndex < 0 ? 0 : _selectedIndex;
            _selectedIndex = Math.Clamp(seedIndex, 0, _properties.Count - 1);
        }
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousProperty);
    }

    /// <summary>Sets selected row index using bounds clamping.</summary>
    /// <param name="index">The requested selected index.</param>
    /// <returns><see langword="true"/> when the selection changed; otherwise <see langword="false"/>.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_properties.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _properties.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousProperty = SelectedProperty;
        _selectedIndex = clamped;
        RaiseSelectionChangedIfNeeded(previousIndex, previousProperty);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _properties.Count == 0 || message is not KeyPressed key)
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
            return SetSelectedIndex(_properties.Count - 1);
        }

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
        if (content.IsEmpty || !content.Contains(pointer.X, pointer.Y))
        {
            return Handle(message);
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

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return Handle(message);
        }

        RequestFocus();
        if (pointer.Y == content.Y)
        {
            return true;
        }

        var row = pointer.Y - content.Y - 1;
        if (row < 0)
        {
            return true;
        }

        var visibleRows = Math.Max(0, content.Height - 1);
        var displayRows = BuildDisplayRows();
        EnsureSelectionVisible(visibleRows, displayRows);
        var displayIndex = _scrollOffset + row;
        if (displayIndex < 0 || displayIndex >= displayRows.Count)
        {
            return true;
        }

        var displayRow = displayRows[displayIndex];
        return displayRow.PropertyIndex >= 0 && SetSelectedIndex(displayRow.PropertyIndex);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var keyWidth = ResolveKeyColumnWidth(content.Width);
        RenderHeader(canvas, content, keyWidth);

        var visibleRows = Math.Max(0, content.Height - 1);
        if (visibleRows == 0)
        {
            return;
        }

        var displayRows = BuildDisplayRows();
        if (displayRows.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, "(empty)", content.Width);
            return;
        }

        EnsureSelectionVisible(visibleRows, displayRows);
        var maxRows = Math.Min(visibleRows, displayRows.Count - _scrollOffset);
        for (var row = 0; row < maxRows; row++)
        {
            var display = displayRows[_scrollOffset + row];
            RenderBodyRow(canvas, content, content.Y + 1 + row, keyWidth, display);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var keyWidth = Math.Max(8, PreferredKeyColumnWidth);
        var width = 2 + keyWidth + 3 + 16 + Padding.Horizontal;
        var height = 1 + Math.Max(1, _properties.Count) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, Title.Length + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderHeader(Canvas canvas, Rect content, int keyWidth)
    {
        var key = ApplyStyle(PadRight(HeaderKeyText ?? string.Empty, keyWidth), HeaderStyle);
        var value = ApplyStyle(HeaderValueText ?? string.Empty, HeaderStyle);
        canvas.WriteText(content.X, content.Y, $"  {key} : {value}", content.Width);
    }

    private void RenderBodyRow(Canvas canvas, Rect content, int y, int keyWidth, DisplayRow display)
    {
        if (display.IsCategory)
        {
            canvas.WriteText(content.X, y, $"  {ApplyStyle($"[{display.Category}]", HeaderStyle)}", content.Width);
            return;
        }

        if (display.PropertyIndex < 0 || display.PropertyIndex >= _properties.Count)
        {
            return;
        }

        var property = _properties[display.PropertyIndex];
        var selected = display.PropertyIndex == _selectedIndex;
        var keyStyle = selected ? KeyStyle.Merge(SelectedRowStyle) : KeyStyle;
        var valueStyle = selected ? ValueStyle.Merge(SelectedRowStyle) : ValueStyle;
        var marker = selected ? SelectedMarker : UnselectedMarker;
        marker ??= string.Empty;

        var keyLabel = ShowCategoryHeaders || string.IsNullOrWhiteSpace(property.Category)
            ? property.Name
            : $"{property.Category}.{property.Name}";
        var key = ApplyStyle(PadRight(keyLabel, keyWidth), keyStyle);
        var value = ApplyStyle(property.Value, valueStyle);
        canvas.WriteText(content.X, y, $"{marker} {key} : {value}", content.Width);
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title ?? string.Empty, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private int ResolveKeyColumnWidth(int contentWidth)
    {
        var maxWidth = Math.Max(8, contentWidth - 6);
        return Math.Clamp(Math.Max(8, PreferredKeyColumnWidth), 8, maxWidth);
    }

    private List<DisplayRow> BuildDisplayRows()
    {
        var rows = new List<DisplayRow>(_properties.Count);
        var previousCategory = string.Empty;
        for (var i = 0; i < _properties.Count; i++)
        {
            var property = _properties[i];
            var category = property.Category ?? string.Empty;
            if (ShowCategoryHeaders
                && !string.IsNullOrEmpty(category)
                && !string.Equals(category, previousCategory, StringComparison.Ordinal))
            {
                rows.Add(new DisplayRow(PropertyIndex: -1, IsCategory: true, Category: category));
            }

            rows.Add(new DisplayRow(PropertyIndex: i, IsCategory: false, Category: category));
            previousCategory = category;
        }

        return rows;
    }

    private void EnsureSelectionVisible(int visibleRows, IReadOnlyList<DisplayRow> rows)
    {
        if (visibleRows <= 0 || rows.Count == 0 || _selectedIndex < 0)
        {
            _scrollOffset = 0;
            return;
        }

        var selectedDisplayIndex = -1;
        for (var i = 0; i < rows.Count; i++)
        {
            if (rows[i].PropertyIndex == _selectedIndex)
            {
                selectedDisplayIndex = i;
                break;
            }
        }

        if (selectedDisplayIndex < 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (selectedDisplayIndex < _scrollOffset)
        {
            _scrollOffset = selectedDisplayIndex;
        }
        else if (selectedDisplayIndex >= _scrollOffset + visibleRows)
        {
            _scrollOffset = selectedDisplayIndex - visibleRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, rows.Count - visibleRows));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, PropertyGridProperty? previousProperty)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousProperty, SelectedProperty))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new PropertyGridSelectionChangedEventArgs(previousIndex, _selectedIndex, previousProperty, SelectedProperty));
    }

    private static string PadRight(string text, int width)
    {
        var measured = ControlTextLayout.MeasureDisplayWidth(text);
        return measured >= width ? text : text + new string(' ', width - measured);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }

    private readonly record struct DisplayRow(int PropertyIndex, bool IsCategory, string Category);
}
