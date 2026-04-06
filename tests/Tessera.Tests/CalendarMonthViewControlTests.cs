using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class CalendarMonthViewControlTests
{
    [Test]
    public void CalendarMonthViewRenderShowsMonthWeekdayHeadersAndSelection()
    {
        var control = new CalendarMonthView
        {
            Today = new DateOnly(2026, 3, 15),
            ShowAdjacentMonthDays = true,
        };
        control.SetDisplayedMonth(new DateOnly(2026, 3, 1));
        control.SelectDate(new DateOnly(2026, 3, 15));

        var output = Render(control, width: 36, height: 10);

        Assert.That(output.Contains("March 2026", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Mo", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("[15]", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void CalendarMonthViewKeyboardNavigationChangesSelectionAndRaisesEvent()
    {
        var control = new CalendarMonthView
        {
            IsFocused = true,
        };
        control.SelectDate(new DateOnly(2026, 3, 15));
        DateOnly previous = default;
        DateOnly selected = default;
        var raised = false;
        control.DateSelected += (_, args) =>
        {
            raised = true;
            previous = args.PreviousDate;
            selected = args.SelectedDate;
        };

        var handled = control.Handle(new KeyPressed(Key.Right));

        Assert.That(handled, Is.True);
        Assert.That(raised, Is.True);
        Assert.That(previous, Is.EqualTo(new DateOnly(2026, 3, 15)));
        Assert.That(selected, Is.EqualTo(new DateOnly(2026, 3, 16)));
        Assert.That(control.SelectedDate, Is.EqualTo(new DateOnly(2026, 3, 16)));
    }

    [Test]
    public void CalendarMonthViewPointerPressSelectsDay()
    {
        var control = new CalendarMonthView
        {
            ShowAdjacentMonthDays = true,
        };
        control.SetDisplayedMonth(new DateOnly(2026, 3, 1));
        control.SelectDate(new DateOnly(2026, 3, 1));

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 17, Y: 5),
            new Rect(0, 0, 36, 10));

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedDate, Is.EqualTo(new DateOnly(2026, 3, 20)));
    }

    [Test]
    public void CalendarMonthViewFocusedTitleStyleEmitsAnsi()
    {
        var style = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(101, 202, 77));
        var control = new CalendarMonthView
        {
            IsFocused = true,
            Title = "Plan",
            FocusMarker = "!",
            FocusedTitleStyle = style,
        };
        control.SelectDate(new DateOnly(2026, 3, 15));

        var output = Render(control, width: 36, height: 10);

        Assert.That(output.Contains("38;2;101;202;77", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Plan !", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void CalendarMonthViewDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new CalendarMonthView();
        control.SetDisplayedMonth(new DateOnly(2026, 3, 1));
        control.SelectDate(new DateOnly(2026, 3, 15));
        var bounds = new Rect(0, 0, 36, 10);
        var firstCanvas = new Canvas(36, 10);
        var secondCanvas = new Canvas(36, 10);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(CalendarMonthView control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
