using NUnit.Framework;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ThemeOverridesFocusMarkerWiringTests
{
    [Test]
    public void ThemeOverridesApplyThemeMapsFocusMarkerForChoiceComboBoxAndTreeView()
    {
        var theme = new TesseraTheme
        {
            Focus = new TesseraThemeFocusTokens
            {
                Marker = "»",
            },
        };

        var choice = new Choice { FocusMarker = "*" }.ApplyTheme(theme);
        var comboBox = new ComboBox { FocusMarker = "*" }.ApplyTheme(theme);
        var treeView = new TreeView { FocusMarker = "*" }.ApplyTheme(theme);

        TestAssert.Equal("»", choice.FocusMarker, "Choice ApplyTheme should map Focus.Marker.");
        TestAssert.Equal("»", comboBox.FocusMarker, "ComboBox ApplyTheme should map Focus.Marker.");
        TestAssert.Equal("»", treeView.FocusMarker, "TreeView ApplyTheme should map Focus.Marker.");
    }

    [Test]
    public void ThemeOverridesApplyThemeDefaultsOnlyFillsEmptyFocusMarkerForChoiceComboBoxAndTreeView()
    {
        var theme = new TesseraTheme
        {
            Focus = new TesseraThemeFocusTokens
            {
                Marker = "::",
            },
        };

        var choiceEmpty = new Choice { FocusMarker = string.Empty };
        var comboEmpty = new ComboBox { FocusMarker = string.Empty };
        var treeEmpty = new TreeView { FocusMarker = string.Empty };
        var choiceExplicit = new Choice { FocusMarker = "!" };
        var comboExplicit = new ComboBox { FocusMarker = "!" };
        var treeExplicit = new TreeView { FocusMarker = "!" };

        choiceEmpty.ApplyThemeDefaults(theme);
        comboEmpty.ApplyThemeDefaults(theme);
        treeEmpty.ApplyThemeDefaults(theme);
        choiceExplicit.ApplyThemeDefaults(theme);
        comboExplicit.ApplyThemeDefaults(theme);
        treeExplicit.ApplyThemeDefaults(theme);

        TestAssert.Equal("::", choiceEmpty.FocusMarker, "Choice defaults should fill empty FocusMarker.");
        TestAssert.Equal("::", comboEmpty.FocusMarker, "ComboBox defaults should fill empty FocusMarker.");
        TestAssert.Equal("::", treeEmpty.FocusMarker, "TreeView defaults should fill empty FocusMarker.");
        TestAssert.Equal("!", choiceExplicit.FocusMarker, "Choice defaults should not overwrite explicit FocusMarker.");
        TestAssert.Equal("!", comboExplicit.FocusMarker, "ComboBox defaults should not overwrite explicit FocusMarker.");
        TestAssert.Equal("!", treeExplicit.FocusMarker, "TreeView defaults should not overwrite explicit FocusMarker.");
    }
}
