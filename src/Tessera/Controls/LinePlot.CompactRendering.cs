using Tessera.Components.Primitives;

namespace Tessera.Controls;

public sealed partial class LinePlot
{
    private static ReadOnlySpan<char> BlockGlyphs => "▁▂▃▄▅▆▇█";
    private const int CompactVirtualWidthPerCell = 4;
    private const int CompactVirtualHeightPerCell = 4;

    private bool TryRenderCompactSeries(
        Canvas canvas,
        Rect plotArea,
        int maxSampleCount,
        int visibleCount,
        int offset,
        double min,
        double max,
        LinePlotRenderMode renderMode)
    {
        if (_series.Count != 1)
        {
            return false;
        }

        var series = _series[0];
        if (!TryResolveSeriesScaleRange(series, maxSampleCount, visibleCount, offset, min, max, out var seriesMin, out var seriesMax))
        {
            return false;
        }

        return renderMode switch
        {
            LinePlotRenderMode.CompactBraille => plotArea.Width >= 2 && plotArea.Height >= 2
                ? RenderCompactBrailleSeries(canvas, plotArea, series, maxSampleCount, visibleCount, offset, seriesMin, seriesMax)
                : RenderCompactBlockSeries(canvas, plotArea, series, maxSampleCount, visibleCount, offset, seriesMin, seriesMax),
            _ => plotArea.Width >= 2 && plotArea.Height >= 2
                ? RenderCompactLineSeries(canvas, plotArea, series, maxSampleCount, visibleCount, offset, seriesMin, seriesMax)
                : RenderCompactBlockSeries(canvas, plotArea, series, maxSampleCount, visibleCount, offset, seriesMin, seriesMax),
        };
    }

    private bool RenderCompactLineSeries(
        Canvas canvas,
        Rect plotArea,
        LineSeries series,
        int maxSampleCount,
        int visibleCount,
        int offset,
        double min,
        double max)
    {
        if (plotArea.Width < 2 || plotArea.Height < 2)
        {
            return RenderCompactBlockSeries(canvas, plotArea, series, maxSampleCount, visibleCount, offset, min, max);
        }

        var topology = new CompactCellTopology[plotArea.Width * plotArea.Height];
        var previousX = -1;
        var previousY = -1;
        var hasPoint = false;
        var virtualWidth = plotArea.Width * CompactVirtualWidthPerCell;
        var virtualHeight = plotArea.Height * CompactVirtualHeightPerCell;

        for (var index = 0; index < visibleCount; index++)
        {
            var globalIndex = offset + index;
            if (!TryGetSeriesValue(series, maxSampleCount, globalIndex, out var value))
            {
                previousX = -1;
                previousY = -1;
                continue;
            }

            hasPoint = true;
            var virtualX = visibleCount <= 1
                ? 0
                : (int)Math.Round(index * (virtualWidth - 1) / (double)(visibleCount - 1), MidpointRounding.AwayFromZero);
            var virtualY = NormalizeVirtualY(value, min, max, virtualHeight);
            if (previousX >= 0)
            {
                RasterizeCompactPolyline(
                    topology,
                    plotArea.Width,
                    plotArea.Height,
                    previousX,
                    previousY,
                    virtualX,
                    virtualY);
            }
            else
            {
                MarkCompactPoint(topology, plotArea.Width, plotArea.Height, virtualX, virtualY);
            }

            previousX = virtualX;
            previousY = virtualY;
        }

        if (!hasPoint)
        {
            return false;
        }

        var style = ResolveStyled(series.Style);
        for (var cellY = 0; cellY < plotArea.Height; cellY++)
        {
            for (var cellX = 0; cellX < plotArea.Width; cellX++)
            {
                var cell = topology[(cellY * plotArea.Width) + cellX];
                if (!cell.HasPath)
                {
                    continue;
                }

                var glyph = ResolveCompactLineGlyph(cell);
                WriteGlyph(canvas, plotArea.X + cellX, plotArea.Y + cellY, glyph, RenderGlyph(glyph, style));
            }
        }

        return true;
    }

