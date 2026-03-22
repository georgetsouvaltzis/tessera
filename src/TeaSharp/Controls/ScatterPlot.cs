using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a scatter plot control for telemetry and analytical point series.
/// </summary>
public sealed class ScatterPlot : Control
{
    private readonly List<ScatterPlotPoint> _points = [];
    private int? _capacity;

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Scatter Plot";

    /// <summary>
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focused title marker text is rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets style used for title text when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for title text when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for plotted points.
    /// </summary>
    public TeaStyle PointStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for axis lines and axis labels.
    /// </summary>
    public TeaStyle AxisStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for optional point labels.
    /// </summary>
    public TeaStyle LabelStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for optional legend text.
    /// </summary>
    public TeaStyle LegendStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets an optional explicit minimum X value for scaling.
    /// </summary>
    public double? MinX { get; set; }

    /// <summary>
    /// Gets or sets an optional explicit maximum X value for scaling.
    /// </summary>
    public double? MaxX { get; set; }

    /// <summary>
    /// Gets or sets an optional explicit minimum Y value for scaling.
    /// </summary>
    public double? MinY { get; set; }

    /// <summary>
    /// Gets or sets an optional explicit maximum Y value for scaling.
    /// </summary>
    public double? MaxY { get; set; }

    /// <summary>
    /// Gets or sets advanced scatter rendering options.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public ScatterPlotOptions? Options { get; set; }

    /// <summary>
    /// Gets currently configured points.
    /// </summary>
    public IReadOnlyList<ScatterPlotPoint> Points => _points;

    /// <summary>
    /// Gets or sets an optional retained point capacity.
    /// </summary>
    /// <remarks>
    /// When set, older points are trimmed automatically after <see cref="SetPoints"/> and <see cref="Append"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
    public int? Capacity
    {
        get => _capacity;
        set
        {
            if (value is <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Capacity must be greater than zero.");
            }

            _capacity = value;
            TrimToCapacity();
        }
    }

    /// <summary>
    /// Replaces current points.
    /// </summary>
    /// <param name="points">Point values to render.</param>
    public void SetPoints(IEnumerable<ScatterPlotPoint> points)
    {
        ArgumentNullException.ThrowIfNull(points);
        _points.Clear();
        foreach (var point in points)
        {
            _points.Add(new ScatterPlotPoint(point.X, point.Y, point.Label));
        }

        TrimToCapacity();
    }

    /// <summary>
    /// Appends one point.
    /// </summary>
    /// <param name="point">The point to append.</param>
    public void Append(ScatterPlotPoint point)
    {
        _points.Add(new ScatterPlotPoint(point.X, point.Y, point.Label));
        TrimToCapacity();
    }

    /// <summary>
    /// Trims retained points to the last <paramref name="count"/> values.
    /// </summary>
    /// <param name="count">The number of trailing points to keep.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative.</exception>
    public void TrimToLast(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Trim count must be non-negative.");
        }

        if (count == 0)
        {
            _points.Clear();
            return;
        }

