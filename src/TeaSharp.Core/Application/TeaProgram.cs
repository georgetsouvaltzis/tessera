using System.Threading.Channels;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;
using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

public sealed class TeaProgram
{
    private static readonly int[] DefaultCapabilityProbeModes = [1000, 1002, 1003, 1004, 1006, 2004, 2026];
    private static readonly int[] CapabilityProbeRepresentativeModes = [1004, 1006, 2004, 2026];

    private readonly ProgramOptions _options;
    private readonly Channel<IMessage> _messages;
    private readonly Channel<Command> _commands;
    private readonly object _stateLock = new();

    private ITerminalAdapter? _terminal;
    private IProgramRenderer? _renderer;
    private TerminalReader? _reader;
    private CancellationTokenSource? _cts;
    private bool _running;
    private CapabilityProbeState? _capabilityProbe;
    private TerminalCapabilityProfile _runtimeCapabilities = TerminalCapabilityProfile.AllSupported;
    private ExceptionDispatchInfo? _unhandledCommandException;

    public TeaProgram(IModel initialModel, ProgramOptions? options = null)
    {
        Model = initialModel ?? throw new ArgumentNullException(nameof(initialModel));
        _options = options ?? new ProgramOptions();
        _messages = Channel.CreateUnbounded<IMessage>();
        _commands = Channel.CreateUnbounded<Command>();
    }

    public IModel Model { get; private set; }

    public void Send(IMessage message)
    {
        if (message is null)
        {
            return;
        }

        _messages.Writer.TryWrite(message);
    }

