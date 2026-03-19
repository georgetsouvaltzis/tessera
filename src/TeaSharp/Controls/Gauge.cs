using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a gauge-style metric control.
/// </summary>
/// <remarks>
/// Values render against the inclusive <see cref="MinValue"/> to <see cref="MaxValue"/> range
/// and clamp when they fall outside that interval. Equal minimum and maximum values render as a flat range.
/// </remarks>
public sealed class Gauge : Control
{
    /// <summary>
    /// Gets or sets the gauge title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Gauge";

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
    /// Gets or sets style used for the value label text.
    /// </summary>
    public TeaStyle ValueLabelStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double MinValue
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double MaxValue
    {
        get;
        set;
    } = 100;

    /// <summary>
    /// Gets or sets the optional label shown inside the gauge.
    /// </summary>
    public string? Label
    {
        get;
        set;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 6 || clipped.Height < 3)
        {
            return;
        }

        canvas.DrawBox(clipped, RenderTitle());
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var span = Math.Abs(MaxValue - MinValue) < double.Epsilon ? 1 : MaxValue - MinValue;
        var normalized = Math.Clamp((Value - MinValue) / span, 0, 1);
        var label = Label ?? Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        label = ApplyStyle(label, ValueLabelStyle);
        var barHeight = Math.Min(content.Height, 2);
        TeaSharp.Components.Primitives.Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, barHeight), normalized, label);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        var height = 4;
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
        return style.IsEmpty ? text : style.Render(text);
    }
}
