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

    [Test]
    public void BuildRows_HeightClip_KeepsBottomWrappedRows()
    {
        var rows = RenderFrameContent.BuildRows("abcd\nefgh", width: 2, height: 2);

        Assert.That(rows, Has.Count.EqualTo(2));
        Assert.That(RowText(rows[0]), Is.EqualTo("ef"));
        Assert.That(RowText(rows[1]), Is.EqualTo("gh"));
    }

    private static string RowText(RenderFrameRow row)
    {
        var parts = new List<string>();
        for (var column = 0; column < row.ColumnCount; column++)
        {
            var cell = row.CellAt(column);
            if (cell is not null)
            {
                parts.Add(cell);
            }
        }

        return string.Concat(parts);
    }
}
