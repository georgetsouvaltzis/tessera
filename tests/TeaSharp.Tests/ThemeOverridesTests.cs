using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class ThemeOverridesTests
{
    public static IEnumerable<TestCase> Cases()
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
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForBreadcrumbAndPaginator",
            ApplyHelpers_MapExpectedTokensForBreadcrumbAndPaginator);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoesNotOverwriteExplicitStyles_ForBreadcrumbAndPaginator",
            ApplyThemeDefaults_DoesNotOverwriteExplicitStyles_ForBreadcrumbAndPaginator);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForBreadcrumbAndPaginator",
            OverrideOverloads_ResolveExpectedTokens_ForBreadcrumbAndPaginator);
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

    private static Task ApplyHelpers_MapExpectedTokensForBreadcrumbAndPaginator()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(111, 112, 113)),
            },
        };

        var breadcrumb = new Breadcrumb().ApplyTheme(theme);
        var paginator = new Paginator().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, breadcrumb.TitleStyle, "Breadcrumb title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, breadcrumb.FocusedTitleStyle, "Breadcrumb focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, breadcrumb.ItemStyle, "Breadcrumb item style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            breadcrumb.SelectedItemStyle,
            "Breadcrumb selected item style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, breadcrumb.SeparatorStyle, "Breadcrumb separator style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, paginator.TitleStyle, "Paginator title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, paginator.FocusedTitleStyle, "Paginator focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, paginator.LabelStyle, "Paginator label style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            paginator.ActivePageLabelStyle,
            "Paginator active page style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, paginator.DisabledNavigationLabelStyle, "Paginator disabled style should map to Text.Muted.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoesNotOverwriteExplicitStyles_ForBreadcrumbAndPaginator()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(41, 42, 43)),
            },
        };

        var breadcrumb = new Breadcrumb
        {
            ItemStyle = explicitStyle,
        };
        var paginator = new Paginator
        {
            LabelStyle = explicitStyle,
        };

        breadcrumb.ApplyThemeDefaults(theme);
        paginator.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, breadcrumb.ItemStyle, "Defaults should not overwrite explicit Breadcrumb.ItemStyle.");
        TestAssert.Equal(theme.Text.Muted, breadcrumb.SeparatorStyle, "Defaults should fill empty Breadcrumb.SeparatorStyle.");
        TestAssert.Equal(explicitStyle, paginator.LabelStyle, "Defaults should not overwrite explicit Paginator.LabelStyle.");
        TestAssert.Equal(theme.Text.Muted, paginator.DisabledNavigationLabelStyle, "Defaults should fill empty Paginator.DisabledNavigationLabelStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForBreadcrumbAndPaginator()
    {
        var breadcrumb = new Breadcrumb
        {
            ItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203)),
        };
        var paginator = new Paginator
        {
            LabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(211, 212, 213)),
        };
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163)),
            },
        };
        overrides.SetControlType<Breadcrumb>(typeTheme);
        overrides.SetControlType<Paginator>(typeTheme);

        breadcrumb.ApplyTheme(overrides, baseTheme);
        paginator.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(typeTheme.Text.Primary, breadcrumb.ItemStyle, "Override-based apply should map Breadcrumb item style from resolved theme.");
        TestAssert.Equal(typeTheme.Text.Secondary, breadcrumb.TitleStyle, "Override-based apply should map Breadcrumb title style from resolved theme.");
        TestAssert.Equal(
            typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background),
            breadcrumb.SelectedItemStyle,
            "Override-based apply should map Breadcrumb selected style from resolved theme.");
        TestAssert.Equal(
            TeaStyle.Empty.WithForeground(AnsiColor.Rgb(211, 212, 213)),
            paginator.LabelStyle,
            "Override-based defaults should not overwrite explicit Paginator.LabelStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, paginator.DisabledNavigationLabelStyle, "Override-based defaults should fill empty Paginator disabled style.");

        return Task.CompletedTask;
    }

    private static TeaTheme BuildThemeWithPrimary(byte red, byte green, byte blue)
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue)),
            },
        };
    }
}
