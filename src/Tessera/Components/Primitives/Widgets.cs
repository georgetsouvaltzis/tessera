namespace Tessera.Components.Primitives;

internal static class Widgets
{
    private static ReadOnlySpan<char> SparklineSteps => "▁▂▃▄▅▆▇█";

    public static void DrawPanel(Canvas canvas, Rect rect, string title, IReadOnlyList<string> lines)
    {
        canvas.DrawBox(rect, title);
        var contentRect = rect.Inset(1, 1);
        var maxRows = Math.Max(0, contentRect.Height);
        for (var i = 0; i < lines.Count && i < maxRows; i++)
        {
            canvas.WriteText(contentRect.X, contentRect.Y + i, lines[i], contentRect.Width);
        }
    }

    public static void DrawPanel(Canvas canvas, Rect rect, string title, string content)
    {
        canvas.DrawBox(rect, title);
        var contentRect = rect.Inset(1, 1);
        if (contentRect.IsEmpty)
        {
            return;
        }

        var row = 0;
        var value = content ?? string.Empty;
        var start = 0;
        for (var index = 0; index < value.Length && row < contentRect.Height; index++)
        {
            var current = value[index];
            if (current is not ('\n' or '\r'))
            {
                continue;
            }

            canvas.WriteText(contentRect.X, contentRect.Y + row, value[start..index], contentRect.Width);
            row++;
            if (current == '\r' && index + 1 < value.Length && value[index + 1] == '\n')
            {
                index++;
            }

            start = index + 1;
        }

        if (row < contentRect.Height)
        {
            canvas.WriteText(contentRect.X, contentRect.Y + row, value[start..], contentRect.Width);
        }
    }

