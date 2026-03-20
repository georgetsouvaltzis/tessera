using System.Text;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a tabular data viewer.
/// </summary>
public sealed class Table : Control
{
    private readonly List<IReadOnlyList<string>> _rows = [];
    private readonly IReadOnlyList<string> _columns;
    private int _hoveredVisibleRow = -1;
    private int _selectedVisibleRow = -1;

    public Table(IReadOnlyList<string> columns)
    {
        _columns = columns ?? Array.Empty<string>();
    }

    public Table(params string[] columns)
        : this((IReadOnlyList<string>)columns)
    {
    }

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Table";

    /// <summary>
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be rendered in the title when focused.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style applied when the control is not focused.
    /// </summary>
    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style applied when the control is focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to header rows.
    /// </summary>
    public TeaStyle HeaderStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to non-selected, non-hovered data rows.
    /// </summary>
    public TeaStyle RowStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into hovered data rows.
    /// </summary>
    public TeaStyle HoveredRowStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into selected data rows.
    /// </summary>
    public TeaStyle SelectedRowStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the border style used for the table frame.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets the inner padding applied to the table body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    public int PageSize
    {
        get;
        set;
    } = 8;

    public int PageIndex
    {
        get;
        private set;
    }

    public int SortColumn
    {
        get;
        private set;
    }

    public bool SortDescending
    {
        get;
        private set;
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        _rows.Clear();
        foreach (var row in rows)
        {
            _rows.Add(row);
        }

        NormalizePage();
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || _columns.Count == 0 || _rows.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.IsCharacter(']'))
        {
            PageIndex++;
            NormalizePage();
            return true;
        }

        if (key.IsCharacter('['))
        {
            var previousPage = PageIndex;
            PageIndex = Math.Max(0, PageIndex - 1);
            return PageIndex != previousPage;
        }

        if (key.IsCharacter('s'))
        {
            SortDescending = !SortDescending;
            return true;
        }

        if (key.IsCharacter('c'))
        {
            SortColumn = (SortColumn + 1) % _columns.Count;
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (_columns.Count == 0 || _rows.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var state = BuildState();
        var content = TableViewState.ResolveContentRect(bounds, Border, Padding, state.Title);
        if (content.IsEmpty || content.Height < 3)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredVisibleRow(-1);
            }

            if (pointer.Kind != PointerEventKind.Wheel)
            {
                return changed || Handle(message);
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            var previousPage = PageIndex;
            if (pointer.Button == PointerButton.WheelDown)
            {
                PageIndex++;
                NormalizePage();
            }
            else if (pointer.Button == PointerButton.WheelUp)
            {
                PageIndex = Math.Max(0, PageIndex - 1);
            }

            NormalizeVisibleRowPointers(state.VisibleRowCount);
            return changed || PageIndex != previousPage;
        }

        if (!inside)
        {
            return changed || Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredVisibleRow(TableViewState.RowFromPointer(content, pointer.Y, state.VisibleRowCount));
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            var headerColumn = TableViewState.HeaderColumnFromPointer(pointer.X, content, _columns.Count);
            if (pointer.Y == content.Y && headerColumn >= 0)
            {
                if (headerColumn == SortColumn)
                {
                    SortDescending = !SortDescending;
                }
                else
                {
                    SortColumn = headerColumn;
                    SortDescending = false;
                }

                return true;
            }

            var row = TableViewState.RowFromPointer(content, pointer.Y, state.VisibleRowCount);
            if (row >= 0)
            {
                return SetHoveredVisibleRow(row) | SetSelectedVisibleRow(row);
            }
        }

        return changed || Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _columns.Count == 0)
        {
            return;
        }

        var state = BuildState();
        NormalizeVisibleRowPointers(state.VisibleRowCount);

        var showBorder = Border != BorderStyle.None;
        Rect content;
        if (showBorder)
        {
            canvas.DrawBox(clipped, state.Title, Border, ResolveBorderStyleText());
            content = clipped.Inset(1, 1).Inset(Padding);
        }
        else
        {
            content = clipped.Inset(Padding);
            if (!string.IsNullOrWhiteSpace(state.Title))
            {
                canvas.WriteText(content.X, content.Y, state.Title, content.Width);
                content = new Rect(content.X, content.Y + 1, content.Width, content.Height - 1);
            }
        }

        if (content.Height < 3 || content.Width <= 0)
        {
            return;
        }

        var separatorCount = _columns.Count - 1;
        var availableWidth = Math.Max(_columns.Count, content.Width - separatorCount);
        var widths = ComputeColumnWidths(availableWidth, _columns.Count);

        var header = BuildRowText(widths, _columns, selectedMarker: false);
        canvas.WriteText(content.X, content.Y, ApplyStyle(header, HeaderStyle), content.Width);

