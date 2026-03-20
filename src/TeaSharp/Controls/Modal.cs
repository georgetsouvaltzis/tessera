using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a dismissible overlay panel.
/// </summary>
public sealed class Modal : Control
{
    private List<string> _bodyLines = ["(empty)"];

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Modal";

    public bool IsVisible
    {
        get;
        set;
    }

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.Rounded;

    public Thickness Padding
    {
        get;
        set;
    }

    public IReadOnlyList<string> BodyLines
    {
        get => _bodyLines;
        set => _bodyLines = [.. (value ?? ["(empty)"])];
    }

    public char BackdropFill
    {
        get;
        set;
    } = '·';

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
    /// Gets or sets body text style.
    /// </summary>
    public TeaStyle BodyTextStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                canvas.Set(x, y, BackdropFill);
            }
        }

        if (clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        var modalWidth = Math.Clamp(clipped.Width * 3 / 5, 4, Math.Max(4, clipped.Width - 2));
        var modalHeight = Math.Clamp(clipped.Height / 2, 4, Math.Max(4, clipped.Height - 2));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        var modal = new Rect(modalX, modalY, modalWidth, modalHeight);

        FillRect(canvas, modal, ' ');
        var body = FrameLayout.DrawFrameAndResolveContent(canvas, modal, RenderTitle(), Border, Padding, ResolveBorderStyleText());
        if (body.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(body.Height, _bodyLines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(body.X, body.Y + row, ApplyStyle(_bodyLines[row], BodyTextStyle), body.Width);
        }
    }

    public void SetBodyLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        _bodyLines = [.. lines];
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var longest = _bodyLines.Count == 0 ? 8 : _bodyLines.Max(ControlTextLayout.MeasureDisplayWidth);
        var width = Math.Max(ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4, longest + Padding.Horizontal) + 2;
        var height = Math.Max(4, _bodyLines.Count + Padding.Vertical + 2);
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

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }

    private static void FillRect(Canvas canvas, Rect rect, char fill)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                canvas.Set(x, y, fill);
            }
        }
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
