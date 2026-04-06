using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace Tessera.Core.Terminal;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal static class TerminalCapabilityDetector
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
        var overrideRaw = readEnv("TESSERA_CAPS");
        var profile = DetectFromEnvironment(termLower, termProgramLower, wtSession);

        if (isWindows || readTerminfo is null || string.IsNullOrWhiteSpace(termLower))
        {
            return ApplyOverrides(profile, overrideRaw);
        }

        var terminfo = readTerminfo(termLower);
        if (string.IsNullOrWhiteSpace(terminfo))
        {
            return ApplyOverrides(profile, overrideRaw);
        }

        return ApplyOverrides(EnrichWithTerminfo(profile, termLower, terminfo), overrideRaw);
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
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: false,
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
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: false,
                Source: $"env:TERM={termLower}");
        }

        if (termProgramLower == "iterm.app")
        {
            return new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: true,
                ModeReports: true,
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: true,
                Source: "env:TERM_PROGRAM=iTerm.app");
        }

        if (termProgramLower == "apple_terminal")
        {
            return new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: false,
                ModeReports: true,
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: false,
                Source: "env:TERM_PROGRAM=Apple_Terminal");
        }

        if (termProgramLower == "wezterm" || termProgramLower == "ghostty")
        {
            return new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: true,
                ModeReports: true,
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: false,
                Source: $"env:TERM_PROGRAM={termProgramLower}");
        }

        if (!string.IsNullOrWhiteSpace(wtSession))
        {
            return new TerminalCapabilityProfile(
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: false,
                Source: "env:WT_SESSION");
        }

        if (termLower.Contains("wezterm", StringComparison.Ordinal)
            || termLower.Contains("ghostty", StringComparison.Ordinal)
            || termLower.Contains("kitty", StringComparison.Ordinal))
        {
            return new TerminalCapabilityProfile(
                SupportsOsc50FontRequests: false,
                SupportsIterm2ProfileRequests: false,
                Source: $"env:TERM={termLower}");
        }

        if (termLower.Contains("xterm", StringComparison.Ordinal)
            || termLower.Contains("rxvt", StringComparison.Ordinal))
        {
            return new TerminalCapabilityProfile(
                SupportsOsc50FontRequests: true,
                SupportsIterm2ProfileRequests: false,
                Source: $"env:TERM={termLower}");
        }

        return TerminalCapabilityProfile.AllSupported with
        {
            SupportsOsc50FontRequests = false,
            SupportsIterm2ProfileRequests = false,
            Source = "assumed-supported",
        };
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
            SupportsOsc50FontRequests = profile.SupportsOsc50FontRequests,
            SupportsIterm2ProfileRequests = profile.SupportsIterm2ProfileRequests,
        };

        var changed = next.FocusReporting != profile.FocusReporting
            || next.MouseReporting != profile.MouseReporting
            || next.BracketedPaste != profile.BracketedPaste
            || next.SynchronizedUpdates != profile.SynchronizedUpdates
            || next.ModeReports != profile.ModeReports
            || next.SupportsOsc50FontRequests != profile.SupportsOsc50FontRequests
            || next.SupportsIterm2ProfileRequests != profile.SupportsIterm2ProfileRequests;

        if (!changed)
        {
            return profile;
        }

        return next with { Source = $"{profile.Source}+terminfo:{term}" };
    }

    private static TerminalCapabilityProfile ApplyOverrides(TerminalCapabilityProfile profile, string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return profile;
        }

        var next = profile;
        foreach (var token in raw.Split([',', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = token.IndexOf('=', StringComparison.Ordinal);
            if (separator <= 0 || separator >= token.Length - 1)
            {
                continue;
            }

            var key = token[..separator].Trim().ToLowerInvariant();
            var value = token[(separator + 1)..].Trim().ToLowerInvariant();
            var enabled = value is "1" or "true" or "yes" or "on";
            var disabled = value is "0" or "false" or "no" or "off";
            if (!enabled && !disabled)
            {
                continue;
            }

            next = key switch
            {
                "focus" => next with { FocusReporting = enabled },
                "mouse" => next with { MouseReporting = enabled },
                "paste" => next with { BracketedPaste = enabled },
                "sync" => next with { SynchronizedUpdates = enabled },
                "decrpm" or "mode_reports" or "mode-reports" => next with { ModeReports = enabled },
                "osc50" or "font_osc50" or "font-osc50" or "supports_osc50_font_requests" or "supports-osc50-font-requests" => next with { SupportsOsc50FontRequests = enabled },
                "iterm2_profile" or "iterm2-profile" or "iterm_profile" or "iterm-profile" or "supports_iterm2_profile_requests" or "supports-iterm2-profile-requests" => next with { SupportsIterm2ProfileRequests = enabled },
                _ => next,
            };
        }

        if (next == profile)
        {
            return profile;
        }

        return next with { Source = $"{profile.Source}+override" };
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
