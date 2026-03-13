using System.ComponentModel;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a simple bar chart control.
/// </summary>
public sealed class BarChart : Control
{
    private readonly BarChartComponent _component = new();
    private readonly List<BarPoint> _bars = [];

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the optional maximum value used when scaling bars.
    /// </summary>
    public double? MaxValue
    {
        get => _component.MaxValue;
        set => _component.MaxValue = value;
    }

    /// <summary>
    /// Gets the current chart values.
    /// </summary>
    public IReadOnlyList<BarPoint> Bars => _bars;

    /// <summary>
    /// Gets or sets advanced chart rendering options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public BarChartOptions? Options
    {
        get => _component.Options;
        set => _component.Options = value;
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

        _component.SetBars(_bars.Select(static bar => new BarDatum(bar.Label, bar.Value)));
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
                _component.SetValue(normalizedLabel, value);
                return;
            }
        }

        _bars.Add(new BarPoint(normalizedLabel, value));
        _component.SetValue(normalizedLabel, value);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
