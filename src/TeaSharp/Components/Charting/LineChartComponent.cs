using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Charting;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class LineChartComponent : ICanvasComponent
{
    private readonly List<double> _samples = [];

    public LineChartComponent(int capacity = 240)
    {
        Capacity = Math.Max(1, capacity);
    }

    public int Capacity { get; }

    public string Title { get; set; } = "Line Chart";

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public LineChartOptions? Options { get; set; }

    public double Zoom { get; set; } = 1.0;

    public int Offset { get; set; }

    public IReadOnlyList<double> Samples => _samples;

    public void SetSamples(IEnumerable<double> samples)
    {
        _samples.Clear();
        foreach (var sample in samples)
        {
            Append(sample);
        }
    }

    public void Append(double sample)
    {
        _samples.Add(sample);
        if (_samples.Count > Capacity)
        {
            _samples.RemoveRange(0, _samples.Count - Capacity);
        }
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var options = (Options ?? new LineChartOptions()) with
        {
            Zoom = Zoom,
            Offset = Offset,
        };
        Charts.DrawLineChart(canvas, rect, _samples, Title, MinValue, MaxValue, options);
    }

    public void ZoomIn(double step = 0.25)
    {
        Zoom = Math.Clamp(Zoom + Math.Max(0.01, step), 0.1, 8.0);
    }

    public void ZoomOut(double step = 0.25)
    {
        Zoom = Math.Clamp(Zoom - Math.Max(0.01, step), 0.1, 8.0);
    }

    public void Pan(int delta)
    {
        Offset = Math.Max(0, Offset + delta);
    }
}
