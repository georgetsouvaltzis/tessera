using BenchmarkDotNet.Attributes;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Benchmarks;

[MemoryDiagnoser]
public class ViewportRenderBenchmarks
{
    private const int FrameCount = 64;
    private static readonly KeyPressed UpKey = new(Key.Up);
    private static readonly KeyPressed DownKey = new(Key.Down);

    private readonly Rect _bounds = new(0, 0, 160, 42);
    private readonly Canvas _canvas = new(160, 42);

    private readonly LogView _logView = new() { Border = BorderStyle.SingleLine, AutoScroll = false, IsFocused = true };

    [GlobalSetup]
    public void Setup()
    {
        _logView.Clear();
        for (var index = 0; index < 4_096; index++)
        {
            _logView.Append($"[{index % 7:D2}] viewport line {index:D4} token={(index * 23 + 17) % 100_000:D5}");
        }
    }

    [Benchmark(Description = "viewport no-decoration render (log view)")]
    public int RenderViewportNoDecoration()
    {
        return RenderViewportNoDecorationCore(true);
    }

    [Benchmark(Description = "viewport no-decoration render-only (log view)")]
    public int RenderViewportNoDecorationOnly()
    {
        return RenderViewportNoDecorationCore(false);
    }

    private int RenderViewportNoDecorationCore(bool materialize)
    {
        var totalLength = 0;
        for (var frame = 0; frame < FrameCount; frame++)
        {
            _logView.Handle((frame & 1) == 0 ? DownKey : UpKey);
            _canvas.Clear();
            _logView.Render(_canvas, _bounds);
            totalLength += materialize
                ? _canvas.Render().Length
                : _canvas.Bounds.Width * _canvas.Bounds.Height;
        }

        return totalLength;
    }
}
