using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a compact bullet chart with qualitative ranges, an actual value bar, and a target marker.
/// </summary>
/// <remarks>
/// The chart domain is inferred from configured ranges. When no ranges are configured, the default domain is 0..100.
/// Values and targets are clamped to the resolved domain during rendering.
/// </remarks>
public sealed class BulletChart : Control
{
    private readonly List<BulletRange> _ranges = [];

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Bullet Chart";

    /// <summary>
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focused title marker text is rendered.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets style used for title text when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for title text when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for neutral range segments.
    /// </summary>
    public TeaStyle RangeStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for warning range segments.
    /// </summary>
    public TeaStyle WarningRangeStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for critical range segments.
    /// </summary>
    public TeaStyle CriticalRangeStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for the actual value bar.
    /// </summary>
    public TeaStyle ValueBarStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for the target marker glyph.
    /// </summary>
    public TeaStyle TargetMarkerStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for value/target label text.
    /// </summary>
    public TeaStyle ValueLabelStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets the glyph used for background range segments.
    /// </summary>
    public char RangeGlyph { get; set; } = '░';

    /// <summary>
    /// Gets or sets the glyph used for the actual value bar.
    /// </summary>
    public char ValueGlyph { get; set; } = '█';

    /// <summary>
    /// Gets or sets the glyph used for the target marker.
    /// </summary>
    public char TargetGlyph { get; set; } = '│';

    /// <summary>
    /// Gets currently configured ranges.
    /// </summary>
    public IReadOnlyList<BulletRange> Ranges => _ranges;

    /// <summary>
    /// Gets the current value.
    /// </summary>
    public double Value { get; private set; }

    /// <summary>
    /// Gets the target value.
    /// </summary>
    public double Target { get; private set; }

    /// <summary>
    /// Replaces chart ranges.
    /// </summary>
    /// <param name="ranges">Ranges to render.</param>
    public void SetRanges(IEnumerable<BulletRange> ranges)
    {
        ArgumentNullException.ThrowIfNull(ranges);

        _ranges.Clear();
        foreach (var range in ranges)
        {
            var start = Math.Min(range.Start, range.End);
            var end = Math.Max(range.Start, range.End);
            _ranges.Add(new BulletRange(start, end, range.Kind, range.Label));
        }

        _ranges.Sort(static (left, right) =>
        {
            var startCompare = left.Start.CompareTo(right.Start);
            return startCompare != 0 ? startCompare : left.End.CompareTo(right.End);
        });
    }

    /// <summary>
    /// Sets the current value.
    /// </summary>
    /// <param name="value">Value represented by the foreground bar.</param>
    public void SetValue(double value)
    {
        Value = value;
    }

    /// <summary>
    /// Sets the target value.
    /// </summary>
    /// <param name="target">Target marker value.</param>
    public void SetTarget(double target)
    {
        Target = target;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 2)
        {
            return;
        }

        var content = Border == BorderStyle.None
            ? clipped.Inset(Padding)
            : FrameLayout.DrawFrameAndResolveContent(
                canvas,
                clipped,
                RenderTitle(),
                Border,
                Padding,
                ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        var (min, max) = ResolveDomain();
        DrawRanges(canvas, content, min, max);
        DrawValueBar(canvas, content, min, max);
        DrawTargetMarker(canvas, content, min, max);
        DrawValueLabel(canvas, content);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(18, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 6);
        var height = 2;
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

    private void DrawRanges(Canvas canvas, Rect content, double min, double max)
    {
        if (content.Width <= 0 || content.Height <= 0)
        {
            return;
        }

        if (_ranges.Count == 0)
        {
            var fallback = new string(RangeGlyph, content.Width);
            canvas.WriteText(content.X, content.Y, ApplyStyle(fallback, RangeStyle), content.Width);
            return;
        }

        for (var index = 0; index < _ranges.Count; index++)
        {
            var range = _ranges[index];
            var start = MapToOffset(range.Start, min, max, content.Width);
            var end = MapToOffset(range.End, min, max, content.Width);
            if (end < start)
            {
                (start, end) = (end, start);
            }

            var width = (end - start) + 1;
            if (width <= 0)
            {
                continue;
            }

            var segment = new string(RangeGlyph, width);
            canvas.WriteText(content.X + start, content.Y, ApplyStyle(segment, ResolveRangeStyle(range.Kind)), width);
        }
    }

    private void DrawValueBar(Canvas canvas, Rect content, double min, double max)
    {
        if (content.Width <= 0 || content.Height <= 0)
        {
            return;
        }

        var end = MapToOffset(Value, min, max, content.Width);
        var width = end + 1;
        if (width <= 0)
        {
            return;
        }

        var valueBar = new string(ValueGlyph, width);
        canvas.WriteText(content.X, content.Y, ApplyStyle(valueBar, ValueBarStyle), width);
    }

    private void DrawTargetMarker(Canvas canvas, Rect content, double min, double max)
    {
        if (content.Width <= 0 || content.Height <= 0)
        {
            return;
        }

        var offset = MapToOffset(Target, min, max, content.Width);
        canvas.WriteText(content.X + offset, content.Y, ApplyStyle(TargetGlyph.ToString(), TargetMarkerStyle), 1);
    }

    private void DrawValueLabel(Canvas canvas, Rect content)
    {
        if (content.Height < 2)
        {
            return;
        }

        var label = string.Create(
            CultureInfo.InvariantCulture,
            $"value:{Value:0.##} target:{Target:0.##}");
        canvas.WriteText(content.X, content.Y + 1, ApplyStyle(label, ValueLabelStyle), content.Width);
    }

    private (double Min, double Max) ResolveDomain()
    {
        if (_ranges.Count == 0)
        {
            return (0, 100);
        }

        var min = double.PositiveInfinity;
        var max = double.NegativeInfinity;
        for (var index = 0; index < _ranges.Count; index++)
        {
            min = Math.Min(min, _ranges[index].Start);
            max = Math.Max(max, _ranges[index].End);
        }

        if (!double.IsFinite(min) || !double.IsFinite(max))
        {
            return (0, 100);
        }

        if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }

        return (min, max);
    }

    private TeaStyle ResolveRangeStyle(BulletRangeKind kind)
    {
        return kind switch
        {
            BulletRangeKind.Warning => RangeStyle.Merge(WarningRangeStyle),
            BulletRangeKind.Critical => RangeStyle.Merge(CriticalRangeStyle),
            _ => RangeStyle,
        };
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
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

    private static int MapToOffset(double value, double min, double max, int width)
    {
        if (width <= 1)
        {
            return 0;
        }

        var normalized = Math.Clamp((value - min) / (max - min), 0d, 1d);
        return Math.Clamp(
            (int)Math.Round(normalized * (width - 1), MidpointRounding.AwayFromZero),
            0,
            width - 1);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
