using NUnit.Framework;
using TeaSharp.Core.Rendering;

namespace TeaSharp.Tests;

[TestFixture]
public sealed class RenderFrameContentTests
{
    [Test]
    public void NormalizeLines_MixedLineEndings_AreNormalizedWithoutDataLoss()
    {
        var lines = RenderFrameContent.NormalizeLines("alpha\r\nbeta\rgamma\ndelta");

        Assert.That(lines, Is.EqualTo(new[] { "alpha", "beta", "gamma", "delta" }));
    }

    [Test]
    public void NormalizeLines_TrailingBreak_ProducesTrailingEmptyRow()
    {
        var lines = RenderFrameContent.NormalizeLines("alpha\n");

        Assert.That(lines, Is.EqualTo(new[] { "alpha", string.Empty }));
    }
}
