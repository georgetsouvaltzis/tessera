using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a dense matrix heatmap with optional headers, legend, and cell styling.
/// </summary>
public sealed class Heatmap : Control
{
    private readonly List<HeatmapCell> _cells = [];
    private readonly List<string> _columnLabels = [];
    private readonly List<HeatmapLegend> _legend = [];
    private readonly List<string> _rowLabels = [];
    private int _hoveredColumn = -1;
    private int _hoveredRow = -1;

    /// <summary>Gets or sets chart title.</summary>
    public string Title { get; set; } = "Heatmap";

    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether <see cref="FocusMarker" /> is shown while focused.</summary>
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
    public int RowCount { get; private set; }

    /// <summary>Gets matrix column count.</summary>
    public int ColumnCount { get; private set; }

    /// <summary>Gets selected row index.</summary>
    public int SelectedRow { get; private set; } = -1;

    /// <summary>Gets selected column index.</summary>
    public int SelectedColumn { get; private set; } = -1;

    /// <summary>Gets selected cell when selection exists.</summary>
    public HeatmapCell? SelectedCell => TryGetCell(SelectedRow, SelectedColumn, out var value) ? value : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Occurs when selected cell changes.</summary>
    public event EventHandler<ListSelectionChangedEventArgs<HeatmapCell?>>? SelectionChanged;

    /// <summary>Replaces matrix values from row-major data.</summary>
    /// <param name="values">Matrix values indexed by row, then column.</param>
    public void SetMatrix(IReadOnlyList<IReadOnlyList<double>> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var previousIndex = ToLinear(SelectedRow, SelectedColumn);
        var previousCell = SelectedCell;
        RowCount = values.Count;
        ColumnCount = RowCount == 0 ? 0 : values[0].Count;
        _cells.Clear();
        for (var row = 0; row < RowCount; row++)
        {
            if (values[row].Count != ColumnCount)
            {
                throw new ArgumentException("All heatmap rows must have the same column count.", nameof(values));
            }

            for (var column = 0; column < ColumnCount; column++)
            {
                _cells.Add(new HeatmapCell(row, column, values[row][column]));
            }
        }

        NormalizeState();
        RaiseSelectionChangedIfNeeded(previousIndex, previousCell);
    }

    /// <summary>Replaces row labels used by rendering.</summary>
    /// <param name="labels">Row-label sequence.</param>
    public void SetRowLabels(IEnumerable<string> labels)
    {
        ReplaceLabels(labels, _rowLabels);
    }

