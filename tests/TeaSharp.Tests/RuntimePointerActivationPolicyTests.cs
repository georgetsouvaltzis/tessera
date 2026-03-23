using NUnit.Framework;
namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class RuntimePointerActivationPolicyTests
{
    [Test]
    public void RuntimePointerActivationPolicy_PointerInput_BackwardCompatibleClickCountDefaultsToOne()
    {
        var pointer = new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 4);

        Assert.That(pointer.ClickCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicy_DefaultRuntimeOptions_UsesDoubleClickActivation()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(new TeaRuntimeOptions());

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(0));

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicy_DoubleClickPolicy_BlocksSingleClickButtonActivation()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(0));

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicy_DoubleClickPolicy_RespectsSlopThreshold()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 0,
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(0));
    }

    [Test]
    public void RuntimePointerActivationPolicy_SingleClickPolicy_PreservesPressRoutingAndAddsClickCount()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.Seen.Count, Is.EqualTo(2));
        Assert.That(app.Seen[0], Is.TypeOf<PointerInput>());
        Assert.That(app.Seen[1], Is.TypeOf<PointerInput>());

        var first = (PointerInput)app.Seen[0];
        var second = (PointerInput)app.Seen[1];
        Assert.That(first.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(first.ClickCount, Is.EqualTo(1));
        Assert.That(second.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(second.ClickCount, Is.EqualTo(2));
    }

    [Test]
    public void RuntimePointerActivationPolicy_DoubleClickPolicy_RoutesSingleClickAsMotionAndSecondAsPress()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.Seen.Count, Is.EqualTo(2));
        Assert.That(app.Seen[0], Is.TypeOf<PointerInput>());
        Assert.That(app.Seen[1], Is.TypeOf<PointerInput>());

        var first = (PointerInput)app.Seen[0];
        var second = (PointerInput)app.Seen[1];
        Assert.That(first.Kind, Is.EqualTo(PointerEventKind.Motion));
        Assert.That(first.Button, Is.EqualTo(PointerButton.None));
        Assert.That(first.ClickCount, Is.EqualTo(0));
        Assert.That(second.Kind, Is.EqualTo(PointerEventKind.Press));
        Assert.That(second.Button, Is.EqualTo(PointerButton.Left));
        Assert.That(second.ClickCount, Is.EqualTo(2));
    }

    private sealed class PolicyActivationApp : TeaApp
    {
        public int ActivationCount { get; private set; }

        public override TeaEffect? Update(Message message)
        {
            if (message is PointerInput { Kind: PointerEventKind.Press, Button: PointerButton.Left })
            {
                ActivationCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) => Screen.From("policy");
    }

    private sealed class CaptureMessageApp : TeaApp
    {
        public List<Message> Seen { get; } = [];

        public override TeaEffect? Update(Message message)
        {
            Seen.Add(message);
            return null;
        }

        public override Screen Build(ScreenContext context) => Screen.From("capture");
    }
}