    public async Task<IModel> RunAsync(CancellationToken cancellationToken = default)
    {
        IDisposable? resizeSignalRegistration = null;

        lock (_stateLock)
        {
            if (_running)
            {
                throw new InvalidOperationException("Program is already running.");
            }

            _running = true;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _unhandledCommandException = null;
            _capabilityProbe = null;
        }

        var token = _cts.Token;

        try
        {
            _terminal = _options.Terminal ?? new ConsoleTerminalAdapter();
            var capabilities = _options.TerminalCapabilities ?? TerminalCapabilityDetector.Detect();
            _runtimeCapabilities = capabilities;
            _renderer = _options.DisableRenderer
                ? new NullRenderer()
                : _options.Renderer ?? new AnsiDiffRenderer(capabilities);

            await _terminal.PrepareAsync(token).ConfigureAwait(false);
            await _renderer.InitializeAsync(_terminal.Output, token).ConfigureAwait(false);
            Send(new TerminalCapabilitiesMsg(capabilities));

            Task? resizeLoop = null;
            if (_terminal.IsOutputInteractive)
            {
                var size = await _terminal.GetSizeAsync(token).ConfigureAwait(false);
                _renderer.Resize(size.Width, size.Height);
                Send(new WindowSizeMsg(size.Width, size.Height));
                (resizeLoop, resizeSignalRegistration) = StartResizeLoop(size, token);
            }

            var commandLoop = Task.Run(() => CommandLoopAsync(token), token);
            var inputLoop = StartInputLoop(token);
            await StartCapabilityProbeAsync(token).ConfigureAwait(false);

            if (Model.Init() is { } initCommand)
            {
                await _commands.Writer.WriteAsync(initCommand, token).ConfigureAwait(false);
            }

            await RenderAsync(Model.View(), token).ConfigureAwait(false);

            var minFrame = TimeSpan.FromSeconds(1.0 / Math.Clamp(_options.MaxFps, 1, 120));
            var lastRender = DateTimeOffset.MinValue;

            while (await _messages.Reader.WaitToReadAsync(token).ConfigureAwait(false))
            {
                while (_messages.Reader.TryRead(out var message))
                {
                    if (message is CapabilityProbeTimeoutMsg probeTimeout)
                    {
                        HandleCapabilityProbeTimeout(probeTimeout);
                        continue;
                    }

                    if (message is ModeReportMsg probeModeReport)
                    {
                        ObserveCapabilityProbeResponse(probeModeReport);
                    }

                    var filtered = _options.Filter is null
                        ? message
                        : _options.Filter(Model, message);
                    if (filtered is null)
                    {
                        continue;
                    }

                    if (filtered is QuitMsg)
                    {
                        _cts?.Cancel();
                        await AwaitBackgroundLoops(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
                        await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
                        return Model;
                    }

                    if (filtered is InterruptMsg)
                    {
                        var unhandledCommandException = Interlocked.Exchange(ref _unhandledCommandException, null);
                        if (unhandledCommandException is not null)
                        {
                            unhandledCommandException.Throw();
                        }

                        throw new TeaProgramInterruptedException();
                    }

                    if (filtered is ModeReportMsg modeReport
                        && TryApplyModeReport(_runtimeCapabilities, modeReport, out var refinedCapabilities))
                    {
                        _runtimeCapabilities = refinedCapabilities;
                        Send(new TerminalCapabilitiesMsg(refinedCapabilities));
                    }

                    switch (filtered)
                    {
                        case WindowSizeMsg ws:
                            _renderer.Resize(ws.Width, ws.Height);
                            break;
                        case BatchMsg batch:
                            foreach (var command in batch.Commands)
                            {
                                await _commands.Writer.WriteAsync(command, token).ConfigureAwait(false);
                            }
                            continue;
                        case SequenceMsg sequence:
                            _ = Task.Run(() => RunSequenceAsync(sequence.Commands, token), token);
                            continue;
                    }

                    var update = Model.Update(filtered);
                    Model = update.Model;

                    if (update.Command is not null)
                    {
                        await _commands.Writer.WriteAsync(update.Command, token).ConfigureAwait(false);
                    }

                    var now = DateTimeOffset.UtcNow;
                    var elapsed = now - lastRender;
                    if (elapsed < minFrame)
                    {
                        var delay = minFrame - elapsed;
                        await Task.Delay(delay, token).ConfigureAwait(false);
                    }

                    await RenderAsync(Model.View(), token).ConfigureAwait(false);
                    lastRender = DateTimeOffset.UtcNow;
                }
            }

            _cts?.Cancel();
            await AwaitBackgroundLoops(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
            await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
            return Model;
        }
        finally
        {
            resizeSignalRegistration?.Dispose();

            if (_terminal is not null || _renderer is not null)
            {
                await ShutdownAsync(kill: true, CancellationToken.None).ConfigureAwait(false);
            }

            lock (_stateLock)
            {
                _running = false;
            }

            _cts?.Dispose();
            _cts = null;
        }
    }

    public async Task StopAsync(bool kill = false, CancellationToken cancellationToken = default)
    {
        if (_cts is null)
        {
            return;
        }

        _cts.Cancel();
        await ShutdownAsync(kill, cancellationToken).ConfigureAwait(false);
    }

    private Task? StartInputLoop(CancellationToken token)
    {
        if (_options.DisableInput || _terminal is null || !_terminal.IsInputInteractive)
        {
            return null;
        }

        if (_terminal is ConsoleTerminalAdapter consoleTerminal
            && (_options.UseConsoleKeyEvents || !consoleTerminal.IsRawModeActive))
        {
            return Task.Run(() => consoleTerminal.StreamConsoleKeyEventsAsync(token, Send), token);
        }

        _reader = new TerminalReader(_terminal.Input, new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _reader.StreamEventsAsync(token, Send), token);
    }

    private (Task? Loop, IDisposable? SignalRegistration) StartResizeLoop(TerminalSize initialSize, CancellationToken token)
    {
        if (_terminal is null || !_terminal.IsOutputInteractive)
        {
            return (null, null);
        }

        var signalTicks = Channel.CreateUnbounded<bool>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });

        var registration = TryRegisterResizeSignal(() => signalTicks.Writer.TryWrite(true));

        var loop = Task.Run(async () =>
        {
            var last = initialSize;
            var interval = _options.ResizePollInterval < TimeSpan.FromMilliseconds(16)
                ? TimeSpan.FromMilliseconds(16)
                : _options.ResizePollInterval;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var pollDelay = Task.Delay(interval, token);
                    var signalWait = signalTicks.Reader.WaitToReadAsync(token).AsTask();
                    await Task.WhenAny(pollDelay, signalWait).ConfigureAwait(false);

                    while (signalTicks.Reader.TryRead(out _))
                    {
                        // Drain queued resize signals.
                    }

                    var current = await _terminal.GetSizeAsync(token).ConfigureAwait(false);
                    if (current.Width != last.Width || current.Height != last.Height)
                    {
                        last = current;
                        Send(new WindowSizeMsg(current.Width, current.Height));
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // ignored: size monitoring best-effort.
                }
            }
        }, token);

