using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a tiny-card telemetry chart optimized for dense dashboard surfaces.
/// </summary>
public sealed partial class TelemetryChart : Control
{
    private readonly List<double> _samples = [];

    /// <summary>
    /// Initializes a new telemetry chart with the provided retained sample capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained samples.</param>
    public TelemetryChart(int capacity = 240)
    {
        Capacity = Math.Max(1, capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained samples.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Telemetry";

    /// <summary>
    /// Gets or sets the marker appended to the title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker"/> should be shown while focused.
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
    /// Gets or sets the style used for rendered telemetry coverage.
    /// </summary>
    public TesseraStyle FillStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style used for stats and legend text.
    /// </summary>
    public TesseraStyle MetaStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rendered output while <see cref="Control.IsDisabled"/> is <see langword="true"/>.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to empty-state text.
    /// </summary>
    public TesseraStyle EmptyTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.None;

    /// <summary>
    /// Gets or sets inner padding applied to chart content.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets optional explicit minimum value used for normalization.
    /// </summary>
    public double? MinValue { get; set; }

    /// <summary>
    /// Gets or sets optional explicit maximum value used for normalization.
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    /// Gets or sets text shown when no samples are present.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no samples)";

    /// <summary>
    /// Gets or sets advanced telemetry-chart options.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public TelemetryChartOptions? Options { get; set; }

    /// <summary>
    /// Gets retained sample values.
    /// </summary>
    public IReadOnlyList<double> Samples => _samples;

    /// <summary>
    /// Replaces the current sample values.
    /// </summary>
    /// <param name="samples">The sample values to render.</param>
    public void SetSamples(IEnumerable<double> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);

        _samples.Clear();
        foreach (var sample in samples)
        {
            _samples.Add(sample);
        }

        TrimToCapacity();
    }

    /// <summary>
    /// Appends one sample.
    /// </summary>
    /// <param name="sample">The sample value.</param>
    public void Append(double sample)
    {
        _samples.Add(sample);
        TrimToCapacity();
    }

    /// <summary>
    /// Trims retained samples to the last <paramref name="count"/> values.
    /// </summary>
    /// <param name="count">The number of trailing samples to keep.</param>
    public void TrimToLast(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Trim count must be non-negative.");
        }

        if (count == 0)
        {
            _samples.Clear();
            return;
        }

        if (_samples.Count > count)
        {
            _samples.RemoveRange(0, _samples.Count - count);
        }
    }

    /// <summary>
    /// Clears all samples.
    /// </summary>
    public void Clear()
    {
        _samples.Clear();
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = ResolveContent(canvas, clipped);
        if (content.IsEmpty)
        {
            return;
        }

        if (_samples.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveStyled(EmptyTextStyle)), content.Width);
            return;
        }

        var options = Options ?? new TelemetryChartOptions();
        var statsHeight = ResolveStatsHeight(content, options.Legend);
        if (statsHeight > 0)
        {
            RenderStatsRows(canvas, content, options.Legend, statsHeight);
        }

        var chartArea = statsHeight == 0
            ? content
            : new Rect(content.X, content.Y + statsHeight, content.Width, content.Height - statsHeight);
        if (chartArea.IsEmpty)
        {
            return;
        }

        RenderTelemetry(canvas, chartArea, options.RenderMode);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var titleWidth = ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure());
        var preferredWidth = Math.Max(12, Math.Min(Capacity, 48));
        var width = Math.Max(preferredWidth, titleWidth + 4);
        var height = (Options?.ShowStats ?? false) ? 5 : 4;

        width += Padding.Horizontal;
        height += Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private Rect ResolveContent(Canvas canvas, Rect clipped)
    {
        if (Border == BorderStyle.None)
        {
            return clipped.Inset(Padding);
        }

        return FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
    }

    private int ResolveStatsHeight(Rect content, string? legend)
    {
        var options = Options ?? new TelemetryChartOptions();
        if (!options.ShowStats || content.Height <= 2)
        {
            return 0;
        }

        var statsWidth = ResolveStatsTextWidth(legend);
        if (content.Width >= statsWidth)
        {
            return 1;
        }

        return content.Height > 3 ? 2 : 1;
    }

    private void RenderStatsRows(Canvas canvas, Rect content, string? legend, int statsHeight)
    {
        var current = _samples.Count == 0 ? 0d : _samples[^1];
        var (min, max) = ResolveBounds();
        var style = ResolveStyled(MetaStyle);
        var legendText = string.IsNullOrWhiteSpace(legend) ? string.Empty : legend.Trim();

        if (statsHeight == 1)
        {
            var left = $"now:{FormatStat(current)} min:{FormatStat(min)} max:{FormatStat(max)}";
            canvas.WriteText(content.X, content.Y, ApplyStyle(left, style), content.Width);

            if (!string.IsNullOrEmpty(legendText))
            {
                var rightX = Math.Max(content.X, content.Right - legendText.Length);
                canvas.WriteText(rightX, content.Y, ApplyStyle(legendText, style), content.Right - rightX);
            }

            return;
        }

        canvas.WriteText(content.X, content.Y, ApplyStyle($"now:{FormatStat(current)}", style), content.Width);
        canvas.WriteText(content.X, content.Y + 1, ApplyStyle($"min:{FormatStat(min)} max:{FormatStat(max)}", style), content.Width);

        if (!string.IsNullOrEmpty(legendText))
        {
            var rightX = Math.Max(content.X, content.Right - legendText.Length);
            canvas.WriteText(rightX, content.Y + 1, ApplyStyle(legendText, style), content.Right - rightX);
        }
    }

    private int ResolveStatsTextWidth(string? legend)
    {
        var current = _samples.Count == 0 ? 0d : _samples[^1];
        var (min, max) = ResolveBounds();
        var stats = $"now:{FormatStat(current)} min:{FormatStat(min)} max:{FormatStat(max)}";
        var legendText = string.IsNullOrWhiteSpace(legend) ? string.Empty : legend.Trim();
        return stats.Length + (legendText.Length > 0 ? legendText.Length + 1 : 0);
    }

    private (double Min, double Max) ResolveBounds()
    {
        var min = MinValue ?? double.PositiveInfinity;
        var max = MaxValue ?? double.NegativeInfinity;
        if (!MinValue.HasValue || !MaxValue.HasValue)
        {
            for (var i = 0; i < _samples.Count; i++)
            {
                var value = _samples[i];
                if (!MinValue.HasValue && value < min)
                {
                    min = value;
                }

                if (!MaxValue.HasValue && value > max)
                {
                    max = value;
                }
            }
        }

        if (!double.IsFinite(min))
        {
            min = 0;
        }

        if (!double.IsFinite(max))
        {
            max = min + 1;
        }

        if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }

        return (min, max);
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return ResolveStyled(style);
    }

    private TesseraStyle ResolveStyled(TesseraStyle style)
    {
        return IsDisabled
            ? style.Merge(DisabledStyle)
            : style;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), ResolveStyled(IsFocused ? FocusedTitleStyle : TitleStyle));
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private string FormatTitleForMeasure()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }

    private static string FormatStat(double value)
    {
        return Math.Abs(value) >= 100
            ? value.ToString("0", CultureInfo.InvariantCulture)
            : value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    private void TrimToCapacity()
    {
        if (_samples.Count > Capacity)
        {
            _samples.RemoveRange(0, _samples.Count - Capacity);
        }
    }
}
