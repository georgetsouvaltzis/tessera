using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

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
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the title style used when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner content padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets style applied to border glyphs when not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style used for item key text.
    /// </summary>
    public TesseraStyle KeyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style used for item value text.
    /// </summary>
    public TesseraStyle ValueStyle { get; set; } = TesseraStyle.Empty;

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

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || bounds.IsEmpty || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return false;
        }

        if (!bounds.Contains(pointer.X, pointer.Y))
        {
            return false;
        }

        RequestFocus();
        return true;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 2 || clipped.Height < 1)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        var body = content;
        if (Border == BorderStyle.None)
        {
            var title = RenderTitle();
            if (!string.IsNullOrEmpty(title))
            {
                canvas.WriteText(content.X, content.Y, title, content.Width);
                if (content.Height <= 1)
                {
                    return;
                }

                body = new Rect(content.X, content.Y + 1, content.Width, content.Height - 1);
            }
        }

        if (body.IsEmpty || _items.Count == 0)
        {
            return;
        }

        var rows = Math.Min(body.Height, _items.Count);
        var keyWidth = Math.Clamp(body.Width / 3, 4, 16);
        for (var row = 0; row < rows; row++)
        {
            var item = _items[row];
            var label = item.Label.Length > keyWidth
                ? item.Label[..keyWidth]
                : item.Label.PadRight(keyWidth);
            var line = $"{ApplyStyle(label, KeyStyle)} {ApplyStyle(item.Value, ValueStyle)}";
            canvas.WriteText(body.X, body.Y + row, line, body.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(1, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()));
        for (var index = 0; index < _items.Count; index++)
        {
            width = Math.Max(width, _items[index].Label.Length + _items[index].Value.Length + 2);
        }

        width += Padding.Horizontal;
        var height = Math.Max(1, _items.Count) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, 8);
            height = Math.Max(height, 3);
        }
        else if (!string.IsNullOrEmpty(FormatTitleForMeasure()))
        {
            height += 1;
        }

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
        if (!string.IsNullOrEmpty(Title) && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private TesseraStyle ResolveBorderStyle()
    {
        return IsFocused
            ? BorderStyleText.Merge(FocusedBorderStyleText)
            : BorderStyleText;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text ?? string.Empty);
    }
}
