using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class DataGrid
{
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
        if (content.IsEmpty)
        {
            return;
        }

        if (_columns.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(no columns)", MutedStyle), content.Width);
            return;
        }

        var separator = ResolveColumnSeparatorText();
        var separatorWidth = ResolveColumnSeparatorWidth(separator);
        var widths = ResolveColumnWidths(content.Width, separatorWidth);
        var y = content.Y;
        if (ShowHeader && y < content.Bottom)
        {
            WriteHeader(canvas, content, y, widths, separator, separatorWidth);
            y++;
        }

        var rowCapacity = ResolveVisibleRowCapacity(content.Height);
        _lastViewportRowCount = rowCapacity;
        if (_rows.Count == 0)
        {
            if (y <= content.Bottom)
            {
                canvas.WriteText(content.X, y, ApplyStyle("(empty)", MutedStyle), content.Width);
            }

            return;
        }

        EnsureSelectionVisible(rowCapacity);
        for (var row = 0; row < rowCapacity; row++)
        {
            var rowIndex = _scrollOffset + row;
            if (rowIndex < 0 || rowIndex >= _rows.Count || y + row > content.Bottom)
            {
                break;
            }

            WriteRow(canvas, content, y + row, widths, rowIndex, separator, separatorWidth);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var minimumWidth = _columns.Count == 0 ? 12 : _columns.Count * 8 + Math.Max(0, _columns.Count - 1);
        var width = minimumWidth;
        var separatorWidth = ResolveColumnSeparatorWidth(ResolveColumnSeparatorText());
        for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(RenderHeaderText(columnIndex)) + 2);
        }

        for (var rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            var row = _rows[rowIndex];
            var lineWidth = 0;
            for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
            {
                lineWidth += Math.Max(3, ControlTextLayout.MeasureDisplayWidth(GetCellValue(row, columnIndex)));
            }

            lineWidth += Math.Max(0, _columns.Count - 1) * separatorWidth;
            width = Math.Max(width, lineWidth);
        }

        var rowCapacity = _rows.Count == 0 ? 1 : Math.Min(Math.Max(1, PageSize), _rows.Count);
        var height = rowCapacity + (ShowHeader ? 1 : 0);
        if (Border != BorderStyle.None)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
            width += 2;
            height += 2;
        }

        width += Padding.Horizontal;
        height += Padding.Vertical;

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void EnsureSelectionVisible(int rowCapacity)
    {
        if (_rows.Count == 0)
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

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _rows.Count - safeCapacity));
    }

    private int ResolveVisibleRowCapacity(int contentHeight)
    {
        var rows = contentHeight - (ShowHeader ? 1 : 0);
        return Math.Max(1, rows);
    }

    private int[] ResolveColumnWidths(int availableWidth, int separatorWidth)
    {
        var count = _columns.Count;
        var widths = new int[count];
        if (count == 0 || availableWidth <= 0)
        {
            return widths;
        }

        var totalSeparatorWidth = Math.Max(0, count - 1) * separatorWidth;
        var budget = Math.Max(count, availableWidth - totalSeparatorWidth);
        var total = 0;
        for (var index = 0; index < count; index++)
        {
            var explicitWidth = _columns[index].Width ?? -1;
            if (explicitWidth > 0)
            {
                widths[index] = explicitWidth;
            }
            else
            {
                widths[index] = Math.Max(3, ControlTextLayout.MeasureDisplayWidth(RenderHeaderText(index)));
            }

            total += widths[index];
        }

        if (total > budget)
        {
            var index = count - 1;
            while (total > budget && index >= 0)
            {
                if (widths[index] > 3)
                {
                    widths[index]--;
                    total--;
                }
                else
                {
                    index--;
                }
            }
        }
        else if (total < budget)
        {
            widths[count - 1] += budget - total;
        }

        return widths;
    }

    private static int HitTestColumn(int pointerX, int contentX, int[] widths, int separatorWidth)
    {
        var cursor = contentX;
        for (var index = 0; index < widths.Length; index++)
        {
            var width = Math.Max(0, widths[index]);
            if (pointerX >= cursor && pointerX < cursor + width)
            {
                return index;
            }

            cursor += width;
            if (index < widths.Length - 1 && separatorWidth > 0)
            {
                cursor += separatorWidth;
            }
        }

        return -1;
    }

    private void WriteHeader(Canvas canvas, Rect content, int y, int[] widths, string separator, int separatorWidth)
    {
        var x = content.X;
        for (var columnIndex = 0; columnIndex < _columns.Count && x < content.Right; columnIndex++)
        {
            var width = Math.Max(1, widths[columnIndex]);
            var remainingWidth = content.Right - x;
            var style = HeaderStyle;
            if (IsDisabled)
            {
                style = style.Merge(DisabledStyle);
            }

            WritePaddedCell(canvas, x, y, RenderHeaderText(columnIndex), width, style, remainingWidth);
            x += width;
            if (columnIndex < _columns.Count - 1 && separatorWidth > 0 && x < content.Right)
            {
                WritePaddedCell(canvas, x, y, separator, separatorWidth, style, content.Right - x);
                x += separatorWidth;
            }
        }
    }

    private void WriteRow(Canvas canvas, Rect content, int y, int[] widths, int rowIndex, string separator, int separatorWidth)
    {
        var x = content.X;
        var row = _rows[rowIndex];
        for (var columnIndex = 0; columnIndex < _columns.Count && x < content.Right; columnIndex++)
        {
            var width = Math.Max(1, widths[columnIndex]);
            var remainingWidth = content.Right - x;
            var style = ResolveCellStyle(rowIndex, columnIndex);
            WritePaddedCell(canvas, x, y, GetCellValue(row, columnIndex), width, style, remainingWidth);
            x += width;
            if (columnIndex < _columns.Count - 1 && separatorWidth > 0 && x < content.Right)
            {
                WritePaddedCell(canvas, x, y, separator, separatorWidth, style, content.Right - x);
                x += separatorWidth;
            }
        }
    }

    private TeaStyle ResolveCellStyle(int rowIndex, int columnIndex)
    {
        var style = RowStyle;
        if (IsRowMuted(rowIndex))
        {
            style = style.Merge(MutedStyle);
        }

        if (rowIndex == _selectedRowIndex)
        {
            style = style.Merge(SelectedRowStyle);
        }

        if (rowIndex == _selectedRowIndex && columnIndex == _selectedColumnIndex)
        {
            style = style.Merge(SelectedCellStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
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

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle).Merge(MutedStyle);
        }

        return style;
    }

    private static int ResolveColumnSeparatorWidth(string separator) =>
        string.IsNullOrEmpty(separator)
            ? 0
            : ControlTextLayout.MeasureDisplayWidth(separator);

    private int ResolveColumnSeparatorWidth() => ResolveColumnSeparatorWidth(ResolveColumnSeparatorText());

    private string ResolveColumnSeparatorText() => ColumnSeparatorText;

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

    private static void WritePaddedCell(
        Canvas canvas,
        int x,
        int y,
        string value,
        int width,
        TeaStyle style,
        int maxWidth)
    {
        var effectiveWidth = Math.Max(0, Math.Min(width, maxWidth));
        if (effectiveWidth <= 0)
        {
            return;
        }

        if (style.IsEmpty)
        {
            canvas.WriteTextPadded(x, y, value, effectiveWidth);
            return;
        }

        var text = PadToWidth(value, effectiveWidth);
        canvas.WriteText(x, y, ApplyStyle(text, style), maxWidth);
    }
}
