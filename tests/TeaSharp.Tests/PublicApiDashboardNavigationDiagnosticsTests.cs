using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
public sealed class PublicApiDashboardNavigationDiagnosticsTests
{
    [Test]
    public void PublicApiDashboardNavigationDiagnostics_TabsPointerMotion_DoesNotRaiseSelectionChanged()
    {
        var tabs = new Tabs("Overview", "Operations", "Audit");
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
    public void PublicApiDashboardNavigationDiagnostics_TabsPointerWheel_ChangesSelectionAndRaisesEvent()
    {
        var tabs = new Tabs("Overview", "Operations", "Audit");
        var changes = 0;
        tabs.SelectionChanged += (_, _) => changes++;

        var handled = tabs.Handle(
            new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, X: 14, Y: 0),
            new Rect(0, 0, 64, 1));

        Assert.That(handled, Is.True);
        Assert.That(tabs.SelectedIndex, Is.EqualTo(1));
        Assert.That(changes, Is.EqualTo(1));
    }
}
