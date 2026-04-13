using System.ComponentModel;
using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a histogram control for bucketed telemetry distributions.
/// </summary>
public sealed class Histogram : Control
{
    private readonly List<HistogramBucket> _buckets = [];

    /// <summary>
    ///     Gets or sets the chart title.
    /// </summary>
    public string Title { get; set; } = "Histogram";

    /// <summary>
    ///     Gets or sets marker shown in the title while focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether focused title marker text is rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets style used for title text when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for title text when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for rendered bars.
    /// </summary>
    public TesseraStyle BarStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for axis lines and axis labels.
    /// </summary>
    public TesseraStyle AxisStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for bucket labels.
    /// </summary>
    public TesseraStyle LabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for legend text.
    /// </summary>
    public TesseraStyle LegendStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets optional explicit maximum value used for scaling bars.
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    ///     Gets or sets advanced histogram rendering options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public HistogramOptions? Options { get; set; }

    /// <summary>
    ///     Gets current buckets.
    /// </summary>
    public IReadOnlyList<HistogramBucket> Buckets => _buckets;

    /// <summary>
    ///     Replaces current buckets.
    /// </summary>
    /// <param name="buckets">The buckets to render.</param>
    public void SetBuckets(IEnumerable<HistogramBucket> buckets)
    {
        ArgumentNullException.ThrowIfNull(buckets);
        _buckets.Clear();
        foreach (var bucket in buckets)
        {
            _buckets.Add(new HistogramBucket(bucket.Label, bucket.Value));
        }
    }

    /// <summary>
    ///     Sets or updates one bucket by label.
    /// </summary>
    /// <param name="label">The bucket label.</param>
    /// <param name="value">The bucket value.</param>
    public void SetValue(string label, double value)
    {
        var normalizedLabel = label;
        for (var index = 0; index < _buckets.Count; index++)
        {
            if (string.Equals(_buckets[index].Label, normalizedLabel, StringComparison.Ordinal))
            {
                _buckets[index] = new HistogramBucket(normalizedLabel, value);
                return;
            }
        }

        _buckets.Add(new HistogramBucket(normalizedLabel, value));
    }

    /// <summary>
    ///     Clears all buckets.
    /// </summary>
    public void Clear()
    {
        _buckets.Clear();
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 6 || clipped.Height < 4)
        {
            return;
        }

        canvas.DrawBox(clipped, RenderTitle());
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        if (_buckets.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(empty)", LabelStyle), content.Width);
            return;
        }

        var options = Options ?? new HistogramOptions();
        var labelY = -1;
        var plot = ResolvePlotRect(canvas, content, options, ref labelY);
        if (plot.IsEmpty)
        {
            return;
        }

        var max = ResolveMaxValue();
        var barGlyph = options.BarGlyph;
        var styledGlyph = ApplyStyle(barGlyph.ToString(), BarStyle);
        var drawStyled = !BarStyle.IsEmpty;

        for (var index = 0; index < _buckets.Count; index++)
        {
            var x = MapBucketToX(index, _buckets.Count, plot.X, plot.Width);
            var normalized = Math.Clamp(_buckets[index].Value / max, 0, 1);
            var height = Math.Clamp((int)Math.Round(normalized * plot.Height, MidpointRounding.AwayFromZero), 0,
                plot.Height);
            for (var offset = 0; offset < height; offset++)
            {
                var y = plot.Bottom - 1 - offset;
                if (drawStyled)
                {
                    canvas.WriteText(x, y, styledGlyph, 1);
                }
                else
                {
                    canvas.Set(x, y, barGlyph);
                }
            }

            if (options.ShowBucketLabels && labelY >= content.Y && labelY < content.Bottom)
            {
                var label = ResolveBucketLabel(_buckets[index].Label);
                if (LabelStyle.IsEmpty)
                {
                    canvas.Set(x, labelY, label);
                }
                else
                {
                    canvas.WriteText(x, labelY, ApplyStyle(label.ToString(), LabelStyle), 1);
                }
            }
        }

