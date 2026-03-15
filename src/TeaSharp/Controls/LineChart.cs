using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;

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
        ChartRenderer.DrawLineChart(canvas, rect, _samples, Title, MinValue, MaxValue, options);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, Title.Length + 8);
        var height = 8;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