    private bool RenderCompactBrailleSeries(
        Canvas canvas,
        Rect plotArea,
        LineSeries series,
        int maxSampleCount,
        int visibleCount,
        int offset,
        double min,
        double max)
    {
        var virtualWidth = Math.Max(1, plotArea.Width * 2);
        var virtualHeight = Math.Max(1, plotArea.Height * 4);
        var masks = new byte[plotArea.Width * plotArea.Height];
        var previousX = -1;
        var previousY = -1;
        var hasPoint = false;

        for (var index = 0; index < visibleCount; index++)
        {
            var globalIndex = offset + index;
            if (!TryGetSeriesValue(series, maxSampleCount, globalIndex, out var value))
            {
                previousX = -1;
                previousY = -1;
                continue;
            }

            hasPoint = true;
            var x = visibleCount <= 1
                ? 0
                : (int)Math.Round(index * (virtualWidth - 1) / (double)(visibleCount - 1), MidpointRounding.AwayFromZero);
            var y = NormalizeVirtualY(value, min, max, virtualHeight);

            if (previousX >= 0)
            {
                RasterizeCompactLine(masks, plotArea.Width, plotArea.Height, previousX, previousY, x, y);
            }
            else
            {
                SetBrailleDot(masks, plotArea.Width, plotArea.Height, x, y);
            }

            previousX = x;
            previousY = y;
        }

        if (!hasPoint)
        {
            return false;
        }

        var style = ResolveStyled(series.Style);
        for (var cellY = 0; cellY < plotArea.Height; cellY++)
        {
            for (var cellX = 0; cellX < plotArea.Width; cellX++)
            {
                var mask = masks[(cellY * plotArea.Width) + cellX];
                if (mask == 0)
                {
                    continue;
                }

                var glyph = (char)(0x2800 + mask);
                WriteGlyph(canvas, plotArea.X + cellX, plotArea.Y + cellY, glyph, RenderGlyph(glyph, style));
            }
        }

        return true;
    }

    private bool RenderCompactBlockSeries(
        Canvas canvas,
        Rect plotArea,
        LineSeries series,
        int maxSampleCount,
        int visibleCount,
        int offset,
        double min,
        double max)
    {
        var hasPoint = false;
        var style = ResolveStyled(series.Style);

        for (var cellX = 0; cellX < plotArea.Width; cellX++)
        {
            var globalIndex = plotArea.Width <= 1
                ? offset
                : offset + (int)Math.Round(cellX * (visibleCount - 1) / (double)Math.Max(1, plotArea.Width - 1), MidpointRounding.AwayFromZero);
            if (!TryGetSeriesValue(series, maxSampleCount, globalIndex, out var value))
            {
                continue;
            }

            hasPoint = true;
            var normalized = NormalizeValue(value, min, max);
            var glyphIndex = Math.Clamp((int)Math.Round(normalized * (BlockGlyphs.Length - 1), MidpointRounding.AwayFromZero), 0, BlockGlyphs.Length - 1);
            var glyph = BlockGlyphs[glyphIndex];
            WriteGlyph(canvas, plotArea.X + cellX, plotArea.Bottom - 1, glyph, RenderGlyph(glyph, style));
        }

        return hasPoint;
    }

    private static int NormalizeVirtualY(double value, double min, double max, int virtualHeight)
    {
        var normalized = NormalizeValue(value, min, max);
        return (virtualHeight - 1) - (int)Math.Round(normalized * (virtualHeight - 1), MidpointRounding.AwayFromZero);
    }

    private static double NormalizeValue(double value, double min, double max)
    {
        var range = max - min;
        if (!double.IsFinite(range) || Math.Abs(range) < double.Epsilon)
        {
            return 0d;
        }

        return Math.Clamp((value - min) / range, 0d, 1d);
    }

    private static void RasterizeCompactPolyline(
        CompactCellTopology[] topology,
        int width,
        int height,
        int x0,
        int y0,
        int x1,
        int y1)
    {
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;
        var currentX = x0;
        var currentY = y0;
        MarkCompactPoint(topology, width, height, currentX, currentY);

        while (currentX != x1 || currentY != y1)
        {
            var nextX = currentX;
            var nextY = currentY;
            var err2 = err * 2;
            if (err2 > -dy)
            {
                err -= dy;
                nextX += sx;
            }

            if (err2 < dx)
            {
                err += dx;
                nextY += sy;
            }

            var stepX = Math.Sign(nextX - currentX);
            var stepY = Math.Sign(nextY - currentY);
            MarkCompactStep(topology, width, height, currentX, currentY, stepX, stepY);
            RecordCompactTransition(topology, width, height, currentX, currentY, nextX, nextY);
            currentX = nextX;
            currentY = nextY;
        }

        MarkCompactPoint(topology, width, height, currentX, currentY);
    }

    private static void MarkCompactPoint(CompactCellTopology[] topology, int width, int height, int virtualX, int virtualY)
    {
        if (!TryResolveCompactCell(width, height, virtualX, virtualY, out var cellX, out var cellY))
        {
            return;
        }

        ref var cell = ref topology[(cellY * width) + cellX];
        cell.HasPath = true;
    }

