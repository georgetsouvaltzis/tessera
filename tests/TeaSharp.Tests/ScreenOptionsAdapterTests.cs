using TeaSharp.Internal;

namespace TeaSharp.Tests;

internal static class ScreenOptionsAdapterTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "ScreenOptionsAdapter_ToTerminalOutput_MapsFontSpec",
            ToTerminalOutput_MapsFontSpec);
        yield return new TestCase(
            "ScreenOptionsAdapter_ToScreenOptions_MapsFontSpec",
            ToScreenOptions_MapsFontSpec);
    }

    private static Task ToTerminalOutput_MapsFontSpec()
    {
        var options = new ScreenOptions
        {
            FontSpec = "Iosevka Term 14",
        };

        var output = options.ToTerminalOutput();

        TestAssert.True(
            string.Equals(output.FontSpec, "Iosevka Term 14", StringComparison.Ordinal),
            "ScreenOptionsAdapter should map ScreenOptions.FontSpec to TerminalOutput.FontSpec.");
        return Task.CompletedTask;
    }

    private static Task ToScreenOptions_MapsFontSpec()
    {
        var output = new TeaSharp.Core.Abstractions.TerminalOutput
        {
            FontSpec = "JetBrains Mono 13",
        };

        var options = output.ToScreenOptions();

        TestAssert.True(
            string.Equals(options.FontSpec, "JetBrains Mono 13", StringComparison.Ordinal),
            "ScreenOptionsAdapter should map TerminalOutput.FontSpec to ScreenOptions.FontSpec.");
        return Task.CompletedTask;
    }
}