        var dividerY = content.Y + 1;
        canvas.DrawHorizontalLine(content.X, dividerY, content.Width, '─');
        var separatorX = content.X;
        for (var index = 0; index < widths.Length - 1; index++)
        {
            separatorX += widths[index];
            canvas.Set(separatorX, dividerY, '┼');
            separatorX++;
        }

        var maxRows = Math.Min(state.VisibleRows.Count, Math.Max(0, content.Height - 2));
        for (var row = 0; row < maxRows; row++)
        {
            var markerRow = _selectedVisibleRow >= 0
                ? row == _selectedVisibleRow
                : row == _hoveredVisibleRow;
            var isHovered = row == _hoveredVisibleRow;
            var isSelected = row == _selectedVisibleRow;
            var rowText = BuildRowText(widths, state.VisibleRows[row], markerRow);
            var style = ResolveRowStyle(selected: isSelected, hovered: isHovered);
            canvas.WriteText(content.X, content.Y + 2 + row, ApplyStyle(rowText, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var widest = 0;
        for (var i = 0; i < _columns.Count; i++)
        {
            widest = Math.Max(widest, _columns[i].Length);
        }

        for (var rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            var row = _rows[rowIndex];
            for (var cellIndex = 0; cellIndex < row.Count; cellIndex++)
            {
                widest = Math.Max(widest, row[cellIndex]?.Length ?? 0);
            }
        }

        var width = Math.Max(8, (_columns.Count * Math.Max(3, widest)) + Math.Max(0, _columns.Count - 1));
        var height = Math.Max(4, Math.Min(_rows.Count, Math.Max(1, PageSize)) + 3);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private TableRenderState BuildState()
    {
        return TableViewState.Build(
            _rows,
            _columns,
            FormatTitle(),
            SortColumn,
            SortDescending,
            PageSize,
            PageIndex);
    }

    private void NormalizePage()
    {
        var safePageSize = Math.Max(1, PageSize);
        var pageCount = Math.Max(1, (_rows.Count + safePageSize - 1) / safePageSize);
        PageIndex = Math.Clamp(PageIndex, 0, pageCount - 1);
    }

    private void NormalizeVisibleRowPointers(int visibleRows)
    {
        (_hoveredVisibleRow, _selectedVisibleRow) = TableViewState.NormalizeVisibleRowPointers(
            _hoveredVisibleRow,
            _selectedVisibleRow,
            visibleRows);
    }

    private bool SetHoveredVisibleRow(int row)
    {
        if (_hoveredVisibleRow == row)
        {
            return false;
        }

        _hoveredVisibleRow = row;
        return true;
    }

    private bool SetSelectedVisibleRow(int row)
    {
        if (_selectedVisibleRow == row)
        {
            return false;
        }

        _selectedVisibleRow = row;
        return true;
    }

    private TeaStyle ResolveBorderStyleText()
    {
        return IsFocused
            ? BorderStyleText.Merge(FocusedBorderStyleText)
            : BorderStyleText;
    }

    private TeaStyle ResolveRowStyle(bool selected, bool hovered)
    {
        var style = RowStyle;
        if (hovered)
        {
            style = style.Merge(HoveredRowStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedRowStyle);
        }

        return style;
    }

    private string FormatTitle()
    {
        var title = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            title = $"{title} {FocusMarker}";
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (style.IsEmpty || string.IsNullOrEmpty(title))
        {
            return title;
        }

        return style.Render(title);
    }

    private static int[] ComputeColumnWidths(int width, int columns)
    {
        var widths = new int[columns];
        var baseWidth = width / columns;
        var remainder = width % columns;
        for (var index = 0; index < columns; index++)
        {
            widths[index] = baseWidth + (index < remainder ? 1 : 0);
        }

        return widths;
    }

    private static string BuildRowText(int[] widths, IReadOnlyList<string> cells, bool selectedMarker)
    {
        var totalWidth = 0;
        for (var index = 0; index < widths.Length; index++)
        {
            totalWidth += widths[index];
        }

        var builder = new StringBuilder(Math.Max(0, totalWidth + widths.Length - 1));
        for (var column = 0; column < widths.Length; column++)
        {
            var width = widths[column];
            var value = column < cells.Count ? cells[column] ?? string.Empty : string.Empty;
            if (selectedMarker && column == 0 && width >= 2)
            {
                value = string.Concat("› ", value);
            }

            builder.Append(FitText(value, width));
            if (column < widths.Length - 1)
            {
                builder.Append('│');
            }
        }

        return builder.ToString();
    }

    private static string FitText(string text, int width)
    {
        if (width <= 0)
        {
            return string.Empty;
        }

        if (text.Length >= width)
        {
            return text[..width];
        }

        return text.PadRight(width);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty || string.IsNullOrEmpty(text)
            ? text
            : style.Render(text);
    }
}
