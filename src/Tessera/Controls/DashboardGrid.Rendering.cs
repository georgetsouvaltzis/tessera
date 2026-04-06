using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class DashboardGrid
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = Border == BorderStyle.None
            ? clipped.Inset(Padding)
            : FrameLayout.DrawFrameAndResolveContent(
                canvas,
                clipped,
                RenderTitle(),
                Border,
                Padding,
                ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_tiles.Count == 0)
        {
            var emptyStyle = ResolveTileStyle(-1);
            canvas.WriteText(content.X, content.Y, ApplyStyle("(no tiles)", emptyStyle), content.Width);
            return;
        }

        for (var index = 0; index < _tiles.Count; index++)
        {
            var tileRect = ResolveTileRect(content, _tiles[index]);
            if (tileRect.IsEmpty)
            {
                continue;
            }

            RenderTile(canvas, tileRect, _tiles[index], index);
        }
    }

    private void RenderTile(Canvas canvas, Rect tileRect, DashboardTile tile, int tileIndex)
    {
        var tileStyle = ResolveTileStyle(tileIndex);
        var tileContent = TileBorder == BorderStyle.None
            ? tileRect
            : FrameLayout.DrawFrameAndResolveContent(canvas, tileRect, null, TileBorder, Thickness.None, tileStyle);
        if (tileContent.IsEmpty)
        {
            return;
        }

        canvas.WriteText(tileContent.X, tileContent.Y, ApplyStyle(tile.Title, tileStyle), tileContent.Width);
        if (!string.IsNullOrWhiteSpace(tile.Subtitle) && tileContent.Height > 1)
        {
            canvas.WriteText(tileContent.X, tileContent.Y + 1, ApplyStyle(tile.Subtitle, tileStyle), tileContent.Width);
        }
    }

    private int HitTest(Rect content, int x, int y)
    {
        if (_tiles.Count == 0 || !content.Contains(x, y))
        {
            return -1;
        }

        for (var index = 0; index < _tiles.Count; index++)
        {
            var tileRect = ResolveTileRect(content, _tiles[index]);
            if (tileRect.Contains(x, y))
            {
                return index;
            }
        }

        return -1;
    }

    private Rect ResolveTileRect(Rect content, DashboardTile tile)
    {
        var columnCount = ResolveColumnCount();
        var rowCount = ResolveRowCount();
        if (columnCount <= 0 || rowCount <= 0)
        {
            return new Rect(0, 0, 0, 0);
        }

        var left = content.X + ScaleOffset(content.Width, tile.Column, columnCount);
        var right = content.X + ScaleOffset(content.Width, tile.Column + tile.ColumnSpan, columnCount);
        var top = content.Y + ScaleOffset(content.Height, tile.Row, rowCount);
        var bottom = content.Y + ScaleOffset(content.Height, tile.Row + tile.RowSpan, rowCount);
        var width = right - left;
        var height = bottom - top;
        if (width <= 0 || height <= 0)
        {
            return new Rect(0, 0, 0, 0);
        }

        return new Rect(left, top, width, height);
    }

    private int ResolveColumnCount()
    {
        var columns = 1;
        for (var index = 0; index < _tiles.Count; index++)
        {
            var tile = _tiles[index];
            columns = Math.Max(columns, tile.Column + tile.ColumnSpan);
        }

        return columns;
    }

    private int ResolveRowCount()
    {
        var rows = 1;
        for (var index = 0; index < _tiles.Count; index++)
        {
            var tile = _tiles[index];
            rows = Math.Max(rows, tile.Row + tile.RowSpan);
        }

        return rows;
    }

    private static int ScaleOffset(int length, int value, int total)
    {
        return (int)((long)Math.Clamp(value, 0, total) * length / total);
    }

    private string RenderTitle()
    {
        var title = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            title = string.Concat(title, " ", FocusMarker);
        }

        var style = IsFocused ? FocusedTitleStyleText : TitleStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledTileStyleText);
        }

        return ApplyStyle(title, style);
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
            style = style.Merge(DisabledTileStyleText);
        }

        return style;
    }

    private TesseraStyle ResolveTileStyle(int tileIndex)
    {
        var style = TileStyleText;
        if (tileIndex == _selectedIndex)
        {
            style = style.Merge(SelectedTileStyleText);
        }

        if (tileIndex == _hoveredIndex)
        {
            style = style.Merge(HoveredTileStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledTileStyleText);
        }

        return style;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var columns = Math.Max(1, ResolveColumnCount());
        var rows = Math.Max(1, ResolveRowCount());
        var width = Math.Max(24, columns * 12);
        var height = Math.Max(6, rows * 4);
        width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(Title) + 6);
        width += Padding.Horizontal;
        height += Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
}
