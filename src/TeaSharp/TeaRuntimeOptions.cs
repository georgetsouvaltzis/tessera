using System.ComponentModel;
using TeaSharp.Core.Application;
using TeaSharp.Core.Input;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;
using TeaSharp.Internal;

namespace TeaSharp;

public sealed class TeaRuntimeOptions
{
    public Func<TeaApp, Message, Message?>? MessageFilter { get; set; }

    public int MaxFps { get; set; } = 60;

    public bool AdaptiveFramePacing { get; set; } = true;

    public bool DisableRenderer { get; set; }

    public bool DisableInput { get; set; }

    public bool UseConsoleKeyEvents { get; set; } = true;

    public bool CatchEffectExceptions { get; set; } = true;

    public Func<Exception, Message?>? MapEffectException { get; set; }

    public TimeSpan EscapeTimeout { get; set; } = TimeSpan.FromMilliseconds(50);

    public bool EnableResizeSignals { get; set; } = true;

    public TimeSpan ResizePollInterval { get; set; } = TimeSpan.FromMilliseconds(120);

    public TimeSpan MinResizePollInterval { get; set; } = TimeSpan.FromMilliseconds(16);

    public ScreenOptions Screen { get; set; } = ScreenOptions.Empty;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool EnableCapabilityProbe { get; set; } = true;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TimeSpan CapabilityProbeTimeout { get; set; } = TimeSpan.FromMilliseconds(260);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int MaxConcurrentEffects { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IProgramRenderer? Renderer { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public AnsiRendererOptions? AnsiRendererOptions { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ITerminalAdapter? Terminal { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TerminalCapabilityProfile? TerminalCapabilities { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TerminalCapabilityProfile>? TerminalCapabilityDetector { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TerminalColorProfile? ColorProfile { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TerminalColorProfile>? ColorProfileDetector { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IEventDecoder? EventDecoder { get; set; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IReadOnlyList<int>? CapabilityProbeModes { get; set; }

    internal ProgramOptions ToProgramOptions(TeaApp app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return new ProgramOptions
        {
            MessageFilter = MessageFilter is null
                ? null
                : (_, message) =>
                {
                    var filtered = MessageFilter(app, TeaMessageAdapter.ToPublic(message));
                    return filtered is null ? null : TeaMessageAdapter.ToCore(filtered);
                },
            MaxFps = MaxFps,
            AdaptiveFramePacing = AdaptiveFramePacing,
            DisableRenderer = DisableRenderer,
            DisableInput = DisableInput,
            UseConsoleKeyEvents = UseConsoleKeyEvents,
            CatchEffectExceptions = CatchEffectExceptions,
            MapEffectException = MapEffectException is null
                ? null
                : exception =>
                {
                    var mapped = MapEffectException(exception);
                    return mapped is null ? null : TeaMessageAdapter.ToCore(mapped);
                },
            EscapeTimeout = EscapeTimeout,
            EnableResizeSignals = EnableResizeSignals,
            ResizePollInterval = ResizePollInterval,
            MinResizePollInterval = MinResizePollInterval,
            EnableCapabilityProbe = EnableCapabilityProbe,
            CapabilityProbeTimeout = CapabilityProbeTimeout,
            MaxConcurrentEffects = MaxConcurrentEffects,
            Renderer = Renderer,
            AnsiRendererOptions = AnsiRendererOptions,
            Terminal = Terminal,
            TerminalCapabilities = TerminalCapabilities,
            TerminalCapabilityDetector = TerminalCapabilityDetector,
            ColorProfile = ColorProfile,
            ColorProfileDetector = ColorProfileDetector,
            EventDecoder = EventDecoder,
            CapabilityProbeModes = CapabilityProbeModes,
        };
    }
}
