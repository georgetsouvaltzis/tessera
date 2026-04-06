using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Configures advanced renderer, terminal, decoder, and capability seams for custom hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class TesseraHostingOptions
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TesseraApp, Message, Message?>? MessageFilter { get; set; }

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

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<Exception, Message?>? MapEffectException { get; set; }
}
