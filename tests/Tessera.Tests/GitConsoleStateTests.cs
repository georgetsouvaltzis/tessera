using NUnit.Framework;
using Tessera.Examples.GitConsole;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class GitConsoleStateTests
{
    [Test]
    public void GitConsoleSeed_PublicApiDocsEntry_ShowsAnActualStagedDiff()
    {
        var state = GitConsoleState.CreateSeed();

        Assert.That(state.SetScope("shiproom"), Is.True);
        Assert.That(state.SelectFile("public-api-docs"), Is.True);

        var snapshot = state.BuildDiffSnapshot(GitDiffTab.StagedSnapshot);

        Assert.That(snapshot.OldText, Is.Not.EqualTo(snapshot.NewText));
        Assert.That(snapshot.OldText, Does.Contain("HelloWorld"));
        Assert.That(snapshot.NewText, Does.Contain("DataWorkbench"));
    }
}
