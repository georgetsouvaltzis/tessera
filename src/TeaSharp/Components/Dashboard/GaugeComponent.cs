namespace TeaSharp.Components.Dashboard;

public sealed class GaugeComponent : ICanvasComponent
{
    public string Title { get; set; } = "Gauge";

    public double Value { get; set; }

    public double MinValue { get; set; }

    public double MaxValue { get; set; } = 100;

    public string? Label { get; set; }

    public void Render(Canvas canvas, Rect rect)
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

        var span = Math.Abs(MaxValue - MinValue) < double.Epsilon
            ? 1
            : MaxValue - MinValue;
        var normalized = Math.Clamp((Value - MinValue) / span, 0, 1);
        var label = Label ?? Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        var barHeight = Math.Min(content.Height, 2);
        TeaSharp.Components.Primitives.Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, barHeight), normalized, label);
    }
}
