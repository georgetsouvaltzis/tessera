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
    private const int MinimumVisibleInputWidth = 8;
    private const string OverflowIndicator = "…";

    private readonly record struct InlineFlowLayout(int VisibleTagCount, bool ShowOverflow, int InputX, int InputWidth);

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

        RenderSingleLine(canvas, content);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
        for (var index = 0; index < _tags.Count; index++)
        {
            width += MeasureTagWidth(index);
            if (index > 0)
            {
                width++;
            }
        }

        width += Math.Max(MinimumVisibleInputWidth, ControlTextLayout.MeasureDisplayWidth(_input.Value.Length == 0 ? Placeholder : _input.Value));
        width += Math.Max(0, InputPadding) * 2;
        width += Padding.Horizontal;
        if (Border != BorderStyle.None)
        {
            width += 2;
        }

        var height = Padding.Vertical + 1;
        if (Border != BorderStyle.None)
        {
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderSingleLine(Canvas canvas, Rect content)
    {
        var layout = ResolveInlineFlowLayout(content.Width);
        RenderTags(canvas, content.X, content.Y, layout);
        RenderInput(canvas, content.X + layout.InputX, content.Y, layout.InputWidth);
    }

    private void RenderTags(Canvas canvas, int x, int y, InlineFlowLayout layout)
    {
        if ((layout.VisibleTagCount == 0 && !layout.ShowOverflow) || _tags.Count == 0)
        {
            return;
        }

        var cursor = x;
        for (var index = 0; index < layout.VisibleTagCount; index++)
        {
            if (index > 0)
            {
                canvas.Set(cursor, y, ' ');
                cursor++;
            }

            var token = BuildTagToken(_tags[index]);
            cursor += RenderTextSegment(canvas, cursor, y, token, ResolveTagStyle(index), layout.InputX - (cursor - x));
        }

        if (layout.ShowOverflow)
        {
            if (cursor > x)
            {
                canvas.Set(cursor, y, ' ');
                cursor++;
            }

            cursor += RenderTextSegment(canvas, cursor, y, OverflowIndicator, ResolveBorderStyleText(), 1);
        }

        if (layout.InputX > 0 && cursor < x + layout.InputX)
        {
            canvas.Set(cursor, y, ' ');
        }
    }

    private void RenderInput(Canvas canvas, int x, int y, int width)
    {
        if (width <= 0)
        {
            return;
        }

        var inputPadding = Math.Max(0, InputPadding);
        var innerWidth = Math.Max(1, width - (inputPadding * 2));
        var frame = _input.BuildFrame(innerWidth);
        var inputStyle = ResolveInputStyle(frame.PlaceholderVisible);
        var inputText = string.Concat(new string(' ', inputPadding), frame.Text, new string(' ', inputPadding));
        RenderTextSegment(canvas, x, y, inputText, inputStyle, width);

        if (!ShowCaret || !IsFocused || IsDisabled || IsReadOnly)
        {
            return;
        }

        var caretX = x + inputPadding + Math.Clamp(frame.CursorColumn, 0, Math.Max(0, innerWidth - 1));
        if (caretX < x || caretX >= x + width)
        {
            return;
        }

        var caretGlyph = ResolveCaretGlyph(frame);
        if (string.IsNullOrEmpty(caretGlyph))
        {
            return;
        }

        RenderTextSegment(canvas, caretX, y, caretGlyph, ResolveCaretStyle(inputStyle), 1);
    }

    private int ResolveVisibleInputAreaWidth(int totalWidth)
    {
        if (totalWidth <= 0)
        {
            return 0;
        }

        var inputPadding = Math.Max(0, InputPadding);
        var maxInputWidth = _tags.Count > 0
            ? Math.Max(1, totalWidth - 1)
            : totalWidth;
        var visibleText = _input.Value.Length == 0 ? Placeholder : _input.Value;
        var desiredInnerWidth = Math.Max(
            1,
            Math.Min(
                maxInputWidth - (inputPadding * 2),
                Math.Max(MinimumVisibleInputWidth, ControlTextLayout.MeasureDisplayWidth(visibleText))));

        return Math.Clamp(desiredInnerWidth + (inputPadding * 2), 1, maxInputWidth);
    }

    private InlineFlowLayout ResolveInlineFlowLayout(int totalWidth)
    {
        if (totalWidth <= 0)
        {
            return default;
        }

        if (_tags.Count == 0)
        {
            return new InlineFlowLayout(0, false, 0, totalWidth);
        }

        var minimumInlineInputWidth = ResolveMinimumInlineInputWidth(totalWidth);
        var visibleTagCount = ResolveVisibleTagCount(totalWidth, minimumInlineInputWidth);
        var prefixWidth = MeasureVisiblePrefixWidth(visibleTagCount);
        var showOverflow = visibleTagCount < _tags.Count
            && ResolveTotalInlineWidth(prefixWidth, hasHiddenTags: true, minimumInlineInputWidth) <= totalWidth;

        var inputX = prefixWidth;
        if (showOverflow)
        {
            if (inputX > 0)
            {
                inputX++;
            }

            inputX += ControlTextLayout.MeasureDisplayWidth(OverflowIndicator);
        }

        if (inputX > 0)
        {
            inputX++;
        }

        inputX = Math.Min(inputX, totalWidth - 1);
        return new InlineFlowLayout(
            visibleTagCount,
            showOverflow,
            inputX,
            Math.Max(1, totalWidth - inputX));
    }

    private int ResolveVisibleTagCount(int totalWidth, int minimumInlineInputWidth)
    {
        var visibleTagCount = 0;
        var prefixWidth = 0;
        for (var index = 0; index < _tags.Count; index++)
        {
            var nextPrefixWidth = prefixWidth + (visibleTagCount > 0 ? 1 : 0) + MeasureTagWidth(index);
            var hasHiddenTags = index < _tags.Count - 1;
            if (ResolveTotalInlineWidth(nextPrefixWidth, hasHiddenTags, minimumInlineInputWidth) > totalWidth)
            {
                break;
            }

            prefixWidth = nextPrefixWidth;
            visibleTagCount++;
        }

        return visibleTagCount;
    }

    private int ResolveMinimumInlineInputWidth(int totalWidth)
    {
        var inputPadding = Math.Max(0, InputPadding);
        return Math.Clamp((inputPadding * 2) + 1, 1, totalWidth);
    }

    private int MeasureVisiblePrefixWidth(int visibleTagCount)
    {
        var width = 0;
        for (var index = 0; index < visibleTagCount; index++)
        {
            width += MeasureTagWidth(index);
            if (index > 0)
            {
                width++;
            }
        }

        return width;
    }

    private static int ResolveTotalInlineWidth(int prefixWidth, bool hasHiddenTags, int minimumInputWidth)
    {
        var width = prefixWidth + minimumInputWidth;
        if (hasHiddenTags)
        {
            width += ControlTextLayout.MeasureDisplayWidth(OverflowIndicator);
            if (prefixWidth > 0)
            {
                width++;
            }
        }

        if (prefixWidth > 0 || hasHiddenTags)
        {
            width++;
        }

        return width;
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

    private string ResolveCaretGlyph(TextInputFrame frame)
    {
        if (!string.IsNullOrEmpty(CaretGlyph))
        {
            return FirstTextElement(CaretGlyph);
        }

        if (frame.Text.Length == 0)
        {
            return string.Empty;
        }

        var cursor = Math.Clamp(frame.CursorColumn, 0, Math.Max(0, frame.Text.Length - 1));
        return frame.Text[cursor].ToString();
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
