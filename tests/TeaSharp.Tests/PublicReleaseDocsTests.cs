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
