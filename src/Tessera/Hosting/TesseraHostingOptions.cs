using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Configures advanced renderer, terminal, decoder, and capability seams for custom hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class TesseraHostingOptions
{
    /// <summary>
    ///     Gets or sets the message filter.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TesseraApp, Message, Message?>? MessageFilter { get; set; }

    /// <summary>
    ///     Gets or sets whether enable capability probe.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool EnableCapabilityProbe { get; set; } = true;

    /// <summary>
    ///     Gets or sets the capability probe timeout.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TimeSpan CapabilityProbeTimeout { get; set; } = TimeSpan.FromMilliseconds(260);

    /// <summary>
    ///     Gets or sets the max concurrent effects.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int MaxConcurrentEffects { get; set; }

    /// <summary>
    ///     Gets or sets the renderer.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IProgramRenderer? Renderer { get; set; }

    /// <summary>
    ///     Gets or sets the ansi renderer options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public AnsiRendererOptions? AnsiRendererOptions { get; set; }

    /// <summary>
    ///     Gets or sets the terminal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public ITerminalAdapter? Terminal { get; set; }

    /// <summary>
    ///     Gets or sets the terminal capabilities.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TerminalCapabilityProfile? TerminalCapabilities { get; set; }

    /// <summary>
    ///     Gets or sets the terminal capability detector.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TerminalCapabilityProfile>? TerminalCapabilityDetector { get; set; }

    /// <summary>
    ///     Gets or sets the color profile.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TerminalColorProfile? ColorProfile { get; set; }

    /// <summary>
    ///     Gets or sets the color profile detector.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<TerminalColorProfile>? ColorProfileDetector { get; set; }

    /// <summary>
    ///     Gets or sets the event decoder.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IEventDecoder? EventDecoder { get; set; }

    /// <summary>
    ///     Gets or sets the capability probe modes.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IReadOnlyList<int>? CapabilityProbeModes { get; set; }

    /// <summary>
    ///     Gets or sets the map effect exception.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Func<Exception, Message?>? MapEffectException { get; set; }
}
