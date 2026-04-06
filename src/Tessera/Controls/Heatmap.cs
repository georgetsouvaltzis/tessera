using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a dense matrix heatmap with optional headers, legend, and cell styling.
/// </summary>
public sealed class Heatmap : Control
{
    private readonly List<HeatmapCell> _cells = [];
    private readonly List<string> _rowLabels = [];
    private readonly List<string> _columnLabels = [];
    private readonly List<HeatmapLegend> _legend = [];
    private int _rows;
    private int _columns;
    private int _selectedRow = -1;
    private int _selectedColumn = -1;
    private int _hoveredRow = -1;
    private int _hoveredColumn = -1;

    /// <summary>Occurs when selected cell changes.</summary>
    public event EventHandler<ListSelectionChangedEventArgs<HeatmapCell?>>? SelectionChanged;

    /// <summary>Gets or sets chart title.</summary>
    public string Title { get; set; } = "Heatmap";
    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Gets or sets whether <see cref="FocusMarker"/> is shown while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Gets or sets text shown when there is no matrix data.</summary>
    public string EmptyText { get; set; } = "(empty)";
    /// <summary>Gets or sets whether row labels are rendered.</summary>
    public bool ShowRowLabels { get; set; } = true;
    /// <summary>Gets or sets whether column labels are rendered.</summary>
    public bool ShowColumnLabels { get; set; } = true;
    /// <summary>Gets or sets whether legend footer is rendered.</summary>
    public bool ShowLegend { get; set; } = true;
    /// <summary>Gets or sets frame border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets title style when not focused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets title style when focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets border glyph style while not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets border glyph style merged while focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets base style for heatmap cells.</summary>
    public TesseraStyle CellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged into hovered cells.</summary>
    public TesseraStyle HoveredCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged into selected cells.</summary>
    public TesseraStyle SelectedCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged into selected cells while focused.</summary>
    public TesseraStyle FocusedSelectedCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style merged while control is disabled.</summary>
    public TesseraStyle DisabledCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for low-intensity cells.</summary>
    public TesseraStyle LowCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for medium-intensity cells.</summary>
    public TesseraStyle MidCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for high-intensity cells.</summary>
    public TesseraStyle HighCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for peak-intensity cells.</summary>
    public TesseraStyle PeakCellStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for row/column headers.</summary>
    public TesseraStyle HeaderStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for legend text.</summary>
    public TesseraStyle LegendStyle { get; set; } = TesseraStyle.Empty;
    /// <summary>Gets or sets style for empty-state text.</summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets glyph for low-intensity cells.</summary>
    public char LowGlyph { get; set; } = '░';
    /// <summary>Gets or sets glyph for medium-intensity cells.</summary>
    public char MidGlyph { get; set; } = '▒';
    /// <summary>Gets or sets glyph for high-intensity cells.</summary>
    public char HighGlyph { get; set; } = '▓';
    /// <summary>Gets or sets glyph for peak-intensity cells.</summary>
    public char PeakGlyph { get; set; } = '█';

    /// <summary>Gets current cells in row-major order.</summary>
    public IReadOnlyList<HeatmapCell> Cells => _cells;
    /// <summary>Gets matrix row count.</summary>
    public int RowCount => _rows;
    /// <summary>Gets matrix column count.</summary>
    public int ColumnCount => _columns;
    /// <summary>Gets selected row index.</summary>
    public int SelectedRow => _selectedRow;
    /// <summary>Gets selected column index.</summary>
    public int SelectedColumn => _selectedColumn;
    /// <summary>Gets selected cell when selection exists.</summary>
    public HeatmapCell? SelectedCell => TryGetCell(_selectedRow, _selectedColumn, out var value) ? value : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces matrix values from a 2D array.</summary>
    /// <param name="values">Matrix values indexed by <c>[row, column]</c>.</param>
    public void SetMatrix(double[,] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var previousIndex = ToLinear(_selectedRow, _selectedColumn);
        var previousCell = SelectedCell;
        _rows = values.GetLength(0);
        _columns = values.GetLength(1);
        _cells.Clear();
        for (var row = 0; row < _rows; row++)
        {
            for (var column = 0; column < _columns; column++)
            {
                _cells.Add(new HeatmapCell(row, column, values[row, column]));
            }
        }

        NormalizeState();
        RaiseSelectionChangedIfNeeded(previousIndex, previousCell);
    }

    /// <summary>Replaces row labels used by rendering.</summary>
    /// <param name="labels">Row-label sequence.</param>
    public void SetRowLabels(IEnumerable<string> labels) => ReplaceLabels(labels, _rowLabels);

