using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Rendering;

public sealed class AnsiDiffRenderer : IProgramRenderer
{
    private static readonly TimeSpan DefaultFlushTimeout = TimeSpan.FromSeconds(2);

    private Stream? _output;
    private StreamWriter? _writer;
    private RenderFrameBuffer _previousFrame = RenderFrameBuffer.Empty;
    private View _currentView = View.From(string.Empty);
    private bool _initialized;
    private bool _altScreen;
    private bool _bracketedPaste;
    private bool _focusReporting;
    private MouseMode _mouseMode;
    private CursorStyle? _cursorStyle;
    private string? _cursorColor;
    private string? _foregroundColor;
    private string? _backgroundColor;
    private TerminalProgress? _progress;
    private int _keyboardEnhancementFlags;
    private readonly HashSet<int> _queriedModes = [];
    private string? _windowTitle;
    private int _width;
    private int _height;
    private bool _fullRepaintRequired;
    private TerminalCapabilityProfile _capabilities;

    public AnsiDiffRenderer(TerminalCapabilityProfile? capabilities = null)
    {
        _capabilities = capabilities ?? TerminalCapabilityProfile.AllSupported;
    }

    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
    {
        _output = output;
        _writer = new StreamWriter(output, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true)
        {
            AutoFlush = false,
            NewLine = "\n",
        };
        _initialized = true;
        _previousFrame = RenderFrameBuffer.Empty;
        _altScreen = false;
        _bracketedPaste = false;
        _focusReporting = false;
        _mouseMode = MouseMode.None;
        _cursorStyle = null;
        _cursorColor = null;
        _foregroundColor = null;
        _backgroundColor = null;
        _progress = null;
        _keyboardEnhancementFlags = 0;
        _queriedModes.Clear();
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

    public void UpdateCapabilities(TerminalCapabilityProfile capabilities)
    {
        _capabilities = capabilities;
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
            _previousFrame = RenderFrameBuffer.Empty;
            if (_keyboardEnhancementFlags != 0)
            {
                await _writer.WriteAsync("\u001b[>0u").ConfigureAwait(false);
                _keyboardEnhancementFlags = 0;
            }
        }

        var requestedBracketedPaste = _currentView.EnableBracketedPaste && _capabilities.BracketedPaste;
        if (requestedBracketedPaste != _bracketedPaste)
        {
            await _writer.WriteAsync(requestedBracketedPaste ? "\u001b[?2004h" : "\u001b[?2004l").ConfigureAwait(false);
            _bracketedPaste = requestedBracketedPaste;
            if (requestedBracketedPaste)
            {
                await QueryModeReportOnceAsync(2004).ConfigureAwait(false);
            }
        }

        var requestedFocusReporting = _currentView.EnableFocusReporting && _capabilities.FocusReporting;
        if (requestedFocusReporting != _focusReporting)
        {
            await _writer.WriteAsync(requestedFocusReporting ? "\u001b[?1004h" : "\u001b[?1004l").ConfigureAwait(false);
            _focusReporting = requestedFocusReporting;
            if (requestedFocusReporting)
            {
                await QueryModeReportOnceAsync(1004).ConfigureAwait(false);
            }
        }

        var requestedSyncUpdates = _currentView.EnableSynchronizedUpdates && _capabilities.SynchronizedUpdates;
        if (requestedSyncUpdates)
        {
            await _writer.WriteAsync("\u001b[?2026h").ConfigureAwait(false);
            await QueryModeReportOnceAsync(2026).ConfigureAwait(false);
        }

        var requestedMouseMode = _capabilities.MouseReporting
            ? _currentView.MouseMode
            : MouseMode.None;
        if (requestedMouseMode != _mouseMode)
        {
            await WriteMouseModeAsync(requestedMouseMode).ConfigureAwait(false);
            _mouseMode = requestedMouseMode;
            if (requestedMouseMode != MouseMode.None)
            {
                await QueryModeReportOnceAsync(1006).ConfigureAwait(false);
            }
        }

        if (!string.Equals(_windowTitle, _currentView.WindowTitle, StringComparison.Ordinal))
        {
            if (_currentView.WindowTitle is not null)
            {
                await _writer.WriteAsync($"\u001b]2;{_currentView.WindowTitle}\u0007").ConfigureAwait(false);
            }

            _windowTitle = _currentView.WindowTitle;
        }

        var requestedKeyboardFlags = GetKeyboardEnhancementFlags(_currentView.KeyboardEnhancements);
        if (requestedKeyboardFlags != _keyboardEnhancementFlags)
        {
            await _writer.WriteAsync($"\u001b[>{requestedKeyboardFlags}u").ConfigureAwait(false);
            _keyboardEnhancementFlags = requestedKeyboardFlags;
        }

        var requestedForegroundColor = NormalizeColorHex(_currentView.ForegroundColor);
        if (!string.Equals(_foregroundColor, requestedForegroundColor, StringComparison.Ordinal))
        {
            await WriteTerminalColorAsync(10, 110, requestedForegroundColor).ConfigureAwait(false);
            _foregroundColor = requestedForegroundColor;
        }

        var requestedBackgroundColor = NormalizeColorHex(_currentView.BackgroundColor);
        if (!string.Equals(_backgroundColor, requestedBackgroundColor, StringComparison.Ordinal))
        {
            await WriteTerminalColorAsync(11, 111, requestedBackgroundColor).ConfigureAwait(false);
            _backgroundColor = requestedBackgroundColor;
        }

        var requestedCursorColor = NormalizeColorHex(_currentView.CursorColor);
        if (!string.Equals(_cursorColor, requestedCursorColor, StringComparison.Ordinal))
        {
            await WriteTerminalColorAsync(12, 112, requestedCursorColor).ConfigureAwait(false);
            _cursorColor = requestedCursorColor;
        }

        if (_progress != _currentView.Progress)
        {
            await WriteProgressAsync(_currentView.Progress).ConfigureAwait(false);
            _progress = _currentView.Progress;
        }

        if (_fullRepaintRequired)
        {
            await _writer.WriteAsync("\u001b[2J\u001b[H").ConfigureAwait(false);
            _previousFrame = RenderFrameBuffer.Empty;
            _fullRepaintRequired = false;
        }

        var nextFrame = RenderFrameBuffer.FromContent(_currentView.Content, _width, _height);
        await WriteFrameDiffAsync(nextFrame).ConfigureAwait(false);

        if (_currentView.CursorX is int x && _currentView.CursorY is int y)
        {
            if (_currentView.CursorStyle is CursorStyle requestedCursorStyle
                && requestedCursorStyle != _cursorStyle)
            {
                await WriteCursorStyleAsync(requestedCursorStyle).ConfigureAwait(false);
                _cursorStyle = requestedCursorStyle;
            }

            await _writer.WriteAsync("\u001b[?25h").ConfigureAwait(false);
            await _writer.WriteAsync($"\u001b[{y + 1};{x + 1}H").ConfigureAwait(false);
        }
        else
        {
            await _writer.WriteAsync("\u001b[?25l").ConfigureAwait(false);
        }

        if (requestedSyncUpdates)
        {
            await _writer.WriteAsync("\u001b[?2026l").ConfigureAwait(false);
        }

        await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        _previousFrame = nextFrame;

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
        if (_cursorStyle is not null)
        {
            await _writer.WriteAsync("\u001b[0 q").ConfigureAwait(false);
            _cursorStyle = null;
        }

        if (_keyboardEnhancementFlags != 0)
        {
            await _writer.WriteAsync("\u001b[>0u").ConfigureAwait(false);
            _keyboardEnhancementFlags = 0;
        }

        if (_foregroundColor is not null)
        {
            await _writer.WriteAsync("\u001b]110;\u001b\\").ConfigureAwait(false);
            _foregroundColor = null;
        }

        if (_backgroundColor is not null)
        {
            await _writer.WriteAsync("\u001b]111;\u001b\\").ConfigureAwait(false);
            _backgroundColor = null;
        }

        if (_cursorColor is not null)
        {
            await _writer.WriteAsync("\u001b]112;\u001b\\").ConfigureAwait(false);
            _cursorColor = null;
        }

        if (_progress is not null && _progress.Value.State != TerminalProgressState.None)
        {
            await _writer.WriteAsync("\u001b]9;4;0\u001b\\").ConfigureAwait(false);
            _progress = null;
        }

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
        _previousFrame = RenderFrameBuffer.Empty;
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

    public async ValueTask WriteRawAsync(string content, CancellationToken cancellationToken)
    {
        if (!_initialized || _writer is null || string.IsNullOrEmpty(content))
        {
            return;
        }

        await _writer.WriteAsync(content).ConfigureAwait(false);
        await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task WriteFrameDiffAsync(RenderFrameBuffer nextFrame)
    {
        if (_writer is null)
        {
            return;
        }

        var rowCount = Math.Max(_previousFrame.RowCount, nextFrame.RowCount);
        if (_height > 0 && rowCount > _height)
        {
            rowCount = _height;
        }

        for (var row = 0; row < rowCount; row++)
        {
            if (nextFrame.RowEquals(_previousFrame, row))
            {
                continue;
            }

            await WriteRowDiffAsync(row, nextFrame).ConfigureAwait(false);
        }
    }

    private async Task WriteRowDiffAsync(int row, RenderFrameBuffer nextFrame)
    {
        if (_writer is null)
        {
            return;
        }

        var max = Math.Max(_previousFrame.ColumnCountAt(row), nextFrame.ColumnCountAt(row));
        if (_width > 0 && max > _width)
        {
            max = _width;
        }
        var runStart = -1;

        for (var column = 0; column < max; column++)
        {
            var changed = !string.Equals(
                _previousFrame.SignatureAt(row, column),
                nextFrame.SignatureAt(row, column),
                StringComparison.Ordinal);

            if (changed && runStart < 0)
            {
                runStart = column;
                continue;
            }

            if (!changed && runStart >= 0)
            {
                await WriteRunAsync(row, runStart, column, nextFrame).ConfigureAwait(false);
                runStart = -1;
            }
        }

        if (runStart >= 0)
        {
            await WriteRunAsync(row, runStart, max, nextFrame).ConfigureAwait(false);
        }
    }

    private async Task WriteRunAsync(int row, int startColumn, int endColumn, RenderFrameBuffer nextFrame)
    {
        if (_writer is null)
        {
            return;
        }

        await _writer.WriteAsync($"\u001b[{row + 1};{startColumn + 1}H").ConfigureAwait(false);
        var activeStyle = string.Empty;
        for (var column = startColumn; column < endColumn;)
        {
            var cell = nextFrame.CellAt(row, column);
            if (cell is null)
            {
                await _writer.WriteAsync(" ").ConfigureAwait(false);
                column++;
                continue;
            }

            var nextStyle = nextFrame.StyleAt(row, column);
            if (!string.Equals(activeStyle, nextStyle, StringComparison.Ordinal))
            {
                if (activeStyle.Length > 0)
                {
                    await _writer.WriteAsync("\u001b[0m").ConfigureAwait(false);
                }

                if (nextStyle.Length > 0)
                {
                    await _writer.WriteAsync(nextStyle).ConfigureAwait(false);
                }

                activeStyle = nextStyle;
            }

            var cellWidth = nextFrame.CellWidthAt(row, column);
            if (cellWidth == 2 && column + 1 >= endColumn)
            {
                await _writer.WriteAsync(" ").ConfigureAwait(false);
                column++;
                continue;
            }

            await _writer.WriteAsync(cell).ConfigureAwait(false);
            column += cellWidth;
        }

        if (activeStyle.Length > 0)
        {
            await _writer.WriteAsync("\u001b[0m").ConfigureAwait(false);
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

    private async Task WriteTerminalColorAsync(int setCode, int resetCode, string? color)
    {
        if (_writer is null)
        {
            return;
        }

        if (color is null)
        {
            await _writer.WriteAsync($"\u001b]{resetCode};\u001b\\").ConfigureAwait(false);
            return;
        }

        await _writer.WriteAsync($"\u001b]{setCode};{color}\u001b\\").ConfigureAwait(false);
    }

    private async Task WriteProgressAsync(TerminalProgress? progress)
    {
        if (_writer is null)
        {
            return;
        }

        if (progress is not TerminalProgress current || current.State == TerminalProgressState.None)
        {
            await _writer.WriteAsync("\u001b]9;4;0\u001b\\").ConfigureAwait(false);
            return;
        }

        if (current.State == TerminalProgressState.Indeterminate)
        {
            await _writer.WriteAsync("\u001b]9;4;3\u001b\\").ConfigureAwait(false);
            return;
        }

        var clamped = Math.Clamp(current.Value, 0, 100);
        var state = current.State switch
        {
            TerminalProgressState.Default => 1,
            TerminalProgressState.Error => 2,
            TerminalProgressState.Warning => 4,
            _ => 0,
        };

        if (state == 0)
        {
            await _writer.WriteAsync("\u001b]9;4;0\u001b\\").ConfigureAwait(false);
            return;
        }

        await _writer.WriteAsync($"\u001b]9;4;{state};{clamped}\u001b\\").ConfigureAwait(false);
    }

    private Task WriteCursorStyleAsync(CursorStyle style)
    {
        if (_writer is null)
        {
            return Task.CompletedTask;
        }

        var parameter = style switch
        {
            CursorStyle.BlinkingBlock => 1,
            CursorStyle.SteadyBlock => 2,
            CursorStyle.BlinkingUnderline => 3,
            CursorStyle.SteadyUnderline => 4,
            CursorStyle.BlinkingBar => 5,
            CursorStyle.SteadyBar => 6,
            _ => 0,
        };

        return _writer.WriteAsync($"\u001b[{parameter} q");
    }

    private Task QueryModeReportOnceAsync(int mode)
    {
        if (_writer is null || !_capabilities.ModeReports || !_queriedModes.Add(mode))
        {
            return Task.CompletedTask;
        }

        return _writer.WriteAsync($"\u001b[?{mode}$p");
    }

    private static int GetKeyboardEnhancementFlags(KeyboardEnhancementOptions options)
    {
        var flags = 0b1;
        if (options.ReportEventTypes)
        {
            flags |= 0b10;
        }

        return flags;
    }

    private static string? NormalizeColorHex(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var value = input.Trim();
        if (value.StartsWith("rgb:", StringComparison.OrdinalIgnoreCase))
        {
            var channels = value[4..].Split('/');
            if (channels.Length != 3)
            {
                return null;
            }

            if (!TryParseRgbChannel(channels[0], out var r)
                || !TryParseRgbChannel(channels[1], out var g)
                || !TryParseRgbChannel(channels[2], out var b))
            {
                return null;
            }

            return $"#{r:X2}{g:X2}{b:X2}";
        }

        if (value[0] == '#')
        {
            value = value[1..];
        }

        if (value.Length == 3
            && byte.TryParse(new string(value[0], 2), System.Globalization.NumberStyles.HexNumber, null, out var shortR)
            && byte.TryParse(new string(value[1], 2), System.Globalization.NumberStyles.HexNumber, null, out var shortG)
            && byte.TryParse(new string(value[2], 2), System.Globalization.NumberStyles.HexNumber, null, out var shortB))
        {
            return $"#{shortR:X2}{shortG:X2}{shortB:X2}";
        }

        if (value.Length == 6
            && byte.TryParse(value[..2], System.Globalization.NumberStyles.HexNumber, null, out var r6)
            && byte.TryParse(value[2..4], System.Globalization.NumberStyles.HexNumber, null, out var g6)
            && byte.TryParse(value[4..], System.Globalization.NumberStyles.HexNumber, null, out var b6))
        {
            return $"#{r6:X2}{g6:X2}{b6:X2}";
        }

        return null;
    }

    private static bool TryParseRgbChannel(string value, out byte result)
    {
        result = 0;
        var normalized = value.Trim();
        if (normalized.Length is < 1 or > 4)
        {
            return false;
        }

        if (!ushort.TryParse(normalized, System.Globalization.NumberStyles.HexNumber, null, out var parsed))
        {
            return false;
        }

        if (normalized.Length <= 2)
        {
            result = (byte)parsed;
            return true;
        }

        var max = normalized.Length == 3 ? 0x0FFFu : 0xFFFFu;
        result = (byte)Math.Round((parsed / (double)max) * 255d, MidpointRounding.AwayFromZero);
        return true;
    }

}
