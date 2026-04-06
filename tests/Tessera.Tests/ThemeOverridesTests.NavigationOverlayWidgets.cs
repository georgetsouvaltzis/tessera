using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

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
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(37, 38, 39)),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(74, 75, 76)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(91, 92, 93)),
            },
            State = new TesseraThemeStateTokens
            {
                Info = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Success = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Warning = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
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
        TestAssert.Equal(theme.Text.Muted, choice.DisabledStyle, "Choice disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, choice.MutedStyle, "Choice muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, choice.BorderStyleText, "Choice border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), choice.FocusedBorderStyleText, "Choice focused border style should map to focused border tokens.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            choice.SelectedOptionStyle,
            "Choice selected option style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Primary, comboBox.ValueTextStyle, "ComboBox value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, comboBox.PlaceholderTextStyle, "ComboBox placeholder style should map to Text.Muted.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            comboBox.SelectedOptionStyle,
            "ComboBox selected option style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Secondary, comboBox.HoveredOptionStyle, "ComboBox hovered option style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Muted, comboBox.DisabledStyle, "ComboBox disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, comboBox.MutedStyle, "ComboBox muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, comboBox.BorderStyleText, "ComboBox border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), comboBox.FocusedBorderStyleText, "ComboBox focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Accent.Primary, treeView.BranchStyle, "TreeView branch style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Primary, treeView.LeafStyle, "TreeView leaf style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            treeView.SelectedItemStyle,
            "TreeView selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Secondary, treeView.HoveredItemStyle, "TreeView hovered style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Muted, treeView.DisabledStyle, "TreeView disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, treeView.MutedStyle, "TreeView muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, treeView.BorderStyleText, "TreeView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), treeView.FocusedBorderStyleText, "TreeView focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Primary, menuBar.ItemStyle, "MenuBar item style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            menuBar.SelectedItemStyle,
            "MenuBar selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Focus.Ring, menuBar.FocusedItemStyle, "MenuBar focused style should map to Focus.Ring.");
        TestAssert.Equal(theme.Text.Muted, menuBar.DisabledItemStyle, "MenuBar disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, menuBar.BorderStyleText, "MenuBar border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), menuBar.FocusedBorderStyleText, "MenuBar focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, contextMenu.TitleStyle, "ContextMenu title style should map to Text.Secondary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            contextMenu.SelectedItemStyle,
            "ContextMenu selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Secondary, contextMenu.HoveredItemStyle, "ContextMenu hovered style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Muted, contextMenu.DisabledItemStyle, "ContextMenu disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, contextMenu.MutedItemStyle, "ContextMenu muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, contextMenu.BorderStyleText, "ContextMenu border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), contextMenu.FocusedBorderStyleText, "ContextMenu focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, commandPalette.TitleStyle, "CommandPalette title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, commandPalette.QueryTextStyle, "CommandPalette query style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, commandPalette.PlaceholderTextStyle, "CommandPalette placeholder style should map to Text.Muted.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            commandPalette.SelectedItemStyle,
            "CommandPalette selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, commandPalette.DisabledItemStyle, "CommandPalette disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, commandPalette.MutedItemStyle, "CommandPalette muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, commandPalette.BorderStyleText, "CommandPalette border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), commandPalette.FocusedBorderStyleText, "CommandPalette focused border style should map to focused border tokens.");

        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            notifications.SelectedItemStyle,
            "Notifications selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Secondary, notifications.HoveredItemStyle, "Notifications hovered style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Accent.Primary, notifications.UnreadItemStyle, "Notifications unread style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, notifications.MutedItemStyle, "Notifications muted style should map to Text.Muted.");
        TestAssert.Equal(theme.State.Info, notifications.InfoItemStyle, "Notifications info style should map to State.Info.");
        TestAssert.Equal(theme.State.Success, notifications.SuccessItemStyle, "Notifications success style should map to State.Success.");
        TestAssert.Equal(theme.State.Warning, notifications.WarningItemStyle, "Notifications warning style should map to State.Warning.");
        TestAssert.Equal(theme.State.Error, notifications.ErrorItemStyle, "Notifications error style should map to State.Error.");
        TestAssert.Equal(theme.Text.Muted, notifications.DisabledItemStyle, "Notifications disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, notifications.BorderStyleText, "Notifications border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), notifications.FocusedBorderStyleText, "Notifications focused border style should map to focused border tokens.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(9, 10, 11)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(12, 13, 14)),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(28, 29, 30)),
            },
            State = new TesseraThemeStateTokens
            {
                Success = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
            },
        };

        var choice = new Choice { OptionStyle = explicitStyle, BorderStyleText = explicitStyle };
        var comboBox = new ComboBox { ValueTextStyle = explicitStyle, BorderStyleText = explicitStyle };
        var treeView = new TreeView { BranchStyle = explicitStyle, BorderStyleText = explicitStyle };
        var menuBar = new MenuBar { ItemStyle = explicitStyle, BorderStyleText = explicitStyle };
        var contextMenu = new ContextMenu { ItemStyle = explicitStyle, BorderStyleText = explicitStyle };
        var commandPalette = new CommandPalette { ItemStyle = explicitStyle, BorderStyleText = explicitStyle };
        var notifications = new Notifications { ItemStyle = explicitStyle, BorderStyleText = explicitStyle };

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
        TestAssert.Equal(explicitStyle, treeView.BorderStyleText, "Defaults should not overwrite explicit TreeView.BorderStyleText.");
        TestAssert.Equal(theme.Text.Primary, treeView.LeafStyle, "Defaults should fill empty TreeView.LeafStyle.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), treeView.FocusedBorderStyleText, "Defaults should fill empty TreeView.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, menuBar.ItemStyle, "Defaults should not overwrite explicit MenuBar.ItemStyle.");
        TestAssert.Equal(theme.Focus.Ring, menuBar.FocusedItemStyle, "Defaults should fill empty MenuBar.FocusedItemStyle.");
        TestAssert.Equal(explicitStyle, menuBar.BorderStyleText, "Defaults should not overwrite explicit MenuBar.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), menuBar.FocusedBorderStyleText, "Defaults should fill empty MenuBar.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, contextMenu.ItemStyle, "Defaults should not overwrite explicit ContextMenu.ItemStyle.");
        TestAssert.Equal(theme.Text.Secondary, contextMenu.TitleStyle, "Defaults should fill empty ContextMenu.TitleStyle.");
        TestAssert.Equal(explicitStyle, contextMenu.BorderStyleText, "Defaults should not overwrite explicit ContextMenu.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), contextMenu.FocusedBorderStyleText, "Defaults should fill empty ContextMenu.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, commandPalette.ItemStyle, "Defaults should not overwrite explicit CommandPalette.ItemStyle.");
        TestAssert.Equal(theme.Text.Primary, commandPalette.QueryTextStyle, "Defaults should fill empty CommandPalette.QueryTextStyle.");
        TestAssert.Equal(explicitStyle, commandPalette.BorderStyleText, "Defaults should not overwrite explicit CommandPalette.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), commandPalette.FocusedBorderStyleText, "Defaults should fill empty CommandPalette.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, notifications.ItemStyle, "Defaults should not overwrite explicit Notifications.ItemStyle.");
        TestAssert.Equal(theme.State.Success, notifications.SuccessItemStyle, "Defaults should fill empty Notifications.SuccessItemStyle.");
        TestAssert.Equal(theme.Text.Muted, notifications.DisabledItemStyle, "Defaults should fill empty Notifications.DisabledItemStyle.");
        TestAssert.Equal(explicitStyle, notifications.BorderStyleText, "Defaults should not overwrite explicit Notifications.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), notifications.FocusedBorderStyleText, "Defaults should fill empty Notifications.FocusedBorderStyleText.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var choice = new Choice { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };
        var comboBox = new ComboBox();
        var treeView = new TreeView();
        var menuBar = new MenuBar { ItemStyle = explicitStyle };
        var contextMenu = new ContextMenu();
        var commandPalette = new CommandPalette { ItemStyle = explicitStyle };
        var notifications = new Notifications();

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TesseraThemeOverrides();
        var typeTheme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(124, 125, 126)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(127, 128, 129)),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(161, 162, 163)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(164, 165, 166)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(171, 172, 173)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(181, 182, 183)),
            },
            State = new TesseraThemeStateTokens
            {
                Info = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(191, 192, 193)),
                Success = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203)),
                Warning = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(211, 212, 213)),
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 224)),
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
        TestAssert.Equal(typeTheme.Border.Default, treeView.BorderStyleText, "Override apply should map TreeView border styles.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), treeView.FocusedBorderStyleText, "Override apply should map TreeView focused border styles.");
        TestAssert.Equal(explicitStyle, menuBar.ItemStyle, "Override defaults should not overwrite explicit MenuBar.ItemStyle.");
        TestAssert.Equal(typeTheme.Border.Default, menuBar.BorderStyleText, "Override defaults should map MenuBar border styles.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), menuBar.FocusedBorderStyleText, "Override defaults should map MenuBar focused border styles.");
        TestAssert.Equal(typeTheme.Text.Secondary, contextMenu.TitleStyle, "Override apply should map ContextMenu title style.");
        TestAssert.Equal(typeTheme.Border.Default, contextMenu.BorderStyleText, "Override apply should map ContextMenu border styles.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), contextMenu.FocusedBorderStyleText, "Override apply should map ContextMenu focused border styles.");
        TestAssert.Equal(explicitStyle, commandPalette.ItemStyle, "Override defaults should not overwrite explicit CommandPalette.ItemStyle.");
        TestAssert.Equal(typeTheme.Border.Default, commandPalette.BorderStyleText, "Override defaults should map CommandPalette border styles.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), commandPalette.FocusedBorderStyleText, "Override defaults should map CommandPalette focused border styles.");
        TestAssert.Equal(typeTheme.State.Warning, notifications.WarningItemStyle, "Override apply should map Notifications warning style.");
        TestAssert.Equal(typeTheme.Text.Muted, notifications.DisabledItemStyle, "Override apply should map Notifications disabled style.");
        TestAssert.Equal(typeTheme.Border.Default, notifications.BorderStyleText, "Override apply should map Notifications border styles.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), notifications.FocusedBorderStyleText, "Override apply should map Notifications focused border styles.");

        return Task.CompletedTask;
    }
}
