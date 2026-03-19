using System.Reflection;
using System.Text.RegularExpressions;

namespace TeaSharp.Tests;

internal static class PublicApiBoundaryTests
{
    private static readonly string[] PublicApiGuidelineTerms =
    [
        "C#-first",
        "TeaSharp.Core",
        "TeaSharp.Hosting",
        "EventHandler",
        "Update(...)",
        "Build(...)",
    ];

    private static readonly Regex TeaSharpCoreImportRegex = new(@"(?m)^\s*using\s+.*TeaSharp\.Core.*;", RegexOptions.Compiled);

    private static readonly string[] CoreImportAllowList =
    [
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "PublicApiBoundary_DocsDoNotReferenceTeaSharpStyling",
            Docs_DoNotReferenceTeaSharpStyling);
        yield return new TestCase(
            "PublicApiBoundary_ReadmeDocsSectionDoesNotLinkLegacyWidgetsCatalog",
            Readme_DocsSection_DoesNotLinkLegacyWidgetsCatalog);
        yield return new TestCase(
            "PublicApiBoundary_ExamplesDoNotImportTeaSharpCoreOutsideAllowList",
            Examples_DoNotImportTeaSharpCoreOutsideAllowList);
        yield return new TestCase(
            "PublicApiBoundary_PublicApiGuidelinesExistAndDescribeCSharpFirstBoundaries",
            PublicApiGuidelines_ExistAndDescribeCSharpFirstBoundaries);
    }

    private static Task Docs_DoNotReferenceTeaSharpStyling()
    {
        var repoRoot = GetRepoRoot();
        var markdownFiles = EnumerateMarkdownFiles(repoRoot).ToArray();
        var offenders = markdownFiles
            .Where(path => File.ReadAllText(path).Contains("TeaSharp.Styling", StringComparison.Ordinal))
            .Select(ToRepoRelativePath)
            .ToArray();

        TestAssert.True(offenders.Length == 0, $"Docs and README must not reference TeaSharp.Styling. Offenders: {string.Join(", ", offenders)}.");
        return Task.CompletedTask;
    }

    private static Task Readme_DocsSection_DoesNotLinkLegacyWidgetsCatalog()
    {
        var repoRoot = GetRepoRoot();
        var readmePath = Path.Combine(repoRoot, "README.md");
        var docsSection = ReadSection(readmePath, "## Docs");

        TestAssert.True(
            !docsSection.Contains("docs/widgets.md", StringComparison.Ordinal),
            "README docs section must not link docs/widgets.md as the primary catalog.");

        return Task.CompletedTask;
    }

    private static Task Examples_DoNotImportTeaSharpCoreOutsideAllowList()
    {
        var repoRoot = GetRepoRoot();
        var offenders = Directory
            .EnumerateFiles(Path.Combine(repoRoot, "examples"), "Program.cs", SearchOption.AllDirectories)
            .Where(path => !IsAllowListed(path))
            .Where(path => TeaSharpCoreImportRegex.IsMatch(File.ReadAllText(path)))
            .Select(ToRepoRelativePath)
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Example apps must not import TeaSharp.Core.* outside the allow-list. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task PublicApiGuidelines_ExistAndDescribeCSharpFirstBoundaries()
    {
        var repoRoot = GetRepoRoot();
        var guidelinesPath = Path.Combine(repoRoot, "docs", "public-api-guidelines.md");

        TestAssert.True(File.Exists(guidelinesPath), $"Expected public API guidelines at {ToRepoRelativePath(guidelinesPath)}.");

        var text = File.ReadAllText(guidelinesPath);
        foreach (var term in PublicApiGuidelineTerms)
        {
            TestAssert.True(
                text.Contains(term, StringComparison.Ordinal),
                $"Expected {ToRepoRelativePath(guidelinesPath)} to mention {term}.");
        }

        return Task.CompletedTask;
    }

    private static IEnumerable<string> EnumerateMarkdownFiles(string repoRoot)
    {
        yield return Path.Combine(repoRoot, "README.md");

        var docsDirectory = Path.Combine(repoRoot, "docs");
        if (Directory.Exists(docsDirectory))
        {
            foreach (var path in Directory.EnumerateFiles(docsDirectory, "*.md", SearchOption.TopDirectoryOnly))
            {
                yield return path;
            }
        }
    }

    private static string ReadSection(string markdownPath, string heading)
    {
        var lines = File.ReadAllLines(markdownPath);
        var section = new List<string>();
        var inSection = false;

        foreach (var line in lines)
        {
            if (line.StartsWith(heading, StringComparison.Ordinal))
            {
                inSection = true;
                continue;
            }

            if (inSection && line.StartsWith('#'))
            {
                break;
            }

            if (inSection)
            {
                section.Add(line);
            }
        }

        TestAssert.True(section.Count > 0, $"Could not find section {heading} in {ToRepoRelativePath(markdownPath)}.");
        return string.Join(Environment.NewLine, section);
    }

    private static bool IsAllowListed(string path)
    {
        var relativePath = Path.GetRelativePath(GetRepoRoot(), path);
        return CoreImportAllowList.Contains(relativePath, StringComparer.Ordinal);
    }

    private static string GetRepoRoot()
    {
        var start = Path.GetDirectoryName(typeof(PublicApiBoundaryTests).Assembly.Location)
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

    private static string ToRepoRelativePath(string path)
    {
        return Path.GetRelativePath(GetRepoRoot(), path).Replace(Path.DirectorySeparatorChar, '/');
    }
}
