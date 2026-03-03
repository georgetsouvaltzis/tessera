using System.Text;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Rendering;

public sealed class AnsiDiffRenderer : IProgramRenderer
{
    private static readonly TimeSpan DefaultFlushTimeout = TimeSpan.FromSeconds(2);

    private Stream? _output;
    private StreamWriter? _writer;
    private List<string> _previousLines = [];
    private View _currentView = View.From(string.Empty);
    private bool _initialized;
    private bool _altScreen;
    private bool _bracketedPaste;
    private bool _focusReporting;
    private bool _synchronizedUpdates;
    private string? _windowTitle;

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
        _windowTitle = null;

        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public void Resize(int width, int height)
    {
        _ = width;
        _ = height;
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

        if (!string.Equals(_windowTitle, _currentView.WindowTitle, StringComparison.Ordinal))
        {
            if (_currentView.WindowTitle is not null)
            {
                await _writer.WriteAsync($"\u001b]2;{_currentView.WindowTitle}\u0007").ConfigureAwait(false);
            }

            _windowTitle = _currentView.WindowTitle;
        }

        var lines = NormalizeLines(_currentView.Content);
        var max = Math.Max(lines.Count, _previousLines.Count);

        for (var i = 0; i < max; i++)
        {
            var next = i < lines.Count ? lines[i] : string.Empty;
            var prev = i < _previousLines.Count ? _previousLines[i] : null;

            if (prev is not null && string.Equals(prev, next, StringComparison.Ordinal))
            {
                continue;
            }

            await _writer.WriteAsync($"\u001b[{i + 1};1H").ConfigureAwait(false);
            await _writer.WriteAsync(next).ConfigureAwait(false);
            await _writer.WriteAsync("\u001b[K").ConfigureAwait(false);
        }

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

        if (_altScreen)
        {
            await _writer.WriteAsync("\u001b[?1049l").ConfigureAwait(false);
            _altScreen = false;
        }

        await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        _previousLines.Clear();
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

}
