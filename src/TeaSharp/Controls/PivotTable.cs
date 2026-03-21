using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a pivot-style analytical table with row headers, value columns, and optional sort hooks.
/// </summary>
public sealed partial class PivotTable : Control
{
    private readonly List<PivotTableColumn> _columns = [];
    private readonly List<string> _rowKeys = [];
    private readonly Dictionary<string, Dictionary<string, string>> _cells = new(StringComparer.Ordinal);
    private int _selectedRowIndex;
    private int _selectedColumnIndex;
    private int _scrollOffset;
    private int _lastViewportRows = 8;
    private int _sortColumnIndex = -1;
    private bool _sortDescending;

    /// <summary>Occurs when sorting is requested for a column without a built-in comparer.</summary>
    public event EventHandler<PivotSortRequestedEventArgs>? SortRequested;

    /// <summary>Gets or sets the title rendered in the control frame.</summary>
    public string Title { get; set; } = "Pivot Table";

    /// <summary>Gets or sets the marker appended to the title when focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether the focus marker should be rendered.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets title style when the control is not focused.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets title style when the control is focused.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style for header cells.</summary>
    public TeaStyle HeaderStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style for body cells.</summary>
    public TeaStyle BodyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style merged into selected cells.</summary>
    public TeaStyle SelectedCellStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style merged into selected cells when focused.</summary>
    public TeaStyle FocusedCellStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style merged into all rendered output while disabled.</summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style applied to border glyphs.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets style merged into border glyphs while focused.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets header text for row keys.</summary>
    public string RowHeaderTitle { get; set; } = "Row";

    /// <summary>Gets or sets marker appended to the sorted header in ascending mode.</summary>
    public string SortAscendingMarker { get; set; } = "▲";

    /// <summary>Gets or sets marker appended to the sorted header in descending mode.</summary>
    public string SortDescendingMarker { get; set; } = "▼";

    /// <summary>Gets or sets fallback text rendered when rows/columns are missing.</summary>
    public string EmptyText { get; set; } = "(empty)";

    /// <summary>Gets or sets the border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets fallback page size used by PageUp/PageDown.</summary>
    public int PageSize { get; set; } = 8;

    /// <summary>Gets configured value columns.</summary>
    public IReadOnlyList<PivotTableColumn> Columns => _columns;

    /// <summary>Gets configured row keys.</summary>
    public IReadOnlyList<string> RowKeys => _rowKeys;

    /// <summary>Gets selected row index, or <c>-1</c> when there are no rows.</summary>
    public int SelectedRowIndex => _rowKeys.Count == 0 ? -1 : _selectedRowIndex;

    /// <summary>Gets selected value-column index, or <c>-1</c> when there are no columns.</summary>
    public int SelectedColumnIndex => _columns.Count == 0 ? -1 : _selectedColumnIndex;

    /// <summary>Gets selected value, or <see langword="null"/> when selection is invalid.</summary>
    public string? SelectedCellValue => TryGetCellValue(_selectedRowIndex, _selectedColumnIndex);

    /// <summary>Gets current sort column index, or <c>-1</c> when not sorted.</summary>
    public int SortColumnIndex => _sortColumnIndex;

    /// <summary>Gets a value indicating whether the active sort direction is descending.</summary>
    public bool SortDescending => _sortDescending;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces pivot value columns.
    /// </summary>
    /// <param name="columns">Columns in display order.</param>
    public void SetColumns(IEnumerable<PivotTableColumn> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        _columns.Clear();
        foreach (var column in columns)
        {
            if (column is null)
            {
                continue;
            }

            _columns.Add(new PivotTableColumn(column.Key, column.Header)
            {
                IsSortable = column.IsSortable,
                SortComparer = column.SortComparer,
            });
        }

        _selectedColumnIndex = _columns.Count == 0 ? 0 : Math.Clamp(_selectedColumnIndex, 0, _columns.Count - 1);
        if (_sortColumnIndex < 0 || _sortColumnIndex >= _columns.Count)
        {
            _sortColumnIndex = -1;
            _sortDescending = false;
        }
    }

    /// <summary>
    /// Replaces row keys in display order.
    /// </summary>
    /// <param name="rowKeys">Row keys.</param>
    public void SetRows(IEnumerable<string> rowKeys)
    {
        ArgumentNullException.ThrowIfNull(rowKeys);
        _rowKeys.Clear();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var rowKey in rowKeys)
        {
            var safe = rowKey ?? string.Empty;
            if (!seen.Add(safe))
            {
                continue;
            }

            _rowKeys.Add(safe);
            _ = EnsureRowMap(safe);
        }

