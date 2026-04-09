using Tessera.Components.Composition;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using NUnit.Framework;
using Tessera.IntegrationFixtureApp;
using Tessera.Core.Messages;

namespace Tessera.IntegrationTests;

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

        Assert.That(result, Is.EqualTo(Tessera.Core.Commands.Effects.Quit));
    }
}
