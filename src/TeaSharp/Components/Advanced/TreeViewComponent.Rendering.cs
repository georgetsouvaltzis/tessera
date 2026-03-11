using TeaSharp.Components.Advanced.Internal;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Advanced;

public sealed partial class TreeViewComponent
{
    private Rect ResolveRenderContentRect(Canvas canvas, Rect clipped)
    {
        return FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);
    }

    private void RenderVisibleNodes(Canvas canvas, Rect content)
    {
        if (_visible.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, NodeStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_visible.Count, start + content.Height);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var (node, depth, _) = _visible[i];
            var indent = new string(' ', Math.Max(0, depth) * 2);
            var marker = node.Children.Count == 0
                ? "•"
                : node.Expanded ? "▾" : "▸";
            var cursor = i == _selectedIndex ? ">" : " ";
            var states = ResolveNodeStates(node, i == _selectedIndex, i == _hoveredIndex);
            canvas.WriteText(content.X, content.Y + row, NodeStatePalette.Render($"{cursor} {indent}{marker} {node.Label}", states), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveNodeStates(TreeItemNode node, bool selected, bool hovered)
    {
        var states = new List<WidgetVisualState>(6);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (IsDisabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (IsReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (selected)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        states.AddRange(node.States);
        return states;
    }
}
