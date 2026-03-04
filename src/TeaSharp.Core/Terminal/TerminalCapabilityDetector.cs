namespace TeaSharp.Core.Terminal;

public static class TerminalCapabilityDetector
{
    public static TerminalCapabilityProfile Detect()
    {
        return Detect(TryGetEnvironmentVariable);
    }

    internal static TerminalCapabilityProfile Detect(Func<string, string?> readEnv)
    {
        var term = (readEnv("TERM") ?? string.Empty).Trim();
        var termLower = term.ToLowerInvariant();
        var termProgram = (readEnv("TERM_PROGRAM") ?? string.Empty).Trim();
        var termProgramLower = termProgram.ToLowerInvariant();
        var wtSession = readEnv("WT_SESSION");

        if (string.Equals(termLower, "dumb", StringComparison.Ordinal))
        {
            return new TerminalCapabilityProfile(
                FocusReporting: false,
                MouseReporting: false,
                BracketedPaste: false,
                SynchronizedUpdates: false,
                ModeReports: false,
                Source: "env:TERM=dumb");
        }

        if (termLower.StartsWith("linux", StringComparison.Ordinal)
            || termLower.StartsWith("vt100", StringComparison.Ordinal)
            || termLower.StartsWith("ansi", StringComparison.Ordinal))
        {
            return new TerminalCapabilityProfile(
                FocusReporting: false,
                MouseReporting: false,
                BracketedPaste: false,
                SynchronizedUpdates: false,
                ModeReports: false,
                Source: $"env:TERM={termLower}");
        }

        if (termProgramLower == "apple_terminal")
        {
            return new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: false,
                ModeReports: true,
                Source: "env:TERM_PROGRAM=Apple_Terminal");
        }

        if (!string.IsNullOrWhiteSpace(wtSession))
        {
            return new TerminalCapabilityProfile(Source: "env:WT_SESSION");
        }

        if (termLower.Contains("xterm", StringComparison.Ordinal)
            || termLower.Contains("screen", StringComparison.Ordinal)
            || termLower.Contains("tmux", StringComparison.Ordinal)
            || termLower.Contains("ghostty", StringComparison.Ordinal)
            || termLower.Contains("wezterm", StringComparison.Ordinal)
            || termLower.Contains("alacritty", StringComparison.Ordinal)
            || termLower.Contains("kitty", StringComparison.Ordinal)
            || termLower.Contains("rxvt", StringComparison.Ordinal))
        {
            return new TerminalCapabilityProfile(Source: $"env:TERM={termLower}");
        }

        return TerminalCapabilityProfile.AllSupported with { Source = "assumed-supported" };
    }

    private static string? TryGetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name);
    }
}
