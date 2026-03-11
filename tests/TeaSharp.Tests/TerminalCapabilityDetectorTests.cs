using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class TerminalCapabilityDetectorTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CapabilityDetector_TERM_Dumb_DisablesAdvancedModes", TermDumb_DisablesAdvancedModes);
        yield return new TestCase("CapabilityDetector_AppleTerminal_DisablesSyncUpdates", AppleTerminal_DisablesSyncUpdates);
        yield return new TestCase("CapabilityDetector_Xterm_EnablesAllModes", Xterm_EnablesAllModes);
        yield return new TestCase("CapabilityDetector_TerminfoEnrichesLinuxMouse", TerminfoEnrichesLinuxMouse);
        yield return new TestCase("CapabilityDetector_TerminfoEnrichesVt100Extensions", TerminfoEnrichesVt100Extensions);
        yield return new TestCase("CapabilityDetector_EnvOverride_AppliesCaps", EnvOverride_AppliesCaps);
    }

    private static Task TermDumb_DisablesAdvancedModes()
    {
        // Arrange + Act
        var profile = Detect(("TERM", "dumb"));

        // Assert
        TestAssert.True(!profile.FocusReporting, "TERM=dumb should disable focus reporting.");
        TestAssert.True(!profile.MouseReporting, "TERM=dumb should disable mouse reporting.");
        TestAssert.True(!profile.BracketedPaste, "TERM=dumb should disable bracketed paste.");
        TestAssert.True(!profile.SynchronizedUpdates, "TERM=dumb should disable synchronized updates.");
        TestAssert.True(!profile.ModeReports, "TERM=dumb should disable mode reports.");
        return Task.CompletedTask;
    }

    private static Task AppleTerminal_DisablesSyncUpdates()
    {
        // Arrange + Act
        var profile = Detect(
            ("TERM", "xterm-256color"),
            ("TERM_PROGRAM", "Apple_Terminal"),
            ("WT_SESSION", null));

        // Assert
        TestAssert.True(profile.FocusReporting, "Apple Terminal should keep focus reporting enabled.");
        TestAssert.True(profile.MouseReporting, "Apple Terminal should keep mouse reporting enabled.");
        TestAssert.True(profile.BracketedPaste, "Apple Terminal should keep bracketed paste enabled.");
        TestAssert.True(!profile.SynchronizedUpdates, "Apple Terminal should disable synchronized updates.");
        TestAssert.True(profile.ModeReports, "Apple Terminal should keep mode reports enabled.");
        return Task.CompletedTask;
    }

    private static Task Xterm_EnablesAllModes()
    {
        // Arrange + Act
        var profile = Detect(("TERM", "xterm-256color"));

        // Assert
        TestAssert.True(profile.FocusReporting, "xterm should enable focus reporting.");
        TestAssert.True(profile.MouseReporting, "xterm should enable mouse reporting.");
        TestAssert.True(profile.BracketedPaste, "xterm should enable bracketed paste.");
        TestAssert.True(profile.SynchronizedUpdates, "xterm should enable synchronized updates.");
        TestAssert.True(profile.ModeReports, "xterm should enable mode reports.");
        return Task.CompletedTask;
    }

    private static Task TerminfoEnrichesLinuxMouse()
    {
        // Arrange + Act
        var profile = Detect(
            terminfo: "linux|linux console, kmous=\\E[M,",
            ("TERM", "linux"));

        // Assert
        TestAssert.True(!profile.FocusReporting, "linux should keep focus reporting disabled.");
        TestAssert.True(profile.MouseReporting, "terminfo kmous should enable mouse reporting for linux.");
        TestAssert.True(!profile.BracketedPaste, "linux should keep bracketed paste disabled.");
        TestAssert.True(!profile.SynchronizedUpdates, "linux should keep synchronized updates disabled.");
        TestAssert.True(!profile.ModeReports, "linux should keep mode reports disabled.");
        TestAssert.True(
            profile.Source.Contains("terminfo:linux", StringComparison.Ordinal),
            "Source should include terminfo enrichment marker.");
        return Task.CompletedTask;
    }

    private static Task TerminfoEnrichesVt100Extensions()
    {
        // Arrange + Act
        var profile = Detect(
            terminfo: "vt100|vt100, XT,",
            ("TERM", "vt100"));

        // Assert
        TestAssert.True(profile.FocusReporting, "terminfo XT should enable focus reporting.");
        TestAssert.True(!profile.MouseReporting, "vt100 should keep mouse reporting disabled without kmous.");
        TestAssert.True(profile.BracketedPaste, "terminfo XT should enable bracketed paste.");
        TestAssert.True(!profile.SynchronizedUpdates, "vt100 should keep synchronized updates disabled without Sync.");
        TestAssert.True(profile.ModeReports, "terminfo XT should enable mode reports.");
        return Task.CompletedTask;
    }

    private static Task EnvOverride_AppliesCaps()
    {
        // Arrange + Act
        var profile = Detect(
            ("TERM", "xterm-256color"),
            ("TEASHARP_CAPS", "focus=0,mouse=0,paste=1,sync=0,decrpm=0"));

        // Assert
        TestAssert.True(!profile.FocusReporting, "Override should disable focus reporting.");
        TestAssert.True(!profile.MouseReporting, "Override should disable mouse reporting.");
        TestAssert.True(profile.BracketedPaste, "Override should allow enabling bracketed paste.");
        TestAssert.True(!profile.SynchronizedUpdates, "Override should disable synchronized updates.");
        TestAssert.True(!profile.ModeReports, "Override should disable mode reports.");
        TestAssert.True(
            profile.Source.Contains("+override", StringComparison.Ordinal),
            "Source should include override marker when TEASHARP_CAPS applies.");
        return Task.CompletedTask;
    }

    private static TerminalCapabilityProfile Detect(
        params (string Name, string? Value)[] values)
    {
        return Detect(null, values);
    }

    private static TerminalCapabilityProfile Detect(
        string? terminfo,
        params (string Name, string? Value)[] values)
    {
        var env = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var (name, value) in values)
        {
            env[name] = value;
        }

        return TerminalCapabilityDetector.Detect(
            name => env.TryGetValue(name, out var value) ? value : null,
            _ => terminfo,
            isWindows: false);
    }
}
