using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Describes the terminal capabilities available to advanced TeaSharp hosting seams.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed record TerminalCapabilityProfile(
    bool FocusReporting = true,
    bool MouseReporting = true,
    bool BracketedPaste = true,
    bool SynchronizedUpdates = true,
    bool ModeReports = true,
    string Source = "assumed-supported")
{
    public static TerminalCapabilityProfile AllSupported { get; } = new();
}