        if (_points.Count > count)
        {
            _points.RemoveRange(0, _points.Count - count);
        }
    }

    /// <summary>
    /// Clears all points.
    /// </summary>
    public void Clear()
    {
        _points.Clear();
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        canvas.DrawBox(clipped, RenderTitle());
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        if (_points.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(empty)", LabelStyle), content.Width);
            return;
        }

        var options = Options ?? new ScatterPlotOptions();
        var plot = content;
        if (options.ShowAxes && content.Width >= 4 && content.Height >= 4)
        {
            DrawVerticalAxis(canvas, content.X, content.Y, content.Height);
            DrawHorizontalAxis(canvas, content.X, content.Bottom - 1, content.Width);
            DrawAxisIntersection(canvas, content.X, content.Bottom - 1);
            plot = new Rect(content.X + 1, content.Y, content.Width - 1, content.Height - 1);
        }

        if (plot.IsEmpty)
        {
            return;
        }

        ResolveBounds(out var minX, out var maxX, out var minY, out var maxY);
        var glyph = options.PointGlyph;
        var pointGlyph = glyph.ToString();
        var styledGlyph = ApplyStyle(pointGlyph, PointStyle);
        var writeStyledGlyph = !PointStyle.IsEmpty;

        for (var index = 0; index < _points.Count; index++)
        {
            var point = _points[index];
            var x = MapToPlot(point.X, minX, maxX, plot.X, plot.Width);
            var y = plot.Bottom - 1 - MapToPlot(point.Y, minY, maxY, 0, plot.Height);

            if (writeStyledGlyph)
            {
                canvas.WriteText(x, y, styledGlyph, 1);
            }
            else
            {
                canvas.Set(x, y, glyph);
            }

            if (options.ShowLabels && !string.IsNullOrWhiteSpace(point.Label) && x + 1 < plot.Right)
            {
                canvas.WriteText(x + 1, y, ApplyStyle(point.Label, LabelStyle), plot.Right - (x + 1));
            }
        }

        RenderOptionalMeta(canvas, content, options);
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 8);
        var height = 8;

        if (_points.Count > 0)
        {
            var longestLabel = 0;
            for (var index = 0; index < _points.Count; index++)
            {
                longestLabel = Math.Max(longestLabel, ControlTextLayout.MeasureDisplayWidth(_points[index].Label));
            }

            width = Math.Max(width, longestLabel + 12);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void ResolveBounds(out double minX, out double maxX, out double minY, out double maxY)
    {
        minX = MinX ?? double.PositiveInfinity;
        maxX = MaxX ?? double.NegativeInfinity;
        minY = MinY ?? double.PositiveInfinity;
        maxY = MaxY ?? double.NegativeInfinity;

        if (!MinX.HasValue || !MaxX.HasValue || !MinY.HasValue || !MaxY.HasValue)
        {
            for (var index = 0; index < _points.Count; index++)
            {
                var point = _points[index];
                if (!MinX.HasValue && point.X < minX)
                {
                    minX = point.X;
                }

                if (!MaxX.HasValue && point.X > maxX)
                {
                    maxX = point.X;
                }

                if (!MinY.HasValue && point.Y < minY)
                {
                    minY = point.Y;
                }

                if (!MaxY.HasValue && point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }
        }

        if (Math.Abs(maxX - minX) < double.Epsilon)
        {
            maxX = minX + 1;
        }

        if (Math.Abs(maxY - minY) < double.Epsilon)
        {
            maxY = minY + 1;
        }
    }

    private static int MapToPlot(double value, double min, double max, int origin, int span)
    {
        if (span <= 1)
        {
            return origin;
        }

        var normalized = Math.Clamp((value - min) / (max - min), 0, 1);
        return origin + (int)Math.Round(normalized * (span - 1), MidpointRounding.AwayFromZero);
    }

    private void RenderOptionalMeta(Canvas canvas, Rect content, ScatterPlotOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Legend))
        {
            var legend = ApplyStyle(options.Legend.Trim(), LegendStyle);
            var legendX = Math.Max(content.X, content.Right - legend.Length);
            canvas.WriteText(legendX, content.Y, legend, content.Right - legendX);
        }

        if (options.ShowAxes && !string.IsNullOrWhiteSpace(options.XLabel))
        {
            var xLabel = ApplyStyle(options.XLabel.Trim(), AxisStyle);
            var xLabelX = Math.Max(content.X, content.Right - xLabel.Length);
            canvas.WriteText(xLabelX, content.Bottom - 1, xLabel, content.Right - xLabelX);
        }

        if (options.ShowAxes && !string.IsNullOrWhiteSpace(options.YLabel))
        {
            var yLabel = ApplyStyle(options.YLabel.Trim(), AxisStyle);
            canvas.WriteText(content.X, content.Y, yLabel, Math.Min(4, content.Width));
        }
    }

    private void DrawVerticalAxis(Canvas canvas, int x, int y, int height)
    {
        if (AxisStyle.IsEmpty)
        {
            canvas.DrawVerticalLine(x, y, height, '│');
            return;
        }

        var glyph = ApplyStyle("│", AxisStyle);
        for (var index = 0; index < height; index++)
        {
            canvas.WriteText(x, y + index, glyph, 1);
        }
    }

    private void DrawHorizontalAxis(Canvas canvas, int x, int y, int width)
    {
        if (AxisStyle.IsEmpty)
        {
            canvas.DrawHorizontalLine(x, y, width, '─');
            return;
        }

        var glyph = ApplyStyle("─", AxisStyle);
        for (var index = 0; index < width; index++)
        {
            canvas.WriteText(x + index, y, glyph, 1);
        }
    }

    private void DrawAxisIntersection(Canvas canvas, int x, int y)
    {
        if (AxisStyle.IsEmpty)
        {
            canvas.Set(x, y, '└');
            return;
        }

        canvas.WriteText(x, y, ApplyStyle("└", AxisStyle), 1);
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

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text ?? string.Empty);
    }

    private void TrimToCapacity()
    {
        if (_capacity.HasValue && _points.Count > _capacity.Value)
        {
            _points.RemoveRange(0, _points.Count - _capacity.Value);
        }
    }
}
