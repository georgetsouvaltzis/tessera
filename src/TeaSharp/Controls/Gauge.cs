using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a gauge-style metric control.
/// </summary>
/// <remarks>
/// Values render against the inclusive <see cref="MinValue"/> to <see cref="MaxValue"/> range
/// and clamp when they fall outside that interval. Equal minimum and maximum values render as a flat range.
/// </remarks>
public sealed class Gauge : Control
{
    /// <summary>
    /// Gets or sets the gauge title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Gauge";

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double MinValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double MaxValue
    {
        get;
        set;
    } = 100;

    /// <summary>
    /// Gets or sets the optional label shown inside the gauge.
    /// </summary>
    public string? Label
    {
        get;
        set;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 6 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var span = Math.Abs(MaxValue - MinValue) < double.Epsilon ? 1 : MaxValue - MinValue;
        var normalized = Math.Clamp((Value - MinValue) / span, 0, 1);
        var label = Label ?? Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        var barHeight = Math.Min(content.Height, 2);
        TeaSharp.Components.Primitives.Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, barHeight), normalized, label);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, Title.Length + 4);
        var height = 4;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
