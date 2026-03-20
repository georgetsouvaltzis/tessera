using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a grid control with column definitions, row data, selection, and optional sorting hooks.
/// </summary>
public sealed partial class DataGrid : Control
{
    private readonly List<DataGridColumn> _columns = [];
    private readonly List<IReadOnlyList<string>> _rows = [];
    private int _selectedRowIndex;
    private int _selectedColumnIndex;
    private int _hoveredRowIndex = -1;
    private int _hoveredColumnIndex = -1;
    private int _scrollOffset;
    private int _sortColumnIndex = -1;
    private bool _sortDescending;
    private int _lastViewportRowCount = 8;

    /// <summary>
    /// Occurs when a sort action is requested for a column that does not provide a built-in comparer.
    /// </summary>
    public event EventHandler<DataGridSortRequestedEventArgs>? SortRequested;

    /// <summary>
    /// Gets or sets the optional title rendered in the frame.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Data Grid";

    /// <summary>
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether the focus marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets title style when the control is not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style when the control is focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for header cells.
    /// </summary>
    public TeaStyle HeaderStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for normal row cells.
    /// </summary>
    public TeaStyle RowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into the selected row.
    /// </summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into the selected cell.
    /// </summary>
    public TeaStyle SelectedCellStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into the hovered row.
    /// </summary>
    public TeaStyle HoveredRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into the hovered cell.
    /// </summary>
    public TeaStyle HoveredCellStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into muted rows.
    /// </summary>
    public TeaStyle MutedStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into all output while disabled.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the separator text rendered between columns.
    /// </summary>
    public string ColumnSeparatorText
    {
        get;
        set => field = value ?? string.Empty;
    } = "|";

    /// <summary>
    /// Gets or sets the marker appended to sorted headers in ascending mode.
    /// </summary>
    public string SortAscendingMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▲";

    /// <summary>
    /// Gets or sets the marker appended to sorted headers in descending mode.
    /// </summary>
    public string SortDescendingMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▼";

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets whether the header row should be rendered.
    /// </summary>
    public bool ShowHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets fallback page size used by PageUp/PageDown navigation.
    /// </summary>
    public int PageSize { get; set; } = 8;

    /// <summary>
    /// Gets or sets an optional predicate that marks rows as muted.
    /// </summary>
    public Func<int, IReadOnlyList<string>, bool>? MutedRowPredicate { get; set; }

    /// <summary>
    /// Gets configured column definitions.
    /// </summary>
    public IReadOnlyList<DataGridColumn> Columns => _columns;

    /// <summary>
    /// Gets configured row data.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>> Rows => _rows;

    /// <summary>
    /// Gets selected row index.
    /// Returns <c>-1</c> when there are no rows.
    /// </summary>
    public int SelectedRowIndex => _rows.Count == 0 ? -1 : _selectedRowIndex;

    /// <summary>
    /// Gets selected column index.
    /// Returns <c>-1</c> when there are no columns.
    /// </summary>
    public int SelectedColumnIndex => _columns.Count == 0 ? -1 : _selectedColumnIndex;

    /// <summary>
    /// Gets selected cell text when row/column are available.
    /// </summary>
    public string? SelectedCellValue => TryGetCellValue(_selectedRowIndex, _selectedColumnIndex);

    /// <summary>
    /// Gets the current sort column index.
    /// Returns <c>-1</c> when no sort has been requested.
    /// </summary>
    public int SortColumnIndex => _sortColumnIndex;

    /// <summary>
    /// Gets a value indicating whether current sort direction is descending.
    /// </summary>
    public bool SortDescending => _sortDescending;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces grid columns.
    /// </summary>
    /// <param name="columns">The new columns in display order.</param>
    public void SetColumns(IEnumerable<DataGridColumn> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        _columns.Clear();
        foreach (var column in columns)
        {
            if (column is not null)
            {
                _columns.Add(column);
            }
        }

        if (_columns.Count == 0)
        {
            _selectedColumnIndex = 0;
            _hoveredColumnIndex = -1;
            _sortColumnIndex = -1;
            _sortDescending = false;
        }
        else
        {
            _selectedColumnIndex = Math.Clamp(_selectedColumnIndex, 0, _columns.Count - 1);
            _hoveredColumnIndex = Math.Clamp(_hoveredColumnIndex, -1, _columns.Count - 1);
            if (_sortColumnIndex < 0 || _sortColumnIndex >= _columns.Count)
            {
                _sortColumnIndex = -1;
                _sortDescending = false;
            }
        }
    }

