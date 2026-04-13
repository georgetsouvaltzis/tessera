using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static partial class ThemeOverridesTests
{
    private static Task ApplyHelpers_MapExpectedTokens_ForAccordionMultiSelectAndRadioGroup()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(12, 22, 32)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(42, 52, 62)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(72, 82, 92))
            },
            Accent = new TesseraThemeAccentTokens
            {
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(102, 112, 122)),
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(132, 142, 152))
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(162, 172, 182))
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(192, 202, 212)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(222, 232, 242))
            }
        };

        var accordion = new Accordion().ApplyTheme(theme);
        var multiSelect = new MultiSelect().ApplyTheme(theme);
        var radioGroup = new RadioGroup().ApplyTheme(theme);

        var selectionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);

        TestAssert.Equal(theme.Text.Secondary, accordion.TitleStyle,
            "Accordion title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, accordion.FocusedTitleStyle,
            "Accordion focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, accordion.ItemStyle, "Accordion item style should map to Text.Primary.");
        TestAssert.Equal(selectionStyle, accordion.SelectedItemStyle,
            "Accordion selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Secondary, accordion.ExpandedItemStyle,
            "Accordion expanded style should map to Accent.Secondary.");
        TestAssert.Equal(theme.Text.Secondary, accordion.BodyStyle,
            "Accordion body style should map to Text.Secondary.");
        TestAssert.Equal(theme.Text.Muted, accordion.DisabledItemStyle,
            "Accordion disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, multiSelect.TitleStyle,
            "MultiSelect title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, multiSelect.FocusedTitleStyle,
            "MultiSelect focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, multiSelect.ItemStyle,
            "MultiSelect item style should map to Text.Primary.");
        TestAssert.Equal(selectionStyle, multiSelect.SelectedItemStyle,
            "MultiSelect selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Accent.Primary, multiSelect.CheckedItemStyle,
            "MultiSelect checked style should map to Accent.Primary.");
        TestAssert.Equal(theme.Text.Muted, multiSelect.DisabledItemStyle,
            "MultiSelect disabled style should map to Text.Muted.");

        TestAssert.Equal(theme.Text.Secondary, radioGroup.TitleStyle,
            "RadioGroup title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, radioGroup.FocusedTitleStyle,
            "RadioGroup focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, radioGroup.ItemStyle, "RadioGroup item style should map to Text.Primary.");
        TestAssert.Equal(selectionStyle, radioGroup.SelectedItemStyle,
            "RadioGroup selected style should map to merged Selection styles.");
        TestAssert.Equal(theme.Text.Muted, radioGroup.DisabledItemStyle,
            "RadioGroup disabled style should map to Text.Muted.");

        return Task.CompletedTask;
    }

    private static Task ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForAccordionMultiSelectAndRadioGroup()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(207, 208, 209));
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9))
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(14, 15, 16))
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19))
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(24, 25, 26))
            }
        };

        var accordion = new Accordion { ItemStyle = explicitStyle };
        var multiSelect = new MultiSelect { SelectedItemStyle = explicitStyle };
        var radioGroup = new RadioGroup { ItemStyle = explicitStyle };

        accordion.ApplyThemeDefaults(theme);
        multiSelect.ApplyThemeDefaults(theme);
        radioGroup.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, accordion.ItemStyle,
            "Defaults should not overwrite explicit Accordion.ItemStyle.");
        TestAssert.Equal(theme.Text.Muted, accordion.DisabledItemStyle,
            "Defaults should fill empty Accordion.DisabledItemStyle.");
        TestAssert.Equal(explicitStyle, multiSelect.SelectedItemStyle,
            "Defaults should not overwrite explicit MultiSelect.SelectedItemStyle.");
        TestAssert.Equal(theme.Accent.Primary, multiSelect.CheckedItemStyle,
            "Defaults should fill empty MultiSelect.CheckedItemStyle.");
        TestAssert.Equal(explicitStyle, radioGroup.ItemStyle,
            "Defaults should not overwrite explicit RadioGroup.ItemStyle.");
        TestAssert.Equal(theme.Text.Muted, radioGroup.DisabledItemStyle,
            "Defaults should fill empty RadioGroup.DisabledItemStyle.");

        return Task.CompletedTask;
    }

    private static Task OverrideOverloads_ResolveExpectedTokens_ForAccordionMultiSelectAndRadioGroup()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(231, 232, 233));
        var accordion = new Accordion { ItemStyle = explicitStyle };
        var multiSelect = new MultiSelect();
        var radioGroup = new RadioGroup { ItemStyle = explicitStyle };
        var baseTheme = BuildThemeWithPrimary(1, 1, 1);
        var overrides = new TesseraThemeOverrides();
        var typeTheme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(111, 112, 113)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(121, 122, 123))
            },
            Accent = new TesseraThemeAccentTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(131, 132, 133)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(141, 142, 143))
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(151, 152, 153))
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(161, 162, 163)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(171, 172, 173))
            }
        };
        overrides.SetControlType<Accordion>(typeTheme);
        overrides.SetControlType<MultiSelect>(typeTheme);
        overrides.SetControlType<RadioGroup>(typeTheme);

        accordion.ApplyThemeDefaults(overrides, baseTheme);
        multiSelect.ApplyTheme(overrides, baseTheme);
        radioGroup.ApplyThemeDefaults(overrides, baseTheme);

        TestAssert.Equal(explicitStyle, accordion.ItemStyle,
            "Override defaults should not overwrite explicit Accordion.ItemStyle.");
        TestAssert.Equal(typeTheme.Accent.Secondary, accordion.ExpandedItemStyle,
            "Override defaults should fill empty Accordion.ExpandedItemStyle.");
        TestAssert.Equal(typeTheme.Accent.Primary, multiSelect.CheckedItemStyle,
            "Override apply should map MultiSelect checked style.");
        TestAssert.Equal(
            typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background),
            multiSelect.SelectedItemStyle,
            "Override apply should map MultiSelect selected style.");
        TestAssert.Equal(explicitStyle, radioGroup.ItemStyle,
            "Override defaults should not overwrite explicit RadioGroup.ItemStyle.");
        TestAssert.Equal(
            typeTheme.Selection.Foreground.Merge(typeTheme.Selection.Background),
            radioGroup.SelectedItemStyle,
            "Override defaults should fill empty RadioGroup.SelectedItemStyle.");

        return Task.CompletedTask;
    }
}