    public static void DrawProgressBar(Canvas canvas, Rect rect, double value, string? label = null)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1 || clipped.Width < 3)
        {
            return;
        }

        var fraction = Math.Clamp(value, 0.0, 1.0);
        var innerWidth = clipped.Width - 2;
        var filled = (int)Math.Round(innerWidth * fraction, MidpointRounding.AwayFromZero);
        filled = Math.Clamp(filled, 0, innerWidth);

        canvas.Set(clipped.X, clipped.Y, '[');
        canvas.Set(clipped.Right - 1, clipped.Y, ']');
        for (var i = 0; i < innerWidth; i++)
        {
            canvas.Set(clipped.X + 1 + i, clipped.Y, i < filled ? '█' : '░');
        }

        if (!string.IsNullOrWhiteSpace(label) && clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, label, clipped.Width);
        }
    }

    public static void DrawSparkline(Canvas canvas, Rect rect, IReadOnlyList<int> values, int minValue = 0, int maxValue = 100)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width <= 0 || clipped.Height <= 0 || values.Count == 0 || minValue >= maxValue)
        {
            return;
        }

        var count = Math.Min(values.Count, clipped.Width);
        var offset = Math.Max(0, values.Count - count);
        var steps = SparklineSteps;
        for (var i = 0; i < count; i++)
        {
            var value = values[offset + i];
            var normalized = (double)(value - minValue) / (maxValue - minValue);
            normalized = Math.Clamp(normalized, 0.0, 1.0);
            var stepIndex = (int)Math.Round(normalized * (steps.Length - 1), MidpointRounding.AwayFromZero);
            stepIndex = Math.Clamp(stepIndex, 0, steps.Length - 1);
            canvas.Set(clipped.X + i, clipped.Y, steps[stepIndex]);
        }
    }

    public static void DrawList(Canvas canvas, Rect rect, IReadOnlyList<string> items, int selectedIndex)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(clipped.Height, items.Count);
        for (var row = 0; row < rows; row++)
        {
            var isSelected = row == selectedIndex;
            var prefix = isSelected ? "› " : "  ";
            canvas.WriteText(clipped.X, clipped.Y + row, prefix + items[row], clipped.Width);
        }
    }

    public static void DrawCard(Canvas canvas, Rect rect, string title, IReadOnlyList<string> lines, char accent = '▌')
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, title);
        var contentRect = clipped.Inset(1, 1);
        var maxRows = Math.Min(lines.Count, contentRect.Height);
        for (var row = 0; row < maxRows; row++)
        {
            var y = contentRect.Y + row;
            canvas.Set(contentRect.X, y, accent);
            if (contentRect.Width > 2)
            {
                canvas.WriteText(contentRect.X + 2, y, lines[row], contentRect.Width - 2);
            }
        }
    }

    public static void DrawTable(
        Canvas canvas,
        Rect rect,
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<string>> rows,
        int selectedRow = -1,
        string? title = null,
        BorderStyle border = BorderStyle.SingleLine,
        Thickness padding = default)
        => DrawTable(
            canvas,
            rect,
            headers,
            rows,
            selectedRow,
            title,
            border,
            padding,
            borderStyleText: default);

    public static void DrawTable(
        Canvas canvas,
        Rect rect,
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<string>> rows,
        int selectedRow,
        string? title,
        BorderStyle border,
        Thickness padding,
        global::Tessera.Styles.TesseraStyle borderStyleText)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || headers.Count == 0)
        {
            return;
        }

        var showBorder = border != BorderStyle.None;
        var minHeight = showBorder
            ? 4
            : string.IsNullOrWhiteSpace(title) ? 3 : 4;
        var minWidth = showBorder
            ? (headers.Count * 2) + 1
            : (headers.Count * 2) - 1;
        if (clipped.Width < minWidth || clipped.Height < minHeight)
        {
            return;
        }

        Rect contentRect;
        if (showBorder)
        {
            canvas.DrawBox(clipped, title, border, borderStyleText);
            contentRect = clipped.Inset(1, 1).Inset(padding);
        }
        else
        {
            contentRect = clipped.Inset(padding);
            if (!string.IsNullOrWhiteSpace(title))
            {
                canvas.WriteText(contentRect.X, contentRect.Y, title!, contentRect.Width);
                contentRect = new Rect(contentRect.X, contentRect.Y + 1, contentRect.Width, contentRect.Height - 1);
            }
        }

        if (contentRect.Height < 3)
        {
            return;
        }

        var separatorCount = headers.Count - 1;
        var availableWidth = Math.Max(headers.Count, contentRect.Width - separatorCount);
        var widths = ComputeColumnWidths(availableWidth, headers.Count);

        DrawTableRow(canvas, contentRect.X, contentRect.Y, widths, headers, isSelected: false);

        var dividerY = contentRect.Y + 1;
        canvas.DrawHorizontalLine(contentRect.X, dividerY, contentRect.Width, '─');
        var separatorX = contentRect.X;
        for (var i = 0; i < widths.Length - 1; i++)
        {
            separatorX += widths[i];
            canvas.Set(separatorX, dividerY, '┼');
            separatorX++;
        }

        var maxRows = Math.Min(rows.Count, Math.Max(0, contentRect.Height - 2));
        for (var i = 0; i < maxRows; i++)
        {
            var y = contentRect.Y + 2 + i;
            DrawTableRow(canvas, contentRect.X, y, widths, rows[i], isSelected: i == selectedRow);
        }
    }

    private static int[] ComputeColumnWidths(int width, int columns)
    {
        var widths = new int[columns];
        var baseWidth = width / columns;
        var remainder = width % columns;
        for (var i = 0; i < columns; i++)
        {
            widths[i] = baseWidth + (i < remainder ? 1 : 0);
        }

        return widths;
    }

    private static void DrawTableRow(
        Canvas canvas,
        int x,
        int y,
        int[] widths,
        IReadOnlyList<string> cells,
        bool isSelected)
    {
        var cx = x;
        for (var col = 0; col < widths.Length; col++)
        {
            var width = widths[col];
            var value = col < cells.Count ? cells[col] : string.Empty;
            if (isSelected && col == 0 && width >= 2)
            {
                value = "› " + value;
            }

            canvas.WriteText(cx, y, FitText(value, width), width);
            cx += width;
            if (col < widths.Length - 1)
            {
                canvas.Set(cx, y, '│');
                cx++;
            }
        }
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
}
