using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Detects terminal capabilities for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TerminalCapabilityDetector
{
    public static TerminalCapabilityProfile Detect() =>
        global::Tessera.Core.Terminal.TerminalCapabilityDetector.Detect().AsHosting();
}
