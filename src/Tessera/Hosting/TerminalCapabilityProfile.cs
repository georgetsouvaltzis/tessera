using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Describes the terminal capabilities available to advanced Tessera hosting seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed record TerminalCapabilityProfile(
    bool FocusReporting = true,
    bool MouseReporting = true,
    bool BracketedPaste = true,
    bool SynchronizedUpdates = true,
    bool ModeReports = true,
    bool SupportsOsc50FontRequests = false,
    bool SupportsIterm2ProfileRequests = false,
    string Source = "assumed-supported")
{
    /// <summary>
    ///     Gets the all supported.
    /// </summary>
    public static TerminalCapabilityProfile AllSupported { get; } = new();
}