    private static void MarkCompactStep(CompactCellTopology[] topology, int width, int height, int virtualX, int virtualY, int stepX, int stepY)
    {
        if (!TryResolveCompactCell(width, height, virtualX, virtualY, out var cellX, out var cellY))
        {
            return;
        }

        ref var cell = ref topology[(cellY * width) + cellX];
        cell.HasPath = true;
        cell.RecordDirection(stepX, stepY);
    }

    private static void RecordCompactTransition(
        CompactCellTopology[] topology,
        int width,
        int height,
        int currentX,
        int currentY,
        int nextX,
        int nextY)
    {
        if (!TryResolveCompactCell(width, height, currentX, currentY, out var currentCellX, out var currentCellY)
            || !TryResolveCompactCell(width, height, nextX, nextY, out var nextCellX, out var nextCellY))
        {
            return;
        }

        if (currentCellX == nextCellX && currentCellY == nextCellY)
        {
            return;
        }

        var dx = nextCellX - currentCellX;
        var dy = nextCellY - currentCellY;
        var exitPort = ResolveExitPort(dx, dy);
        var entryPort = ResolveOppositePort(exitPort);

        ref var currentCell = ref topology[(currentCellY * width) + currentCellX];
        ref var nextCell = ref topology[(nextCellY * width) + nextCellX];
        currentCell.HasPath = true;
        nextCell.HasPath = true;
        currentCell.RecordExit(exitPort);
        nextCell.RecordEntry(entryPort);
    }

    private static char ResolveCompactLineGlyph(CompactCellTopology cell)
    {
        if (cell.Entry != CompactPort.None && cell.Exit != CompactPort.None)
        {
            if (TryResolveOrthogonalCornerGlyph(cell.Entry, cell.Exit, out var corner))
            {
                return corner;
            }

            if (TryResolveLinearGlyph(cell.Entry, cell.Exit, out var linear))
            {
                return linear;
            }

            if (TryResolveDiagonalGlyph(cell.Entry, cell.Exit, out var diagonal))
            {
                return diagonal;
            }
        }

        if (cell.DiagonalUpVotes > cell.HorizontalVotes && cell.DiagonalUpVotes >= cell.DiagonalDownVotes)
        {
            return '╱';
        }

        if (cell.DiagonalDownVotes > cell.HorizontalVotes && cell.DiagonalDownVotes > cell.DiagonalUpVotes)
        {
            return '╲';
        }

        if (cell.HorizontalVotes >= cell.VerticalVotes && cell.HorizontalVotes > 0)
        {
            return '─';
        }

        if (cell.VerticalVotes > 0)
        {
            return '│';
        }

        return '•';
    }

