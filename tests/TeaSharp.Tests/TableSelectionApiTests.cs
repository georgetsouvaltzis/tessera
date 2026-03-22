using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TableSelectionApiTests
{
    [Test]
    public void Controls_Table_SelectionApis_AreSafeWhenNoSelectionExists()
    {
        var table = new Table("Name")
        {
            Border = BorderStyle.SingleLine,
        };
        table.SetRows(
        [
            ["alpha"],
            ["beta"],
        ]);

        var hasSelectedRow = table.TryGetSelectedRow(out var selectedRow);

        Assert.That(table.SelectedRowIndex, Is.EqualTo(-1));
        Assert.That(table.SelectedRow, Is.Null);
        Assert.That(hasSelectedRow, Is.False);
        Assert.That(selectedRow, Is.Null);
    }

    [Test]
    public void Controls_Table_PointerSelection_UpdatesPublicSelectionState_AndRaisesEvent()
    {
        var table = new Table("Name")
        {
            Border = BorderStyle.SingleLine,
        };
        table.SetRows(
        [
            ["alpha"],
            ["beta"],
            ["gamma"],
        ]);

        ListSelectionChangedEventArgs<IReadOnlyList<string>>? latest = null;
        var raised = 0;
        table.SelectionChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var handled = table.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 4),
            new Rect(0, 0, 40, 10));
        var hasSelectedRow = table.TryGetSelectedRow(out var selectedRow);

        Assert.That(handled, Is.True);
        Assert.That(raised, Is.EqualTo(1));
        Assert.That(table.SelectedRowIndex, Is.EqualTo(1));
        Assert.That(table.SelectedRow, Is.Not.Null);
        Assert.That(table.SelectedRow![0], Is.EqualTo("beta"));
        Assert.That(hasSelectedRow, Is.True);
        Assert.That(selectedRow, Is.Not.Null);
        Assert.That(selectedRow![0], Is.EqualTo("beta"));
        Assert.That(latest, Is.Not.Null);
        Assert.That(latest!.PreviousIndex, Is.EqualTo(-1));
        Assert.That(latest.SelectedIndex, Is.EqualTo(1));
        Assert.That(latest.PreviousItem, Is.Null);
        Assert.That(latest.SelectedItem, Is.Not.Null);
        Assert.That(latest.SelectedItem![0], Is.EqualTo("beta"));
    }

    [Test]
    public void Controls_Table_ReSelectingSameRow_DoesNotRaiseSelectionChangedAgain()
    {
        var table = new Table("Name")
        {
            Border = BorderStyle.SingleLine,
        };
        table.SetRows(
        [
            ["alpha"],
            ["beta"],
        ]);

        var raised = 0;
        table.SelectionChanged += (_, _) => raised++;
        var bounds = new Rect(0, 0, 40, 10);

        var first = table.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 4), bounds);
        var second = table.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 4), bounds);

        Assert.That(first, Is.True);
        Assert.That(second, Is.False);
        Assert.That(raised, Is.EqualTo(1));
    }

    [Test]
    public void Controls_Table_ClearRows_ResetsSelectionApis()
    {
        var table = new Table("Name")
        {
            Border = BorderStyle.SingleLine,
        };
        table.SetRows(
        [
            ["alpha"],
            ["beta"],
        ]);

        _ = table.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 4),
            new Rect(0, 0, 40, 10));
        table.ClearRows();

        var hasSelectedRow = table.TryGetSelectedRow(out var selectedRow);

        Assert.That(table.SelectedRowIndex, Is.EqualTo(-1));
        Assert.That(table.SelectedRow, Is.Null);
        Assert.That(hasSelectedRow, Is.False);
        Assert.That(selectedRow, Is.Null);
    }
}
