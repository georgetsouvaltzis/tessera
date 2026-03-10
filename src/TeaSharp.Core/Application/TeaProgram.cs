using System.Threading.Channels;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

/// <summary>
/// Runs a TeaSharp model inside the runtime event loop.
/// </summary>
public sealed class TeaProgram
{
    private readonly ProgramOptions _options;
    private readonly Channel<IMessage> _messages;
    private readonly Channel<Command> _commands;
    private readonly object _stateLock = new();
    private readonly TeaCapabilityProbe _capabilityProbe = new();
    private readonly TeaProgramRuntimeState _runtime = new();
    private CancellationTokenSource? _cts;
    private bool _running;

    /// <summary>
    /// Initializes a program for the provided model.
    /// </summary>
    /// <param name="initialModel">The initial application model.</param>
    /// <param name="options">Advanced runtime options.</param>
    public TeaProgram(IModel initialModel, ProgramOptions? options = null)
    {
        Model = initialModel ?? throw new ArgumentNullException(nameof(initialModel));
        _options = options ?? new ProgramOptions();
        _messages = Channel.CreateUnbounded<IMessage>();
        _commands = Channel.CreateUnbounded<Command>();
    }

    /// <summary>
    /// Gets the current application model.
    /// </summary>
    public IModel Model { get; private set; }

    /// <summary>
    /// Enqueues a message for delivery to the running program.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    public void Send(IMessage message)
    {
        if (message is not null)
        {
            _messages.Writer.TryWrite(message);
        }
    }

    /// <summary>
    /// Runs the program until it exits or the provided token is canceled.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels program execution.</param>
    /// <returns>The final application model.</returns>
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
            _runtime.Terminal = _options.Terminal ?? new ConsoleTerminalAdapter();
            _runtime.Capabilities = _options.TerminalCapabilities
                ?? _options.TerminalCapabilityDetector?.Invoke()
                ?? TerminalCapabilityDetector.Detect();
            _runtime.ColorProfile = _options.ColorProfile
                ?? _options.ColorProfileDetector?.Invoke()
                ?? TerminalColorProfileDetector.Detect();
            _runtime.Renderer = _options.DisableRenderer
                ? new NullRenderer()
                : _options.Renderer ?? new AnsiDiffRenderer(_runtime.Capabilities, _options.AnsiRendererOptions);
            _runtime.Renderer.UpdateCapabilities(_runtime.Capabilities);
            _runtime.CommandScheduler = new TeaProgramCommandScheduler(_options, Send);

            await _runtime.Terminal.PrepareAsync(token).ConfigureAwait(false);
            await _runtime.Renderer.InitializeAsync(_runtime.Terminal.Output, token).ConfigureAwait(false);
            Send(new TerminalCapabilitiesMsg(_runtime.Capabilities));
            Send(new ColorProfileMsg(_runtime.ColorProfile));

            Task? resizeLoop = null;
            if (_runtime.Terminal.IsOutputInteractive)
            {
                var size = await _runtime.Terminal.GetSizeAsync(token).ConfigureAwait(false);
                _runtime.Renderer.Resize(size.Width, size.Height);
                Send(new WindowSizeMsg(size.Width, size.Height));
                (resizeLoop, resizeSignalRegistration) = TeaProgramResizeMonitor.Start(_runtime.Terminal, _options, size, Send, token);
            }

            var commandLoop = Task.Run(() => _runtime.CommandScheduler!.RunLoopAsync(_commands.Reader, token), token);
            var inputLoop = StartInputLoop(token);
            await _capabilityProbe.StartAsync(_runtime.Terminal, _options, _runtime.Capabilities, Send, token).ConfigureAwait(false);

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
            if (_runtime.Terminal is not null || _runtime.Renderer is not null)
            {
                await ShutdownAsync(kill: true, CancellationToken.None).ConfigureAwait(false);
            }

            lock (_stateLock)
            {
                _running = false;
            }

