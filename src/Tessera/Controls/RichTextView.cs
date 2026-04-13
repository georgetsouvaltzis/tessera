using System.Text;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a read-only rich-text viewer for structured docs/help panes.
/// </summary>
public sealed class RichTextView : Control
{
    private readonly List<List<RichTextSegment>> _lines = [];
    private int _lastContentHeight;
    private int _lastContentWidth;

    /// <summary>
    ///     Gets or sets the frame title.
    /// </summary>
    public string Title { get; set; } = "Rich Text";

    /// <summary>
    ///     Gets or sets the marker appended to the title while focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets a value indicating whether the focus marker is shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets style used for title text while not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for title text while focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for plain body text.
    /// </summary>
    public TesseraStyle TextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for headings.
    /// </summary>
    public TesseraStyle HeadingStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for list markers.
    /// </summary>
    public TesseraStyle ListMarkerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for quote markers.
    /// </summary>
    public TesseraStyle QuoteMarkerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for inline emphasis.
    /// </summary>
    public TesseraStyle EmphasisStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for inline strong emphasis.
    /// </summary>
    public TesseraStyle StrongStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for inline code text.
    /// </summary>
    public TesseraStyle InlineCodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into rendered text while <see cref="Control.IsDisabled" /> is <see langword="true" />.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into border glyphs while the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets inner padding for content.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether line wrapping is enabled.
    /// </summary>
    public bool Wrap { get; set; } = true;

    /// <summary>
    ///     Gets the currently configured logical lines.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<RichTextSegment>> Lines => _lines;

    /// <summary>
    ///     Gets the current visual scroll offset (in rendered rows).
    /// </summary>
    public int ScrollOffset { get; private set; }

    /// <summary>
    ///     Sets all logical lines.
    /// </summary>
    /// <param name="lines">The line collection to render.</param>
    public void SetLines(IEnumerable<IEnumerable<RichTextSegment>> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        _lines.Clear();
        foreach (var line in lines)
        {

            var renderedLine = new List<RichTextSegment>();
            foreach (var segment in line)
            {
                renderedLine.Add(new RichTextSegment(segment.Text, segment.StyleKind));
            }

            _lines.Add(renderedLine);
        }

        ScrollOffset = 0;
    }

    /// <summary>
    ///     Replaces content from plain text where each input line becomes one plain rich-text line.
    /// </summary>
    /// <param name="text">The plain text to load.</param>
    public void SetPlainText(string? text)
    {
        var sourceLines = ControlTextLayout.SplitLines(text ?? string.Empty);
        _lines.Clear();
        for (var index = 0; index < sourceLines.Length; index++)
        {
            _lines.Add([RichTextSegment.Plain(sourceLines[index])]);
        }

        ScrollOffset = 0;
    }

