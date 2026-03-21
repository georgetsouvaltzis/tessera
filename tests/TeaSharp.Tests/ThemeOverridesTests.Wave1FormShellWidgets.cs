using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    private static IEnumerable<TestCase> FlowWave1Cases()
    {
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForWave1FormShellControls",
            ApplyHelpers_MapExpectedTokens_ForWave1FormShellControls);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave1FormShellControls",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave1FormShellControls);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForWave1FormShellControls",
            OverrideOverloads_ResolveExpectedTokens_ForWave1FormShellControls);
    }

    private static Task ApplyHelpers_MapExpectedTokens_ForWave1FormShellControls()
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
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(94, 95, 96)),
            },
        };

        var form = new Form().ApplyTheme(theme);
        var fieldSet = new FieldSet().ApplyTheme(theme);
        var splitView = new SplitView().ApplyTheme(theme);
        var inspector = new InspectorPanel().ApplyTheme(theme);
        var wizard = new Wizard().ApplyTheme(theme);
        var dataForm = new DataForm<Wave1DataFormModel>().ApplyTheme(theme);

        TestAssert.Equal(theme.Text.Secondary, form.LabelStyle, "Form label style should map to Text.Secondary.");
        TestAssert.Equal(theme.State.Error, form.RequiredMarkerStyle, "Form required marker style should map to State.Error.");
        TestAssert.Equal(theme.Border.Default, form.BorderStyleText, "Form border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Primary, fieldSet.ItemStyle, "FieldSet item style should map to Text.Primary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), fieldSet.SelectedItemStyle, "FieldSet selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), fieldSet.FocusedBorderStyleText, "FieldSet focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Text.Muted, splitView.DividerStyle, "SplitView divider style should map to Text.Muted.");
        TestAssert.Equal(theme.Focus.Ring, splitView.FocusedDividerStyle, "SplitView focused divider style should map to Focus.Ring.");
        TestAssert.Equal(theme.Border.Default, splitView.BorderStyleText, "SplitView border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Secondary, inspector.SectionStyle, "InspectorPanel section style should map to Text.Secondary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), inspector.SelectedRowStyle, "InspectorPanel selected row style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), inspector.FocusedBorderStyleText, "InspectorPanel focused border style should map to focused border tokens.");

        TestAssert.Equal(theme.Accent.Primary, wizard.ActiveStepStyle, "Wizard active step style should map to Accent.Primary.");
        TestAssert.Equal(theme.State.Success, wizard.CompletedStepStyle, "Wizard completed step style should map to State.Success.");
        TestAssert.Equal(theme.Border.Default, wizard.BorderStyleText, "Wizard border style should map to Border.Default.");

        TestAssert.Equal(theme.Text.Secondary, dataForm.LabelStyle, "DataForm label style should map to Text.Secondary.");
        TestAssert.Equal(theme.Selection.Foreground.Merge(theme.Selection.Background), dataForm.SelectedFieldStyle, "DataForm selected field style should map to merged Selection styles.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), dataForm.FocusedBorderStyleText, "DataForm focused border style should map to focused border tokens.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForWave1FormShellControls()
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
                Title = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19)),
                Ring = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
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
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(38, 39, 40)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43)),
            },
        };

        var form = new Form { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };
        var fieldSet = new FieldSet { ItemStyle = explicitStyle, BorderStyleText = explicitStyle };
        var splitView = new SplitView { DividerStyle = explicitStyle, BorderStyleText = explicitStyle };
        var inspector = new InspectorPanel { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };
        var wizard = new Wizard { StepStyle = explicitStyle, BorderStyleText = explicitStyle };
        var dataForm = new DataForm<Wave1DataFormModel> { ValueStyle = explicitStyle, BorderStyleText = explicitStyle };

        form.ApplyThemeDefaults(theme);
        fieldSet.ApplyThemeDefaults(theme);
        splitView.ApplyThemeDefaults(theme);
        inspector.ApplyThemeDefaults(theme);
        wizard.ApplyThemeDefaults(theme);
        dataForm.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, form.ValueStyle, "Defaults should not overwrite explicit Form.ValueStyle.");
        TestAssert.Equal(theme.State.Error, form.RequiredMarkerStyle, "Defaults should fill empty Form.RequiredMarkerStyle.");
        TestAssert.Equal(explicitStyle, form.BorderStyleText, "Defaults should not overwrite explicit Form.BorderStyleText.");

        TestAssert.Equal(explicitStyle, fieldSet.ItemStyle, "Defaults should not overwrite explicit FieldSet.ItemStyle.");
        TestAssert.Equal(theme.Accent.Secondary, fieldSet.HoveredItemStyle, "Defaults should fill empty FieldSet.HoveredItemStyle.");
        TestAssert.Equal(explicitStyle, fieldSet.BorderStyleText, "Defaults should not overwrite explicit FieldSet.BorderStyleText.");

        TestAssert.Equal(explicitStyle, splitView.DividerStyle, "Defaults should not overwrite explicit SplitView.DividerStyle.");
        TestAssert.Equal(theme.Focus.Ring, splitView.FocusedDividerStyle, "Defaults should fill empty SplitView.FocusedDividerStyle.");
        TestAssert.Equal(explicitStyle, splitView.BorderStyleText, "Defaults should not overwrite explicit SplitView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, inspector.ValueStyle, "Defaults should not overwrite explicit InspectorPanel.ValueStyle.");
        TestAssert.Equal(theme.Accent.Secondary, inspector.MarkerStyle, "Defaults should fill empty InspectorPanel.MarkerStyle.");
        TestAssert.Equal(explicitStyle, inspector.BorderStyleText, "Defaults should not overwrite explicit InspectorPanel.BorderStyleText.");

        TestAssert.Equal(explicitStyle, wizard.StepStyle, "Defaults should not overwrite explicit Wizard.StepStyle.");
        TestAssert.Equal(theme.State.Success, wizard.CompletedStepStyle, "Defaults should fill empty Wizard.CompletedStepStyle.");
        TestAssert.Equal(explicitStyle, wizard.BorderStyleText, "Defaults should not overwrite explicit Wizard.BorderStyleText.");

        TestAssert.Equal(explicitStyle, dataForm.ValueStyle, "Defaults should not overwrite explicit DataForm.ValueStyle.");
        TestAssert.Equal(theme.Text.Muted, dataForm.PlaceholderStyle, "Defaults should fill empty DataForm.PlaceholderStyle.");
        TestAssert.Equal(explicitStyle, dataForm.BorderStyleText, "Defaults should not overwrite explicit DataForm.BorderStyleText.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForWave1FormShellControls()
    {
        var explicitStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(221, 222, 223));
        var form = new Form { ValueStyle = explicitStyle };
        var fieldSet = new FieldSet { ItemStyle = explicitStyle };
        var splitView = new SplitView { DividerStyle = explicitStyle };
        var inspector = new InspectorPanel { ValueStyle = explicitStyle };
        var wizard = new Wizard { StepStyle = explicitStyle };
        var dataForm = new DataForm<Wave1DataFormModel> { ValueStyle = explicitStyle };

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
                Success = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(181, 182, 183)),
                Error = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(184, 185, 186)),
            },
        };

        overrides.SetControlType<Form>(typeTheme);
        overrides.SetControlType<FieldSet>(typeTheme);
        overrides.SetControlType<SplitView>(typeTheme);
        overrides.SetControlType<InspectorPanel>(typeTheme);
        overrides.SetControlType<Wizard>(typeTheme);
        overrides.SetControlType<DataForm<Wave1DataFormModel>>(typeTheme);

        form.ApplyThemeDefaults(overrides, baseTheme);
        fieldSet.ApplyThemeDefaults(overrides, baseTheme);
        splitView.ApplyThemeDefaults(overrides, baseTheme);
        inspector.ApplyThemeDefaults(overrides, baseTheme);
        wizard.ApplyThemeDefaults(overrides, baseTheme);
        dataForm.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, form.ValueStyle, "Override defaults should not overwrite explicit Form.ValueStyle.");
        TestAssert.Equal(typeTheme.Border.Default, form.BorderStyleText, "Override defaults should fill empty Form.BorderStyleText.");

        TestAssert.Equal(explicitStyle, fieldSet.ItemStyle, "Override defaults should not overwrite explicit FieldSet.ItemStyle.");
        TestAssert.Equal(typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background), fieldSet.SelectedItemStyle, "Override defaults should fill empty FieldSet.SelectedItemStyle.");

        TestAssert.Equal(explicitStyle, splitView.DividerStyle, "Override defaults should not overwrite explicit SplitView.DividerStyle.");
        TestAssert.Equal(typeTheme.Border.Default, splitView.BorderStyleText, "Override defaults should fill empty SplitView.BorderStyleText.");

        TestAssert.Equal(explicitStyle, inspector.ValueStyle, "Override defaults should not overwrite explicit InspectorPanel.ValueStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, inspector.MarkerStyle, "Override defaults should fill empty InspectorPanel.MarkerStyle.");

        TestAssert.Equal(explicitStyle, wizard.StepStyle, "Override defaults should not overwrite explicit Wizard.StepStyle.");
        TestAssert.Equal(typeTheme.State.Success, wizard.CompletedStepStyle, "Override defaults should fill empty Wizard.CompletedStepStyle.");

        TestAssert.Equal(explicitStyle, dataForm.ValueStyle, "Override defaults should not overwrite explicit DataForm.ValueStyle.");
        TestAssert.Equal(typeTheme.Text.Muted, dataForm.PlaceholderStyle, "Override defaults should fill empty DataForm.PlaceholderStyle.");
        TestAssert.Equal(typeTheme.Border.Default, dataForm.BorderStyleText, "Override defaults should fill empty DataForm.BorderStyleText.");

        return Task.CompletedTask;
    }

    private sealed class Wave1DataFormModel
    {
        public string Value { get; set; } = string.Empty;
    }
}
