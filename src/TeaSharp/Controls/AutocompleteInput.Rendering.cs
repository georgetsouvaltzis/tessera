using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class AutocompleteInput
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        RenderInputRow(canvas, content);
        RenderSuggestionRows(canvas, content);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(Title) + 6) + Padding.Horizontal;
        var height = 1 + Padding.Vertical + ResolveVisibleSuggestionCount(Math.Max(0, availableBounds.Height - Padding.Vertical - 1));
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderInputRow(Canvas canvas, Rect content)
    {
        if (content.Height < 1)
        {
            return;
        }

        var commitMarker = ResolveCommitMarker();
        var commitWidth = ControlTextLayout.MeasureDisplayWidth(commitMarker);
        var textWidth = Math.Max(1, content.Width - commitWidth);
        var frame = _input.BuildFrame(textWidth);
        var textStyle = frame.PlaceholderVisible ? PlaceholderTextStyle : InputTextStyle;

        canvas.WriteText(content.X, content.Y, ApplyStyle(frame.Text, ResolveDisabledStyle(textStyle)), textWidth);
        if (commitWidth > 0)
        {
            canvas.WriteText(content.X + textWidth, content.Y, ApplyStyle(commitMarker, ResolveDisabledStyle(CommitMarkerStyle)), commitWidth);
        }
    }

    private void RenderSuggestionRows(Canvas canvas, Rect content)
    {
        var visibleSuggestions = ResolveVisibleSuggestionCount(content.Height);
        if (visibleSuggestions <= 0)
        {
            return;
        }

        var suggestionMarkerWidth = ControlTextLayout.MeasureDisplayWidth(Glyphs.SuggestionMarker);
        var separatorWidth = ControlTextLayout.MeasureDisplayWidth(Glyphs.MarkerSeparator);
        var prefixWidth = suggestionMarkerWidth + separatorWidth;
        var emptyPrefix = new string(' ', prefixWidth);

        for (var row = 0; row < visibleSuggestions; row++)
        {
            var suggestion = _suggestions[_filteredSuggestionIndices[row]];
            var y = content.Y + 1 + row;
            var selected = row == _selectedSuggestionIndex;
            var hovered = row == _hoveredSuggestionIndex;
            var prefix = selected
                ? string.Concat(Glyphs.SuggestionMarker, Glyphs.MarkerSeparator)
                : emptyPrefix;
            var line = string.Concat(prefix, suggestion);

            var style = ResolveSuggestionStyle(row, hovered, selected);
            canvas.WriteText(content.X, y, ApplyStyle(line, style), content.Width);
        }
    }

    private int ResolveVisibleSuggestionCount(int contentHeight)
    {
        if (contentHeight <= 1 || !IsPopupVisible)
        {
            return 0;
        }

        var availableRows = contentHeight - 1;
        return Math.Min(Math.Max(0, MaxVisibleSuggestions), Math.Min(availableRows, _filteredSuggestionIndices.Count));
    }

    private string ResolveTitleText()
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

    private string RenderTitle()
    {
        return ApplyStyle(ResolveTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return ResolveDisabledStyle(style);
    }

    private TeaStyle ResolveSuggestionStyle(int index, bool hovered, bool selected)
    {
        var style = PopupStyle.Merge(SuggestionStyle);
        if (hovered)
        {
            style = style.Merge(HoveredSuggestionStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedSuggestionStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedSuggestionStyle);
            }
        }

        return ResolveDisabledStyle(style);
    }

    private TeaStyle ResolveDisabledStyle(TeaStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private string ResolveCommitMarker()
    {
        return IsPopupVisible && !string.IsNullOrEmpty(Glyphs.CommitMarker)
            ? Glyphs.CommitMarker
            : string.Empty;
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
