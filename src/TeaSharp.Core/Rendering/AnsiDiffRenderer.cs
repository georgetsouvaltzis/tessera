using System.Text;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Rendering;

public sealed class AnsiDiffRenderer : IProgramRenderer
{
    private static readonly TimeSpan DefaultFlushTimeout = TimeSpan.FromSeconds(2);

    private Stream? _output;
    private StreamWriter? _writer;
    private List<DisplayLine> _previousLines = [];
    private View _currentView = View.From(string.Empty);
    private bool _initialized;
    private bool _altScreen;
    private bool _bracketedPaste;
    private bool _focusReporting;
    private bool _synchronizedUpdates;
    private MouseMode _mouseMode;
    private string? _windowTitle;
    private int _width;
    private int _height;
    private bool _fullRepaintRequired;

    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
    {
        _output = output;
        _writer = new StreamWriter(output, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true)
        {
            AutoFlush = false,
            NewLine = "\n",
        };
        _initialized = true;
        _previousLines = [];
        _altScreen = false;
        _bracketedPaste = false;
        _focusReporting = false;
        _synchronizedUpdates = false;
        _mouseMode = MouseMode.None;
        _windowTitle = null;
        _width = 0;
        _height = 0;
        _fullRepaintRequired = true;

        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public void Resize(int width, int height)
    {
        if (width != _width || height != _height)
        {
            _fullRepaintRequired = true;
        }

        _width = width;
        _height = height;
    }

    public void Render(View view)
    {
        _currentView = view;
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        if (!_initialized || _writer is null)
        {
            return;
        }

        if (_currentView.AltScreen != _altScreen)
        {
            await _writer.WriteAsync(_currentView.AltScreen ? "\u001b[?1049h" : "\u001b[?1049l")
                .ConfigureAwait(false);
            _altScreen = _currentView.AltScreen;
            _previousLines.Clear();
        }

        if (_currentView.EnableBracketedPaste != _bracketedPaste)
        {
            await _writer.WriteAsync(_currentView.EnableBracketedPaste ? "\u001b[?2004h" : "\u001b[?2004l").ConfigureAwait(false);
            _bracketedPaste = _currentView.EnableBracketedPaste;
        }

        if (_currentView.EnableFocusReporting != _focusReporting)
        {
            await _writer.WriteAsync(_currentView.EnableFocusReporting ? "\u001b[?1004h" : "\u001b[?1004l").ConfigureAwait(false);
            _focusReporting = _currentView.EnableFocusReporting;
        }

        if (_currentView.EnableSynchronizedUpdates != _synchronizedUpdates)
        {
            await _writer.WriteAsync(_currentView.EnableSynchronizedUpdates ? "\u001b[?2026h" : "\u001b[?2026l").ConfigureAwait(false);
            _synchronizedUpdates = _currentView.EnableSynchronizedUpdates;
        }

        if (_currentView.MouseMode != _mouseMode)
        {
            await WriteMouseModeAsync(_currentView.MouseMode).ConfigureAwait(false);
            _mouseMode = _currentView.MouseMode;
        }

        if (!string.Equals(_windowTitle, _currentView.WindowTitle, StringComparison.Ordinal))
        {
            if (_currentView.WindowTitle is not null)
            {
                await _writer.WriteAsync($"\u001b]2;{_currentView.WindowTitle}\u0007").ConfigureAwait(false);
            }

            _windowTitle = _currentView.WindowTitle;
        }

        if (_fullRepaintRequired)
        {
            await _writer.WriteAsync("\u001b[2J\u001b[H").ConfigureAwait(false);
            _previousLines.Clear();
            _fullRepaintRequired = false;
        }

        var lines = PrepareLines(_currentView.Content);
        await WriteFrameDiffAsync(lines).ConfigureAwait(false);

        if (_currentView.CursorX is int x && _currentView.CursorY is int y)
        {
            await _writer.WriteAsync("\u001b[?25h").ConfigureAwait(false);
            await _writer.WriteAsync($"\u001b[{y + 1};{x + 1}H").ConfigureAwait(false);
        }
        else
        {
            await _writer.WriteAsync("\u001b[?25l").ConfigureAwait(false);
        }

        await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        _previousLines = lines;

        if (cancellationToken.CanBeCanceled)
        {
            using var timeoutCts = new CancellationTokenSource(DefaultFlushTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            await Task.Yield();
            linked.Token.ThrowIfCancellationRequested();
        }
    }

    public async ValueTask ResetAsync(CancellationToken cancellationToken)
    {
        if (!_initialized || _writer is null)
        {
            return;
        }

        await _writer.WriteAsync("\u001b[0m\u001b[?25h").ConfigureAwait(false);
        if (_bracketedPaste)
        {
            await _writer.WriteAsync("\u001b[?2004l").ConfigureAwait(false);
            _bracketedPaste = false;
        }

        if (_focusReporting)
        {
            await _writer.WriteAsync("\u001b[?1004l").ConfigureAwait(false);
            _focusReporting = false;
        }

        if (_synchronizedUpdates)
        {
            await _writer.WriteAsync("\u001b[?2026l").ConfigureAwait(false);
            _synchronizedUpdates = false;
        }

        if (_mouseMode != MouseMode.None)
        {
            await WriteMouseModeAsync(MouseMode.None).ConfigureAwait(false);
            _mouseMode = MouseMode.None;
        }

        if (_altScreen)
        {
            await _writer.WriteAsync("\u001b[?1049l").ConfigureAwait(false);
            _altScreen = false;
        }

        await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        _previousLines.Clear();
        _fullRepaintRequired = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_writer is not null)
        {
            await _writer.DisposeAsync().ConfigureAwait(false);
        }

        if (_output is not null)
        {
            await _output.DisposeAsync().ConfigureAwait(false);
        }

        _initialized = false;
    }

    private static List<string> NormalizeLines(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return [string.Empty];
        }

        content = content.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        return [.. content.Split('\n')];
    }

