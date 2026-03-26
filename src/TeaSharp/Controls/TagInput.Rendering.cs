using System.Globalization;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

public sealed partial class TagInput
{
    private const int MinimumInlineWidth = 1;
    private const int MinimumWrappedInputStartWidth = 4;

    private readonly record struct FlowInlineElement(string Text, TeaStyle Style, int Width);
    private readonly record struct TagPlacement(int Index, int X, int Y, string Text, TeaStyle Style, int Width);
    private readonly record struct TextPlacement(int X, int Y, string Text, TeaStyle Style, int Width);

    private sealed class FlowLayoutResult
    {
        public List<TagPlacement> Tags { get; } = [];
        public List<TextPlacement> TextRuns { get; } = [];
        public int Height { get; set; } = 1;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        RenderWrappedFlow(canvas, content);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var frameWidth = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
        var naturalTotalWidth = Math.Max(frameWidth, ResolveNaturalContentWidth() + Padding.Horizontal + ResolveBorderChrome());
        var measuredWidth = availableBounds.Width > 0
            ? Math.Min(naturalTotalWidth, availableBounds.Width)
            : naturalTotalWidth;
        var contentWidth = ResolveContentWidth(measuredWidth);
        var flow = BuildFlowLayout(contentWidth);
        var height = flow.Height + Padding.Vertical + ResolveBorderChrome();

        return new LayoutMeasurement(
            Math.Clamp(measuredWidth, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderWrappedFlow(Canvas canvas, Rect content)
    {
        var flow = BuildFlowLayout(content.Width);
        for (var index = 0; index < flow.Tags.Count; index++)
        {
            var placement = flow.Tags[index];
            if (placement.Y >= content.Height)
            {
                continue;
            }

            RenderTextSegment(
                canvas,
                content.X + placement.X,
                content.Y + placement.Y,
                placement.Text,
                placement.Style,
                placement.Width);
        }

        for (var index = 0; index < flow.TextRuns.Count; index++)
        {
            var placement = flow.TextRuns[index];
            if (placement.Y >= content.Height)
            {
                continue;
            }

            RenderTextSegment(
                canvas,
                content.X + placement.X,
                content.Y + placement.Y,
                placement.Text,
                placement.Style,
                placement.Width);
        }
    }

    private FlowLayoutResult BuildFlowLayout(int width)
    {
        width = Math.Max(MinimumInlineWidth, width);
        var result = new FlowLayoutResult();
        var cursorX = 0;
        var cursorY = 0;

        for (var index = 0; index < _tags.Count; index++)
        {
            var token = BuildTagToken(_tags[index]);
            var tokenWidth = Math.Max(1, ControlTextLayout.MeasureDisplayWidth(token));
            if (cursorX > 0 && cursorX + 1 + tokenWidth > width)
            {
                cursorY++;
                cursorX = 0;
            }

            if (cursorX > 0)
            {
                cursorX++;
            }

            result.Tags.Add(new TagPlacement(
                index,
                cursorX,
                cursorY,
                token,
                ResolveTagStyle(index),
                Math.Min(tokenWidth, width)));
            cursorX = Math.Min(width, cursorX + tokenWidth);
        }

        var inputElements = BuildInputElements();
        if (inputElements.Count > 0 && cursorX > 0)
        {
            var remainingWidth = width - cursorX - 1;
            if (cursorX + 1 > width || remainingWidth < ResolveMinimumInputStartWidth(inputElements))
            {
                cursorY++;
                cursorX = 0;
            }
            else
            {
                cursorX++;
            }
        }

        for (var index = 0; index < inputElements.Count; index++)
        {
            var element = inputElements[index];
            if (cursorX > 0 && cursorX + element.Width > width)
            {
                cursorY++;
                cursorX = 0;
            }

            result.TextRuns.Add(new TextPlacement(cursorX, cursorY, element.Text, element.Style, element.Width));
            cursorX += element.Width;
            if (cursorX >= width)
            {
                cursorY++;
                cursorX = 0;
            }
        }

        result.Height = Math.Max(1, cursorY + (cursorX > 0 || result.TextRuns.Count == 0 && result.Tags.Count == 0 ? 1 : 0));
        return result;
    }

    private int ResolveNaturalContentWidth()
    {
        var width = 0;
        for (var index = 0; index < _tags.Count; index++)
        {
            width += MeasureTagWidth(index);
            if (index > 0)
            {
                width++;
            }
        }

        var inputElements = BuildInputElements();
        if (inputElements.Count > 0 && width > 0)
        {
            width++;
        }

        for (var index = 0; index < inputElements.Count; index++)
        {
            width += inputElements[index].Width;
        }

        return Math.Max(MinimumInlineWidth, width);
    }

    private int ResolveContentWidth(int totalWidth)
    {
        return Math.Max(MinimumInlineWidth, totalWidth - Padding.Horizontal - ResolveBorderChrome());
    }

    private int ResolveBorderChrome()
    {
        return Border == BorderStyle.None ? 0 : 2;
    }

    private List<FlowInlineElement> BuildInputElements()
    {
        var elements = new List<FlowInlineElement>();
        var placeholderVisible = _input.Value.Length == 0;
        var inputStyle = ResolveInputStyle(placeholderVisible);
        var renderCaret = ShowCaret && IsFocused && !IsDisabled && !IsReadOnly;
        var caretGlyph = renderCaret ? ResolveCaretGlyph() : string.Empty;
        var caretStyle = ResolveCaretStyle(inputStyle);
        var inputPadding = Math.Max(0, InputPadding);

        AppendSpaceElements(elements, inputPadding, inputStyle);
        if (placeholderVisible)
        {
            if (!string.IsNullOrEmpty(caretGlyph))
            {
                elements.Add(new FlowInlineElement(caretGlyph, caretStyle, ControlTextLayout.MeasureDisplayWidth(caretGlyph)));
            }

            AppendTextElements(elements, Placeholder, inputStyle);
        }
        else
        {
            var value = _input.Value;
            var cursor = Math.Clamp(_input.Cursor, 0, value.Length);
            var insertedCaret = false;
            var enumerator = StringInfo.GetTextElementEnumerator(value);
            while (enumerator.MoveNext())
            {
                var element = enumerator.GetTextElement();
                var start = enumerator.ElementIndex;
                var end = start + element.Length;
                if (!insertedCaret && !string.IsNullOrEmpty(caretGlyph) && cursor <= start)
                {
                    elements.Add(new FlowInlineElement(caretGlyph, caretStyle, ControlTextLayout.MeasureDisplayWidth(caretGlyph)));
                    insertedCaret = true;
                }

                elements.Add(new FlowInlineElement(element, inputStyle, ControlTextLayout.MeasureDisplayWidth(element)));
                if (!insertedCaret && !string.IsNullOrEmpty(caretGlyph) && cursor == end)
                {
                    elements.Add(new FlowInlineElement(caretGlyph, caretStyle, ControlTextLayout.MeasureDisplayWidth(caretGlyph)));
                    insertedCaret = true;
                }
            }

            if (!insertedCaret && !string.IsNullOrEmpty(caretGlyph))
            {
                elements.Add(new FlowInlineElement(caretGlyph, caretStyle, ControlTextLayout.MeasureDisplayWidth(caretGlyph)));
            }
        }

        AppendSpaceElements(elements, inputPadding, inputStyle);
        return elements;
    }

    private static int ResolveMinimumInputStartWidth(IReadOnlyList<FlowInlineElement> inputElements)
    {
        var width = 0;
        var countedVisibleGlyph = false;
        for (var index = 0; index < inputElements.Count; index++)
        {
            var element = inputElements[index];
            width += element.Width;
            if (!string.IsNullOrWhiteSpace(element.Text))
            {
                countedVisibleGlyph = true;
            }

            if (countedVisibleGlyph && width >= MinimumWrappedInputStartWidth)
            {
                return MinimumWrappedInputStartWidth;
            }
        }

        return Math.Min(MinimumWrappedInputStartWidth, Math.Max(1, width));
    }

    private static void AppendSpaceElements(List<FlowInlineElement> elements, int count, TeaStyle style)
    {
        for (var index = 0; index < count; index++)
        {
            elements.Add(new FlowInlineElement(" ", style, 1));
        }
    }

    private static void AppendTextElements(List<FlowInlineElement> elements, string text, TeaStyle style)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            elements.Add(new FlowInlineElement(element, style, ControlTextLayout.MeasureDisplayWidth(element)));
        }
    }

