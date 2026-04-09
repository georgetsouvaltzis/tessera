using NUnit.Framework;
using Tessera;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Layout;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class PublicApiDashboardPointerFocusRegressionTests
{
    [Test]
    public void PublicApiDashboardFirstClickInServicesRegionFocusesServicesWithoutNavigationMutation()
    {
        var target = ResolveServicesHitCoordinate();
        var app = new DashboardPointerFocusProbeApp();
        app.ConfigureRuntimeOptions(new TesseraRuntimeOptions());

        _ = app.UpdateRuntime(new WindowResized(120, 36));
        _ = app.RenderRuntime();

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(
            app.Services.IsFocused,
            Is.True,
            "First pointer press in Services pane should move focus to services.");
        Assert.That(
            app.Navigation.SelectedIndex,
            Is.EqualTo(0),
            "Services click should not switch dashboard tab.");
        Assert.That(
            app.NavigationSelectionChanges,
            Is.EqualTo(0),
            "Services click should not raise navigation SelectionChanged.");
    }

    [Test]
    public void PublicApiDashboardDefaultDoubleClickPressReleaseNonePressOnServicesRowChangesSelection()
    {
        var target = ResolveServicesRowHitCoordinate(rowIndex: 1);
        var app = new DashboardPointerFocusProbeApp();
        app.ConfigureRuntimeOptions(new TesseraRuntimeOptions());

        _ = app.UpdateRuntime(new WindowResized(120, 36));
        _ = app.RenderRuntime();

        Assert.That(app.Services.SelectedIndex, Is.EqualTo(0), "Precondition: first service should be selected initially.");

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.None, target.X, target.Y));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(
            app.Services.SelectedIndex,
            Is.EqualTo(1),
            "press -> release(None) -> press should select the second service row under default double-click policy.");
        Assert.That(
            app.ServiceSelectionChanges,
            Is.GreaterThanOrEqualTo(1),
            "Service selection change event should fire when row selection changes.");
        Assert.That(app.Navigation.SelectedIndex, Is.EqualTo(0), "Service row click flow should not switch dashboard tab.");
    }

    [Test]
    public void PublicApiDashboardSingleClickPolicyFirstPressOnServicesRowChangesSelection()
    {
        var target = ResolveServicesRowHitCoordinate(rowIndex: 1);
        var app = new DashboardPointerFocusProbeApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });

        _ = app.UpdateRuntime(new WindowResized(120, 36));
        _ = app.RenderRuntime();

        Assert.That(app.Services.SelectedIndex, Is.EqualTo(0), "Precondition: first service should be selected initially.");

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(
            app.Services.SelectedIndex,
            Is.EqualTo(1),
            "Single-click pointer policy should select the second service row on first press.");
        Assert.That(
            app.ServiceSelectionChanges,
            Is.GreaterThanOrEqualTo(1),
            "Service selection change event should fire when row selection changes.");
        Assert.That(app.Navigation.SelectedIndex, Is.EqualTo(0), "Service row click should not switch dashboard tab.");
    }

    private static (int X, int Y) ResolveServicesHitCoordinate()
    {
        const int width = 120;
        const int height = 36;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var probe = new DashboardPointerFocusProbeApp();
                probe.ConfigureRuntimeOptions(
                    new TesseraRuntimeOptions
                    {
                        PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                        DoubleClickTimeout = TimeSpan.FromSeconds(5),
                        DoubleClickSlop = 1,
                    });

                _ = probe.UpdateRuntime(new WindowResized(width, height));
                _ = probe.RenderRuntime();
                _ = probe.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, x, y));

                if (probe.Services.IsFocused
                    && probe.Navigation.SelectedIndex == 0
                    && probe.NavigationSelectionChanges == 0)
                {
                    return (x, y);
                }
            }
        }

        throw new AssertionException("Unable to resolve pointer coordinate that focuses the services pane.");
    }

    private static (int X, int Y) ResolveServicesRowHitCoordinate(int rowIndex)
    {
        const int width = 120;
        const int height = 36;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var probe = new DashboardPointerFocusProbeApp();
                probe.ConfigureRuntimeOptions(
                    new TesseraRuntimeOptions
                    {
                        PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                        DoubleClickTimeout = TimeSpan.FromSeconds(5),
                        DoubleClickSlop = 1,
                    });

                _ = probe.UpdateRuntime(new WindowResized(width, height));
                _ = probe.RenderRuntime();
                _ = probe.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, x, y));

                if (probe.Services.SelectedIndex == rowIndex
                    && probe.Navigation.SelectedIndex == 0
                    && probe.NavigationSelectionChanges == 0)
                {
                    return (x, y);
                }
            }
        }

        throw new AssertionException($"Unable to resolve pointer coordinate that selects services row index {rowIndex}.");
    }

    private sealed class DashboardPointerFocusProbeApp : TesseraApp
    {
        private readonly DashboardNavigationTabs _navigation = new("Overview", "Operations", "Audit")
        {
            Title = "Public API Dashboard",
            FocusMarker = "◆",
        };

        private readonly ListView<string> _services = new(static name => name)
        {
            Title = "Services",
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
            FocusMarker = "◆",
        };

        private readonly Label _body = new()
        {
            Text = "Body",
            Title = "Body",
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
        };

        private readonly Dialog _confirmDeploy = new()
        {
            Title = "Confirm Deployment",
            BodyLines =
            [
                "Deploy selected service?",
                "Enter accepts, Esc cancels.",
            ],
        };

        public DashboardPointerFocusProbeApp()
        {
            _services.SetItems(["API", "Worker", "Scheduler", "Gateway"]);
            _navigation.SelectionChanged += (_, _) => NavigationSelectionChanges++;
            _services.SelectionChanged += (_, _) => ServiceSelectionChanges++;
        }

        public DashboardNavigationTabs Navigation => _navigation;

        public ListView<string> Services => _services;

        public int NavigationSelectionChanges { get; private set; }

        public int ServiceSelectionChanges { get; private set; }

        public override TesseraEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            return Screen.Build(window =>
            {
                window.Gap(1);
                window.Padding(1);
                window.Header(1, _navigation);
                window.Left(Math.Min(36, Math.Max(28, context.Width / 4)), _services);
                window.Body(body => body.Center(_body, width: 24, height: 5));
                if (_confirmDeploy.IsVisible)
                {
                    window.Overlay(new CenterLayout
                    {
                        Content = _confirmDeploy,
                        Width = Math.Min(60, Math.Max(44, context.Width - 8)),
                        Height = 9,
                    });
                }
            });
        }
    }
}