    private List<DisplayLine> PrepareLines(string content)
    {
        var normalized = NormalizeLines(content);
        var rendered = new List<DisplayLine>(normalized.Count);
        foreach (var line in normalized)
        {
            rendered.AddRange(DisplayLine.WrapText(line, _width));
        }

        if (_height > 0 && rendered.Count > _height)
        {
            rendered = rendered.GetRange(0, _height);
        }

        if (rendered.Count == 0)
        {
            rendered.Add(DisplayLine.FromText(string.Empty, _width));
        }

        return rendered;
    }

    private async Task WriteFrameDiffAsync(List<DisplayLine> nextLines)
    {
        if (_writer is null)
        {
            return;
        }

        var rowCount = Math.Max(_previousLines.Count, nextLines.Count);
        if (_height > 0 && rowCount > _height)
        {
            rowCount = _height;
        }

        for (var row = 0; row < rowCount; row++)
        {
            var previousLine = row < _previousLines.Count ? _previousLines[row] : DisplayLine.FromText(string.Empty, _width);
            var nextLine = row < nextLines.Count ? nextLines[row] : DisplayLine.FromText(string.Empty, _width);
            if (LinesEqual(previousLine, nextLine))
            {
                continue;
            }

            await WriteRowDiffAsync(row, previousLine, nextLine).ConfigureAwait(false);
        }
    }

    private static bool LinesEqual(DisplayLine previousLine, DisplayLine nextLine)
    {
        var maxColumns = Math.Max(previousLine.ColumnCount, nextLine.ColumnCount);
        for (var column = 0; column < maxColumns; column++)
        {
            if (!string.Equals(previousLine.SignatureAt(column), nextLine.SignatureAt(column), StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private async Task WriteRowDiffAsync(int row, DisplayLine previousLine, DisplayLine nextLine)
    {
        if (_writer is null)
        {
            return;
        }

        var max = Math.Max(previousLine.ColumnCount, nextLine.ColumnCount);
        if (_width > 0 && max > _width)
        {
            max = _width;
        }
        var runStart = -1;

        for (var column = 0; column < max; column++)
        {
            var changed = !string.Equals(
                previousLine.SignatureAt(column),
                nextLine.SignatureAt(column),
                StringComparison.Ordinal);

            if (changed && runStart < 0)
            {
                runStart = column;
                continue;
            }

            if (!changed && runStart >= 0)
            {
                await WriteRunAsync(row, runStart, column, nextLine).ConfigureAwait(false);
                runStart = -1;
            }
        }

        if (runStart >= 0)
        {
            await WriteRunAsync(row, runStart, max, nextLine).ConfigureAwait(false);
        }
    }

    private async Task WriteRunAsync(int row, int startColumn, int endColumn, DisplayLine nextLine)
    {
        if (_writer is null)
        {
            return;
        }

        await _writer.WriteAsync($"\u001b[{row + 1};{startColumn + 1}H").ConfigureAwait(false);
        for (var column = startColumn; column < endColumn;)
        {
            var cell = nextLine.CellAt(column);
            if (cell is null)
            {
                await _writer.WriteAsync(" ").ConfigureAwait(false);
                column++;
                continue;
            }

            var cellWidth = nextLine.CellWidthAt(column);
            if (cellWidth == 2 && column + 1 >= endColumn)
            {
                await _writer.WriteAsync(" ").ConfigureAwait(false);
                column++;
                continue;
            }

            await _writer.WriteAsync(cell).ConfigureAwait(false);
            column += cellWidth;
        }
    }

    private Task WriteMouseModeAsync(MouseMode mode)
    {
        if (_writer is null)
        {
            return Task.CompletedTask;
        }

        return mode switch
        {
            MouseMode.CellMotion => _writer.WriteAsync("\u001b[?1000h\u001b[?1002h\u001b[?1003l\u001b[?1006h"),
            MouseMode.AllMotion => _writer.WriteAsync("\u001b[?1000h\u001b[?1002l\u001b[?1003h\u001b[?1006h"),
            _ => _writer.WriteAsync("\u001b[?1000l\u001b[?1002l\u001b[?1003l\u001b[?1006l"),
        };
    }

}
