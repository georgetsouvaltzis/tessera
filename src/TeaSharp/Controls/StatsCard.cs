using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

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
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether the focused title marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets the title style used when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style used when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for item key text.
    /// </summary>
    public TeaStyle KeyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for item value text.
    /// </summary>
    public TeaStyle ValueStyle { get; set; } = TeaStyle.Empty;

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

        canvas.DrawBox(clipped, RenderTitle());
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
            var line = $"{ApplyStyle(label, KeyStyle)} {ApplyStyle(item.Value, ValueStyle)}";
            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, _items[index].Label.Length + _items[index].Value.Length + 2);
        }

        var height = Math.Max(3, _items.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text ?? string.Empty);
    }
}