    /// <summary>Replaces column labels used by rendering.</summary>
    /// <param name="labels">Column-label sequence.</param>
    public void SetColumnLabels(IEnumerable<string> labels) => ReplaceLabels(labels, _columnLabels);

    /// <summary>Sets custom legend bands. Empty bands use implicit quartiles.</summary>
    /// <param name="bands">Legend bands to use.</param>
    public void SetLegend(IEnumerable<HeatmapLegend> bands)
    {
        ArgumentNullException.ThrowIfNull(bands);
        _legend.Clear();
        foreach (var band in bands)
        {
            _legend.Add(band);
        }
    }

    /// <summary>Clears matrix data and selection state.</summary>
    public void Clear()
    {
        var previousIndex = ToLinear(_selectedRow, _selectedColumn);
        var previousCell = SelectedCell;
        _cells.Clear();
        _rows = 0;
        _columns = 0;
        _selectedRow = _selectedColumn = _hoveredRow = _hoveredColumn = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousCell);
    }

    /// <summary>Sets selected cell using bounds clamping.</summary>
    /// <param name="row">Requested row index.</param>
    /// <param name="column">Requested column index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedCell(int row, int column)
    {
        if (_rows <= 0 || _columns <= 0)
        {
            return false;
        }

        var nextRow = Math.Clamp(row, 0, _rows - 1);
        var nextColumn = Math.Clamp(column, 0, _columns - 1);
        if (_selectedRow == nextRow && _selectedColumn == nextColumn)
        {
            return false;
        }

        var previousIndex = ToLinear(_selectedRow, _selectedColumn);
        var previousCell = SelectedCell;
        _selectedRow = nextRow;
        _selectedColumn = nextColumn;
        RaiseSelectionChangedIfNeeded(previousIndex, previousCell);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _rows == 0 || _columns == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var row = _selectedRow < 0 ? 0 : _selectedRow;
        var column = _selectedColumn < 0 ? 0 : _selectedColumn;
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedCell(row + 1, column);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedCell(row - 1, column);
        if (key.Is(Key.Right) || key.IsCharacter('l')) return SetSelectedCell(row, column + 1);
        if (key.Is(Key.Left) || key.IsCharacter('h')) return SetSelectedCell(row, column - 1);
        if (key.Is(Key.Home)) return SetSelectedCell(0, 0);
        if (key.Is(Key.End)) return SetSelectedCell(_rows - 1, _columns - 1);
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (!TryLayout(content, out var layout))
        {
            return Handle(message);
        }

        var changed = false;
        var inside = layout.Plot.Contains(pointer.X, pointer.Y);
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press) changed |= SetHovered(-1, -1);
        if (pointer.Kind == PointerEventKind.Wheel)
        {
            var row = _selectedRow < 0 ? 0 : _selectedRow;
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedCell(row + 1, _selectedColumn) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedCell(row - 1, _selectedColumn) || changed;
        }

        if (!inside)
        {
            return changed;
        }
        var rowIndex = pointer.Y - layout.Plot.Y;
        var columnIndex = pointer.X - layout.Plot.X;
        rowIndex = rowIndex >= 0 && rowIndex < layout.VisibleRows ? rowIndex : -1;
        columnIndex = columnIndex >= 0 && columnIndex < layout.VisibleColumns ? columnIndex : -1;
        if (pointer.Kind == PointerEventKind.Motion) return SetHovered(rowIndex, columnIndex);
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && rowIndex >= 0 && columnIndex >= 0)
        {
            RequestFocus();
            changed |= SetHovered(rowIndex, columnIndex);
            changed |= SetSelectedCell(rowIndex, columnIndex);
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, Border == BorderStyle.None ? null : RenderTitle(), Border, Padding, ResolveBorderStyle());
        if (!TryLayout(content, out var layout) || _cells.Count == 0)
        {
            Write(canvas, content.X, content.Y, EmptyText, ResolveStyle(EmptyStyle), content.Width);
            return;
        }

        ResolveRange(out var min, out var max);
        RenderHeaders(canvas, layout);
        RenderCells(canvas, layout, min, max);
        RenderLegend(canvas, layout);
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, Math.Max(ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 4, _columns + 8));
        var height = Math.Max(6, _rows + 4);
        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        height += Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderHeaders(Canvas canvas, in PlotLayout layout)
    {
        var headerStyle = ResolveStyle(HeaderStyle);
        if (layout.ColumnY >= 0)
        {
            for (var c = 0; c < layout.VisibleColumns; c++)
            {
                var glyph = HeaderGlyph(ColumnLabel(c), c);
                if (headerStyle.IsEmpty) canvas.Set(layout.Plot.X + c, layout.ColumnY, glyph);
                else Write(canvas, layout.Plot.X + c, layout.ColumnY, Glyph(glyph), headerStyle, 1);
            }
        }

        if (layout.RowLabelWidth > 0)
        {
            for (var r = 0; r < layout.VisibleRows; r++)
            {
                Write(canvas, layout.RowLabelX, layout.Plot.Y + r, RowLabel(r), headerStyle, layout.RowLabelWidth);
            }
        }
    }

    private void RenderCells(Canvas canvas, in PlotLayout layout, double min, double max)
    {
        for (var r = 0; r < layout.VisibleRows; r++)
        {
            for (var c = 0; c < layout.VisibleColumns; c++)
            {
                var cell = _cells[ToLinear(r, c)];
                var (glyph, bandStyle) = ResolveBand(cell.Value, min, max);
                var style = ResolveStyle(CellStyle.Merge(bandStyle).Merge(StateStyle(r, c)));
                if (style.IsEmpty) canvas.Set(layout.Plot.X + c, layout.Plot.Y + r, glyph);
                else Write(canvas, layout.Plot.X + c, layout.Plot.Y + r, Glyph(glyph), style, 1);
            }
        }
    }

    private void RenderLegend(Canvas canvas, in PlotLayout layout)
    {
        if (!ShowLegend || layout.LegendY < 0)
        {
            return;
        }
        var x = layout.Plot.X;
        var width = layout.Plot.Right - layout.Plot.X;
        if (_legend.Count > 0)
        {
            for (var i = 0; i < _legend.Count && x < layout.Plot.Right; i++)
            {
                var e = _legend[i];
                var text = string.Concat(Glyph(e.Glyph), " ", e.Label, " ");
                Write(canvas, x, layout.LegendY, text, ResolveStyle(LegendStyle.Merge(e.Style)), width - (x - layout.Plot.X));
                x += text.Length;
            }

            return;
        }

        LegendBand(canvas, ref x, layout.LegendY, width, LowGlyph, "low", LowCellStyle);
        LegendBand(canvas, ref x, layout.LegendY, width, MidGlyph, "mid", MidCellStyle);
        LegendBand(canvas, ref x, layout.LegendY, width, HighGlyph, "high", HighCellStyle);
        LegendBand(canvas, ref x, layout.LegendY, width, PeakGlyph, "peak", PeakCellStyle);
    }

    private void LegendBand(Canvas canvas, ref int x, int y, int width, char glyph, string label, TesseraStyle style)
    {
        var text = string.Concat(Glyph(glyph), " ", label, " ");
        Write(canvas, x, y, text, ResolveStyle(LegendStyle.Merge(style)), width - x);
        x += text.Length;
    }

    private (char Glyph, TesseraStyle Style) ResolveBand(double value, double min, double max)
    {
        for (var i = 0; i < _legend.Count; i++)
        {
            var e = _legend[i];
            if (value >= e.MinInclusive && value <= e.MaxInclusive) return (e.Glyph, e.Style);
        }

        var n = Math.Clamp((value - min) / (max - min), 0d, 1d);
        if (n <= 0.25d) return (LowGlyph, LowCellStyle);
        if (n <= 0.5d) return (MidGlyph, MidCellStyle);
        if (n <= 0.75d) return (HighGlyph, HighCellStyle);
        return (PeakGlyph, PeakCellStyle);
    }

    private TesseraStyle StateStyle(int row, int column)
    {
        var style = TesseraStyle.Empty;
        if (row == _hoveredRow && column == _hoveredColumn) style = style.Merge(HoveredCellStyle);
        if (row == _selectedRow && column == _selectedColumn)
        {
            style = style.Merge(SelectedCellStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedCellStyle);
        }

        return style;
    }

    private TesseraStyle ResolveBorderStyle() => ResolveStyle(IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText);
    private TesseraStyle ResolveStyle(TesseraStyle style) => IsDisabled ? style.Merge(DisabledCellStyle) : style;
    private string RenderTitle() => StyleText(MeasureTitle(), IsFocused ? FocusedTitleStyle : TitleStyle);
    private string MeasureTitle() => IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? string.Concat(Title ?? string.Empty, " ", FocusMarker) : Title ?? string.Empty;
    private string RowLabel(int row) => row < _rowLabels.Count && _rowLabels[row].Length > 0 ? _rowLabels[row] : (row + 1).ToString(CultureInfo.InvariantCulture);
    private string ColumnLabel(int col) => col < _columnLabels.Count && _columnLabels[col].Length > 0 ? _columnLabels[col] : (col + 1).ToString(CultureInfo.InvariantCulture);

    private bool TryLayout(Rect content, out PlotLayout layout)
    {
        layout = default;
        if (content.IsEmpty || _rows <= 0 || _columns <= 0)
        {
            return false;
        }

        var rowLabelWidth = ShowRowLabels ? Math.Clamp(MaxLabelWidth(_rowLabels), 0, Math.Min(12, Math.Max(0, content.Width - 2))) : 0;
        var rowOffset = rowLabelWidth > 0 ? rowLabelWidth + 1 : 0;
        var colOffset = ShowColumnLabels ? 1 : 0;
        var legendOffset = ShowLegend ? 1 : 0;
        var plot = new Rect(content.X + rowOffset, content.Y + colOffset, content.Width - rowOffset, content.Height - colOffset - legendOffset);
        if (plot.IsEmpty)
        {
            return false;
        }

        var vr = Math.Min(_rows, plot.Height);
        var vc = Math.Min(_columns, plot.Width);
        if (vr <= 0 || vc <= 0)
        {
            return false;
        }

        layout = new PlotLayout(plot, vr, vc, rowLabelWidth > 0 ? content.X : -1, rowLabelWidth, ShowColumnLabels ? content.Y : -1, ShowLegend ? plot.Bottom : -1);
        return true;
    }

    private void ResolveRange(out double min, out double max)
    {
        min = double.PositiveInfinity;
        max = double.NegativeInfinity;
        for (var i = 0; i < _cells.Count; i++)
        {
            var v = _cells[i].Value;
            if (v < min) min = v;
            if (v > max) max = v;
        }

        if (min == double.PositiveInfinity) (min, max) = (0, 1);
        else if (Math.Abs(max - min) < double.Epsilon) max = min + 1;
    }

    private void NormalizeState()
    {
        if (_rows <= 0 || _columns <= 0)
        {
            _selectedRow = _selectedColumn = _hoveredRow = _hoveredColumn = -1;
            return;
        }

        _selectedRow = Math.Clamp(_selectedRow < 0 ? 0 : _selectedRow, 0, _rows - 1);
        _selectedColumn = Math.Clamp(_selectedColumn < 0 ? 0 : _selectedColumn, 0, _columns - 1);
        _hoveredRow = Math.Clamp(_hoveredRow, -1, _rows - 1);
        _hoveredColumn = Math.Clamp(_hoveredColumn, -1, _columns - 1);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, HeatmapCell? previousCell)
    {
        var selected = ToLinear(_selectedRow, _selectedColumn);
        if (previousIndex == selected)
        {
            return;
        }

        SelectionChanged?.Invoke(this, new ListSelectionChangedEventArgs<HeatmapCell?>(previousIndex, selected, previousCell, SelectedCell));
    }

    private bool SetHovered(int row, int column)
    {
        if (_hoveredRow == row && _hoveredColumn == column) return false;
        _hoveredRow = row;
        _hoveredColumn = column;
        return true;
    }

    private bool TryGetCell(int row, int column, out HeatmapCell value)
    {
        if (TryIndex(row, column, out var index))
        {
            value = _cells[index];
            return true;
        }

        value = default;
        return false;
    }

    private bool TryIndex(int row, int column, out int index)
    {
        if (row >= 0 && row < _rows && column >= 0 && column < _columns)
        {
            index = (row * _columns) + column;
            return true;
        }

        index = -1;
        return false;
    }

    private int ToLinear(int row, int column) => TryIndex(row, column, out var i) ? i : -1;
    private static void ReplaceLabels(IEnumerable<string> labels, List<string> target) { ArgumentNullException.ThrowIfNull(labels); target.Clear(); foreach (var l in labels) target.Add(l ?? string.Empty); }
    private static int MaxLabelWidth(List<string> labels) { var max = 0; for (var i = 0; i < labels.Count; i++) max = Math.Max(max, ControlTextLayout.MeasureDisplayWidth(labels[i])); return max; }
    private static char HeaderGlyph(string label, int fallback) { for (var i = 0; i < label.Length; i++) if (!char.IsWhiteSpace(label[i])) return label[i]; return (char)('0' + ((fallback + 1) % 10)); }
    private static string Glyph(char glyph) => glyph switch { '░' => "░", '▒' => "▒", '▓' => "▓", '█' => "█", _ => glyph.ToString() };
    private static string StyleText(string text, TesseraStyle style) => style.IsEmpty ? text : style.Render(text);
    private static void Write(Canvas canvas, int x, int y, string text, TesseraStyle style, int width) => canvas.WriteText(x, y, StyleText(text, style), width);
    private readonly record struct PlotLayout(Rect Plot, int VisibleRows, int VisibleColumns, int RowLabelX, int RowLabelWidth, int ColumnY, int LegendY);
}
