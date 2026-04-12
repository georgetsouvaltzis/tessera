using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Detects terminal color support for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TerminalColorProfileDetector
{
    /// <summary>
    /// Executes detect.
    /// </summary>
    /// <returns>The result of detect.</returns>
    public static TerminalColorProfile Detect() =>
        global::Tessera.Core.Terminal.Capabilities.TerminalColorProfileDetector.Detect().AsHosting();
}
