using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a gauge-style metric control.
/// </summary>
public sealed class Gauge : Control
{
    private readonly GaugeComponent _component = new();

    /// <summary>
    /// Gets or sets the gauge title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get => _component.Value;
        set => _component.Value = value;
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double MinValue
    {
        get => _component.MinValue;
        set => _component.MinValue = value;
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double MaxValue
    {
        get => _component.MaxValue;
        set => _component.MaxValue = value;
    }

    /// <summary>
    /// Gets or sets the optional label shown inside the gauge.
    /// </summary>
    public string? Label
    {
        get => _component.Label;
        set => _component.Label = value;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
