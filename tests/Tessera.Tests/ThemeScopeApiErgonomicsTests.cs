using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ThemeScopeApiErgonomicsTests
{
    [Test]
    public void ThemeScopeApplyNullThemeThrows()
    {
        var controls = new Control[]
        {
            new Button(),
        };

        Assert.Throws<ArgumentNullException>(() => ThemeScope.Apply(null!, controls));
    }

    [Test]
    public void ThemeScopeApplyNullControlsThrows()
    {
        var theme = TesseraThemes.Catppuccin();
        Control[]? controls = null;
        IEnumerable<Control?>? enumerableControls = null;

        Assert.Throws<ArgumentNullException>(() => ThemeScope.Apply(theme, controls!));
        Assert.Throws<ArgumentNullException>(() => ThemeScope.Apply(theme, enumerableControls!));
    }

    [Test]
    public void ThemeScopeApplyEmptyAndUnsupportedControlsReturnZero()
    {
        var theme = TesseraThemes.Catppuccin();
        var emptyApplied = ThemeScope.Apply(theme, Array.Empty<Control>());
        var unsupportedApplied = ThemeScope.Apply(theme, new Control[]
        {
            new NoThemeControl(),
        });
        var nullElementApplied = ThemeScope.Apply(theme, (IEnumerable<Control?>)new Control?[] { null });

        Assert.That(emptyApplied, Is.EqualTo(0));
        Assert.That(unsupportedApplied, Is.EqualTo(0));
        Assert.That(nullElementApplied, Is.EqualTo(0));
    }

    [Test]
    public void ThemeScopeApplyDelegatesToPerControlApplyThemeForConcreteAndGenericControls()
    {
        var theme = TesseraThemes.RosePine();

        var expectedButton = new Button().ApplyTheme(theme);
        var expectedList = new ListView<int>().ApplyTheme(theme);

        var button = new Button();
        var list = new ListView<int>();

        var applied = ThemeScope.Apply(theme, button, list);

        Assert.That(applied, Is.EqualTo(2));
        Assert.That(button.LabelStyle, Is.EqualTo(expectedButton.LabelStyle));
        Assert.That(button.FocusedLabelStyle, Is.EqualTo(expectedButton.FocusedLabelStyle));
        Assert.That(button.SurfaceStyle, Is.EqualTo(expectedButton.SurfaceStyle));
        Assert.That(button.FocusedSurfaceStyle, Is.EqualTo(expectedButton.FocusedSurfaceStyle));

        Assert.That(list.DefaultRowStyle, Is.EqualTo(expectedList.DefaultRowStyle));
        Assert.That(list.HoveredRowStyle, Is.EqualTo(expectedList.HoveredRowStyle));
        Assert.That(list.SelectedRowStyle, Is.EqualTo(expectedList.SelectedRowStyle));
        Assert.That(list.FocusMarker, Is.EqualTo(expectedList.FocusMarker));
        Assert.That(list.BorderStyleText, Is.EqualTo(expectedList.BorderStyleText));
        Assert.That(list.FocusedBorderStyleText, Is.EqualTo(expectedList.FocusedBorderStyleText));
    }

    private sealed class NoThemeControl : Control
    {
        public override void Render(Canvas canvas, Rect rect)
        {
        }
    }
}
