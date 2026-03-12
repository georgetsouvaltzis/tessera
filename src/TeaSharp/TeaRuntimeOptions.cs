using System.ComponentModel;
using TeaSharp.Core.Application;
using TeaSharp.Internal;

namespace TeaSharp;

public sealed class TeaRuntimeOptions
{
    public int MaxFps { get; set; } = 60;

    public bool AdaptiveFramePacing { get; set; } = true;

    public bool DisableRenderer { get; set; }

    public bool DisableInput { get; set; }

    public bool UseConsoleKeyEvents { get; set; } = true;

    public bool CatchEffectExceptions { get; set; } = true;

    public TimeSpan EscapeTimeout { get; set; } = TimeSpan.FromMilliseconds(50);

    public bool EnableResizeSignals { get; set; } = true;

    public TimeSpan ResizePollInterval { get; set; } = TimeSpan.FromMilliseconds(120);

    public TimeSpan MinResizePollInterval { get; set; } = TimeSpan.FromMilliseconds(16);

    public ScreenOptions Screen { get; set; } = ScreenOptions.Empty;

    internal ProgramOptions ToProgramOptions(TeaApp app, TeaSharp.Hosting.TeaHostingOptions? hosting = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        return new ProgramOptions
        {
            MessageFilter = hosting?.MessageFilter is null
                ? null
                : (_, message) =>
                {
                    var filtered = hosting.MessageFilter(app, TeaMessageAdapter.ToPublic(message));
                    return filtered is null ? null : TeaMessageAdapter.ToCore(filtered);
                },
            MaxFps = MaxFps,
            AdaptiveFramePacing = AdaptiveFramePacing,
            DisableRenderer = DisableRenderer,
            DisableInput = DisableInput,
            UseConsoleKeyEvents = UseConsoleKeyEvents,
            CatchEffectExceptions = CatchEffectExceptions,
            MapEffectException = hosting?.MapEffectException is null
                ? null
                : exception =>
                {
                    var mapped = hosting.MapEffectException(exception);
                    return mapped is null ? null : TeaMessageAdapter.ToCore(mapped);
                },
            EscapeTimeout = EscapeTimeout,
            EnableResizeSignals = EnableResizeSignals,
            ResizePollInterval = ResizePollInterval,
            MinResizePollInterval = MinResizePollInterval,
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
