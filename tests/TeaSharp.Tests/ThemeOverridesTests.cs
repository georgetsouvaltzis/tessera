using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static partial class ThemeOverridesTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "ThemeOverrides_Precedence_InstanceStateBeatsTypeAndGlobal",
            Precedence_InstanceStateBeatsTypeAndGlobal);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForButtonAndStatusBar",
            ApplyHelpers_MapExpectedTokensForButtonAndStatusBar);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoesNotOverwriteExplicitStyles",
            ApplyThemeDefaults_DoesNotOverwriteExplicitStyles);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForTableAndTabs",
            ApplyHelpers_MapExpectedTokensForTableAndTabs);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokensForBreadcrumbAndPaginator",
            ApplyHelpers_MapExpectedTokensForBreadcrumbAndPaginator);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoesNotOverwriteExplicitStyles_ForBreadcrumbAndPaginator",
            ApplyThemeDefaults_DoesNotOverwriteExplicitStyles_ForBreadcrumbAndPaginator);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForBreadcrumbAndPaginator",
            OverrideOverloads_ResolveExpectedTokens_ForBreadcrumbAndPaginator);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForToolbarCommandBarAndSearchBox",
            ApplyHelpers_MapExpectedTokens_ForToolbarCommandBarAndSearchBox);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForToolbarCommandBarAndSearchBox",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForToolbarCommandBarAndSearchBox);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForToolbarCommandBarAndSearchBox",
            OverrideOverloads_ResolveExpectedTokens_ForToolbarCommandBarAndSearchBox);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForDiffViewAndPropertyGrid",
            ApplyHelpers_MapExpectedTokens_ForDiffViewAndPropertyGrid);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDiffViewAndPropertyGrid",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDiffViewAndPropertyGrid);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForDiffViewAndPropertyGrid",
            OverrideOverloads_ResolveExpectedTokens_ForDiffViewAndPropertyGrid);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter",
            ApplyHelpers_MapExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter",
            OverrideOverloads_ResolveExpectedTokens_ForFileExplorerFuzzyFinderAndToastCenter);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForDataGridTreeTableAndKeyValueList",
            ApplyHelpers_MapExpectedTokens_ForDataGridTreeTableAndKeyValueList);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDataGridTreeTableAndKeyValueList",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForDataGridTreeTableAndKeyValueList);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForDataGridTreeTableAndKeyValueList",
            OverrideOverloads_ResolveExpectedTokens_ForDataGridTreeTableAndKeyValueList);
        yield return new TestCase(
            "ThemeOverrides_ApplyHelpers_MapExpectedTokens_ForTimelineAndStepper",
            ApplyHelpers_MapExpectedTokens_ForTimelineAndStepper);
        yield return new TestCase(
            "ThemeOverrides_ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForTimelineAndStepper",
            ApplyThemeDefaults_DoNotOverwriteExplicitStyles_ForTimelineAndStepper);
        yield return new TestCase(
            "ThemeOverrides_OverrideOverloads_ResolveExpectedTokens_ForTimelineAndStepper",
            OverrideOverloads_ResolveExpectedTokens_ForTimelineAndStepper);
    }

    private static TeaTheme BuildThemeWithPrimary(byte red, byte green, byte blue)
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue)),
            },
        };
    }
}
