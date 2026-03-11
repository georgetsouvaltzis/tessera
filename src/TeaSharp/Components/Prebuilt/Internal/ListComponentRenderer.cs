using TeaSharp.Components.UiKit;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt.Internal;

internal static class ListComponentRenderer
{
    public static void Render<T>(
        Canvas canvas,
        Rect rect,
        ListModel<T> model,
        string title,
        bool focused,
        bool disabled,
        bool readOnly,
        BorderStyle border,
        Thickness padding,
        int? hoveredFilteredIndex,
        WidgetStatePalette itemStatePalette,
        Func<T, IReadOnlyCollection<WidgetVisualState>?>? itemStateResolver)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            border == BorderStyle.None ? null : focused ? $"{title} *" : title,
            border,
            padding);

        if (content.IsEmpty)
        {
            return;
        }

        model.PageSize = Math.Max(1, content.Height);
        var rows = model.VisibleRows();
        if (rows.Count == 0 && content.Height > 0)
        {
            var emptyStates = ListComponentStateResolver.ResolveBaseStates(focused, disabled, readOnly, isEmpty: true);
            canvas.WriteText(content.X, content.Y, itemStatePalette.Render("(empty)", emptyStates), content.Width);
            return;
        }

        for (var row = 0; row < rows.Count && row < content.Height; row++)
        {
            var visible = rows[row];
            var marker = visible.Selected
                ? "›"
                : hoveredFilteredIndex == visible.Index ? "▸" : " ";
            var text = $"{marker} {model.LabelFor(visible.Item)}";
            var states = ListComponentStateResolver.ResolveRowStates(
                visible,
                focused,
                disabled,
                readOnly,
                hoveredFilteredIndex,
                itemStateResolver);
            canvas.WriteText(content.X, content.Y + row, itemStatePalette.Render(text, states), content.Width);
        }
    }
}
