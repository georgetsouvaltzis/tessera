using System.ComponentModel;

namespace Tessera.Core.Terminal;

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
    public static TerminalCapabilityProfile AllSupported { get; } = new();
}
