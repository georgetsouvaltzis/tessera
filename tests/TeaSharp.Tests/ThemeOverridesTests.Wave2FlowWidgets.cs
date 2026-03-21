using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> FlowWave2Cases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForWave2DataPlanningControls",
            ApplyHelpers_MapExpectedTokens_ForWave2DataPlanningControls);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave2DataPlanningControls",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave2DataPlanningControls);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForWave2DataPlanningControls",
            OverrideOverloads_ResolveExpectedTokens_ForWave2DataPlanningControls);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForWave2QueryPivotRichTextControls",
            ApplyHelpers_MapExpectedTokens_ForWave2QueryPivotRichTextControls);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave2QueryPivotRichTextControls",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave2QueryPivotRichTextControls);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForWave2QueryPivotRichTextControls",
            OverrideOverloads_ResolveExpectedTokens_ForWave2QueryPivotRichTextControls);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForWave2DataPlanningControls()
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
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(94, 95, 96)),
            },
        };

        var virtualizedList = new VirtualizedListView<string>().ApplyTheme(theme);
        var groupedList = new GroupedListView<string, string>().ApplyTheme(theme);
        var kanban = new KanbanBoard().ApplyTheme(theme);
        var tagInput = new TagInput().ApplyTheme(theme);
        var calendar = new CalendarMonthView().ApplyTheme(theme);
        var scheduler = new SchedulerTimeline().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Primary, virtualizedList.DefaultRowStyle, "VirtualizedListView default row style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, virtualizedList.HoveredRowStyle, "VirtualizedListView hovered row style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Border.Default, virtualizedList.BorderStyleText, "VirtualizedListView border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), virtualizedList.FocusedBorderStyleText, "VirtualizedListView focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, groupedList.GroupHeaderStyle, "GroupedListView group header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, groupedList.ItemStyle, "GroupedListView item style should map to Text.Primary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), groupedList.SelectedRowStyle, "GroupedListView selected row style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Default, groupedList.BorderStyleText, "GroupedListView border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Secondary, kanban.LaneHeaderStyle, "KanbanBoard lane header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), kanban.SelectedCardStyle, "KanbanBoard selected card style should map to merged Selection styles.");
        TestAssert.Equal(theme.Focus.Ring, kanban.FocusedCardStyle, "KanbanBoard focused card style should map to Focus.Ring.");
        TestAssert.Equal(theme.State.Error, kanban.ErrorCardStyle, "KanbanBoard error card style should map to State.Error.");

        TestAssert.Equal(theme.Text.Secondary, tagInput.TagStyle, "TagInput tag style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, tagInput.ValueTextStyle, "TagInput value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, tagInput.PlaceholderTextStyle, "TagInput placeholder style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), tagInput.FocusedBorderStyleText, "TagInput focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, calendar.MonthHeaderStyle, "CalendarMonthView month header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Primary, calendar.TodayDayStyle, "CalendarMonthView today style should map to Accent.Primary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), calendar.SelectedDayStyle, "CalendarMonthView selected day style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, calendar.DisabledStyle, "CalendarMonthView disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, scheduler.TimeTextStyle, "SchedulerTimeline time style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, scheduler.EntryTextStyle, "SchedulerTimeline entry style should map to Text.Primary.");
        TestAssert.Equal(theme.State.Warning, scheduler.ConflictRowStyle, "SchedulerTimeline conflict style should map to State.Warning.");
        TestAssert.Equal(theme.Text.Muted, scheduler.EmptyTextStyle, "SchedulerTimeline empty style should map to Text.Muted.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave2DataPlanningControls()
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
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(38, 39, 40)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
        };

        var virtualizedList = new VirtualizedListView<string> { DefaultRowStyle = explicitStyle, BorderStyleText = explicitStyle };
        var groupedList = new GroupedListView<string, string> { ItemStyle = explicitStyle, BorderStyleText = explicitStyle };
        var kanban = new KanbanBoard { CardStyle = explicitStyle, BorderStyleText = explicitStyle };
        var tagInput = new TagInput { TagStyle = explicitStyle, BorderStyleText = explicitStyle };
        var calendar = new CalendarMonthView { DayStyle = explicitStyle };
        var scheduler = new SchedulerTimeline { EntryTextStyle = explicitStyle };

        virtualizedList.ApplyThemeDefaults(theme);
        groupedList.ApplyThemeDefaults(theme);
        kanban.ApplyThemeDefaults(theme);
        tagInput.ApplyThemeDefaults(theme);
        calendar.ApplyThemeDefaults(theme);
        scheduler.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, virtualizedList.DefaultRowStyle, "Defaults should not overwrite explicit VirtualizedListView.DefaultRowStyle.");
        TestAssert.Equal(theme.Accent.Secondary, virtualizedList.HoveredRowStyle, "Defaults should fill empty VirtualizedListView.HoveredRowStyle.");
        TestAssert.Equal(explicitStyle, virtualizedList.BorderStyleText, "Defaults should not overwrite explicit VirtualizedListView.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), virtualizedList.FocusedBorderStyleText, "Defaults should fill empty VirtualizedListView.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, groupedList.ItemStyle, "Defaults should not overwrite explicit GroupedListView.ItemStyle.");
        TestAssert.Equal(theme.Text.Secondary, groupedList.GroupHeaderStyle, "Defaults should fill empty GroupedListView.GroupHeaderStyle.");
        TestAssert.Equal(explicitStyle, groupedList.BorderStyleText, "Defaults should not overwrite explicit GroupedListView.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), groupedList.FocusedBorderStyleText, "Defaults should fill empty GroupedListView.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, kanban.CardStyle, "Defaults should not overwrite explicit KanbanBoard.CardStyle.");
        TestAssert.Equal(theme.Focus.Ring, kanban.FocusedCardStyle, "Defaults should fill empty KanbanBoard.FocusedCardStyle.");
        TestAssert.Equal(explicitStyle, kanban.BorderStyleText, "Defaults should not overwrite explicit KanbanBoard.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), kanban.FocusedBorderStyleText, "Defaults should fill empty KanbanBoard.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, tagInput.TagStyle, "Defaults should not overwrite explicit TagInput.TagStyle.");
        TestAssert.Equal(theme.Text.Primary, tagInput.ValueTextStyle, "Defaults should fill empty TagInput.ValueTextStyle.");
        TestAssert.Equal(explicitStyle, tagInput.BorderStyleText, "Defaults should not overwrite explicit TagInput.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), tagInput.FocusedBorderStyleText, "Defaults should fill empty TagInput.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, calendar.DayStyle, "Defaults should not overwrite explicit CalendarMonthView.DayStyle.");
        TestAssert.Equal(theme.Text.Secondary, calendar.WeekdayHeaderStyle, "Defaults should fill empty CalendarMonthView.WeekdayHeaderStyle.");
        TestAssert.Equal(theme.Accent.Primary, calendar.TodayDayStyle, "Defaults should fill empty CalendarMonthView.TodayDayStyle.");

        TestAssert.Equal(explicitStyle, scheduler.EntryTextStyle, "Defaults should not overwrite explicit SchedulerTimeline.EntryTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, scheduler.TimeTextStyle, "Defaults should fill empty SchedulerTimeline.TimeTextStyle.");
        TestAssert.Equal(theme.State.Warning, scheduler.ConflictRowStyle, "Defaults should fill empty SchedulerTimeline.ConflictRowStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForWave2DataPlanningControls()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var virtualizedList = new VirtualizedListView<string> { DefaultRowStyle = explicitStyle };
        var groupedList = new GroupedListView<string, string> { ItemStyle = explicitStyle };
        var kanban = new KanbanBoard { CardStyle = explicitStyle };
        var tagInput = new TagInput { TagStyle = explicitStyle };
        var calendar = new CalendarMonthView { DayStyle = explicitStyle };
        var scheduler = new SchedulerTimeline { EntryTextStyle = explicitStyle };

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
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(144, 145, 146)),
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
                Warning = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(181, 182, 183)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(184, 185, 186)),
            },
        };

        overrides.SetControlType<VirtualizedListView<string>>(typeTheme);
        overrides.SetControlType<GroupedListView<string, string>>(typeTheme);
        overrides.SetControlType<KanbanBoard>(typeTheme);
        overrides.SetControlType<TagInput>(typeTheme);
        overrides.SetControlType<CalendarMonthView>(typeTheme);
        overrides.SetControlType<SchedulerTimeline>(typeTheme);

        virtualizedList.ApplyThemeDefaults(overrides, baseTheme);
        groupedList.ApplyThemeDefaults(overrides, baseTheme);
        kanban.ApplyThemeDefaults(overrides, baseTheme);
        tagInput.ApplyThemeDefaults(overrides, baseTheme);
        calendar.ApplyThemeDefaults(overrides, baseTheme);
        scheduler.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, virtualizedList.DefaultRowStyle, "Override defaults should not overwrite explicit VirtualizedListView.DefaultRowStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, virtualizedList.HoveredRowStyle, "Override defaults should fill empty VirtualizedListView.HoveredRowStyle.");
        TestAssert.Equal(typeTheme.Border.Default, virtualizedList.BorderStyleText, "Override defaults should fill empty VirtualizedListView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, groupedList.ItemStyle, "Override defaults should not overwrite explicit GroupedListView.ItemStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, groupedList.GroupHeaderStyle, "Override defaults should fill empty GroupedListView.GroupHeaderStyle.");
        TestAssert.Equal(typeTheme.Border.Default, groupedList.BorderStyleText, "Override defaults should fill empty GroupedListView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, kanban.CardStyle, "Override defaults should not overwrite explicit KanbanBoard.CardStyle.");
        TestAssert.Equal(typeTheme.Focus.Ring, kanban.FocusedCardStyle, "Override defaults should fill empty KanbanBoard.FocusedCardStyle.");
        TestAssert.Equal(typeTheme.State.Error, kanban.ErrorCardStyle, "Override defaults should fill empty KanbanBoard.ErrorCardStyle.");

        TestAssert.Equal(explicitStyle, tagInput.TagStyle, "Override defaults should not overwrite explicit TagInput.TagStyle.");
        TestAssert.Equal(typeTheme.Text.Primary, tagInput.ValueTextStyle, "Override defaults should fill empty TagInput.ValueTextStyle.");
        TestAssert.Equal(typeTheme.Border.Default, tagInput.BorderStyleText, "Override defaults should fill empty TagInput.BorderStyleText.");

        TestAssert.Equal(explicitStyle, calendar.DayStyle, "Override defaults should not overwrite explicit CalendarMonthView.DayStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, calendar.MonthHeaderStyle, "Override defaults should fill empty CalendarMonthView.MonthHeaderStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, calendar.TodayDayStyle, "Override defaults should fill empty CalendarMonthView.TodayDayStyle.");

        TestAssert.Equal(explicitStyle, scheduler.EntryTextStyle, "Override defaults should not overwrite explicit SchedulerTimeline.EntryTextStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, scheduler.TimeTextStyle, "Override defaults should fill empty SchedulerTimeline.TimeTextStyle.");
        TestAssert.Equal(typeTheme.State.Warning, scheduler.ConflictRowStyle, "Override defaults should fill empty SchedulerTimeline.ConflictRowStyle.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForWave2QueryPivotRichTextControls()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Muted = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Surface = new TeaThemeSurfaceTokens
            {
                Panel = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(35, 36, 37)),
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
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
            },
        };

        var pivotTable = new PivotTable().ApplyTheme(theme);
        var queryBuilder = new QueryBuilder().ApplyTheme(theme);
        var richTextView = new RichTextView().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, pivotTable.TitleStyle, "PivotTable title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, pivotTable.HeaderStyle, "PivotTable header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, pivotTable.BodyStyle, "PivotTable body style should map to Text.Primary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), pivotTable.SelectedCellStyle, "PivotTable selected cell style should map to merged Selection styles.");
        TestAssert.Equal(theme.Focus.Ring, pivotTable.FocusedCellStyle, "PivotTable focused cell style should map to Focus.Ring.");
        TestAssert.Equal(theme.Border.Default, pivotTable.BorderStyleText, "PivotTable border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), pivotTable.FocusedBorderStyleText, "PivotTable focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, queryBuilder.TitleStyle, "QueryBuilder title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, queryBuilder.RuleStyle, "QueryBuilder rule style should map to Text.Primary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), queryBuilder.SelectedRuleStyle, "QueryBuilder selected rule style should map to merged Selection styles.");
        TestAssert.Equal(theme.Focus.Ring, queryBuilder.FocusedRuleStyle, "QueryBuilder focused rule style should map to Focus.Ring.");
        TestAssert.Equal(theme.Accent.Secondary, queryBuilder.HoveredRuleStyle, "QueryBuilder hovered rule style should map to Accent.Secondary.");
        TestAssert.Equal(theme.State.Error, queryBuilder.ErrorRuleStyle, "QueryBuilder error rule style should map to State.Error.");
        TestAssert.Equal(theme.Border.Default, queryBuilder.BorderStyleText, "QueryBuilder border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Secondary, richTextView.TitleStyle, "RichTextView title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, richTextView.TextStyle, "RichTextView text style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Primary, richTextView.HeadingStyle, "RichTextView heading style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Secondary, richTextView.ListMarkerStyle, "RichTextView list marker style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, richTextView.EmphasisStyle, "RichTextView emphasis style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Surface.Panel.Merge(theme.Text.Primary), richTextView.InlineCodeStyle, "RichTextView inline code style should merge Surface.Panel and Text.Primary.");
        TestAssert.Equal(theme.Border.Default, richTextView.BorderStyleText, "RichTextView border style should map to Border.Default.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave2QueryPivotRichTextControls()
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
            Surface = new TeaThemeSurfaceTokens
            {
                Panel = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(10, 11, 12)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(28, 29, 30)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(31, 32, 33)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(34, 35, 36)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(37, 38, 39)),
            },
            State = new TeaThemeStateTokens
            {
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(40, 41, 42)),
            },
        };

        var pivotTable = new PivotTable { BodyStyle = explicitStyle, BorderStyleText = explicitStyle };
        var queryBuilder = new QueryBuilder { RuleStyle = explicitStyle, BorderStyleText = explicitStyle };
        var richTextView = new RichTextView { TextStyle = explicitStyle, BorderStyleText = explicitStyle };

        pivotTable.ApplyThemeDefaults(theme);
        queryBuilder.ApplyThemeDefaults(theme);
        richTextView.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, pivotTable.BodyStyle, "Defaults should not overwrite explicit PivotTable.BodyStyle.");
        TestAssert.Equal(theme.Text.Secondary, pivotTable.HeaderStyle, "Defaults should fill empty PivotTable.HeaderStyle.");
        TestAssert.Equal(explicitStyle, pivotTable.BorderStyleText, "Defaults should not overwrite explicit PivotTable.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), pivotTable.FocusedBorderStyleText, "Defaults should fill empty PivotTable.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, queryBuilder.RuleStyle, "Defaults should not overwrite explicit QueryBuilder.RuleStyle.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), queryBuilder.SelectedRuleStyle, "Defaults should fill empty QueryBuilder.SelectedRuleStyle.");
        TestAssert.Equal(theme.State.Error, queryBuilder.ErrorRuleStyle, "Defaults should fill empty QueryBuilder.ErrorRuleStyle.");
        TestAssert.Equal(explicitStyle, queryBuilder.BorderStyleText, "Defaults should not overwrite explicit QueryBuilder.BorderStyleText.");

        TestAssert.Equal(explicitStyle, richTextView.TextStyle, "Defaults should not overwrite explicit RichTextView.TextStyle.");
        TestAssert.Equal(theme.Accent.Primary, richTextView.HeadingStyle, "Defaults should fill empty RichTextView.HeadingStyle.");
        TestAssert.Equal(theme.Surface.Panel.Merge(theme.Text.Primary), richTextView.InlineCodeStyle, "Defaults should fill empty RichTextView.InlineCodeStyle.");
        TestAssert.Equal(explicitStyle, richTextView.BorderStyleText, "Defaults should not overwrite explicit RichTextView.BorderStyleText.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForWave2QueryPivotRichTextControls()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var pivotTable = new PivotTable { BodyStyle = explicitStyle };
        var queryBuilder = new QueryBuilder { RuleStyle = explicitStyle };
        var richTextView = new RichTextView { TextStyle = explicitStyle };

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
            Surface = new TeaThemeSurfaceTokens
            {
                Panel = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(125, 126, 127)),
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
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(181, 182, 183)),
            },
        };

        overrides.SetControlType<PivotTable>(typeTheme);
        overrides.SetControlType<QueryBuilder>(typeTheme);
        overrides.SetControlType<RichTextView>(typeTheme);

        pivotTable.ApplyThemeDefaults(overrides, baseTheme);
        queryBuilder.ApplyThemeDefaults(overrides, baseTheme);
        richTextView.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, pivotTable.BodyStyle, "Override defaults should not overwrite explicit PivotTable.BodyStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, pivotTable.HeaderStyle, "Override defaults should fill empty PivotTable.HeaderStyle.");
        TestAssert.Equal(typeTheme.Border.Default, pivotTable.BorderStyleText, "Override defaults should fill empty PivotTable.BorderStyleText.");

        TestAssert.Equal(explicitStyle, queryBuilder.RuleStyle, "Override defaults should not overwrite explicit QueryBuilder.RuleStyle.");
        TestAssert.Equal(typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background), queryBuilder.SelectedRuleStyle, "Override defaults should fill empty QueryBuilder.SelectedRuleStyle.");
        TestAssert.Equal(typeTheme.State.Error, queryBuilder.ErrorRuleStyle, "Override defaults should fill empty QueryBuilder.ErrorRuleStyle.");

        TestAssert.Equal(explicitStyle, richTextView.TextStyle, "Override defaults should not overwrite explicit RichTextView.TextStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, richTextView.HeadingStyle, "Override defaults should fill empty RichTextView.HeadingStyle.");
        TestAssert.Equal(typeTheme.Surface.Panel.Merge(typeTheme.Text.Primary), richTextView.InlineCodeStyle, "Override defaults should fill empty RichTextView.InlineCodeStyle.");
        TestAssert.Equal(typeTheme.Border.Default, richTextView.BorderStyleText, "Override defaults should fill empty RichTextView.BorderStyleText.");

        return Task.CompletedTask;
    }
}
