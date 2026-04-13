using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TableSelectionApiTests
{
    [Test]
    public void ControlsTableSelectionApisAreSafeWhenNoSelectionExists()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };
        table.SetRows(
        [
            ["alpha"],
            ["beta"]
        ]);

        var hasSelectedRow = table.TryGetSelectedRow(out var selectedRow);

        Assert.That(table.SelectedRowIndex, Is.EqualTo(-1));
        Assert.That(table.SelectedRow, Is.Null);
        Assert.That(hasSelectedRow, Is.False);
        Assert.That(selectedRow, Is.Null);
    }

    [Test]
    public void ControlsTablePointerSelectionUpdatesPublicSelectionStateAndRaisesEvent()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };
        table.SetRows(
        [
            ["alpha"],
            ["beta"],
            ["gamma"]
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
    public void ControlsTableReSelectingSameRowDoesNotRaiseSelectionChangedAgain()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };
        table.SetRows(
        [
            ["alpha"],
            ["beta"]
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
    public void ControlsTableClearRowsResetsSelectionApis()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };
        table.SetRows(
        [
            ["alpha"],
            ["beta"]
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

    [Test]
    public void ControlsTableSetSelectedIndexSelectsRowAndRaisesEvent()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };
        table.SetRows(
        [
            ["alpha"],
            ["beta"],
            ["gamma"]
        ]);

        ListSelectionChangedEventArgs<IReadOnlyList<string>>? latest = null;
        var raised = 0;
        table.SelectionChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var changed = table.SetSelectedIndex(1);
        var hasSelectedRow = table.TryGetSelectedRow(out var selectedRow);

        Assert.That(changed, Is.True);
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
    }

    [Test]
    public void ControlsTableSetSelectedIndexClampsAndNoopsWhenUnchanged()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };
        table.SetRows(
        [
            ["alpha"],
            ["beta"]
        ]);

        var raised = 0;
        table.SelectionChanged += (_, _) => raised++;

        var first = table.SetSelectedIndex(999);
        var second = table.SetSelectedIndex(999);

        Assert.That(first, Is.True);
        Assert.That(second, Is.False);
        Assert.That(raised, Is.EqualTo(1));
        Assert.That(table.SelectedRowIndex, Is.EqualTo(1));
        Assert.That(table.SelectedRow, Is.Not.Null);
        Assert.That(table.SelectedRow![0], Is.EqualTo("beta"));
    }

    [Test]
    public void ControlsTableSetSelectedIndexReturnsFalseWhenNoVisibleRows()
    {
        var table = new Table("Name") { Border = BorderStyle.SingleLine };

        var raised = 0;
        table.SelectionChanged += (_, _) => raised++;

        var changed = table.SetSelectedIndex(0);

        Assert.That(changed, Is.False);
        Assert.That(raised, Is.EqualTo(0));
        Assert.That(table.SelectedRowIndex, Is.EqualTo(-1));
        Assert.That(table.SelectedRow, Is.Null);
    }
}
