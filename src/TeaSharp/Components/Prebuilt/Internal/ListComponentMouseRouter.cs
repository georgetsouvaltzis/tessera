using TeaSharp.Components.UiKit;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt.Internal;

internal static class ListComponentMouseRouter
{
    public static bool Update<T>(
        MouseMsg message,
        Rect bounds,
        bool showBorder,
        ListModel<T> model,
        Func<int?, bool> setHoveredFilteredIndex)
    {
        var content = showBorder ? bounds.Inset(1, 1) : bounds;
        if (content.IsEmpty)
        {
            return false;
        }

        if (!content.Contains(message.X, message.Y) && message is not MouseWheelMsg)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                return setHoveredFilteredIndex(null);
            }

            return false;
        }

        model.PageSize = Math.Max(1, content.Height);
        var hoverChanged = message is MouseMotionMsg or MouseClickMsg
            ? SetHoveredByPointer(message.X, message.Y, content, model, setHoveredFilteredIndex)
            : false;

        if (message is MouseMotionMsg)
        {
            return hoverChanged;
        }

        if (message is MouseWheelMsg wheel)
        {
            return hoverChanged | model.Update(wheel);
        }

        if (message is not MouseClickMsg { Button: MouseButton.Left } click || !content.Contains(click.X, click.Y))
        {
            return false;
        }

        var row = click.Y - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return false;
        }

        var visibleRows = model.VisibleRows();
        if (row >= visibleRows.Count)
        {
            return false;
        }

        return hoverChanged | model.SelectFilteredIndex(visibleRows[row].Index);
    }

    private static bool SetHoveredByPointer<T>(
        int x,
        int y,
        Rect content,
        ListModel<T> model,
        Func<int?, bool> setHoveredFilteredIndex)
    {
        if (!content.Contains(x, y))
        {
            return setHoveredFilteredIndex(null);
        }

        var row = y - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return setHoveredFilteredIndex(null);
        }

        var rows = model.VisibleRows();
        if (row >= rows.Count)
        {
            return setHoveredFilteredIndex(null);
        }

        return setHoveredFilteredIndex(rows[row].Index);
    }
}
