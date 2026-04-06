using System.ComponentModel;

namespace Tessera.Core.Terminal;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class TerminalColorProfileDetector
{
    public static TerminalColorProfile Detect()
    {
        var colorTerm = (Environment.GetEnvironmentVariable("COLORTERM") ?? string.Empty)
            .Trim()
            .ToLowerInvariant();
        if (colorTerm.Contains("truecolor", StringComparison.Ordinal)
            || colorTerm.Contains("24bit", StringComparison.Ordinal))
        {
            return TerminalColorProfile.TrueColor;
        }

        var term = (Environment.GetEnvironmentVariable("TERM") ?? string.Empty)
            .Trim()
            .ToLowerInvariant();
        if (term.Contains("256color", StringComparison.Ordinal))
        {
            return TerminalColorProfile.Ansi256;
        }

        if (string.IsNullOrWhiteSpace(term) || string.Equals(term, "dumb", StringComparison.Ordinal))
        {
            return TerminalColorProfile.Unknown;
        }

        return TerminalColorProfile.Ansi16;
    }
}
