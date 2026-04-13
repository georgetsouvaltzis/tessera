using BenchmarkDotNet.Attributes;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Benchmarks;

[MemoryDiagnoser]
public class OverlayStressBenchmarks
{
    private const int FrameCount = 48;
    private static readonly KeyPressed DownKey = new(Key.Down);
    private readonly Canvas _canvas = new(160, 46);

    private readonly CommandPalette _commandPalette = new() { IsFocused = true, MaxVisibleItems = 10 };

    private readonly ContextMenu _contextMenu = new() { IsFocused = true, Border = BorderStyle.Rounded };

    private readonly Dialog _dialog = new() { IsFocused = true };

    private readonly DataGrid _grid = new() { Border = BorderStyle.SingleLine, ShowHeader = true };

    private readonly Rect _gridBounds = new(0, 0, 160, 45);
    private readonly string[] _queries = ["op", "deploy", "open", "refresh", "toggle"];

    private readonly Rect _rootBounds = new(0, 0, 160, 46);

    private readonly StatusBar _statusBar = new() { LeftText = "overlay stress", RightText = "palette/context/dialog" };

    private readonly Rect _statusBounds = new(0, 45, 160, 1);

    [GlobalSetup]
    public void Setup()
    {
        _grid.SetColumns(BenchmarkDataFactory.CreateColumns(8, 14));
        _grid.SetRows(BenchmarkDataFactory.CreateRows(640, 8, 8701));
        _grid.SelectCell(120, 3);

        _commandPalette.SetItems(
        [
            new CommandPaletteItem("open", "Open Workspace", "Open active workspace"),
            new CommandPaletteItem("deploy", "Deploy", "Run deterministic deploy flow"),
            new CommandPaletteItem("refresh", "Refresh Data", "Reload dashboard state"),
            new CommandPaletteItem("logs", "Open Logs", "Switch to logs panel"),
            new CommandPaletteItem("help", "Show Help", "Display command cheat sheet"),
            new CommandPaletteItem("theme", "Theme Picker", "Switch active palette")
        ]);

        _contextMenu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
            new ContextMenuItem("rename", "Rename"),
            new ContextMenuItem("delete", "Delete"),
            new ContextMenuItem("open", "Open")
        ]);

        _dialog.Title = "Confirm Action";
        _dialog.BodyLines = ["Apply overlay action to current selection?"];
    }

    [Benchmark(Description = "overlay stress with palette/context/dialog layers")]
    public int RenderOverlayStressFrames()
    {
        return RenderOverlayStressFramesCore(true);
    }

    [Benchmark(Description = "overlay stress render-only with palette/context/dialog layers")]
    public int RenderOverlayStressFramesOnly()
    {
        return RenderOverlayStressFramesCore(false);
    }

    private int RenderOverlayStressFramesCore(bool materialize)
    {
        var totalLength = 0;
        for (var frame = 0; frame < FrameCount; frame++)
        {
            _canvas.Clear();
            _grid.Render(_canvas, _gridBounds);
            _statusBar.Render(_canvas, _statusBounds);

            _commandPalette.Open();
            _commandPalette.SetQueryText(_queries[frame % _queries.Length]);
            _commandPalette.Handle(DownKey);
            _commandPalette.Handle(DownKey);
            _commandPalette.Render(_canvas, _rootBounds);

            _contextMenu.OpenAt(2 + frame % 24, 3 + frame % 11);
            _contextMenu.Handle(DownKey);
            _contextMenu.Render(_canvas, _rootBounds);

            _dialog.IsVisible = true;
            _dialog.Render(_canvas, _rootBounds);

            totalLength += materialize
                ? _canvas.Render().Length
                : _canvas.Bounds.Width * _canvas.Bounds.Height;

            _dialog.Hide();
            _contextMenu.Close();
            _commandPalette.Close();
        }

        return totalLength;
    }
}
