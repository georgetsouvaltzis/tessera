namespace TeaSharp.Components;

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

