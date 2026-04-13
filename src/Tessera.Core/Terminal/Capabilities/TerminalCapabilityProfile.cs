using System.ComponentModel;

namespace Tessera.Core.Terminal.Capabilities;

/// <summary>
///     Describes terminal capabilities discovered or assumed for the current session.
/// </summary>
/// <param name="FocusReporting">Whether focus-in and focus-out reports are available.</param>
/// <param name="MouseReporting">Whether mouse reporting is available.</param>
/// <param name="BracketedPaste">Whether bracketed paste reporting is available.</param>
/// <param name="SynchronizedUpdates">Whether synchronized updates are available.</param>
/// <param name="ModeReports">Whether DEC mode reports are available.</param>
/// <param name="SupportsOsc50FontRequests">Whether OSC 50 font requests are available.</param>
/// <param name="SupportsIterm2ProfileRequests">Whether iTerm2 profile queries are available.</param>
/// <param name="Source">Describes how the capability decision was produced.</param>
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
    ///     Gets a capability profile where all currently modeled capabilities are available.
    /// </summary>
    public static TerminalCapabilityProfile AllSupported { get; } = new();
}
