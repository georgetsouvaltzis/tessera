using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class PivotTableControlTests
{
    [Test]
    public void ControlsPivotTableRendersHeadersValuesAndSortMarker()
    {
        var control = CreateSortableControl();
        var sorted = control.SortByColumn(0, PivotSortDirection.Ascending);
        var output = Render(control, width: 48, height: 8);

        Assert.That(sorted, Is.True);
        Assert.That(output.Contains("Row", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("CPU ▲", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("srv-a", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("42", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsPivotTableKeyboardNavigationAndEnterSortWork()
    {
        var control = CreateSortableControl();
        control.IsFocused = true;

        var down = control.Handle(new KeyPressed(Key.Down));
        var right = control.Handle(new KeyPressed(Key.Right));
        var enter = control.Handle(new KeyPressed(Key.Enter));

        Assert.That(down, Is.True);
        Assert.That(right, Is.True);
        Assert.That(enter, Is.True);
        Assert.That(control.SelectedRowIndex, Is.EqualTo(1));
        Assert.That(control.SelectedColumnIndex, Is.EqualTo(1));
        Assert.That(control.SortColumnIndex, Is.EqualTo(1));
        Assert.That(control.SortDescending, Is.False);
        Assert.That(control.RowKeys[0], Is.EqualTo("srv-c"), "Ascending sort should move smallest memory row first.");
    }

    [Test]
    public void ControlsPivotTableSortRequestedEventAllowsExternalSortHandling()
    {
        var control = new PivotTable();
        control.SetColumns([new PivotTableColumn("latency", "Latency") { IsSortable = true }]);
        control.SetRows(["srv-a", "srv-b"]);
        control.SetCells(
        [
            new PivotTableCell("srv-a", "latency", "12"),
            new PivotTableCell("srv-b", "latency", "4"),
        ]);

        PivotSortRequestedEventArgs? captured = null;
        control.SortRequested += (_, args) =>
        {
            captured = args;
            args.Handled = true;
        };

        var result = control.SortByColumn(0, PivotSortDirection.Descending);

        Assert.That(result, Is.True);
        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.ColumnIndex, Is.EqualTo(0));
        Assert.That(captured.Direction, Is.EqualTo(PivotSortDirection.Descending));
        Assert.That(control.SortColumnIndex, Is.EqualTo(0));
        Assert.That(control.SortDescending, Is.True);
    }

    [Test]
    public void ControlsPivotTablePointerHeaderSortAndBodyClickSelectsCell()
    {
        var control = CreateSortableControl();
        control.Border = BorderStyle.SingleLine;
        var bounds = new Rect(0, 0, 50, 8);

        var headerSort = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 8, 1), bounds);
        var bodySelect = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 8, 3), bounds);

        Assert.That(headerSort, Is.True);
        Assert.That(bodySelect, Is.True);
        Assert.That(control.SortColumnIndex, Is.EqualTo(0));
        Assert.That(control.SelectedColumnIndex, Is.EqualTo(0));
        Assert.That(control.SelectedRowIndex, Is.EqualTo(1));
    }

    [Test]
    public void ControlsPivotTableDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = CreateSortableControl();

        var first = Render(control, width: 48, height: 8);
        var second = Render(control, width: 48, height: 8);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    private static PivotTable CreateSortableControl()
    {
        var control = new PivotTable
        {
            Title = "Analytics",
            Border = BorderStyle.SingleLine,
        };
        control.SetColumns(
        [
            new PivotTableColumn("cpu", "CPU")
            {
                IsSortable = true,
                SortComparer = static (left, right) => ParseNumber(left).CompareTo(ParseNumber(right)),
            },
            new PivotTableColumn("mem", "MEM")
            {
                IsSortable = true,
                SortComparer = static (left, right) => ParseNumber(left).CompareTo(ParseNumber(right)),
            },
        ]);
        control.SetRows(["srv-a", "srv-b", "srv-c"]);
        control.SetCells(
        [
            new PivotTableCell("srv-a", "cpu", "42"),
            new PivotTableCell("srv-a", "mem", "4096"),
            new PivotTableCell("srv-b", "cpu", "18"),
            new PivotTableCell("srv-b", "mem", "2048"),
            new PivotTableCell("srv-c", "cpu", "72"),
            new PivotTableCell("srv-c", "mem", "1024"),
        ]);
        return control;
    }

    private static double ParseNumber(string value)
    {
        _ = double.TryParse(value, out var result);
        return result;
    }

    private static string Render(PivotTable control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
