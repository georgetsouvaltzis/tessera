using System.ComponentModel;
using TeaSharp.Core.Input;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Hosting;

/// <summary>
/// Configures advanced renderer, terminal, decoder, and capability seams for custom hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class TeaHostingOptions
{
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
}
