using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact key-value statistics card.
/// </summary>
public sealed class StatsCard : Control
{
    private readonly StatsCardComponent _component = new();
    private readonly List<StatItem> _items = [];

    /// <summary>
    /// Gets or sets the card title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

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

        _component.SetItems(_items.Select(static item => new TeaSharp.Components.Dashboard.StatsCardItem(item.Label, item.Value)));
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
                _component.SetValue(normalizedLabel, normalizedValue);
                return;
            }
        }

        _items.Add(new StatItem(normalizedLabel, normalizedValue));
        _component.SetValue(normalizedLabel, normalizedValue);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