    /// <summary>
    ///     Clears all lines.
    /// </summary>
    public void Clear()
    {
        _lines.Clear();
        ScrollOffset = 0;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        var pageSize = Math.Max(1, _lastContentHeight);
        var maxOffset = CalculateMaxScrollOffset();
        int nextOffset;

        switch (key.Key)
        {
            case Key.Up:
                nextOffset = Math.Max(0, ScrollOffset - 1);
                break;
            case Key.Down:
                nextOffset = Math.Min(maxOffset, ScrollOffset + 1);
                break;
            case Key.PageUp:
                nextOffset = Math.Max(0, ScrollOffset - pageSize);
                break;
            case Key.PageDown:
                nextOffset = Math.Min(maxOffset, ScrollOffset + pageSize);
                break;
            case Key.Home:
                nextOffset = 0;
                break;
            case Key.End:
                nextOffset = maxOffset;
                break;
            default:
                return false;
        }

        if (nextOffset == ScrollOffset)
        {
            return false;
        }

        ScrollOffset = nextOffset;
        return true;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = Border == BorderStyle.None
            ? clipped.Inset(Padding)
            : FrameLayout.DrawFrameAndResolveContent(
                canvas,
                clipped,
                FormatTitle(),
                Border,
                Padding,
                ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        _lastContentWidth = content.Width;
        _lastContentHeight = content.Height;
        var visualLines = BuildVisualLines(content.Width);
        var maxOffset = Math.Max(0, visualLines.Count - content.Height);
        if (ScrollOffset > maxOffset)
        {
            ScrollOffset = maxOffset;
        }

        for (var row = 0; row < content.Height; row++)
        {
            var visualIndex = ScrollOffset + row;
            if (visualIndex < 0 || visualIndex >= visualLines.Count)
            {
                continue;
            }

            var rendered = RenderVisualLine(visualLines[visualIndex], content.Width);
            canvas.WriteText(content.X, content.Y + row, rendered, content.Width);
        }
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var borderPadding = Border == BorderStyle.None ? 0 : 2;
        var availableContentWidth = Math.Max(1, availableBounds.Width - borderPadding - Padding.Horizontal);
        var visualLineCount = BuildVisualLines(availableContentWidth).Count;
        var maxContentWidth = ComputeMaxContentWidth();
        var contentWidth = Wrap ? Math.Min(maxContentWidth, availableContentWidth) : maxContentWidth;
        var width = contentWidth + Padding.Horizontal + borderPadding;
        var height = Math.Max(1, visualLineCount) + Padding.Vertical + borderPadding;

        if (Border != BorderStyle.None)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private int CalculateMaxScrollOffset()
    {
        if (_lastContentWidth <= 0 || _lastContentHeight <= 0)
        {
            return Math.Max(0, _lines.Count - 1);
        }

        var visualLineCount = BuildVisualLines(_lastContentWidth).Count;
        return Math.Max(0, visualLineCount - _lastContentHeight);
    }

    private List<List<RichTextSegment>> BuildVisualLines(int width)
    {
        var clampedWidth = Math.Max(1, width);
        var visualLines = new List<List<RichTextSegment>>(_lines.Count);
        for (var index = 0; index < _lines.Count; index++)
        {
            var line = _lines[index];
            if (line.Count == 0)
            {
                visualLines.Add([]);
                continue;
            }

            if (!Wrap)
            {
                visualLines.Add(TruncateSegmentsToWidth(line, clampedWidth));
                continue;
            }

            var current = new List<RichTextSegment>();
            var remaining = clampedWidth;
            for (var segmentIndex = 0; segmentIndex < line.Count; segmentIndex++)
            {
                var segment = line[segmentIndex];
                var text = segment.Text;
                if (text.Length == 0)
                {
                    continue;
                }

                var start = 0;
                while (start < text.Length)
                {
                    if (remaining == 0)
                    {
                        visualLines.Add(current);
                        current = [];
                        remaining = clampedWidth;
                    }

                    var take = Math.Min(remaining, text.Length - start);
                    var chunk = text.Substring(start, take);
                    current.Add(new RichTextSegment(chunk, segment.StyleKind));
                    start += take;
                    remaining -= take;
                }
            }

            visualLines.Add(current);
        }

        if (visualLines.Count == 0)
        {
            visualLines.Add([]);
        }

        return visualLines;
    }

    private static List<RichTextSegment> TruncateSegmentsToWidth(List<RichTextSegment> line, int width)
    {
        var truncated = new List<RichTextSegment>(line.Count);
        var remaining = width;
        for (var index = 0; index < line.Count && remaining > 0; index++)
        {
            var segment = line[index];
            var text = segment.Text;
            if (text.Length == 0)
            {
                continue;
            }

            var take = Math.Min(remaining, text.Length);
            truncated.Add(new RichTextSegment(text[..take], segment.StyleKind));
            remaining -= take;
        }

        return truncated;
    }

    private string RenderVisualLine(List<RichTextSegment> line, int width)
    {
        if (line.Count == 0)
        {
            return string.Empty;
        }

        var remaining = width;
        var builder = new StringBuilder(width);
        for (var index = 0; index < line.Count && remaining > 0; index++)
        {
            var segment = line[index];
            var text = segment.Text;
            if (text.Length == 0)
            {
                continue;
            }

            var value = text.Length <= remaining ? text : text[..remaining];
            var style = ResolveTextStyle(segment.StyleKind);
            builder.Append(style.IsEmpty ? value : style.Render(value));
            remaining -= value.Length;
        }

        return builder.ToString();
    }

    private int ComputeMaxContentWidth()
    {
        var maxWidth = 0;
        for (var lineIndex = 0; lineIndex < _lines.Count; lineIndex++)
        {
            var line = _lines[lineIndex];
            var lineWidth = 0;
            for (var segmentIndex = 0; segmentIndex < line.Count; segmentIndex++)
            {
                lineWidth += ControlTextLayout.MeasureDisplayWidth(line[segmentIndex].Text);
            }

            if (lineWidth > maxWidth)
            {
                maxWidth = lineWidth;
            }
        }

        return Math.Max(1, maxWidth);
    }

    private string? FormatTitle()
    {
        if (Border == BorderStyle.None)
        {
            return null;
        }

        var value = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return style.IsEmpty ? value : style.Render(value);
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private TesseraStyle ResolveTextStyle(RichTextStyleKind styleKind)
    {
        var style = TextStyle;
        style = styleKind switch
        {
            RichTextStyleKind.Heading => style.Merge(HeadingStyle),
            RichTextStyleKind.ListMarker => style.Merge(ListMarkerStyle),
            RichTextStyleKind.QuoteMarker => style.Merge(QuoteMarkerStyle),
            RichTextStyleKind.Emphasis => style.Merge(EmphasisStyle),
            RichTextStyleKind.Strong => style.Merge(StrongStyle),
            RichTextStyleKind.InlineCode => style.Merge(InlineCodeStyle),
            _ => style
        };

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
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
