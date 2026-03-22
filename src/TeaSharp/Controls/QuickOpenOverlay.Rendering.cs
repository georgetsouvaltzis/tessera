using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class QuickOpenOverlay
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        if (!IsOpen)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (!TryResolveOverlay(clipped, out var overlay, out var content))
        {
            return;
        }

        var title = BorderStyle == BorderStyle.None ? null : RenderTitle();
        content = FrameLayout.DrawFrameAndResolveContent(canvas, overlay, title, BorderStyle, Padding, ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        RenderQuery(canvas, content);
        if (content.Height <= 1)
        {
            return;
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ApplyStyle(EmptyText, ResolveRowStyle(-1)), content.Width);
            return;
        }

        if (_filteredIndices.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ApplyStyle(NoMatchesText, ResolveRowStyle(-1)), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = ComputeWindowStart(_selectedFilteredIndex, visibleRows, _filteredIndices.Count);
        var end = Math.Min(_filteredIndices.Count, start + visibleRows);
        var row = 0;
        for (var filteredIndex = start; filteredIndex < end; filteredIndex++, row++)
        {
            var item = _items[_filteredIndices[filteredIndex]];
            var rowStyle = ResolveRowStyle(filteredIndex);
            var marker = ResolveRowMarker(filteredIndex);
            var hasQuery = _query.Length > 0;
            var matchMarker = hasQuery
                ? ApplyStyle(_glyphs.MatchMarker, MatchMarkerStyle.Merge(rowStyle))
                : string.Empty;
            var summary = BuildSummary(item);
            var line = hasQuery
                ? string.Concat(marker, _glyphs.MarkerSeparator, matchMarker, _glyphs.MarkerSeparator, ApplyStyle(summary, rowStyle))
                : string.Concat(marker, _glyphs.MarkerSeparator, ApplyStyle(summary, rowStyle));
            canvas.WriteText(content.X, content.Y + 1 + row, line, content.Width);
        }
    }

    private void RenderQuery(Canvas canvas, Rect content)
    {
        var prompt = string.Concat(_glyphs.QueryPrompt, _glyphs.MarkerSeparator);
        var visibleText = string.IsNullOrEmpty(_query) ? Placeholder : _query;
        var queryStyle = string.IsNullOrEmpty(_query) ? PlaceholderStyle : QueryTextStyle;
        if (IsDisabled)
        {
            queryStyle = queryStyle.Merge(DisabledStyle);
        }

        var available = Math.Max(1, content.Width - ControlTextLayout.MeasureDisplayWidth(prompt));
        var text = ClipToWidth(visibleText, available);
        canvas.WriteText(content.X, content.Y, string.Concat(prompt, ApplyStyle(text, queryStyle)), content.Width);
    }

    private string RenderTitle()
    {
        var raw = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            raw = string.Concat(raw, " ", FocusMarker);
        }

        if (string.IsNullOrEmpty(raw))
        {
            return raw;
        }

        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return ApplyStyle(raw, style);
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TeaStyle ResolveRowStyle(int filteredIndex)
    {
        var style = ItemStyle;
        if (filteredIndex == _selectedFilteredIndex)
        {
            style = style.Merge(SelectedItemStyle);
        }

        if (filteredIndex == _hoveredFilteredIndex)
        {
            style = style.Merge(HoveredItemStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private string ResolveRowMarker(int filteredIndex)
    {
        if (filteredIndex == _selectedFilteredIndex)
        {
            return _glyphs.SelectedRowMarker;
        }

        if (filteredIndex == _hoveredFilteredIndex)
        {
            return _glyphs.HoveredRowMarker;
        }

        return _glyphs.NormalRowMarker;
    }

    private static string ClipToWidth(string text, int width)
    {
        if (width <= 0 || string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (text.Length <= width)
        {
            return text;
        }

        return text[..width];
    }
}
