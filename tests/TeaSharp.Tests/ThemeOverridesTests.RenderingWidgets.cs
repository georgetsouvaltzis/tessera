using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
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
}
