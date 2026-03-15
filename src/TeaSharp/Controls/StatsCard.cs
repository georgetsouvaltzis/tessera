using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact key-value statistics card.
/// </summary>
public sealed class StatsCard : Control
{
    private readonly List<StatItem> _items = [];

    /// <summary>
    /// Gets or sets the card title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Stats";

    /// <summary>
    /// Gets the current card items.
    /// </summary>
    public IReadOnlyList<StatItem> Items => _items;

    /// <summary>
    /// Replaces the current card items.
    /// </summary>
    /// <param name="items">The items to render.</param>
    public void SetItems(IEnumerable<StatItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        foreach (var item in items)
        {
            _items.Add(new StatItem(item.Label ?? string.Empty, item.Value ?? string.Empty));
        }
    }

    /// <summary>
    /// Sets or updates one statistic by label.
    /// </summary>
    /// <param name="label">The item label.</param>
    /// <param name="value">The item value.</param>
    public void SetValue(string label, string value)
    {
        var normalizedLabel = label ?? string.Empty;
        var normalizedValue = value ?? string.Empty;
        for (var i = 0; i < _items.Count; i++)
        {
            if (string.Equals(_items[i].Label, normalizedLabel, StringComparison.Ordinal))
            {
                _items[i] = new StatItem(normalizedLabel, normalizedValue);
                return;
            }
        }

        _items.Add(new StatItem(normalizedLabel, normalizedValue));
    }

    public override void Render(Canvas canvas, Rect rect)
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

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, Title.Length + 4);
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, _items[index].Label.Length + _items[index].Value.Length + 2);
        }

        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
