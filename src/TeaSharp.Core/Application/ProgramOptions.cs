using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ProgramOptions
{
    public Func<IMessage, IMessage?>? MessageFilter { get; init; }

    public int MaxFps { get; init; } = 60;

    public bool AdaptiveFramePacing { get; init; } = true;

    public bool DisableRenderer { get; init; }

    public bool DisableInput { get; init; }

    public bool UseConsoleKeyEvents { get; init; } = true;

    public bool CatchEffectExceptions { get; init; } = true;

    public Func<Exception, IMessage?>? MapEffectException { get; init; }

    public TimeSpan EscapeTimeout { get; init; } = TimeSpan.FromMilliseconds(50);

    public bool EnableResizeSignals { get; init; } = true;

    public TimeSpan ResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(120);

    public TimeSpan MinResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(16);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool EnableCapabilityProbe { get; init; } = true;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TimeSpan CapabilityProbeTimeout { get; init; } = TimeSpan.FromMilliseconds(260);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int MaxConcurrentEffects { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IProgramRenderer? Renderer { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public AnsiRendererOptions? AnsiRendererOptions { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ITerminalAdapter? Terminal { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TerminalCapabilityProfile? TerminalCapabilities { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TerminalCapabilityProfile>? TerminalCapabilityDetector { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TerminalColorProfile? ColorProfile { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TerminalColorProfile>? ColorProfileDetector { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IEventDecoder? EventDecoder { get; init; }

    internal Func<Action, IDisposable?>? ResizeSignalRegistrationFactory { get; init; }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IReadOnlyList<int>? CapabilityProbeModes { get; init; }
}
