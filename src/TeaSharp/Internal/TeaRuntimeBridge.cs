using TeaSharp.Core.Application;

namespace TeaSharp.Internal;

internal interface ITeaRuntime
{
    void Send(Message message);

    Task RunAsync(CancellationToken cancellationToken);

    Task StopAsync(bool kill, CancellationToken cancellationToken);
}

internal static class TeaRuntimeFactory
{
    public static ITeaRuntime Create(TeaApp app, TeaRuntimeOptions options, TeaSharp.Hosting.TeaHostingOptions? hosting)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(options);
        return new TeaAppRuntime(app, options, hosting);
    }
}

internal sealed class TeaAppRuntime : ITeaRuntime
{
    private readonly TeaRuntimeLoop _runtime;

    public TeaAppRuntime(TeaApp app, TeaRuntimeOptions options, TeaSharp.Hosting.TeaHostingOptions? hosting)
    {
        app.ConfigureRuntimeOptions(options);
        _runtime = new TeaRuntimeLoop(
            () => TeaEffectAdapter.ToCore(app.InitializeRuntime()),
            message => TeaEffectAdapter.ToCore(app.UpdateRuntime(TeaMessageAdapter.ToPublic(message))),
            () => app.RenderRuntime().Output,
            CreateRuntimeLoopOptions(app, options, hosting));
    }

    public void Send(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _runtime.Send(TeaMessageAdapter.ToCore(message));
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        return _runtime.RunAsync(cancellationToken);
    }

    public Task StopAsync(bool kill, CancellationToken cancellationToken)
    {
        return _runtime.StopAsync(kill, cancellationToken);
    }

    private static TeaRuntimeLoopOptions CreateRuntimeLoopOptions(TeaApp app, TeaRuntimeOptions options, TeaSharp.Hosting.TeaHostingOptions? hosting)
    {
        return new TeaRuntimeLoopOptions
        {
            MessageFilter = hosting?.MessageFilter is null
                ? null
                : message =>
                {
                    var filtered = hosting.MessageFilter(app, TeaMessageAdapter.ToPublic(message));
                    return filtered is null ? null : TeaMessageAdapter.ToCore(filtered);
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
                    return mapped is null ? null : TeaMessageAdapter.ToCore(mapped);
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
            CapabilityProbeModes = hosting?.CapabilityProbeModes,
        };
    }

    private sealed class HostingTerminalAdapter(TeaSharp.Hosting.ITerminalAdapter inner)
        : global::TeaSharp.Core.Terminal.ITerminalAdapter
    {
        public Stream Input => inner.Input;

        public Stream Output => inner.Output;

        public bool IsInputInteractive => inner.IsInputInteractive;

        public bool IsOutputInteractive => inner.IsOutputInteractive;

        public ValueTask PrepareAsync(CancellationToken cancellationToken) => inner.PrepareAsync(cancellationToken);

        public ValueTask RestoreAsync(CancellationToken cancellationToken) => inner.RestoreAsync(cancellationToken);

        public async ValueTask<global::TeaSharp.Core.Terminal.TerminalSize> GetSizeAsync(CancellationToken cancellationToken) =>
            (await inner.GetSizeAsync(cancellationToken).ConfigureAwait(false)).ToCore();

        public ValueTask DisposeAsync() => inner.DisposeAsync();
    }

    private sealed class HostingRendererAdapter(TeaSharp.Hosting.IProgramRenderer inner)
        : global::TeaSharp.Core.Rendering.IProgramRenderer
    {
        public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken) =>
            inner.InitializeAsync(output, cancellationToken);

        public void Resize(int width, int height) => inner.Resize(width, height);

        public void UpdateCapabilities(global::TeaSharp.Core.Terminal.TerminalCapabilityProfile capabilities) =>
            inner.UpdateCapabilities(capabilities.AsHosting());

        public void Render(global::TeaSharp.Core.Abstractions.ScreenOutput output) =>
            inner.Render(output.ToHosting());

        public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken) =>
            inner.WriteRawAsync(content, cancellationToken);

        public ValueTask FlushAsync(CancellationToken cancellationToken) =>
            inner.FlushAsync(cancellationToken);

        public ValueTask ResetAsync(CancellationToken cancellationToken) =>
            inner.ResetAsync(cancellationToken);

        public ValueTask DisposeAsync() => inner.DisposeAsync();
    }

    private sealed class HostingEventDecoderAdapter(TeaSharp.Hosting.IEventDecoder inner)
        : global::TeaSharp.Core.Input.IEventDecoder
    {
        public global::TeaSharp.Core.Input.DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired) =>
            inner.Decode(buffer, timeoutExpired).ToCore();
    }
}