    private int MeasureTagWidth(int index)
    {
        return ControlTextLayout.MeasureDisplayWidth(BuildTagToken(_tags[index]));
    }

    private string BuildTagToken(string tag)
    {
        var options = Options;
        var tagPadding = Math.Max(0, TagPadding);
        var innerPadding = tagPadding == 0 ? string.Empty : new string(' ', tagPadding);
        return string.Concat(options.TagPrefix, innerPadding, tag, innerPadding, options.TagSuffix);
    }

    private TeaStyle ResolveTagStyle(int tagIndex)
    {
        var style = TagStyle;
        if (tagIndex == _selectedTagIndex)
        {
            style = style.Merge(SelectedTagStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedTagStyle);
            }
        }

        if (tagIndex == _hoveredTagIndex)
        {
            style = style.Merge(HoveredTagStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledTagStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorTagStyle);
        }

        return style;
    }

    private TeaStyle ResolveInputStyle(bool placeholderVisible)
    {
        var style = placeholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledTagStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorTagStyle);
        }

        return style;
    }

    private TeaStyle ResolveCaretStyle(TeaStyle inputStyle)
    {
        var style = CaretStyle.IsEmpty ? inputStyle.Merge(TeaStyle.Empty.WithInverse()) : CaretStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledTagStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorTagStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledTagStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorTagStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        return ApplyStyle(
            FormatTitleText(),
            IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatTitleText()
    {
        return string.IsNullOrEmpty(Title)
            ? string.Empty
            : IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title;
    }

    private string ResolveCaretGlyph()
    {
        if (!string.IsNullOrEmpty(CaretGlyph))
        {
            return FirstTextElement(CaretGlyph);
        }

        return string.Empty;
    }

    private static int RenderTextSegment(Canvas canvas, int x, int y, string text, TeaStyle style, int maxWidth)
    {
        if (maxWidth <= 0)
        {
            return 0;
        }

        var visible = SliceToDisplayWidth(text, maxWidth);
        if (string.IsNullOrEmpty(visible))
        {
            return 0;
        }

        canvas.WriteText(x, y, ApplyStyle(visible, style), maxWidth);
        return ControlTextLayout.MeasureDisplayWidth(visible);
    }

    private static string SliceToDisplayWidth(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text) || maxWidth <= 0)
        {
            return string.Empty;
        }

        if (ControlTextLayout.MeasureDisplayWidth(text) <= maxWidth)
        {
            return text;
        }

        var builder = new System.Text.StringBuilder();
        var width = 0;
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();
            var elementWidth = ControlTextLayout.MeasureDisplayWidth(element);
            if (width + elementWidth > maxWidth)
            {
                break;
            }

            builder.Append(element);
            width += elementWidth;
        }

        return builder.ToString();
    }

    private static string FirstTextElement(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var enumerator = StringInfo.GetTextElementEnumerator(value);
        return enumerator.MoveNext() ? enumerator.GetTextElement() : value[0].ToString();
    }
}
