using System.Buffers;
using TeaSharp.Components.Primitives;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class TelemetryChart
{
    private static readonly char[] BlockFillGlyphs = [' ', '▁', '▂', '▃', '▄', '▅', '▆', '▇', '█'];
    private static readonly byte[] BrailleDotMasks =
    [
        0b00000001,
        0b00000010,
        0b00000100,
        0b01000000,
        0b00001000,
        0b00010000,
        0b00100000,
        0b10000000,
    ];

    private void RenderTelemetry(Canvas canvas, Rect plotArea, TelemetryChartRenderMode renderMode)
    {
        if (plotArea.Width <= 0 || plotArea.Height <= 0)
        {
            return;
        }

        if (plotArea.Height == 1)
        {
            RenderAreaRows(canvas, plotArea, ResolveLevels(plotArea.Width, 8));
            return;
        }

        switch (ResolveRenderMode(renderMode))
        {
            case TelemetryChartRenderMode.Braille:
                RenderBrailleArea(canvas, plotArea);
                break;
            case TelemetryChartRenderMode.Block:
                RenderRibbonRows(canvas, plotArea, ResolveLevels(plotArea.Width, plotArea.Height * 8));
                break;
            default:
                RenderAreaRows(canvas, plotArea, ResolveLevels(plotArea.Width, plotArea.Height * 8));
                break;
        }
    }

    private static TelemetryChartRenderMode ResolveRenderMode(TelemetryChartRenderMode renderMode)
    {
        if (renderMode != TelemetryChartRenderMode.Auto)
        {
            return renderMode;
        }

        return TelemetryChartRenderMode.Braille;
    }

    private int[] ResolveLevels(int width, int verticalUnits)
    {
        var levels = new int[Math.Max(0, width)];
        if (levels.Length == 0)
        {
            return levels;
        }

        var (min, max) = ResolveBounds();
        var range = max - min;
        if (Math.Abs(range) < double.Epsilon)
        {
            range = 1;
        }

        for (var x = 0; x < levels.Length; x++)
        {
            var sample = SampleAtColumn(levels.Length, x);
            var normalized = Math.Clamp((sample - min) / range, 0d, 1d);
            levels[x] = (int)Math.Round(normalized * verticalUnits, MidpointRounding.AwayFromZero);
        }

        return levels;
    }

    private double SampleAtColumn(int width, int column)
    {
        if (_samples.Count == 1 || width <= 1)
        {
            return _samples[^1];
        }

        var sourcePosition = column * (_samples.Count - 1d) / Math.Max(1, width - 1d);
        var lowerIndex = (int)Math.Floor(sourcePosition);
        var upperIndex = Math.Min(_samples.Count - 1, lowerIndex + 1);
        if (upperIndex == lowerIndex)
        {
            return _samples[lowerIndex];
        }

        var amount = sourcePosition - lowerIndex;
        return _samples[lowerIndex] + ((_samples[upperIndex] - _samples[lowerIndex]) * amount);
    }

    private void RenderAreaRows(Canvas canvas, Rect plotArea, int[] levels)
    {
        var style = ResolveStyled(FillStyle);
        var rented = ArrayPool<char>.Shared.Rent(plotArea.Width);
        try
        {
            for (var row = 0; row < plotArea.Height; row++)
            {
                var rowFloor = (plotArea.Height - row - 1) * 8;
                var buffer = rented.AsSpan(0, plotArea.Width);
                for (var x = 0; x < plotArea.Width; x++)
                {
                    var fillUnits = Math.Clamp(levels[x] - rowFloor, 0, 8);
                    buffer[x] = BlockFillGlyphs[fillUnits];
                }

                var line = new string(buffer);
                canvas.WriteText(plotArea.X, plotArea.Y + row, ApplyStyle(line, style), plotArea.Width);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    private void RenderRibbonRows(Canvas canvas, Rect plotArea, int[] levels)
    {
        var style = ResolveStyled(FillStyle);
        var thickness = Math.Clamp(Math.Max(3, (plotArea.Height * 8) / 5), 3, 8);
        var rented = ArrayPool<char>.Shared.Rent(plotArea.Width);
        try
        {
            for (var row = 0; row < plotArea.Height; row++)
            {
                var rowFloor = (plotArea.Height - row - 1) * 8;
                var buffer = rented.AsSpan(0, plotArea.Width);
                for (var x = 0; x < plotArea.Width; x++)
                {
                    var upper = levels[x];
                    var lower = Math.Max(0, upper - thickness);
                    var localLower = Math.Clamp(lower - rowFloor, 0, 8);
                    var localUpper = Math.Clamp(upper - rowFloor, 0, 8);
                    var fillUnits = Math.Clamp(localUpper - localLower, 0, 8);
                    buffer[x] = BlockFillGlyphs[fillUnits];
                }

                var line = new string(buffer);
                canvas.WriteText(plotArea.X, plotArea.Y + row, ApplyStyle(line, style), plotArea.Width);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    private void RenderBrailleArea(Canvas canvas, Rect plotArea)
    {
        var subcellWidth = plotArea.Width * 2;
        var subcellHeight = plotArea.Height * 4;
        var levels = ResolveLevels(subcellWidth, subcellHeight);
        var masks = new byte[plotArea.Width * plotArea.Height];
        for (var x = 0; x < subcellWidth; x++)
        {
            var filled = levels[x];
            for (var unit = 0; unit < filled; unit++)
            {
                var subcellY = subcellHeight - unit - 1;
                var cellX = x / 2;
                var cellY = subcellY / 4;
                var localX = x % 2;
                var localY = subcellY % 4;
                masks[(cellY * plotArea.Width) + cellX] |= BrailleDotMasks[(localX * 4) + localY];
            }
        }

        var style = ResolveStyled(FillStyle);
        var rented = ArrayPool<char>.Shared.Rent(plotArea.Width);
        try
        {
            for (var row = 0; row < plotArea.Height; row++)
            {
                var buffer = rented.AsSpan(0, plotArea.Width);
                for (var column = 0; column < plotArea.Width; column++)
                {
                    var mask = masks[(row * plotArea.Width) + column];
                    buffer[column] = mask == 0 ? ' ' : (char)(0x2800 + mask);
                }

                var line = new string(buffer);
                canvas.WriteText(plotArea.X, plotArea.Y + row, ApplyStyle(line, style), plotArea.Width);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }
}
