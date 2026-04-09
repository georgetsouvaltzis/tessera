using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ThemeOverrideBundleApiErgonomicsTests
{
    [Test]
    public void ThemeApiErgonomicsCreateDashboardBundleComputesExpectedStyles()
    {
        var theme = TesseraThemes.Catppuccin(CatppuccinVariant.Macchiato);

        var bundle = TesseraThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");

        Assert.That(bundle.Theme, Is.SameAs(theme));
        Assert.That(bundle.FocusMarker, Is.EqualTo("◆"));
        Assert.That(bundle.BorderStyleText, Is.EqualTo(theme.Border.Strong));
        Assert.That(bundle.FocusedBorderStyleText, Is.EqualTo(theme.Border.Focused.Merge(theme.Focus.Border)));
        Assert.That(bundle.TitleStyle, Is.EqualTo(theme.Accent.Primary.WithBold()));
        Assert.That(bundle.FocusedTitleStyle, Is.EqualTo(theme.Focus.Title.WithBold()));
        Assert.That(bundle.DefaultItemStyle, Is.EqualTo(theme.Text.Secondary));
        Assert.That(bundle.HeaderStyle, Is.EqualTo(theme.Text.Primary.WithBold()));
        Assert.That(bundle.HoveredItemStyle, Is.EqualTo(theme.Accent.Secondary.WithUnderline()));
        Assert.That(bundle.UnreadItemStyle, Is.EqualTo(theme.Text.Primary.WithBold()));
    }

    [Test]
    public void ThemeApiErgonomicsListViewTableNotificationsLogViewApplyDashboardOverridesSetExpectedProperties()
    {
        var theme = TesseraThemes.RosePine(RosePineVariant.Moon);
        var bundle = TesseraThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");

        var listView = new ListView<string>(static value => value);
        var table = new Table("Name");
        var notifications = new Notifications();
        var logView = new LogView();
        var button = new Button();
        var dialog = new Dialog();

        listView.ApplyDashboardOverrides(bundle);
        table.ApplyDashboardOverrides(bundle);
        notifications.ApplyDashboardOverrides(bundle);
        logView.ApplyDashboardOverrides(bundle);
        button.ApplyDashboardOverrides(bundle);
        dialog.ApplyDashboardOverrides(bundle);

        Assert.That(listView.FocusMarker, Is.EqualTo(bundle.FocusMarker));
        Assert.That(listView.TitleStyle, Is.EqualTo(bundle.TitleStyle));
        Assert.That(listView.FocusedTitleStyle, Is.EqualTo(bundle.FocusedTitleStyle));
        Assert.That(listView.BorderStyleText, Is.EqualTo(bundle.BorderStyleText));
        Assert.That(listView.FocusedBorderStyleText, Is.EqualTo(bundle.FocusedBorderStyleText));
        Assert.That(listView.DefaultRowStyle, Is.EqualTo(bundle.DefaultItemStyle));
        Assert.That(listView.SelectedRowStyle, Is.EqualTo(bundle.SelectedItemStyle));
        Assert.That(listView.HoveredRowStyle, Is.EqualTo(bundle.HoveredItemStyle));

        Assert.That(table.FocusMarker, Is.EqualTo(bundle.FocusMarker));
        Assert.That(table.TitleStyle, Is.EqualTo(bundle.TitleStyle));
        Assert.That(table.FocusedTitleStyle, Is.EqualTo(bundle.FocusedTitleStyle));
        Assert.That(table.BorderStyleText, Is.EqualTo(bundle.BorderStyleText));
        Assert.That(table.FocusedBorderStyleText, Is.EqualTo(bundle.FocusedBorderStyleText));
        Assert.That(table.HeaderStyle, Is.EqualTo(bundle.HeaderStyle));
        Assert.That(table.RowStyle, Is.EqualTo(bundle.DefaultItemStyle));
        Assert.That(table.SelectedRowStyle, Is.EqualTo(bundle.SelectedItemStyle));
        Assert.That(table.HoveredRowStyle, Is.EqualTo(bundle.HoveredItemStyle));

        Assert.That(notifications.FocusMarker, Is.EqualTo(bundle.FocusMarker));
        Assert.That(notifications.BorderStyleText, Is.EqualTo(bundle.BorderStyleText));
        Assert.That(notifications.FocusedBorderStyleText, Is.EqualTo(bundle.FocusedBorderStyleText));
        Assert.That(notifications.SelectedItemStyle, Is.EqualTo(bundle.SelectedItemStyle));
        Assert.That(notifications.HoveredItemStyle, Is.EqualTo(bundle.HoveredItemStyle));
        Assert.That(notifications.UnreadItemStyle, Is.EqualTo(bundle.UnreadItemStyle));

        Assert.That(logView.FocusMarker, Is.EqualTo(bundle.FocusMarker));
        Assert.That(logView.BorderStyleText, Is.EqualTo(bundle.BorderStyleText));
        Assert.That(logView.FocusedBorderStyleText, Is.EqualTo(bundle.FocusedBorderStyleText));
        Assert.That(logView.EntryStyle, Is.EqualTo(bundle.EntryTextStyle));

        Assert.That(button.LabelStyle, Is.EqualTo(bundle.ActionLabelStyle));
        Assert.That(button.FocusedLabelStyle, Is.EqualTo(bundle.FocusedActionLabelStyle));
        Assert.That(button.PressedLabelStyle, Is.EqualTo(bundle.PressedActionLabelStyle));
        Assert.That(button.SurfaceStyle, Is.EqualTo(bundle.ActionSurfaceStyle));
        Assert.That(button.FocusedSurfaceStyle, Is.EqualTo(bundle.FocusedActionSurfaceStyle));
        Assert.That(button.PressedSurfaceStyle, Is.EqualTo(bundle.PressedActionSurfaceStyle));

        Assert.That(dialog.FocusMarker, Is.EqualTo(bundle.FocusMarker));
        Assert.That(dialog.BorderStyleText, Is.EqualTo(bundle.BorderStyleText));
        Assert.That(dialog.FocusedBorderStyleText, Is.EqualTo(bundle.FocusedBorderStyleText));
        Assert.That(dialog.BodyTextStyle, Is.EqualTo(bundle.BodyTextStyle));
    }

    [Test]
    public void ThemeApiErgonomicsButtonDashboardOverridesLeaveButtonsBorderless()
    {
        var theme = TesseraThemes.Catppuccin(CatppuccinVariant.Macchiato);
        var bundle = TesseraThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");
        var button = new Button
        {
            Text = "Launch",
        };

        button.ApplyDashboardOverrides(bundle);

        Assert.That(button.Padding, Is.EqualTo(Thickness.Symmetric(1, 0)));
    }

    [Test]
    public void ThemePublicApiDashboardTableBundleApplyIsDeterministicAcrossRepeatedCalls()
    {
        var theme = TesseraThemes.Catppuccin(CatppuccinVariant.Macchiato);
        var bundle = TesseraThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");
        var table = new Table("Service", "State")
        {
            Border = BorderStyle.Rounded,
            Title = "Metrics",
            IsFocused = true,
        };
        table.SetRows(
        [
            ["api", "Healthy"],
            ["worker", "Warning"],
        ]);

        table.ApplyThemeAndDashboardOverrides(bundle);
        var first = Render(table, width: 64, height: 10);

        table.ApplyThemeAndDashboardOverrides(bundle);
        var second = Render(table, width: 64, height: 10);

        Assert.That(second, Is.EqualTo(first));
    }

    private static string Render(Table table, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        table.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
