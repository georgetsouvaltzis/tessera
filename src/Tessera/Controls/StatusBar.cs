using Tessera.Components.Primitives;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a two-sided status strip.
/// </summary>
public sealed class StatusBar : Control
{
    /// <summary>
    /// Represents left text.
    /// </summary>
    public string LeftText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Represents right text.
    /// </summary>
    public string RightText
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Represents fill.
    /// </summary>
    public char Fill
    {
        get;
        set;
    } = ' ';

    /// <summary>
    /// Gets or sets the style used for <see cref="LeftText"/>.
    /// </summary>
    public TesseraStyle LeftTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style used for <see cref="RightText"/>.
    /// </summary>
    public TesseraStyle RightTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style used for the fill row generated from <see cref="Fill"/>.
    /// </summary>
    public TesseraStyle FillStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var leftWidth = Math.Min(clipped.Width, LeftText.Length);
        var rightWidth = Math.Min(clipped.Width, RightText.Length);
        var rightStart = Math.Max(0, clipped.Width - RightText.Length);
        var gapStart = Math.Clamp(leftWidth, 0, clipped.Width);
        var gapWidth = Math.Max(0, rightStart - gapStart);
        if (gapWidth > 0)
        {
            var fillText = new string(Fill, gapWidth);
            canvas.WriteText(clipped.X + gapStart, clipped.Y, ApplyStyle(fillText, FillStyle), gapWidth);
        }

        if (leftWidth > 0)
        {
            canvas.WriteText(clipped.X, clipped.Y, ApplyStyle(LeftText, LeftTextStyle), clipped.Width);
        }

        if (rightStart < clipped.Width)
        {
            canvas.WriteText(
                clipped.X + rightStart,
                clipped.Y,
                ApplyStyle(RightText, RightTextStyle),
                rightWidth);
        }

        if (leftWidth == 0 && rightWidth == 0)
        {
            var fillText = new string(Fill, Math.Max(0, clipped.Width));
            canvas.WriteText(clipped.X, clipped.Y, ApplyStyle(fillText, FillStyle), clipped.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new LayoutMeasurement(
            Math.Clamp(Math.Max(LeftText.Length + RightText.Length, 1), 0, availableBounds.Width),
            Math.Clamp(1, 0, availableBounds.Height));
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }
}
