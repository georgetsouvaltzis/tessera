using System.ComponentModel;
using System.Globalization;
using System.Text;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a multi-series line plot control.
/// </summary>
public sealed class LinePlot : Control
{
    private const string EmptySeriesText = "(no series)";
    private readonly List<LineSeries> _series = [];

    /// <summary>
    /// Gets or sets the plot title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Line Plot";

    /// <summary>
    /// Gets or sets the marker appended to the title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets style used for the title when the control is not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for the title when the control is focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for stats text.
    /// </summary>
    public TeaStyle StatsStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for legend text.
    /// </summary>
    public TeaStyle LegendStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for axis labels and axis glyphs.
    /// </summary>
    public TeaStyle AxisStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for optional grid lines.
    /// </summary>
    public TeaStyle GridStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for empty-state text.
    /// </summary>
    public TeaStyle EmptyTextStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rendered output while <see cref="Control.IsDisabled"/> is <see langword="true"/>.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding applied to plot content.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets optional explicit minimum value used for normalization.
    /// </summary>
    public double? MinValue { get; set; }

    /// <summary>
    /// Gets or sets optional explicit maximum value used for normalization.
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    /// Gets or sets text shown when there are no plot series or sample points.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = EmptySeriesText;

    /// <summary>
    /// Gets or sets advanced line-plot rendering options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public LinePlotOptions? Options { get; set; }

    /// <summary>
    /// Gets the current line series collection.
    /// </summary>
    public IReadOnlyList<LineSeries> Series => _series;

    /// <summary>
    /// Replaces the current series set.
    /// </summary>
    /// <param name="series">The series to render.</param>
    public void SetSeries(IEnumerable<LineSeries> series)
    {
        ArgumentNullException.ThrowIfNull(series);

        _series.Clear();
        foreach (var item in series)
        {
            if (item is not null)
            {
                _series.Add(item);
            }
        }
    }

    /// <summary>
    /// Adds one series to the plot.
    /// </summary>
    /// <param name="series">The series to add.</param>
    public void AddSeries(LineSeries series)
    {
        ArgumentNullException.ThrowIfNull(series);
        _series.Add(series);
    }

