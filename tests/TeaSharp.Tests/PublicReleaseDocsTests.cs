using System.Text.RegularExpressions;

namespace TeaSharp.Tests;

internal static class PublicReleaseDocsTests
{
    private static readonly Regex SemVerHeadingRegex = new(@"^## \[(\d+)\.(\d+)\.(\d+)(?:-[0-9A-Za-z.-]+)?\] - \d{4}-\d{2}-\d{2}$", RegexOptions.Multiline | RegexOptions.Compiled);

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "PublicReleaseDocs_ChangelogExists_AndUsesSemVerHeadings",
            ChangelogExists_AndUsesSemVerHeadings);
        yield return new TestCase(
            "PublicReleaseDocs_SupportDocExists_AndReadmeLinksIt",
            SupportDocExists_AndReadmeLinksIt);
        yield return new TestCase(
            "PublicReleaseDocs_ExamplesSolutionExists_UnderExamples",
            ExamplesSolutionExists_UnderExamples);
    }

    private static Task ChangelogExists_AndUsesSemVerHeadings()
    {
        var repoRoot = GetRepoRoot();
        var changelogPath = Path.Combine(repoRoot, "CHANGELOG.md");
        var readmePath = Path.Combine(repoRoot, "README.md");

        TestAssert.True(File.Exists(changelogPath), "Expected root CHANGELOG.md to exist.");

        var changelog = File.ReadAllText(changelogPath);
        TestAssert.True(
            changelog.Contains("major.minor.patch", StringComparison.Ordinal),
            "CHANGELOG.md should describe the versioning scheme.");
        TestAssert.True(
            changelog.Contains("1.0.0-alpha.1", StringComparison.Ordinal),
            "CHANGELOG.md should record the first public alpha as 1.0.0-alpha.1.");
        TestAssert.True(
            SemVerHeadingRegex.IsMatch(changelog),
            "CHANGELOG.md should contain at least one SemVer heading in the form ## [x.y.z] - YYYY-MM-DD or ## [x.y.z-prerelease] - YYYY-MM-DD.");

        var readme = File.ReadAllText(readmePath);
        TestAssert.True(
            readme.Contains("CHANGELOG.md", StringComparison.Ordinal),
            "README docs section should link CHANGELOG.md.");

        return Task.CompletedTask;
    }

    private static Task SupportDocExists_AndReadmeLinksIt()
    {
        var repoRoot = GetRepoRoot();
        var supportPath = Path.Combine(repoRoot, "SUPPORT.md");
        var readmePath = Path.Combine(repoRoot, "README.md");

        TestAssert.True(File.Exists(supportPath), "Expected root SUPPORT.md to exist.");

        var support = File.ReadAllText(supportPath);
        TestAssert.True(
            support.Contains("GitHub Issue", StringComparison.Ordinal),
            "SUPPORT.md should direct users to GitHub Issues.");
        TestAssert.True(
            support.Contains("examples/TeaSharp.Examples.slnx", StringComparison.Ordinal),
            "SUPPORT.md should mention the examples solution build path.");

        var readme = File.ReadAllText(readmePath);
        TestAssert.True(
            readme.Contains("SUPPORT.md", StringComparison.Ordinal),
            "README docs section should link SUPPORT.md.");

        return Task.CompletedTask;
    }

    private static Task ExamplesSolutionExists_UnderExamples()
    {
        var repoRoot = GetRepoRoot();
        var solutionPath = Path.Combine(repoRoot, "examples", "TeaSharp.Examples.slnx");

        TestAssert.True(File.Exists(solutionPath), "Expected examples/TeaSharp.Examples.slnx to exist.");

        var solution = File.ReadAllText(solutionPath);
        foreach (var path in new[]
                 {
                     "HelloWorld/HelloWorld.csproj",
                     "CounterForm/CounterForm.csproj",
                     "WorkspaceApp/WorkspaceApp.csproj",
                     "GitConsole/GitConsole.csproj",
                     "OpsWatch/OpsWatch.csproj",
                     "DataWorkbench/DataWorkbench.csproj",
                 })
        {
            TestAssert.True(
                solution.Contains(path, StringComparison.Ordinal),
                $"Expected examples solution to include {path}.");
        }

        return Task.CompletedTask;
    }

    private static string GetRepoRoot()
    {
        var start = Path.GetDirectoryName(typeof(PublicReleaseDocsTests).Assembly.Location)
            ?? AppContext.BaseDirectory;
        var directory = new DirectoryInfo(start);

        while (directory is not null)
        {
            var hasReadme = File.Exists(Path.Combine(directory.FullName, "README.md"));
            var hasGlobalJson = File.Exists(Path.Combine(directory.FullName, "global.json"));
            if (hasReadme && hasGlobalJson)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the TeaSharp repository root.");
    }
}
