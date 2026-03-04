namespace TeaSharp.Components;

public static class Widgets
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
        var lines = content
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');
        DrawPanel(canvas, rect, title, lines);
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
}