    /// <summary>
    /// Removes series by exact name match.
    /// </summary>
    /// <param name="name">The series name.</param>
    /// <returns><see langword="true"/> if a series was removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveSeries(string name)
    {
        var normalized = name ?? string.Empty;
        for (var i = 0; i < _series.Count; i++)
        {
            if (string.Equals(_series[i].Name, normalized, StringComparison.Ordinal))
            {
                _series.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Appends one sample to the named series.
    /// </summary>
    /// <param name="seriesName">The target series name.</param>
    /// <param name="sample">The sample value.</param>
    /// <returns><see langword="true"/> when the series exists; otherwise, <see langword="false"/>.</returns>
    public bool AppendSample(string seriesName, double sample)
    {
        var normalized = seriesName ?? string.Empty;
        for (var i = 0; i < _series.Count; i++)
        {
            if (!string.Equals(_series[i].Name, normalized, StringComparison.Ordinal))
            {
                continue;
            }

            _series[i].Append(sample);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes all series and sample values.
    /// </summary>
    public void Clear()
    {
        _series.Clear();
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = Border == BorderStyle.None
            ? clipped.Inset(Padding)
            : FrameLayout.DrawFrameAndResolveContent(
                canvas,
                clipped,
                RenderTitle(),
                Border,
                Padding,
                ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        var options = Options ?? new LinePlotOptions();
        var maxSampleCount = GetMaxSampleCount();
        if (maxSampleCount == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveStyled(EmptyTextStyle)), content.Width);
            return;
        }

        var showStatsRow = options.ShowStats && content.Height >= 3;
        var showFooterRow = (options.ShowLegend || (options.ShowAxes && !string.IsNullOrWhiteSpace(options.XLabel)))
            && content.Height >= (showStatsRow ? 4 : 3);

        var plotTop = content.Y;
        var plotBottom = content.Bottom;
        if (showStatsRow)
        {
            plotTop++;
        }

        if (showFooterRow)
        {
            plotBottom--;
        }

        var plot = new Rect(content.X, plotTop, content.Width, plotBottom - plotTop);
        if (plot.IsEmpty)
        {
            return;
        }

        var zoom = double.IsFinite(options.Zoom)
            ? Math.Clamp(options.Zoom, 0.1, 8.0)
            : 1.0;
        var visibleCount = Math.Clamp((int)Math.Round(plot.Width / zoom, MidpointRounding.AwayFromZero), 1, maxSampleCount);
        var maxOffset = Math.Max(0, maxSampleCount - visibleCount);
        var offset = Math.Clamp(options.Offset, 0, maxOffset);
        if (!TryResolveVisibleRange(maxSampleCount, visibleCount, offset, out var min, out var max))
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveStyled(EmptyTextStyle)), content.Width);
            return;
        }

        var plotArea = plot;
        if (options.ShowAxes && plot.Width >= 3 && plot.Height >= 3)
        {
            var axisStyle = ResolveStyled(AxisStyle);
            DrawVerticalLine(canvas, plot.X, plot.Y, plot.Height, axisStyle, '│');
            DrawHorizontalLine(canvas, plot.X, plot.Bottom - 1, plot.Width, axisStyle, '─');
            WriteStyledGlyph(canvas, plot.X, plot.Bottom - 1, '└', axisStyle);
            if (!string.IsNullOrWhiteSpace(options.YLabel))
            {
                var yLabel = options.YLabel.Trim();
                canvas.WriteText(plot.X + 1, plot.Y, ApplyStyle(yLabel, axisStyle), Math.Max(0, plot.Width - 1));
            }

            plotArea = new Rect(plot.X + 1, plot.Y, plot.Width - 1, plot.Height - 1);
        }

        if (plotArea.IsEmpty)
        {
            return;
        }

        if (options.ShowGrid && plotArea.Width >= 3 && plotArea.Height >= 2)
        {
            DrawGrid(canvas, plotArea, ResolveStyled(GridStyle));
        }

        RenderSeries(canvas, plotArea, maxSampleCount, visibleCount, offset, min, max);

        if (showStatsRow)
        {
            var stats = $"min:{FormatStat(min)} max:{FormatStat(max)}";
            canvas.WriteText(content.X, content.Y, ApplyStyle(stats, ResolveStyled(StatsStyle)), content.Width);
        }

        if (showFooterRow)
        {
            var footerY = content.Bottom - 1;
            var rightReserved = 0;
            if (options.ShowAxes && !string.IsNullOrWhiteSpace(options.XLabel))
            {
                var xLabel = options.XLabel.Trim();
                rightReserved = Math.Min(content.Width, xLabel.Length + 1);
                var xLabelX = Math.Max(content.X, content.Right - xLabel.Length);
                canvas.WriteText(xLabelX, footerY, ApplyStyle(xLabel, ResolveStyled(AxisStyle)), content.Right - xLabelX);
            }

            if (options.ShowLegend && content.Width - rightReserved > 0)
            {
                RenderLegendRow(canvas, footerY, content.X, content.Width - rightReserved);
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 6);
        var options = Options ?? new LinePlotOptions();
        if (options.ShowLegend)
        {
            width = Math.Max(width, Math.Min(72, EstimateLegendWidth() + 2));
        }

        var height = 8;
        if (options.ShowStats)
        {
            height++;
        }

        if (options.ShowLegend || (options.ShowAxes && !string.IsNullOrWhiteSpace(options.XLabel)))
        {
            height++;
        }

        width += Padding.Horizontal;
        height += Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderSeries(Canvas canvas, Rect plotArea, int maxSampleCount, int visibleCount, int offset, double min, double max)
    {
        var range = max - min;
        if (Math.Abs(range) < double.Epsilon)
        {
            range = 1d;
        }

        for (var seriesIndex = 0; seriesIndex < _series.Count; seriesIndex++)
        {
            var series = _series[seriesIndex];
            var style = ResolveStyled(series.Style);
            var pointGlyph = RenderGlyph(series.PointGlyph, style);
            var horizontalGlyph = RenderGlyph('─', style);
            var upwardGlyph = RenderGlyph('╱', style);
            var downwardGlyph = RenderGlyph('╲', style);
            var verticalGlyph = RenderGlyph('│', style);
            var previousX = -1;
            var previousY = -1;
            for (var index = 0; index < visibleCount; index++)
            {
                var globalIndex = offset + index;
                if (!TryGetSeriesValue(series, maxSampleCount, globalIndex, out var value))
                {
                    previousX = -1;
                    previousY = -1;
                    continue;
                }

                var normalized = Math.Clamp((value - min) / range, 0d, 1d);
                var y = plotArea.Bottom - 1 - (int)Math.Round(normalized * (plotArea.Height - 1), MidpointRounding.AwayFromZero);
                var x = visibleCount <= 1
                    ? plotArea.X
                    : plotArea.X + (int)Math.Round(
                        index * (plotArea.Width - 1) / (double)(visibleCount - 1),
                        MidpointRounding.AwayFromZero);

                if (previousX >= 0)
                {
                    DrawConnection(canvas, previousX, previousY, x, y, horizontalGlyph, upwardGlyph, downwardGlyph, verticalGlyph);
                }

                WriteGlyph(canvas, x, y, series.PointGlyph, pointGlyph);
                previousX = x;
                previousY = y;
            }
        }
    }

    private void RenderLegendRow(Canvas canvas, int y, int x, int width)
    {
        if (width <= 0)
        {
            return;
        }

        var cursor = x;
        var maxX = x + width;
        for (var i = 0; i < _series.Count && cursor < maxX; i++)
        {
            var series = _series[i];
            var name = string.IsNullOrWhiteSpace(series.Name) ? $"S{i + 1}" : series.Name.Trim();
            var segment = $"{series.PointGlyph} {name}";
            var style = ResolveStyled(LegendStyle.Merge(series.Style));
            var remaining = maxX - cursor;
            canvas.WriteText(cursor, y, ApplyStyle(segment, style), remaining);
            cursor += segment.Length + 1;
        }
    }

    private bool TryResolveVisibleRange(int maxSampleCount, int visibleCount, int offset, out double min, out double max)
    {
        min = MinValue ?? double.PositiveInfinity;
        max = MaxValue ?? double.NegativeInfinity;
        var hasData = false;

        if (!MinValue.HasValue || !MaxValue.HasValue)
        {
            for (var i = 0; i < visibleCount; i++)
            {
                var globalIndex = offset + i;
                for (var seriesIndex = 0; seriesIndex < _series.Count; seriesIndex++)
                {
                    if (!TryGetSeriesValue(_series[seriesIndex], maxSampleCount, globalIndex, out var value))
                    {
                        continue;
                    }

                    hasData = true;
                    if (!MinValue.HasValue && value < min)
                    {
                        min = value;
                    }

                    if (!MaxValue.HasValue && value > max)
                    {
                        max = value;
                    }
                }
            }
        }
        else
        {
            hasData = true;
        }

        if (!double.IsFinite(min))
        {
            min = 0;
        }

        if (!double.IsFinite(max))
        {
            max = min + 1;
        }

        if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }

        return hasData;
    }

    private int GetMaxSampleCount()
    {
        var max = 0;
        for (var i = 0; i < _series.Count; i++)
        {
            max = Math.Max(max, _series[i].Samples.Count);
        }

        return max;
    }

    private int EstimateLegendWidth()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < _series.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(' ');
            }

            var name = string.IsNullOrWhiteSpace(_series[i].Name) ? $"S{i + 1}" : _series[i].Name.Trim();
            builder.Append(_series[i].PointGlyph);
            builder.Append(' ');
            builder.Append(name);
        }

