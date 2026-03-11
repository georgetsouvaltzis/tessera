using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed partial class ContextMenuComponent
{
    private void RenderMenu(Canvas canvas, Rect menuBounds, Rect content)
    {
        if (ShowBorder)
        {
            canvas.DrawBox(menuBounds, Title, BorderStyle.Rounded);
        }

        if (_items.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ItemStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var i = 0; i < rows; i++)
        {
            var cursor = i == _selectedIndex ? ">" : " ";
            var states = ResolveItemStates(i);
            canvas.WriteText(content.X, content.Y + i, ItemStatePalette.Render($"{cursor} {_items[i].Title}", states), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveItemStates(int index)
    {
        var states = new List<WidgetVisualState>(6);
        if (Focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (Disabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (ReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (index == _selectedIndex)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (index == _hoveredIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        var itemStates = _items[index].States;
        if (itemStates is not null)
        {
            states.AddRange(itemStates);
        }

        return states;
    }

    private bool TryResolveMenuBounds(Rect bounds, out Rect menuBounds, out Rect content)
    {
        menuBounds = default;
        content = default;

        if (bounds.IsEmpty)
        {
            return false;
        }

        var itemWidth = _items.Count == 0
            ? 12
            : Math.Max(12, _items.Max(item => item.Title.Length + 4));
        var width = Math.Min(itemWidth, bounds.Width);
        var height = Math.Min(Math.Max(3, _items.Count + 2), bounds.Height);

        var x = Math.Clamp(AnchorX, bounds.X, Math.Max(bounds.X, bounds.Right - width));
        var y = Math.Clamp(AnchorY, bounds.Y, Math.Max(bounds.Y, bounds.Bottom - height));
        menuBounds = new Rect(x, y, width, height);
        content = ShowBorder
            ? menuBounds.Inset(1, 1)
            : menuBounds;
        return !content.IsEmpty;
    }
}
