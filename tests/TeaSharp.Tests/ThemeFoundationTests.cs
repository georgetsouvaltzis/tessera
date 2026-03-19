using TeaSharp.Styles;

namespace TeaSharp.Tests;

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
    }

    private static Task CatppuccinMocha_ProvidesExpectedKeyTokens()
    {
        var theme = TeaThemes.Catppuccin(CatppuccinVariant.Mocha);

        AssertForegroundRgb(theme.Text.Primary, 0xCD, 0xD6, 0xF4, "Catppuccin Mocha should set Text.Primary.");
        AssertForegroundRgb(theme.Border.Focused, 0x89, 0xB4, 0xFA, "Catppuccin Mocha should set Border.Focused.");
        AssertBackgroundRgb(theme.Surface.Base, 0x1E, 0x1E, 0x2E, "Catppuccin Mocha should set Surface.Base.");
        return Task.CompletedTask;
    }

    private static Task RosePineMain_ProvidesExpectedKeyTokens()
    {
        var theme = TeaThemes.RosePine(RosePineVariant.Main);

        AssertForegroundRgb(theme.Text.Primary, 0xE0, 0xDE, 0xF4, "Rosé Pine Main should set Text.Primary.");
        AssertForegroundRgb(theme.Focus.Border, 0x9C, 0xCF, 0xD8, "Rosé Pine Main should set Focus.Border.");
        AssertBackgroundRgb(theme.Surface.Base, 0x19, 0x17, 0x24, "Rosé Pine Main should set Surface.Base.");
        return Task.CompletedTask;
    }

    private static Task CustomObjectInit_AllowsTokenOverrides()
    {
        var customPrimary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3));
        var customFocusBorder = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(10, 20, 30));

        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = customPrimary,
            },
            Focus = new TeaThemeFocusTokens
            {
                Border = customFocusBorder,
            },
        };

        TestAssert.Equal(customPrimary, theme.Text.Primary, "Custom theme should preserve Text.Primary overrides.");
        TestAssert.Equal(customFocusBorder, theme.Focus.Border, "Custom theme should preserve Focus.Border overrides.");
        return Task.CompletedTask;
    }

    private static Task RuntimeOptions_HoldsThemeWithoutSideEffects()
    {
        var theme = TeaThemes.Catppuccin(CatppuccinVariant.Frappe);
        var options = new TeaRuntimeOptions
        {
            Theme = theme,
        };

        TestAssert.ReferenceSame(theme, options.Theme!, "TeaRuntimeOptions.Theme should hold the assigned theme reference.");
        TestAssert.Equal(60, options.MaxFps, "Theme assignment should not change MaxFps defaults.");
        TestAssert.ReferenceSame(ScreenOptions.Empty, options.Screen, "Theme assignment should not replace Screen defaults.");
        return Task.CompletedTask;
    }

    private static void AssertForegroundRgb(TeaStyle style, byte red, byte green, byte blue, string message)
    {
        TestAssert.True(!style.IsEmpty, $"{message} Style should not be empty.");
        TestAssert.True(style.Foreground is not null, $"{message} Foreground should be set.");
        var foreground = style.Foreground!.Value;
        TestAssert.True(foreground.Mode == AnsiColorMode.Rgb, $"{message} Foreground should use RGB mode.");
        TestAssert.True(foreground.Red == red && foreground.Green == green && foreground.Blue == blue, message);
    }

    private static void AssertBackgroundRgb(TeaStyle style, byte red, byte green, byte blue, string message)
    {
        TestAssert.True(!style.IsEmpty, $"{message} Style should not be empty.");
        TestAssert.True(style.Background is not null, $"{message} Background should be set.");
        var background = style.Background!.Value;
        TestAssert.True(background.Mode == AnsiColorMode.Rgb, $"{message} Background should use RGB mode.");
        TestAssert.True(background.Red == red && background.Green == green && background.Blue == blue, message);
    }
}