    /// <summary>
    /// Replaces grid rows.
    /// </summary>
    /// <param name="rows">The new row data.</param>
    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        _rows.Clear();
        foreach (var row in rows)
        {
            if (row is null)
            {
                continue;
            }

            var snapshot = new string[row.Count];
            for (var index = 0; index < row.Count; index++)
            {
                snapshot[index] = row[index] ?? string.Empty;
            }

            _rows.Add(snapshot);
        }

        _selectedRowIndex = _rows.Count == 0 ? 0 : Math.Clamp(_selectedRowIndex, 0, _rows.Count - 1);
        _hoveredRowIndex = Math.Clamp(_hoveredRowIndex, -1, _rows.Count - 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _rows.Count - 1));
    }

    /// <summary>
    /// Selects a specific row/column.
    /// </summary>
    /// <param name="rowIndex">Requested row index.</param>
    /// <param name="columnIndex">Requested column index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SelectCell(int rowIndex, int columnIndex)
    {
        return SetSelectedCell(rowIndex, columnIndex);
    }

    /// <summary>
    /// Requests sorting for a column.
    /// Uses built-in sorting when the column defines <see cref="DataGridColumn.SortComparer" />; otherwise raises
    /// <see cref="SortRequested" /> so application code can handle sorting externally.
    /// </summary>
    /// <param name="columnIndex">The target column index.</param>
    /// <param name="direction">Optional explicit sort direction.</param>
    /// <returns><see langword="true" /> when sort was applied or externally handled; otherwise <see langword="false" />.</returns>
    public bool SortByColumn(int columnIndex, DataGridSortDirection? direction = null)
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
        var descending = resolvedDirection == DataGridSortDirection.Descending;
        var changed = false;

        if (column.SortComparer is not null)
        {
            _rows.Sort((left, right) =>
            {
                var leftValue = GetCellValue(left, columnIndex);
                var rightValue = GetCellValue(right, columnIndex);
                var comparison = column.SortComparer(leftValue, rightValue);
                return descending ? -comparison : comparison;
            });
            changed = true;
        }
        else
        {
            var args = new DataGridSortRequestedEventArgs(columnIndex, column, resolvedDirection);
            SortRequested?.Invoke(this, args);
            changed = args.Handled;
        }

        if (!changed)
        {
            return false;
        }

        _sortColumnIndex = columnIndex;
        _sortDescending = descending;
        _selectedRowIndex = Math.Clamp(_selectedRowIndex, 0, Math.Max(0, _rows.Count - 1));
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _rows.Count - 1));
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (_columns.Count == 0 && _rows.Count == 0)
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
            return SetSelectedCell(Math.Max(0, _rows.Count - 1), Math.Max(0, _columns.Count - 1));
        }

        var page = Math.Max(1, _lastViewportRowCount > 0 ? _lastViewportRowCount : PageSize);
        if (key.Is(Key.PageUp))
        {
            return SetSelectedCell(_selectedRowIndex - page, _selectedColumnIndex);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedCell(_selectedRowIndex + page, _selectedColumnIndex);
        }

        if (!IsReadOnly && key.IsCharacter('s'))
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

        _lastViewportRowCount = ResolveVisibleRowCapacity(content.Height);
        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;

        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHoveredCell(-1, -1);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            if (!inside)
            {
                return changed;
            }

            var hoveredColumn = ResolveHoveredColumnIndex(pointer.X, content);
            var hoveredRow = ResolveHoveredRowIndex(pointer.Y, content);
            return changed | SetHoveredCell(hoveredRow, hoveredColumn);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return changed | SetSelectedCell(_selectedRowIndex + 1, _selectedColumnIndex);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return changed | SetSelectedCell(_selectedRowIndex - 1, _selectedColumnIndex);
            }

            return changed;
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left || !inside)
        {
            return changed || Handle(message);
        }

        RequestFocus();
        if (_columns.Count == 0)
        {
            return true;
        }

        var separatorWidth = ResolveColumnSeparatorWidth();
        var widths = ResolveColumnWidths(content.Width, separatorWidth);
        var columnIndex = HitTestColumn(pointer.X, content.X, widths, separatorWidth);
        if (columnIndex < 0)
        {
            SetHoveredCell(-1, -1);
            return true;
        }

        if (ShowHeader && pointer.Y == content.Y)
        {
            changed |= SetHoveredCell(-1, columnIndex);
            var selectionChanged = SetSelectedCell(_selectedRowIndex, columnIndex);
            var sortChanged = !IsReadOnly && SortByColumn(columnIndex);
            return changed || selectionChanged || sortChanged;
        }

        var firstRowY = content.Y + (ShowHeader ? 1 : 0);
        if (pointer.Y < firstRowY || _rows.Count == 0)
        {
            return true;
        }

        EnsureSelectionVisible(_lastViewportRowCount);
        var rowIndex = _scrollOffset + (pointer.Y - firstRowY);
        changed |= SetHoveredCell(rowIndex, columnIndex);
        return changed | SetSelectedCell(rowIndex, columnIndex);
    }

    private DataGridSortDirection ResolveSortDirection(int columnIndex)
    {
        if (_sortColumnIndex == columnIndex)
        {
            return _sortDescending ? DataGridSortDirection.Ascending : DataGridSortDirection.Descending;
        }

        return DataGridSortDirection.Ascending;
    }

    private bool SetSelectedCell(int rowIndex, int columnIndex)
    {
        if (_rows.Count == 0 && _columns.Count == 0)
        {
            return false;
        }

        var normalizedRow = _rows.Count == 0
            ? 0
            : Math.Clamp(rowIndex, 0, _rows.Count - 1);
        var normalizedColumn = _columns.Count == 0
            ? 0
            : Math.Clamp(columnIndex, 0, _columns.Count - 1);

        if (normalizedRow == _selectedRowIndex && normalizedColumn == _selectedColumnIndex)
        {
            return false;
        }

        _selectedRowIndex = normalizedRow;
        _selectedColumnIndex = normalizedColumn;
        EnsureSelectionVisible(_lastViewportRowCount);
        return true;
    }

    private int ResolveHoveredRowIndex(int pointerY, Rect content)
    {
        var firstRowY = content.Y + (ShowHeader ? 1 : 0);
        if (pointerY < firstRowY || _rows.Count == 0)
        {
            return -1;
        }

        EnsureSelectionVisible(_lastViewportRowCount);
        var rowIndex = _scrollOffset + (pointerY - firstRowY);
        return rowIndex >= 0 && rowIndex < _rows.Count
            ? rowIndex
            : -1;
    }

    private int ResolveHoveredColumnIndex(int pointerX, Rect content)
    {
        if (_columns.Count == 0)
        {
            return -1;
        }

        var separatorWidth = ResolveColumnSeparatorWidth();
        var widths = ResolveColumnWidths(content.Width, separatorWidth);
        return HitTestColumn(pointerX, content.X, widths, separatorWidth);
    }

    private bool SetHoveredCell(int rowIndex, int columnIndex)
    {
        if (_hoveredRowIndex == rowIndex && _hoveredColumnIndex == columnIndex)
        {
            return false;
        }

        _hoveredRowIndex = rowIndex;
        _hoveredColumnIndex = columnIndex;
        return true;
    }

    private bool IsRowMuted(int rowIndex)
    {
        if (MutedRowPredicate is null || rowIndex < 0 || rowIndex >= _rows.Count)
        {
            return false;
        }

        return MutedRowPredicate(rowIndex, _rows[rowIndex]);
    }

    private string? TryGetCellValue(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= _rows.Count || columnIndex < 0 || columnIndex >= _columns.Count)
        {
            return null;
        }

        return GetCellValue(_rows[rowIndex], columnIndex);
    }

    private static string GetCellValue(IReadOnlyList<string> row, int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= row.Count)
        {
            return string.Empty;
        }

        return row[columnIndex] ?? string.Empty;
    }
}
