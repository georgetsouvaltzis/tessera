namespace TeaSharp.Components;

public readonly record struct StatsCardItem(string Label, string Value);

public sealed class GaugeComponent : ICanvasComponent
{
    public string Title { get; set; } = "Gauge";

    public double Value { get; set; }

    public double MinValue { get; set; } = 0;

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
        Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, barHeight), normalized, label);
    }
}

public sealed class StatsCardComponent : ICanvasComponent
{
    private readonly List<StatsCardItem> _items = [];

    public string Title { get; set; } = "Stats";

    public IReadOnlyList<StatsCardItem> Items => _items;

    public void SetItems(IEnumerable<StatsCardItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }

    public void SetValue(string label, string value)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            if (string.Equals(_items[i].Label, label, StringComparison.Ordinal))
            {
                _items[i] = _items[i] with { Value = value };
                return;
            }
        }

        _items.Add(new StatsCardItem(label, value));
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 8 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty || _items.Count == 0)
        {
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        var keyWidth = Math.Clamp(content.Width / 3, 4, 16);
        for (var row = 0; row < rows; row++)
        {
            var item = _items[row];
            var label = item.Label.Length > keyWidth
                ? item.Label[..keyWidth]
                : item.Label.PadRight(keyWidth);
            var line = $"{label} {item.Value}";
            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }
}

public sealed class MiniLogComponent : ICanvasComponent
{
    private readonly List<string> _entries = [];

    public MiniLogComponent(int capacity = 120)
    {
        Capacity = Math.Max(1, capacity);
    }

    public int Capacity { get; }

    public string Title { get; set; } = "Mini Log";

    public IReadOnlyList<string> Entries => _entries;

    public void Append(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        var normalized = line
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var parts = normalized.Split('\n');
        foreach (var part in parts)
        {
            _entries.Add(part);
            if (_entries.Count > Capacity)
            {
                _entries.RemoveAt(0);
            }
        }
    }

    public void Clear()
    {
        _entries.Clear();
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty || _entries.Count == 0)
        {
            return;
        }

        var rows = Math.Min(content.Height, _entries.Count);
        var offset = Math.Max(0, _entries.Count - rows);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, _entries[offset + row], content.Width);
        }
    }
}
