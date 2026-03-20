using System.Buffers;
using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents an inline trend sparkline control.
/// </summary>
public sealed class Sparkline : Control
{
    private const string DefaultSteps = "▁▂▃▄▅▆▇█";
    private readonly List<double> _samples = [];

    /// <summary>
    /// Initializes a new sparkline with the provided retained sample capacity.
    /// </summary>
    /// <param name="capacity">The maximum number of retained samples.</param>
    public Sparkline(int capacity = 240)
    {
        Capacity = Math.Max(1, capacity);
    }

    /// <summary>
    /// Gets the maximum number of retained samples.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets or sets the sparkline title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Sparkline";

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
    /// Gets or sets the style used for rendered sparkline glyphs.
    /// </summary>
    public TeaStyle DataStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style used for stats/legend rows when enabled by options.
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
    /// Gets or sets advanced sparkline options.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public SparklineOptions? Options { get; set; }

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
            var style = ResolveStyled(EmptyTextStyle);
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, style), content.Width);
            return;
        }

        var options = Options ?? new SparklineOptions();
        var statsRow = options.ShowStats && content.Height > 1;
        if (statsRow)
        {
            RenderStatsRow(canvas, content, options.Legend);
        }

        var lineY = statsRow ? content.Bottom - 1 : content.Y;
        RenderSparklineRow(canvas, content, lineY, options.Steps);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var titleWidth = ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure());
        var preferredWidth = Math.Max(8, Math.Min(Capacity, 64));
        var width = Math.Max(preferredWidth, titleWidth + 4);
        var height = (Options?.ShowStats ?? false) ? 2 : 1;

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
        if (content.Height <= 1)
        {
            return;
        }

        var (min, max) = ResolveBounds(_samples.Count, Math.Max(0, _samples.Count - Math.Min(_samples.Count, content.Width)));
        var left = $"min:{FormatStat(min)} max:{FormatStat(max)}";
        var styled = ApplyStyle(left, ResolveStyled(MetaStyle));
        canvas.WriteText(content.X, content.Y, styled, content.Width);

        if (!string.IsNullOrWhiteSpace(legend))
        {
            var legendText = legend.Trim();
            if (legendText.Length > 0)
            {
                var rightX = Math.Max(content.X, content.Right - legendText.Length);
                canvas.WriteText(rightX, content.Y, ApplyStyle(legendText, ResolveStyled(MetaStyle)), content.Right - rightX);
            }
        }
    }

    private void RenderSparklineRow(Canvas canvas, Rect content, int y, string? configuredSteps)
    {
        if (y < content.Y || y >= content.Bottom || content.Width <= 0)
        {
            return;
        }

        var steps = ResolveSteps(configuredSteps);
        var count = Math.Min(_samples.Count, content.Width);
        if (count <= 0)
        {
            return;
        }

        var offset = _samples.Count - count;
        var (min, max) = ResolveBounds(count, offset);
        var range = max - min;
        if (Math.Abs(range) < double.Epsilon)
        {
            range = 1;
        }

        var startX = content.X + (content.Width - count);
        var rented = ArrayPool<char>.Shared.Rent(count);
        try
        {
            var buffer = rented.AsSpan(0, count);
            for (var index = 0; index < count; index++)
            {
                var sample = _samples[offset + index];
                var normalized = Math.Clamp((sample - min) / range, 0d, 1d);
                var stepIndex = (int)Math.Round(normalized * (steps.Length - 1), MidpointRounding.AwayFromZero);
                stepIndex = Math.Clamp(stepIndex, 0, steps.Length - 1);
                buffer[index] = steps[stepIndex];
            }

            var line = new string(buffer);
            canvas.WriteText(startX, y, ApplyStyle(line, ResolveStyled(DataStyle)), count);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
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

    private TeaStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return ResolveStyled(style);
    }

    private TeaStyle ResolveStyled(TeaStyle style)
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

    private static ReadOnlySpan<char> ResolveSteps(string? configured)
    {
        var steps = string.IsNullOrWhiteSpace(configured)
            ? DefaultSteps
            : configured!;
        return steps.AsSpan().Length >= 2
            ? steps.AsSpan()
            : DefaultSteps.AsSpan();
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
