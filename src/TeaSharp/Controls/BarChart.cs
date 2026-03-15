using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a simple bar chart control.
/// </summary>
public sealed class BarChart : Control
{
    private readonly List<BarPoint> _bars = [];

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Bar Chart";

    /// <summary>
    /// Gets or sets the optional maximum value used when scaling bars.
    /// </summary>
    public double? MaxValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the current chart values.
    /// </summary>
    public IReadOnlyList<BarPoint> Bars => _bars;

    /// <summary>
    /// Gets or sets advanced chart rendering options.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public BarChartOptions? Options
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces the current chart values.
    /// </summary>
    /// <param name="bars">The bar values to render.</param>
    public void SetBars(IEnumerable<BarPoint> bars)
    {
        ArgumentNullException.ThrowIfNull(bars);

        _bars.Clear();
        foreach (var bar in bars)
        {
            _bars.Add(new BarPoint(bar.Label ?? string.Empty, bar.Value));
        }
    }

    /// <summary>
    /// Sets or updates one bar value by label.
    /// </summary>
    /// <param name="label">The bar label.</param>
    /// <param name="value">The bar value.</param>
    public void SetValue(string label, double value)
    {
        var normalizedLabel = label ?? string.Empty;
        for (var i = 0; i < _bars.Count; i++)
        {
            if (string.Equals(_bars[i].Label, normalizedLabel, StringComparison.Ordinal))
            {
                _bars[i] = new BarPoint(normalizedLabel, value);
                return;
            }
        }

        _bars.Add(new BarPoint(normalizedLabel, value));
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        ChartRenderer.DrawBarChart(canvas, rect, _bars, Title, MaxValue, Options);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var widestLabel = 0;
        for (var i = 0; i < _bars.Count; i++)
        {
            widestLabel = Math.Max(widestLabel, _bars[i].Label.Length);
        }

        var width = Math.Max(12, widestLabel + 14);
        var height = Math.Max(4, _bars.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
