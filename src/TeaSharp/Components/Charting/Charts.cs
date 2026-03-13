using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
namespace TeaSharp.Components.Charting;

internal static class Charts
{
    public static void DrawLineChart(
        Canvas canvas,
        Rect rect,
        IReadOnlyList<double> samples,
        string title = "Line Chart",
        double? minValue = null,
        double? maxValue = null,
        LineChartOptions? options = null)
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

        var chartOptions = options ?? new LineChartOptions();
        var plot = content;
        if (chartOptions.ShowAxes && content.Width >= 6 && content.Height >= 4)
        {
            canvas.DrawVerticalLine(content.X, content.Y, content.Height, '│');
            canvas.DrawHorizontalLine(content.X, content.Bottom - 1, content.Width, '─');
            canvas.Set(content.X, content.Bottom - 1, '└');
            plot = new Rect(content.X + 1, content.Y, content.Width - 1, content.Height - 1);
        }

        if (plot.IsEmpty)
        {
            return;
        }

        var zoom = double.IsFinite(chartOptions.Zoom)
            ? Math.Clamp(chartOptions.Zoom, 0.1, 8.0)
            : 1.0;
        var count = Math.Clamp((int)Math.Round(plot.Width / zoom, MidpointRounding.AwayFromZero), 1, samples.Count);
        var maxOffset = Math.Max(0, samples.Count - count);
        var offset = Math.Clamp(chartOptions.Offset, 0, maxOffset);
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

        if (!chartOptions.ShowAxes)
        {
            canvas.DrawHorizontalLine(plot.X, plot.Bottom - 1, plot.Width, '·');
        }

        var prevX = -1;
        var prevY = -1;
        for (var i = 0; i < count; i++)
        {
            var value = samples[offset + i];
            var normalized = Math.Clamp((value - min) / (max - min), 0, 1);
            var y = plot.Bottom - 1 - (int)Math.Round(normalized * (plot.Height - 1), MidpointRounding.AwayFromZero);
            var x = count <= 1
                ? plot.X
                : plot.X + (int)Math.Round(
                    i * (plot.Width - 1) / (double)(count - 1),
                    MidpointRounding.AwayFromZero);

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
        if (!string.IsNullOrWhiteSpace(chartOptions.Legend))
        {
            var legend = chartOptions.Legend.Trim();
            var legendX = Math.Max(content.X, content.Right - legend.Length);
            canvas.WriteText(legendX, content.Y, legend, content.Right - legendX);
        }

        if (chartOptions.ShowAxes && !string.IsNullOrWhiteSpace(chartOptions.XLabel))
        {
            var xLabel = chartOptions.XLabel.Trim();
            var xLabelX = Math.Max(content.X, content.Right - xLabel.Length);
            canvas.WriteText(xLabelX, content.Bottom - 1, xLabel, content.Right - xLabelX);
        }

        if (chartOptions.ShowAxes && !string.IsNullOrWhiteSpace(chartOptions.YLabel))
        {
            canvas.WriteText(content.X, content.Y, chartOptions.YLabel.Trim(), Math.Min(content.Width, 4));
        }
    }

    public static void DrawBarChart(
        Canvas canvas,
        Rect rect,
        IReadOnlyList<BarDatum> bars,
        string title = "Bar Chart",
        double? maxValue = null,
        BarChartOptions? options = null)
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

        var chartOptions = options ?? new BarChartOptions();
        if (chartOptions.ShowScale && content.Height > 0)
        {
            var scale = $"0..{FormatStat(max)}";
            var scaleX = Math.Max(content.X, content.Right - scale.Length);
            canvas.WriteText(scaleX, content.Bottom - 1, scale, content.Right - scaleX);
        }

        if (!string.IsNullOrWhiteSpace(chartOptions.Legend))
        {
            var legend = chartOptions.Legend.Trim();
            var legendX = Math.Max(content.X, content.Right - legend.Length);
            canvas.WriteText(legendX, content.Y, legend, content.Right - legendX);
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
