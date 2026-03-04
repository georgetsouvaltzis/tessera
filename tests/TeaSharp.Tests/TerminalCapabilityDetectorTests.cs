using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class TerminalCapabilityDetectorTests
{
    private static readonly object EnvLock = new();

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CapabilityDetector_TERM_Dumb_DisablesAdvancedModes", TermDumb_DisablesAdvancedModes);
        yield return new TestCase("CapabilityDetector_AppleTerminal_DisablesSyncUpdates", AppleTerminal_DisablesSyncUpdates);
        yield return new TestCase("CapabilityDetector_Xterm_EnablesAllModes", Xterm_EnablesAllModes);
    }

    private static Task TermDumb_DisablesAdvancedModes()
    {
        // Arrange + Act
        var profile = DetectWithEnvironment(("TERM", "dumb"), ("TERM_PROGRAM", null), ("WT_SESSION", null));

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
        var profile = DetectWithEnvironment(
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
        var profile = DetectWithEnvironment(("TERM", "xterm-256color"), ("TERM_PROGRAM", null), ("WT_SESSION", null));

        // Assert
        TestAssert.True(profile.FocusReporting, "xterm should enable focus reporting.");
        TestAssert.True(profile.MouseReporting, "xterm should enable mouse reporting.");
        TestAssert.True(profile.BracketedPaste, "xterm should enable bracketed paste.");
        TestAssert.True(profile.SynchronizedUpdates, "xterm should enable synchronized updates.");
        TestAssert.True(profile.ModeReports, "xterm should enable mode reports.");
        return Task.CompletedTask;
    }

    private static TerminalCapabilityProfile DetectWithEnvironment(params (string Name, string? Value)[] values)
    {
        lock (EnvLock)
        {
            var originals = new Dictionary<string, string?>(StringComparer.Ordinal);
            foreach (var (name, _) in values)
            {
                originals[name] = Environment.GetEnvironmentVariable(name);
            }

            try
            {
                foreach (var (name, value) in values)
                {
                    Environment.SetEnvironmentVariable(name, value);
                }

                return TerminalCapabilityDetector.Detect();
            }
            finally
            {
                foreach (var original in originals)
                {
                    Environment.SetEnvironmentVariable(original.Key, original.Value);
                }
            }
        }
    }
}
