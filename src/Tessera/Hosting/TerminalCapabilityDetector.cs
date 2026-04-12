using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Detects terminal capabilities for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TerminalCapabilityDetector
{
    /// <summary>
    /// Executes detect.
    /// </summary>
    /// <returns>The result of detect.</returns>
    public static TerminalCapabilityProfile Detect() =>
        global::Tessera.Core.Terminal.Capabilities.TerminalCapabilityDetector.Detect().AsHosting();
}
