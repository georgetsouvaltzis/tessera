using NUnit.Framework;
using System.Text;
using TeaSharp.Controls;
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
    public void RuntimePointerActivationPolicy_UnconfiguredTeaApp_UsesDoubleClickActivation()
    {
        var app = new PolicyActivationApp();

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicy_DefaultRuntimeOptions_UsesDoubleClickActivation()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(new TeaRuntimeOptions());

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        Assert.That(app.ActivationCount, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));

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
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));

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
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 0));
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
    public void RuntimePointerActivationPolicy_SingleClickPolicy_PressOnlyInputStillActivatesImmediately()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0));

        Assert.That(app.ActivationCount, Is.EqualTo(2));
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
    public void RuntimePointerActivationPolicy_DoubleClickPolicy_PressOnlyNoise_DoesNotActivate()
    {
        var app = new PolicyActivationApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1));

        Assert.That(app.ActivationCount, Is.EqualTo(0));
    }

    [Test]
    public void RuntimePointerActivationPolicy_DoubleClickPolicy_MotionAndNoReleaseCannotEscalateClickCount()
    {
        var app = new CaptureMessageApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.DoubleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
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
    public void RuntimePointerActivationPolicy_DefaultDoubleClick_TabsFirstPressDoesNotSwitchSelection()
    {
        var app = new TabsInteractionApp();
        app.ConfigureRuntimeOptions(new TeaRuntimeOptions());
        _ = app.UpdateRuntime(new WindowResized(64, 6));
        var initialFrame = app.RenderRuntime().Output.Frame.Content;
        var target = ResolveTextHitCoordinate(initialFrame, "Operations");

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(app.Tabs.SelectedIndex, Is.EqualTo(0));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Release, PointerButton.Left, target.X, target.Y));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));
        Assert.That(app.Tabs.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void RuntimePointerActivationPolicy_SingleClickOptBack_TabsFirstPressSwitchesSelection()
    {
        var app = new TabsInteractionApp();
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
                DoubleClickTimeout = TimeSpan.FromSeconds(5),
                DoubleClickSlop = 1,
            });
        _ = app.UpdateRuntime(new WindowResized(64, 6));
        var initialFrame = app.RenderRuntime().Output.Frame.Content;
        var target = ResolveTextHitCoordinate(initialFrame, "Operations");

        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, target.X, target.Y));

        Assert.That(app.Tabs.SelectedIndex, Is.EqualTo(1));
    }

    private static (int X, int Y) ResolveTextHitCoordinate(string frame, string text)
    {
        var lines = frame.Split('\n');
        for (var y = 0; y < lines.Length; y++)
        {
            var line = StripAnsiSequences(lines[y]);
            var start = line.IndexOf(text, StringComparison.Ordinal);
            if (start < 0)
            {
                continue;
            }

            return (start + (text.Length / 2), y);
        }

        throw new AssertionException($"Unable to locate tab label '{text}' in runtime frame.");
    }

    private static string StripAnsiSequences(string line)
    {
        if (string.IsNullOrEmpty(line) || line.IndexOf('\u001b') < 0)
        {
            return line;
        }

        var builder = new StringBuilder(line.Length);
        for (var index = 0; index < line.Length; index++)
        {
            if (line[index] != '\u001b')
            {
                builder.Append(line[index]);
                continue;
            }

            if (index + 1 >= line.Length)
            {
                break;
            }

            index++;
            if (line[index] == '[')
            {
                while (index + 1 < line.Length)
                {
                    index++;
                    var next = line[index];
                    if (next is >= '@' and <= '~')
                    {
                        break;
                    }
                }

                continue;
            }

            if (line[index] == ']')
            {
                while (index + 1 < line.Length)
                {
                    index++;
                    if (line[index] == '\a')
                    {
                        break;
                    }

                    if (line[index] == '\u001b'
                        && index + 1 < line.Length
                        && line[index + 1] == '\\')
                    {
                        index++;
                        break;
                    }
                }
            }
        }

        return builder.ToString();
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

    private sealed class TabsInteractionApp : TeaApp
    {
        public Tabs Tabs { get; } = new("Overview", "Operations", "Audit");

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From(Tabs);
    }
}