        return (loop, registration);
    }

    private IDisposable? TryRegisterResizeSignal(Action onResize)
    {
        if (!_options.EnableResizeSignals)
        {
            return null;
        }

        if (_options.ResizeSignalRegistrationFactory is not null)
        {
            try
            {
                return _options.ResizeSignalRegistrationFactory(onResize);
            }
            catch
            {
                return null;
            }
        }

        if (_terminal is ConsoleTerminalAdapter consoleTerminal)
        {
            var windowsRegistration = consoleTerminal.TryRegisterResizeSignal(onResize);
            if (windowsRegistration is not null)
            {
                return windowsRegistration;
            }
        }

        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()))
        {
            return null;
        }

        try
        {
            return PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ =>
            {
                try
                {
                    onResize();
                }
                catch
                {
                    // ignored: callback must not throw.
                }
            });
        }
        catch
        {
            return null;
        }
    }

    private async Task CommandLoopAsync(CancellationToken token)
    {
        while (await _commands.Reader.WaitToReadAsync(token).ConfigureAwait(false))
        {
            while (_commands.Reader.TryRead(out var command))
            {
                _ = Task.Run(() => ExecuteCommandAsync(command, token), token);
            }
        }
    }

    private async Task ExecuteCommandAsync(Command command, CancellationToken token)
    {
        try
        {
            var message = await command(token).ConfigureAwait(false);
            if (message is not null)
            {
                Send(message);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // ignored: command canceled during shutdown.
        }
        catch (Exception ex)
        {
            if (_options.CatchCommandExceptions)
            {
                Send(new CommandErrorMsg(ex));
                return;
            }

            _ = Interlocked.CompareExchange(
                ref _unhandledCommandException,
                ExceptionDispatchInfo.Capture(ex),
                comparand: null);
            Send(new InterruptMsg());
        }
    }

    private async Task RunSequenceAsync(IReadOnlyList<Command> commands, CancellationToken token)
    {
        foreach (var command in commands)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await ExecuteCommandAsync(command, token).ConfigureAwait(false);
        }
    }

    private async Task RenderAsync(View view, CancellationToken token)
    {
        if (_renderer is null)
        {
            return;
        }

        _renderer.Render(view);
        await _renderer.FlushAsync(token).ConfigureAwait(false);
    }

    private async Task ShutdownAsync(bool kill, CancellationToken token)
    {
        _commands.Writer.TryComplete();
        _messages.Writer.TryComplete();

        if (_renderer is not null)
        {
            if (!kill)
            {
                await _renderer.ResetAsync(token).ConfigureAwait(false);
            }

            await _renderer.DisposeAsync().ConfigureAwait(false);
            _renderer = null;
        }

        if (_terminal is not null)
        {
            await _terminal.RestoreAsync(token).ConfigureAwait(false);
            await _terminal.DisposeAsync().ConfigureAwait(false);
            _terminal = null;
        }
    }

    private static async Task AwaitBackgroundLoops(Task commandLoop, Task? inputLoop, Task? resizeLoop)
    {
        try
        {
            await commandLoop.ConfigureAwait(false);
        }
        catch
        {
            // ignored: shutdown path.
        }

        if (inputLoop is not null)
        {
            try
            {
                await inputLoop.ConfigureAwait(false);
            }
            catch
            {
                // ignored: shutdown path.
            }
        }

        if (resizeLoop is not null)
        {
            try
            {
                await resizeLoop.ConfigureAwait(false);
            }
            catch
            {
                // ignored: shutdown path.
            }
        }
    }

    private static bool TryApplyModeReport(
        TerminalCapabilityProfile current,
        ModeReportMsg report,
        out TerminalCapabilityProfile next)
    {
        next = current;
        if (!TryClassifyModeReportState(report.State, out var supported, out var enabled))
        {
            return false;
        }

        var isTrackedMode = report.Mode is 1004 or 1006 or 2004 or 2026;
        if (!isTrackedMode)
        {
            return false;
        }

        var updated = report.Mode switch
        {
            1004 => current with { FocusReporting = supported, ModeReports = true },
            1006 => current with { MouseReporting = supported, ModeReports = true },
            2004 => current with { BracketedPaste = supported, ModeReports = true },
            2026 => current with { SynchronizedUpdates = supported, ModeReports = true },
            _ => current,
        };

        var source = updated.Source;
        if (!source.Contains("+mode-report", StringComparison.Ordinal))
        {
            source += "+mode-report";
        }

        if (!supported && !source.Contains("+mode-report-unsupported", StringComparison.Ordinal))
        {
            source += "+mode-report-unsupported";
        }
        else if (supported && !enabled && !source.Contains("+mode-report-reset", StringComparison.Ordinal))
        {
            source += "+mode-report-reset";
        }

        next = updated with { Source = source };
        return next != current;
    }

    private static bool TryClassifyModeReportState(ModeReportState state, out bool supported, out bool enabled)
    {
        supported = false;
        enabled = false;
        switch (state)
        {
            case ModeReportState.Unsupported:
                supported = false;
                enabled = false;
                return true;
            case ModeReportState.Set:
            case ModeReportState.PermanentlySet:
                supported = true;
                enabled = true;
                return true;
            case ModeReportState.Reset:
            case ModeReportState.PermanentlyReset:
                supported = true;
                enabled = false;
                return true;
            default:
                return false;
        }
    }

    private async Task StartCapabilityProbeAsync(CancellationToken token)
    {
        if (_terminal is null
            || _options.DisableInput
            || !_options.EnableCapabilityProbe
            || !_terminal.IsInputInteractive
            || !_terminal.IsOutputInteractive
            || !_runtimeCapabilities.ModeReports)
        {
            return;
        }

        var modes = _options.CapabilityProbeModes is { Count: > 0 }
            ? _options.CapabilityProbeModes
            : DefaultCapabilityProbeModes;
        if (modes.Count == 0)
        {
            return;
        }

        var probe = new CapabilityProbeState(Guid.NewGuid(), modes);
        _capabilityProbe = probe;
        await SendCapabilityProbeQueriesAsync(modes, token).ConfigureAwait(false);

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_options.CapabilityProbeTimeout, token).ConfigureAwait(false);
                Send(new CapabilityProbeTimeoutMsg(probe.Id));
            }
            catch (OperationCanceledException)
            {
                // ignored: normal shutdown path.
            }
        }, CancellationToken.None);
    }

    private async Task SendCapabilityProbeQueriesAsync(IReadOnlyList<int> modes, CancellationToken token)
    {
        if (_terminal is null || modes.Count == 0)
        {
            return;
        }

        var sequence = new StringBuilder(modes.Count * 10);
        foreach (var mode in modes)
        {
            sequence.Append("\u001b[?");
            sequence.Append(mode);
            sequence.Append("$p");
        }

        try
        {
            var bytes = Encoding.ASCII.GetBytes(sequence.ToString());
            await _terminal.Output.WriteAsync(bytes, token).ConfigureAwait(false);
            await _terminal.Output.FlushAsync(token).ConfigureAwait(false);
        }
        catch
        {
            // ignored: probe is best-effort.
        }
    }

    private void ObserveCapabilityProbeResponse(ModeReportMsg report)
    {
        if (_capabilityProbe is null)
        {
            return;
        }

        if (!_capabilityProbe.PendingModes.Remove(report.Mode))
        {
            return;
        }

        _capabilityProbe.SawAnyResponse = true;
        if (_capabilityProbe.PendingModes.Count == 0)
        {
            _capabilityProbe = null;
        }
    }

    private void HandleCapabilityProbeTimeout(CapabilityProbeTimeoutMsg timeout)
    {
        if (_capabilityProbe is null || _capabilityProbe.Id != timeout.ProbeId)
        {
            return;
        }

        var sawAnyResponse = _capabilityProbe.SawAnyResponse;
        var unresolvedModes = _capabilityProbe.PendingModes
            .Where(IsCapabilityRepresentativeProbeMode)
            .ToArray();
        _capabilityProbe = null;
        if (!sawAnyResponse)
        {
            if (!_runtimeCapabilities.ModeReports)
            {
                return;
            }

            var source = _runtimeCapabilities.Source;
            if (!source.Contains("+probe-timeout", StringComparison.Ordinal))
            {
                source += "+probe-timeout";
            }

            _runtimeCapabilities = _runtimeCapabilities with
            {
                ModeReports = false,
                Source = source,
            };
            Send(new TerminalCapabilitiesMsg(_runtimeCapabilities));
            return;
        }

        var next = _runtimeCapabilities;
        foreach (var unresolvedMode in unresolvedModes)
        {
            next = unresolvedMode switch
            {
                1004 => next with { FocusReporting = false },
                1006 => next with { MouseReporting = false },
                2004 => next with { BracketedPaste = false },
                2026 => next with { SynchronizedUpdates = false },
                _ => next,
            };
        }

        if (next == _runtimeCapabilities)
        {
            return;
        }

        var nextSource = next.Source;
        if (!nextSource.Contains("+probe-partial-timeout", StringComparison.Ordinal))
        {
            nextSource += "+probe-partial-timeout";
        }

        _runtimeCapabilities = next with { Source = nextSource };
        Send(new TerminalCapabilitiesMsg(_runtimeCapabilities));
    }

    private static bool IsCapabilityRepresentativeProbeMode(int mode)
    {
        for (var i = 0; i < CapabilityProbeRepresentativeModes.Length; i++)
        {
            if (CapabilityProbeRepresentativeModes[i] == mode)
            {
                return true;
            }
        }

        return false;
    }

    private sealed class CapabilityProbeState
    {
        public CapabilityProbeState(Guid id, IReadOnlyList<int> modes)
        {
            Id = id;
            PendingModes = new HashSet<int>(modes);
        }

        public Guid Id { get; }

        public HashSet<int> PendingModes { get; }

        public bool SawAnyResponse { get; set; }
    }

    private sealed record CapabilityProbeTimeoutMsg(Guid ProbeId) : IMessage;
}
