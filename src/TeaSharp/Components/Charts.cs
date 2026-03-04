namespace TeaSharp.Components;

public readonly record struct BarDatum(string Label, double Value);

public static class Charts
{
    public static void DrawLineChart(
        Canvas canvas,
        Rect rect,
        IReadOnlyList<double> samples,
        string title = "Line Chart",
        double? minValue = null,
        double? maxValue = null)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        canvas.DrawBox(clipped, title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty || samples.Count == 0)
        {
            return;
        }

        var count = Math.Min(content.Width, samples.Count);
        var offset = Math.Max(0, samples.Count - count);
        var min = minValue ?? double.PositiveInfinity;
        var max = maxValue ?? double.NegativeInfinity;
        if (!minValue.HasValue || !maxValue.HasValue)
        {
            for (var i = 0; i < count; i++)
            {
                var value = samples[offset + i];
                if (!minValue.HasValue && value < min)
                {
                    min = value;
                }

                if (!maxValue.HasValue && value > max)
                {
                    max = value;
                }
            }
        }

        if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }

        // baseline
        canvas.DrawHorizontalLine(content.X, content.Bottom - 1, content.Width, '·');

        var prevX = -1;
        var prevY = -1;
        for (var i = 0; i < count; i++)
        {
            var value = samples[offset + i];
            var normalized = Math.Clamp((value - min) / (max - min), 0, 1);
            var y = content.Bottom - 1 - (int)Math.Round(normalized * (content.Height - 1), MidpointRounding.AwayFromZero);
            var x = content.X + i;

            if (prevX >= 0)
            {
                DrawConnection(canvas, prevX, prevY, x, y);
            }

            canvas.Set(x, y, '●');
            prevX = x;
            prevY = y;
        }

        var stats = $"min:{FormatStat(min)} max:{FormatStat(max)}";
        canvas.WriteText(content.X, content.Y, stats, content.Width);
    }

    public static void DrawBarChart(
        Canvas canvas,
        Rect rect,
        IReadOnlyList<BarDatum> bars,
        string title = "Bar Chart",
        double? maxValue = null)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 6 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty || bars.Count == 0)
        {
            return;
        }

        var rows = Math.Min(content.Height, bars.Count);
        var max = maxValue ?? 0;
        if (!maxValue.HasValue)
        {
            for (var i = 0; i < rows; i++)
            {
                if (bars[i].Value > max)
                {
                    max = bars[i].Value;
                }
            }
        }

        if (max <= 0)
        {
            max = 1;
        }

        var widestLabel = 0;
        for (var i = 0; i < rows; i++)
        {
            if (bars[i].Label.Length > widestLabel)
            {
                widestLabel = bars[i].Label.Length;
            }
        }

        var labelWidth = Math.Clamp(widestLabel, 3, 12);
        var barWidth = Math.Max(1, content.Width - labelWidth - 2);

        for (var row = 0; row < rows; row++)
        {
            var bar = bars[row];
            var y = content.Y + row;
            var normalized = Math.Clamp(bar.Value / max, 0, 1);
            var filled = (int)Math.Round(barWidth * normalized, MidpointRounding.AwayFromZero);
            filled = Math.Clamp(filled, 0, barWidth);

            var label = bar.Label.Length > labelWidth
                ? bar.Label[..labelWidth]
                : bar.Label.PadRight(labelWidth);

            canvas.WriteText(content.X, y, label, labelWidth);
            canvas.Set(content.X + labelWidth, y, ' ');
            for (var i = 0; i < barWidth; i++)
            {
                canvas.Set(content.X + labelWidth + 1 + i, y, i < filled ? '█' : '░');
            }
        }
    }

    private static void DrawConnection(Canvas canvas, int x0, int y0, int x1, int y1)
    {
        if (x0 == x1)
        {
            canvas.Set(x0, y0, '│');
            return;
        }

        var dx = x1 - x0;
        var dy = y1 - y0;
        var step = Math.Abs(dy) <= 1 ? '─' : dy > 0 ? '╲' : '╱';
        for (var x = Math.Min(x0, x1) + 1; x < Math.Max(x0, x1); x++)
        {
            canvas.Set(x, y0 + ((x - x0) * dy / dx), step);
        }
    }

    private static string FormatStat(double value)
    {
        return Math.Abs(value) >= 100
            ? value.ToString("0", System.Globalization.CultureInfo.InvariantCulture)
            : value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
    }
}

public sealed class LineChartComponent : ICanvasComponent
{
    private readonly List<double> _samples = [];

    public LineChartComponent(int capacity = 240)
    {
        Capacity = Math.Max(1, capacity);
    }

    public int Capacity { get; }

    public string Title { get; set; } = "Line Chart";

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public IReadOnlyList<double> Samples => _samples;

    public void SetSamples(IEnumerable<double> samples)
    {
        _samples.Clear();
        foreach (var sample in samples)
        {
            Append(sample);
        }
    }

    public void Append(double sample)
    {
        _samples.Add(sample);
        if (_samples.Count > Capacity)
        {
            _samples.RemoveRange(0, _samples.Count - Capacity);
        }
    }

    public void Render(Canvas canvas, Rect rect)
    {
        Charts.DrawLineChart(canvas, rect, _samples, Title, MinValue, MaxValue);
    }
}

public sealed class BarChartComponent : ICanvasComponent
{
    private readonly List<BarDatum> _bars = [];

    public string Title { get; set; } = "Bar Chart";

    public double? MaxValue { get; set; }

    public IReadOnlyList<BarDatum> Bars => _bars;

    public void SetBars(IEnumerable<BarDatum> bars)
    {
        _bars.Clear();
        _bars.AddRange(bars);
    }

    public void SetValue(string label, double value)
    {
        for (var i = 0; i < _bars.Count; i++)
        {
            if (string.Equals(_bars[i].Label, label, StringComparison.Ordinal))
            {
                _bars[i] = _bars[i] with { Value = value };
                return;
            }
        }

        _bars.Add(new BarDatum(label, value));
    }

    public void Render(Canvas canvas, Rect rect)
    {
        Charts.DrawBarChart(canvas, rect, _bars, Title, MaxValue);
    }
}
