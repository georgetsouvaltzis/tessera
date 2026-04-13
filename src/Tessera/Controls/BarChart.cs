using System.ComponentModel;
using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a simple bar chart control.
/// </summary>
public sealed class BarChart : Control
{
    private readonly List<BarPoint> _bars = [];

    /// <summary>
    ///     Gets or sets the chart title.
    /// </summary>
    public string Title { get; set; } = "Bar Chart";

    /// <summary>
    ///     Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether the focused title marker should be rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets the title style used when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the title style used when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for bar labels.
    /// </summary>
    public TesseraStyle LabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for legend text.
    /// </summary>
    public TesseraStyle LegendStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the optional maximum value used when scaling bars.
    /// </summary>
    public double? MaxValue
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets the current chart values.
    /// </summary>
    public IReadOnlyList<BarPoint> Bars => _bars;

    /// <summary>
    ///     Gets or sets advanced chart rendering options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public BarChartOptions? Options
    {
        get;
        set;
    }

    /// <summary>
    ///     Replaces the current chart values.
    /// </summary>
    /// <param name="bars">The bar values to render.</param>
    public void SetBars(IEnumerable<BarPoint> bars)
    {
        ArgumentNullException.ThrowIfNull(bars);

        _bars.Clear();
        foreach (var bar in bars)
        {
            _bars.Add(new BarPoint(bar.Label, bar.Value));
        }
    }

    /// <summary>
    ///     Sets or updates one bar value by label.
    /// </summary>
    /// <param name="label">The bar label.</param>
    /// <param name="value">The bar value.</param>
    public void SetValue(string label, double value)
    {
        var normalizedLabel = label;
        for (var i = 0; i < _bars.Count; i++)
        {
            if (string.Equals(_bars[i].Label, normalizedLabel, StringComparison.Ordinal))
            {
                _bars[i] = new BarPoint(normalizedLabel, value);
                return;
            }
        }

        _bars.Add(new BarPoint(normalizedLabel, value));
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var barsToRender = CreateRenderBars();
        var options = Options;
        if (!LegendStyle.IsEmpty && options is { } chartOptions &&
            !string.IsNullOrWhiteSpace(chartOptions.Legend))
        {
            options = chartOptions with { Legend = ApplyStyle(chartOptions.Legend, LegendStyle) };
        }

        ChartRenderer.DrawBarChart(canvas, rect, barsToRender, RenderTitle(), MaxValue, options);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var widestLabel = 0;
        for (var i = 0; i < _bars.Count; i++)
        {
            widestLabel = Math.Max(widestLabel, ControlTextLayout.MeasureDisplayWidth(_bars[i].Label));
        }

        var width = Math.Max(12, widestLabel + 14);
        width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        var height = Math.Max(4, _bars.Count + 2);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private IReadOnlyList<BarPoint> CreateRenderBars()
    {
        if (LabelStyle.IsEmpty)
        {
            return _bars;
        }

        var styled = new BarPoint[_bars.Count];
        for (var index = 0; index < _bars.Count; index++)
        {
            styled[index] = new BarPoint(ApplyStyle(_bars[index].Label, LabelStyle), _bars[index].Value);
        }

        return styled;
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

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
