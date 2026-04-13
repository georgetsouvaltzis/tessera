using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ProcessListViewControlTests
{
    [Test]
    public void ControlsProcessListViewRendersStatusCpuAndMemoryColumns()
    {
        var control = new ProcessListView
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            StatusHeaderText = "STATE",
            CpuHeaderText = "CPU%",
            MemoryHeaderText = "MEM"
        };
        control.SetEntries(
        [
            new ProcessListEntry(101, "tea-worker", ProcessListStatus.Running, 12.5, 128.3),
            new ProcessListEntry(222, "tea-cache", ProcessListStatus.Sleeping, 0.4, 64.0),
            new ProcessListEntry(333, "tea-sync", ProcessListStatus.Stopped, 0, 32.1)
        ]);

        var output = Render(control, 100, 8);

        Assert.That(output.Contains("STATE", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("CPU%", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("MEM", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("RUN", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("SLP", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("tea-worker", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("12.5%", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("128.3M", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsProcessListViewKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new ProcessListView { Border = BorderStyle.None, Title = string.Empty, IsFocused = true };
        control.SetEntries(
        [
            new ProcessListEntry(101, "tea-worker", ProcessListStatus.Running, 12.5, 128.3),
            new ProcessListEntry(222, "tea-cache", ProcessListStatus.Sleeping, 0.4, 64.0),
            new ProcessListEntry(333, "tea-sync", ProcessListStatus.Stopped, 0, 32.1)
        ]);

        var raised = 0;
        ProcessListSelectionChangedEventArgs? latest = null;
        control.SelectionChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var clickHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 3),
            new Rect(0, 0, 100, 8));

        Assert.That(downHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(raised, Is.GreaterThanOrEqualTo(2));
        Assert.That(latest?.SelectedEntry?.Pid, Is.EqualTo(333));
    }

    [Test]
    public void ControlsProcessListViewDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new ProcessListView { Border = BorderStyle.None, Title = string.Empty };
        control.SetEntries(
        [
            new ProcessListEntry(101, "tea-worker", ProcessListStatus.Running, 12.5, 128.3),
            new ProcessListEntry(222, "tea-cache", ProcessListStatus.Sleeping, 0.4, 64.0)
        ]);

        var first = Render(control, 100, 6);
        var second = Render(control, 100, 6);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(ProcessListView control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
