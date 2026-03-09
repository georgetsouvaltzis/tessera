using NUnit.Framework;
using TeaSharp.Core.Messages;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class WorkspaceUxIntegrationTests
{
    [Test]
    public void UpKeyIncrementsCounter()
    {
        var model = new CounterModel();

        model.Update(new KeyPressMsg(KeyCode.Up));

        Assert.That(model.View().Content, Does.Contain("Count: 1"));
    }

    [Test]
    public void DownKeyDecrementsCounter()
    {
        var model = new CounterModel();

        model.Update(new KeyPressMsg(KeyCode.Down));

        Assert.That(model.View().Content, Does.Contain("Count: -1"));
    }

    [Test]
    public void UpThenDownReturnsCounterToZero()
    {
        var model = new CounterModel();

        model.Update(new KeyPressMsg(KeyCode.Up));
        model.Update(new KeyPressMsg(KeyCode.Down));

        Assert.That(model.View().Content, Does.Contain("Count: 0"));
    }

    [Test]
    public void LowercaseQReturnsQuitCommand()
    {
        var model = new CounterModel();

        var result = model.Update(new KeyPressMsg(KeyCode.Character, "q"));

        Assert.That(result.Command, Is.EqualTo(TeaSharp.Tea.Cmd.Quit));
    }
}
