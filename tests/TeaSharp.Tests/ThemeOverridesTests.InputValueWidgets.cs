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
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            },
        };

        var label = new Label();
        var textArea = new TextArea { Title = "TextArea A" };
        var toggle = new Toggle { Title = "Toggle A" };
        var slider = new Slider { Title = "Slider A" };
        var spinner = new Spinner { Title = "Spinner A" };
        var progressBar = new ProgressBar { Title = "Progress A" };
        var numberInput = new NumberInput { Title = "Number A" };
        var datePicker = new DatePicker { Title = "Date A" };
        var timePicker = new TimePicker { Title = "Time A" };

        var labelResult = label.ApplyTheme(theme);
        var textAreaResult = textArea.ApplyTheme(theme);
        var toggleResult = toggle.ApplyTheme(theme);
        var sliderResult = slider.ApplyTheme(theme);
        var spinnerResult = spinner.ApplyTheme(theme);
        var progressBarResult = progressBar.ApplyTheme(theme);
        var numberInputResult = numberInput.ApplyTheme(theme);
        var datePickerResult = datePicker.ApplyTheme(theme);
        var timePickerResult = timePicker.ApplyTheme(theme);

        TestAssert.ReferenceSame(label, labelResult, "ApplyTheme should return same Label instance.");
        TestAssert.Equal(theme.Text.Primary, label.TextStyle, "Label text style should map to Text.Primary.");

        TestAssert.ReferenceSame(textArea, textAreaResult, "ApplyTheme should return same TextArea instance.");
        TestAssert.ReferenceSame(toggle, toggleResult, "ApplyTheme should return same Toggle instance.");
        TestAssert.ReferenceSame(slider, sliderResult, "ApplyTheme should return same Slider instance.");
        TestAssert.ReferenceSame(spinner, spinnerResult, "ApplyTheme should return same Spinner instance.");
        TestAssert.ReferenceSame(progressBar, progressBarResult, "ApplyTheme should return same ProgressBar instance.");
        TestAssert.ReferenceSame(numberInput, numberInputResult, "ApplyTheme should return same NumberInput instance.");
        TestAssert.ReferenceSame(datePicker, datePickerResult, "ApplyTheme should return same DatePicker instance.");
        TestAssert.ReferenceSame(timePicker, timePickerResult, "ApplyTheme should return same TimePicker instance.");

        TestAssert.Equal("TextArea A", textArea.Title, "ApplyTheme should not alter TextArea title.");
        TestAssert.Equal("Toggle A", toggle.Title, "ApplyTheme should not alter Toggle title.");
        TestAssert.Equal("Slider A", slider.Title, "ApplyTheme should not alter Slider title.");
        TestAssert.Equal("Spinner A", spinner.Title, "ApplyTheme should not alter Spinner title.");
        TestAssert.Equal("Progress A", progressBar.Title, "ApplyTheme should not alter ProgressBar title.");
        TestAssert.Equal("Number A", numberInput.Title, "ApplyTheme should not alter NumberInput title.");
        TestAssert.Equal("Date A", datePicker.Title, "ApplyTheme should not alter DatePicker title.");
        TestAssert.Equal("Time A", timePicker.Title, "ApplyTheme should not alter TimePicker title.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForLabelAndInputValueWidgets()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
            },
        };

        var label = new Label
        {
            TextStyle = explicitStyle,
        };
        var textArea = new TextArea { Title = "TextArea Defaults" };
        var toggle = new Toggle { Title = "Toggle Defaults" };
        var slider = new Slider { Title = "Slider Defaults" };
        var spinner = new Spinner { Title = "Spinner Defaults" };
        var progressBar = new ProgressBar { Title = "Progress Defaults" };
        var numberInput = new NumberInput { Title = "Number Defaults" };
        var datePicker = new DatePicker { Title = "Date Defaults" };
        var timePicker = new TimePicker { Title = "Time Defaults" };

        label.ApplyThemeDefaults(theme);
        textArea.ApplyThemeDefaults(theme);
        toggle.ApplyThemeDefaults(theme);
        slider.ApplyThemeDefaults(theme);
        spinner.ApplyThemeDefaults(theme);
        progressBar.ApplyThemeDefaults(theme);
        numberInput.ApplyThemeDefaults(theme);
        datePicker.ApplyThemeDefaults(theme);
        timePicker.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, label.TextStyle, "Defaults should not overwrite explicit Label.TextStyle.");
        TestAssert.Equal("TextArea Defaults", textArea.Title, "Defaults should not alter TextArea title.");
        TestAssert.Equal("Toggle Defaults", toggle.Title, "Defaults should not alter Toggle title.");
        TestAssert.Equal("Slider Defaults", slider.Title, "Defaults should not alter Slider title.");
        TestAssert.Equal("Spinner Defaults", spinner.Title, "Defaults should not alter Spinner title.");
        TestAssert.Equal("Progress Defaults", progressBar.Title, "Defaults should not alter ProgressBar title.");
        TestAssert.Equal("Number Defaults", numberInput.Title, "Defaults should not alter NumberInput title.");
        TestAssert.Equal("Date Defaults", datePicker.Title, "Defaults should not alter DatePicker title.");
        TestAssert.Equal("Time Defaults", timePicker.Title, "Defaults should not alter TimePicker title.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForLabelAndInputValueWidgets()
    {
        var label = new Label();
        var textArea = new TextArea { Title = "TextArea Override" };
        var toggle = new Toggle { Title = "Toggle Override" };
        var slider = new Slider { Title = "Slider Override" };
        var spinner = new Spinner { Title = "Spinner Override" };
        var progressBar = new ProgressBar { Title = "Progress Override" };
        var numberInput = new NumberInput { Title = "Number Override" };
        var datePicker = new DatePicker { Title = "Date Override" };
        var timePicker = new TimePicker { Title = "Time Override" };

        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TeaThemeOverrides();
        var typeTheme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
            },
        };

        overrides.SetControlType<Label>(typeTheme);
        overrides.SetControlType<TextArea>(typeTheme);
        overrides.SetControlType<Toggle>(typeTheme);
        overrides.SetControlType<Slider>(typeTheme);
        overrides.SetControlType<Spinner>(typeTheme);
        overrides.SetControlType<ProgressBar>(typeTheme);
        overrides.SetControlType<NumberInput>(typeTheme);
        overrides.SetControlType<DatePicker>(typeTheme);
        overrides.SetControlType<TimePicker>(typeTheme);

        var labelResult = label.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var textAreaResult = textArea.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var toggleResult = toggle.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var sliderResult = slider.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var spinnerResult = spinner.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var progressBarResult = progressBar.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var numberInputResult = numberInput.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);
        var datePickerResult = datePicker.ApplyThemeDefaults(overrides, baseTheme, TeaThemeVisualState.Focused);
        var timePickerResult = timePicker.ApplyTheme(overrides, baseTheme, TeaThemeVisualState.Focused);

        TestAssert.ReferenceSame(label, labelResult, "Override apply should return same Label instance.");
        TestAssert.Equal(typeTheme.Text.Primary, label.TextStyle, "Override apply should map Label text style.");

        TestAssert.ReferenceSame(textArea, textAreaResult, "Override defaults should return same TextArea instance.");
        TestAssert.ReferenceSame(toggle, toggleResult, "Override apply should return same Toggle instance.");
        TestAssert.ReferenceSame(slider, sliderResult, "Override defaults should return same Slider instance.");
        TestAssert.ReferenceSame(spinner, spinnerResult, "Override apply should return same Spinner instance.");
        TestAssert.ReferenceSame(progressBar, progressBarResult, "Override defaults should return same ProgressBar instance.");
        TestAssert.ReferenceSame(numberInput, numberInputResult, "Override apply should return same NumberInput instance.");
        TestAssert.ReferenceSame(datePicker, datePickerResult, "Override defaults should return same DatePicker instance.");
        TestAssert.ReferenceSame(timePicker, timePickerResult, "Override apply should return same TimePicker instance.");

        TestAssert.Equal("TextArea Override", textArea.Title, "Override defaults should not alter TextArea title.");
        TestAssert.Equal("Toggle Override", toggle.Title, "Override apply should not alter Toggle title.");
        TestAssert.Equal("Slider Override", slider.Title, "Override defaults should not alter Slider title.");
        TestAssert.Equal("Spinner Override", spinner.Title, "Override apply should not alter Spinner title.");
        TestAssert.Equal("Progress Override", progressBar.Title, "Override defaults should not alter ProgressBar title.");
        TestAssert.Equal("Number Override", numberInput.Title, "Override apply should not alter NumberInput title.");
        TestAssert.Equal("Date Override", datePicker.Title, "Override defaults should not alter DatePicker title.");
        TestAssert.Equal("Time Override", timePicker.Title, "Override apply should not alter TimePicker title.");

        return Task.CompletedTask;
    }
}