    private static void RasterizeCompactLine(byte[] masks, int width, int height, int x0, int y0, int x1, int y1)
    {
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            SetBrailleDot(masks, width, height, x0, y0);
            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            var err2 = err * 2;
            if (err2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (err2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private static void SetBrailleDot(byte[] masks, int width, int height, int virtualX, int virtualY)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        virtualX = Math.Clamp(virtualX, 0, (width * 2) - 1);
        virtualY = Math.Clamp(virtualY, 0, (height * 4) - 1);

        var cellX = virtualX / 2;
        var cellY = virtualY / 4;
        var dotX = virtualX % 2;
        var dotY = virtualY % 4;
        var index = (cellY * width) + cellX;
        masks[index] |= ResolveBrailleMask(dotX, dotY);
    }

    private static byte ResolveBrailleMask(int dotX, int dotY)
    {
        return (dotX, dotY) switch
        {
            (0, 0) => 0x01,
            (0, 1) => 0x02,
            (0, 2) => 0x04,
            (0, 3) => 0x40,
            (1, 0) => 0x08,
            (1, 1) => 0x10,
            (1, 2) => 0x20,
            (1, 3) => 0x80,
            _ => 0x00,
        };
    }

    private static bool TryResolveCompactCell(int width, int height, int virtualX, int virtualY, out int cellX, out int cellY)
    {
        cellX = 0;
        cellY = 0;
        if (width <= 0 || height <= 0)
        {
            return false;
        }

        var maxVirtualX = (width * CompactVirtualWidthPerCell) - 1;
        var maxVirtualY = (height * CompactVirtualHeightPerCell) - 1;
        virtualX = Math.Clamp(virtualX, 0, maxVirtualX);
        virtualY = Math.Clamp(virtualY, 0, maxVirtualY);
        cellX = virtualX / CompactVirtualWidthPerCell;
        cellY = virtualY / CompactVirtualHeightPerCell;
        return true;
    }

    private static CompactPort ResolveExitPort(int dx, int dy)
    {
        return (dx, dy) switch
        {
            (1, 0) => CompactPort.East,
            (-1, 0) => CompactPort.West,
            (0, 1) => CompactPort.South,
            (0, -1) => CompactPort.North,
            (1, 1) => CompactPort.SouthEast,
            (1, -1) => CompactPort.NorthEast,
            (-1, 1) => CompactPort.SouthWest,
            (-1, -1) => CompactPort.NorthWest,
            _ => CompactPort.None,
        };
    }

    private static CompactPort ResolveOppositePort(CompactPort port)
    {
        return port switch
        {
            CompactPort.West => CompactPort.East,
            CompactPort.East => CompactPort.West,
            CompactPort.North => CompactPort.South,
            CompactPort.South => CompactPort.North,
            CompactPort.NorthWest => CompactPort.SouthEast,
            CompactPort.NorthEast => CompactPort.SouthWest,
            CompactPort.SouthWest => CompactPort.NorthEast,
            CompactPort.SouthEast => CompactPort.NorthWest,
            _ => CompactPort.None,
        };
    }

    private static bool TryResolveLinearGlyph(CompactPort entry, CompactPort exit, out char glyph)
    {
        var ports = entry | exit;
        glyph = ports switch
        {
            CompactPort.West or CompactPort.East or CompactPort.West | CompactPort.East => '─',
            CompactPort.North or CompactPort.South or CompactPort.North | CompactPort.South => '│',
            _ => default,
        };
        return glyph != default;
    }

    private static bool TryResolveDiagonalGlyph(CompactPort entry, CompactPort exit, out char glyph)
    {
        var ports = entry | exit;
        glyph = ports switch
        {
            CompactPort.SouthWest | CompactPort.NorthEast => '╱',
            CompactPort.NorthWest | CompactPort.SouthEast => '╲',
            _ => ResolveSideCornerDiagonal(entry, exit),
        };
        return glyph != default;
    }

    private static char ResolveSideCornerDiagonal(CompactPort entry, CompactPort exit)
    {
        var (a, b) = NormalizePortPair(entry, exit);
        return (a, b) switch
        {
            (CompactPort.West, CompactPort.NorthEast) => '╱',
            (CompactPort.SouthWest, CompactPort.East) => '╱',
            (CompactPort.SouthWest, CompactPort.North) => '╱',
            (CompactPort.South, CompactPort.NorthEast) => '╱',
            (CompactPort.West, CompactPort.SouthEast) => '╲',
            (CompactPort.NorthWest, CompactPort.East) => '╲',
            (CompactPort.NorthWest, CompactPort.South) => '╲',
            (CompactPort.North, CompactPort.SouthEast) => '╲',
            _ => default,
        };
    }

    private static bool TryResolveOrthogonalCornerGlyph(CompactPort entry, CompactPort exit, out char glyph)
    {
        var (a, b) = NormalizePortPair(entry, exit);
        glyph = (a, b) switch
        {
            (CompactPort.North, CompactPort.East) => '╰',
            (CompactPort.North, CompactPort.West) => '╯',
            (CompactPort.South, CompactPort.East) => '╭',
            (CompactPort.South, CompactPort.West) => '╮',
            _ => default,
        };
        return glyph != default;
    }

    private static (CompactPort first, CompactPort second) NormalizePortPair(CompactPort entry, CompactPort exit)
    {
        return entry <= exit ? (entry, exit) : (exit, entry);
    }

    [Flags]
    private enum CompactPort : ushort
    {
        None = 0,
        West = 1,
        East = 2,
        North = 4,
        South = 8,
        NorthWest = 16,
        NorthEast = 32,
        SouthWest = 64,
        SouthEast = 128,
    }

    private struct CompactCellTopology
    {
        public bool HasPath;
        public CompactPort Entry;
        public CompactPort Exit;
        public int HorizontalVotes;
        public int VerticalVotes;
        public int DiagonalUpVotes;
        public int DiagonalDownVotes;

        public void RecordEntry(CompactPort port)
        {
            if (Entry == CompactPort.None)
            {
                Entry = port;
            }
        }

        public void RecordExit(CompactPort port)
        {
            Exit = port;
        }

        public void RecordDirection(int dx, int dy)
        {
            if (dx == 0 && dy == 0)
            {
                return;
            }

            if (dy == 0)
            {
                HorizontalVotes++;
                return;
            }

            if (dx == 0)
            {
                VerticalVotes++;
                return;
            }

            if (dx * dy < 0)
            {
                DiagonalUpVotes++;
                return;
            }

            DiagonalDownVotes++;
        }
    }
}
