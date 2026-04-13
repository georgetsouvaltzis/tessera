using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SchedulerTimelineControlTests
{
    [Test]
    public void SchedulerTimelineRenderSortsEntriesAndShowsDuration()
    {
        var day = new DateTimeOffset(2026, 3, 21, 0, 0, 0, TimeSpan.Zero);
        var control = new SchedulerTimeline();
        control.SetEntries(
        [
            new SchedulerEntry("b", "Standup", day.AddHours(9), day.AddHours(10)),
            new SchedulerEntry("a", "Prep", day.AddHours(8), day.AddHours(8).AddMinutes(30))
        ]);

        var output = Render(control, 72, 8);

        var firstIndex = output.IndexOf("08:00-08:30", StringComparison.Ordinal);
        var secondIndex = output.IndexOf("09:00-10:00", StringComparison.Ordinal);
        Assert.That(firstIndex >= 0, Is.True);
        Assert.That(secondIndex > firstIndex, Is.True);
        Assert.That(output.Contains("(30m)", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void SchedulerTimelineKeyboardSelectionRaisesEvent()
    {
        var day = new DateTimeOffset(2026, 3, 21, 0, 0, 0, TimeSpan.Zero);
        var control = new SchedulerTimeline { IsFocused = true };
        control.SetEntries(
        [
            new SchedulerEntry("a", "Prep", day.AddHours(8), day.AddHours(8).AddMinutes(30)),
            new SchedulerEntry("b", "Standup", day.AddHours(9), day.AddHours(10)),
            new SchedulerEntry("c", "Review", day.AddHours(11), day.AddHours(12))
        ]);

        var raised = false;
        var previousIndex = -1;
        var selectedIndex = -1;
        control.SelectionChanged += (_, args) =>
        {
            raised = true;
            previousIndex = args.PreviousIndex;
            selectedIndex = args.SelectedIndex;
        };

        var handled = control.Handle(new KeyPressed(Key.Down));

        Assert.That(handled, Is.True);
        Assert.That(raised, Is.True);
        Assert.That(previousIndex, Is.EqualTo(0));
        Assert.That(selectedIndex, Is.EqualTo(1));
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void SchedulerTimelinePointerSelectsTargetRowAndRendersConflictMarker()
    {
        var day = new DateTimeOffset(2026, 3, 21, 0, 0, 0, TimeSpan.Zero);
        var control = new SchedulerTimeline();
        control.SetEntries(
        [
            new SchedulerEntry("a", "Deploy", day.AddHours(9), day.AddHours(10)),
            new SchedulerEntry("b", "Overlap", day.AddHours(9).AddMinutes(30), day.AddHours(10).AddMinutes(30))
        ]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 8, 2),
            new Rect(0, 0, 72, 8));
        var output = Render(control, 72, 8);

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(output.Contains('!'), Is.True);
    }

    [Test]
    public void SchedulerTimelineSelectedRowStyleEmitsAnsi()
    {
        var day = new DateTimeOffset(2026, 3, 21, 0, 0, 0, TimeSpan.Zero);
        var control = new SchedulerTimeline
        {
            SelectedRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 120, 210))
        };
        control.SetEntries(
        [
            new SchedulerEntry("a", "Prep", day.AddHours(8), day.AddHours(8).AddMinutes(30)),
            new SchedulerEntry("b", "Standup", day.AddHours(9), day.AddHours(10))
        ]);
        control.Select(1);

        var output = Render(control, 72, 8);

        Assert.That(output.Contains("38;2;90;120;210", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void SchedulerTimelineDefaultRenderIsDeterministicAndMonochrome()
    {
        var day = new DateTimeOffset(2026, 3, 21, 0, 0, 0, TimeSpan.Zero);
        var control = new SchedulerTimeline();
        control.SetEntries(
        [
            new SchedulerEntry("a", "Prep", day.AddHours(8), day.AddHours(8).AddMinutes(30)),
            new SchedulerEntry("b", "Standup", day.AddHours(9), day.AddHours(10))
        ]);
        var bounds = new Rect(0, 0, 72, 8);
        var firstCanvas = new Canvas(72, 8);
        var secondCanvas = new Canvas(72, 8);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(SchedulerTimeline control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
