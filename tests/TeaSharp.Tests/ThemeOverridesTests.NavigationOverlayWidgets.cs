using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> NavigationOverlayCases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications",
            ApplyHelpers_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications",
            ApplyThemeDefaults_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications",
            OverrideOverloads_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications);
    }

    private static Task ApplyHelpers_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var theme = new TeaTheme
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
            Focus = new TeaThemeFocusTokens
            {
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(101, 102, 103)),
            },
            State = new TeaThemeStateTokens
            {
                Info = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
        };

        var choice = new Choice { Title = "Choice X", Border = BorderStyle.Rounded };
        choice.SetItems(["one", "two"]);
        var comboBox = new ComboBox { Title = "Combo X", Placeholder = "Search..." };
        comboBox.SetItems(["alpha", "beta"]);
        var treeView = new TreeView { Title = "Tree X", Border = BorderStyle.Heavy };
        treeView.SetItems([new TreeItem("root", "Root")]);
        var menuBar = new MenuBar();
        menuBar.SetItems([new MenuItem("file", "File", 'f')]);
        var contextMenu = new ContextMenu { Title = "Context X", Border = BorderStyle.Ascii };
        contextMenu.SetItems([new ContextMenuItem("copy", "Copy")]);
        var commandPalette = new CommandPalette { Title = "Palette X", MaxVisibleItems = 11 };
        commandPalette.SetItems([new CommandPaletteItem("open", "Open", "Open file")]);
        var notifications = new Notifications { Title = "Notify X", ShowTimestamp = false };

        var choiceResult = choice.ApplyTheme(theme);
        var comboBoxResult = comboBox.ApplyTheme(theme);
        var treeViewResult = treeView.ApplyTheme(theme);
        var menuBarResult = menuBar.ApplyTheme(theme);
        var contextMenuResult = contextMenu.ApplyTheme(theme);
        var commandPaletteResult = commandPalette.ApplyTheme(theme);
        var notificationsResult = notifications.ApplyTheme(theme);

        TestAssert.ReferenceSame(choice, choiceResult, "ApplyTheme should return same Choice instance.");
        TestAssert.ReferenceSame(comboBox, comboBoxResult, "ApplyTheme should return same ComboBox instance.");
        TestAssert.ReferenceSame(treeView, treeViewResult, "ApplyTheme should return same TreeView instance.");
        TestAssert.ReferenceSame(menuBar, menuBarResult, "ApplyTheme should return same MenuBar instance.");
        TestAssert.ReferenceSame(contextMenu, contextMenuResult, "ApplyTheme should return same ContextMenu instance.");
        TestAssert.ReferenceSame(commandPalette, commandPaletteResult, "ApplyTheme should return same CommandPalette instance.");
        TestAssert.ReferenceSame(notifications, notificationsResult, "ApplyTheme should return same Notifications instance.");

        TestAssert.Equal("Choice X", choice.Title, "ApplyTheme should not alter Choice title.");
        TestAssert.Equal("Combo X", comboBox.Title, "ApplyTheme should not alter ComboBox title.");
        TestAssert.Equal("Tree X", treeView.Title, "ApplyTheme should not alter TreeView title.");
        TestAssert.Equal("Context X", contextMenu.Title, "ApplyTheme should not alter ContextMenu title.");
        TestAssert.Equal("Palette X", commandPalette.Title, "ApplyTheme should not alter CommandPalette title.");
        TestAssert.Equal("Notify X", notifications.Title, "ApplyTheme should not alter Notifications title.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
            },
        };

        var choice = new Choice { Title = "Choice Defaults" };
        var comboBox = new ComboBox { Title = "Combo Defaults", Placeholder = "keep me" };
        var treeView = new TreeView { Title = "Tree Defaults" };
        var menuBar = new MenuBar();
        menuBar.SetItems([new MenuItem("help", "Help", 'h')]);
        var contextMenu = new ContextMenu { Title = "Context Defaults" };
        var commandPalette = new CommandPalette { Title = "Palette Defaults", MaxVisibleItems = 5 };
        var notifications = new Notifications { Title = "Notify Defaults", ShowTimestamp = false };

        choice.ApplyThemeDefaults(theme);
        comboBox.ApplyThemeDefaults(theme);
        treeView.ApplyThemeDefaults(theme);
        menuBar.ApplyThemeDefaults(theme);
        contextMenu.ApplyThemeDefaults(theme);
        commandPalette.ApplyThemeDefaults(theme);
        notifications.ApplyThemeDefaults(theme);

        TestAssert.Equal("Choice Defaults", choice.Title, "ApplyThemeDefaults should not alter Choice title.");
        TestAssert.Equal("Combo Defaults", comboBox.Title, "ApplyThemeDefaults should not alter ComboBox title.");
        TestAssert.Equal("keep me", comboBox.Placeholder, "ApplyThemeDefaults should not alter ComboBox placeholder.");
        TestAssert.Equal("Tree Defaults", treeView.Title, "ApplyThemeDefaults should not alter TreeView title.");
        TestAssert.Equal("Context Defaults", contextMenu.Title, "ApplyThemeDefaults should not alter ContextMenu title.");
        TestAssert.Equal("Palette Defaults", commandPalette.Title, "ApplyThemeDefaults should not alter CommandPalette title.");
        TestAssert.Equal("Notify Defaults", notifications.Title, "ApplyThemeDefaults should not alter Notifications title.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_NoOp_ForChoiceComboBoxTreeViewMenuBarContextMenuCommandPaletteAndNotifications()
    {
        var choice = new Choice { Title = "Choice Override" };
        var comboBox = new ComboBox { Title = "Combo Override" };
        var treeView = new TreeView { Title = "Tree Override" };
        var menuBar = new MenuBar();
        var contextMenu = new ContextMenu { Title = "Context Override" };
        var commandPalette = new CommandPalette { Title = "Palette Override" };
        var notifications = new Notifications { Title = "Notify Override" };
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();

        overrides.SetControlType<Choice>(BuildThemeWithPrimary(101, 102, 103));
        overrides.SetControlType<ComboBox>(BuildThemeWithPrimary(111, 112, 113));
        overrides.SetControlType<TreeView>(BuildThemeWithPrimary(121, 122, 123));
        overrides.SetControlType<MenuBar>(BuildThemeWithPrimary(131, 132, 133));
        overrides.SetControlType<ContextMenu>(BuildThemeWithPrimary(141, 142, 143));
        overrides.SetControlType<CommandPalette>(BuildThemeWithPrimary(151, 152, 153));
        overrides.SetControlType<Notifications>(BuildThemeWithPrimary(161, 162, 163));

        var choiceResult = choice.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var comboBoxResult = comboBox.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var treeViewResult = treeView.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var menuBarResult = menuBar.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var contextMenuResult = contextMenu.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var commandPaletteResult = commandPalette.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var notificationsResult = notifications.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);

        TestAssert.ReferenceSame(choice, choiceResult, "Override apply should return same Choice instance.");
        TestAssert.ReferenceSame(comboBox, comboBoxResult, "Override defaults should return same ComboBox instance.");
        TestAssert.ReferenceSame(treeView, treeViewResult, "Override apply should return same TreeView instance.");
        TestAssert.ReferenceSame(menuBar, menuBarResult, "Override defaults should return same MenuBar instance.");
        TestAssert.ReferenceSame(contextMenu, contextMenuResult, "Override apply should return same ContextMenu instance.");
        TestAssert.ReferenceSame(commandPalette, commandPaletteResult, "Override defaults should return same CommandPalette instance.");
        TestAssert.ReferenceSame(notifications, notificationsResult, "Override apply should return same Notifications instance.");

        TestAssert.Equal("Choice Override", choice.Title, "Override apply should not alter Choice title.");
        TestAssert.Equal("Combo Override", comboBox.Title, "Override defaults should not alter ComboBox title.");
        TestAssert.Equal("Tree Override", treeView.Title, "Override apply should not alter TreeView title.");
        TestAssert.Equal("Context Override", contextMenu.Title, "Override apply should not alter ContextMenu title.");
        TestAssert.Equal("Palette Override", commandPalette.Title, "Override defaults should not alter CommandPalette title.");
        TestAssert.Equal("Notify Override", notifications.Title, "Override apply should not alter Notifications title.");

        return Task.CompletedTask;
    }
}
