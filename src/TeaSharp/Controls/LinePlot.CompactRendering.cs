using TeaSharp.Components.Primitives;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

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

        var cells = new CompactTraceCell[plotArea.Width * plotArea.Height];
        var previousX = -1;
        var previousY = -1;
        var hasPoint = false;
        var order = 0;
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
                order = RasterizeCompactPolyline(
                    cells,
                    plotArea.Width,
                    plotArea.Height,
                    previousX,
                    previousY,
                    virtualX,
                    virtualY,
                    order);
            }
            else
            {
                RecordCompactTracePoint(cells, plotArea.Width, plotArea.Height, virtualX, virtualY, order++);
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
                ref var cell = ref cells[(cellY * plotArea.Width) + cellX];
                if (!cell.HasPoint)
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

    private static int RasterizeCompactPolyline(
        CompactTraceCell[] cells,
        int width,
        int height,
        int x0,
        int y0,
        int x1,
        int y1,
        int order)
    {
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = x0 < x1 ? 1 : -1;
        var sy = y0 < y1 ? 1 : -1;
        var err = dx - dy;
        var currentX = x0;
        var currentY = y0;

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

            RecordCompactTracePoint(cells, width, height, currentX, currentY, order++);
            currentX = nextX;
            currentY = nextY;
        }

        RecordCompactTracePoint(cells, width, height, currentX, currentY, order++);
        return order;
    }

    private static void RecordCompactTracePoint(CompactTraceCell[] cells, int width, int height, int virtualX, int virtualY, int order)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        var maxVirtualX = (width * CompactVirtualWidthPerCell) - 1;
        var maxVirtualY = (height * CompactVirtualHeightPerCell) - 1;
        virtualX = Math.Clamp(virtualX, 0, maxVirtualX);
        virtualY = Math.Clamp(virtualY, 0, maxVirtualY);

        var cellX = virtualX / CompactVirtualWidthPerCell;
        var cellY = virtualY / CompactVirtualHeightPerCell;
        var localX = virtualX % CompactVirtualWidthPerCell;
        var localY = virtualY % CompactVirtualHeightPerCell;
        ref var cell = ref cells[(cellY * width) + cellX];
        cell.Record(localX, localY, order);
    }

    private static char ResolveCompactLineGlyph(CompactTraceCell cell)
    {
        var dx = cell.LastX - cell.FirstX;
        var dy = cell.LastY - cell.FirstY;
        var absDx = Math.Abs(dx);
        var absDy = Math.Abs(dy);

        if (absDx == 0 && absDy == 0)
        {
            return '•';
        }

        if (absDy <= 1 && absDx > 0)
        {
            return '─';
        }

        if (absDx <= 1 && absDy > 0)
        {
            return '│';
        }

        return dx * dy < 0 ? '╱' : '╲';
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

    private struct CompactTraceCell
    {
        public bool HasPoint { get; private set; }
        public int FirstOrder { get; private set; }
        public int LastOrder { get; private set; }
        public int FirstX { get; private set; }
        public int FirstY { get; private set; }
        public int LastX { get; private set; }
        public int LastY { get; private set; }

        public void Record(int localX, int localY, int order)
        {
            if (!HasPoint)
            {
                HasPoint = true;
                FirstOrder = order;
                LastOrder = order;
                FirstX = localX;
                FirstY = localY;
                LastX = localX;
                LastY = localY;
                return;
            }

            if (order < FirstOrder)
            {
                FirstOrder = order;
                FirstX = localX;
                FirstY = localY;
            }

            if (order >= LastOrder)
            {
                LastOrder = order;
                LastX = localX;
                LastY = localY;
            }
        }
    }
}
