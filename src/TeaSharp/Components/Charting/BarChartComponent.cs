namespace TeaSharp.Components.Charting;

public sealed class BarChartComponent : ICanvasComponent
{
    private readonly List<BarDatum> _bars = [];

    public string Title { get; set; } = "Bar Chart";

    public double? MaxValue { get; set; }

    public BarChartOptions? Options { get; set; }

    public IReadOnlyList<BarDatum> Bars => _bars;

    public void SetBars(IEnumerable<BarDatum> bars)
    {
        _bars.Clear();
        _bars.AddRange(bars);
    }

    public void SetValue(string label, double value)
    {
        for (var i = 0; i < _bars.Count; i++)
        {
            if (string.Equals(_bars[i].Label, label, StringComparison.Ordinal))
            {
                _bars[i] = _bars[i] with { Value = value };
                return;
            }
        }

        _bars.Add(new BarDatum(label, value));
    }

    public void Render(Canvas canvas, Rect rect)
    {
        Charts.DrawBarChart(canvas, rect, _bars, Title, MaxValue, Options);
    }
}
