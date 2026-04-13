using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class ThemeFoundationTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "Theme_CatppuccinMocha_ProvidesExpectedKeyTokens",
            CatppuccinMocha_ProvidesExpectedKeyTokens);
        yield return new TestCase(
            "Theme_RosePineMain_ProvidesExpectedKeyTokens",
            RosePineMain_ProvidesExpectedKeyTokens);
        yield return new TestCase(
            "Theme_CustomObjectInit_AllowsTokenOverrides",
            CustomObjectInit_AllowsTokenOverrides);
        yield return new TestCase(
            "Theme_RuntimeOptions_HoldsThemeWithoutSideEffects",
            RuntimeOptions_HoldsThemeWithoutSideEffects);
        yield return new TestCase(
            "Theme_BuiltInThemes_ProvideDefaultFocusMarker",
            BuiltInThemes_ProvideDefaultFocusMarker);
        yield return new TestCase(
            "Theme_Merge_PreservesFocusMarkerWhenOverlayUnspecified",
            Merge_PreservesFocusMarkerWhenOverlayUnspecified);
    }

    private static Task CatppuccinMocha_ProvidesExpectedKeyTokens()
    {
        var theme = TesseraThemes.Catppuccin();

        AssertForegroundRgb(theme.Text.Primary, 0xCD, 0xD6, 0xF4, "Catppuccin Mocha should set Text.Primary.");
        AssertForegroundRgb(theme.Border.Focused, 0x89, 0xB4, 0xFA, "Catppuccin Mocha should set Border.Focused.");
        AssertBackgroundRgb(theme.Surface.Base, 0x1E, 0x1E, 0x2E, "Catppuccin Mocha should set Surface.Base.");
        return Task.CompletedTask;
    }

    private static Task RosePineMain_ProvidesExpectedKeyTokens()
    {
        var theme = TesseraThemes.RosePine();

        AssertForegroundRgb(theme.Text.Primary, 0xE0, 0xDE, 0xF4, "Rosé Pine Main should set Text.Primary.");
        AssertForegroundRgb(theme.Focus.Border, 0x9C, 0xCF, 0xD8, "Rosé Pine Main should set Focus.Border.");
        TestAssert.Equal("*", theme.Focus.Marker, "Rosé Pine Main should set default focus marker.");
        AssertBackgroundRgb(theme.Surface.Base, 0x19, 0x17, 0x24, "Rosé Pine Main should set Surface.Base.");
        return Task.CompletedTask;
    }

    private static Task CustomObjectInit_AllowsTokenOverrides()
    {
        var customPrimary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3));
        var customFocusBorder = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(10, 20, 30));
        const string customMarker = ">>";

        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens { Primary = customPrimary },
            Focus = new TesseraThemeFocusTokens { Border = customFocusBorder, Marker = customMarker }
        };

        TestAssert.Equal(customPrimary, theme.Text.Primary, "Custom theme should preserve Text.Primary overrides.");
        TestAssert.Equal(customFocusBorder, theme.Focus.Border, "Custom theme should preserve Focus.Border overrides.");
        TestAssert.Equal(customMarker, theme.Focus.Marker, "Custom theme should preserve Focus.Marker overrides.");
        return Task.CompletedTask;
    }

    private static Task RuntimeOptions_HoldsThemeWithoutSideEffects()
    {
        var theme = TesseraThemes.Catppuccin(CatppuccinVariant.Frappe);
        var options = new TesseraRuntimeOptions { Theme = theme };

        TestAssert.ReferenceSame(theme, options.Theme!,
            "TesseraRuntimeOptions.Theme should hold the assigned theme reference.");
        TestAssert.Equal(60, options.MaxFps, "Theme assignment should not change MaxFps defaults.");
        TestAssert.ReferenceSame(ScreenOptions.Empty, options.Screen,
            "Theme assignment should not replace Screen defaults.");
        return Task.CompletedTask;
    }

    private static Task BuiltInThemes_ProvideDefaultFocusMarker()
    {
        var catppuccin = TesseraThemes.Catppuccin();
        var rosePine = TesseraThemes.RosePine();

        TestAssert.Equal("*", catppuccin.Focus.Marker, "Catppuccin themes should set default focus marker.");
        TestAssert.Equal("*", rosePine.Focus.Marker, "Rosé Pine themes should set default focus marker.");
        return Task.CompletedTask;
    }

    private static Task Merge_PreservesFocusMarkerWhenOverlayUnspecified()
    {
        var baseTheme = new TesseraTheme { Focus = new TesseraThemeFocusTokens { Marker = ">>" } };

        var overrides = new TesseraThemeOverrides
        {
            GlobalTheme = new TesseraTheme { Focus = new TesseraThemeFocusTokens() }
        };

        var resolvedUnspecified = overrides.Resolve(new Choice(), baseTheme);
        TestAssert.Equal(">>", resolvedUnspecified.Focus.Marker,
            "Unspecified overlay marker should not clear base marker.");

        overrides.GlobalTheme = new TesseraTheme { Focus = new TesseraThemeFocusTokens { Marker = "::" } };

        var resolvedSpecified = overrides.Resolve(new Choice(), baseTheme);
        TestAssert.Equal("::", resolvedSpecified.Focus.Marker, "Specified overlay marker should override base marker.");
        return Task.CompletedTask;
    }

    private static void AssertForegroundRgb(TesseraStyle style, byte red, byte green, byte blue, string message)
    {
        TestAssert.True(!style.IsEmpty, $"{message} Style should not be empty.");
        TestAssert.True(style.Foreground is not null, $"{message} Foreground should be set.");
        var foreground = style.Foreground!.Value;
        TestAssert.True(foreground.Mode == AnsiColorMode.Rgb, $"{message} Foreground should use RGB mode.");
        TestAssert.True(foreground.Red == red && foreground.Green == green && foreground.Blue == blue, message);
    }

    private static void AssertBackgroundRgb(TesseraStyle style, byte red, byte green, byte blue, string message)
    {
        TestAssert.True(!style.IsEmpty, $"{message} Style should not be empty.");
        TestAssert.True(style.Background is not null, $"{message} Background should be set.");
        var background = style.Background!.Value;
        TestAssert.True(background.Mode == AnsiColorMode.Rgb, $"{message} Background should use RGB mode.");
        TestAssert.True(background.Red == red && background.Green == green && background.Blue == blue, message);
    }
}
