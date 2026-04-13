using NUnit.Framework;

namespace Tessera.Tests;

[TestFixture]
public sealed class RenderFrameContentTests
{
    [Test]
    public void NormalizeLinesMixedLineEndingsAreNormalizedWithoutDataLoss()
    {
        var lines = RenderFrameContent.NormalizeLines("alpha\r\nbeta\rgamma\ndelta");

        var expected = new[] { "alpha", "beta", "gamma", "delta" };
        Assert.That(lines, Is.EqualTo(expected));
    }

    [Test]
    public void NormalizeLinesTrailingBreakProducesTrailingEmptyRow()
    {
        var lines = RenderFrameContent.NormalizeLines("alpha\n");

        var expected = new[] { "alpha", string.Empty };
        Assert.That(lines, Is.EqualTo(expected));
    }

    [Test]
    public void BuildRowsHeightClipKeepsBottomWrappedRows()
    {
        var rows = RenderFrameContent.BuildRows("abcd\nefgh", 2, 2);

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
