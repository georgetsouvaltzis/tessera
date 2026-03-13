using System.ComponentModel;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a simple line chart control.
/// </summary>
public sealed class LineChart : Control
{
    private readonly LineChartComponent _component;

    /// <summary>
    /// Creates a line chart with the provided sample capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained samples.</param>
    public LineChart(int capacity = 240)
    {
        _component = new LineChartComponent(capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained samples.
    /// </summary>
    public int Capacity => _component.Capacity;

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the optional minimum value used when scaling samples.
    /// </summary>
    public double? MinValue
    {
        get => _component.MinValue;
        set => _component.MinValue = value;
    }

    /// <summary>
    /// Gets or sets the optional maximum value used when scaling samples.
    /// </summary>
    public double? MaxValue
    {
        get => _component.MaxValue;
        set => _component.MaxValue = value;
    }

    /// <summary>
    /// Gets or sets the current zoom factor.
    /// </summary>
    public double Zoom
    {
        get => _component.Zoom;
        set => _component.Zoom = value;
    }

    /// <summary>
    /// Gets or sets the current sample offset.
    /// </summary>
    public int Offset
    {
        get => _component.Offset;
        set => _component.Offset = value;
    }

    /// <summary>
    /// Gets the retained sample values.
    /// </summary>
    public IReadOnlyList<double> Samples => _component.Samples;

    /// <summary>
    /// Gets or sets advanced chart rendering options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public LineChartOptions? Options
    {
        get => _component.Options;
        set => _component.Options = value;
    }

    /// <summary>
    /// Replaces the current sample values.
    /// </summary>
    /// <param name="samples">The sample values to render.</param>
    public void SetSamples(IEnumerable<double> samples) => _component.SetSamples(samples ?? Array.Empty<double>());

    /// <summary>
    /// Appends one sample to the chart.
    /// </summary>
    /// <param name="sample">The sample value.</param>
    public void Append(double sample) => _component.Append(sample);

    /// <summary>
    /// Zooms in by the provided step.
    /// </summary>
    /// <param name="step">The zoom step.</param>
    public void ZoomIn(double step = 0.25) => _component.ZoomIn(step);

    /// <summary>
    /// Zooms out by the provided step.
    /// </summary>
    /// <param name="step">The zoom step.</param>
    public void ZoomOut(double step = 0.25) => _component.ZoomOut(step);

    /// <summary>
    /// Pans the visible range by the provided delta.
    /// </summary>
    /// <param name="delta">The pan delta.</param>
    public void Pan(int delta) => _component.Pan(delta);

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
