using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class QuickOpenOverlayControlTests
{
    [Test]
    public void ControlsQuickOpenOverlayKeyboardFilterAndSubmitRaisesSubmitted()
    {
        var overlay = CreateOverlay();
        overlay.SetItems(
        [
            new QuickOpenItem("readme", "README.md", "docs"),
            new QuickOpenItem("roadmap", "widget-roadmap.md", "docs"),
            new QuickOpenItem("build", "Tessera.slnx", "repo")
        ]);
        overlay.Open();

        QuickOpenOverlaySubmittedEventArgs? submitted = null;
        overlay.Submitted += (_, args) => submitted = args;

        Assert.That(overlay.Handle(new KeyPressed(Key.Character, "r")), Is.True);
        Assert.That(overlay.Query, Is.EqualTo("r"));
        Assert.That(overlay.Handle(new KeyPressed(Key.Down)), Is.True);
        Assert.That(overlay.Handle(new KeyPressed(Key.Enter)), Is.True);

        Assert.That(submitted, Is.Not.Null);
        Assert.That(submitted!.ItemId, Is.EqualTo("roadmap"));
        Assert.That(submitted.Query, Is.EqualTo("r"));
        Assert.That(overlay.IsOpen, Is.False);
    }

    [Test]
    public void ControlsQuickOpenOverlayEscapeCancelsAndCloses()
    {
        var overlay = CreateOverlay();
        overlay.SetItems([new QuickOpenItem("readme", "README.md")]);
        overlay.Open();

        var cancelled = 0;
        overlay.Cancelled += (_, _) => cancelled++;

        var handled = overlay.Handle(new KeyPressed(Key.Escape));

        Assert.That(handled, Is.True);
        Assert.That(cancelled, Is.EqualTo(1));
        Assert.That(overlay.IsOpen, Is.False);
    }

    [Test]
    public void ControlsQuickOpenOverlayPointerPressSelectsAndSubmits()
    {
        var overlay = CreateOverlay();
        overlay.SetItems(
        [
            new QuickOpenItem("readme", "README.md", "docs"),
            new QuickOpenItem("roadmap", "widget-roadmap.md", "docs")
        ]);
        overlay.Open();

        QuickOpenOverlaySubmittedEventArgs? submitted = null;
        overlay.Submitted += (_, args) => submitted = args;

        var handled = overlay.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 20, 7),
            new Rect(0, 0, 80, 24));

        Assert.That(handled, Is.True);
        Assert.That(submitted, Is.Not.Null);
        Assert.That(submitted!.ItemId, Is.EqualTo("roadmap"));
        Assert.That(overlay.IsOpen, Is.False);
    }

    [Test]
    public void ControlsQuickOpenOverlayStyleAndGlyphHooksRenderExpectedAnsi()
    {
        var overlay = CreateOverlay();
        overlay.Title = "Open";
        overlay.FocusMarker = "!";
        overlay.BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30));
        overlay.FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60));
        overlay.FocusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(70, 80, 90));
        overlay.SelectedItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(100, 110, 120));
        overlay.Glyphs = new QuickOpenOverlayGlyphSet("?", ".", "▶", "▹", "*", "|");
        overlay.SetItems([new QuickOpenItem("readme", "README.md", "docs")]);
        overlay.SetQuery("read");
        overlay.Open();

        var canvas = new Canvas(80, 24, CanvasTextMode.GraphemeAware);
        overlay.Render(canvas, new Rect(0, 0, 80, 24));
        var output = canvas.Render();

        Assert.That(output.Contains("Open !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('?'), Is.True);
        Assert.That(output.Contains('▶'), Is.True);
        Assert.That(output.Contains('*'), Is.True);
        Assert.That(output.Contains("README.md - docs", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;40;50;60", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;70;80;90", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;100;110;120", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsQuickOpenOverlayDefaultRenderIsDeterministicAndMonochrome()
    {
        var overlay = CreateOverlay();
        overlay.SetItems([new QuickOpenItem("readme", "README.md", "docs")]);
        overlay.Open();

        var first = Render(overlay);
        var second = Render(overlay);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static QuickOpenOverlay CreateOverlay()
    {
        return new QuickOpenOverlay { IsFocused = true };
    }

    private static string Render(QuickOpenOverlay overlay)
    {
        var canvas = new Canvas(80, 24, CanvasTextMode.GraphemeAware);
        overlay.Render(canvas, new Rect(0, 0, 80, 24));
        return canvas.Render();
    }
}