        return ControlTextLayout.MeasureDisplayWidth(builder.ToString());
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? ResolveStyled(FocusedTitleStyle) : ResolveStyled(TitleStyle));
    }

    private string FormatTitleForMeasure()
    {
        if (!ShowFocusMarker || string.IsNullOrWhiteSpace(FocusMarker))
        {
            return Title;
        }

        return string.IsNullOrEmpty(Title) ? string.Empty : $"{Title} {FocusMarker}";
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

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return ResolveStyled(style);
    }

    private TeaStyle ResolveStyled(TeaStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private static bool TryGetSeriesValue(LineSeries series, int maxSampleCount, int globalIndex, out double value)
    {
        var start = maxSampleCount - series.Samples.Count;
        if (globalIndex < start || globalIndex >= start + series.Samples.Count)
        {
            value = default;
            return false;
        }

        value = series.Samples[globalIndex - start];
        return true;
    }

    private static void DrawGrid(Canvas canvas, Rect area, TeaStyle style)
    {
        var horizontalStep = Math.Max(1, area.Height / 4);
        var verticalStep = Math.Max(2, area.Width / 6);

        for (var y = area.Y; y < area.Bottom; y += horizontalStep)
        {
            DrawHorizontalLine(canvas, area.X, y, area.Width, style, '┈');
        }

        for (var x = area.X; x < area.Right; x += verticalStep)
        {
            DrawVerticalLine(canvas, x, area.Y, area.Height, style, '┊');
        }
    }

    private static void DrawConnection(
        Canvas canvas,
        int x0,
        int y0,
        int x1,
        int y1,
        string? horizontalGlyph,
        string? upwardGlyph,
        string? downwardGlyph,
        string? verticalGlyph)
    {
        if (x0 == x1)
        {
            WriteGlyph(canvas, x0, y0, '│', verticalGlyph);
            return;
        }

        var dx = x1 - x0;
        var dy = y1 - y0;
        var glyph = Math.Abs(dy) <= 1
            ? horizontalGlyph
            : dy > 0 ? downwardGlyph : upwardGlyph;
        var fallback = Math.Abs(dy) <= 1
            ? '─'
            : dy > 0 ? '╲' : '╱';

        for (var x = Math.Min(x0, x1) + 1; x < Math.Max(x0, x1); x++)
        {
            var y = y0 + ((x - x0) * dy / dx);
            WriteGlyph(canvas, x, y, fallback, glyph);
        }
    }

    private static void DrawHorizontalLine(Canvas canvas, int x, int y, int width, TeaStyle style, char glyph)
    {
        var styledGlyph = RenderGlyph(glyph, style);
        for (var index = 0; index < width; index++)
        {
            WriteGlyph(canvas, x + index, y, glyph, styledGlyph);
        }
    }

    private static void DrawVerticalLine(Canvas canvas, int x, int y, int height, TeaStyle style, char glyph)
    {
        var styledGlyph = RenderGlyph(glyph, style);
        for (var index = 0; index < height; index++)
        {
            WriteGlyph(canvas, x, y + index, glyph, styledGlyph);
        }
    }

    private static void WriteStyledGlyph(Canvas canvas, int x, int y, char fallbackGlyph, TeaStyle style)
    {
        WriteGlyph(canvas, x, y, fallbackGlyph, RenderGlyph(fallbackGlyph, style));
    }

    private static void WriteGlyph(Canvas canvas, int x, int y, char fallbackGlyph, string? styledGlyph)
    {
        if (string.IsNullOrEmpty(styledGlyph))
        {
            canvas.Set(x, y, fallbackGlyph);
            return;
        }

        canvas.WriteText(x, y, styledGlyph, 1);
    }

    private static string? RenderGlyph(char glyph, TeaStyle style)
    {
        return style.IsEmpty ? null : style.Render(glyph.ToString());
    }

    private static string ApplyStyle(string value, TeaStyle style)
    {
        if (string.IsNullOrEmpty(value) || style.IsEmpty)
        {
            return value;
        }

        return style.Render(value);
    }

    private static string FormatStat(double value)
    {
        return Math.Abs(value) >= 100
            ? value.ToString("0", CultureInfo.InvariantCulture)
            : value.ToString("0.0", CultureInfo.InvariantCulture);
    }
}
