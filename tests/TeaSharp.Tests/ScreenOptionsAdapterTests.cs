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
        yield return new TestCase(
            "ScreenOptionsAdapter_ToTerminalOutput_MapsStructuredFontRequest",
            ToTerminalOutput_MapsStructuredFontRequest);
        yield return new TestCase(
            "ScreenOptionsAdapter_ToScreenOptions_MapsStructuredFontRequest",
            ToScreenOptions_MapsStructuredFontRequest);
        yield return new TestCase(
            "ScreenOptionsAdapter_HostingCapabilityProfile_ToCore_MapsFontFlags",
            HostingCapabilityProfile_ToCore_MapsFontFlags);
        yield return new TestCase(
            "ScreenOptionsAdapter_CoreCapabilityProfile_AsHosting_MapsFontFlags",
            CoreCapabilityProfile_AsHosting_MapsFontFlags);
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

    private static Task ToTerminalOutput_MapsStructuredFontRequest()
    {
        var options = new ScreenOptions
        {
            FontFamily = "Iosevka Term",
            FontSize = 14,
            Iterm2Profile = "TeaSharp",
        };

        var output = options.ToTerminalOutput();

        TestAssert.True(
            string.Equals(output.FontFamily, "Iosevka Term", StringComparison.Ordinal),
            "ScreenOptionsAdapter should map ScreenOptions.FontFamily to TerminalOutput.FontFamily.");
        TestAssert.True(output.FontSize == 14, "ScreenOptionsAdapter should map ScreenOptions.FontSize to TerminalOutput.FontSize.");
        TestAssert.True(
            string.Equals(output.Iterm2Profile, "TeaSharp", StringComparison.Ordinal),
            "ScreenOptionsAdapter should map ScreenOptions.Iterm2Profile to TerminalOutput.Iterm2Profile.");
        return Task.CompletedTask;
    }

    private static Task ToScreenOptions_MapsStructuredFontRequest()
    {
        var output = new TeaSharp.Core.Abstractions.TerminalOutput
        {
            FontFamily = "JetBrains Mono",
            FontSize = 13,
            Iterm2Profile = "Work",
        };

        var options = output.ToScreenOptions();

        TestAssert.True(
            string.Equals(options.FontFamily, "JetBrains Mono", StringComparison.Ordinal),
            "ScreenOptionsAdapter should map TerminalOutput.FontFamily to ScreenOptions.FontFamily.");
        TestAssert.True(options.FontSize == 13, "ScreenOptionsAdapter should map TerminalOutput.FontSize to ScreenOptions.FontSize.");
        TestAssert.True(
            string.Equals(options.Iterm2Profile, "Work", StringComparison.Ordinal),
            "ScreenOptionsAdapter should map TerminalOutput.Iterm2Profile to ScreenOptions.Iterm2Profile.");
        return Task.CompletedTask;
    }

    private static Task HostingCapabilityProfile_ToCore_MapsFontFlags()
    {
        var hostingProfile = new TeaSharp.Hosting.TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            SupportsOsc50FontRequests: false,
            SupportsIterm2ProfileRequests: true,
            Source: "hosting");

        var coreProfile = hostingProfile.ToCore();

        TestAssert.True(!coreProfile.SupportsOsc50FontRequests, "Hosting profile should map SupportsOsc50FontRequests to core.");
        TestAssert.True(coreProfile.SupportsIterm2ProfileRequests, "Hosting profile should map SupportsIterm2ProfileRequests to core.");
        return Task.CompletedTask;
    }

    private static Task CoreCapabilityProfile_AsHosting_MapsFontFlags()
    {
        var coreProfile = new TeaSharp.Core.Terminal.TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            SupportsOsc50FontRequests: false,
            SupportsIterm2ProfileRequests: true,
            Source: "core");

        var hostingProfile = coreProfile.AsHosting();

        TestAssert.True(!hostingProfile.SupportsOsc50FontRequests, "Core profile should map SupportsOsc50FontRequests to hosting.");
        TestAssert.True(hostingProfile.SupportsIterm2ProfileRequests, "Core profile should map SupportsIterm2ProfileRequests to hosting.");
        return Task.CompletedTask;
    }
}
