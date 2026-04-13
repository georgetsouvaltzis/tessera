using NUnit.Framework;
using Tessera.Controls;
using Tessera.Layout;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class RuntimePointerActivationPolicyTests
{
    [Test]
    public void RuntimePointerActivationPolicyPointerInputBackwardCompatibleClickCountDefaultsToOne()
    {
        var pointer = new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 4);

        Assert.That(pointer.ClickCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicyUnconfiguredTeaAppUsesDoubleClickActivation()
    {
        var app = new PolicyActivationApp();

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicyDefaultRuntimeOptionsUsesDoubleClickActivation()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(new TesseraRuntimeOptions());

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyBlocksSingleClickButtonActivation()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyRespectsSlopThreshold()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 0
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(0));
    }

    [Test]
    public void RuntimePointerActivationPolicySingleClickPolicyPreservesPressRoutingAndAddsClickCount()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.Seen.Count, Is.EqualTo(3));
        Assert.That(app.Seen[0], Is.TypeOf<PointerInput>());
        Assert.That(app.Seen[1], Is.TypeOf<PointerInput>());
        Assert.That(app.Seen[2], Is.TypeOf<PointerInput>());

        var first = (PointerInput)app.Seen[0];
        var second = (PointerInput)app.Seen[1];
        var third = (PointerInput)app.Seen[2];
        Assert.That(first.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(first.ClickCount, Is.EqualTo(1));
        Assert.That(second.Kind, Is.EqualTo(PointerEventKind.Release));
        Assert.That(third.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(third.ClickCount, Is.EqualTo(2));
    }

    [Test]
    public void RuntimePointerActivationPolicySingleClickPolicyPressOnlyInputStillActivatesImmediately()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(2));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyRoutesSingleClickAsMotionAndSecondAsPress()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.Seen.Count, Is.EqualTo(3));
        Assert.That(app.Seen[0], Is.TypeOf<PointerInput>());
        Assert.That(app.Seen[1], Is.TypeOf<PointerInput>());
        Assert.That(app.Seen[2], Is.TypeOf<PointerInput>());

        var first = (PointerInput)app.Seen[0];
        var second = (PointerInput)app.Seen[1];
        var third = (PointerInput)app.Seen[2];
        Assert.That(first.Kind, Is.EqualTo(PointerEventKind.Motion));
        Assert.That(first.Button, Is.EqualTo(PointerButton.None));
        Assert.That(first.ClickCount, Is.EqualTo(0));
        Assert.That(second.Kind, Is.EqualTo(PointerEventKind.Release));
        Assert.That(third.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(third.Button, Is.EqualTo(PointerButton.Left));
        Assert.That(third.ClickCount, Is.EqualTo(2));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyReleaseNoneCompletesCycleAndEscalatesSecondPress()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.None, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.Seen.Count, Is.EqualTo(3));

        var first = (PointerInput)app.Seen[0];
        var second = (PointerInput)app.Seen[1];
        var third = (PointerInput)app.Seen[2];

        Assert.That(first.Kind, Is.EqualTo(PointerEventKind.Motion));
        Assert.That(first.Button, Is.EqualTo(PointerButton.None));
        Assert.That(second.Kind, Is.EqualTo(PointerEventKind.Release));
        Assert.That(second.Button, Is.EqualTo(PointerButton.None));
        Assert.That(third.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(third.Button, Is.EqualTo(PointerButton.Left));
        Assert.That(third.ClickCount, Is.EqualTo(2));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyReleaseDifferentButtonDoesNotCompleteCycle()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Right, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.ActivationCount, Is.EqualTo(0));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyPressOnlyNoiseDoesNotActivate()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.ActivationCount, Is.EqualTo(0));
    }

    [Test]
    public void RuntimePointerActivationPolicyDoubleClickPolicyMotionAndNoReleaseCannotEscalateClickCount()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2));

        var pressWithDoubleCount = app.Seen
            .OfType<PointerInput>()
            .Any(static pointer => pointer.Kind == PointerEventKind.Press && pointer.ClickCount >= 2);

        Assert.That(pressWithDoubleCount, Is.False);
    }

    [Test]
    public void RuntimePointerActivationPolicyDefaultDoubleClickTabsFirstPressDoesNotSwitchSelection()
    {
        var target = ResolveSecondTabHitCoordinate();
        var app = new TabsInteractionApp();
        app.ConfigureRuntimeOptions(new TesseraRuntimeOptions());
        _ = app.UpdateRuntime(new WindowResized(64, 6));
        _ = app.RenderRuntime();

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(app.Tabs.SelectedIndex, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, target.X, target.Y));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));
        Assert.That(app.Tabs.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicySingleClickOptBackTabsFirstPressSwitchesSelection()
    {
        var target = ResolveSecondTabHitCoordinate();
        var app = new TabsInteractionApp();
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1
            });
        _ = app.UpdateRuntime(new WindowResized(64, 6));
        _ = app.RenderRuntime();

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(app.Tabs.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicyDefaultDoubleClickFirstClickFocusesClickedControlWithoutActivation()
    {
        var app = new FocusRoutingApp();
        app.ConfigureRuntimeOptions(new TesseraRuntimeOptions());
        _ = app.UpdateRuntime(new WindowResized(64, 6));
        _ = app.RenderRuntime();

        Assert.That(app.LeftButton.IsFocused, Is.True);
        Assert.That(app.RightButton.IsFocused, Is.False);

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 14, 0));

        Assert.That(app.LeftButton.IsFocused, Is.False);
        Assert.That(app.RightButton.IsFocused, Is.True);
        Assert.That(app.LeftActivationCount, Is.EqualTo(0));
        Assert.That(app.RightActivationCount, Is.EqualTo(0));
    }

    private static (int X, int Y) ResolveSecondTabHitCoordinate()
    {
        const int width = 64;
        const int height = 6;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var probe = new TabsInteractionApp();
                probe.ConfigureRuntimeOptions(
                    new TesseraRuntimeOptions
                    {
                        PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                        DoubleClickTimeout = TimeSpan.FromSeconds(5),
                        DoubleClickSlop = 1
                    });
                _ = probe.UpdateRuntime(new WindowResized(width, height));
                _ = probe.RenderRuntime();
                _ = probe.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, x, y));

                if (probe.Tabs.SelectedIndex == 1)
                {
                    return (x, y);
                }
            }
        }

        throw new AssertionException("Unable to resolve a runtime pointer coordinate that selects the second tab.");
    }

    private sealed class PolicyActivationApp : TesseraApp
    {
        public int ActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            if (message is PointerInput { Kind: PointerEventKind.Press, Button: PointerButton.Left })
            {
                ActivationCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From("policy");
        }
    }

    private sealed class CaptureMessageApp : TesseraApp
    {
        public List<Message> Seen { get; } = [];

        public override TesseraEffect? Update(Message message)
        {
            Seen.Add(message);
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From("capture");
        }
    }

    private sealed class TabsInteractionApp : TesseraApp
    {
        public Tabs Tabs { get; } = new("Overview", "Operations", "Audit");

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(Tabs);
        }
    }

    private sealed class FocusRoutingApp : TesseraApp
    {
        public FocusRoutingApp()
        {
            LeftButton.Activated += (_, _) => LeftActivationCount++;
            RightButton.Activated += (_, _) => RightActivationCount++;
            LeftButton.RequestFocus();
        }

        public Button LeftButton { get; } = new() { Text = "Left" };

        public Button RightButton { get; } = new() { Text = "Right" };

        public int LeftActivationCount { get; private set; }

        public int RightActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            var row = new RowLayout { Gap = 2 };
            row.AddFixed(LeftButton, 12);
            row.AddFixed(RightButton, 12);
            return Screen.From(row);
        }
    }
}
