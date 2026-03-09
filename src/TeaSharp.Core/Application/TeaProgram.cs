using System.Threading.Channels;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

public sealed class TeaProgram
{
    private readonly ProgramOptions _options;
    private readonly Channel<IMessage> _messages;
    private readonly Channel<Command> _commands;
    private readonly object _stateLock = new();
    private readonly TeaCapabilityProbe _capabilityProbe = new();

    private ITerminalAdapter? _terminal;
    private IProgramRenderer? _renderer;
    private TerminalReader? _reader;
    private CancellationTokenSource? _cts;
    private TeaProgramCommandScheduler? _commandScheduler;
    private bool _running;
    private TerminalCapabilityProfile _runtimeCapabilities = TerminalCapabilityProfile.AllSupported;
    private TerminalColorProfile _runtimeColorProfile = TerminalColorProfile.Unknown;
    private View _lastRenderedView = View.From(string.Empty);

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
        if (message is not null)
        {
            _messages.Writer.TryWrite(message);
        }
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
        }

        var token = _cts.Token;

        try
        {
            _terminal = _options.Terminal ?? new ConsoleTerminalAdapter();
            _runtimeCapabilities = _options.TerminalCapabilities
                ?? _options.TerminalCapabilityDetector?.Invoke()
                ?? TerminalCapabilityDetector.Detect();
            _runtimeColorProfile = _options.ColorProfile
                ?? _options.ColorProfileDetector?.Invoke()
                ?? TerminalColorProfileDetector.Detect();
            _renderer = _options.DisableRenderer
                ? new NullRenderer()
                : _options.Renderer ?? new AnsiDiffRenderer(_runtimeCapabilities, _options.AnsiRendererOptions);
            _renderer.UpdateCapabilities(_runtimeCapabilities);
            _commandScheduler = new TeaProgramCommandScheduler(_options, Send);

            await _terminal.PrepareAsync(token).ConfigureAwait(false);
            await _renderer.InitializeAsync(_terminal.Output, token).ConfigureAwait(false);
            Send(new TerminalCapabilitiesMsg(_runtimeCapabilities));
            Send(new ColorProfileMsg(_runtimeColorProfile));

            Task? resizeLoop = null;
            if (_terminal.IsOutputInteractive)
            {
                var size = await _terminal.GetSizeAsync(token).ConfigureAwait(false);
                _renderer.Resize(size.Width, size.Height);
                Send(new WindowSizeMsg(size.Width, size.Height));
                (resizeLoop, resizeSignalRegistration) = TeaProgramResizeMonitor.Start(_terminal, _options, size, Send, token);
            }

            var commandLoop = Task.Run(() => _commandScheduler.RunLoopAsync(_commands.Reader, token), token);
            var inputLoop = StartInputLoop(token);
            await _capabilityProbe.StartAsync(_terminal, _options, _runtimeCapabilities, Send, token).ConfigureAwait(false);

            if (Model.Init() is { } initCommand)
            {
                await _commands.Writer.WriteAsync(initCommand, token).ConfigureAwait(false);
            }

            await RenderAsync(Model.View(), token).ConfigureAwait(false);
            await ProcessMessageLoopAsync(commandLoop, inputLoop, resizeLoop, token).ConfigureAwait(false);
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
            _commandScheduler?.Dispose();
            _commandScheduler = null;
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

    private async Task ProcessMessageLoopAsync(Task commandLoop, Task? inputLoop, Task? resizeLoop, CancellationToken token)
    {
        var minFrame = TimeSpan.FromSeconds(1.0 / Math.Clamp(_options.MaxFps, 1, 120));
        var lastRender = DateTimeOffset.UtcNow;
        var pendingRender = false;

        while (await _messages.Reader.WaitToReadAsync(token).ConfigureAwait(false))
        {
            while (_messages.Reader.TryRead(out var message))
            {
                if (message is TeaCapabilityProbe.CapabilityProbeTimeoutMsg probeTimeout)
                {
                    _capabilityProbe.HandleTimeout(probeTimeout, ref _runtimeCapabilities, _renderer, Send);
                    continue;
                }

                if (message is ModeReportMsg probeModeReport)
                {
                    _capabilityProbe.Observe(probeModeReport);
                }

                var filtered = _options.Filter is null ? message : _options.Filter(Model, message);
                if (filtered is null)
                {
                    continue;
                }

                if (filtered is QuitMsg)
                {
                    _cts?.Cancel();
                    await AwaitBackgroundLoops(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
                    await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
                    return;
                }

                if (filtered is InterruptMsg)
                {
                    var unhandledCommandException = _commandScheduler?.ConsumeUnhandledException();
                    if (unhandledCommandException is not null)
                    {
                        unhandledCommandException.Throw();
                    }

                    throw new TeaProgramInterruptedException();
                }

                if (await TryHandleCommandEnvelopeAsync(filtered, token).ConfigureAwait(false))
                {
                    continue;
                }

                if (filtered is ModeReportMsg modeReport
                    && TeaCapabilityProbe.TryApplyModeReport(_runtimeCapabilities, modeReport, out var refinedCapabilities))
                {
                    _runtimeCapabilities = refinedCapabilities;
                    _renderer?.UpdateCapabilities(refinedCapabilities);
                    Send(new TerminalCapabilitiesMsg(refinedCapabilities));
                }

                if (await TryHandleMessageSideEffectsAsync(filtered, token).ConfigureAwait(false))
                {
                    continue;
                }

                var update = Model.Update(filtered);
                Model = update.Model;

                if (update.Command is not null)
                {
                    await _commands.Writer.WriteAsync(update.Command, token).ConfigureAwait(false);
                }

                pendingRender = true;
                var renderAttempt = await TryRenderFrameAsync(minFrame, lastRender, pendingRender, token).ConfigureAwait(false);
                lastRender = renderAttempt.LastRender;
                pendingRender = renderAttempt.PendingRender;
                if (renderAttempt.Rendered)
                {
                    continue;
                }
            }

            if (_options.AdaptiveFramePacing && pendingRender)
            {
                var delayedRender = await DelayAndRenderAsync(minFrame, lastRender, token).ConfigureAwait(false);
                lastRender = delayedRender.LastRender;
                pendingRender = false;
            }
        }

        _cts?.Cancel();
        await AwaitBackgroundLoops(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
        await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task<bool> TryHandleCommandEnvelopeAsync(IMessage filtered, CancellationToken token)
    {
        if (filtered is SequenceMsg sequence)
        {
            _ = Task.Run(() => _commandScheduler!.RunSequenceAsync(sequence.Commands, token), token);
            return true;
        }

        if (filtered is BatchMsg batch)
        {
            foreach (var command in batch.Commands)
            {
                await _commands.Writer.WriteAsync(command, token).ConfigureAwait(false);
            }

            return true;
        }

        return false;
    }

    private async Task<bool> TryHandleMessageSideEffectsAsync(IMessage filtered, CancellationToken token)
    {
        switch (filtered)
        {
            case RawOutputMsg rawOutput:
                if (_renderer is not null)
                {
                    await _renderer.WriteRawAsync(rawOutput.Content, token).ConfigureAwait(false);
                }

                return true;
            case WindowSizeMsg ws:
                _renderer?.Resize(ws.Width, ws.Height);
                break;
        }

        if (filtered is CapabilityMsg capability
            && TeaCapabilityProbe.TryRefineColorProfile(_runtimeColorProfile, capability, out var refinedColorProfile))
        {
            _runtimeColorProfile = refinedColorProfile;
            Send(new ColorProfileMsg(refinedColorProfile));
        }

        if (filtered is MouseMsg mouse && _lastRenderedView.OnMouse is { } onMouse)
        {
            var callbackCommand = onMouse(mouse);
            if (callbackCommand is not null)
            {
                await _commands.Writer.WriteAsync(callbackCommand, token).ConfigureAwait(false);
            }
        }

        return false;
    }

    private async Task<(bool Rendered, DateTimeOffset LastRender, bool PendingRender)> TryRenderFrameAsync(
        TimeSpan minFrame,
        DateTimeOffset lastRender,
        bool pendingRender,
        CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var elapsed = now - lastRender;
        if (!_options.AdaptiveFramePacing)
        {
            if (elapsed < minFrame)
            {
                await Task.Delay(minFrame - elapsed, token).ConfigureAwait(false);
            }

            await RenderAsync(Model.View(), token).ConfigureAwait(false);
            return (true, DateTimeOffset.UtcNow, false);
        }

        if (elapsed >= minFrame)
        {
            await RenderAsync(Model.View(), token).ConfigureAwait(false);
            return (true, DateTimeOffset.UtcNow, false);
        }

        return (false, lastRender, pendingRender);
    }

    private async Task<(DateTimeOffset LastRender, bool PendingRender)> DelayAndRenderAsync(
        TimeSpan minFrame,
        DateTimeOffset lastRender,
        CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var elapsed = now - lastRender;
        if (elapsed < minFrame)
        {
            await Task.Delay(minFrame - elapsed, token).ConfigureAwait(false);
        }

        await RenderAsync(Model.View(), token).ConfigureAwait(false);
        return (DateTimeOffset.UtcNow, false);
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

        _reader = new TerminalReader(_terminal.Input, _options.EventDecoder ?? new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _reader.StreamEventsAsync(token, Send), token);
    }

    private async Task RenderAsync(View view, CancellationToken token)
    {
        if (_renderer is null)
        {
            return;
        }

        _lastRenderedView = view;
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
        }

        if (inputLoop is not null)
        {
            try
            {
                await inputLoop.ConfigureAwait(false);
            }
            catch
            {
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
            }
        }
    }
}
