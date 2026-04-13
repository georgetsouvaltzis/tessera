using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class TokenEditor
{
    /// <inheritdoc />
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
        var glyphs = ResolveGlyphs();
        var markerSeparatorWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.MarkerSeparator);
        var prefixWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.TokenPrefix);
        var suffixWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.TokenSuffix);
        var selectedMarkerWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.SelectedMarker);
        var unselectedMarkerWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.UnselectedMarker);
        var tokenSeparatorWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.TokenSeparator);

        var width = Math.Max(20, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
        for (var index = 0; index < _tokens.Count; index++)
        {
            var markerWidth = index == SelectedTokenIndex ? selectedMarkerWidth : unselectedMarkerWidth;
            width += markerSeparatorWidth + prefixWidth + suffixWidth + markerWidth;
            width += ControlTextLayout.MeasureDisplayWidth(_tokens[index].Value);
            if (index < _tokens.Count - 1)
            {
                width += tokenSeparatorWidth;
            }
        }

        width += 8;
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
        var glyphs = ResolveGlyphs();
        var markerSeparator = glyphs.MarkerSeparator;
        var tokenPrefix = glyphs.TokenPrefix;
        var tokenSuffix = glyphs.TokenSuffix;
        var tokenSeparator = glyphs.TokenSeparator;

        var markerSeparatorWidth = ControlTextLayout.MeasureDisplayWidth(markerSeparator);
        var tokenPrefixWidth = ControlTextLayout.MeasureDisplayWidth(tokenPrefix);
        var tokenSuffixWidth = ControlTextLayout.MeasureDisplayWidth(tokenSuffix);
        var selectedMarkerWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.SelectedMarker);
        var unselectedMarkerWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.UnselectedMarker);
        var tokenSeparatorWidth = ControlTextLayout.MeasureDisplayWidth(tokenSeparator);

        var x = content.X;
        var y = content.Y;
        var right = content.Right;

        for (var index = 0; index < _tokens.Count && x < right; index++)
        {
            var token = _tokens[index];
            var marker = index == SelectedTokenIndex ? glyphs.SelectedMarker : glyphs.UnselectedMarker;
            var markerWidth = index == SelectedTokenIndex ? selectedMarkerWidth : unselectedMarkerWidth;
            var valueWidth = ControlTextLayout.MeasureDisplayWidth(token.Value);
            var tokenWidth = markerWidth + markerSeparatorWidth + tokenPrefixWidth + valueWidth + tokenSuffixWidth;
            if (tokenWidth <= 0 || x + tokenWidth > right)
            {
                break;
            }

            var style = ResolveTokenStyle(index);
            if (style.IsEmpty)
            {
                x = WriteSegment(canvas, x, y, marker, markerWidth, right);
                x = WriteSegment(canvas, x, y, markerSeparator, markerSeparatorWidth, right);
                x = WriteSegment(canvas, x, y, tokenPrefix, tokenPrefixWidth, right);
                x = WriteSegment(canvas, x, y, token.Value, valueWidth, right);
                x = WriteSegment(canvas, x, y, tokenSuffix, tokenSuffixWidth, right);
            }
            else
            {
                var tokenText = BuildTokenText(marker, markerSeparator, tokenPrefix, token.Value, tokenSuffix);
                canvas.WriteText(x, y, style.Render(tokenText), right - x);
                x += tokenWidth;
            }

            if (index < _tokens.Count - 1 && x < right)
            {
                if (style.IsEmpty)
                {
                    x = WriteSegment(canvas, x, y, tokenSeparator, tokenSeparatorWidth, right);
                }
                else
                {
                    canvas.WriteText(x, y, style.Render(tokenSeparator), right - x);
                    x += tokenSeparatorWidth;
                }
            }
        }

        var inputWidth = right - x;
        if (inputWidth <= 0)
        {
            return;
        }

        var frame = _input.BuildFrame(inputWidth);
        var inputStyle = frame.PlaceholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        if (IsDisabled)
        {
            inputStyle = inputStyle.Merge(DisabledTokenStyle);
        }

        canvas.WriteText(x, y, ApplyStyle(frame.Text, inputStyle), inputWidth);
    }

    private TesseraStyle ResolveTokenStyle(int index)
    {
        var style = TokenStyle;
        if (index == SelectedTokenIndex)
        {
            style = style.Merge(SelectedTokenStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedTokenStyle);
            }
        }

        if (index == _hoveredTokenIndex)
        {
            style = style.Merge(HoveredTokenStyle);
        }

        if (_tokens[index].IsDisabled || IsDisabled)
        {
            style = style.Merge(DisabledTokenStyle);
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

        if (IsDisabled)
        {
            style = style.Merge(DisabledTokenStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(FormatTitleText(), style);
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

    private static string BuildTokenText(
        string marker,
        string markerSeparator,
        string prefix,
        string value,
        string suffix)
    {
        return string.Concat(marker, markerSeparator, string.Concat(prefix, value, suffix));
    }

    private static int WriteSegment(Canvas canvas, int x, int y, string text, int displayWidth, int right)
    {
        if (displayWidth <= 0 || x >= right)
        {
            return x;
        }

        canvas.WriteText(x, y, text, right - x);
        return x + displayWidth;
    }

    private TokenEditorGlyphSet ResolveGlyphs()
    {
        var glyphs = Glyphs;
        var selectedMarker = string.IsNullOrEmpty(glyphs.SelectedMarker) ? "●" : glyphs.SelectedMarker;
        var unselectedMarker = string.IsNullOrEmpty(glyphs.UnselectedMarker) ? "○" : glyphs.UnselectedMarker;
        var prefix = glyphs.TokenPrefix;
        var suffix = glyphs.TokenSuffix;
        var markerSeparator = glyphs.MarkerSeparator;
        var tokenSeparator = glyphs.TokenSeparator;
        return new TokenEditorGlyphSet(selectedMarker, unselectedMarker, prefix, suffix, markerSeparator,
            tokenSeparator);
    }
}
