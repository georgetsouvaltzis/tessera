using System.Text;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

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

        var set = new HashSet<int>(indices);
        var buffer = new StringBuilder(text.Length * 2);
        for (var index = 0; index < text.Length; index++)
        {
            var value = text[index].ToString();
            buffer.Append(set.Contains(index) ? MatchHighlightStyle.Render(value) : value);
        }

        return buffer.ToString();
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
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
            style = style.Merge(PlaceholderTextStyle);
        }

        return style;
    }
}
