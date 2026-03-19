using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> NavigationOverlayCases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications",
            ApplyHelpers_MapExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications",
            OverrideOverloads_ResolveExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(37, 38, 39)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(91, 92, 93)),
            },
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
            },
        };

        var choice = new Choice().ApplyTheme(theme);
        var comboBox = new ComboBox().ApplyTheme(theme);
        var treeView = new TreeView().ApplyTheme(theme);
        var menuBar = new MenuBar().ApplyTheme(theme);
        var contextMenu = new ContextMenu().ApplyTheme(theme);
        var commandPalette = new CommandPalette().ApplyTheme(theme);
        var notifications = new Notifications().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, choice.TitleStyle, "Choice title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, choice.FocusedTitleStyle, "Choice focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, choice.OptionStyle, "Choice option style should map to Text.Primary.");
        TestAssert.Equal(theme.Border.Default, choice.BorderStyleText, "Choice border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), choice.FocusedBorderStyleText, "Choice focused border style should map to focused border tokens.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            choice.SelectedOptionStyle,
            "Choice selected option style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Primary, comboBox.ValueTextStyle, "ComboBox value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, comboBox.PlaceholderTextStyle, "ComboBox placeholder style should map to Text.Muted.");
        TestAssert.Equal(theme.Accent.Secondary, comboBox.HoveredOptionStyle, "ComboBox hovered option style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Border.Default, comboBox.BorderStyleText, "ComboBox border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), comboBox.FocusedBorderStyleText, "ComboBox focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Accent.Primary, treeView.BranchStyle, "TreeView branch style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Primary, treeView.LeafStyle, "TreeView leaf style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, treeView.HoveredItemStyle, "TreeView hovered style should map to Accent.Secondary.");

        TestAssert.Equal(theme.Text.Primary, menuBar.ItemStyle, "MenuBar item style should map to Text.Primary.");
        TestAssert.Equal(theme.Focus.Ring, menuBar.FocusedItemStyle, "MenuBar focused style should map to Focus.Ring.");

        TestAssert.Equal(theme.Text.Secondary, contextMenu.TitleStyle, "ContextMenu title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, contextMenu.HoveredItemStyle, "ContextMenu hovered style should map to Accent.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, commandPalette.TitleStyle, "CommandPalette title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, commandPalette.QueryTextStyle, "CommandPalette query style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, commandPalette.PlaceholderTextStyle, "CommandPalette placeholder style should map to Text.Muted.");

        TestAssert.Equal(theme.State.Info, notifications.InfoItemStyle, "Notifications info style should map to State.Info.");
        TestAssert.Equal(theme.State.Success, notifications.SuccessItemStyle, "Notifications success style should map to State.Success.");
        TestAssert.Equal(theme.State.Warning, notifications.WarningItemStyle, "Notifications warning style should map to State.Warning.");
        TestAssert.Equal(theme.State.Error, notifications.ErrorItemStyle, "Notifications error style should map to State.Error.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
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
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(9, 10, 11)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(12, 13, 14)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(25, 26, 27)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(28, 29, 30)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
        };

        var choice = new Choice { OptionStyle = explicitStyle, BorderStyleText = explicitStyle };
        var comboBox = new ComboBox { ValueTextStyle = explicitStyle, BorderStyleText = explicitStyle };
        var treeView = new TreeView { BranchStyle = explicitStyle };
        var menuBar = new MenuBar { ItemStyle = explicitStyle };
        var contextMenu = new ContextMenu { ItemStyle = explicitStyle };
        var commandPalette = new CommandPalette { ItemStyle = explicitStyle };
        var notifications = new Notifications { ItemStyle = explicitStyle };

        choice.ApplyThemeDefaults(theme);
        comboBox.ApplyThemeDefaults(theme);
        treeView.ApplyThemeDefaults(theme);
        menuBar.ApplyThemeDefaults(theme);
        contextMenu.ApplyThemeDefaults(theme);
        commandPalette.ApplyThemeDefaults(theme);
        notifications.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, choice.OptionStyle, "Defaults should not overwrite explicit Choice.OptionStyle.");
        TestAssert.Equal(explicitStyle, choice.BorderStyleText, "Defaults should not overwrite explicit Choice.BorderStyleText.");
        TestAssert.Equal(theme.Text.Secondary, choice.TitleStyle, "Defaults should fill empty Choice.TitleStyle.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), choice.FocusedBorderStyleText, "Defaults should fill empty Choice.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, comboBox.ValueTextStyle, "Defaults should not overwrite explicit ComboBox.ValueTextStyle.");
        TestAssert.Equal(explicitStyle, comboBox.BorderStyleText, "Defaults should not overwrite explicit ComboBox.BorderStyleText.");
        TestAssert.Equal(theme.Text.Muted, comboBox.PlaceholderTextStyle, "Defaults should fill empty ComboBox.PlaceholderTextStyle.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), comboBox.FocusedBorderStyleText, "Defaults should fill empty ComboBox.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, treeView.BranchStyle, "Defaults should not overwrite explicit TreeView.BranchStyle.");
        TestAssert.Equal(theme.Text.Primary, treeView.LeafStyle, "Defaults should fill empty TreeView.LeafStyle.");

        TestAssert.Equal(explicitStyle, menuBar.ItemStyle, "Defaults should not overwrite explicit MenuBar.ItemStyle.");
        TestAssert.Equal(theme.Focus.Ring, menuBar.FocusedItemStyle, "Defaults should fill empty MenuBar.FocusedItemStyle.");

        TestAssert.Equal(explicitStyle, contextMenu.ItemStyle, "Defaults should not overwrite explicit ContextMenu.ItemStyle.");
        TestAssert.Equal(theme.Text.Secondary, contextMenu.TitleStyle, "Defaults should fill empty ContextMenu.TitleStyle.");

        TestAssert.Equal(explicitStyle, commandPalette.ItemStyle, "Defaults should not overwrite explicit CommandPalette.ItemStyle.");
        TestAssert.Equal(theme.Text.Primary, commandPalette.QueryTextStyle, "Defaults should fill empty CommandPalette.QueryTextStyle.");

        TestAssert.Equal(explicitStyle, notifications.ItemStyle, "Defaults should not overwrite explicit Notifications.ItemStyle.");
        TestAssert.Equal(theme.State.Success, notifications.SuccessItemStyle, "Defaults should fill empty Notifications.SuccessItemStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var choice = new Choice { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };
        var comboBox = new ComboBox();
        var treeView = new TreeView();
        var menuBar = new MenuBar { ItemStyle = explicitStyle };
        var contextMenu = new ContextMenu();
        var commandPalette = new CommandPalette { ItemStyle = explicitStyle };
        var notifications = new Notifications();

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
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(191, 192, 193)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(211, 212, 213)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 224)),
            },
        };

        overrides.SetControlType<Choice>(typeTheme);
        overrides.SetControlType<ComboBox>(typeTheme);
        overrides.SetControlType<TreeView>(typeTheme);
        overrides.SetControlType<MenuBar>(typeTheme);
        overrides.SetControlType<ContextMenu>(typeTheme);
        overrides.SetControlType<CommandPalette>(typeTheme);
        overrides.SetControlType<Notifications>(typeTheme);

        choice.ApplyThemeDefaults(overrides, baseTheme);
        comboBox.ApplyTheme(overrides, baseTheme);
        treeView.ApplyTheme(overrides, baseTheme);
        menuBar.ApplyThemeDefaults(overrides, baseTheme);
        contextMenu.ApplyTheme(overrides, baseTheme);
        commandPalette.ApplyThemeDefaults(overrides, baseTheme);
        notifications.ApplyTheme(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, choice.ValueStyle, "Override defaults should not overwrite explicit Choice.ValueStyle.");
        TestAssert.Equal(explicitStyle, choice.BorderStyleText, "Override defaults should not overwrite explicit Choice.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), choice.FocusedBorderStyleText, "Override defaults should map Choice focused border styles.");
        TestAssert.Equal(typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background), comboBox.SelectedOptionStyle, "Override apply should map ComboBox selected option style.");
        TestAssert.Equal(typeTheme.Border.Default, comboBox.BorderStyleText, "Override apply should map ComboBox border styles.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), comboBox.FocusedBorderStyleText, "Override apply should map ComboBox focused border styles.");
        TestAssert.Equal(typeTheme.Accent.Primary, treeView.BranchStyle, "Override apply should map TreeView branch style.");
        TestAssert.Equal(explicitStyle, menuBar.ItemStyle, "Override defaults should not overwrite explicit MenuBar.ItemStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, contextMenu.TitleStyle, "Override apply should map ContextMenu title style.");
        TestAssert.Equal(explicitStyle, commandPalette.ItemStyle, "Override defaults should not overwrite explicit CommandPalette.ItemStyle.");
        TestAssert.Equal(typeTheme.State.Warning, notifications.WarningItemStyle, "Override apply should map Notifications warning style.");

        return Task.CompletedTask;
    }
}
