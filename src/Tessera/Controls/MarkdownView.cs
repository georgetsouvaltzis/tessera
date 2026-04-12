using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
/// Represents a read-only markdown renderer.
/// </summary>
public sealed class MarkdownView : Control
{
    private readonly ViewportModel _viewport = new();
    private string _markdown = string.Empty;

    /// <summary>
    /// Represents title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Markdown";

    /// <summary>
    /// Represents focus marker.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Represents show focus marker.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents content style.
    /// </summary>
    public TesseraStyle ContentStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into border glyphs while the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Represents wrap.
    /// </summary>
    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    /// <summary>
    /// Represents show line numbers.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    /// Executes set markdown.
    /// </summary>
    /// <param name="markdown">The markdown value.</param>
    public void SetMarkdown(string markdown)
    {
        _markdown = markdown ?? string.Empty;
        _viewport.SetLines(MarkdownLineRenderer.Render(_markdown));
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        return _viewport.Update(message, ViewportKeyMap.Default);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : FormatTitle();
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

        _viewport.Resize(content.Width, content.Height);
        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            var line = lines[row];
            if (!ContentStyle.IsEmpty)
            {
                line = ContentStyle.Render(line);
            }

            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var lines = MarkdownLineRenderer.Render(_markdown);
        var width = 0;
        for (var index = 0; index < lines.Count; index++)
        {
            width = Math.Max(width, lines[index].Length);
        }

        if (ShowLineNumbers)
        {
            width += 4;
        }

        width += Padding.Horizontal;
        var height = Math.Max(1, lines.Count) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private string FormatTitle()
    {
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
