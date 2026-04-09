
namespace Tessera.Core.Rendering.Internal;

internal static class AnsiFrameDiffer
{
    public static async Task WriteAsync(
        StreamWriter writer,
        RenderFrameBuffer previousFrame,
        RenderFrameBuffer nextFrame,
        int width,
        int height)
    {
        var rowCount = Math.Max(previousFrame.RowCount, nextFrame.RowCount);
        if (height > 0 && rowCount > height)
        {
            rowCount = height;
        }

        for (var row = 0; row < rowCount; row++)
        {
            if (nextFrame.RowEquals(previousFrame, row))
            {
                continue;
            }

            await WriteRowDiffAsync(writer, previousFrame, nextFrame, row, width).ConfigureAwait(false);
        }
    }

    private static async Task WriteRowDiffAsync(
        StreamWriter writer,
        RenderFrameBuffer previousFrame,
        RenderFrameBuffer nextFrame,
        int row,
        int width)
    {
        var max = Math.Max(previousFrame.ColumnCountAt(row), nextFrame.ColumnCountAt(row));
        if (width > 0 && max > width)
        {
            max = width;
        }

        var runStart = -1;
        for (var column = 0; column < max; column++)
        {
            var changed = !nextFrame.CellEquals(previousFrame, row, column);

            if (changed && runStart < 0)
            {
                runStart = column;
                continue;
            }

            if (!changed && runStart >= 0)
            {
                await WriteRunAsync(writer, row, runStart, column, nextFrame).ConfigureAwait(false);
                runStart = -1;
            }
        }

        if (runStart >= 0)
        {
            await WriteRunAsync(writer, row, runStart, max, nextFrame).ConfigureAwait(false);
        }
    }

    private static async Task WriteRunAsync(
        StreamWriter writer,
        int row,
        int startColumn,
        int endColumn,
        RenderFrameBuffer nextFrame)
    {
        await writer.WriteAsync($"\u001b[{row + 1};{startColumn + 1}H").ConfigureAwait(false);
        var activeStyle = string.Empty;

        for (var column = startColumn; column < endColumn;)
        {
            var cell = nextFrame.CellAt(row, column);
            if (cell is null)
            {
                await writer.WriteAsync(" ").ConfigureAwait(false);
                column++;
                continue;
            }

            var nextStyle = nextFrame.StyleAt(row, column);
            if (!string.Equals(activeStyle, nextStyle, StringComparison.Ordinal))
            {
                if (activeStyle.Length > 0)
                {
                    await writer.WriteAsync("\u001b[0m").ConfigureAwait(false);
                }

                if (nextStyle.Length > 0)
                {
                    await writer.WriteAsync(nextStyle).ConfigureAwait(false);
                }

                activeStyle = nextStyle;
            }

            var cellWidth = nextFrame.CellWidthAt(row, column);
            if (cellWidth == 2 && column + 1 >= endColumn)
            {
                await writer.WriteAsync(" ").ConfigureAwait(false);
                column++;
                continue;
            }

            await writer.WriteAsync(cell).ConfigureAwait(false);
            column += cellWidth;
        }

        if (activeStyle.Length > 0)
        {
            await writer.WriteAsync("\u001b[0m").ConfigureAwait(false);
        }
    }
}
