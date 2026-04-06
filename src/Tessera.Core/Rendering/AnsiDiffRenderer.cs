using System.ComponentModel;
using System.Globalization;
using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Rendering.Internal;
using Tessera.Core.Terminal;

namespace Tessera.Core.Rendering;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class AnsiDiffRenderer : IProgramRenderer
{
    private readonly AnsiRendererOptions _options;
    private Stream? _output;
    private StreamWriter? _writer;
    private RenderFrameBuffer _previousFrame = RenderFrameBuffer.Empty;
    private ScreenOutput _currentOutput = ScreenOutput.From(string.Empty);
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
    private string? _fontSpec;
    private string? _iterm2Profile;
    private int _width;
    private int _height;
    private bool _fullRepaintRequired;
    private TerminalCapabilityProfile _capabilities;

    public AnsiDiffRenderer(
        TerminalCapabilityProfile? capabilities = null,
        AnsiRendererOptions? options = null)
    {
        _capabilities = capabilities ?? TerminalCapabilityProfile.AllSupported;
        _options = options ?? new AnsiRendererOptions();
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
        _fontSpec = null;
        _iterm2Profile = null;
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

    public void Render(ScreenOutput output)
    {
        _currentOutput = output;
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        if (!_initialized || _writer is null)
        {
            return;
        }

        var terminal = _currentOutput.Terminal;
        var frame = _currentOutput.Frame;

        if (terminal.AltScreen != _altScreen)
        {
            await _writer.WriteAsync(terminal.AltScreen ? "\u001b[?1049h" : "\u001b[?1049l")
                .ConfigureAwait(false);
            _altScreen = terminal.AltScreen;
            _previousFrame = RenderFrameBuffer.Empty;
            if (_keyboardEnhancementFlags != 0)
            {
                await _writer.WriteAsync("\u001b[>0u").ConfigureAwait(false);
                _keyboardEnhancementFlags = 0;
            }
        }

        var requestedBracketedPaste = terminal.EnableBracketedPaste && _capabilities.BracketedPaste;
        if (requestedBracketedPaste != _bracketedPaste)
        {
            await _writer.WriteAsync(requestedBracketedPaste ? "\u001b[?2004h" : "\u001b[?2004l").ConfigureAwait(false);
            _bracketedPaste = requestedBracketedPaste;
            if (requestedBracketedPaste)
            {
                await QueryModeReportAsync(2004).ConfigureAwait(false);
            }
        }

        var requestedFocusReporting = terminal.EnableFocusReporting && _capabilities.FocusReporting;
        if (requestedFocusReporting != _focusReporting)
        {
            await _writer.WriteAsync(requestedFocusReporting ? "\u001b[?1004h" : "\u001b[?1004l").ConfigureAwait(false);
            _focusReporting = requestedFocusReporting;
            if (requestedFocusReporting)
            {
                await QueryModeReportAsync(1004).ConfigureAwait(false);
            }
        }

        var requestedSyncUpdates = terminal.EnableSynchronizedUpdates && _capabilities.SynchronizedUpdates;
        if (requestedSyncUpdates)
        {
            await _writer.WriteAsync("\u001b[?2026h").ConfigureAwait(false);
            await QueryModeReportAsync(2026).ConfigureAwait(false);
        }

        var requestedMouseMode = _capabilities.MouseReporting
            ? terminal.MouseMode
            : MouseMode.None;
        if (requestedMouseMode != _mouseMode)
        {
            await WriteMouseModeAsync(requestedMouseMode).ConfigureAwait(false);
            _mouseMode = requestedMouseMode;
            if (requestedMouseMode != MouseMode.None)
            {
                await QueryModeReportAsync(1006).ConfigureAwait(false);
            }
        }

        if (!string.Equals(_windowTitle, terminal.WindowTitle, StringComparison.Ordinal))
        {
            if (terminal.WindowTitle is not null)
            {
                await _writer.WriteAsync($"\u001b]2;{terminal.WindowTitle}\u0007").ConfigureAwait(false);
            }

            _windowTitle = terminal.WindowTitle;
        }

        var requestedIterm2Profile = SanitizeIterm2Profile(terminal.Iterm2Profile);
        var requestedFontSpec = SanitizeFontSpec(terminal.FontSpec)
            ?? BuildStructuredFontSpec(terminal.FontFamily, terminal.FontSize);
        var shouldPreferIterm2Profile = _capabilities.SupportsIterm2ProfileRequests
            && requestedIterm2Profile is not null;
        if (_capabilities.SupportsIterm2ProfileRequests)
        {
            if (!string.Equals(_iterm2Profile, requestedIterm2Profile, StringComparison.Ordinal))
            {
                if (requestedIterm2Profile is not null)
                {
                    await _writer.WriteAsync($"\u001b]1337;SetProfile={requestedIterm2Profile}\u0007").ConfigureAwait(false);
                }

                _iterm2Profile = requestedIterm2Profile;
            }
        }
        else
        {
            _iterm2Profile = null;
        }

        if (_capabilities.SupportsOsc50FontRequests && !shouldPreferIterm2Profile)
        {
            if (!string.Equals(_fontSpec, requestedFontSpec, StringComparison.Ordinal))
            {
                if (requestedFontSpec is not null)
                {
                    await _writer.WriteAsync($"\u001b]50;{requestedFontSpec}\u0007").ConfigureAwait(false);
                }

                _fontSpec = requestedFontSpec;
            }
        }
        else
        {
            _fontSpec = null;
        }

        var requestedKeyboardFlags = GetKeyboardEnhancementFlags(terminal.KeyboardEnhancements);
        if (requestedKeyboardFlags != _keyboardEnhancementFlags)
        {
            await _writer.WriteAsync($"\u001b[>{requestedKeyboardFlags}u").ConfigureAwait(false);
            _keyboardEnhancementFlags = requestedKeyboardFlags;
        }

        var requestedForegroundColor = AnsiColorNormalizer.NormalizeHex(terminal.ForegroundColor);
        if (!string.Equals(_foregroundColor, requestedForegroundColor, StringComparison.Ordinal))
        {
            await WriteTerminalColorAsync(10, 110, requestedForegroundColor).ConfigureAwait(false);
            _foregroundColor = requestedForegroundColor;
        }

        var requestedBackgroundColor = AnsiColorNormalizer.NormalizeHex(terminal.BackgroundColor);
        if (!string.Equals(_backgroundColor, requestedBackgroundColor, StringComparison.Ordinal))
        {
            await WriteTerminalColorAsync(11, 111, requestedBackgroundColor).ConfigureAwait(false);
            _backgroundColor = requestedBackgroundColor;
        }

        var requestedCursorColor = AnsiColorNormalizer.NormalizeHex(terminal.CursorColor);
        if (!string.Equals(_cursorColor, requestedCursorColor, StringComparison.Ordinal))
        {
            await WriteTerminalColorAsync(12, 112, requestedCursorColor).ConfigureAwait(false);
            _cursorColor = requestedCursorColor;
        }

        if (_progress != terminal.Progress)
        {
            await WriteProgressAsync(terminal.Progress).ConfigureAwait(false);
            _progress = terminal.Progress;
        }

        if (_fullRepaintRequired)
        {
            await _writer.WriteAsync("\u001b[2J\u001b[H").ConfigureAwait(false);
            _previousFrame = RenderFrameBuffer.Empty;
            _fullRepaintRequired = false;
        }

        var nextFrame = RenderFrameBuffer.FromContent(frame.Content, _width, _height);
        await WriteFrameDiffAsync(nextFrame).ConfigureAwait(false);

        if (frame.CursorX is int x && frame.CursorY is int y)
        {
            if (frame.CursorStyle is CursorStyle requestedCursorStyle
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

        if (cancellationToken.CanBeCanceled && _options.FlushTimeout > TimeSpan.Zero)
        {
            using var timeoutCts = new CancellationTokenSource(_options.FlushTimeout);
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

        // Font restore sequence is intentionally omitted because no portable "reset to previous font" contract exists.
        _fontSpec = null;
        _iterm2Profile = null;

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
        await AnsiFrameDiffer.WriteAsync(_writer, _previousFrame, nextFrame, _width, _height).ConfigureAwait(false);
    }

    private Task WriteMouseModeAsync(MouseMode mode)
    {
        if (_writer is null)
        {
            return Task.CompletedTask;
        }

        return _writer.WriteAsync(AnsiEscapeSequences.SequenceForMouseMode(mode));
    }

    private async Task WriteTerminalColorAsync(int setCode, int resetCode, string? color)
    {
        if (_writer is null)
        {
            return;
        }

        await _writer.WriteAsync(AnsiEscapeSequences.TerminalColor(setCode, resetCode, color)).ConfigureAwait(false);
    }

    private async Task WriteProgressAsync(TerminalProgress? progress)
    {
        if (_writer is null)
        {
            return;
        }

        await _writer.WriteAsync(AnsiEscapeSequences.Progress(progress)).ConfigureAwait(false);
    }

    private Task WriteCursorStyleAsync(CursorStyle style)
    {
        if (_writer is null)
        {
            return Task.CompletedTask;
        }

        return _writer.WriteAsync(AnsiEscapeSequences.SequenceForCursorStyle(style));
    }

    private Task QueryModeReportAsync(int mode)
    {
        if (_writer is null || !_capabilities.ModeReports || !_options.QueryModeReports)
        {
            return Task.CompletedTask;
        }

        if (_options.QueryModeReportsOncePerMode && !_queriedModes.Add(mode))
        {
            return Task.CompletedTask;
        }

        return _writer.WriteAsync($"\u001b[?{mode}$p");
    }

    private int GetKeyboardEnhancementFlags(KeyboardEnhancementOptions options)
    {
        return AnsiEscapeSequences.KeyboardEnhancementFlags(options, _options.IncludeKittyKeyboardBaseFlag);
    }

    private static string? SanitizeFontSpec(string? fontSpec)
    {
        if (string.IsNullOrWhiteSpace(fontSpec))
        {
            return null;
        }

        var builder = new StringBuilder(fontSpec.Length);
        for (var index = 0; index < fontSpec.Length; index++)
        {
            var character = fontSpec[index];
            if (character is '\u001b' or '\u0007' or '\\')
            {
                continue;
            }

            if (char.IsControl(character))
            {
                continue;
            }

            _ = builder.Append(character);
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    private static string? SanitizeIterm2Profile(string? profile)
    {
        if (string.IsNullOrWhiteSpace(profile))
        {
            return null;
        }

        var builder = new StringBuilder(profile.Length);
        for (var index = 0; index < profile.Length; index++)
        {
            var character = profile[index];
            if (character is '\u001b' or '\u0007' or '\\' or ';')
            {
                continue;
            }

            if (char.IsControl(character))
            {
                continue;
            }

            _ = builder.Append(character);
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    private static string? BuildStructuredFontSpec(string? family, int? size)
    {
        var sanitizedFamily = SanitizeFontSpec(family);
        if (sanitizedFamily is null)
        {
            return null;
        }

        if (size is null || size <= 0)
        {
            return sanitizedFamily;
        }

        return string.Concat(sanitizedFamily, " ", size.Value.ToString(CultureInfo.InvariantCulture));
    }
}
