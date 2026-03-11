using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

public sealed partial class TeaProgram
{
    private async Task<IScreen> RunProgramAsync(CancellationToken cancellationToken)
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
            _runtime.EffectScheduler = new TeaProgramEffectScheduler(_options, Send);

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

            var commandLoop = Task.Run(() => _runtime.EffectScheduler!.RunLoopAsync(_effects.Reader, token), token);
            var inputLoop = StartInputLoop(token);
            await _capabilityProbe.StartAsync(_runtime.Terminal, _options, _runtime.Capabilities, Send, token).ConfigureAwait(false);

            if (Screen.Init() is { } initEffect)
            {
                await _effects.Writer.WriteAsync(initEffect, token).ConfigureAwait(false);
            }

            await RenderAsync(Screen.Render(), token).ConfigureAwait(false);
            await ProcessMessageLoopAsync(commandLoop, inputLoop, resizeLoop, token).ConfigureAwait(false);
            return Screen;
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

    private async Task StopProgramAsync(bool kill, CancellationToken cancellationToken)
    {
        if (_cts is null)
        {
            return;
        }

        _cts.Cancel();
        await ShutdownAsync(kill, cancellationToken).ConfigureAwait(false);
    }
}
