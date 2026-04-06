using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Detects terminal color support for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TerminalColorProfileDetector
{
    public static TerminalColorProfile Detect() =>
        global::Tessera.Core.Terminal.TerminalColorProfileDetector.Detect().AsHosting();
}