        RenderMeta(canvas, content, options, max, labelY);
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 8);
        width = Math.Max(width, _buckets.Count * 2 + 4);
        var height = 8;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private Rect ResolvePlotRect(Canvas canvas, Rect content, HistogramOptions options, ref int labelY)
    {
        if (options.ShowAxes && content.Width >= 4 && content.Height >= 3)
        {
            var axisY = content.Bottom - 2;
            DrawVerticalAxis(content.X, content.Y, axisY - content.Y + 1);
            DrawHorizontalAxis(content.X, axisY, content.Width);
            DrawAxisIntersection(content.X, axisY);
            labelY = axisY + 1;
            return new Rect(content.X + 1, content.Y, content.Width - 1, Math.Max(1, axisY - content.Y));
        }

        if (options.ShowBucketLabels && content.Height > 1)
        {
            labelY = content.Bottom - 1;
            return new Rect(content.X, content.Y, content.Width, content.Height - 1);
        }

        return content;

        void DrawVerticalAxis(int x, int y, int height)
        {
            if (AxisStyle.IsEmpty)
            {
                canvas.DrawVerticalLine(x, y, height);
                return;
            }

            var glyph = ApplyStyle("│", AxisStyle);
            for (var index = 0; index < height; index++)
            {
                canvas.WriteText(x, y + index, glyph, 1);
            }
        }

        void DrawHorizontalAxis(int x, int y, int width)
        {
            if (AxisStyle.IsEmpty)
            {
                canvas.DrawHorizontalLine(x, y, width);
                return;
            }

            var glyph = ApplyStyle("─", AxisStyle);
            for (var index = 0; index < width; index++)
            {
                canvas.WriteText(x + index, y, glyph, 1);
            }
        }

        void DrawAxisIntersection(int x, int y)
        {
            if (AxisStyle.IsEmpty)
            {
                canvas.Set(x, y, '└');
                return;
            }

            canvas.WriteText(x, y, ApplyStyle("└", AxisStyle), 1);
        }
    }

    private double ResolveMaxValue()
    {
        var max = MaxValue ?? 0;
        if (!MaxValue.HasValue)
        {
            for (var index = 0; index < _buckets.Count; index++)
            {
                if (_buckets[index].Value > max)
                {
                    max = _buckets[index].Value;
                }
            }
        }

        return max <= 0 ? 1 : max;
    }

    private void RenderMeta(Canvas canvas, Rect content, HistogramOptions options, double max, int labelY)
    {
        if (!string.IsNullOrWhiteSpace(options.Legend))
        {
            var legend = ApplyStyle(options.Legend.Trim(), LegendStyle);
            var x = Math.Max(content.X, content.Right - legend.Length);
            canvas.WriteText(x, content.Y, legend, content.Right - x);
        }

        if (options.ShowScale)
        {
            var scaleText = ApplyStyle($"max:{FormatStat(max)}", AxisStyle);
            canvas.WriteText(content.X, content.Y, scaleText, content.Width);
        }

        if (options.ShowAxes && !string.IsNullOrWhiteSpace(options.XLabel))
        {
            var xLabel = ApplyStyle(options.XLabel.Trim(), AxisStyle);
            var y = labelY >= content.Y && labelY < content.Bottom ? labelY : content.Bottom - 1;
            var x = Math.Max(content.X, content.Right - xLabel.Length);
            canvas.WriteText(x, y, xLabel, content.Right - x);
        }

        if (options.ShowAxes && !string.IsNullOrWhiteSpace(options.YLabel))
        {
            var yLabel = ApplyStyle(options.YLabel.Trim(), AxisStyle);
            canvas.WriteText(content.X, content.Y, yLabel, Math.Min(4, content.Width));
        }
    }

    private static int MapBucketToX(int index, int count, int origin, int width)
    {
        if (width <= 1 || count <= 1)
        {
            return origin;
        }

        return origin + (int)Math.Round(index * (width - 1) / (double)(count - 1), MidpointRounding.AwayFromZero);
    }

    private static char ResolveBucketLabel(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            return ' ';
        }

        return label[0];
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
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private static string FormatStat(double value)
    {
        return Math.Abs(value) >= 100
            ? value.ToString("0", CultureInfo.InvariantCulture)
            : value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
