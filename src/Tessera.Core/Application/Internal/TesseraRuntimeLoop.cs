using System.Threading.Channels;
using Tessera.Core.Abstractions;
using Tessera.Core.Input;
using Tessera.Core.Messages;
using Tessera.Core.Rendering;

namespace Tessera.Core.Application.Internal;

internal sealed class TesseraRuntimeLoop(
    Func<Effect?>? initialize,
    Func<IMessage, Effect?> update,
    Func<ScreenOutput> render,
    TesseraRuntimeLoopOptions? options = null)
{
    private static readonly UnboundedChannelOptions MessageChannelOptions = new()
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = true,
    };

    private static readonly UnboundedChannelOptions EffectChannelOptions = new()
    {
        SingleReader = true,
        SingleWriter = true,
        AllowSynchronousContinuations = true,
    };

    private readonly Func<Effect?>? _initialize = initialize;
    private readonly Func<IMessage, Effect?> _update = update ?? throw new ArgumentNullException(nameof(update));
    private readonly Func<ScreenOutput> _render = render ?? throw new ArgumentNullException(nameof(render));
    private readonly TesseraRuntimeLoopOptions _options = options ?? new TesseraRuntimeLoopOptions();
    private readonly Channel<IMessage> _messages = Channel.CreateUnbounded<IMessage>(MessageChannelOptions);
    private readonly Channel<Effect> _effects = Channel.CreateUnbounded<Effect>(EffectChannelOptions);
    private readonly object _stateLock = new();
    private readonly TesseraCapabilityProbe _capabilityProbe = new();
    private readonly TesseraRuntimeState _runtime = new();
    private CancellationTokenSource? _cts;
    private bool _running;

    public void Send(IMessage message)
    {
        if (message is not null)
        {
            _messages.Writer.TryWrite(message);
        }
    }

    public Task RunAsync(CancellationToken cancellationToken = default) => RunLoopAsync(cancellationToken);

    public Task StopAsync(bool kill = false, CancellationToken cancellationToken = default) => StopLoopAsync(kill, cancellationToken);

    private async Task RunLoopAsync(CancellationToken cancellationToken)
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
            _runtime.EffectScheduler = new TesseraEffectScheduler(_options, Send);

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
                (resizeLoop, resizeSignalRegistration) = TesseraResizeMonitor.Start(_runtime.Terminal, _options, size, Send, token);
            }

            var commandLoop = Task.Run(() => _runtime.EffectScheduler!.RunLoopAsync(_effects.Reader, token), token);
            var inputLoop = StartInputLoop(token);
            await _capabilityProbe.StartAsync(_runtime.Terminal, _options, _runtime.Capabilities, Send, token).ConfigureAwait(false);

            if (_initialize?.Invoke() is { } initEffect)
            {
                await _effects.Writer.WriteAsync(initEffect, token).ConfigureAwait(false);
            }

            await RenderAsync(_render(), token).ConfigureAwait(false);
            await ProcessMessageLoopAsync(commandLoop, inputLoop, resizeLoop, token).ConfigureAwait(false);
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
            _runtime.EffectScheduler?.Dispose();
            _runtime.EffectScheduler = null;
        }
    }

    private async Task StopLoopAsync(bool kill, CancellationToken cancellationToken)
    {
        if (_cts is null)
        {
            return;
        }

        await _cts.CancelAsync().ConfigureAwait(false);
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
                if (message is TesseraCapabilityProbe.CapabilityProbeTimeoutMsg probeTimeout)
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

                var filtered = _options.MessageFilter is null ? message : _options.MessageFilter(message);
                if (filtered is null)
                {
                    continue;
                }

                if (filtered is QuitMsg)
                {
                    if (_cts is { } quitCts)
                    {
                        await quitCts.CancelAsync().ConfigureAwait(false);
                    }

                    await TesseraBackgroundLoops.AwaitAsync(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
                    await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
                    return;
                }

                if (filtered is InterruptMsg)
                {
                    var unhandledCommandException = _runtime.EffectScheduler?.ConsumeUnhandledException();
                    if (unhandledCommandException is not null)
                    {
                        unhandledCommandException.Throw();
                    }

                    throw new TesseraRuntimeInterruptedException();
                }

                if (await TryHandleCommandEnvelopeAsync(filtered, token).ConfigureAwait(false))
                {
                    continue;
                }

                if (filtered is ModeReportMsg modeReport
                    && TesseraCapabilityProbe.TryApplyModeReport(_runtime.Capabilities, modeReport, out var refinedCapabilities))
                {
                    _runtime.Capabilities = refinedCapabilities;
                    _runtime.Renderer?.UpdateCapabilities(refinedCapabilities);
                    Send(new TerminalCapabilitiesMsg(refinedCapabilities));
                }

                if (await TryHandleMessageSideEffectsAsync(filtered, token).ConfigureAwait(false))
                {
                    continue;
                }

                var effect = _update(filtered);

                if (effect is not null)
                {
                    await _effects.Writer.WriteAsync(effect, token).ConfigureAwait(false);
                }

                pendingRender = true;
                var renderAttempt = await TesseraFramePacer.TryRenderAsync(
                    _options.AdaptiveFramePacing,
                    minFrame,
                    lastRender,
                    pendingRender,
                    () => RenderAsync(_render(), token),
                    token).ConfigureAwait(false);
                lastRender = renderAttempt.LastRender;
                pendingRender = renderAttempt.PendingRender;
                if (renderAttempt.Rendered)
                {
                    continue;
                }
            }

            if (_options.AdaptiveFramePacing && pendingRender)
            {
                var delayedRender = await TesseraFramePacer.DelayAndRenderAsync(
                    minFrame,
                    lastRender,
                    () => RenderAsync(_render(), token),
                    token).ConfigureAwait(false);
                lastRender = delayedRender.LastRender;
                pendingRender = false;
            }
        }

        if (_cts is { } completionCts)
        {
            await completionCts.CancelAsync().ConfigureAwait(false);
        }

        await TesseraBackgroundLoops.AwaitAsync(commandLoop, inputLoop, resizeLoop).ConfigureAwait(false);
        await ShutdownAsync(kill: false, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task<bool> TryHandleCommandEnvelopeAsync(IMessage filtered, CancellationToken token)
    {
        if (filtered is SequenceMsg sequence)
        {
            _ = Task.Run(() => _runtime.EffectScheduler!.RunSequenceAsync(sequence.Effects, token), token);
            return true;
        }

        if (filtered is BatchMsg batch)
        {
            foreach (var effect in batch.Effects)
            {
                await _effects.Writer.WriteAsync(effect, token).ConfigureAwait(false);
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
            && TesseraCapabilityProbe.TryRefineColorProfile(_runtime.ColorProfile, capability, out var refinedColorProfile))
        {
            _runtime.ColorProfile = refinedColorProfile;
            Send(new ColorProfileMsg(refinedColorProfile));
        }

        if (filtered is MouseMsg mouse && _runtime.LastRenderedOutput.Input.OnMouse is { } onMouse)
        {
            var callbackEffect = onMouse(mouse);
            if (callbackEffect is not null)
            {
                await _effects.Writer.WriteAsync(callbackEffect, token).ConfigureAwait(false);
            }
        }

        return false;
    }

    private Task StartInputLoop(CancellationToken token)
    {
        if (_options.DisableInput || _runtime.Terminal is null || !_runtime.Terminal.IsInputInteractive)
        {
            return Task.CompletedTask;
        }

        if (_runtime.Terminal is ConsoleTerminalAdapter consoleTerminal
            && ShouldUseConsoleKeyEventLoop(_options.UseConsoleKeyEvents, consoleTerminal.IsRawModeActive, _runtime.Capabilities))
        {
            return Task.Run(() => ConsoleTerminalAdapter.StreamConsoleKeyEventsAsync(Send, token), token);
        }

        _runtime.Reader = new TerminalReader(_runtime.Terminal.Input, _options.EventDecoder ?? new EventDecoder(), _options.EscapeTimeout);
        return Task.Run(() => _runtime.Reader.StreamEventsAsync(Send, token), token);
    }

    internal static bool ShouldUseConsoleKeyEventLoop(
        bool useConsoleKeyEvents,
        bool isRawModeActive,
        TerminalCapabilityProfile runtimeCapabilities)
    {
        ArgumentNullException.ThrowIfNull(runtimeCapabilities);

        // Raw mode should prefer terminal byte-stream decoding so mouse/focus/paste CSI
        // sequences remain available. Console.ReadKey fallback is for non-raw consoles.
        if (!useConsoleKeyEvents || isRawModeActive)
        {
            return false;
        }

        return !RequiresCsiInputDecoding(runtimeCapabilities);
    }

    private static bool RequiresCsiInputDecoding(TerminalCapabilityProfile runtimeCapabilities)
    {
        return runtimeCapabilities.MouseReporting
            || runtimeCapabilities.FocusReporting
            || runtimeCapabilities.BracketedPaste
            || runtimeCapabilities.ModeReports;
    }

    private async Task RenderAsync(ScreenOutput output, CancellationToken token)
    {
        if (_runtime.Renderer is null)
        {
            return;
        }

        _runtime.LastRenderedOutput = output;
        _runtime.Renderer.Render(output);
        await _runtime.Renderer.FlushAsync(token).ConfigureAwait(false);
    }

    private async Task ShutdownAsync(bool kill, CancellationToken token)
    {
        _effects.Writer.TryComplete();
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
        _runtime.LastRenderedOutput = ScreenOutput.From(string.Empty);
    }
}
