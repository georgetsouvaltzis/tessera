using System.ComponentModel;
using TeaSharp.Internal;

namespace TeaSharp.Hosting;

/// <summary>
/// Detects terminal color support for advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TerminalColorProfileDetector
{
    public static TerminalColorProfile Detect() =>
        global::TeaSharp.Core.Terminal.TerminalColorProfileDetector.Detect().AsHosting();
}
