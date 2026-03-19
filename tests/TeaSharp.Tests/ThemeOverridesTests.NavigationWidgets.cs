using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> NavigationCases()
    {
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
}
