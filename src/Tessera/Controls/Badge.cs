using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a compact status badge with a semantic tone.
/// </summary>
public sealed class Badge : Control
{
    /// <summary>
    /// Represents text.
    /// </summary>
    public string Text
    {
        get;
        set => field = value ?? string.Empty;
    } = "Badge";

    /// <summary>
    /// Represents show brackets.
    /// </summary>
    public bool ShowBrackets
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the tone.
    /// </summary>
    public BadgeTone Tone { get; set; }

    /// <summary>
    /// Represents text style.
    /// </summary>
    public TesseraStyle TextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents focused text style.
    /// </summary>
    public TesseraStyle FocusedTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents success text style.
    /// </summary>
    public TesseraStyle SuccessTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents warning text style.
    /// </summary>
    public TesseraStyle WarningTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents error text style.
    /// </summary>
    public TesseraStyle ErrorTextStyle
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

        var label = ShowBrackets
            ? $"[{Text}]"
            : Text;
        var state = Tone switch
        {
            BadgeTone.Success => WidgetVisualState.Success,
            BadgeTone.Warning => WidgetVisualState.Warning,
            BadgeTone.Error => WidgetVisualState.Error,
            _ => WidgetVisualState.Default,
        };
        var palette = WidgetStatePalette.CreateDefault();
        var text = palette.Render(label, state);
        var style = ResolveToneStyle();
        if (!style.IsEmpty)
        {
            text = style.Render(text);
        }

        canvas.WriteText(clipped.X, clipped.Y, text, clipped.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(ShowBrackets ? $"[{Text}]" : Text);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(1, 0, availableBounds.Height));
    }

    private TesseraStyle ResolveToneStyle()
    {
        var style = TextStyle;
        switch (Tone)
        {
            case BadgeTone.Success:
                if (!SuccessTextStyle.IsEmpty)
                {
                    style = style.IsEmpty ? SuccessTextStyle : style.Merge(SuccessTextStyle);
                }

                break;
            case BadgeTone.Warning:
                if (!WarningTextStyle.IsEmpty)
                {
                    style = style.IsEmpty ? WarningTextStyle : style.Merge(WarningTextStyle);
                }

                break;
            case BadgeTone.Error:
                if (!ErrorTextStyle.IsEmpty)
                {
                    style = style.IsEmpty ? ErrorTextStyle : style.Merge(ErrorTextStyle);
                }

                break;
        }

        if (IsFocused && !FocusedTextStyle.IsEmpty)
        {
            style = style.IsEmpty ? FocusedTextStyle : style.Merge(FocusedTextStyle);
        }

        return style;
    }
}
