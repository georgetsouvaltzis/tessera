using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using System.Globalization;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a simple line chart control.
/// </summary>
public sealed class LineChart : Control
{
    private readonly List<double> _samples = [];

    /// <summary>
    /// Creates a line chart with the provided sample capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained samples.</param>
    public LineChart(int capacity = 240)
    {
        Capacity = Math.Max(1, capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained samples.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Line Chart";

    /// <summary>
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether the focused title marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets the title style used when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style used when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for min/max stat text.
    /// </summary>
    public TeaStyle StatsStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for legend and axis labels.
    /// </summary>
    public TeaStyle MetaTextStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the optional minimum value used when scaling samples.
    /// </summary>
    public double? MinValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the optional maximum value used when scaling samples.
    /// </summary>
    public double? MaxValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the current zoom factor.
    /// </summary>
    public double Zoom
    {
        get;
        set;
    } = 1.0;

    /// <summary>
    /// Gets or sets the current sample offset.
    /// </summary>
    public int Offset
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the retained sample values.
    /// </summary>
    public IReadOnlyList<double> Samples => _samples;

    /// <summary>
    /// Gets or sets advanced chart rendering options.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public LineChartOptions? Options
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces the current sample values.
    /// </summary>
    /// <param name="samples">The sample values to render.</param>
    public void SetSamples(IEnumerable<double> samples)
    {
        _samples.Clear();
        foreach (var sample in samples ?? Array.Empty<double>())
        {
            Append(sample);
        }
    }

    /// <summary>
    /// Appends one sample to the chart.
    /// </summary>
    /// <param name="sample">The sample value.</param>
    public void Append(double sample)
    {
        _samples.Add(sample);
        if (_samples.Count > Capacity)
        {
            _samples.RemoveRange(0, _samples.Count - Capacity);
        }
    }

    /// <summary>
    /// Zooms in by the provided step.
    /// </summary>
    /// <param name="step">The zoom step.</param>
    public void ZoomIn(double step = 0.25)
    {
        Zoom = Math.Clamp(Zoom + Math.Max(0.01, step), 0.1, 8.0);
    }

    /// <summary>
    /// Zooms out by the provided step.
    /// </summary>
    /// <param name="step">The zoom step.</param>
    public void ZoomOut(double step = 0.25)
    {
        Zoom = Math.Clamp(Zoom - Math.Max(0.01, step), 0.1, 8.0);
    }

    /// <summary>
    /// Pans the visible range by the provided delta.
    /// </summary>
    /// <param name="delta">The pan delta.</param>
    public void Pan(int delta)
    {
        Offset = Math.Max(0, Offset + delta);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var options = (Options ?? new LineChartOptions()) with
        {
            Zoom = Zoom,
            Offset = Offset,
        };
        if (!MetaTextStyle.IsEmpty)
        {
            options = options with
            {
                Legend = StyleOptional(options.Legend, MetaTextStyle),
                XLabel = StyleOptional(options.XLabel, MetaTextStyle),
                YLabel = StyleOptional(options.YLabel, MetaTextStyle),
            };
        }

        ChartRenderer.DrawLineChart(canvas, rect, _samples, RenderTitle(), MinValue, MaxValue, options);
        TryRenderStyledStats(canvas, rect, options);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 8);
        var height = 8;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
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

    private static string StyleOptional(string? value, TeaStyle style)
    {
        if (string.IsNullOrWhiteSpace(value) || style.IsEmpty)
        {
            return value ?? string.Empty;
        }

        return style.Render(value.Trim());
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private void TryRenderStyledStats(Canvas canvas, Rect rect, LineChartOptions options)
    {
        if (StatsStyle.IsEmpty || _samples.Count == 0)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var plot = options.ShowAxes && content.Width >= 6 && content.Height >= 4
            ? new Rect(content.X + 1, content.Y, content.Width - 1, content.Height - 1)
            : content;
        if (plot.IsEmpty)
        {
            return;
        }

        var zoom = double.IsFinite(options.Zoom)
            ? Math.Clamp(options.Zoom, 0.1, 8.0)
            : 1.0;
        var count = Math.Clamp((int)Math.Round(plot.Width / zoom, MidpointRounding.AwayFromZero), 1, _samples.Count);
        var maxOffset = Math.Max(0, _samples.Count - count);
        var offset = Math.Clamp(options.Offset, 0, maxOffset);
        var min = MinValue ?? double.PositiveInfinity;
        var max = MaxValue ?? double.NegativeInfinity;
        if (!MinValue.HasValue || !MaxValue.HasValue)
        {
            for (var index = 0; index < count; index++)
            {
                var value = _samples[offset + index];
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

        if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }

        var stats = StatsStyle.Render($"min:{FormatStat(min)} max:{FormatStat(max)}");
        canvas.WriteText(content.X, content.Y, stats, content.Width);
    }

    private static string FormatStat(double value)
    {
        return Math.Abs(value) >= 100
            ? value.ToString("0", CultureInfo.InvariantCulture)
            : value.ToString("0.0", CultureInfo.InvariantCulture);
    }
}
