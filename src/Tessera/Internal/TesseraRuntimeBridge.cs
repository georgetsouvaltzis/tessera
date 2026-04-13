using Tessera.Core.Abstractions;
using Tessera.Core.Application;
using Tessera.Core.Application.Internal;
using Tessera.Core.Input.Decoding;
using Tessera.Hosting;
using IEventDecoder = Tessera.Hosting.IEventDecoder;
using TerminalCapabilityProfile = Tessera.Core.Terminal.Capabilities.TerminalCapabilityProfile;
using TerminalSize = Tessera.Core.Terminal.TerminalSize;

namespace Tessera.Internal;

internal interface ITeaRuntime
{
    void Send(Message message);

    Task RunAsync(CancellationToken cancellationToken);

    Task StopAsync(bool kill, CancellationToken cancellationToken);
}

internal static class TesseraRuntimeFactory
{
    public static ITeaRuntime Create(TesseraApp app, TesseraRuntimeOptions options, TesseraHostingOptions? hosting)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(options);
        return new TesseraAppRuntime(app, options, hosting);
    }
}

internal sealed class TesseraAppRuntime : ITeaRuntime
{
    private readonly TesseraRuntimeLoop _runtime;

    public TesseraAppRuntime(TesseraApp app, TesseraRuntimeOptions options, TesseraHostingOptions? hosting)
    {
        app.ConfigureRuntimeOptions(options);
        _runtime = new TesseraRuntimeLoop(
            () => TesseraEffectAdapter.ToCore(app.InitializeRuntime()),
            message => TesseraEffectAdapter.ToCore(app.UpdateRuntime(TesseraMessageAdapter.ToPublic(message))),
            () => app.RenderRuntime().Output,
            CreateRuntimeLoopOptions(app, options, hosting));
    }

    public void Send(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _runtime.Send(TesseraMessageAdapter.ToCore(message));
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        return _runtime.RunAsync(cancellationToken);
    }

    public Task StopAsync(bool kill, CancellationToken cancellationToken)
    {
        return _runtime.StopAsync(kill, cancellationToken);
    }

    private static TesseraRuntimeLoopOptions CreateRuntimeLoopOptions(TesseraApp app, TesseraRuntimeOptions options,
        TesseraHostingOptions? hosting)
    {
        return new TesseraRuntimeLoopOptions
        {
            MessageFilter = hosting?.MessageFilter is null
                ? null
                : message =>
                {
                    var filtered = hosting.MessageFilter(app, TesseraMessageAdapter.ToPublic(message));
                    return filtered is null ? null : TesseraMessageAdapter.ToCore(filtered);
                },
            MaxFps = options.MaxFps,
            AdaptiveFramePacing = options.AdaptiveFramePacing,
            DisableRenderer = options.DisableRenderer,
            DisableInput = options.DisableInput,
            UseConsoleKeyEvents = options.UseConsoleKeyEvents,
            CatchEffectExceptions = options.CatchEffectExceptions,
            MapEffectException = hosting?.MapEffectException is null
                ? null
                : exception =>
                {
                    var mapped = hosting.MapEffectException(exception);
                    return mapped is null ? null : TesseraMessageAdapter.ToCore(mapped);
                },
            EscapeTimeout = options.EscapeTimeout,
            EnableResizeSignals = options.EnableResizeSignals,
            ResizePollInterval = options.ResizePollInterval,
            MinResizePollInterval = options.MinResizePollInterval,
            EnableCapabilityProbe = hosting?.EnableCapabilityProbe ?? true,
            CapabilityProbeTimeout = hosting?.CapabilityProbeTimeout ?? TimeSpan.FromMilliseconds(260),
            MaxConcurrentEffects = hosting?.MaxConcurrentEffects ?? 0,
            Renderer = hosting?.Renderer is null ? null : new HostingRendererAdapter(hosting.Renderer),
            AnsiRendererOptions = hosting?.AnsiRendererOptions?.ToCore(),
            Terminal = hosting?.Terminal is null ? null : new HostingTerminalAdapter(hosting.Terminal),
            TerminalCapabilities = hosting?.TerminalCapabilities?.ToCore(),
            TerminalCapabilityDetector = hosting?.TerminalCapabilityDetector is null
                ? null
                : () => hosting.TerminalCapabilityDetector().ToCore(),
            ColorProfile = hosting?.ColorProfile?.ToCore(),
            ColorProfileDetector = hosting?.ColorProfileDetector is null
                ? null
                : () => hosting.ColorProfileDetector().ToCore(),
            EventDecoder = hosting?.EventDecoder is null ? null : new HostingEventDecoderAdapter(hosting.EventDecoder),
            CapabilityProbeModes = hosting?.CapabilityProbeModes
        };
    }

    private sealed class HostingTerminalAdapter(ITerminalAdapter inner)
        : Core.Terminal.ITerminalAdapter
    {
        public Stream Input => inner.Input;

        public Stream Output => inner.Output;

        public bool IsInputInteractive => inner.IsInputInteractive;

        public bool IsOutputInteractive => inner.IsOutputInteractive;

        public ValueTask PrepareAsync(CancellationToken cancellationToken)
        {
            return inner.PrepareAsync(cancellationToken);
        }

        public ValueTask RestoreAsync(CancellationToken cancellationToken)
        {
            return inner.RestoreAsync(cancellationToken);
        }

        public async ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
        {
            return (await inner.GetSizeAsync(cancellationToken).ConfigureAwait(false)).ToCore();
        }

        public ValueTask DisposeAsync()
        {
            return inner.DisposeAsync();
        }
    }

    private sealed class HostingRendererAdapter(IProgramRenderer inner)
        : Core.Rendering.IProgramRenderer
    {
        public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
        {
            return inner.InitializeAsync(output, cancellationToken);
        }

        public void Resize(int width, int height)
        {
            inner.Resize(width, height);
        }

        public void UpdateCapabilities(TerminalCapabilityProfile capabilities)
        {
            inner.UpdateCapabilities(capabilities.AsHosting());
        }

        public void Render(ScreenOutput output)
        {
            inner.Render(output.ToHosting());
        }

        public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken)
        {
            return inner.WriteRawAsync(content, cancellationToken);
        }

        public ValueTask FlushAsync(CancellationToken cancellationToken)
        {
            return inner.FlushAsync(cancellationToken);
        }

        public ValueTask ResetAsync(CancellationToken cancellationToken)
        {
            return inner.ResetAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return inner.DisposeAsync();
        }
    }

    private sealed class HostingEventDecoderAdapter(IEventDecoder inner)
        : Core.Input.Decoding.IEventDecoder
    {
        public DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired)
        {
            return inner.Decode(buffer, timeoutExpired).ToCore();
        }
    }
}