            _cts?.Dispose();
            _cts = null;
            _runtime.CommandScheduler?.Dispose();
            _runtime.CommandScheduler = null;
        }
    }

    /// <summary>
    /// Requests program shutdown and waits for runtime cleanup to complete.
    /// </summary>
    /// <param name="kill">When <see langword="true"/>, forces terminal teardown without a graceful quit message.</param>
    /// <param name="cancellationToken">A token that cancels the stop operation.</param>
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
                    var capabilities = _runtime.Capabilities;
                    _capabilityProbe.HandleTimeout(probeTimeout, ref capabilities, _runtime.Renderer, Send);
                    _runtime.Capabilities = capabilities;
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
                    await TeaProgramBackgroundLoops.AwaitAsync(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
                    await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
                    return;
                }

                if (filtered is InterruptMsg)
                {
                    var unhandledCommandException = _runtime.CommandScheduler?.ConsumeUnhandledException();
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
                    && TeaCapabilityProbe.TryApplyModeReport(_runtime.Capabilities, modeReport, out var refinedCapabilities))
                {
                    _runtime.Capabilities = refinedCapabilities;
                    _runtime.Renderer?.UpdateCapabilities(refinedCapabilities);
                    Send(new TerminalCapabilitiesMsg(refinedCapabilities));
                }

                if (await TryHandleMessageSideEffectsAsync(filtered, token).ConfigureAwait(false))
                {
                    continue;
                }

                var command = Model.Update(filtered);

                if (command is not null)
                {
                    await _commands.Writer.WriteAsync(command, token).ConfigureAwait(false);
                }

                pendingRender = true;
                var renderAttempt = await TeaProgramFramePacer.TryRenderAsync(_options.AdaptiveFramePacing, minFrame, lastRender, pendingRender, () => RenderAsync(Model.View(), token), token).ConfigureAwait(false);
                lastRender = renderAttempt.LastRender;
                pendingRender = renderAttempt.PendingRender;
                if (renderAttempt.Rendered)
                {
                    continue;
                }
            }

            if (_options.AdaptiveFramePacing && pendingRender)
            {
                var delayedRender = await TeaProgramFramePacer.DelayAndRenderAsync(minFrame, lastRender, () => RenderAsync(Model.View(), token), token).ConfigureAwait(false);
                lastRender = delayedRender.LastRender;
                pendingRender = false;
            }
        }

        _cts?.Cancel();
        await TeaProgramBackgroundLoops.AwaitAsync(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
        await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task<bool> TryHandleCommandEnvelopeAsync(IMessage filtered, CancellationToken token)
    {
        if (filtered is SequenceMsg sequence)
        {
            _ = Task.Run(() => _runtime.CommandScheduler!.RunSequenceAsync(sequence.Commands, token), token);
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
                if (_runtime.Renderer is not null)
                {
                    await _runtime.Renderer.WriteRawAsync(rawOutput.Content, token).ConfigureAwait(false);
                }

                return true;
            case WindowSizeMsg ws:
                _runtime.Renderer?.Resize(ws.Width, ws.Height);
                break;
        }

        if (filtered is CapabilityMsg capability
            && TeaCapabilityProbe.TryRefineColorProfile(_runtime.ColorProfile, capability, out var refinedColorProfile))
        {
            _runtime.ColorProfile = refinedColorProfile;
            Send(new ColorProfileMsg(refinedColorProfile));
        }

        if (filtered is MouseMsg mouse && _runtime.LastRenderedView.Input.OnMouse is { } onMouse)
        {
            var callbackCommand = onMouse(mouse);
            if (callbackCommand is not null)
            {
                await _commands.Writer.WriteAsync(callbackCommand, token).ConfigureAwait(false);
            }
        }

        return false;
    }

    private Task? StartInputLoop(CancellationToken token)
    {
        if (_options.DisableInput || _runtime.Terminal is null || !_runtime.Terminal.IsInputInteractive)
        {
            return null;
        }

        if (_runtime.Terminal is ConsoleTerminalAdapter consoleTerminal
            && (_options.UseConsoleKeyEvents || !consoleTerminal.IsRawModeActive))
        {
            return Task.Run(() => consoleTerminal.StreamConsoleKeyEventsAsync(token, Send), token);
        }

        _runtime.Reader = new TerminalReader(_runtime.Terminal.Input, _options.EventDecoder ?? new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _runtime.Reader.StreamEventsAsync(token, Send), token);
    }

    private async Task RenderAsync(View view, CancellationToken token)
    {
        if (_runtime.Renderer is null)
        {
            return;
        }

        _runtime.LastRenderedView = view;
        _runtime.Renderer.Render(view);
        await _runtime.Renderer.FlushAsync(token).ConfigureAwait(false);
    }

    private async Task ShutdownAsync(bool kill, CancellationToken token)
    {
        _commands.Writer.TryComplete();
        _messages.Writer.TryComplete();

        if (_runtime.Renderer is not null)
        {
            if (!kill)
            {
                await _runtime.Renderer.ResetAsync(token).ConfigureAwait(false);
            }

            await _runtime.Renderer.DisposeAsync().ConfigureAwait(false);
            _runtime.Renderer = null;
        }

        if (_runtime.Terminal is not null)
        {
            await _runtime.Terminal.RestoreAsync(token).ConfigureAwait(false);
            await _runtime.Terminal.DisposeAsync().ConfigureAwait(false);
            _runtime.Terminal = null;
        }

        _runtime.Reader = null;
        _runtime.LastRenderedView = View.From(string.Empty);
    }

}
