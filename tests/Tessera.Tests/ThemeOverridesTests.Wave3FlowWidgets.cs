using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> FlowWave3Cases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForWave3DevOpsControls",
            ApplyHelpers_MapExpectedTokens_ForWave3DevOpsControls);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave3DevOpsControls",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave3DevOpsControls);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForWave3DevOpsControls",
            OverrideOverloads_ResolveExpectedTokens_ForWave3DevOpsControls);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForWave3DevOpsControls()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 45, 46)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(57, 58, 59)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(71, 72, 73)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(84, 85, 86)),
            },
            State = new TesseraThemeStateTokens
            {
                Info = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Success = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(94, 95, 96)),
                Warning = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(97, 98, 99)),
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
            },
        };

        var json = new JsonTreeView().ApplyTheme(theme);
        var commandOutput = new CommandOutput().ApplyTheme(theme);
        var logTail = new LogTailPanel().ApplyTheme(theme);
        var activity = new ActivityFeed().ApplyTheme(theme);
        var inbox = new NotificationInbox().ApplyTheme(theme);
        var help = new KeyBindingHelpDialog().ApplyTheme(theme);
        var traceViewer = new TraceViewer().ApplyTheme(theme);
        var taskRunner = new TaskRunnerPanel().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, json.TitleStyle, "JsonTreeView title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, json.FocusedTitleStyle, "JsonTreeView focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), json.SelectedRowStyle, "JsonTreeView selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Default, json.BorderStyleText, "JsonTreeView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), json.FocusedBorderStyleText, "JsonTreeView focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Primary, commandOutput.StdOutStyle, "CommandOutput stdout style should map to Text.Primary.");
        TestAssert.Equal(theme.State.Error, commandOutput.StdErrStyle, "CommandOutput stderr style should map to State.Error.");
        TestAssert.Equal(theme.Accent.Secondary, commandOutput.SystemStyle, "CommandOutput system style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Border.Default, commandOutput.BorderStyleText, "CommandOutput border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Primary, logTail.EntryStyle, "LogTailPanel entry style should map to Text.Primary.");
        TestAssert.Equal(theme.State.Warning, logTail.WarningEntryStyle, "LogTailPanel warning style should map to State.Warning.");
        TestAssert.Equal(theme.State.Error, logTail.ErrorEntryStyle, "LogTailPanel error style should map to State.Error.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), logTail.FocusedBorderStyleText, "LogTailPanel focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.State.Info, activity.InfoItemStyle, "ActivityFeed info style should map to State.Info.");
        TestAssert.Equal(theme.State.Success, activity.SuccessItemStyle, "ActivityFeed success style should map to State.Success.");
        TestAssert.Equal(theme.Accent.Primary, activity.UnreadItemStyle, "ActivityFeed unread style should map to Accent.Primary.");
        TestAssert.Equal(theme.Border.Default, activity.BorderStyleText, "ActivityFeed border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Secondary, inbox.TitleStyle, "NotificationInbox title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), inbox.SelectedItemStyle, "NotificationInbox selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.State.Warning, inbox.WarningItemStyle, "NotificationInbox warning style should map to State.Warning.");
        TestAssert.Equal(theme.Accent.Primary, inbox.PinnedItemStyle, "NotificationInbox pinned style should map to Accent.Primary.");

        TestAssert.Equal(theme.Text.Secondary, help.GroupStyle, "KeyBindingHelpDialog group style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Primary, help.KeysStyle, "KeyBindingHelpDialog keys style should map to Accent.Primary.");
        TestAssert.Equal(theme.State.Info, help.GlobalBindingStyle, "KeyBindingHelpDialog global binding style should map to State.Info.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), help.SelectedRowStyle, "KeyBindingHelpDialog selected style should map to merged Selection styles.");

        TestAssert.Equal(theme.Text.Primary, traceViewer.EntryStyle, "TraceViewer entry style should map to Text.Primary.");
        TestAssert.Equal(theme.State.Warning, traceViewer.WarningRowStyle, "TraceViewer warning style should map to State.Warning.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), traceViewer.SelectedRowStyle, "TraceViewer selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Default, traceViewer.BorderStyleText, "TraceViewer border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Primary, taskRunner.RowStyle, "TaskRunnerPanel row style should map to Text.Primary.");
        TestAssert.Equal(theme.State.Success, taskRunner.SucceededStatusStyle, "TaskRunnerPanel succeeded status style should map to State.Success.");
        TestAssert.Equal(theme.State.Error, taskRunner.FailedStatusStyle, "TaskRunnerPanel failed status style should map to State.Error.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), taskRunner.FocusedBorderStyleText, "TaskRunnerPanel focused border style should map to focused border tokens.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave3DevOpsControls()
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
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(14, 15, 16)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19)),
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(23, 24, 25)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(26, 27, 28)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(29, 30, 31)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(32, 33, 34)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(35, 36, 37)),
            },
            State = new TesseraThemeStateTokens
            {
                Info = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(38, 39, 40)),
                Success = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Warning = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 45, 46)),
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(47, 48, 49)),
            },
        };

        var json = new JsonTreeView { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };
        var commandOutput = new CommandOutput { StdOutStyle = explicitStyle, BorderStyleText = explicitStyle };
        var logTail = new LogTailPanel { EntryStyle = explicitStyle, BorderStyleText = explicitStyle };
        var activity = new ActivityFeed { SuccessItemStyle = explicitStyle, BorderStyleText = explicitStyle };
        var inbox = new NotificationInbox { ItemStyle = explicitStyle };
        var help = new KeyBindingHelpDialog { KeysStyle = explicitStyle };
        var traceViewer = new TraceViewer { EntryStyle = explicitStyle, BorderStyleText = explicitStyle };
        var taskRunner = new TaskRunnerPanel { RowStyle = explicitStyle, BorderStyleText = explicitStyle };

        json.ApplyThemeDefaults(theme);
        commandOutput.ApplyThemeDefaults(theme);
        logTail.ApplyThemeDefaults(theme);
        activity.ApplyThemeDefaults(theme);
        inbox.ApplyThemeDefaults(theme);
        help.ApplyThemeDefaults(theme);
        traceViewer.ApplyThemeDefaults(theme);
        taskRunner.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, json.ValueStyle, "Defaults should not overwrite explicit JsonTreeView.ValueStyle.");
        TestAssert.Equal(theme.Text.Secondary, json.ContainerStyle, "Defaults should fill empty JsonTreeView.ContainerStyle.");
        TestAssert.Equal(explicitStyle, json.BorderStyleText, "Defaults should not overwrite explicit JsonTreeView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, commandOutput.StdOutStyle, "Defaults should not overwrite explicit CommandOutput.StdOutStyle.");
        TestAssert.Equal(theme.State.Error, commandOutput.StdErrStyle, "Defaults should fill empty CommandOutput.StdErrStyle.");
        TestAssert.Equal(explicitStyle, commandOutput.BorderStyleText, "Defaults should not overwrite explicit CommandOutput.BorderStyleText.");

        TestAssert.Equal(explicitStyle, logTail.EntryStyle, "Defaults should not overwrite explicit LogTailPanel.EntryStyle.");
        TestAssert.Equal(theme.State.Warning, logTail.WarningEntryStyle, "Defaults should fill empty LogTailPanel.WarningEntryStyle.");
        TestAssert.Equal(explicitStyle, logTail.BorderStyleText, "Defaults should not overwrite explicit LogTailPanel.BorderStyleText.");

        TestAssert.Equal(explicitStyle, activity.SuccessItemStyle, "Defaults should not overwrite explicit ActivityFeed.SuccessItemStyle.");
        TestAssert.Equal(theme.State.Info, activity.InfoItemStyle, "Defaults should fill empty ActivityFeed.InfoItemStyle.");
        TestAssert.Equal(explicitStyle, activity.BorderStyleText, "Defaults should not overwrite explicit ActivityFeed.BorderStyleText.");

        TestAssert.Equal(explicitStyle, inbox.ItemStyle, "Defaults should not overwrite explicit NotificationInbox.ItemStyle.");
        TestAssert.Equal(theme.State.Warning, inbox.WarningItemStyle, "Defaults should fill empty NotificationInbox.WarningItemStyle.");
        TestAssert.Equal(theme.Text.Muted, inbox.EmptyTextStyle, "Defaults should fill empty NotificationInbox.EmptyTextStyle.");

        TestAssert.Equal(explicitStyle, help.KeysStyle, "Defaults should not overwrite explicit KeyBindingHelpDialog.KeysStyle.");
        TestAssert.Equal(theme.State.Info, help.GlobalBindingStyle, "Defaults should fill empty KeyBindingHelpDialog.GlobalBindingStyle.");
        TestAssert.Equal(theme.Text.Muted, help.EmptyTextStyle, "Defaults should fill empty KeyBindingHelpDialog.EmptyTextStyle.");

        TestAssert.Equal(explicitStyle, traceViewer.EntryStyle, "Defaults should not overwrite explicit TraceViewer.EntryStyle.");
        TestAssert.Equal(theme.State.Warning, traceViewer.WarningRowStyle, "Defaults should fill empty TraceViewer.WarningRowStyle.");
        TestAssert.Equal(explicitStyle, traceViewer.BorderStyleText, "Defaults should not overwrite explicit TraceViewer.BorderStyleText.");

        TestAssert.Equal(explicitStyle, taskRunner.RowStyle, "Defaults should not overwrite explicit TaskRunnerPanel.RowStyle.");
        TestAssert.Equal(theme.State.Success, taskRunner.SucceededStatusStyle, "Defaults should fill empty TaskRunnerPanel.SucceededStatusStyle.");
        TestAssert.Equal(explicitStyle, taskRunner.BorderStyleText, "Defaults should not overwrite explicit TaskRunnerPanel.BorderStyleText.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForWave3DevOpsControls()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var json = new JsonTreeView { ValueStyle = explicitStyle };
        var commandOutput = new CommandOutput { StdOutStyle = explicitStyle };
        var logTail = new LogTailPanel { EntryStyle = explicitStyle };
        var activity = new ActivityFeed { SuccessItemStyle = explicitStyle };
        var inbox = new NotificationInbox { ItemStyle = explicitStyle };
        var help = new KeyBindingHelpDialog { KeysStyle = explicitStyle };
        var traceViewer = new TraceViewer { EntryStyle = explicitStyle };
        var taskRunner = new TaskRunnerPanel { RowStyle = explicitStyle };

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
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(134, 135, 136)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(144, 145, 146)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(147, 148, 149)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(171, 172, 173)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(174, 175, 176)),
            },
            State = new TesseraThemeStateTokens
            {
                Info = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(181, 182, 183)),
                Success = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(184, 185, 186)),
                Warning = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(187, 188, 189)),
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(190, 191, 192)),
            },
        };

        overrides.SetControlType<JsonTreeView>(typeTheme);
        overrides.SetControlType<CommandOutput>(typeTheme);
        overrides.SetControlType<LogTailPanel>(typeTheme);
        overrides.SetControlType<ActivityFeed>(typeTheme);
        overrides.SetControlType<NotificationInbox>(typeTheme);
        overrides.SetControlType<KeyBindingHelpDialog>(typeTheme);
        overrides.SetControlType<TraceViewer>(typeTheme);
        overrides.SetControlType<TaskRunnerPanel>(typeTheme);

        json.ApplyThemeDefaults(overrides, baseTheme);
        commandOutput.ApplyThemeDefaults(overrides, baseTheme);
        logTail.ApplyThemeDefaults(overrides, baseTheme);
        activity.ApplyThemeDefaults(overrides, baseTheme);
        inbox.ApplyThemeDefaults(overrides, baseTheme);
        help.ApplyThemeDefaults(overrides, baseTheme);
        traceViewer.ApplyThemeDefaults(overrides, baseTheme);
        taskRunner.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, json.ValueStyle, "Override defaults should not overwrite explicit JsonTreeView.ValueStyle.");
        TestAssert.Equal(typeTheme.Border.Default, json.BorderStyleText, "Override defaults should fill empty JsonTreeView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, commandOutput.StdOutStyle, "Override defaults should not overwrite explicit CommandOutput.StdOutStyle.");
        TestAssert.Equal(typeTheme.State.Error, commandOutput.StdErrStyle, "Override defaults should fill empty CommandOutput.StdErrStyle.");

        TestAssert.Equal(explicitStyle, logTail.EntryStyle, "Override defaults should not overwrite explicit LogTailPanel.EntryStyle.");
        TestAssert.Equal(typeTheme.State.Warning, logTail.WarningEntryStyle, "Override defaults should fill empty LogTailPanel.WarningEntryStyle.");

        TestAssert.Equal(explicitStyle, activity.SuccessItemStyle, "Override defaults should not overwrite explicit ActivityFeed.SuccessItemStyle.");
        TestAssert.Equal(typeTheme.State.Info, activity.InfoItemStyle, "Override defaults should fill empty ActivityFeed.InfoItemStyle.");

        TestAssert.Equal(explicitStyle, inbox.ItemStyle, "Override defaults should not overwrite explicit NotificationInbox.ItemStyle.");
        TestAssert.Equal(typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background), inbox.SelectedItemStyle, "Override defaults should fill empty NotificationInbox.SelectedItemStyle.");

        TestAssert.Equal(explicitStyle, help.KeysStyle, "Override defaults should not overwrite explicit KeyBindingHelpDialog.KeysStyle.");
        TestAssert.Equal(typeTheme.State.Info, help.GlobalBindingStyle, "Override defaults should fill empty KeyBindingHelpDialog.GlobalBindingStyle.");

        TestAssert.Equal(explicitStyle, traceViewer.EntryStyle, "Override defaults should not overwrite explicit TraceViewer.EntryStyle.");
        TestAssert.Equal(typeTheme.State.Warning, traceViewer.WarningRowStyle, "Override defaults should fill empty TraceViewer.WarningRowStyle.");
        TestAssert.Equal(typeTheme.Border.Default, traceViewer.BorderStyleText, "Override defaults should fill empty TraceViewer.BorderStyleText.");

        TestAssert.Equal(explicitStyle, taskRunner.RowStyle, "Override defaults should not overwrite explicit TaskRunnerPanel.RowStyle.");
        TestAssert.Equal(typeTheme.State.Success, taskRunner.SucceededStatusStyle, "Override defaults should fill empty TaskRunnerPanel.SucceededStatusStyle.");
        TestAssert.Equal(typeTheme.Border.Default, taskRunner.BorderStyleText, "Override defaults should fill empty TaskRunnerPanel.BorderStyleText.");

        return Task.CompletedTask;
    }
}
