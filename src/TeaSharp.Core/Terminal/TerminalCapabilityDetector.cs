using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TeaSharp.Core.Terminal;

public static class TerminalCapabilityDetector
{
    public static TerminalCapabilityProfile Detect()
    {
        return Detect(
            TryGetEnvironmentVariable,
            TryReadTerminfo,
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
    }

    internal static TerminalCapabilityProfile Detect(
        Func<string, string?> readEnv,
        Func<string, string?>? readTerminfo,
        bool isWindows)
    {
        var term = (readEnv("TERM") ?? string.Empty).Trim();
        var termLower = term.ToLowerInvariant();
        var termProgram = (readEnv("TERM_PROGRAM") ?? string.Empty).Trim();
        var termProgramLower = termProgram.ToLowerInvariant();
        var wtSession = readEnv("WT_SESSION");
        var profile = DetectFromEnvironment(termLower, termProgramLower, wtSession);

        if (isWindows || readTerminfo is null || string.IsNullOrWhiteSpace(termLower))
        {
            return profile;
        }

        var terminfo = readTerminfo(termLower);
        if (string.IsNullOrWhiteSpace(terminfo))
        {
            return profile;
        }

        return EnrichWithTerminfo(profile, termLower, terminfo);
    }

    private static TerminalCapabilityProfile DetectFromEnvironment(
        string termLower,
        string termProgramLower,
        string? wtSession)
    {
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

    private static TerminalCapabilityProfile EnrichWithTerminfo(
        TerminalCapabilityProfile profile,
        string term,
        string terminfo)
    {
        var capabilities = ParseCapabilityNames(terminfo);
        var hasXt = capabilities.Contains("XT");
        var hasKmous = capabilities.Contains("kmous");
        var hasBd = capabilities.Contains("BD");
        var hasBe = capabilities.Contains("BE");
        var hasSync = capabilities.Contains("Sync");
        var hasXm = capabilities.Contains("XM");

        var next = profile with
        {
            FocusReporting = profile.FocusReporting || hasXt,
            MouseReporting = profile.MouseReporting || hasKmous,
            BracketedPaste = profile.BracketedPaste || (hasBd && hasBe) || hasXt,
            SynchronizedUpdates = profile.SynchronizedUpdates || hasSync,
            ModeReports = profile.ModeReports || hasXt || hasXm,
        };

        var changed = next.FocusReporting != profile.FocusReporting
            || next.MouseReporting != profile.MouseReporting
            || next.BracketedPaste != profile.BracketedPaste
            || next.SynchronizedUpdates != profile.SynchronizedUpdates
            || next.ModeReports != profile.ModeReports;

        if (!changed)
        {
            return profile;
        }

        return next with { Source = $"{profile.Source}+terminfo:{term}" };
    }

    private static HashSet<string> ParseCapabilityNames(string infocmp)
    {
        var capabilities = new HashSet<string>(StringComparer.Ordinal);
        var token = new StringBuilder();
        var escaped = false;

        foreach (var ch in infocmp)
        {
            if (ch == ',' && !escaped)
            {
                AddCapability(capabilities, token.ToString());
                token.Clear();
                continue;
            }

            token.Append(ch);
            if (escaped)
            {
                escaped = false;
                continue;
            }

            escaped = ch == '\\';
        }

        AddCapability(capabilities, token.ToString());
        return capabilities;
    }

    private static void AddCapability(HashSet<string> capabilities, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var trimmed = token.Trim();
        if (trimmed.Length == 0 || trimmed[0] == '#')
        {
            return;
        }

        var separatorIndex = trimmed.IndexOfAny(['=', '#']);
        var name = separatorIndex >= 0
            ? trimmed[..separatorIndex].Trim()
            : trimmed;

        if (name.Length == 0 || name.Contains('|', StringComparison.Ordinal))
        {
            return;
        }

        capabilities.Add(name);
    }

    private static string? TryGetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name);
    }

    private static string? TryReadTerminfo(string term)
    {
        if (OperatingSystem.IsWindows() || string.IsNullOrWhiteSpace(term))
        {
            return null;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "infocmp",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            startInfo.ArgumentList.Add("-x");
            startInfo.ArgumentList.Add(term);

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            if (!process.WaitForExit(200))
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // ignored: probe best-effort.
                }

                return null;
            }

            _ = errorTask.GetAwaiter().GetResult();
            if (process.ExitCode != 0)
            {
                return null;
            }

            return outputTask.GetAwaiter().GetResult();
        }
        catch
        {
            return null;
        }
    }
}
