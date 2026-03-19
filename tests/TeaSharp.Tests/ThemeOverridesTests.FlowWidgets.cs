using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> FlowCases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForTimelineAndStepper",
            ApplyHelpers_MapExpectedTokens_ForTimelineAndStepper);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForTimelineAndStepper",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForTimelineAndStepper);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForTimelineAndStepper",
            OverrideOverloads_ResolveExpectedTokens_ForTimelineAndStepper);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForDialogModalAndCharts",
            ApplyHelpers_MapExpectedTokens_ForDialogModalAndCharts);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDialogModalAndCharts",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDialogModalAndCharts);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForDialogModalAndCharts",
            OverrideOverloads_ResolveExpectedTokens_ForDialogModalAndCharts);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForTimelineAndStepper()
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
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(71, 72, 73)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(81, 82, 83)),
            },
        };

        var timeline = new Timeline().ApplyTheme(theme);
        var stepper = new Stepper().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, timeline.TitleStyle, "Timeline title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, timeline.FocusedTitleStyle, "Timeline focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, timeline.TimestampStyle, "Timeline timestamp style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, timeline.ContentStyle, "Timeline content style should map to Text.Primary.");
        TestAssert.Equal(
            theme.Selection.Foreground.Merge(theme.Selection.Background),
            timeline.SelectedRowStyle,
            "Timeline selected row style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, timeline.MutedStyle, "Timeline muted style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, timeline.SeparatorStyle, "Timeline separator style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, stepper.TitleStyle, "Stepper title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, stepper.FocusedTitleStyle, "Stepper focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, stepper.StepTextStyle, "Stepper step style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Primary, stepper.ActiveStepStyle, "Stepper active step style should map to Accent.Primary.");
        TestAssert.Equal(theme.State.Success, stepper.CompletedStepStyle, "Stepper completed step style should map to State.Success.");
        TestAssert.Equal(theme.Text.Secondary, stepper.PendingStepStyle, "Stepper pending step style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Muted, stepper.ConnectorStyle, "Stepper connector style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, stepper.DisabledStepStyle, "Stepper disabled step style should map to Text.Muted.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForTimelineAndStepper()
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
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(19, 20, 21)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(22, 23, 24)),
            },
        };

        var timeline = new Timeline
        {
            ContentStyle = explicitStyle,
        };
        var stepper = new Stepper
        {
            ActiveStepStyle = explicitStyle,
        };

        timeline.ApplyThemeDefaults(theme);
        stepper.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, timeline.ContentStyle, "Defaults should not overwrite explicit Timeline.ContentStyle.");
        TestAssert.Equal(theme.Text.Secondary, timeline.TimestampStyle, "Defaults should fill empty Timeline.TimestampStyle.");
        TestAssert.Equal(explicitStyle, stepper.ActiveStepStyle, "Defaults should not overwrite explicit Stepper.ActiveStepStyle.");
        TestAssert.Equal(theme.State.Success, stepper.CompletedStepStyle, "Defaults should fill empty Stepper.CompletedStepStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForTimelineAndStepper()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var timeline = new Timeline();
        var stepper = new Stepper
        {
            StepTextStyle = explicitStyle,
        };

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
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(171, 172, 173)),
            },
        };

        overrides.SetControlType<Timeline>(typeTheme);
        overrides.SetControlType<Stepper>(typeTheme);

        timeline.ApplyTheme(overrides, baseTheme);
        stepper.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(typeTheme.Text.Secondary, timeline.TimestampStyle, "Override apply should map Timeline timestamp style.");
        TestAssert.Equal(typeTheme.Text.Primary, timeline.ContentStyle, "Override apply should map Timeline content style.");
        TestAssert.Equal(
            typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background),
            timeline.SelectedRowStyle,
            "Override apply should map Timeline selected row style.");
        TestAssert.Equal(explicitStyle, stepper.StepTextStyle, "Override defaults should not overwrite explicit Stepper.StepTextStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, stepper.ActiveStepStyle, "Override defaults should fill empty Stepper.ActiveStepStyle.");
        TestAssert.Equal(typeTheme.State.Success, stepper.CompletedStepStyle, "Override defaults should fill empty Stepper.CompletedStepStyle.");

        return Task.CompletedTask;
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForDialogModalAndCharts()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
        };

        var dialog = new Dialog().ApplyTheme(theme);
        var modal = new Modal().ApplyTheme(theme);
        var barChart = new BarChart().ApplyTheme(theme);
        var lineChart = new LineChart().ApplyTheme(theme);
        var gauge = new Gauge().ApplyTheme(theme);
        var statsCard = new StatsCard().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, dialog.TitleStyle, "Dialog title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, dialog.FocusedTitleStyle, "Dialog focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, dialog.BodyTextStyle, "Dialog body style should map to Text.Primary.");

        TestAssert.Equal(theme.Text.Secondary, modal.TitleStyle, "Modal title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, modal.FocusedTitleStyle, "Modal focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, modal.BodyTextStyle, "Modal body style should map to Text.Primary.");

        TestAssert.Equal(theme.Text.Secondary, barChart.TitleStyle, "BarChart title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, barChart.FocusedTitleStyle, "BarChart focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, barChart.LabelStyle, "BarChart label style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Secondary, barChart.LegendStyle, "BarChart legend style should map to Text.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, lineChart.TitleStyle, "LineChart title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, lineChart.FocusedTitleStyle, "LineChart focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, lineChart.StatsStyle, "LineChart stats style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, lineChart.MetaTextStyle, "LineChart meta text style should map to Accent.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, gauge.TitleStyle, "Gauge title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, gauge.FocusedTitleStyle, "Gauge focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, gauge.ValueLabelStyle, "Gauge value label style should map to Text.Primary.");

        TestAssert.Equal(theme.Text.Secondary, statsCard.TitleStyle, "StatsCard title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, statsCard.FocusedTitleStyle, "StatsCard focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, statsCard.KeyStyle, "StatsCard key style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, statsCard.ValueStyle, "StatsCard value style should map to Text.Primary.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDialogModalAndCharts()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            },
        };

        var dialog = new Dialog
        {
            BodyTextStyle = explicitStyle,
        };
        var modal = new Modal
        {
            TitleStyle = explicitStyle,
        };
        var barChart = new BarChart
        {
            LabelStyle = explicitStyle,
        };
        var lineChart = new LineChart
        {
            MetaTextStyle = explicitStyle,
        };
        var gauge = new Gauge
        {
            ValueLabelStyle = explicitStyle,
        };
        var statsCard = new StatsCard
        {
            KeyStyle = explicitStyle,
        };

        dialog.ApplyThemeDefaults(theme);
        modal.ApplyThemeDefaults(theme);
        barChart.ApplyThemeDefaults(theme);
        lineChart.ApplyThemeDefaults(theme);
        gauge.ApplyThemeDefaults(theme);
        statsCard.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, dialog.BodyTextStyle, "Defaults should not overwrite explicit Dialog.BodyTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, dialog.TitleStyle, "Defaults should fill empty Dialog.TitleStyle.");
        TestAssert.Equal(explicitStyle, modal.TitleStyle, "Defaults should not overwrite explicit Modal.TitleStyle.");
        TestAssert.Equal(theme.Text.Primary, modal.BodyTextStyle, "Defaults should fill empty Modal.BodyTextStyle.");
        TestAssert.Equal(explicitStyle, barChart.LabelStyle, "Defaults should not overwrite explicit BarChart.LabelStyle.");
        TestAssert.Equal(theme.Text.Secondary, barChart.LegendStyle, "Defaults should fill empty BarChart.LegendStyle.");
        TestAssert.Equal(explicitStyle, lineChart.MetaTextStyle, "Defaults should not overwrite explicit LineChart.MetaTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, lineChart.StatsStyle, "Defaults should fill empty LineChart.StatsStyle.");
        TestAssert.Equal(explicitStyle, gauge.ValueLabelStyle, "Defaults should not overwrite explicit Gauge.ValueLabelStyle.");
        TestAssert.Equal(theme.Text.Secondary, gauge.TitleStyle, "Defaults should fill empty Gauge.TitleStyle.");
        TestAssert.Equal(explicitStyle, statsCard.KeyStyle, "Defaults should not overwrite explicit StatsCard.KeyStyle.");
        TestAssert.Equal(theme.Text.Primary, statsCard.ValueStyle, "Defaults should fill empty StatsCard.ValueStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForDialogModalAndCharts()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var dialog = new Dialog();
        var modal = new Modal
        {
            BodyTextStyle = explicitStyle,
        };
        var barChart = new BarChart();
        var lineChart = new LineChart
        {
            StatsStyle = explicitStyle,
        };
        var gauge = new Gauge();
        var statsCard = new StatsCard
        {
            ValueStyle = explicitStyle,
        };

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
            },
            Accent = new TeaThemeAccentTokens
            {
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
            },
        };

        overrides.SetControlType<Dialog>(typeTheme);
        overrides.SetControlType<Modal>(typeTheme);
        overrides.SetControlType<BarChart>(typeTheme);
        overrides.SetControlType<LineChart>(typeTheme);
        overrides.SetControlType<Gauge>(typeTheme);
        overrides.SetControlType<StatsCard>(typeTheme);

        dialog.ApplyTheme(overrides, baseTheme);
        modal.ApplyThemeDefaults(overrides, baseTheme);
        barChart.ApplyTheme(overrides, baseTheme);
        lineChart.ApplyThemeDefaults(overrides, baseTheme);
        gauge.ApplyTheme(overrides, baseTheme);
        statsCard.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(typeTheme.Text.Secondary, dialog.TitleStyle, "Override apply should map Dialog title style.");
        TestAssert.Equal(typeTheme.Text.Primary, dialog.BodyTextStyle, "Override apply should map Dialog body style.");
        TestAssert.Equal(typeTheme.Focus.Title, modal.FocusedTitleStyle, "Override defaults should fill empty Modal.FocusedTitleStyle.");
        TestAssert.Equal(explicitStyle, modal.BodyTextStyle, "Override defaults should not overwrite explicit Modal.BodyTextStyle.");
        TestAssert.Equal(typeTheme.Text.Primary, barChart.LabelStyle, "Override apply should map BarChart label style.");
        TestAssert.Equal(typeTheme.Text.Secondary, barChart.LegendStyle, "Override apply should map BarChart legend style.");
        TestAssert.Equal(explicitStyle, lineChart.StatsStyle, "Override defaults should not overwrite explicit LineChart.StatsStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, lineChart.MetaTextStyle, "Override defaults should fill empty LineChart.MetaTextStyle.");
        TestAssert.Equal(typeTheme.Text.Primary, gauge.ValueLabelStyle, "Override apply should map Gauge value label style.");
        TestAssert.Equal(explicitStyle, statsCard.ValueStyle, "Override defaults should not overwrite explicit StatsCard.ValueStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, statsCard.KeyStyle, "Override defaults should fill empty StatsCard.KeyStyle.");

        return Task.CompletedTask;
    }
}
