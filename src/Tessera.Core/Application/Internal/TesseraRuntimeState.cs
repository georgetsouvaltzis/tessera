using Tessera.Core.Abstractions;
using Tessera.Core.Input;
using Tessera.Core.Rendering;
using Tessera.Core.Terminal;

namespace Tessera.Core.Application;

internal sealed class TesseraRuntimeState
{
    public ITerminalAdapter? Terminal { get; set; }

    public IProgramRenderer? Renderer { get; set; }

    public TerminalReader? Reader { get; set; }

    public TesseraEffectScheduler? EffectScheduler { get; set; }

    public TerminalCapabilityProfile Capabilities { get; set; } = TerminalCapabilityProfile.AllSupported;

    public TerminalColorProfile ColorProfile { get; set; } = TerminalColorProfile.Unknown;

    public ScreenOutput LastRenderedOutput { get; set; } = ScreenOutput.From(string.Empty);
}
