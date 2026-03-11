using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

internal sealed class TeaProgramRuntimeState
{
    public ITerminalAdapter? Terminal { get; set; }

    public IProgramRenderer? Renderer { get; set; }

    public TerminalReader? Reader { get; set; }

    public TeaProgramEffectScheduler? EffectScheduler { get; set; }

    public TerminalCapabilityProfile Capabilities { get; set; } = TerminalCapabilityProfile.AllSupported;

    public TerminalColorProfile ColorProfile { get; set; } = TerminalColorProfile.Unknown;

    public ScreenOutput LastRenderedOutput { get; set; } = ScreenOutput.From(string.Empty);
}
