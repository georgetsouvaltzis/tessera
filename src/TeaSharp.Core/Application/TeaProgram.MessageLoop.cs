using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Application;

public sealed partial class TeaProgram
{
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
                var renderAttempt = await TeaProgramFramePacer.TryRenderAsync(
                    _options.AdaptiveFramePacing,
                    minFrame,
                    lastRender,
                    pendingRender,
                    () => RenderAsync(Model.View(), token),
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
                var delayedRender = await TeaProgramFramePacer.DelayAndRenderAsync(
                    minFrame,
                    lastRender,
                    () => RenderAsync(Model.View(), token),
                    token).ConfigureAwait(false);
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
}
