using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents read-only text content.
/// </summary>
public sealed class Label : Control
{
    /// <summary>
    ///     Represents text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string? Title
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    ///     Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents text style.
    /// </summary>
    public TesseraStyle TextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents horizontal alignment.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents vertical alignment.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : Title;
        if (!string.IsNullOrEmpty(title))
        {
            var titleStyle = IsFocused ? FocusedTitleStyle : TitleStyle;
            if (!titleStyle.IsEmpty)
            {
                title = titleStyle.Render(title);
            }
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        var lines = ControlTextLayout.SplitLines(Text);
        var rows = Math.Min(content.Height, lines.Length);
        var startY = content.Y + ResolveVerticalOffset(content.Height, lines.Length);
        for (var row = 0; row < rows; row++)
        {
            var line = lines[row];
            var rendered = TextStyle.IsEmpty ? line : TextStyle.Render(line);
            var x = content.X + ResolveHorizontalOffset(content.Width, line);
            canvas.WriteText(x, startY + row, rendered, Math.Max(0, content.Right - x));
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var lines = ControlTextLayout.SplitLines(Text);
        var width = 0;
        for (var index = 0; index < lines.Length; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(lines[index]));
        }

        width += Padding.Horizontal;
        var height = lines.Length + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            if (!string.IsNullOrWhiteSpace(Title))
            {
                var title = Title ?? string.Empty;
                width = Math.Max(width, title.Length + 4);
            }
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private int ResolveHorizontalOffset(int availableWidth, string line)
    {
        return HorizontalAlignment switch
        {
            HorizontalAlignment.Center => Math.Max(0,
                (availableWidth - ControlTextLayout.MeasureDisplayWidth(line)) / 2),
            HorizontalAlignment.Right => Math.Max(0, availableWidth - ControlTextLayout.MeasureDisplayWidth(line)),
            _ => 0
        };
    }

    private int ResolveVerticalOffset(int availableHeight, int lineCount)
    {
        return VerticalAlignment switch
        {
            VerticalAlignment.Center => Math.Max(0, (availableHeight - Math.Min(availableHeight, lineCount)) / 2),
            VerticalAlignment.Bottom => Math.Max(0, availableHeight - Math.Min(availableHeight, lineCount)),
            _ => 0
        };
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }
}
