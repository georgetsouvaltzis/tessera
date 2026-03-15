using System.ComponentModel;
using TeaSharp.Internal;

namespace TeaSharp.Hosting;

/// <summary>
/// Detects terminal capabilities for advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TerminalCapabilityDetector
{
    public static TerminalCapabilityProfile Detect() =>
        global::TeaSharp.Core.Terminal.TerminalCapabilityDetector.Detect().AsHosting();
}
