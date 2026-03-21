using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> FlowWave4Cases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForWave4WorkspaceControls",
            ApplyHelpers_MapExpectedTokens_ForWave4WorkspaceControls);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave4WorkspaceControls",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave4WorkspaceControls);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForWave4WorkspaceControls",
            OverrideOverloads_ResolveExpectedTokens_ForWave4WorkspaceControls);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForWave4WorkspaceControls()
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
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(44, 45, 46)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(57, 58, 59)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(71, 72, 73)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(84, 85, 86)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(94, 95, 96)),
            },
        };

        var workspace = new DockWorkspace().ApplyTheme(theme);
        var tabs = new PaneTabs().ApplyTheme(theme);
        var heatmap = new Heatmap().ApplyTheme(theme);
        var processList = new ProcessListView().ApplyTheme(theme);
        var treeMap = new TreeMapChart().ApplyTheme(theme);
        var paletteEditor = new PaletteEditor().ApplyTheme(theme);
        var terminalPanel = new TerminalPanel().ApplyTheme(theme);

        TestAssert.Equal(theme.Border.Default, workspace.BorderStyleText, "DockWorkspace border style should map to Border.Default.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), workspace.SelectedPaneBodyStyle, "DockWorkspace selected pane body style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Primary, tabs.TabStyle, "PaneTabs tab style should map to Text.Primary.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), tabs.FocusedBorderStyleText, "PaneTabs focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.State.Success, heatmap.PeakCellStyle, "Heatmap peak cell style should map to State.Success.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), heatmap.SelectedCellStyle, "Heatmap selected cell style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Secondary, processList.HeaderStyle, "ProcessListView header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Border.Default, processList.BorderStyleText, "ProcessListView border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Primary, treeMap.NodeStyle, "TreeMapChart node style should map to Text.Primary.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), treeMap.FocusedBorderStyleText, "TreeMapChart focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Primary, paletteEditor.SwatchStyle, "PaletteEditor swatch style should map to Text.Primary.");
        TestAssert.Equal(theme.Focus.Ring, paletteEditor.FocusedSelectedSwatchStyle, "PaletteEditor focused selected style should map to Focus.Ring.");

        TestAssert.Equal(theme.State.Error, terminalPanel.StandardErrorStyle, "TerminalPanel stderr style should map to State.Error.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), terminalPanel.SelectedLineStyle, "TerminalPanel selected style should map to merged Selection styles.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave4WorkspaceControls()
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
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19)),
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(23, 24, 25)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(26, 27, 28)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(29, 30, 31)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(32, 33, 34)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(35, 36, 37)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(38, 39, 40)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
        };

        var workspace = new DockWorkspace { PaneBodyStyle = explicitStyle, BorderStyleText = explicitStyle };
        var tabs = new PaneTabs { TabStyle = explicitStyle, BorderStyleText = explicitStyle };
        var heatmap = new Heatmap { CellStyle = explicitStyle, BorderStyleText = explicitStyle };
        var processList = new ProcessListView { RowStyle = explicitStyle, BorderStyleText = explicitStyle };
        var treeMap = new TreeMapChart { NodeStyle = explicitStyle, BorderStyleText = explicitStyle };
        var paletteEditor = new PaletteEditor { SwatchStyle = explicitStyle };
        var terminalPanel = new TerminalPanel { StandardOutputStyle = explicitStyle };

        workspace.ApplyThemeDefaults(theme);
        tabs.ApplyThemeDefaults(theme);
        heatmap.ApplyThemeDefaults(theme);
        processList.ApplyThemeDefaults(theme);
        treeMap.ApplyThemeDefaults(theme);
        paletteEditor.ApplyThemeDefaults(theme);
        terminalPanel.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, workspace.PaneBodyStyle, "Defaults should not overwrite explicit DockWorkspace.PaneBodyStyle.");
        TestAssert.Equal(theme.Text.Secondary, workspace.PaneTitleStyle, "Defaults should fill empty DockWorkspace.PaneTitleStyle.");
        TestAssert.Equal(explicitStyle, workspace.BorderStyleText, "Defaults should not overwrite explicit DockWorkspace.BorderStyleText.");

        TestAssert.Equal(explicitStyle, tabs.TabStyle, "Defaults should not overwrite explicit PaneTabs.TabStyle.");
        TestAssert.Equal(theme.Accent.Secondary, tabs.HoveredTabStyle, "Defaults should fill empty PaneTabs.HoveredTabStyle.");
        TestAssert.Equal(explicitStyle, tabs.BorderStyleText, "Defaults should not overwrite explicit PaneTabs.BorderStyleText.");

        TestAssert.Equal(explicitStyle, heatmap.CellStyle, "Defaults should not overwrite explicit Heatmap.CellStyle.");
        TestAssert.Equal(theme.State.Success, heatmap.PeakCellStyle, "Defaults should fill empty Heatmap.PeakCellStyle.");
        TestAssert.Equal(explicitStyle, heatmap.BorderStyleText, "Defaults should not overwrite explicit Heatmap.BorderStyleText.");

        TestAssert.Equal(explicitStyle, processList.RowStyle, "Defaults should not overwrite explicit ProcessListView.RowStyle.");
        TestAssert.Equal(theme.Text.Secondary, processList.HeaderStyle, "Defaults should fill empty ProcessListView.HeaderStyle.");
        TestAssert.Equal(explicitStyle, processList.BorderStyleText, "Defaults should not overwrite explicit ProcessListView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, treeMap.NodeStyle, "Defaults should not overwrite explicit TreeMapChart.NodeStyle.");
        TestAssert.Equal(theme.Accent.Primary, treeMap.HighNodeStyle, "Defaults should fill empty TreeMapChart.HighNodeStyle.");
        TestAssert.Equal(explicitStyle, treeMap.BorderStyleText, "Defaults should not overwrite explicit TreeMapChart.BorderStyleText.");

        TestAssert.Equal(explicitStyle, paletteEditor.SwatchStyle, "Defaults should not overwrite explicit PaletteEditor.SwatchStyle.");
        TestAssert.Equal(theme.Accent.Primary, paletteEditor.PreviewSwatchStyle, "Defaults should fill empty PaletteEditor.PreviewSwatchStyle.");

        TestAssert.Equal(explicitStyle, terminalPanel.StandardOutputStyle, "Defaults should not overwrite explicit TerminalPanel.StandardOutputStyle.");
        TestAssert.Equal(theme.State.Error, terminalPanel.StandardErrorStyle, "Defaults should fill empty TerminalPanel.StandardErrorStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForWave4WorkspaceControls()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var workspace = new DockWorkspace { PaneBodyStyle = explicitStyle };
        var tabs = new PaneTabs { TabStyle = explicitStyle };
        var heatmap = new Heatmap { CellStyle = explicitStyle };
        var processList = new ProcessListView { RowStyle = explicitStyle };
        var treeMap = new TreeMapChart { NodeStyle = explicitStyle };
        var paletteEditor = new PaletteEditor { SwatchStyle = explicitStyle };
        var terminalPanel = new TerminalPanel { StandardOutputStyle = explicitStyle };

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
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(134, 135, 136)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(144, 145, 146)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(147, 148, 149)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(171, 172, 173)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(174, 175, 176)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(181, 182, 183)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(184, 185, 186)),
            },
        };

        overrides.SetControlType<DockWorkspace>(typeTheme);
        overrides.SetControlType<PaneTabs>(typeTheme);
        overrides.SetControlType<Heatmap>(typeTheme);
        overrides.SetControlType<ProcessListView>(typeTheme);
        overrides.SetControlType<TreeMapChart>(typeTheme);
        overrides.SetControlType<PaletteEditor>(typeTheme);
        overrides.SetControlType<TerminalPanel>(typeTheme);

        workspace.ApplyThemeDefaults(overrides, baseTheme);
        tabs.ApplyThemeDefaults(overrides, baseTheme);
        heatmap.ApplyThemeDefaults(overrides, baseTheme);
        processList.ApplyThemeDefaults(overrides, baseTheme);
        treeMap.ApplyThemeDefaults(overrides, baseTheme);
        paletteEditor.ApplyThemeDefaults(overrides, baseTheme);
        terminalPanel.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, workspace.PaneBodyStyle, "Override defaults should not overwrite explicit DockWorkspace.PaneBodyStyle.");
        TestAssert.Equal(typeTheme.Border.Default, workspace.BorderStyleText, "Override defaults should fill empty DockWorkspace.BorderStyleText.");

        TestAssert.Equal(explicitStyle, tabs.TabStyle, "Override defaults should not overwrite explicit PaneTabs.TabStyle.");
        TestAssert.Equal(typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background), tabs.SelectedTabStyle, "Override defaults should fill empty PaneTabs.SelectedTabStyle.");

        TestAssert.Equal(explicitStyle, heatmap.CellStyle, "Override defaults should not overwrite explicit Heatmap.CellStyle.");
        TestAssert.Equal(typeTheme.State.Success, heatmap.PeakCellStyle, "Override defaults should fill empty Heatmap.PeakCellStyle.");

        TestAssert.Equal(explicitStyle, processList.RowStyle, "Override defaults should not overwrite explicit ProcessListView.RowStyle.");
        TestAssert.Equal(typeTheme.Border.Default, processList.BorderStyleText, "Override defaults should fill empty ProcessListView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, treeMap.NodeStyle, "Override defaults should not overwrite explicit TreeMapChart.NodeStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, treeMap.HighNodeStyle, "Override defaults should fill empty TreeMapChart.HighNodeStyle.");

        TestAssert.Equal(explicitStyle, paletteEditor.SwatchStyle, "Override defaults should not overwrite explicit PaletteEditor.SwatchStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, paletteEditor.PreviewSwatchStyle, "Override defaults should fill empty PaletteEditor.PreviewSwatchStyle.");

        TestAssert.Equal(explicitStyle, terminalPanel.StandardOutputStyle, "Override defaults should not overwrite explicit TerminalPanel.StandardOutputStyle.");
        TestAssert.Equal(typeTheme.State.Error, terminalPanel.StandardErrorStyle, "Override defaults should fill empty TerminalPanel.StandardErrorStyle.");

        return Task.CompletedTask;
    }
}
