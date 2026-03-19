using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
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
}
