using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> RenderingCases()
    {
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
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForBadgeLogViewMarkdownViewAndMiniLog",
            ApplyHelpers_MapExpectedTokens_ForBadgeLogViewMarkdownViewAndMiniLog);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForBadgeLogViewMarkdownViewAndMiniLog",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForBadgeLogViewMarkdownViewAndMiniLog);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForBadgeLogViewMarkdownViewAndMiniLog",
            OverrideOverloads_ResolveExpectedTokens_ForBadgeLogViewMarkdownViewAndMiniLog);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter",
            ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter",
            OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter);
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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(57, 58, 59)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(60, 61, 62)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(63, 64, 65)),
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
        TestAssert.Equal(theme.Border.Default, diffView.BorderStyleText, "DiffView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), diffView.FocusedBorderStyleText, "DiffView focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, propertyGrid.TitleStyle, "PropertyGrid title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, propertyGrid.FocusedTitleStyle, "PropertyGrid focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, propertyGrid.HeaderStyle, "PropertyGrid header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, propertyGrid.KeyStyle, "PropertyGrid key style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, propertyGrid.ValueStyle, "PropertyGrid value style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            propertyGrid.SelectedRowStyle,
            "PropertyGrid selected row style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Default, propertyGrid.BorderStyleText, "PropertyGrid border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), propertyGrid.FocusedBorderStyleText, "PropertyGrid focused border style should map to focused border tokens.");

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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(28, 29, 30)),
            },
        };

        var diffView = new DiffView
        {
            AddedLineStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var propertyGrid = new PropertyGrid
        {
            ValueStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };

        diffView.ApplyThemeDefaults(theme);
        propertyGrid.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, diffView.AddedLineStyle, "Defaults should not overwrite explicit DiffView.AddedLineStyle.");
        TestAssert.Equal(theme.State.Error, diffView.RemovedLineStyle, "Defaults should fill empty DiffView.RemovedLineStyle.");
        TestAssert.Equal(explicitStyle, diffView.BorderStyleText, "Defaults should not overwrite explicit DiffView.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), diffView.FocusedBorderStyleText, "Defaults should fill empty DiffView.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, propertyGrid.ValueStyle, "Defaults should not overwrite explicit PropertyGrid.ValueStyle.");
        TestAssert.Equal(theme.Text.Secondary, propertyGrid.HeaderStyle, "Defaults should fill empty PropertyGrid.HeaderStyle.");
        TestAssert.Equal(explicitStyle, propertyGrid.BorderStyleText, "Defaults should not overwrite explicit PropertyGrid.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), propertyGrid.FocusedBorderStyleText, "Defaults should fill empty PropertyGrid.FocusedBorderStyleText.");

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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(124, 125, 126)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(127, 128, 129)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(130, 131, 132)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(133, 134, 135)),
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
        TestAssert.Equal(typeTheme.Border.Default, diffView.BorderStyleText, "Override defaults should fill DiffView border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), diffView.FocusedBorderStyleText, "Override defaults should fill DiffView focused border style.");
        TestAssert.Equal(typeTheme.Text.Secondary, propertyGrid.KeyStyle, "Override apply should map PropertyGrid key style.");
        TestAssert.Equal(typeTheme.Text.Primary, propertyGrid.ValueStyle, "Override apply should map PropertyGrid value style.");
        TestAssert.Equal(typeTheme.Border.Default, propertyGrid.BorderStyleText, "Override apply should map PropertyGrid border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), propertyGrid.FocusedBorderStyleText, "Override apply should map PropertyGrid focused border style.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForBadgeLogViewMarkdownViewAndMiniLog()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(57, 58, 59)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(74, 75, 76)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
            },
        };

        var badge = new Badge().ApplyTheme(theme);
        var logView = new LogView().ApplyTheme(theme);
        var markdownView = new MarkdownView().ApplyTheme(theme);
        var miniLog = new MiniLog().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Primary, badge.TextStyle, "Badge text style should map to Text.Primary.");
        TestAssert.Equal(theme.Focus.Ring, badge.FocusedTextStyle, "Badge focused style should map to Focus.Ring.");
        TestAssert.Equal(theme.State.Success, badge.SuccessTextStyle, "Badge success style should map to State.Success.");
        TestAssert.Equal(theme.State.Warning, badge.WarningTextStyle, "Badge warning style should map to State.Warning.");
        TestAssert.Equal(theme.State.Error, badge.ErrorTextStyle, "Badge error style should map to State.Error.");

        TestAssert.Equal(theme.Text.Secondary, logView.TitleStyle, "LogView title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, logView.FocusedTitleStyle, "LogView focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, logView.EntryStyle, "LogView entry style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, logView.PausedTitleStyle, "LogView paused title style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, logView.BorderStyleText, "LogView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), logView.FocusedBorderStyleText, "LogView focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, markdownView.TitleStyle, "MarkdownView title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, markdownView.FocusedTitleStyle, "MarkdownView focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, markdownView.ContentStyle, "MarkdownView content style should map to Text.Primary.");
        TestAssert.Equal(theme.Border.Default, markdownView.BorderStyleText, "MarkdownView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), markdownView.FocusedBorderStyleText, "MarkdownView focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, miniLog.TitleStyle, "MiniLog title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, miniLog.FocusedTitleStyle, "MiniLog focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, miniLog.EntryStyle, "MiniLog entry style should map to Text.Primary.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForBadgeLogViewMarkdownViewAndMiniLog()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(211, 212, 213));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(8, 9, 10)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(14, 15, 16)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
            },
        };

        var badge = new Badge
        {
            TextStyle = explicitStyle,
        };
        var logView = new LogView
        {
            EntryStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var markdownView = new MarkdownView
        {
            ContentStyle = explicitStyle,
        };
        var miniLog = new MiniLog
        {
            EntryStyle = explicitStyle,
        };

        badge.ApplyThemeDefaults(theme);
        logView.ApplyThemeDefaults(theme);
        markdownView.ApplyThemeDefaults(theme);
        miniLog.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, badge.TextStyle, "Defaults should not overwrite explicit Badge.TextStyle.");
        TestAssert.Equal(theme.State.Error, badge.ErrorTextStyle, "Defaults should fill empty Badge.ErrorTextStyle.");
        TestAssert.Equal(explicitStyle, logView.EntryStyle, "Defaults should not overwrite explicit LogView.EntryStyle.");
        TestAssert.Equal(theme.Text.Secondary, logView.TitleStyle, "Defaults should fill empty LogView.TitleStyle.");
        TestAssert.Equal(explicitStyle, logView.BorderStyleText, "Defaults should not overwrite explicit LogView.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), logView.FocusedBorderStyleText, "Defaults should fill empty LogView.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, markdownView.ContentStyle, "Defaults should not overwrite explicit MarkdownView.ContentStyle.");
        TestAssert.Equal(theme.Focus.Title, markdownView.FocusedTitleStyle, "Defaults should fill empty MarkdownView.FocusedTitleStyle.");
        TestAssert.Equal(theme.Border.Default, markdownView.BorderStyleText, "Defaults should fill empty MarkdownView.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), markdownView.FocusedBorderStyleText, "Defaults should fill empty MarkdownView.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, miniLog.EntryStyle, "Defaults should not overwrite explicit MiniLog.EntryStyle.");
        TestAssert.Equal(theme.Focus.Title, miniLog.FocusedTitleStyle, "Defaults should fill empty MiniLog.FocusedTitleStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForBadgeLogViewMarkdownViewAndMiniLog()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var badge = new Badge
        {
            TextStyle = explicitStyle,
        };
        var logView = new LogView();
        var markdownView = new MarkdownView
        {
            ContentStyle = explicitStyle,
        };
        var miniLog = new MiniLog();
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
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(124, 125, 126)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(127, 128, 129)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(144, 145, 146)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(161, 162, 163)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(171, 172, 173)),
            },
        };
        overrides.SetControlType<Badge>(typeTheme);
        overrides.SetControlType<LogView>(typeTheme);
        overrides.SetControlType<MarkdownView>(typeTheme);
        overrides.SetControlType<MiniLog>(typeTheme);

        badge.ApplyThemeDefaults(overrides, baseTheme);
        logView.ApplyTheme(overrides, baseTheme);
        markdownView.ApplyThemeDefaults(overrides, baseTheme);
        miniLog.ApplyTheme(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, badge.TextStyle, "Override defaults should not overwrite explicit Badge.TextStyle.");
        TestAssert.Equal(typeTheme.State.Warning, badge.WarningTextStyle, "Override defaults should fill empty Badge.WarningTextStyle.");
        TestAssert.Equal(typeTheme.Text.Primary, logView.EntryStyle, "Override apply should map LogView.EntryStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, logView.PausedTitleStyle, "Override apply should map LogView.PausedTitleStyle.");
        TestAssert.Equal(typeTheme.Border.Default, logView.BorderStyleText, "Override apply should map LogView.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), logView.FocusedBorderStyleText, "Override apply should map LogView focused border style.");
        TestAssert.Equal(explicitStyle, markdownView.ContentStyle, "Override defaults should not overwrite explicit MarkdownView.ContentStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, markdownView.TitleStyle, "Override defaults should fill empty MarkdownView.TitleStyle.");
        TestAssert.Equal(typeTheme.Border.Default, markdownView.BorderStyleText, "Override defaults should fill MarkdownView.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), markdownView.FocusedBorderStyleText, "Override defaults should fill MarkdownView focused border style.");
        TestAssert.Equal(typeTheme.Text.Secondary, miniLog.TitleStyle, "Override apply should map MiniLog.TitleStyle.");
        TestAssert.Equal(typeTheme.Text.Primary, miniLog.EntryStyle, "Override apply should map MiniLog.EntryStyle.");

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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(64, 65, 66)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(67, 68, 69)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(70, 71, 72)),
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
        TestAssert.Equal(theme.Border.Default, fileExplorer.BorderStyleText, "FileExplorer border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), fileExplorer.FocusedBorderStyleText, "FileExplorer focused border style should map to focused border tokens.");

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
        TestAssert.Equal(theme.Border.Default, fuzzyFinder.BorderStyleText, "FuzzyFinder border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), fuzzyFinder.FocusedBorderStyleText, "FuzzyFinder focused border style should map to focused border tokens.");

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
        TestAssert.Equal(theme.Border.Default, toastCenter.BorderStyleText, "ToastCenter border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), toastCenter.FocusedBorderStyleText, "ToastCenter focused border style should map to focused border tokens.");

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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(28, 29, 30)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(31, 32, 33)),
            },
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(37, 38, 39)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(40, 41, 42)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(43, 44, 45)),
            },
        };

        var fileExplorer = new FileExplorer
        {
            DirectoryStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var fuzzyFinder = new FuzzyFinder
        {
            MatchHighlightStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var toastCenter = new ToastCenter
        {
            WarningItemStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };

        fileExplorer.ApplyThemeDefaults(theme);
        fuzzyFinder.ApplyThemeDefaults(theme);
        toastCenter.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, fileExplorer.DirectoryStyle, "Defaults should not overwrite explicit FileExplorer.DirectoryStyle.");
        TestAssert.Equal(theme.Text.Primary, fileExplorer.FileStyle, "Defaults should fill empty FileExplorer.FileStyle.");
        TestAssert.Equal(explicitStyle, fileExplorer.BorderStyleText, "Defaults should not overwrite explicit FileExplorer.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), fileExplorer.FocusedBorderStyleText, "Defaults should fill empty FileExplorer.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, fuzzyFinder.MatchHighlightStyle, "Defaults should not overwrite explicit FuzzyFinder.MatchHighlightStyle.");
        TestAssert.Equal(theme.Text.Muted, fuzzyFinder.PlaceholderTextStyle, "Defaults should fill empty FuzzyFinder.PlaceholderTextStyle.");
        TestAssert.Equal(explicitStyle, fuzzyFinder.BorderStyleText, "Defaults should not overwrite explicit FuzzyFinder.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), fuzzyFinder.FocusedBorderStyleText, "Defaults should fill empty FuzzyFinder.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, toastCenter.WarningItemStyle, "Defaults should not overwrite explicit ToastCenter.WarningItemStyle.");
        TestAssert.Equal(theme.State.Error, toastCenter.ErrorItemStyle, "Defaults should fill empty ToastCenter.ErrorItemStyle.");
        TestAssert.Equal(explicitStyle, toastCenter.BorderStyleText, "Defaults should not overwrite explicit ToastCenter.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), toastCenter.FocusedBorderStyleText, "Defaults should fill empty ToastCenter.FocusedBorderStyleText.");

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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(154, 155, 156)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(157, 158, 159)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(160, 161, 162)),
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
        TestAssert.Equal(typeTheme.Border.Default, fileExplorer.BorderStyleText, "Override apply should map FileExplorer border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), fileExplorer.FocusedBorderStyleText, "Override apply should map FileExplorer focused border style.");
        TestAssert.Equal(explicitStyle, fuzzyFinder.ListItemStyle, "Override defaults should not overwrite explicit FuzzyFinder.ListItemStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, fuzzyFinder.MatchHighlightStyle, "Override defaults should fill empty FuzzyFinder.MatchHighlightStyle.");
        TestAssert.Equal(typeTheme.Border.Default, fuzzyFinder.BorderStyleText, "Override defaults should fill FuzzyFinder border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), fuzzyFinder.FocusedBorderStyleText, "Override defaults should fill FuzzyFinder focused border style.");
        TestAssert.Equal(typeTheme.State.Info, toastCenter.InfoItemStyle, "Override apply should map ToastCenter info style.");
        TestAssert.Equal(typeTheme.State.Warning, toastCenter.WarningItemStyle, "Override apply should map ToastCenter warning style.");
        TestAssert.Equal(typeTheme.Border.Default, toastCenter.BorderStyleText, "Override apply should map ToastCenter border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), toastCenter.FocusedBorderStyleText, "Override apply should map ToastCenter focused border style.");

        return Task.CompletedTask;
    }
}
