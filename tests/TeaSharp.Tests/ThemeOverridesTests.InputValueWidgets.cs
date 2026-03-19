using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> InputValueCases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForLabelAndInputValueWidgets",
            ApplyHelpers_MapExpectedTokens_ForLabelAndInputValueWidgets);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForLabelAndInputValueWidgets",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForLabelAndInputValueWidgets);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForLabelAndInputValueWidgets",
            OverrideOverloads_ResolveExpectedTokens_ForLabelAndInputValueWidgets);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForLabelAndInputValueWidgets()
    {
        var theme = BuildInputValueTheme();
        var mergedSelection = theme.Selection.Foreground.Merge(theme.Selection.Background);

        var label = new Label().ApplyTheme(theme);
        var textInput = new TextInput().ApplyTheme(theme);
        var textArea = new TextArea().ApplyTheme(theme);
        var toggle = new Toggle().ApplyTheme(theme);
        var slider = new Slider().ApplyTheme(theme);
        var spinner = new Spinner().ApplyTheme(theme);
        var progressBar = new ProgressBar().ApplyTheme(theme);
        var numberInput = new NumberInput().ApplyTheme(theme);
        var datePicker = new DatePicker().ApplyTheme(theme);
        var timePicker = new TimePicker().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, label.TitleStyle, "Label title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, label.FocusedTitleStyle, "Label focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, label.TextStyle, "Label text style should map to Text.Primary.");

        TestAssert.Equal(theme.Text.Primary, textInput.ValueTextStyle, "TextInput value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, textInput.PlaceholderTextStyle, "TextInput placeholder style should map to Text.Muted.");
        TestAssert.Equal(theme.Focus.Title, textInput.FocusedTitleStyle, "TextInput focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Border.Default, textInput.BorderStyleText, "TextInput border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), textInput.FocusedBorderStyleText, "TextInput focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, textArea.TitleStyle, "TextArea title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, textArea.FocusedTitleStyle, "TextArea focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, textArea.ValueTextStyle, "TextArea value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, textArea.DisabledValueTextStyle, "TextArea disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, textArea.BorderStyleText, "TextArea border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), textArea.FocusedBorderStyleText, "TextArea focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, toggle.TitleStyle, "Toggle title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, toggle.FocusedTitleStyle, "Toggle focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, toggle.ValueStyle, "Toggle value style should map to Text.Primary.");
        TestAssert.Equal(theme.State.Success, toggle.OnValueStyle, "Toggle on style should map to State.Success.");
        TestAssert.Equal(theme.Text.Secondary, toggle.OffValueStyle, "Toggle off style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Muted, toggle.DisabledValueStyle, "Toggle disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, slider.TitleStyle, "Slider title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, slider.FocusedTitleStyle, "Slider focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, slider.ValueLabelStyle, "Slider value label style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Primary, slider.FillStyle, "Slider fill style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, slider.TrackStyle, "Slider track style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Muted, slider.DisabledStyle, "Slider disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, spinner.TitleStyle, "Spinner title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, spinner.FocusedTitleStyle, "Spinner focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, spinner.ValueStyle, "Spinner value style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Primary, spinner.RunningValueStyle, "Spinner running style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Secondary, spinner.StoppedValueStyle, "Spinner stopped style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Muted, spinner.DisabledValueStyle, "Spinner disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, progressBar.TitleStyle, "ProgressBar title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, progressBar.FocusedTitleStyle, "ProgressBar focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Accent.Primary, progressBar.FillStyle, "ProgressBar fill style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, progressBar.TrackStyle, "ProgressBar track style should map to Text.Muted.");
        TestAssert.Equal(theme.Text.Primary, progressBar.LabelStyle, "ProgressBar label style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Muted, progressBar.DisabledStyle, "ProgressBar disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, numberInput.TitleStyle, "NumberInput title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, numberInput.FocusedTitleStyle, "NumberInput focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, numberInput.ValueTextStyle, "NumberInput value style should map to Text.Primary.");
        TestAssert.Equal(theme.Text.Secondary, numberInput.SummaryTextStyle, "NumberInput summary style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Muted, numberInput.DisabledTextStyle, "NumberInput disabled style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, numberInput.BorderStyleText, "NumberInput border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), numberInput.FocusedBorderStyleText, "NumberInput focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, datePicker.TitleStyle, "DatePicker title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, datePicker.FocusedTitleStyle, "DatePicker focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Secondary, datePicker.MonthHeaderStyle, "DatePicker month header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, datePicker.WeekdayHeaderStyle, "DatePicker weekday header style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Primary, datePicker.DayStyle, "DatePicker day style should map to Text.Primary.");
        TestAssert.Equal(mergedSelection, datePicker.SelectedDayStyle, "DatePicker selected day style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Secondary, datePicker.HoveredDayStyle, "DatePicker hovered day style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Muted, datePicker.DisabledDayStyle, "DatePicker disabled day style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, datePicker.BorderStyleText, "DatePicker border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), datePicker.FocusedBorderStyleText, "DatePicker focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Secondary, timePicker.TitleStyle, "TimePicker title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, timePicker.FocusedTitleStyle, "TimePicker focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, timePicker.ValueTextStyle, "TimePicker value style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Primary, timePicker.ActiveFieldStyle, "TimePicker active field style should map to Accent.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, timePicker.HoveredFieldStyle, "TimePicker hovered field style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, timePicker.SeparatorStyle, "TimePicker separator style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Muted, timePicker.DisabledValueStyle, "TimePicker disabled value style should map to Text.Muted.");
        TestAssert.Equal(theme.Border.Default, timePicker.BorderStyleText, "TimePicker border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), timePicker.FocusedBorderStyleText, "TimePicker focused border style should map to focused border tokens.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForLabelAndInputValueWidgets()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = BuildInputValueTheme();

        var label = new Label { TextStyle = explicitStyle };
        var textInput = new TextInput { BorderStyleText = explicitStyle };
        var textArea = new TextArea { ValueTextStyle = explicitStyle, BorderStyleText = explicitStyle };
        var toggle = new Toggle { OnValueStyle = explicitStyle };
        var slider = new Slider { FillStyle = explicitStyle };
        var spinner = new Spinner { RunningValueStyle = explicitStyle };
        var progressBar = new ProgressBar { FillStyle = explicitStyle };
        var numberInput = new NumberInput { ValueTextStyle = explicitStyle, BorderStyleText = explicitStyle };
        var datePicker = new DatePicker { SelectedDayStyle = explicitStyle, BorderStyleText = explicitStyle };
        var timePicker = new TimePicker { ActiveFieldStyle = explicitStyle, BorderStyleText = explicitStyle };

        label.ApplyThemeDefaults(theme);
        textInput.ApplyThemeDefaults(theme);
        textArea.ApplyThemeDefaults(theme);
        toggle.ApplyThemeDefaults(theme);
        slider.ApplyThemeDefaults(theme);
        spinner.ApplyThemeDefaults(theme);
        progressBar.ApplyThemeDefaults(theme);
        numberInput.ApplyThemeDefaults(theme);
        datePicker.ApplyThemeDefaults(theme);
        timePicker.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, label.TextStyle, "Defaults should not overwrite explicit Label.TextStyle.");
        TestAssert.Equal(theme.Text.Secondary, label.TitleStyle, "Defaults should fill empty Label.TitleStyle.");

        TestAssert.Equal(explicitStyle, textInput.BorderStyleText, "Defaults should not overwrite explicit TextInput.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), textInput.FocusedBorderStyleText, "Defaults should fill empty TextInput.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, textArea.ValueTextStyle, "Defaults should not overwrite explicit TextArea.ValueTextStyle.");
        TestAssert.Equal(theme.Text.Muted, textArea.DisabledValueTextStyle, "Defaults should fill empty TextArea.DisabledValueTextStyle.");
        TestAssert.Equal(explicitStyle, textArea.BorderStyleText, "Defaults should not overwrite explicit TextArea.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), textArea.FocusedBorderStyleText, "Defaults should fill empty TextArea.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, toggle.OnValueStyle, "Defaults should not overwrite explicit Toggle.OnValueStyle.");
        TestAssert.Equal(theme.Text.Secondary, toggle.OffValueStyle, "Defaults should fill empty Toggle.OffValueStyle.");

        TestAssert.Equal(explicitStyle, slider.FillStyle, "Defaults should not overwrite explicit Slider.FillStyle.");
        TestAssert.Equal(theme.Text.Muted, slider.TrackStyle, "Defaults should fill empty Slider.TrackStyle.");

        TestAssert.Equal(explicitStyle, spinner.RunningValueStyle, "Defaults should not overwrite explicit Spinner.RunningValueStyle.");
        TestAssert.Equal(theme.Text.Secondary, spinner.StoppedValueStyle, "Defaults should fill empty Spinner.StoppedValueStyle.");

        TestAssert.Equal(explicitStyle, progressBar.FillStyle, "Defaults should not overwrite explicit ProgressBar.FillStyle.");
        TestAssert.Equal(theme.Text.Muted, progressBar.TrackStyle, "Defaults should fill empty ProgressBar.TrackStyle.");

        TestAssert.Equal(explicitStyle, numberInput.ValueTextStyle, "Defaults should not overwrite explicit NumberInput.ValueTextStyle.");
        TestAssert.Equal(theme.Text.Secondary, numberInput.SummaryTextStyle, "Defaults should fill empty NumberInput.SummaryTextStyle.");
        TestAssert.Equal(explicitStyle, numberInput.BorderStyleText, "Defaults should not overwrite explicit NumberInput.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), numberInput.FocusedBorderStyleText, "Defaults should fill empty NumberInput.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, datePicker.SelectedDayStyle, "Defaults should not overwrite explicit DatePicker.SelectedDayStyle.");
        TestAssert.Equal(theme.Accent.Secondary, datePicker.HoveredDayStyle, "Defaults should fill empty DatePicker.HoveredDayStyle.");
        TestAssert.Equal(explicitStyle, datePicker.BorderStyleText, "Defaults should not overwrite explicit DatePicker.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), datePicker.FocusedBorderStyleText, "Defaults should fill empty DatePicker.FocusedBorderStyleText.");

        TestAssert.Equal(explicitStyle, timePicker.ActiveFieldStyle, "Defaults should not overwrite explicit TimePicker.ActiveFieldStyle.");
        TestAssert.Equal(theme.Accent.Secondary, timePicker.HoveredFieldStyle, "Defaults should fill empty TimePicker.HoveredFieldStyle.");
        TestAssert.Equal(explicitStyle, timePicker.BorderStyleText, "Defaults should not overwrite explicit TimePicker.BorderStyleText.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), timePicker.FocusedBorderStyleText, "Defaults should fill empty TimePicker.FocusedBorderStyleText.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForLabelAndInputValueWidgets()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var label = new Label { TextStyle = explicitStyle };
        var textInput = new TextInput { BorderStyleText = explicitStyle };
        var textArea = new TextArea { BorderStyleText = explicitStyle };
        var toggle = new Toggle { ValueStyle = explicitStyle };
        var slider = new Slider();
        var spinner = new Spinner { ValueStyle = explicitStyle };
        var progressBar = new ProgressBar();
        var numberInput = new NumberInput { ValueTextStyle = explicitStyle, BorderStyleText = explicitStyle };
        var datePicker = new DatePicker { BorderStyleText = explicitStyle };
        var timePicker = new TimePicker { ActiveFieldStyle = explicitStyle, BorderStyleText = explicitStyle };

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = BuildInputValueTheme();
        var mergedSelection = typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background);

        overrides.SetControlType<Label>(typeTheme);
        overrides.SetControlType<TextInput>(typeTheme);
        overrides.SetControlType<TextArea>(typeTheme);
        overrides.SetControlType<Toggle>(typeTheme);
        overrides.SetControlType<Slider>(typeTheme);
        overrides.SetControlType<Spinner>(typeTheme);
        overrides.SetControlType<ProgressBar>(typeTheme);
        overrides.SetControlType<NumberInput>(typeTheme);
        overrides.SetControlType<DatePicker>(typeTheme);
        overrides.SetControlType<TimePicker>(typeTheme);

        label.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        textInput.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        textArea.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        toggle.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        slider.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        spinner.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        progressBar.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        numberInput.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        datePicker.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        timePicker.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);

        TestAssert.Equal(explicitStyle, label.TextStyle, "Override defaults should not overwrite explicit Label.TextStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, label.TitleStyle, "Override defaults should fill empty Label.TitleStyle.");

        TestAssert.Equal(explicitStyle, textInput.BorderStyleText, "Override defaults should not overwrite explicit TextInput.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), textInput.FocusedBorderStyleText, "Override defaults should fill empty TextInput.FocusedBorderStyleText.");

        TestAssert.Equal(typeTheme.Text.Primary, textArea.ValueTextStyle, "Override apply should map TextArea.ValueTextStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, textArea.DisabledValueTextStyle, "Override apply should map TextArea.DisabledValueTextStyle.");
        TestAssert.Equal(typeTheme.Border.Default, textArea.BorderStyleText, "Override apply should map TextArea.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), textArea.FocusedBorderStyleText, "Override apply should map TextArea focused border style.");

        TestAssert.Equal(explicitStyle, toggle.ValueStyle, "Override defaults should not overwrite explicit Toggle.ValueStyle.");
        TestAssert.Equal(typeTheme.State.Success, toggle.OnValueStyle, "Override defaults should fill empty Toggle.OnValueStyle.");

        TestAssert.Equal(typeTheme.Accent.Primary, slider.FillStyle, "Override apply should map Slider.FillStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, slider.TrackStyle, "Override apply should map Slider.TrackStyle.");

        TestAssert.Equal(explicitStyle, spinner.ValueStyle, "Override defaults should not overwrite explicit Spinner.ValueStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, spinner.RunningValueStyle, "Override defaults should fill empty Spinner.RunningValueStyle.");

        TestAssert.Equal(typeTheme.Accent.Primary, progressBar.FillStyle, "Override apply should map ProgressBar.FillStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, progressBar.TrackStyle, "Override apply should map ProgressBar.TrackStyle.");

        TestAssert.Equal(explicitStyle, numberInput.ValueTextStyle, "Override defaults should not overwrite explicit NumberInput.ValueTextStyle.");
        TestAssert.Equal(typeTheme.Text.Secondary, numberInput.SummaryTextStyle, "Override defaults should fill empty NumberInput.SummaryTextStyle.");
        TestAssert.Equal(explicitStyle, numberInput.BorderStyleText, "Override defaults should not overwrite explicit NumberInput.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), numberInput.FocusedBorderStyleText, "Override defaults should fill NumberInput focused border style.");

        TestAssert.Equal(typeTheme.Text.Primary, datePicker.DayStyle, "Override apply should map DatePicker.DayStyle.");
        TestAssert.Equal(mergedSelection, datePicker.SelectedDayStyle, "Override apply should map DatePicker.SelectedDayStyle.");
        TestAssert.Equal(typeTheme.Border.Default, datePicker.BorderStyleText, "Override apply should map DatePicker border style.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), datePicker.FocusedBorderStyleText, "Override apply should map DatePicker focused border style.");

        TestAssert.Equal(explicitStyle, timePicker.ActiveFieldStyle, "Override defaults should not overwrite explicit TimePicker.ActiveFieldStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, timePicker.HoveredFieldStyle, "Override defaults should fill empty TimePicker.HoveredFieldStyle.");
        TestAssert.Equal(explicitStyle, timePicker.BorderStyleText, "Override defaults should not overwrite explicit TimePicker.BorderStyleText.");
        TestAssert.Equal(typeTheme.Border.Focused.Merge(typeTheme.Focus.Border), timePicker.FocusedBorderStyleText, "Override defaults should fill TimePicker focused border style.");

        return Task.CompletedTask;
    }

    private static TeaTheme BuildInputValueTheme()
    {
        return new TeaTheme
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
                Secondary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(51, 52, 53)),
            },
            Focus = new TeaThemeFocusTokens
            {
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 62, 63)),
                Border = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(64, 65, 66)),
            },
            Border = new TeaThemeBorderTokens
            {
                Default = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Focused = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
            },
            Selection = new TeaThemeSelectionTokens
            {
                Foreground = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(71, 72, 73)),
                Background = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(81, 82, 83)),
            },
            State = new TeaThemeStateTokens
            {
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
            },
        };
    }
}
