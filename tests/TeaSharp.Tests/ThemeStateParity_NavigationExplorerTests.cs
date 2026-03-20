using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class ThemeStateParity_NavigationExplorerTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "ThemeStateParity_ApplyTheme_MapsHoverTokens_ForNavigationExplorerControls",
            ApplyTheme_MapsHoverTokens_ForNavigationExplorerControls);
        yield return new TestCase(
            "ThemeStateParity_ApplyThemeDefaults_PreservesExplicitHoverStyles",
            ApplyThemeDefaults_PreservesExplicitHoverStyles);
        yield return new TestCase(
            "ThemeStateParity_Overrides_ResolveHoverTokens_ForNavigationExplorerControls",
            Overrides_ResolveHoverTokens_ForNavigationExplorerControls);
    }

    private static Task ApplyTheme_MapsHoverTokens_ForNavigationExplorerControls()
    {
        var theme = BuildTheme();
        var mergedSelection = theme.Selection.Foreground.Merge(theme.Selection.Background);

        var choice = new Choice().ApplyTheme(theme);
        var comboBox = new ComboBox().ApplyTheme(theme);
        var fuzzyFinder = new FuzzyFinder().ApplyTheme(theme);
        var fileExplorer = new FileExplorer().ApplyTheme(theme);
        var table = new Table("A").ApplyTheme(theme);
        var treeTable = new TreeTable().ApplyTheme(theme);

        TestAssert.Equal(theme.Accent.Secondary, choice.HoveredValueStyle, "Choice hovered field style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, comboBox.HoveredValueStyle, "ComboBox hovered field style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, fuzzyFinder.HoveredItemStyle, "FuzzyFinder hovered row style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, fileExplorer.HoveredStyle, "FileExplorer hovered row style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, table.HoveredRowStyle, "Table hovered row style should map to Accent.Secondary.");
        TestAssert.Equal(mergedSelection, table.SelectedRowStyle, "Table selected row style should map to merged Selection tokens.");
        TestAssert.Equal(theme.Accent.Secondary, treeTable.HoveredRowStyle, "TreeTable hovered row style should map to Accent.Secondary.");
        TestAssert.Equal(mergedSelection, treeTable.SelectedRowStyle, "TreeTable selected row style should map to merged Selection tokens.");
        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_PreservesExplicitHoverStyles()
    {
        var theme = BuildTheme();
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 99, 88));

        var choice = new Choice { HoveredValueStyle = explicitStyle };
        var comboBox = new ComboBox { HoveredValueStyle = explicitStyle };
        var fuzzyFinder = new FuzzyFinder { HoveredItemStyle = explicitStyle };
        var fileExplorer = new FileExplorer { HoveredStyle = explicitStyle };
        var table = new Table("A") { HoveredRowStyle = explicitStyle };
        var treeTable = new TreeTable { HoveredRowStyle = explicitStyle };

        choice.ApplyThemeDefaults(theme);
        comboBox.ApplyThemeDefaults(theme);
        fuzzyFinder.ApplyThemeDefaults(theme);
        fileExplorer.ApplyThemeDefaults(theme);
        table.ApplyThemeDefaults(theme);
        treeTable.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, choice.HoveredValueStyle, "Choice defaults should not overwrite explicit hovered field style.");
        TestAssert.Equal(explicitStyle, comboBox.HoveredValueStyle, "ComboBox defaults should not overwrite explicit hovered field style.");
        TestAssert.Equal(explicitStyle, fuzzyFinder.HoveredItemStyle, "FuzzyFinder defaults should not overwrite explicit hovered row style.");
        TestAssert.Equal(explicitStyle, fileExplorer.HoveredStyle, "FileExplorer defaults should not overwrite explicit hovered row style.");
        TestAssert.Equal(explicitStyle, table.HoveredRowStyle, "Table defaults should not overwrite explicit hovered row style.");
        TestAssert.Equal(explicitStyle, treeTable.HoveredRowStyle, "TreeTable defaults should not overwrite explicit hovered row style.");
        return Task.CompletedTask;
    }

    private static Task Overrides_ResolveHoverTokens_ForNavigationExplorerControls()
    {
        var baseTheme = BuildTheme();
        var typeTheme = BuildTheme();
        var overrides = new TeaThemeOverrides();
        overrides.SetControlType<Choice>(typeTheme);
        overrides.SetControlType<ComboBox>(typeTheme);
        overrides.SetControlType<FuzzyFinder>(typeTheme);
        overrides.SetControlType<FileExplorer>(typeTheme);
        overrides.SetControlType<Table>(typeTheme);
        overrides.SetControlType<TreeTable>(typeTheme);

        var choice = new Choice().ApplyTheme(overrides, baseTheme);
        var comboBox = new ComboBox().ApplyTheme(overrides, baseTheme);
        var fuzzyFinder = new FuzzyFinder().ApplyTheme(overrides, baseTheme);
        var fileExplorer = new FileExplorer().ApplyTheme(overrides, baseTheme);
        var table = new Table("A").ApplyTheme(overrides, baseTheme);
        var treeTable = new TreeTable().ApplyTheme(overrides, baseTheme);

        TestAssert.Equal(typeTheme.Accent.Secondary, choice.HoveredValueStyle, "Choice override apply should map hovered field style.");
        TestAssert.Equal(typeTheme.Accent.Secondary, comboBox.HoveredValueStyle, "ComboBox override apply should map hovered field style.");
        TestAssert.Equal(typeTheme.Accent.Secondary, fuzzyFinder.HoveredItemStyle, "FuzzyFinder override apply should map hovered row style.");
        TestAssert.Equal(typeTheme.Accent.Secondary, fileExplorer.HoveredStyle, "FileExplorer override apply should map hovered row style.");
        TestAssert.Equal(typeTheme.Accent.Secondary, table.HoveredRowStyle, "Table override apply should map hovered row style.");
        TestAssert.Equal(typeTheme.Accent.Secondary, treeTable.HoveredRowStyle, "TreeTable override apply should map hovered row style.");
        return Task.CompletedTask;
    }

    private static TeaTheme BuildTheme()
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(81, 82, 83)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
        };
    }
}