        _selectedRowIndex = _rowKeys.Count == 0 ? 0 : Math.Clamp(_selectedRowIndex, 0, _rowKeys.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _rowKeys.Count - 1));
    }

    /// <summary>
    /// Replaces pivot values.
    /// </summary>
    /// <param name="cells">Pivot values.</param>
    public void SetCells(IEnumerable<PivotTableCell> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _cells.Clear();
        foreach (var rowKey in _rowKeys)
        {
            _ = EnsureRowMap(rowKey);
        }

        foreach (var cell in cells)
        {
            if (cell is not null)
            {
                SetValue(cell.RowKey, cell.ColumnKey, cell.Value);
            }
        }
    }

    /// <summary>
    /// Sets one pivot value.
    /// </summary>
    /// <param name="rowKey">Row key.</param>
    /// <param name="columnKey">Column key.</param>
    /// <param name="value">Display value.</param>
    public void SetValue(string rowKey, string columnKey, string value)
    {
        var safeRow = rowKey ?? string.Empty;
        var safeColumn = columnKey ?? string.Empty;
        if (!_rowKeys.Contains(safeRow, StringComparer.Ordinal))
        {
            _rowKeys.Add(safeRow);
        }

        var rowMap = EnsureRowMap(safeRow);
        rowMap[safeColumn] = value ?? string.Empty;
    }

    /// <summary>
    /// Selects a cell by row and value-column index.
    /// </summary>
    /// <param name="rowIndex">Target row index.</param>
    /// <param name="columnIndex">Target value-column index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
    public bool SelectCell(int rowIndex, int columnIndex)
    {
        if (_rowKeys.Count == 0 || _columns.Count == 0)
        {
            return false;
        }

        var normalizedRow = Math.Clamp(rowIndex, 0, _rowKeys.Count - 1);
        var normalizedColumn = Math.Clamp(columnIndex, 0, _columns.Count - 1);
        if (normalizedRow == _selectedRowIndex && normalizedColumn == _selectedColumnIndex)
        {
            return false;
        }

        _selectedRowIndex = normalizedRow;
        _selectedColumnIndex = normalizedColumn;
        EnsureSelectionVisible(_lastViewportRows);
        return true;
    }

    /// <summary>
    /// Sorts by one value column.
    /// </summary>
    /// <param name="columnIndex">Target column index.</param>
    /// <param name="direction">Optional explicit direction.</param>
    /// <returns><see langword="true"/> when sorting was applied or handled; otherwise <see langword="false"/>.</returns>
    public bool SortByColumn(int columnIndex, PivotSortDirection? direction = null)
    {
        if (columnIndex < 0 || columnIndex >= _columns.Count)
        {
            return false;
        }

        var column = _columns[columnIndex];
        if (!column.IsSortable && SortRequested is null)
        {
            return false;
        }

        var resolvedDirection = direction ?? ResolveSortDirection(columnIndex);
        var descending = resolvedDirection == PivotSortDirection.Descending;
        var changed = false;
        if (column.SortComparer is not null)
        {
            _rowKeys.Sort((left, right) =>
            {
                var leftValue = GetCellValue(left, column.Key);
                var rightValue = GetCellValue(right, column.Key);
                var comparison = column.SortComparer(leftValue, rightValue);
                return descending ? -comparison : comparison;
            });
            changed = true;
        }
        else
        {
            var args = new PivotSortRequestedEventArgs(columnIndex, column, resolvedDirection);
            SortRequested?.Invoke(this, args);
            changed = args.Handled;
        }

        if (!changed)
        {
            return false;
        }

        _sortColumnIndex = columnIndex;
        _sortDescending = descending;
        _selectedRowIndex = _rowKeys.Count == 0 ? 0 : Math.Clamp(_selectedRowIndex, 0, _rowKeys.Count - 1);
        EnsureSelectionVisible(_lastViewportRows);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (_columns.Count == 0 && _rowKeys.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Left))
        {
            return SetSelectedCell(_selectedRowIndex, _selectedColumnIndex - 1);
        }

        if (key.Is(Key.Right))
        {
            return SetSelectedCell(_selectedRowIndex, _selectedColumnIndex + 1);
        }

        if (key.Is(Key.Up))
        {
            return SetSelectedCell(_selectedRowIndex - 1, _selectedColumnIndex);
        }

        if (key.Is(Key.Down))
        {
            return SetSelectedCell(_selectedRowIndex + 1, _selectedColumnIndex);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedCell(0, 0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedCell(Math.Max(0, _rowKeys.Count - 1), Math.Max(0, _columns.Count - 1));
        }

        var page = Math.Max(1, _lastViewportRows > 0 ? _lastViewportRows : PageSize);
        if (key.Is(Key.PageUp))
        {
            return SetSelectedCell(_selectedRowIndex - page, _selectedColumnIndex);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedCell(_selectedRowIndex + page, _selectedColumnIndex);
        }

        if (!IsReadOnly && (key.Is(Key.Enter) || key.IsCharacter('s')))
        {
            var selectedColumn = SelectedColumnIndex < 0 ? 0 : SelectedColumnIndex;
            return SortByColumn(selectedColumn);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var rowCapacity = ResolveVisibleRowCapacity(content.Height);
        _lastViewportRows = rowCapacity;
        if (pointer.Kind == PointerEventKind.Wheel && _rowKeys.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedCell(_selectedRowIndex + 1, _selectedColumnIndex);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedCell(_selectedRowIndex - 1, _selectedColumnIndex);
            }
        }

        if (pointer.Kind != PointerEventKind.Press
            || pointer.Button != PointerButton.Left
            || !content.Contains(pointer.X, pointer.Y))
        {
            return Handle(message);
        }

        RequestFocus();
        if (_columns.Count == 0)
        {
            return true;
        }

        var widths = ResolveColumnWidths(content.Width);
        var headerY = content.Y;
        var rowHeaderWidth = widths.RowHeader;
        if (pointer.Y == headerY)
        {
            var columnIndex = HitTestValueColumn(pointer.X, content.X, rowHeaderWidth, widths.ValueColumns);
            return !IsReadOnly && columnIndex >= 0 && SortByColumn(columnIndex);
        }

        if (_rowKeys.Count == 0)
        {
            return true;
        }

        EnsureSelectionVisible(rowCapacity);
        var rowIndex = _scrollOffset + (pointer.Y - (headerY + 1));
        if (rowIndex < 0 || rowIndex >= _rowKeys.Count)
        {
            return true;
        }

        var selectedColumn = HitTestValueColumn(pointer.X, content.X, rowHeaderWidth, widths.ValueColumns);
        if (selectedColumn < 0)
        {
            selectedColumn = _selectedColumnIndex;
        }

        return SetSelectedCell(rowIndex, selectedColumn);
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
        if (content.IsEmpty)
        {
            return;
        }

        if (_columns.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(no columns)", ResolveBodyStyle(selected: false, focusedCell: false)), content.Width);
            return;
        }

        var widths = ResolveColumnWidths(content.Width);
        WriteHeader(canvas, content, widths);
        if (_rowKeys.Count == 0)
        {
            if (content.Height > 1)
            {
                canvas.WriteText(content.X, content.Y + 1, ApplyStyle(EmptyText, ResolveBodyStyle(selected: false, focusedCell: false)), content.Width);
            }

            return;
        }

        var rowCapacity = ResolveVisibleRowCapacity(content.Height);
        _lastViewportRows = rowCapacity;
        EnsureSelectionVisible(rowCapacity);

        for (var visibleRow = 0; visibleRow < rowCapacity; visibleRow++)
        {
            var rowIndex = _scrollOffset + visibleRow;
            var y = content.Y + 1 + visibleRow;
            if (rowIndex < 0 || rowIndex >= _rowKeys.Count || y >= content.Bottom)
            {
                break;
            }

            WriteRow(canvas, content, widths, y, rowIndex);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
        var rowHeaderWidth = Math.Max(3, ControlTextLayout.MeasureDisplayWidth(RowHeaderTitle));
        for (var rowIndex = 0; rowIndex < _rowKeys.Count; rowIndex++)
        {
            rowHeaderWidth = Math.Max(rowHeaderWidth, ControlTextLayout.MeasureDisplayWidth(_rowKeys[rowIndex]));
        }

        width += rowHeaderWidth + 1;
        for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
        {
            var columnWidth = Math.Max(3, ControlTextLayout.MeasureDisplayWidth(RenderHeaderText(columnIndex)));
            width += columnWidth + 1;
        }

        var rows = Math.Max(1, Math.Min(Math.Max(1, PageSize), _rowKeys.Count == 0 ? 1 : _rowKeys.Count));
        var height = rows + 1;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        width += Padding.Horizontal;
        height += Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private PivotSortDirection ResolveSortDirection(int columnIndex)
    {
        if (_sortColumnIndex == columnIndex)
        {
            return _sortDescending ? PivotSortDirection.Ascending : PivotSortDirection.Descending;
        }

        return PivotSortDirection.Ascending;
    }

    private bool SetSelectedCell(int rowIndex, int columnIndex)
    {
        if (_rowKeys.Count == 0 || _columns.Count == 0)
        {
            return false;
        }

        var normalizedRow = Math.Clamp(rowIndex, 0, _rowKeys.Count - 1);
        var normalizedColumn = Math.Clamp(columnIndex, 0, _columns.Count - 1);
        if (normalizedRow == _selectedRowIndex && normalizedColumn == _selectedColumnIndex)
        {
            return false;
        }

        _selectedRowIndex = normalizedRow;
        _selectedColumnIndex = normalizedColumn;
        EnsureSelectionVisible(_lastViewportRows);
        return true;
    }

    private void EnsureSelectionVisible(int rowCapacity)
    {
        if (_rowKeys.Count == 0)
        {
            _scrollOffset = 0;
            return;
        }

        var safeCapacity = Math.Max(1, rowCapacity);
        if (_selectedRowIndex < _scrollOffset)
        {
            _scrollOffset = _selectedRowIndex;
        }
        else if (_selectedRowIndex >= _scrollOffset + safeCapacity)
        {
            _scrollOffset = _selectedRowIndex - safeCapacity + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _rowKeys.Count - safeCapacity));
    }

    private static int ResolveVisibleRowCapacity(int contentHeight) => Math.Max(1, contentHeight - 1);

    private string? TryGetCellValue(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= _rowKeys.Count || columnIndex < 0 || columnIndex >= _columns.Count)
        {
            return null;
        }

        return GetCellValue(_rowKeys[rowIndex], _columns[columnIndex].Key);
    }

    private string GetCellValue(string rowKey, string columnKey)
    {
        if (!_cells.TryGetValue(rowKey, out var rowMap) || !rowMap.TryGetValue(columnKey, out var value))
        {
            return string.Empty;
        }

        return value ?? string.Empty;
    }

    private Dictionary<string, string> EnsureRowMap(string rowKey)
    {
        if (_cells.TryGetValue(rowKey, out var rowMap))
        {
            return rowMap;
        }

        rowMap = new Dictionary<string, string>(StringComparer.Ordinal);
        _cells[rowKey] = rowMap;
        return rowMap;
    }

    private void WriteHeader(Canvas canvas, Rect content, ColumnWidths widths)
    {
        var x = content.X;
        var headerStyle = ResolveHeaderStyle();
        WriteCell(canvas, x, content.Y, RowHeaderTitle, widths.RowHeader, headerStyle, content.Right - x);
        x += widths.RowHeader;
        if (x < content.Right)
        {
            WriteCell(canvas, x, content.Y, " ", 1, headerStyle, content.Right - x);
            x++;
        }

        for (var columnIndex = 0; columnIndex < _columns.Count && x < content.Right; columnIndex++)
        {
            var width = widths.ValueColumns[columnIndex];
            WriteCell(canvas, x, content.Y, RenderHeaderText(columnIndex), width, headerStyle, content.Right - x);
            x += width;
            if (columnIndex < _columns.Count - 1 && x < content.Right)
            {
                WriteCell(canvas, x, content.Y, " ", 1, headerStyle, content.Right - x);
                x++;
            }
        }
    }

    private void WriteRow(Canvas canvas, Rect content, ColumnWidths widths, int y, int rowIndex)
    {
        var rowKey = _rowKeys[rowIndex];
        var selectedRow = rowIndex == _selectedRowIndex;
        var x = content.X;
        WriteCell(canvas, x, y, rowKey, widths.RowHeader, ResolveBodyStyle(selectedRow, focusedCell: false), content.Right - x);
        x += widths.RowHeader;
        if (x < content.Right)
        {
            WriteCell(canvas, x, y, " ", 1, ResolveBodyStyle(selectedRow, focusedCell: false), content.Right - x);
            x++;
        }

        for (var columnIndex = 0; columnIndex < _columns.Count && x < content.Right; columnIndex++)
        {
            var focusedCell = selectedRow && columnIndex == _selectedColumnIndex;
            var value = GetCellValue(rowKey, _columns[columnIndex].Key);
            var width = widths.ValueColumns[columnIndex];
            WriteCell(canvas, x, y, value, width, ResolveBodyStyle(selectedRow, focusedCell), content.Right - x);
            x += width;
            if (columnIndex < _columns.Count - 1 && x < content.Right)
            {
                WriteCell(canvas, x, y, " ", 1, ResolveBodyStyle(selectedRow, focusedCell: false), content.Right - x);
                x++;
            }
        }
    }

    private ColumnWidths ResolveColumnWidths(int contentWidth)
    {
        if (_columns.Count == 0 || contentWidth <= 0)
        {
            return new ColumnWidths(Math.Max(1, contentWidth), []);
        }

        var rowHeader = Math.Max(3, ControlTextLayout.MeasureDisplayWidth(RowHeaderTitle));
        for (var rowIndex = 0; rowIndex < _rowKeys.Count; rowIndex++)
        {
            rowHeader = Math.Max(rowHeader, ControlTextLayout.MeasureDisplayWidth(_rowKeys[rowIndex]));
        }

        var valueWidths = new int[_columns.Count];
        for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
        {
            valueWidths[columnIndex] = Math.Max(3, ControlTextLayout.MeasureDisplayWidth(RenderHeaderText(columnIndex)));
        }

        var separators = _columns.Count; // one between row header and first value, then between value columns
        var budget = Math.Max(_columns.Count, contentWidth - rowHeader - separators);
        var total = 0;
        for (var index = 0; index < valueWidths.Length; index++)
        {
            total += valueWidths[index];
        }

        if (total > budget)
        {
            var shrinkIndex = valueWidths.Length - 1;
            while (total > budget && shrinkIndex >= 0)
            {
                if (valueWidths[shrinkIndex] > 3)
                {
                    valueWidths[shrinkIndex]--;
                    total--;
                }
                else
                {
                    shrinkIndex--;
                }
            }
        }
        else if (total < budget)
        {
            valueWidths[^1] += budget - total;
        }

        return new ColumnWidths(rowHeader, valueWidths);
    }

    private static int HitTestValueColumn(int pointerX, int contentX, int rowHeaderWidth, IReadOnlyList<int> valueWidths)
    {
        var cursor = contentX + rowHeaderWidth;
        cursor++; // separator after row-header
        for (var columnIndex = 0; columnIndex < valueWidths.Count; columnIndex++)
        {
            var width = Math.Max(0, valueWidths[columnIndex]);
            if (pointerX >= cursor && pointerX < cursor + width)
            {
                return columnIndex;
            }

            cursor += width;
            if (columnIndex < valueWidths.Count - 1)
            {
                cursor++;
            }
        }

        return -1;
    }

    private string RenderHeaderText(int columnIndex)
    {
        var text = _columns[columnIndex].Header;
        if (columnIndex == _sortColumnIndex)
        {
            var marker = _sortDescending ? SortDescendingMarker : SortAscendingMarker;
            if (!string.IsNullOrEmpty(marker))
            {
                text = string.Concat(text, " ", marker);
            }
        }

        return text;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title, " ", FocusMarker);
        }

        return Title;
    }

    private TeaStyle ResolveHeaderStyle()
    {
        var style = HeaderStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TeaStyle ResolveBodyStyle(bool selected, bool focusedCell)
    {
        var style = BodyStyle;
        if (selected)
        {
            style = style.Merge(SelectedCellStyle);
        }

        if (focusedCell && IsFocused)
        {
            style = style.Merge(FocusedCellStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private static void WriteCell(Canvas canvas, int x, int y, string text, int width, TeaStyle style, int maxWidth)
    {
        var effectiveWidth = Math.Max(0, Math.Min(width, maxWidth));
        if (effectiveWidth <= 0)
        {
            return;
        }

        if (style.IsEmpty)
        {
            canvas.WriteTextPadded(x, y, text, effectiveWidth);
            return;
        }

        var padded = PadToWidth(text, effectiveWidth);
        canvas.WriteText(x, y, style.Render(padded), maxWidth);
    }

    private static string PadToWidth(string value, int width)
    {
        var text = value ?? string.Empty;
        if (width <= 0)
        {
            return string.Empty;
        }

        var firstControlCharacter = text.AsSpan().IndexOfAny('\r', '\n');
        if (firstControlCharacter < 0)
        {
            if (text.Length > width)
            {
                return text[..width];
            }

            return text.Length < width
                ? text.PadRight(width)
                : text;
        }

        return string.Create(
            width,
            text,
            static (destination, source) =>
            {
                destination.Fill(' ');
                var writeIndex = 0;
                for (var readIndex = 0; readIndex < source.Length && writeIndex < destination.Length; readIndex++)
                {
                    var current = source[readIndex];
                    if (current == '\r')
                    {
                        continue;
                    }

                    destination[writeIndex++] = current == '\n' ? ' ' : current;
                }
            });
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }

    private readonly record struct ColumnWidths(int RowHeader, int[] ValueColumns);
}
