using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static Task Precedence_InstanceStateBeatsTypeAndGlobal()
    {
        var button = new Button();
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides
        {
            GlobalTheme = BuildThemeWithPrimary(2, 2, 2),
        };

        overrides.SetState(TeaThemeVisualState.Focused, BuildThemeWithPrimary(3, 3, 3));
        overrides.SetControlType<Button>(BuildThemeWithPrimary(4, 4, 4));
        overrides.SetControlTypeState<Button>(TeaThemeVisualState.Focused, BuildThemeWithPrimary(5, 5, 5));
        overrides.SetControlInstance(button, BuildThemeWithPrimary(6, 6, 6));
        overrides.SetControlInstanceState(button, TeaThemeVisualState.Focused, BuildThemeWithPrimary(7, 8, 9));

        var resolved = overrides.Resolve(button, baseTheme, TeaThemeVisualState.Focused);
        var foreground = resolved.Text.Primary.Foreground!.Value;

        TestAssert.True(
            foreground.Red == 7 && foreground.Green == 8 && foreground.Blue == 9,
            "Instance state override should win over type/global layers.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokensForButtonAndStatusBar()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(30, 31, 32)),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Panel = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(40, 41, 42)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(50, 51, 52)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(60, 61, 62)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(70, 71, 72)),
            },
        };

        var button = new Button().ApplyTheme(theme);
        var statusBar = new StatusBar().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Primary, button.LabelStyle, "Button label style should map to Text.Primary.");
        TestAssert.Equal(theme.Focus.Ring, button.FocusedLabelStyle, "Button focused style should map to Focus.Ring.");
        TestAssert.Equal(theme.Text.Muted, button.DisabledLabelStyle, "Button disabled style should map to Text.Muted.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            button.PressedLabelStyle,
            "Button pressed style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Primary, statusBar.LeftTextStyle, "StatusBar left style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Secondary, statusBar.RightTextStyle, "StatusBar right style should map to Text.Secondary.");
        TestAssert.Equal(theme.Surface.Panel, statusBar.FillStyle, "StatusBar fill style should map to Surface.Panel.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoesNotOverwriteExplicitStyles()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Panel = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(10, 20, 30)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
        };

        var button = new Button
        {
            LabelStyle = explicitStyle,
        };
        var statusBar = new StatusBar
        {
            LeftTextStyle = explicitStyle,
        };

        button.ApplyThemeDefaults(theme);
        statusBar.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, button.LabelStyle, "Defaults should not overwrite explicit Button.LabelStyle.");
        TestAssert.Equal(explicitStyle, statusBar.LeftTextStyle, "Defaults should not overwrite explicit StatusBar.LeftTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, statusBar.RightTextStyle, "Defaults should fill empty StatusBar.RightTextStyle.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokensForTableAndTabs()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
        };

        var table = new Table("A", "B").ApplyTheme(theme);
        var tabs = new Tabs("Home", "Logs").ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, table.TitleStyle, "Table title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, table.FocusedTitleStyle, "Table focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, tabs.TitleStyle, "Tabs title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, tabs.FocusedTitleStyle, "Tabs focused title style should map to Focus.Title.");

        return Task.CompletedTask;
    }
}
