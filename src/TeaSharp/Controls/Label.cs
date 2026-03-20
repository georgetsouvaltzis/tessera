using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents read-only text content.
/// </summary>
public sealed class Label : Control
{
    public string Text
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    public string? Title
    {
        get;
        set;
    }

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public TeaStyle TextStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaSharp.Layout.HorizontalAlignment HorizontalAlignment
    {
        get;
        set;
    }

    public TeaSharp.Layout.VerticalAlignment VerticalAlignment
    {
        get;
        set;
    }

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
                width = Math.Max(width, Title!.Length + 4);
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
            TeaSharp.Layout.HorizontalAlignment.Center => Math.Max(0, (availableWidth - ControlTextLayout.MeasureDisplayWidth(line)) / 2),
            TeaSharp.Layout.HorizontalAlignment.Right => Math.Max(0, availableWidth - ControlTextLayout.MeasureDisplayWidth(line)),
            _ => 0,
        };
    }

    private int ResolveVerticalOffset(int availableHeight, int lineCount)
    {
        return VerticalAlignment switch
        {
            TeaSharp.Layout.VerticalAlignment.Center => Math.Max(0, (availableHeight - Math.Min(availableHeight, lineCount)) / 2),
            TeaSharp.Layout.VerticalAlignment.Bottom => Math.Max(0, availableHeight - Math.Min(availableHeight, lineCount)),
            _ => 0,
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
}
