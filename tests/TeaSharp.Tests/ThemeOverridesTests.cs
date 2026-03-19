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
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForToolbarCommandBarAndSearchBox",
            ApplyHelpers_MapExpectedTokens_ForToolbarCommandBarAndSearchBox);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForToolbarCommandBarAndSearchBox",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForToolbarCommandBarAndSearchBox);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForToolbarCommandBarAndSearchBox",
            OverrideOverloads_ResolveExpectedTokens_ForToolbarCommandBarAndSearchBox);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForDiffViewAndPropertyGrid",
            ApplyHelpers_MapExpectedTokens_ForDiffViewAndPropertyGrid);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDiffViewAndPropertyGrid",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDiffViewAndPropertyGrid);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForDiffViewAndPropertyGrid",
            OverrideOverloads_ResolveExpectedTokens_ForDiffViewAndPropertyGrid);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter",
            ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter",
            OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForDataGridTreeTableAndKeyValueList",
            ApplyHelpers_MapExpectedTokens_ForDataGridTreeTableAndKeyValueList);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDataGridTreeTableAndKeyValueList",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDataGridTreeTableAndKeyValueList);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForDataGridTreeTableAndKeyValueList",
            OverrideOverloads_ResolveExpectedTokens_ForDataGridTreeTableAndKeyValueList);
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

    private static Task ApplyHelpers_MapExpectedTokens_ForToolbarCommandBarAndSearchBox()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(12, 13, 14)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(32, 33, 34)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(42, 43, 44)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(52, 53, 54)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(62, 63, 64)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(72, 73, 74)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(82, 83, 84)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(92, 93, 94)),
            },
        };

        var toolbar = new Toolbar().ApplyTheme(theme);
        var commandBar = new CommandBar().ApplyTheme(theme);
        var searchBox = new SearchBox().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, toolbar.TitleStyle, "Toolbar title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, toolbar.FocusedTitleStyle, "Toolbar focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, toolbar.ItemStyle, "Toolbar item style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            toolbar.SelectedItemStyle,
            "Toolbar selected item style should map to merged Selection styles.");
        TestAssert.Equal(theme.Focus.Ring, toolbar.FocusedItemStyle, "Toolbar focused item style should map to Focus.Ring.");
        TestAssert.Equal(theme.Text.Muted, toolbar.SeparatorStyle, "Toolbar separator style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, commandBar.TitleStyle, "CommandBar title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, commandBar.FocusedTitleStyle, "CommandBar focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, commandBar.ItemStyle, "CommandBar item style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, commandBar.HoveredItemStyle, "CommandBar hovered style should map to Accent.Secondary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            commandBar.SelectedItemStyle,
            "CommandBar selected item style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, commandBar.DisabledItemStyle, "CommandBar disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, commandBar.SeparatorStyle, "CommandBar separator style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, searchBox.TitleStyle, "SearchBox title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, searchBox.FocusedTitleStyle, "SearchBox focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, searchBox.ValueTextStyle, "SearchBox value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, searchBox.PlaceholderTextStyle, "SearchBox placeholder style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Secondary, searchBox.MatchCounterStyle, "SearchBox match counter style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Primary, searchBox.MatchHighlightStyle, "SearchBox match highlight style should map to Accent.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, searchBox.NavigationLabelStyle, "SearchBox navigation label style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Muted, searchBox.DisabledNavigationLabelStyle, "SearchBox disabled nav style should map to Text.Muted.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForToolbarCommandBarAndSearchBox()
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
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(14, 15, 16)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(23, 24, 25)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(26, 27, 28)),
            },
        };

        var toolbar = new Toolbar
        {
            ItemStyle = explicitStyle,
        };
        var commandBar = new CommandBar
        {
            SelectedItemStyle = explicitStyle,
        };
        var searchBox = new SearchBox
        {
            ValueTextStyle = explicitStyle,
        };

        toolbar.ApplyThemeDefaults(theme);
        commandBar.ApplyThemeDefaults(theme);
        searchBox.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, toolbar.ItemStyle, "Defaults should not overwrite explicit Toolbar.ItemStyle.");
        TestAssert.Equal(theme.Text.Muted, toolbar.SeparatorStyle, "Defaults should fill empty Toolbar.SeparatorStyle.");
        TestAssert.Equal(explicitStyle, commandBar.SelectedItemStyle, "Defaults should not overwrite explicit CommandBar.SelectedItemStyle.");
        TestAssert.Equal(theme.Text.Muted, commandBar.DisabledItemStyle, "Defaults should fill empty CommandBar.DisabledItemStyle.");
        TestAssert.Equal(explicitStyle, searchBox.ValueTextStyle, "Defaults should not overwrite explicit SearchBox.ValueTextStyle.");
        TestAssert.Equal(theme.Accent.Secondary, searchBox.NavigationLabelStyle, "Defaults should fill empty SearchBox.NavigationLabelStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForToolbarCommandBarAndSearchBox()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(231, 232, 233));
        var toolbar = new Toolbar
        {
            ItemStyle = explicitStyle,
        };
        var commandBar = new CommandBar();
        var searchBox = new SearchBox
        {
            ValueTextStyle = explicitStyle,
        };
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(161, 162, 163)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(171, 172, 173)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(181, 182, 183)),
            },
        };
        overrides.SetControlType<Toolbar>(typeTheme);
        overrides.SetControlType<CommandBar>(typeTheme);
        overrides.SetControlType<SearchBox>(typeTheme);

        toolbar.ApplyThemeDefaults(overrides, baseTheme);
        commandBar.ApplyTheme(overrides, baseTheme);
        searchBox.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, toolbar.ItemStyle, "Override defaults should not overwrite explicit Toolbar.ItemStyle.");
        TestAssert.Equal(typeTheme.Focus.Ring, toolbar.FocusedItemStyle, "Override defaults should fill empty Toolbar.FocusedItemStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, commandBar.HoveredItemStyle, "Override apply should map CommandBar hovered style.");
        TestAssert.Equal(
            typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background),
            commandBar.SelectedItemStyle,
            "Override apply should map CommandBar selected style.");
        TestAssert.Equal(explicitStyle, searchBox.ValueTextStyle, "Override defaults should not overwrite explicit SearchBox.ValueTextStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, searchBox.MatchHighlightStyle, "Override defaults should fill empty SearchBox.MatchHighlightStyle.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForDiffViewAndPropertyGrid()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(91, 92, 93)),
            },
        };

        var diffView = new DiffView().ApplyTheme(theme);
        var propertyGrid = new PropertyGrid().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, diffView.TitleStyle, "DiffView title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, diffView.FocusedTitleStyle, "DiffView focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, diffView.HeaderStyle, "DiffView header style should map to Text.Secondary.");
        TestAssert.Equal(theme.State.Success, diffView.AddedLineStyle, "DiffView added line style should map to State.Success.");
        TestAssert.Equal(theme.State.Error, diffView.RemovedLineStyle, "DiffView removed line style should map to State.Error.");
        TestAssert.Equal(theme.Text.Primary, diffView.UnchangedLineStyle, "DiffView unchanged line style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            diffView.SelectedLineStyle,
            "DiffView selected line style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Secondary, propertyGrid.TitleStyle, "PropertyGrid title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, propertyGrid.FocusedTitleStyle, "PropertyGrid focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, propertyGrid.HeaderStyle, "PropertyGrid header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, propertyGrid.KeyStyle, "PropertyGrid key style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, propertyGrid.ValueStyle, "PropertyGrid value style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            propertyGrid.SelectedRowStyle,
            "PropertyGrid selected row style should map to merged Selection styles.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDiffViewAndPropertyGrid()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(19, 20, 21)),
            },
        };

        var diffView = new DiffView
        {
            AddedLineStyle = explicitStyle,
        };
        var propertyGrid = new PropertyGrid
        {
            ValueStyle = explicitStyle,
        };

        diffView.ApplyThemeDefaults(theme);
        propertyGrid.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, diffView.AddedLineStyle, "Defaults should not overwrite explicit DiffView.AddedLineStyle.");
        TestAssert.Equal(theme.State.Error, diffView.RemovedLineStyle, "Defaults should fill empty DiffView.RemovedLineStyle.");
        TestAssert.Equal(explicitStyle, propertyGrid.ValueStyle, "Defaults should not overwrite explicit PropertyGrid.ValueStyle.");
        TestAssert.Equal(theme.Text.Secondary, propertyGrid.HeaderStyle, "Defaults should fill empty PropertyGrid.HeaderStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForDiffViewAndPropertyGrid()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var diffView = new DiffView
        {
            UnchangedLineStyle = explicitStyle,
        };
        var propertyGrid = new PropertyGrid();
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163)),
            },
        };
        overrides.SetControlType<DiffView>(typeTheme);
        overrides.SetControlType<PropertyGrid>(typeTheme);

        diffView.ApplyThemeDefaults(overrides, baseTheme);
        propertyGrid.ApplyTheme(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, diffView.UnchangedLineStyle, "Override defaults should not overwrite explicit DiffView.UnchangedLineStyle.");
        TestAssert.Equal(typeTheme.State.Success, diffView.AddedLineStyle, "Override defaults should fill empty DiffView.AddedLineStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, propertyGrid.KeyStyle, "Override apply should map PropertyGrid key style.");
        TestAssert.Equal(typeTheme.Text.Primary, propertyGrid.ValueStyle, "Override apply should map PropertyGrid value style.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(81, 82, 83)),
            },
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
        };

        var fileExplorer = new FileExplorer().ApplyTheme(theme);
        var fuzzyFinder = new FuzzyFinder().ApplyTheme(theme);
        var toastCenter = new ToastCenter().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, fileExplorer.TitleStyle, "FileExplorer title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, fileExplorer.FocusedTitleStyle, "FileExplorer focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Accent.Primary, fileExplorer.DirectoryStyle, "FileExplorer directory style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Primary, fileExplorer.FileStyle, "FileExplorer file style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            fileExplorer.SelectedStyle,
            "FileExplorer selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, fileExplorer.MutedStyle, "FileExplorer muted style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, fuzzyFinder.TitleStyle, "FuzzyFinder title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, fuzzyFinder.FocusedTitleStyle, "FuzzyFinder focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, fuzzyFinder.ValueTextStyle, "FuzzyFinder value text style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, fuzzyFinder.PlaceholderTextStyle, "FuzzyFinder placeholder style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Primary, fuzzyFinder.ListItemStyle, "FuzzyFinder list item style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            fuzzyFinder.SelectedItemStyle,
            "FuzzyFinder selected item style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Primary, fuzzyFinder.MatchHighlightStyle, "FuzzyFinder match style should map to Accent.Primary.");

        TestAssert.Equal(theme.Text.Secondary, toastCenter.TitleStyle, "ToastCenter title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, toastCenter.FocusedTitleStyle, "ToastCenter focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, toastCenter.ItemStyle, "ToastCenter item style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, toastCenter.HoveredItemStyle, "ToastCenter hovered style should map to Accent.Secondary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            toastCenter.SelectedItemStyle,
            "ToastCenter selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, toastCenter.MutedItemStyle, "ToastCenter muted style should map to Text.Muted.");
        TestAssert.Equal(theme.State.Info, toastCenter.InfoItemStyle, "ToastCenter info style should map to State.Info.");
        TestAssert.Equal(theme.State.Success, toastCenter.SuccessItemStyle, "ToastCenter success style should map to State.Success.");
        TestAssert.Equal(theme.State.Warning, toastCenter.WarningItemStyle, "ToastCenter warning style should map to State.Warning.");
        TestAssert.Equal(theme.State.Error, toastCenter.ErrorItemStyle, "ToastCenter error style should map to State.Error.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter()
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
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(22, 23, 24)),
            },
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(28, 29, 30)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
            },
        };

        var fileExplorer = new FileExplorer
        {
            DirectoryStyle = explicitStyle,
        };
        var fuzzyFinder = new FuzzyFinder
        {
            MatchHighlightStyle = explicitStyle,
        };
        var toastCenter = new ToastCenter
        {
            WarningItemStyle = explicitStyle,
        };

        fileExplorer.ApplyThemeDefaults(theme);
        fuzzyFinder.ApplyThemeDefaults(theme);
        toastCenter.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, fileExplorer.DirectoryStyle, "Defaults should not overwrite explicit FileExplorer.DirectoryStyle.");
        TestAssert.Equal(theme.Text.Primary, fileExplorer.FileStyle, "Defaults should fill empty FileExplorer.FileStyle.");
        TestAssert.Equal(explicitStyle, fuzzyFinder.MatchHighlightStyle, "Defaults should not overwrite explicit FuzzyFinder.MatchHighlightStyle.");
        TestAssert.Equal(theme.Text.Muted, fuzzyFinder.PlaceholderTextStyle, "Defaults should fill empty FuzzyFinder.PlaceholderTextStyle.");
        TestAssert.Equal(explicitStyle, toastCenter.WarningItemStyle, "Defaults should not overwrite explicit ToastCenter.WarningItemStyle.");
        TestAssert.Equal(theme.State.Error, toastCenter.ErrorItemStyle, "Defaults should fill empty ToastCenter.ErrorItemStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var fileExplorer = new FileExplorer();
        var fuzzyFinder = new FuzzyFinder
        {
            ListItemStyle = explicitStyle,
        };
        var toastCenter = new ToastCenter();
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(161, 162, 163)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(171, 172, 173)),
            },
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(181, 182, 183)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(191, 192, 193)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(211, 212, 213)),
            },
        };
        overrides.SetControlType<FileExplorer>(typeTheme);
        overrides.SetControlType<FuzzyFinder>(typeTheme);
        overrides.SetControlType<ToastCenter>(typeTheme);

        fileExplorer.ApplyTheme(overrides, baseTheme);
        fuzzyFinder.ApplyThemeDefaults(overrides, baseTheme);
        toastCenter.ApplyTheme(overrides, baseTheme);

        TestAssert.Equal(typeTheme.Accent.Primary, fileExplorer.DirectoryStyle, "Override apply should map FileExplorer directory style.");
        TestAssert.Equal(typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background), fileExplorer.SelectedStyle, "Override apply should map FileExplorer selected style.");
        TestAssert.Equal(explicitStyle, fuzzyFinder.ListItemStyle, "Override defaults should not overwrite explicit FuzzyFinder.ListItemStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, fuzzyFinder.MatchHighlightStyle, "Override defaults should fill empty FuzzyFinder.MatchHighlightStyle.");
        TestAssert.Equal(typeTheme.State.Info, toastCenter.InfoItemStyle, "Override apply should map ToastCenter info style.");
        TestAssert.Equal(typeTheme.State.Warning, toastCenter.WarningItemStyle, "Override apply should map ToastCenter warning style.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForDataGridTreeTableAndKeyValueList()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(71, 72, 73)),
            },
        };

        var dataGrid = new DataGrid().ApplyTheme(theme);
        var treeTable = new TreeTable().ApplyTheme(theme);
        var keyValueList = new KeyValueList().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, dataGrid.TitleStyle, "DataGrid title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, dataGrid.FocusedTitleStyle, "DataGrid focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, dataGrid.HeaderStyle, "DataGrid header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, dataGrid.RowStyle, "DataGrid row style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            dataGrid.SelectedRowStyle,
            "DataGrid selected row style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Primary, dataGrid.SelectedCellStyle, "DataGrid selected cell style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, dataGrid.MutedStyle, "DataGrid muted style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, treeTable.TitleStyle, "TreeTable title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, treeTable.FocusedTitleStyle, "TreeTable focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, treeTable.HeaderStyle, "TreeTable header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Primary, treeTable.BranchRowStyle, "TreeTable branch style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Primary, treeTable.LeafRowStyle, "TreeTable leaf style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            treeTable.SelectedRowStyle,
            "TreeTable selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, treeTable.MutedRowStyle, "TreeTable muted style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, keyValueList.TitleStyle, "KeyValueList title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, keyValueList.FocusedTitleStyle, "KeyValueList focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, keyValueList.KeyStyle, "KeyValueList key style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, keyValueList.ValueStyle, "KeyValueList value style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            keyValueList.SelectedRowStyle,
            "KeyValueList selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, keyValueList.SeparatorStyle, "KeyValueList separator style should map to Text.Muted.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDataGridTreeTableAndKeyValueList()
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
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(19, 20, 21)),
            },
        };

        var dataGrid = new DataGrid
        {
            RowStyle = explicitStyle,
        };
        var treeTable = new TreeTable
        {
            LeafRowStyle = explicitStyle,
        };
        var keyValueList = new KeyValueList
        {
            ValueStyle = explicitStyle,
        };

        dataGrid.ApplyThemeDefaults(theme);
        treeTable.ApplyThemeDefaults(theme);
        keyValueList.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, dataGrid.RowStyle, "Defaults should not overwrite explicit DataGrid.RowStyle.");
        TestAssert.Equal(theme.Accent.Primary, dataGrid.SelectedCellStyle, "Defaults should fill empty DataGrid.SelectedCellStyle.");
        TestAssert.Equal(explicitStyle, treeTable.LeafRowStyle, "Defaults should not overwrite explicit TreeTable.LeafRowStyle.");
        TestAssert.Equal(theme.Accent.Primary, treeTable.BranchRowStyle, "Defaults should fill empty TreeTable.BranchRowStyle.");
        TestAssert.Equal(explicitStyle, keyValueList.ValueStyle, "Defaults should not overwrite explicit KeyValueList.ValueStyle.");
        TestAssert.Equal(theme.Text.Muted, keyValueList.SeparatorStyle, "Defaults should fill empty KeyValueList.SeparatorStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForDataGridTreeTableAndKeyValueList()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var dataGrid = new DataGrid
        {
            RowStyle = explicitStyle,
        };
        var treeTable = new TreeTable();
        var keyValueList = new KeyValueList
        {
            ValueStyle = explicitStyle,
        };

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
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

        overrides.SetControlType<DataGrid>(typeTheme);
        overrides.SetControlType<TreeTable>(typeTheme);
        overrides.SetControlType<KeyValueList>(typeTheme);

        dataGrid.ApplyThemeDefaults(overrides, baseTheme);
        treeTable.ApplyTheme(overrides, baseTheme);
        keyValueList.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, dataGrid.RowStyle, "Override defaults should not overwrite explicit DataGrid.RowStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, dataGrid.SelectedCellStyle, "Override defaults should fill empty DataGrid.SelectedCellStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, treeTable.BranchRowStyle, "Override apply should map TreeTable branch style.");
        TestAssert.Equal(typeTheme.Text.Primary, treeTable.LeafRowStyle, "Override apply should map TreeTable leaf style.");
        TestAssert.Equal(explicitStyle, keyValueList.ValueStyle, "Override defaults should not overwrite explicit KeyValueList.ValueStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, keyValueList.SeparatorStyle, "Override defaults should fill empty KeyValueList.SeparatorStyle.");

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