    /// <summary>Replaces column labels used by rendering.</summary>
    /// <param name="labels">Column-label sequence.</param>
    public void SetColumnLabels(IEnumerable<string> labels)
    {
        ReplaceLabels(labels, _columnLabels);
    }

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
        var previousIndex = ToLinear(SelectedRow, SelectedColumn);
        var previousCell = SelectedCell;
        _cells.Clear();
        RowCount = 0;
        ColumnCount = 0;
        SelectedRow = SelectedColumn = _hoveredRow = _hoveredColumn = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousCell);
    }

    /// <summary>Sets selected cell using bounds clamping.</summary>
    /// <param name="row">Requested row index.</param>
    /// <param name="column">Requested column index.</param>
    /// <returns><see langword="true" /> when selection changed.</returns>
    public bool SetSelectedCell(int row, int column)
    {
        if (RowCount <= 0 || ColumnCount <= 0)
        {
            return false;
        }

        var nextRow = Math.Clamp(row, 0, RowCount - 1);
        var nextColumn = Math.Clamp(column, 0, ColumnCount - 1);
        if (SelectedRow == nextRow && SelectedColumn == nextColumn)
        {
            return false;
        }

        var previousIndex = ToLinear(SelectedRow, SelectedColumn);
        var previousCell = SelectedCell;
        SelectedRow = nextRow;
        SelectedColumn = nextColumn;
        RaiseSelectionChangedIfNeeded(previousIndex, previousCell);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || RowCount == 0 || ColumnCount == 0 ||
            message is not KeyPressed key)
        {
            return false;
        }

        var row = SelectedRow < 0 ? 0 : SelectedRow;
        var column = SelectedColumn < 0 ? 0 : SelectedColumn;
        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedCell(row + 1, column);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedCell(row - 1, column);
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return SetSelectedCell(row, column + 1);
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return SetSelectedCell(row, column - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedCell(0, 0);
        }

        return key.Is(Key.End) && SetSelectedCell(RowCount - 1, ColumnCount - 1);
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
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHovered(-1, -1);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            return HandleWheel(pointer.Button, changed);
        }

        if (!inside)
        {
            return changed;
        }

        var rowIndex = pointer.Y - layout.Plot.Y;
        var columnIndex = pointer.X - layout.Plot.X;
        rowIndex = rowIndex >= 0 && rowIndex < layout.VisibleRows ? rowIndex : -1;
        columnIndex = columnIndex >= 0 && columnIndex < layout.VisibleColumns ? columnIndex : -1;
        return pointer.Kind switch
        {
            PointerEventKind.Motion => SetHovered(rowIndex, columnIndex),
            PointerEventKind.Press when pointer is { Button: PointerButton.Left } && rowIndex >= 0 && columnIndex >= 0
                => HandlePress(rowIndex, columnIndex, changed),
            _ => changed
        };
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped,
            Border == BorderStyle.None ? null : RenderTitle(), Border, Padding, ResolveBorderStyle());
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
        var width = Math.Max(12, Math.Max(ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 4, ColumnCount + 8));
        var height = Math.Max(6, RowCount + 4);
        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        height += Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderHeaders(Canvas canvas, in PlotLayout layout)
    {
        var headerStyle = ResolveStyle(HeaderStyle);
        if (layout.ColumnY >= 0)
        {
            for (var c = 0; c < layout.VisibleColumns; c++)
            {
                var glyph = HeaderGlyph(ColumnLabel(c), c);
                if (headerStyle.IsEmpty)
                {
                    canvas.Set(layout.Plot.X + c, layout.ColumnY, glyph);
                }
                else
                {
                    Write(canvas, layout.Plot.X + c, layout.ColumnY, Glyph(glyph), headerStyle, 1);
                }
            }
        }

        if (layout.RowLabelWidth <= 0)
        {
            return;
        }

        for (var r = 0; r < layout.VisibleRows; r++)
        {
            Write(canvas, layout.RowLabelX, layout.Plot.Y + r, RowLabel(r), headerStyle, layout.RowLabelWidth);
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
                if (style.IsEmpty)
                {
                    canvas.Set(layout.Plot.X + c, layout.Plot.Y + r, glyph);
                }
                else
                {
                    Write(canvas, layout.Plot.X + c, layout.Plot.Y + r, Glyph(glyph), style, 1);
                }
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
            foreach (var entry in _legend)
            {
                if (x >= layout.Plot.Right)
                {
                    break;
                }

                var text = string.Concat(Glyph(entry.Glyph), " ", entry.Label, " ");
                Write(canvas, x, layout.LegendY, text, ResolveStyle(LegendStyle.Merge(entry.Style)),
                    width - (x - layout.Plot.X));
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
        var legendIndex = _legend.FindIndex(entry => value >= entry.MinInclusive && value <= entry.MaxInclusive);
        if (legendIndex >= 0)
        {
            var entry = _legend[legendIndex];
            return (entry.Glyph, entry.Style);
        }

        var n = Math.Clamp((value - min) / (max - min), 0d, 1d);
        return n switch
        {
            <= 0.25d => (LowGlyph, LowCellStyle),
            <= 0.5d => (MidGlyph, MidCellStyle),
            <= 0.75d => (HighGlyph, HighCellStyle),
            _ => (PeakGlyph, PeakCellStyle)
        };
    }

    private TesseraStyle StateStyle(int row, int column)
    {
        var style = TesseraStyle.Empty;
        if (row == _hoveredRow && column == _hoveredColumn)
        {
            style = style.Merge(HoveredCellStyle);
        }

        if (row == SelectedRow && column == SelectedColumn)
        {
            style = style.Merge(SelectedCellStyle);
        }

        if (row == SelectedRow && column == SelectedColumn && IsFocused)
        {
            style = style.Merge(FocusedSelectedCellStyle);
        }

        return style;
    }

    private TesseraStyle ResolveBorderStyle()
    {
        return ResolveStyle(IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText);
    }

    private TesseraStyle ResolveStyle(TesseraStyle style)
    {
        return IsDisabled ? style.Merge(DisabledCellStyle) : style;
    }

    private string RenderTitle()
    {
        return StyleText(MeasureTitle(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string MeasureTitle()
    {
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? string.Concat(Title, " ", FocusMarker)
            : Title;
    }

    private string RowLabel(int row)
    {
        return row < _rowLabels.Count && _rowLabels[row].Length > 0
            ? _rowLabels[row]
            : (row + 1).ToString(CultureInfo.InvariantCulture);
    }

    private string ColumnLabel(int col)
    {
        return col < _columnLabels.Count && _columnLabels[col].Length > 0
            ? _columnLabels[col]
            : (col + 1).ToString(CultureInfo.InvariantCulture);
    }

    private bool TryLayout(Rect content, out PlotLayout layout)
    {
        layout = default;
        if (content.IsEmpty || RowCount <= 0 || ColumnCount <= 0)
        {
            return false;
        }

        var rowLabelWidth = ShowRowLabels
            ? Math.Clamp(MaxLabelWidth(_rowLabels), 0, Math.Min(12, Math.Max(0, content.Width - 2)))
            : 0;
        var rowOffset = rowLabelWidth > 0 ? rowLabelWidth + 1 : 0;
        var colOffset = ShowColumnLabels ? 1 : 0;
        var legendOffset = ShowLegend ? 1 : 0;
        var plot = new Rect(content.X + rowOffset, content.Y + colOffset, content.Width - rowOffset,
            content.Height - colOffset - legendOffset);
        if (plot.IsEmpty)
        {
            return false;
        }

        var vr = Math.Min(RowCount, plot.Height);
        var vc = Math.Min(ColumnCount, plot.Width);
        if (vr <= 0 || vc <= 0)
        {
            return false;
        }

        layout = new PlotLayout(plot, vr, vc, rowLabelWidth > 0 ? content.X : -1, rowLabelWidth,
            ShowColumnLabels ? content.Y : -1, ShowLegend ? plot.Bottom : -1);
        return true;
    }

    private void ResolveRange(out double min, out double max)
    {
        min = double.PositiveInfinity;
        max = double.NegativeInfinity;
        foreach (var value in _cells.Select(static cell => cell.Value))
        {
            if (value < min)
            {
                min = value;
            }

            if (value > max)
            {
                max = value;
            }
        }

        if (double.IsPositiveInfinity(min))
        {
            (min, max) = (0, 1);
        }
        else if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }
    }

    private void NormalizeState()
    {
        if (RowCount <= 0 || ColumnCount <= 0)
        {
            SelectedRow = SelectedColumn = _hoveredRow = _hoveredColumn = -1;
            return;
        }

        SelectedRow = Math.Clamp(SelectedRow < 0 ? 0 : SelectedRow, 0, RowCount - 1);
        SelectedColumn = Math.Clamp(SelectedColumn < 0 ? 0 : SelectedColumn, 0, ColumnCount - 1);
        _hoveredRow = Math.Clamp(_hoveredRow, -1, RowCount - 1);
        _hoveredColumn = Math.Clamp(_hoveredColumn, -1, ColumnCount - 1);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, HeatmapCell? previousCell)
    {
        var selected = ToLinear(SelectedRow, SelectedColumn);
        if (previousIndex == selected)
        {
            return;
        }

        SelectionChanged?.Invoke(this,
            new ListSelectionChangedEventArgs<HeatmapCell?>(previousIndex, selected, previousCell, SelectedCell));
    }

    private bool SetHovered(int row, int column)
    {
        if (_hoveredRow == row && _hoveredColumn == column)
        {
            return false;
        }

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
        if (row >= 0 && row < RowCount && column >= 0 && column < ColumnCount)
        {
            index = row * ColumnCount + column;
            return true;
        }

        index = -1;
        return false;
    }

    private int ToLinear(int row, int column)
    {
        return TryIndex(row, column, out var i) ? i : -1;
    }

    private static void ReplaceLabels(IEnumerable<string> labels, List<string> target)
    {
        ArgumentNullException.ThrowIfNull(labels);
        target.Clear();
        target.AddRange(labels);
    }

    private static int MaxLabelWidth(List<string> labels)
    {
        return labels.Count == 0
            ? 0
            : labels.Max(static label => ControlTextLayout.MeasureDisplayWidth(label));
    }

    private static char HeaderGlyph(string label, int fallback)
    {
        var glyph = label.FirstOrDefault(static value => !char.IsWhiteSpace(value));
        if (glyph != '\0')
        {
            return glyph;
        }

        return (char)('0' + (fallback + 1) % 10);
    }

    private static string Glyph(char glyph)
    {
        return glyph switch
        {
            '░' => "░",
            '▒' => "▒",
            '▓' => "▓",
            '█' => "█",
            _ => glyph.ToString()
        };
    }

    private static string StyleText(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private static void Write(Canvas canvas, int x, int y, string text, TesseraStyle style, int width)
    {
        canvas.WriteText(x, y, StyleText(text, style), width);
    }

    private bool HandleWheel(PointerButton button, bool changed)
    {
        var row = SelectedRow < 0 ? 0 : SelectedRow;
        return button switch
        {
            PointerButton.WheelDown => SetSelectedCell(row + 1, SelectedColumn) || changed,
            PointerButton.WheelUp => SetSelectedCell(row - 1, SelectedColumn) || changed,
            _ => changed
        };
    }

    private bool HandlePress(int rowIndex, int columnIndex, bool changed)
    {
        RequestFocus();
        changed |= SetHovered(rowIndex, columnIndex);
        changed |= SetSelectedCell(rowIndex, columnIndex);
        return changed;
    }

    private readonly record struct PlotLayout(
        Rect Plot,
        int VisibleRows,
        int VisibleColumns,
        int RowLabelX,
        int RowLabelWidth,
        int ColumnY,
        int LegendY);
}
