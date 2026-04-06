using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> FoundationCases()
    {
        yield return new TestCase(
            "ThemeOverrides_Precedence_InstanceStateBeatsTypeAndGlobal",
            Precedence_InstanceStateBeatsTypeAndGlobal);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForButtonAndStatusBar",
            ApplyHelpers_MapExpectedTokensForButtonAndStatusBar);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoesNotOverwriteExplicitStyles",
            ApplyThemeDefaults_DoesNotOverwriteExplicitStyles);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForTableAndTabs",
            ApplyHelpers_MapExpectedTokensForTableAndTabs);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForListViewBorderStyles",
            ApplyHelpers_MapExpectedTokensForListViewBorderStyles);
        yield return new TestCase(
            "ThemeOverrides_TableBorderStyles_DefaultsAndOverrides_PreserveExplicitStyles",
            TableBorderStyles_DefaultsAndOverrides_PreserveExplicitStyles);
    }

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
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(43, 44, 45)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(46, 47, 48)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(50, 51, 52)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(53, 54, 55)),
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
        TestAssert.Equal(theme.Text.Primary.WithBold(), button.FocusedLabelStyle, "Button focused label style should stay text-only.");
        TestAssert.Equal(theme.Text.Muted, button.DisabledLabelStyle, "Button disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Surface.Panel, button.SurfaceStyle, "Button surface style should map to the resolved button surface.");
        TestAssert.Equal(theme.Surface.Panel, button.FocusedSurfaceStyle, "Button focused surface style should keep the resolved button surface.");
        TestAssert.Equal(
            theme.Text.Primary.WithBold(),
            button.PressedLabelStyle,
            "Button pressed label style should stay text-only.");
        TestAssert.Equal(theme.Selection.Background, button.PressedSurfaceStyle, "Button pressed surface style should use the pressed selection surface.");

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
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
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
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(44, 45, 46)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(47, 48, 49)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56)),
            },
        };

        var table = new Table("A", "B").ApplyTheme(theme);
        var tabs = new Tabs("Home", "Logs").ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, table.TitleStyle, "Table title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, table.FocusedTitleStyle, "Table focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Border.Default, table.BorderStyleText, "Table border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), table.FocusedBorderStyleText, "Table focused border style should map to focused border tokens.");
        TestAssert.Equal(theme.Text.Secondary, tabs.TitleStyle, "Tabs title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, tabs.FocusedTitleStyle, "Tabs focused title style should map to Focus.Title.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokensForListViewBorderStyles()
    {
        var explicitBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(210, 211, 212));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(91, 92, 93)),
            },
        };

        var list = new ListView<string>(x => x).ApplyTheme(theme);
        TestAssert.Equal(theme.Border.Default, list.BorderStyleText, "ListView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), list.FocusedBorderStyleText, "ListView focused border style should map to focused border tokens.");

        list = new ListView<string>(x => x)
        {
            BorderStyleText = explicitBorderStyle,
        };
        list.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitBorderStyle, list.BorderStyleText, "ListView defaults should not overwrite explicit border style.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), list.FocusedBorderStyleText, "ListView defaults should fill focused border style.");
        return Task.CompletedTask;
    }

    private static Task TableBorderStyles_DefaultsAndOverrides_PreserveExplicitStyles()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
            },
        };

        var table = new Table("A", "B")
        {
            BorderStyleText = explicitStyle,
        };
        table.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, table.BorderStyleText, "Table defaults should not overwrite explicit border style.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), table.FocusedBorderStyleText, "Table defaults should fill focused border style.");

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
        };
        var overrides = new TeaThemeOverrides();
        overrides.SetControlType<Table>(typeTheme);

        var overrideApplied = new Table("A", "B");
        overrideApplied.ApplyTheme(overrides, baseTheme);
        TestAssert.Equal(typeTheme.Border.Default, overrideApplied.BorderStyleText, "Override apply should map Table border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), overrideApplied.FocusedBorderStyleText, "Override apply should map Table focused border style.");

        var overrideDefaults = new Table("A", "B")
        {
            BorderStyleText = explicitStyle,
        };
        overrideDefaults.ApplyThemeDefaults(overrides, baseTheme);
        TestAssert.Equal(explicitStyle, overrideDefaults.BorderStyleText, "Override defaults should not overwrite explicit Table border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), overrideDefaults.FocusedBorderStyleText, "Override defaults should fill Table focused border style.");
        return Task.CompletedTask;
    }
}
