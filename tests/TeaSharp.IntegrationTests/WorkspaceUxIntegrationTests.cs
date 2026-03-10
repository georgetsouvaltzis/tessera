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

        Assert.That(model.View().Frame.Content, Does.Contain("Count: 1"));
    }

    [Test]
    public void DownKeyDecrementsCounter()
    {
        var model = new CounterFixtureModel();

        model.Update(new KeyPressMsg(KeyCode.Down));

        Assert.That(model.View().Frame.Content, Does.Contain("Count: -1"));
    }

    [Test]
    public void UpThenDownReturnsCounterToZero()
    {
        var model = new CounterFixtureModel();

        model.Update(new KeyPressMsg(KeyCode.Up));
        model.Update(new KeyPressMsg(KeyCode.Down));

        Assert.That(model.View().Frame.Content, Does.Contain("Count: 0"));
    }

    [Test]
    public void LowercaseQReturnsQuitCommand()
    {
        var model = new CounterFixtureModel();

        var result = model.Update(new KeyPressMsg(KeyCode.Character, "q"));

        Assert.That(result, Is.EqualTo(TeaSharp.Tea.Cmd.Quit));
    }
}
