using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
public sealed class PublicApiDashboardNavigationDiagnosticsTests
{
    [Test]
    public void PublicApiDashboardNavigationDiagnostics_DashboardTabsPointerMotion_DoesNotRaiseSelectionChangedOrMutateSelection()
    {
        var tabs = new DashboardNavigationTabs("Overview", "Operations", "Audit");
        var changes = 0;
        tabs.SelectionChanged += (_, _) => changes++;

        var handled = tabs.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, X: 14, Y: 0),
            new Rect(0, 0, 64, 1));

        Assert.That(handled, Is.True);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(0));
        Assert.That(changes, Is.EqualTo(0));
    }

    [Test]
    public void PublicApiDashboardNavigationDiagnostics_DashboardTabsPointerMotion_OutsideHeaderBounds_DoesNotRaiseSelectionChangedOrMutateSelection()
    {
        var tabs = new DashboardNavigationTabs("Overview", "Operations", "Audit");
        tabs.SetSelectedIndex(1);
        var changes = 0;
        tabs.SelectionChanged += (_, _) => changes++;

        var handled = tabs.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, X: 22, Y: 14),
            new Rect(0, 0, 64, 1));

        Assert.That(handled, Is.False);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(1));
        Assert.That(changes, Is.EqualTo(0));
    }

    [Test]
    public void PublicApiDashboardNavigationDiagnostics_DashboardTabsPointerWheel_IsBlockedWithoutSelectionMutation()
    {
        var tabs = new DashboardNavigationTabs("Overview", "Operations", "Audit");
        var changes = 0;
        tabs.SelectionChanged += (_, _) => changes++;

        var handled = tabs.Handle(
            new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, X: 14, Y: 0),
            new Rect(0, 0, 64, 1));

        Assert.That(handled, Is.True);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(0));
        Assert.That(changes, Is.EqualTo(0));
    }

    [Test]
    public void PublicApiDashboardNavigationDiagnostics_DashboardTabsPointerPress_ChangesSelectionOnce()
    {
        var tabs = new DashboardNavigationTabs("Overview", "Operations", "Audit");
        var changes = 0;
        tabs.SelectionChanged += (_, _) => changes++;

        var handled = tabs.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 14, Y: 0),
            new Rect(0, 0, 64, 1));

        Assert.That(handled, Is.True);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(1));
        Assert.That(changes, Is.EqualTo(1));
    }
}
