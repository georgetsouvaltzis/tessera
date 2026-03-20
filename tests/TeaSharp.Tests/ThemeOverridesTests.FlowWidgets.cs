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

        foreach (var testCase in FlowWave2Cases())
        {
            yield return testCase;
        }
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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(54, 55, 56)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(71, 72, 73)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(74, 75, 76)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(77, 78, 79)),
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
        TestAssert.Equal(theme.Border.Default, timeline.BorderStyleText, "Timeline border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), timeline.FocusedBorderStyleText, "Timeline focused border style should map to focused border tokens.");

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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(22, 23, 24)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(25, 26, 27)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(28, 29, 30)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
        };

        var timeline = new Timeline
        {
            ContentStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var stepper = new Stepper
        {
            ActiveStepStyle = explicitStyle,
        };

        timeline.ApplyThemeDefaults(theme);
        stepper.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, timeline.ContentStyle, "Defaults should not overwrite explicit Timeline.ContentStyle.");
        TestAssert.Equal(theme.Text.Secondary, timeline.TimestampStyle, "Defaults should fill empty Timeline.TimestampStyle.");
        TestAssert.Equal(explicitStyle, timeline.BorderStyleText, "Defaults should not overwrite explicit Timeline.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), timeline.FocusedBorderStyleText, "Defaults should fill empty Timeline.FocusedBorderStyleText.");
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
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(144, 145, 146)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(161, 162, 163)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(164, 165, 166)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(167, 168, 169)),
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
        TestAssert.Equal(typeTheme.Border.Default, timeline.BorderStyleText, "Override apply should map Timeline border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), timeline.FocusedBorderStyleText, "Override apply should map Timeline focused border style.");
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
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(30, 31, 32)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(44, 45, 46)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(47, 48, 49)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(50, 51, 52)),
            },
        };

        var dialog = new Dialog().ApplyTheme(theme);
        var modal = new Modal().ApplyTheme(theme);
        var barChart = new BarChart().ApplyTheme(theme);
        var lineChart = new LineChart().ApplyTheme(theme);
        var sparkline = new Sparkline().ApplyTheme(theme);
        var areaPlot = new AreaPlot().ApplyTheme(theme);
        var scatterPlot = new ScatterPlot().ApplyTheme(theme);
        var histogram = new Histogram().ApplyTheme(theme);
        var linePlot = new LinePlot().ApplyTheme(theme);
        var plotPanel = new PlotPanel().ApplyTheme(theme);
        var gauge = new Gauge().ApplyTheme(theme);
        var statsCard = new StatsCard().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, dialog.TitleStyle, "Dialog title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, dialog.FocusedTitleStyle, "Dialog focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, dialog.BodyTextStyle, "Dialog body style should map to Text.Primary.");
        TestAssert.Equal(theme.Border.Default, dialog.BorderStyleText, "Dialog border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), dialog.FocusedBorderStyleText, "Dialog focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, modal.TitleStyle, "Modal title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, modal.FocusedTitleStyle, "Modal focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, modal.BodyTextStyle, "Modal body style should map to Text.Primary.");
        TestAssert.Equal(theme.Border.Default, modal.BorderStyleText, "Modal border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), modal.FocusedBorderStyleText, "Modal focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, barChart.TitleStyle, "BarChart title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, barChart.FocusedTitleStyle, "BarChart focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, barChart.LabelStyle, "BarChart label style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Secondary, barChart.LegendStyle, "BarChart legend style should map to Text.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, lineChart.TitleStyle, "LineChart title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, lineChart.FocusedTitleStyle, "LineChart focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, lineChart.StatsStyle, "LineChart stats style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, lineChart.MetaTextStyle, "LineChart meta text style should map to Accent.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, sparkline.TitleStyle, "Sparkline title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, sparkline.FocusedTitleStyle, "Sparkline focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Accent.Primary, sparkline.DataStyle, "Sparkline data style should map to Accent.Primary.");
        TestAssert.Equal(theme.Border.Default, sparkline.BorderStyleText, "Sparkline border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), sparkline.FocusedBorderStyleText, "Sparkline focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, areaPlot.TitleStyle, "AreaPlot title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, areaPlot.FocusedTitleStyle, "AreaPlot focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Accent.Primary, areaPlot.FillStyle, "AreaPlot fill style should map to Accent.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, areaPlot.LineStyle, "AreaPlot line style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Border.Default, areaPlot.BorderStyleText, "AreaPlot border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), areaPlot.FocusedBorderStyleText, "AreaPlot focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, scatterPlot.TitleStyle, "ScatterPlot title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, scatterPlot.FocusedTitleStyle, "ScatterPlot focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Accent.Primary, scatterPlot.PointStyle, "ScatterPlot point style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, scatterPlot.AxisStyle, "ScatterPlot axis style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Secondary, scatterPlot.LegendStyle, "ScatterPlot legend style should map to Text.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, histogram.TitleStyle, "Histogram title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, histogram.FocusedTitleStyle, "Histogram focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Accent.Primary, histogram.BarStyle, "Histogram bar style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, histogram.AxisStyle, "Histogram axis style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Secondary, histogram.LegendStyle, "Histogram legend style should map to Text.Secondary.");

        TestAssert.Equal(theme.Text.Secondary, linePlot.TitleStyle, "LinePlot title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, linePlot.FocusedTitleStyle, "LinePlot focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, linePlot.StatsStyle, "LinePlot stats style should map to Text.Secondary.");
        TestAssert.Equal(theme.Accent.Secondary, linePlot.LegendStyle, "LinePlot legend style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, plotPanel.TitleStyle, "PlotPanel title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, plotPanel.FocusedTitleStyle, "PlotPanel focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Border.Default, plotPanel.BorderStyleText, "PlotPanel border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), plotPanel.FocusedBorderStyleText, "PlotPanel focused border style should map to focused border tokens.");

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
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(6, 7, 8)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(16, 17, 18)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(19, 20, 21)),
            },
        };

        var dialog = new Dialog
        {
            BodyTextStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var modal = new Modal
        {
            TitleStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var barChart = new BarChart
        {
            LabelStyle = explicitStyle,
        };
        var lineChart = new LineChart
        {
            MetaTextStyle = explicitStyle,
        };
        var sparkline = new Sparkline
        {
            DataStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var areaPlot = new AreaPlot
        {
            FillStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var scatterPlot = new ScatterPlot
        {
            PointStyle = explicitStyle,
        };
        var histogram = new Histogram
        {
            BarStyle = explicitStyle,
        };
        var linePlot = new LinePlot
        {
            LegendStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };
        var plotPanel = new PlotPanel
        {
            TitleStyle = explicitStyle,
            BorderStyleText = explicitStyle,
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
        sparkline.ApplyThemeDefaults(theme);
        areaPlot.ApplyThemeDefaults(theme);
        scatterPlot.ApplyThemeDefaults(theme);
        histogram.ApplyThemeDefaults(theme);
        linePlot.ApplyThemeDefaults(theme);
        plotPanel.ApplyThemeDefaults(theme);
        gauge.ApplyThemeDefaults(theme);
        statsCard.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, dialog.BodyTextStyle, "Defaults should not overwrite explicit Dialog.BodyTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, dialog.TitleStyle, "Defaults should fill empty Dialog.TitleStyle.");
        TestAssert.Equal(explicitStyle, dialog.BorderStyleText, "Defaults should not overwrite explicit Dialog.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), dialog.FocusedBorderStyleText, "Defaults should fill empty Dialog.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, modal.TitleStyle, "Defaults should not overwrite explicit Modal.TitleStyle.");
        TestAssert.Equal(theme.Text.Primary, modal.BodyTextStyle, "Defaults should fill empty Modal.BodyTextStyle.");
        TestAssert.Equal(explicitStyle, modal.BorderStyleText, "Defaults should not overwrite explicit Modal.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), modal.FocusedBorderStyleText, "Defaults should fill empty Modal.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, barChart.LabelStyle, "Defaults should not overwrite explicit BarChart.LabelStyle.");
        TestAssert.Equal(theme.Text.Secondary, barChart.LegendStyle, "Defaults should fill empty BarChart.LegendStyle.");
        TestAssert.Equal(explicitStyle, lineChart.MetaTextStyle, "Defaults should not overwrite explicit LineChart.MetaTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, lineChart.StatsStyle, "Defaults should fill empty LineChart.StatsStyle.");
        TestAssert.Equal(explicitStyle, sparkline.DataStyle, "Defaults should not overwrite explicit Sparkline.DataStyle.");
        TestAssert.Equal(theme.Text.Secondary, sparkline.MetaStyle, "Defaults should fill empty Sparkline.MetaStyle.");
        TestAssert.Equal(explicitStyle, sparkline.BorderStyleText, "Defaults should not overwrite explicit Sparkline.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), sparkline.FocusedBorderStyleText, "Defaults should fill empty Sparkline.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, areaPlot.FillStyle, "Defaults should not overwrite explicit AreaPlot.FillStyle.");
        TestAssert.Equal(theme.Accent.Secondary, areaPlot.LineStyle, "Defaults should fill empty AreaPlot.LineStyle.");
        TestAssert.Equal(explicitStyle, areaPlot.BorderStyleText, "Defaults should not overwrite explicit AreaPlot.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), areaPlot.FocusedBorderStyleText, "Defaults should fill empty AreaPlot.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, scatterPlot.PointStyle, "Defaults should not overwrite explicit ScatterPlot.PointStyle.");
        TestAssert.Equal(theme.Text.Muted, scatterPlot.AxisStyle, "Defaults should fill empty ScatterPlot.AxisStyle.");
        TestAssert.Equal(explicitStyle, histogram.BarStyle, "Defaults should not overwrite explicit Histogram.BarStyle.");
        TestAssert.Equal(theme.Text.Muted, histogram.AxisStyle, "Defaults should fill empty Histogram.AxisStyle.");
        TestAssert.Equal(explicitStyle, linePlot.LegendStyle, "Defaults should not overwrite explicit LinePlot.LegendStyle.");
        TestAssert.Equal(theme.Text.Muted, linePlot.AxisStyle, "Defaults should fill empty LinePlot.AxisStyle.");
        TestAssert.Equal(explicitStyle, linePlot.BorderStyleText, "Defaults should not overwrite explicit LinePlot.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), linePlot.FocusedBorderStyleText, "Defaults should fill empty LinePlot.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, plotPanel.TitleStyle, "Defaults should not overwrite explicit PlotPanel.TitleStyle.");
        TestAssert.Equal(theme.Text.Muted, plotPanel.EmptyTextStyle, "Defaults should fill empty PlotPanel.EmptyTextStyle.");
        TestAssert.Equal(explicitStyle, plotPanel.BorderStyleText, "Defaults should not overwrite explicit PlotPanel.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), plotPanel.FocusedBorderStyleText, "Defaults should fill empty PlotPanel.FocusedBorderStyleText.");
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
        var sparkline = new Sparkline
        {
            DataStyle = explicitStyle,
        };
        var areaPlot = new AreaPlot
        {
            FillStyle = explicitStyle,
        };
        var scatterPlot = new ScatterPlot
        {
            PointStyle = explicitStyle,
        };
        var histogram = new Histogram
        {
            BarStyle = explicitStyle,
        };
        var linePlot = new LinePlot
        {
            StatsStyle = explicitStyle,
        };
        var plotPanel = new PlotPanel
        {
            EmptyTextStyle = explicitStyle,
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
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(119, 120, 121)),
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(134, 135, 136)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 138, 139)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(140, 141, 142)),
            },
        };

        overrides.SetControlType<Dialog>(typeTheme);
        overrides.SetControlType<Modal>(typeTheme);
        overrides.SetControlType<BarChart>(typeTheme);
        overrides.SetControlType<LineChart>(typeTheme);
        overrides.SetControlType<Sparkline>(typeTheme);
        overrides.SetControlType<AreaPlot>(typeTheme);
        overrides.SetControlType<ScatterPlot>(typeTheme);
        overrides.SetControlType<Histogram>(typeTheme);
        overrides.SetControlType<LinePlot>(typeTheme);
        overrides.SetControlType<PlotPanel>(typeTheme);
        overrides.SetControlType<Gauge>(typeTheme);
        overrides.SetControlType<StatsCard>(typeTheme);

        dialog.ApplyTheme(overrides, baseTheme);
        modal.ApplyThemeDefaults(overrides, baseTheme);
        barChart.ApplyTheme(overrides, baseTheme);
        lineChart.ApplyThemeDefaults(overrides, baseTheme);
        sparkline.ApplyThemeDefaults(overrides, baseTheme);
        areaPlot.ApplyThemeDefaults(overrides, baseTheme);
        scatterPlot.ApplyThemeDefaults(overrides, baseTheme);
        histogram.ApplyThemeDefaults(overrides, baseTheme);
        linePlot.ApplyThemeDefaults(overrides, baseTheme);
        plotPanel.ApplyThemeDefaults(overrides, baseTheme);
        gauge.ApplyTheme(overrides, baseTheme);
        statsCard.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(typeTheme.Text.Secondary, dialog.TitleStyle, "Override apply should map Dialog title style.");
        TestAssert.Equal(typeTheme.Text.Primary, dialog.BodyTextStyle, "Override apply should map Dialog body style.");
        TestAssert.Equal(typeTheme.Border.Default, dialog.BorderStyleText, "Override apply should map Dialog border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), dialog.FocusedBorderStyleText, "Override apply should map Dialog focused border style.");
        TestAssert.Equal(typeTheme.Focus.Title, modal.FocusedTitleStyle, "Override defaults should fill empty Modal.FocusedTitleStyle.");
        TestAssert.Equal(explicitStyle, modal.BodyTextStyle, "Override defaults should not overwrite explicit Modal.BodyTextStyle.");
        TestAssert.Equal(typeTheme.Border.Default, modal.BorderStyleText, "Override defaults should fill empty Modal.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), modal.FocusedBorderStyleText, "Override defaults should fill empty Modal.FocusedBorderStyleText.");
        TestAssert.Equal(typeTheme.Text.Primary, barChart.LabelStyle, "Override apply should map BarChart label style.");
        TestAssert.Equal(typeTheme.Text.Secondary, barChart.LegendStyle, "Override apply should map BarChart legend style.");
        TestAssert.Equal(explicitStyle, lineChart.StatsStyle, "Override defaults should not overwrite explicit LineChart.StatsStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, lineChart.MetaTextStyle, "Override defaults should fill empty LineChart.MetaTextStyle.");
        TestAssert.Equal(explicitStyle, sparkline.DataStyle, "Override defaults should not overwrite explicit Sparkline.DataStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, sparkline.MetaStyle, "Override defaults should fill empty Sparkline.MetaStyle.");
        TestAssert.Equal(typeTheme.Border.Default, sparkline.BorderStyleText, "Override defaults should fill empty Sparkline.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), sparkline.FocusedBorderStyleText, "Override defaults should fill empty Sparkline.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, areaPlot.FillStyle, "Override defaults should not overwrite explicit AreaPlot.FillStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, areaPlot.LineStyle, "Override defaults should fill empty AreaPlot.LineStyle.");
        TestAssert.Equal(typeTheme.Border.Default, areaPlot.BorderStyleText, "Override defaults should fill empty AreaPlot.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), areaPlot.FocusedBorderStyleText, "Override defaults should fill empty AreaPlot.FocusedBorderStyleText.");
        TestAssert.Equal(explicitStyle, scatterPlot.PointStyle, "Override defaults should not overwrite explicit ScatterPlot.PointStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, scatterPlot.AxisStyle, "Override defaults should fill empty ScatterPlot.AxisStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, scatterPlot.LegendStyle, "Override defaults should fill empty ScatterPlot.LegendStyle.");
        TestAssert.Equal(explicitStyle, histogram.BarStyle, "Override defaults should not overwrite explicit Histogram.BarStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, histogram.AxisStyle, "Override defaults should fill empty Histogram.AxisStyle.");
        TestAssert.Equal(explicitStyle, linePlot.StatsStyle, "Override defaults should not overwrite explicit LinePlot.StatsStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, linePlot.LegendStyle, "Override defaults should fill empty LinePlot.LegendStyle.");
        TestAssert.Equal(typeTheme.Border.Default, linePlot.BorderStyleText, "Override defaults should fill empty LinePlot.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), linePlot.FocusedBorderStyleText, "Override defaults should fill empty LinePlot.FocusedBorderStyleText.");
        TestAssert.Equal(typeTheme.Text.Secondary, plotPanel.TitleStyle, "Override defaults should fill empty PlotPanel.TitleStyle.");
        TestAssert.Equal(explicitStyle, plotPanel.EmptyTextStyle, "Override defaults should not overwrite explicit PlotPanel.EmptyTextStyle.");
        TestAssert.Equal(typeTheme.Border.Default, plotPanel.BorderStyleText, "Override defaults should fill empty PlotPanel.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), plotPanel.FocusedBorderStyleText, "Override defaults should fill empty PlotPanel.FocusedBorderStyleText.");
        TestAssert.Equal(typeTheme.Text.Primary, gauge.ValueLabelStyle, "Override apply should map Gauge value label style.");
        TestAssert.Equal(explicitStyle, statsCard.ValueStyle, "Override defaults should not overwrite explicit StatsCard.ValueStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, statsCard.KeyStyle, "Override defaults should fill empty StatsCard.KeyStyle.");

        return Task.CompletedTask;
    }
}
