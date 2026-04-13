using NUnit.Framework;
using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TraceViewerControlTests
{
    [Test]
    public void ControlsTraceViewerRendersSortedRowsSeverityAndDuration()
    {
        var day = new DateTimeOffset(2026, 3, 21, 9, 0, 0, TimeSpan.Zero);
        var control = new TraceViewer { Border = BorderStyle.None, TimeFormat = "HH:mm:ss" };
        control.SetEntries(
        [
            new TraceEntry("b", day.AddMinutes(2), "Auth", "token validated", TraceSeverity.Info, 2.3),
            new TraceEntry("a", day.AddMinutes(1), "Gateway", "request accepted", TraceSeverity.Warning, 1.2),
            new TraceEntry("c", day.AddMinutes(3), "Db", "timeout", TraceSeverity.Error, 14.8)
        ]);

        var output = Render(control, 96, 8);
        var onePointTwo = 1.2.ToString("0.##", CultureInfo.CurrentCulture);
        var twoPointThree = 2.3.ToString("0.##", CultureInfo.CurrentCulture);
        var fourteenPointEight = 14.8.ToString("0.##", CultureInfo.CurrentCulture);

        var first = output.IndexOf($"09:01:00 WRN Gateway: request accepted ({onePointTwo}ms)",
            StringComparison.Ordinal);
        var second = output.IndexOf($"09:02:00 INF Auth: token validated ({twoPointThree}ms)",
            StringComparison.Ordinal);
        var third = output.IndexOf($"09:03:00 ERR Db: timeout ({fourteenPointEight}ms)", StringComparison.Ordinal);
        Assert.That(first >= 0, Is.True);
        Assert.That(second > first, Is.True);
        Assert.That(third > second, Is.True);
    }

    [Test]
    public void ControlsTraceViewerKeyboardNavigationRaisesSelectionChanged()
    {
        var control = new TraceViewer { IsFocused = true, Border = BorderStyle.None };
        control.SetEntries(CreateEntries());
        TraceSelectionChangedEventArgs? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var down = control.Handle(new KeyPressed(Key.Down));
        var end = control.Handle(new KeyPressed(Key.End));
        var up = control.Handle(new KeyPressed(Key.Up));

        Assert.That(down, Is.True);
        Assert.That(end, Is.True);
        Assert.That(up, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(args, Is.Not.Null);
        Assert.That(TestAssert.NotNull(args).SelectedIndex, Is.EqualTo(1));
        Assert.That(args.SelectedEntry?.Operation, Is.EqualTo("Auth"));
    }

    [Test]
    public void ControlsTraceViewerPointerHoverClickAndWheelUpdateSelection()
    {
        var control = new TraceViewer { Border = BorderStyle.SingleLine };
        control.SetEntries(CreateEntries());
        var bounds = new Rect(0, 0, 96, 8);

        var motion = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 4, 2), bounds);
        var click = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 4, 2), bounds);
        var wheel = control.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, 4, 2), bounds);

        Assert.That(motion, Is.True);
        Assert.That(click, Is.True);
        Assert.That(wheel, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedEntry?.Operation, Is.EqualTo("Db"));
    }

    [Test]
    public void ControlsTraceViewerSeverityAndStateStylesEmitAnsi()
    {
        var control = new TraceViewer
        {
            Border = BorderStyle.None,
            IsFocused = true,
            EntryStyle = TesseraStyle.Empty.WithItalic(),
            WarningRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
            ErrorRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 102, 103)),
            SelectedRowStyle = TesseraStyle.Empty.WithBold(),
            FocusedSelectedRowStyle = TesseraStyle.Empty.WithUnderline(),
            HoveredRowStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(10, 20, 30))
        };
        control.SetEntries(CreateEntries());
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 1), new Rect(0, 0, 96, 6));

        var output = Render(control, 96, 6);
        Assert.That(output.Contains("38;2;91;92;93", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;101;102;103", StringComparison.Ordinal), Is.True);
        Assert.That(
            output.Contains("[1;", StringComparison.Ordinal)
            || output.Contains(";1;", StringComparison.Ordinal)
            || output.Contains("[1m", StringComparison.Ordinal),
            Is.True);
        Assert.That(
            output.Contains(";4;", StringComparison.Ordinal) || output.Contains("[4m", StringComparison.Ordinal),
            Is.True);
    }

    [Test]
    public void ControlsTraceViewerDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new TraceViewer { Border = BorderStyle.None };
        control.SetEntries(CreateEntries());

        var first = Render(control, 96, 6);
        var second = Render(control, 96, 6);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static IReadOnlyList<TraceEntry> CreateEntries()
    {
        var day = new DateTimeOffset(2026, 3, 21, 9, 0, 0, TimeSpan.Zero);
        return
        [
            new TraceEntry("a", day.AddMinutes(1), "Gateway", "request accepted", TraceSeverity.Warning, 1.2),
            new TraceEntry("b", day.AddMinutes(2), "Auth", "token validated", TraceSeverity.Info, 2.3),
            new TraceEntry("c", day.AddMinutes(3), "Db", "timeout", TraceSeverity.Error, 14.8)
        ];
    }

    private static string Render(TraceViewer control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
