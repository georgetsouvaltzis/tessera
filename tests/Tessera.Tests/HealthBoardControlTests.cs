using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class HealthBoardControlTests
{
    [Test]
    public void ControlsHealthBoardRendersSeverityGlyphsAndSummary()
    {
        var control = new HealthBoard
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.SetServices(
        [
            new HealthService("svc-auth", "Auth", HealthServiceSeverity.Healthy),
            new HealthService("svc-cache", "Cache", HealthServiceSeverity.Degraded, "latency high"),
            new HealthService("svc-api", "Api", HealthServiceSeverity.Outage, "timeout"),
        ]);

        var output = Render(control, width: 80, height: 4);

        Assert.That(output.Contains("OK Auth", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("~ Cache - latency high", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("! Api - timeout", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsHealthBoardKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new HealthBoard
        {
            Border = BorderStyle.None,
            IsFocused = true,
            Title = string.Empty,
        };
        control.SetServices(
        [
            new HealthService("svc-a", "A"),
            new HealthService("svc-b", "B"),
            new HealthService("svc-c", "C"),
        ]);

        var raised = 0;
        ListSelectionChangedEventArgs<HealthService>? latest = null;
        control.SelectionChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var clickHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 2),
            new Rect(0, 0, 80, 4));

        Assert.That(downHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(raised, Is.GreaterThanOrEqualTo(2));
        Assert.That(latest?.SelectedItem?.Id, Is.EqualTo("svc-c"));
    }

    [Test]
    public void ControlsHealthBoardAcknowledgeChangesExpectedRow()
    {
        var control = new HealthBoard();
        control.SetServices(
        [
            new HealthService("svc-a", "Auth", HealthServiceSeverity.Healthy),
            new HealthService("svc-b", "Billing", HealthServiceSeverity.Outage),
        ]);

        var first = control.Acknowledge("svc-b");
        var second = control.Acknowledge("svc-b");
        var missing = control.Acknowledge("svc-missing");

        Assert.That(first, Is.True);
        Assert.That(second, Is.False);
        Assert.That(missing, Is.False);
        Assert.That(control.Services[1].IsAcknowledged, Is.True);
    }

    [Test]
    public void ControlsHealthBoardStyleAndGlyphHooksRenderExpectedAnsiAndCustomMarkers()
    {
        var control = new HealthBoard
        {
            IsFocused = true,
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60)),
            FocusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(70, 80, 90)),
            DegradedServiceStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(100, 110, 120)),
            OutageServiceStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(130, 140, 150)),
            SelectedServiceStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(160, 170, 180)),
        };
        control.Glyphs = new HealthBoardGlyphSet(".", ">", "?", "+", "~", "#", "ack", "|");
        control.SetServices(
        [
            new HealthService("svc-auth", "Auth", HealthServiceSeverity.Degraded, "slow"),
            new HealthService("svc-payments", "Payments", HealthServiceSeverity.Outage)
            {
                IsAcknowledged = true,
            },
        ]);
        _ = control.SetSelectedIndex(1);

        var output = Render(control, width: 80, height: 8);

        Assert.That(output.Contains("Health !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(".|~|Auth - slow", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(">|#|Payments [ack]", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;40;50;60", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;70;80;90", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;100;110;120", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;130;140;150", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;160;170;180", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsHealthBoardDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new HealthBoard
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.SetServices([new HealthService("svc-auth", "Auth")]);

        var first = Render(control, width: 48, height: 3);
        var second = Render(control, width: 48, height: 3);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(HealthBoard control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
