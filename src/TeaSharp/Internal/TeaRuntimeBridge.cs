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
        app.ConfigureRuntimeScreen(options.Screen);
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
            Renderer = hosting?.Renderer,
            AnsiRendererOptions = hosting?.AnsiRendererOptions,
            Terminal = hosting?.Terminal,
            TerminalCapabilities = hosting?.TerminalCapabilities,
            TerminalCapabilityDetector = hosting?.TerminalCapabilityDetector,
            ColorProfile = hosting?.ColorProfile,
            ColorProfileDetector = hosting?.ColorProfileDetector,
            EventDecoder = hosting?.EventDecoder,
            CapabilityProbeModes = hosting?.CapabilityProbeModes,
        };
    }
}
