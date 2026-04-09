using System.Text;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class FuzzyFinder
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
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var queryFrame = _query.BuildFrame(Math.Max(1, content.Width - 2));
        var queryText = queryFrame.PlaceholderVisible
            ? ApplyStyle(queryFrame.Text, PlaceholderTextStyle)
            : ApplyStyle(queryFrame.Text, ValueTextStyle);
        canvas.WriteText(content.X, content.Y, $"? {queryText}", content.Width);

        if (!IsOpen || content.Height < 2)
        {
            return;
        }

        var visibleCount = ResolveVisibleResultCount(content.Height);
        EnsureScrollVisible(visibleCount);
        if (_results.Count == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ApplyStyle("(no matches)", ListItemStyle), content.Width);
            return;
        }

        for (var row = 0; row < visibleCount; row++)
        {
            var index = _scrollOffset + row;
            if (index < 0 || index >= _results.Count)
            {
                break;
            }

            var result = _results[index];
            var item = _items[result.ItemIndex];
            var label = BuildDisplayLabel(item);
            if (!MatchHighlightStyle.IsEmpty && result.MatchIndices.Length > 0)
            {
                label = HighlightMatch(label, result.MatchIndices);
            }

            var rowStyle = ListItemStyle;
            if (index == _hoveredIndex)
            {
                rowStyle = rowStyle.Merge(HoveredItemStyle);
            }

            if (index == _selectedIndex)
            {
                rowStyle = rowStyle.Merge(SelectedItemStyle);
            }

            var marker = index == _selectedIndex ? ">" : " ";
            canvas.WriteText(content.X, content.Y + 1 + row, ApplyStyle($"{marker} {label}", rowStyle), content.Width);
        }
    }

    private string HighlightMatch(string text, int[] indices)
    {
        if (indices.Length == 0 || MatchHighlightStyle.IsEmpty || string.IsNullOrEmpty(text))
        {
            return text;
        }

        var buffer = new StringBuilder(text.Length * 2);
        var matchCursor = 0;
        for (var index = 0; index < text.Length; index++)
        {
            while (matchCursor < indices.Length && indices[matchCursor] < index)
            {
                matchCursor++;
            }

            var value = text[index].ToString();
            if (matchCursor < indices.Length && indices[matchCursor] == index)
            {
                buffer.Append(MatchHighlightStyle.Render(value));
                matchCursor++;
            }
            else
            {
                buffer.Append(value);
            }
        }

        return buffer.ToString();
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
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
            style = style.Merge(PlaceholderTextStyle);
        }

        return style;
    }
}
