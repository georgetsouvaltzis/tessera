using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.UiKit;
namespace TeaSharp.Components.UiKit.Internal;

internal static class SortableTablePointerHelper
{
    public static Rect ResolveContentRect(Rect bounds, BorderStyle border, Thickness padding, string title)
    {
        if (border != BorderStyle.None)
        {
            return FrameLayout.ResolveContentRect(bounds, border, padding);
        }

        var content = bounds.Inset(padding);
        if (!string.IsNullOrWhiteSpace(title))
        {
            content = new Rect(content.X, content.Y + 1, content.Width, Math.Max(0, content.Height - 1));
        }

        return content;
    }

    public static int HeaderColumnFromPointer(int x, Rect content, int columnCount)
    {
        var separatorCount = columnCount - 1;
        var availableWidth = Math.Max(columnCount, content.Width - separatorCount);
        var widths = ComputeColumnWidths(availableWidth, columnCount);

        var cursor = content.X;
        for (var i = 0; i < widths.Length; i++)
        {
            var end = cursor + widths[i];
            if (x >= cursor && x < end)
            {
                return i;
            }

            cursor = end;
            if (i < widths.Length - 1)
            {
                cursor++;
            }
        }

        return -1;
    }

    public static int RowFromPointer(Rect content, int y, int visibleRows)
    {
        var row = y - (content.Y + 2);
        return row < 0 || row >= visibleRows ? -1 : row;
    }

    public static (int Hovered, int Selected) NormalizeVisibleRowPointers(int hovered, int selected, int visibleRows)
    {
        if (visibleRows <= 0)
        {
            return (-1, -1);
        }

        if (hovered >= visibleRows)
        {
            hovered = visibleRows - 1;
        }

        if (selected >= visibleRows)
        {
            selected = visibleRows - 1;
        }

        return (hovered, selected);
    }

    private static int[] ComputeColumnWidths(int width, int columns)
    {
        var widths = new int[columns];
        var baseWidth = width / columns;
        var remainder = width % columns;
        for (var i = 0; i < columns; i++)
        {
            widths[i] = baseWidth + (i < remainder ? 1 : 0);
        }

        return widths;
    }
}
