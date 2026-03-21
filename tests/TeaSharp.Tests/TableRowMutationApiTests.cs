using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TableRowMutationApiTests
{
    [Test]
    public void Controls_Table_AddRow_AppendsRow()
    {
        var table = new Table("Service", "State")
        {
            Border = BorderStyle.None,
        };
        table.SetRows(
        [
            ["api", "healthy"],
        ]);

        table.AddRow(["worker", "warning"]);

        var output = Render(table, width: 48, height: 8);
        Assert.That(output.Contains("api", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("worker", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_Table_ReplaceRow_UpdatesRowAndValidatesArguments()
    {
        var table = new Table("Service", "State")
        {
            Border = BorderStyle.None,
        };
        table.SetRows(
        [
            ["api", "healthy"],
            ["worker", "healthy"],
        ]);

        table.ReplaceRow(1, ["worker", "degraded"]);

        var output = Render(table, width: 48, height: 8);
        Assert.That(output.Contains("degraded", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("worker│healthy", StringComparison.Ordinal), Is.False);

        Assert.That(
            () => table.ReplaceRow(-1, ["x", "y"]),
            Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(
            () => table.ReplaceRow(99, ["x", "y"]),
            Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(
            () => table.ReplaceRow(0, null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Controls_Table_RemoveRowAt_RemovesRowAndNormalizesPage()
    {
        var table = new Table("Name")
        {
            Border = BorderStyle.None,
            IsFocused = true,
            PageSize = 2,
        };
        table.SetRows(
        [
            ["row-0"],
            ["row-1"],
            ["row-2"],
            ["row-3"],
            ["row-4"],
        ]);

        _ = table.Handle(new KeyPressed(Key.Character, "]"));
        _ = table.Handle(new KeyPressed(Key.Character, "]"));
        Assert.That(table.PageIndex, Is.EqualTo(2), "Expected to be on third page before mutation.");

        table.RemoveRowAt(4);

        Assert.That(table.PageIndex, Is.EqualTo(1), "Page index should clamp after row removal.");
        var output = Render(table, width: 32, height: 8);
        Assert.That(output.Contains("row-2", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("row-3", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("row-4", StringComparison.Ordinal), Is.False);

        Assert.That(() => table.RemoveRowAt(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => table.RemoveRowAt(99), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Controls_Table_ClearRows_ClearsAndKeepsPointerHandlingSafe()
    {
        var table = new Table("Name")
        {
            Border = BorderStyle.SingleLine,
            PageSize = 2,
        };
        table.SetRows(
        [
            ["row-0"],
            ["row-1"],
            ["row-2"],
        ]);
        var bounds = new Rect(0, 0, 40, 10);

        _ = table.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 4), bounds);
        _ = table.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 4), bounds);

        table.ClearRows();

        Assert.That(table.PageIndex, Is.EqualTo(0), "Clearing should reset page to the first page.");
        var pointerHandled = table.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 4), bounds);
        Assert.That(pointerHandled, Is.False, "Pointer input should be ignored when no rows exist.");

        var canvas = new Canvas(40, 10, CanvasTextMode.GraphemeAware);
        Assert.DoesNotThrow(() => table.Render(canvas, bounds));
        var output = canvas.Render();
        Assert.That(output.Contains("row-0", StringComparison.Ordinal), Is.False);
        Assert.That(output.Contains("row-1", StringComparison.Ordinal), Is.False);
        Assert.That(output.Contains("row-2", StringComparison.Ordinal), Is.False);
    }

    [Test]
    public void Controls_Table_AddRow_ValidatesArguments()
    {
        var table = new Table("Name");
        Assert.That(() => table.AddRow(null!), Throws.TypeOf<ArgumentNullException>());
    }

    private static string Render(Table table, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        table.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
