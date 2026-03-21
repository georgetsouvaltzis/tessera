using NUnit.Framework;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SelectionMutatorNormalizationTests
{
    [Test]
    public void Selection_SchedulerTimeline_SetSelectedIndex_AndSelectWrapper_Work()
    {
        var control = new SchedulerTimeline();
        control.SetEntries(CreateSchedulerEntries());

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.Select(0), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));

        Assert.That(control.SetSelectedIndex(99), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
    }

    [Test]
    public void Selection_Timeline_SetSelectedIndex_AndSelectWrapper_Work()
    {
        var control = new Timeline();
        control.SetEntries(CreateTimelineEntries());

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.Select(1), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));

        Assert.That(control.SetSelectedIndex(-10), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void Selection_TraceViewer_SetSelectedIndex_AndSelectWrapper_Work()
    {
        var control = new TraceViewer();
        control.SetEntries(CreateTraceEntries());

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.Select(1), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));

        Assert.That(control.SetSelectedIndex(999), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
    }

    [Test]
    public void Selection_ProcessListView_SetSelectedIndex_AndSelectWrapper_Work()
    {
        var control = new ProcessListView();
        control.SetEntries(CreateProcessEntries());

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.Select(1), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));

        Assert.That(control.SetSelectedIndex(-5), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void Selection_PaletteEditor_SetSelectedIndex_AndSelectWrapper_Work()
    {
        var control = new PaletteEditor();
        control.SetSwatches(CreateSwatches());

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.Select(1), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));

        Assert.That(control.SetSelectedIndex(999), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
    }

    [Test]
    public void Selection_Tabs_SetSelectedIndex_IsCanonical_AndSelectForwards()
    {
        var tabs = new Tabs("Overview", "Logs", "Metrics");

        Assert.That(tabs.SetSelectedIndex(99), Is.True);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(2));

        tabs.Select(0);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(0));

        var empty = new Tabs(Array.Empty<string>());
        empty.Select(5);
        Assert.That(empty.SelectedIndex, Is.EqualTo(0));
        Assert.That(empty.SetSelectedIndex(0), Is.False);
    }

    [Test]
    public void Selection_Toolbar_SetSelectedIndex_UsesExistingClampingSemantics()
    {
        var control = new Toolbar();
        control.SetItems(
        [
            new ToolbarItem("a", "A"),
            new ToolbarItem("b", "B"),
            new ToolbarItem("c", "C"),
        ]);

        Assert.That(control.SetSelectedIndex(99), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.SetSelectedIndex(-50), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void Selection_PaneTabs_SetSelectedIndex_PreservesDisabledAndRangeRules()
    {
        var control = new PaneTabs();
        control.SetTabs(
        [
            new PaneTabItem("home", "Home"),
            new PaneTabItem("diag", "Diagnostics", isDisabled: true),
            new PaneTabItem("ops", "Operations"),
        ]);

        Assert.That(control.SetSelectedIndex(1), Is.False);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));

        Assert.That(control.SetSelectedIndex(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));

        Assert.That(control.SetSelectedIndex(99), Is.False);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
    }

    private static IReadOnlyList<SchedulerEntry> CreateSchedulerEntries()
    {
        var start = DateTimeOffset.UtcNow;
        return
        [
            new SchedulerEntry("a", "A", start, start.AddMinutes(5)),
            new SchedulerEntry("b", "B", start.AddMinutes(10), start.AddMinutes(15)),
            new SchedulerEntry("c", "C", start.AddMinutes(20), start.AddMinutes(25)),
        ];
    }

    private static IReadOnlyList<TimelineEntry> CreateTimelineEntries()
    {
        return
        [
            new TimelineEntry("a", "A", "10:00"),
            new TimelineEntry("b", "B", "10:05"),
            new TimelineEntry("c", "C", "10:10"),
        ];
    }

    private static IReadOnlyList<TraceEntry> CreateTraceEntries()
    {
        var now = DateTimeOffset.UtcNow;
        return
        [
            new TraceEntry("a", now.AddSeconds(1), "op-a", "first"),
            new TraceEntry("b", now.AddSeconds(2), "op-b", "second"),
            new TraceEntry("c", now.AddSeconds(3), "op-c", "third"),
        ];
    }

    private static IReadOnlyList<ProcessListEntry> CreateProcessEntries()
    {
        return
        [
            new ProcessListEntry(1, "alpha"),
            new ProcessListEntry(2, "beta"),
            new ProcessListEntry(3, "gamma"),
        ];
    }

    private static IReadOnlyList<PaletteSwatch> CreateSwatches()
    {
        return
        [
            new PaletteSwatch("base", "#111111"),
            new PaletteSwatch("accent", "#222222"),
            new PaletteSwatch("focus", "#333333"),
        ];
    }
}
