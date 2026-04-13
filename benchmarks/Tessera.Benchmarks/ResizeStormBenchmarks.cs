using BenchmarkDotNet.Attributes;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Benchmarks;

[MemoryDiagnoser]
public class ResizeStormBenchmarks
{
    private const int ResizeIterations = 72;

    private readonly DataGrid _grid = new() { Border = BorderStyle.SingleLine, ShowHeader = true };

    private readonly MarkdownView _markdownView = new() { Border = BorderStyle.SingleLine, ShowLineNumbers = true };

    private readonly MiniLog _miniLog = new(80);

    private readonly ResizeSnapshot[] _snapshots = BuildSnapshots();

    private readonly StatusBar _statusBar = new() { LeftText = "resize storm", RightText = "recompose" };

    private Canvas[] _snapshotCanvases = [];

    [GlobalSetup]
    public void Setup()
    {
        _grid.SetColumns(BenchmarkDataFactory.CreateColumns(10, 10));
        _grid.SetRows(BenchmarkDataFactory.CreateRows(900, 10, 6060));
        _grid.SelectCell(220, 4);

        _markdownView.SetMarkdown(
            """
            # Resize Storm
            Deterministic layout recomposition benchmark.
            - fixed-size loops
            - repeated bounds changes
            - mixed controls
            """);

        _miniLog.Clear();
        for (var index = 0; index < 80; index++)
        {
            _miniLog.Append($"evt-{index:D3} size-check {(index * 37 + 5) % 997:D3}");
        }

        _snapshotCanvases = CreateCanvases(_snapshots);
    }

    [Benchmark(Description = "resize storm repeated recomposition")]
    public int RenderResizeStormFrames()
    {
        return RenderResizeStormFramesCore(true);
    }

    [Benchmark(Description = "resize storm repeated recomposition render-only")]
    public int RenderResizeStormFramesOnly()
    {
        return RenderResizeStormFramesCore(false);
    }

    private int RenderResizeStormFramesCore(bool materialize)
    {
        var totalLength = 0;
        for (var iteration = 0; iteration < ResizeIterations; iteration++)
        {
            var snapshotIndex = iteration % _snapshots.Length;
            var snapshot = _snapshots[snapshotIndex];
            var canvas = _snapshotCanvases[snapshotIndex];

            _grid.Handle(snapshot.ResizeMessage);
            _markdownView.Handle(snapshot.ResizeMessage);
            _miniLog.Handle(snapshot.ResizeMessage);

            canvas.Clear();
            _grid.Render(canvas, snapshot.GridBounds);
            _markdownView.Render(canvas, snapshot.MarkdownBounds);
            _miniLog.Render(canvas, snapshot.MiniLogBounds);
            _statusBar.Render(canvas, snapshot.StatusBounds);

            totalLength += materialize
                ? canvas.Render().Length
                : canvas.Bounds.Width * canvas.Bounds.Height;
        }

        return totalLength;
    }

    private static ResizeSnapshot[] BuildSnapshots()
    {
        var snapshots = new ResizeSnapshot[12];
        for (var index = 0; index < snapshots.Length; index++)
        {
            var width = 90 + index * 17 % 70;
            var height = 24 + index * 11 % 20;
            var statusBounds = new Rect(0, height - 1, width, 1);
            var bodyHeight = Math.Max(1, height - 1);
            var topHeight = Math.Max(6, bodyHeight * 2 / 3);
            var bottomHeight = Math.Max(1, bodyHeight - topHeight);
            var leftWidth = Math.Max(20, width / 2);
            var rightWidth = Math.Max(1, width - leftWidth);

            snapshots[index] = new ResizeSnapshot(
                new WindowResized(width, height),
                new Rect(0, 0, width, topHeight),
                new Rect(0, topHeight, leftWidth, bottomHeight),
                new Rect(leftWidth, topHeight, rightWidth, bottomHeight),
                statusBounds);
        }

        return snapshots;
    }

    private static Canvas[] CreateCanvases(IReadOnlyList<ResizeSnapshot> snapshots)
    {
        var canvases = new Canvas[snapshots.Count];
        for (var index = 0; index < snapshots.Count; index++)
        {
            var status = snapshots[index].StatusBounds;
            canvases[index] = new Canvas(status.Width, status.Y + status.Height);
        }

        return canvases;
    }

    private readonly record struct ResizeSnapshot(
        WindowResized ResizeMessage,
        Rect GridBounds,
        Rect MarkdownBounds,
        Rect MiniLogBounds,
        Rect StatusBounds);
}
