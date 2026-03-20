using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a filled trend graph control.
/// </summary>
public sealed class AreaPlot : Control
{
    private readonly List<double> _samples = [];

    /// <summary>
    /// Initializes a new area plot with the provided retained sample capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained samples.</param>
    public AreaPlot(int capacity = 240)
    {
        Capacity = Math.Max(1, capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained samples.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets or sets the plot title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Area Plot";

    /// <summary>
    /// Gets or sets the marker appended to the title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> should be shown while focused.
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
    /// Gets or sets style used for area fill glyphs.
    /// </summary>
    public TeaStyle FillStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for the trend line glyph.
    /// </summary>
    public TeaStyle LineStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for baseline glyphs when enabled.
    /// </summary>
    public TeaStyle BaselineStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for stats/legend rows.
    /// </summary>
    public TeaStyle MetaStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rendered output while <see cref="Control.IsDisabled" /> is <see langword="true" />.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to empty text when there are no samples.
    /// </summary>
    public TeaStyle EmptyTextStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding for chart content.
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
    /// Gets or sets advanced area-plot options.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public AreaPlotOptions? Options { get; set; }

    /// <summary>
    /// Gets retained sample values.
    /// </summary>
    public IReadOnlyList<double> Samples => _samples;

    /// <summary>
    /// Replaces current samples.
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
        if (_samples.Count > Capacity)
        {
            _samples.RemoveAt(0);
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

        var options = Options ?? new AreaPlotOptions();
        var plot = content;
        if (options.ShowStats && plot.Height > 1)
        {
            RenderStatsRow(canvas, content, options.Legend);
            plot = new Rect(content.X, content.Y + 1, content.Width, content.Height - 1);
        }

        if (plot.IsEmpty)
        {
            return;
        }

        RenderPlot(canvas, plot, options);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var titleWidth = ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure());
        var width = Math.Max(16, Math.Max(Math.Min(Capacity, 80), titleWidth + 4));
        var height = Math.Max(4, (Options?.ShowStats ?? false) ? 7 : 6);

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

    private void RenderStatsRow(Canvas canvas, Rect content, string? legend)
    {
        var count = Math.Min(_samples.Count, content.Width);
        var offset = Math.Max(0, _samples.Count - count);
        var (min, max) = ResolveBounds(count, offset);
        var stats = $"min:{FormatStat(min)} max:{FormatStat(max)}";
        canvas.WriteText(content.X, content.Y, ApplyStyle(stats, ResolveStyled(MetaStyle)), content.Width);

        if (!string.IsNullOrWhiteSpace(legend))
        {
            var text = legend.Trim();
            if (text.Length > 0)
            {
                var rightX = Math.Max(content.X, content.Right - text.Length);
                canvas.WriteText(rightX, content.Y, ApplyStyle(text, ResolveStyled(MetaStyle)), content.Right - rightX);
            }
        }
    }

    private void RenderPlot(Canvas canvas, Rect plot, AreaPlotOptions options)
    {
        var count = Math.Min(_samples.Count, plot.Width);
        if (count <= 0)
        {
            return;
        }

        var offset = _samples.Count - count;
        var startX = plot.X + (plot.Width - count);
        var (min, max) = ResolveBounds(count, offset);
        var range = max - min;
        if (Math.Abs(range) < double.Epsilon)
        {
            range = 1;
        }

        var fillStyle = ResolveStyled(FillStyle);
        var lineStyle = ResolveStyled(LineStyle);
        var baselineStyle = ResolveStyled(BaselineStyle);

        var fillGlyph = options.FillGlyph == '\0' ? '█' : options.FillGlyph;
        var lineGlyph = options.LineGlyph == '\0' ? '▀' : options.LineGlyph;
        var baselineGlyph = options.BaselineGlyph == '\0' ? '─' : options.BaselineGlyph;

        if (options.ShowBaseline)
        {
            var baselineY = plot.Bottom - 1;
            for (var x = plot.X; x < plot.Right; x++)
            {
                WriteCell(canvas, x, baselineY, baselineGlyph, baselineStyle);
            }
        }

        for (var index = 0; index < count; index++)
        {
            var sample = _samples[offset + index];
            var normalized = Math.Clamp((sample - min) / range, 0d, 1d);
            var topY = plot.Bottom - 1 - (int)Math.Round(normalized * (plot.Height - 1), MidpointRounding.AwayFromZero);
            var x = startX + index;

            for (var y = topY; y < plot.Bottom; y++)
            {
                var isLine = y == topY;
                WriteCell(canvas, x, y, isLine ? lineGlyph : fillGlyph, isLine ? lineStyle : fillStyle);
            }
        }
    }

    private (double Min, double Max) ResolveBounds(int count, int offset)
    {
        var min = MinValue ?? double.PositiveInfinity;
        var max = MaxValue ?? double.NegativeInfinity;
        if (!MinValue.HasValue || !MaxValue.HasValue)
        {
            for (var index = 0; index < count; index++)
            {
                var value = _samples[offset + index];
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

    private static void WriteCell(Canvas canvas, int x, int y, char glyph, TeaStyle style)
    {
        if (style.IsEmpty)
        {
            canvas.Set(x, y, glyph);
            return;
        }

        canvas.WriteText(x, y, style.Render(glyph.ToString()), 1);
    }

    private TeaStyle ResolveStyled(TeaStyle style)
    {
        return IsDisabled
            ? style.Merge(DisabledStyle)
            : style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return ResolveStyled(style);
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

    private static string ApplyStyle(string text, TeaStyle style)
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
