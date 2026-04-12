using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>Selectable palette-authoring grid control for theme workflows.</summary>
public sealed class PaletteEditor : Control
{
    private readonly List<PaletteSwatch> _swatches = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollRow;
    private int _lastViewportRows = 6;

    /// <summary>Raised when selected swatch changes.</summary>
    public event EventHandler<PaletteSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>Gets or sets title text.</summary>
    public string Title { get; set; } = "Palette Editor";
    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Gets or sets whether focus marker is rendered.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Gets or sets empty-state text.</summary>
    public string EmptyText { get; set; } = "(no swatches)";
    /// <summary>Gets or sets desired grid column count.</summary>
    public int ColumnCount { get; set; } = 2;
    /// <summary>Gets or sets whether hex text is rendered.</summary>
    public bool ShowHexCode { get; set; } = true;
    /// <summary>Gets or sets whether descriptions are rendered.</summary>
    public bool ShowDescription { get; set; }
    /// <summary>Gets or sets whether preview glyphs are rendered.</summary>
    public bool ShowPreviewBlock { get; set; } = true;
    /// <summary>Gets or sets preview glyph text.</summary>
    public string PreviewGlyph { get; set; } = "██";
    /// <summary>Gets or sets marker for selected rows.</summary>
    public string SelectedMarker { get; set; } = ">";
    /// <summary>Gets or sets marker for unselected rows.</summary>
    public string UnselectedMarker { get; set; } = " ";
    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets unfocused title style.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets focused title style.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets base row style.</summary>
    public TesseraStyle SwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets hovered row style.</summary>
    public TesseraStyle HoveredSwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets selected row style.</summary>
    public TesseraStyle SelectedSwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets focused selected row style.</summary>
    public TesseraStyle FocusedSelectedSwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets muted row style.</summary>
    public TesseraStyle MutedSwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets preview glyph style.</summary>
    public TesseraStyle PreviewSwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets disabled row style.</summary>
    public TesseraStyle DisabledSwatchStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets empty-state text style.</summary>
    public TesseraStyle EmptyTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets current swatches in display order.</summary>
    public IReadOnlyList<PaletteSwatch> Swatches => _swatches;
    /// <summary>Gets selected index, or <c>-1</c> when empty.</summary>
    public int SelectedIndex => _selectedIndex;
    /// <summary>Gets selected swatch, if any.</summary>
    public PaletteSwatch? SelectedSwatch => _selectedIndex >= 0 && _selectedIndex < _swatches.Count ? _swatches[_selectedIndex] : null;
    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces all swatches.</summary>
    /// <param name="swatches">Swatches to render.</param>
    public void SetSwatches(IEnumerable<PaletteSwatch> swatches)
    {
        ArgumentNullException.ThrowIfNull(swatches);
        var previousIndex = _selectedIndex;
        var previousSwatch = SelectedSwatch;
        _swatches.Clear();
        foreach (var swatch in swatches.Where(static swatch => swatch is not null))
        {
            _swatches.Add(CloneSwatch(swatch));
        }

        if (_swatches.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollRow = 0;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _swatches.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _swatches.Count - 1);
            EnsureSelectionVisible(_lastViewportRows);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousSwatch);
    }

    /// <summary>Appends one swatch.</summary>
    /// <param name="swatch">Swatch to append.</param>
    public void AddSwatch(PaletteSwatch swatch)
    {
        ArgumentNullException.ThrowIfNull(swatch);
        var previousIndex = _selectedIndex;
        var previousSwatch = SelectedSwatch;
        _swatches.Add(CloneSwatch(swatch));
        if (_selectedIndex < 0) _selectedIndex = 0;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousSwatch);
    }

    /// <summary>Clears swatches and selection state.</summary>
    public void Clear()
    {
        var previousIndex = _selectedIndex;
        var previousSwatch = SelectedSwatch;
        _swatches.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _scrollRow = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousSwatch);
    }

    /// <summary>Selects by index using bounds clamping.</summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool Select(int index) => SetSelectedIndex(index);

    /// <summary>Sets the selected swatch index using bounds clamping.</summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_swatches.Count == 0) return false;
        var clamped = Math.Clamp(index, 0, _swatches.Count - 1);
        if (clamped == _selectedIndex) return false;
        var previousIndex = _selectedIndex;
        var previousSwatch = SelectedSwatch;
        _selectedIndex = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousSwatch);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _swatches.Count == 0 || message is not KeyPressed key) return false;
        var columns = ResolveColumnCount();
        var page = Math.Max(1, _lastViewportRows) * columns;
        if (key.Is(Key.Left) || key.IsCharacter('h')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.Right) || key.IsCharacter('l')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - columns);
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + columns);
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_swatches.Count - 1);
        if (key.Is(Key.PageUp)) return SetSelectedIndex(_selectedIndex - page);
        if (key.Is(Key.PageDown)) return SetSelectedIndex(_selectedIndex + page);
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer) return Handle(message);
        var content = bounds.Inset(Padding);
        if (content.IsEmpty) return Handle(message);
        var headerRows = HasTitle() ? 1 : 0;
        var rowY = content.Y + headerRows;
        var rowsHeight = Math.Max(0, content.Height - headerRows);
        _lastViewportRows = Math.Max(1, rowsHeight);

        var inside = content.Contains(pointer.X, pointer.Y);
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                var changed = _hoveredIndex >= 0;
                _hoveredIndex = -1;
                return changed;
            }

            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _swatches.Count > 0)
        {
            var columns = ResolveColumnCount();
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedIndex(_selectedIndex + columns);
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedIndex(_selectedIndex - columns);
        }

        if (_swatches.Count == 0 || pointer.Y < rowY || rowsHeight <= 0) return Handle(message);
        EnsureSelectionVisible(rowsHeight);
        var hovered = ResolveHoveredIndex(pointer.X, pointer.Y, content.X, rowY, content.Width, rowsHeight);

        if (pointer.Kind == PointerEventKind.Motion)
        {
            if (_hoveredIndex == hovered) return false;
            _hoveredIndex = hovered;
            return true;
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            var changed = _hoveredIndex != hovered;
            _hoveredIndex = hovered;
            return SetSelectedIndex(hovered) || changed;
        }

        return Handle(message);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty) return;
        var content = clipped.Inset(Padding);
        if (content.IsEmpty) return;

        var y = content.Y;
        if (HasTitle())
        {
            WriteStyledText(canvas, content.X, y, FormatTitle(), ResolveTitleStyle(), content.Width);
            y++;
        }

        var rowsHeight = Math.Max(0, content.Bottom - y);
        _lastViewportRows = Math.Max(1, rowsHeight);
        if (_swatches.Count == 0 || rowsHeight <= 0)
        {
            if (rowsHeight > 0) WriteStyledText(canvas, content.X, y, EmptyText, ResolveEmptyStyle(), content.Width);
            return;
        }

        EnsureSelectionVisible(rowsHeight);
        var columns = ResolveColumnCount();
        var totalRows = GetRowCount(columns);
        var visibleRows = Math.Min(rowsHeight, totalRows - _scrollRow);
        for (var row = 0; row < visibleRows; row++)
        {
            var gridRow = _scrollRow + row;
            var rowY = y + row;
            for (var column = 0; column < columns; column++)
            {
                var index = (gridRow * columns) + column;
                if (index >= _swatches.Count) continue;
                var cellX = content.X + ((column * content.Width) / columns);
                var cellRight = content.X + (((column + 1) * content.Width) / columns);
                var cellWidth = Math.Max(0, cellRight - cellX);
                if (cellWidth > 0) RenderSwatch(canvas, cellX, rowY, cellWidth, index, _swatches[index]);
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var columns = ResolveColumnCount();
        var maxBodyWidth = 16;
        for (var index = 0; index < _swatches.Count; index++)
        {
            maxBodyWidth = Math.Max(maxBodyWidth, ControlTextLayout.MeasureDisplayWidth(BuildBody(_swatches[index])));
        }

        var cellWidth = maxBodyWidth + (ShowPreviewBlock ? 6 : 3);
        var width = (cellWidth * columns) + Padding.Horizontal;
        if (HasTitle()) width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitle()) + Padding.Horizontal);
        var height = Math.Max(1, GetRowCount(columns)) + (HasTitle() ? 1 : 0) + Padding.Vertical;
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderSwatch(Canvas canvas, int x, int y, int width, int index, PaletteSwatch swatch)
    {
        var rowStyle = ResolveSwatchStyle(index, swatch);
        var remaining = width;
        var cursor = x;
        var marker = index == _selectedIndex ? SelectedMarker : UnselectedMarker;
        var markerUsed = WriteStyledText(canvas, cursor, y, marker, rowStyle, remaining);
        cursor += markerUsed;
        remaining -= markerUsed;
        if (remaining <= 0) return;
        var spacerUsed = WriteStyledText(canvas, cursor, y, " ", rowStyle, remaining);
        cursor += spacerUsed;
        remaining -= spacerUsed;
        if (remaining <= 0) return;

        if (ShowPreviewBlock && !string.IsNullOrWhiteSpace(PreviewGlyph))
        {
            var previewStyle = rowStyle.Merge(ResolvePreviewStyle(swatch));
            var previewUsed = WriteStyledText(canvas, cursor, y, PreviewGlyph, previewStyle, remaining);
            cursor += previewUsed;
            remaining -= previewUsed;
            if (remaining <= 0) return;
            var previewSpaceUsed = WriteStyledText(canvas, cursor, y, " ", rowStyle, remaining);
            cursor += previewSpaceUsed;
            remaining -= previewSpaceUsed;
            if (remaining <= 0) return;
        }

        _ = WriteStyledText(canvas, cursor, y, BuildBody(swatch), rowStyle, remaining);
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_swatches.Count == 0 || viewportRows <= 0)
        {
            _scrollRow = 0;
            return;
        }

        if (_selectedIndex < 0) _selectedIndex = 0;
        var columns = ResolveColumnCount();
        var selectedRow = _selectedIndex / columns;
        if (selectedRow < _scrollRow) _scrollRow = selectedRow;
        else if (selectedRow >= _scrollRow + viewportRows) _scrollRow = selectedRow - viewportRows + 1;
        var maxScroll = Math.Max(0, GetRowCount(columns) - viewportRows);
        _scrollRow = Math.Clamp(_scrollRow, 0, maxScroll);
    }

    private int ResolveHoveredIndex(int pointerX, int pointerY, int contentX, int rowY, int contentWidth, int rowsHeight)
    {
        var relativeRow = pointerY - rowY;
        if (relativeRow < 0 || relativeRow >= rowsHeight) return -1;
        var columns = ResolveColumnCount();
        var cellWidth = Math.Max(1, contentWidth / columns);
        var relativeX = Math.Max(0, pointerX - contentX);
        var column = Math.Clamp(relativeX / cellWidth, 0, columns - 1);
        var index = ((_scrollRow + relativeRow) * columns) + column;
        return index >= 0 && index < _swatches.Count ? index : -1;
    }

    private int ResolveColumnCount() => Math.Clamp(ColumnCount, 1, 12);
    private int GetRowCount(int columns) => _swatches.Count == 0 ? 0 : ((_swatches.Count - 1) / columns) + 1;

    private string BuildBody(PaletteSwatch swatch)
    {
        var body = NormalizeSingleLine(swatch.Name);
        var hex = NormalizeSingleLine(swatch.Hex);
        var description = NormalizeSingleLine(swatch.Description);
        if (ShowHexCode && !string.IsNullOrWhiteSpace(hex)) body = string.Concat(body, " ", hex);
        if (ShowDescription && !string.IsNullOrWhiteSpace(description)) body = string.Concat(body, " - ", description);
        return body;
    }

    private TesseraStyle ResolveTitleStyle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return IsDisabled ? style.Merge(DisabledSwatchStyle) : style;
    }

    private TesseraStyle ResolveEmptyStyle() => IsDisabled ? EmptyTextStyle.Merge(DisabledSwatchStyle) : EmptyTextStyle;
    private TesseraStyle ResolvePreviewStyle(PaletteSwatch swatch) => PreviewSwatchStyle.Merge(swatch.PreviewStyle);

    private TesseraStyle ResolveSwatchStyle(int index, PaletteSwatch swatch)
    {
        var style = SwatchStyle.Merge(swatch.Style);
        if (swatch.IsMuted) style = style.Merge(MutedSwatchStyle);
        if (index == _hoveredIndex) style = style.Merge(HoveredSwatchStyle);
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedSwatchStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedSwatchStyle);
        }

        if (IsDisabled) style = style.Merge(DisabledSwatchStyle);
        return style;
    }

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title ?? string.Empty, " ", FocusMarker);
        }

        return Title ?? string.Empty;
    }

    private bool HasTitle() => !string.IsNullOrWhiteSpace(Title);

    private void RaiseSelectionChangedIfNeeded(int previousIndex, PaletteSwatch? previousSwatch)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousSwatch, SelectedSwatch)) return;
        SelectionChanged?.Invoke(this, new PaletteSelectionChangedEventArgs(previousIndex, _selectedIndex, previousSwatch, SelectedSwatch));
    }

    private static PaletteSwatch CloneSwatch(PaletteSwatch swatch)
    {
        return new PaletteSwatch(swatch.Name, swatch.Hex, swatch.Description)
        {
            IsMuted = swatch.IsMuted,
            Style = swatch.Style,
            PreviewStyle = swatch.PreviewStyle,
        };
    }

    private static string NormalizeSingleLine(string? value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace('\r', ' ').Replace('\n', ' ');

    private static int WriteStyledText(Canvas canvas, int x, int y, string text, TesseraStyle style, int width)
    {
        if (width <= 0 || string.IsNullOrEmpty(text)) return 0;
        canvas.WriteText(x, y, style.IsEmpty ? text : style.Render(text), width);
        return Math.Min(width, text.Length);
    }
}
