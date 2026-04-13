using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> DataCases()
    {
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

    private static Task ApplyHelpers_MapExpectedTokens_ForDataGridTreeTableAndKeyValueList()
    {
        var theme = new TesseraTheme
        {
            Text =
                new TesseraThemeTextTokens
                {
                    Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                    Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                    Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33))
                },
            Accent =
                new TesseraThemeAccentTokens { Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)) },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56))
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(71, 72, 73))
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(74, 75, 76)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(77, 78, 79))
            }
        };

        var dataGrid = new DataGrid().ApplyTheme(theme);
        var treeTable = new TreeTable().ApplyTheme(theme);
        var keyValueList = new KeyValueList().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, dataGrid.TitleStyle,
            "DataGrid title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, dataGrid.FocusedTitleStyle,
            "DataGrid focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, dataGrid.HeaderStyle,
            "DataGrid header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, dataGrid.RowStyle, "DataGrid row style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            dataGrid.SelectedRowStyle,
            "DataGrid selected row style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Primary, dataGrid.SelectedCellStyle,
            "DataGrid selected cell style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, dataGrid.MutedStyle, "DataGrid muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, dataGrid.BorderStyleText,
            "DataGrid border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), dataGrid.FocusedBorderStyleText,
            "DataGrid focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, treeTable.TitleStyle,
            "TreeTable title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, treeTable.FocusedTitleStyle,
            "TreeTable focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, treeTable.HeaderStyle,
            "TreeTable header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Primary, treeTable.BranchRowStyle,
            "TreeTable branch style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Primary, treeTable.LeafRowStyle,
            "TreeTable leaf style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            treeTable.SelectedRowStyle,
            "TreeTable selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, treeTable.MutedRowStyle, "TreeTable muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, treeTable.BorderStyleText,
            "TreeTable border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), treeTable.FocusedBorderStyleText,
            "TreeTable focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, keyValueList.TitleStyle,
            "KeyValueList title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, keyValueList.FocusedTitleStyle,
            "KeyValueList focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, keyValueList.KeyStyle,
            "KeyValueList key style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, keyValueList.ValueStyle,
            "KeyValueList value style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            keyValueList.SelectedRowStyle,
            "KeyValueList selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, keyValueList.SeparatorStyle,
            "KeyValueList separator style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, keyValueList.BorderStyleText,
            "KeyValueList border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), keyValueList.FocusedBorderStyleText,
            "KeyValueList focused border style should map to focused border tokens.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDataGridTreeTableAndKeyValueList()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TesseraTheme
        {
            Text =
                new TesseraThemeTextTokens
                {
                    Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                    Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                    Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9))
                },
            Accent =
                new TesseraThemeAccentTokens { Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)) },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24))
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(19, 20, 21))
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(28, 29, 30))
            }
        };

        var dataGrid = new DataGrid { RowStyle = explicitStyle, BorderStyleText = explicitStyle };
        var treeTable = new TreeTable { LeafRowStyle = explicitStyle, BorderStyleText = explicitStyle };
        var keyValueList = new KeyValueList { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };

        dataGrid.ApplyThemeDefaults(theme);
        treeTable.ApplyThemeDefaults(theme);
        keyValueList.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, dataGrid.RowStyle, "Defaults should not overwrite explicit DataGrid.RowStyle.");
        TestAssert.Equal(explicitStyle, dataGrid.BorderStyleText,
            "Defaults should not overwrite explicit DataGrid.BorderStyleText.");
        TestAssert.Equal(theme.Accent.Primary, dataGrid.SelectedCellStyle,
            "Defaults should fill empty DataGrid.SelectedCellStyle.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), dataGrid.FocusedBorderStyleText,
            "Defaults should fill empty DataGrid.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, treeTable.LeafRowStyle,
            "Defaults should not overwrite explicit TreeTable.LeafRowStyle.");
        TestAssert.Equal(explicitStyle, treeTable.BorderStyleText,
            "Defaults should not overwrite explicit TreeTable.BorderStyleText.");
        TestAssert.Equal(theme.Accent.Primary, treeTable.BranchRowStyle,
            "Defaults should fill empty TreeTable.BranchRowStyle.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), treeTable.FocusedBorderStyleText,
            "Defaults should fill empty TreeTable.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, keyValueList.ValueStyle,
            "Defaults should not overwrite explicit KeyValueList.ValueStyle.");
        TestAssert.Equal(theme.Text.Muted, keyValueList.SeparatorStyle,
            "Defaults should fill empty KeyValueList.SeparatorStyle.");
        TestAssert.Equal(explicitStyle, keyValueList.BorderStyleText,
            "Defaults should not overwrite explicit KeyValueList.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), keyValueList.FocusedBorderStyleText,
            "Defaults should fill empty KeyValueList.FocusedBorderStyleText.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForDataGridTreeTableAndKeyValueList()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var dataGrid = new DataGrid { RowStyle = explicitStyle };
        var treeTable = new TreeTable();
        var keyValueList = new KeyValueList { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TesseraThemeOverrides();
        var typeTheme = new TesseraTheme
        {
            Text =
                new TesseraThemeTextTokens
                {
                    Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                    Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                    Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123))
                },
            Accent =
                new TesseraThemeAccentTokens
                {
                    Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133))
                },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(164, 165, 166))
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163))
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(167, 168, 169)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(170, 171, 172))
            }
        };

        overrides.SetControlType<DataGrid>(typeTheme);
        overrides.SetControlType<TreeTable>(typeTheme);
        overrides.SetControlType<KeyValueList>(typeTheme);

        dataGrid.ApplyThemeDefaults(overrides, baseTheme);
        treeTable.ApplyTheme(overrides, baseTheme);
        keyValueList.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, dataGrid.RowStyle,
            "Override defaults should not overwrite explicit DataGrid.RowStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, dataGrid.SelectedCellStyle,
            "Override defaults should fill empty DataGrid.SelectedCellStyle.");
        TestAssert.Equal(typeTheme.Border.Default, dataGrid.BorderStyleText,
            "Override defaults should fill empty DataGrid.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), dataGrid.FocusedBorderStyleText,
            "Override defaults should fill empty DataGrid.FocusedBorderStyleText.");
        TestAssert.Equal(typeTheme.Accent.Primary, treeTable.BranchRowStyle,
            "Override apply should map TreeTable branch style.");
        TestAssert.Equal(typeTheme.Text.Primary, treeTable.LeafRowStyle,
            "Override apply should map TreeTable leaf style.");
        TestAssert.Equal(typeTheme.Border.Default, treeTable.BorderStyleText,
            "Override apply should map TreeTable border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), treeTable.FocusedBorderStyleText,
            "Override apply should map TreeTable focused border style.");
        TestAssert.Equal(explicitStyle, keyValueList.ValueStyle,
            "Override defaults should not overwrite explicit KeyValueList.ValueStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, keyValueList.SeparatorStyle,
            "Override defaults should fill empty KeyValueList.SeparatorStyle.");
        TestAssert.Equal(explicitStyle, keyValueList.BorderStyleText,
            "Override defaults should not overwrite explicit KeyValueList.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), keyValueList.FocusedBorderStyleText,
            "Override defaults should fill empty KeyValueList.FocusedBorderStyleText.");

        return Task.CompletedTask;
    }
}
