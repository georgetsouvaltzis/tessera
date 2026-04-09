using NUnit.Framework;
using Tessera.Examples.DataWorkbench;

namespace Tessera.Tests;

public sealed class DataWorkbenchLayoutTests
{
    [Test]
    public void InitialExploreScreenRendersReadableButtonLabels()
    {
        var app = new DataWorkbenchApp();
        _ = app.UpdateRuntime(new WindowResized(120, 36));

        var output = app.RenderRuntime().Output.Frame.Content;

        Assert.That(output, Does.Contain("Run"));
        Assert.That(output, Does.Contain("Pin"));
        Assert.That(output, Does.Contain("Save"));
        Assert.That(output, Does.Contain("Export"));
        Assert.That(output, Does.Contain("Clear"));
        Assert.That(output, Does.Contain("Citrine"));
        Assert.That(output, Does.Contain("Cobalt"));
        Assert.That(output, Does.Contain("Ember"));
    }
}
