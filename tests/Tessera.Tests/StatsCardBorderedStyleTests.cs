using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class StatsCardBorderedStyleTests
{
    [Test]
    public void ControlsStatsCardFocusStateTransitionRendersFocusedMarkerAndMergedBorderStyles()
    {
        var control = new StatsCard
        {
            Title = "Stats",
            Border = BorderStyle.SingleLine,
            BorderStyleText = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(11, 22, 33)),
            FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 55, 66)),
            TitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            FocusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(70, 80, 90))
        };
        control.SetItems(
        [
            new StatItem("cpu", "43%"),
            new StatItem("mem", "1.2G")
        ]);

        control.IsFocused = false;
        var unfocused = Render(control, 40, 8);

        control.IsFocused = true;
        var focused = Render(control, 40, 8);

        Assert.That(unfocused.Contains("Stats *", StringComparison.Ordinal), Is.False);
        Assert.That(focused.Contains("Stats *", StringComparison.Ordinal), Is.True);
        Assert.That(unfocused.Contains("48;2;11;22;33", StringComparison.Ordinal), Is.True);
        Assert.That(unfocused.Contains("38;2;44;55;66", StringComparison.Ordinal), Is.False);
        Assert.That(focused.Contains("48;2;11;22;33", StringComparison.Ordinal), Is.True);
        Assert.That(focused.Contains("38;2;44;55;66", StringComparison.Ordinal), Is.True);
        Assert.That(focused.Contains("38;2;70;80;90", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsStatsCardPointerPressRequestsFocusInsideBounds()
    {
        var control = new StatsCard { Border = BorderStyle.None };
        control.SetItems([new StatItem("cpu", "43%")]);

        var outside = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 90, 90),
            new Rect(0, 0, 40, 6));
        var inside = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            new Rect(0, 0, 40, 6));
        var requested = control.TryConsumeFocusRequest(out var requestOrder);

        Assert.That(outside, Is.False);
        Assert.That(inside, Is.True);
        Assert.That(requested, Is.True);
        Assert.That(requestOrder, Is.GreaterThan(0));
    }

    [Test]
    public void ControlsStatsCardDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new StatsCard { Border = BorderStyle.SingleLine, Padding = new Thickness(1, 0, 1, 0) };
        control.SetItems(
        [
            new StatItem("cpu", "43%"),
            new StatItem("mem", "1.2G")
        ]);

        var first = Render(control, 40, 8);
        var second = Render(control, 40, 8);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    [Test]
    public void ThemeStatsCardApplyThemeMapsBorderAndValueTokens()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(14, 15, 16))
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
                Marker = "!"
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(23, 24, 25)),
                Focused = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(26, 27, 28))
            }
        };

        var control = new StatsCard().ApplyTheme(theme);

        Assert.That(control.TitleStyle, Is.EqualTo(theme.Text.Secondary));
        Assert.That(control.FocusedTitleStyle, Is.EqualTo(theme.Focus.Title));
        Assert.That(control.FocusMarker, Is.EqualTo(theme.Focus.Marker));
        Assert.That(control.KeyStyle, Is.EqualTo(theme.Text.Secondary));
        Assert.That(control.ValueStyle, Is.EqualTo(theme.Text.Primary));
        Assert.That(control.BorderStyleText, Is.EqualTo(theme.Border.Default));
        Assert.That(control.FocusedBorderStyleText, Is.EqualTo(theme.Border.Focused.Merge(theme.Focus.Border)));
    }

    [Test]
    public void ThemeStatsCardApplyThemeDefaultsPreservesExplicitOverridesAndFillsEmpty()
    {
        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(41, 42, 43));
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6))
            },
            Focus = new TesseraThemeFocusTokens
            {
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
                Marker = "!"
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(13, 14, 15)),
                Focused = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(16, 17, 18))
            }
        };

        var control = new StatsCard { BorderStyleText = explicitStyle, ValueStyle = explicitStyle, FocusMarker = "#" };

        control.ApplyThemeDefaults(theme);

        Assert.That(control.BorderStyleText, Is.EqualTo(explicitStyle));
        Assert.That(control.ValueStyle, Is.EqualTo(explicitStyle));
        Assert.That(control.FocusMarker, Is.EqualTo("#"));
        Assert.That(control.FocusedBorderStyleText, Is.EqualTo(theme.Border.Focused.Merge(theme.Focus.Border)));
        Assert.That(control.KeyStyle, Is.EqualTo(theme.Text.Secondary));
        Assert.That(control.TitleStyle, Is.EqualTo(theme.Text.Secondary));
    }

    private static string Render(StatsCard control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
