using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using NUnit.Framework;
using TeaSharp.TestFixtures;
using TeaSharp.Core.Messages;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class WorkspaceUxIntegrationTests
{
    [Test]
    public void UpKeyIncrementsCounter()
    {
        var model = new CounterFixtureModel();

        model.Update(new KeyPressMsg(KeyCode.Up));

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: 1"));
    }

    [Test]
    public void DownKeyDecrementsCounter()
    {
        var model = new CounterFixtureModel();

        model.Update(new KeyPressMsg(KeyCode.Down));

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: -1"));
    }

    [Test]
    public void UpThenDownReturnsCounterToZero()
    {
        var model = new CounterFixtureModel();

        model.Update(new KeyPressMsg(KeyCode.Up));
        model.Update(new KeyPressMsg(KeyCode.Down));

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: 0"));
    }

    [Test]
    public void LowercaseQReturnsQuitCommand()
    {
        var model = new CounterFixtureModel();

        var result = model.Update(new KeyPressMsg(KeyCode.Character, "q"));

        Assert.That(result, Is.EqualTo(TeaSharp.Core.Commands.Effects.Quit));
    }
}
