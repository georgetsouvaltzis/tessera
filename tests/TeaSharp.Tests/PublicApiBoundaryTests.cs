using System.Reflection;
using System.Text.RegularExpressions;
using TeaSharp;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class PublicApiBoundaryTests
{
    private static readonly string[] PublicApiGuidelineTerms =
    [
        "C#-first",
        "TeaSharp.Core",
        "TeaSharp.Hosting",
        "opt-in",
        "EventHandler",
        "Update(...)",
        "Build(...)",
    ];

    private static readonly Regex TeaSharpCoreImportRegex = new(@"(?m)^\s*using\s+.*TeaSharp\.Core.*;", RegexOptions.Compiled);
    private static readonly Regex TeaSharpHostingImportRegex = new(@"(?m)^\s*using\s+.*TeaSharp\.Hosting.*;", RegexOptions.Compiled);
    private static readonly Regex DependencyInjectionImportRegex = new(@"(?m)^\s*using\s+.*Microsoft\.Extensions\.DependencyInjection.*;", RegexOptions.Compiled);

    private static readonly string[] FlagshipExampleProjectPaths =
    [
        "examples/DataWorkbench/DataWorkbench.csproj",
        "examples/OpsWatch/OpsWatch.csproj",
        "examples/GitConsole/GitConsole.csproj",
    ];

    private static readonly string[] CoreImportAllowList =
    [
    ];

    private static readonly string[] OnboardingSourceRoots =
    [
        "src/TeaSharp/Controls",
        "src/TeaSharp/Layout",
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
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProjectsExist",
            FlagshipExampleProjects_Exist);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProgramsDoNotImportTeaSharpCore",
            FlagshipExamplePrograms_DoNotImportTeaSharpCore);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProgramsDoNotImportDependencyInjection",
            FlagshipExamplePrograms_DoNotImportDependencyInjection);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProgramsDoNotImportTeaSharpHosting",
            FlagshipExamplePrograms_DoNotImportTeaSharpHosting);
        yield return new TestCase(
            "PublicApiBoundary_OnboardingSourceFilesDoNotReferenceTeaSharpCore",
            OnboardingSourceFiles_DoNotReferenceTeaSharpCore);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProjectsDoNotReferenceTeaSharpCoreProject",
            FlagshipExampleProjects_DoNotReferenceTeaSharpCoreProject);
        yield return new TestCase(
            "PublicApiBoundary_PublicTeaSharpAssemblySurfaceDoesNotExposeTeaSharpCoreTypes",
            PublicTeaSharpAssemblySurface_DoesNotExposeTeaSharpCoreTypes);
        yield return new TestCase(
            "PublicApiBoundary_ThemeScopePublicMethods_DoNotExposeTeaSharpCoreOrHostingTypes",
            ThemeScopePublicMethods_DoNotExposeTeaSharpCoreOrHostingTypes);
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

    private static Task FlagshipExampleProjects_Exist()
    {
        var repoRoot = GetRepoRoot();
        var missing = FlagshipExampleProjectPaths
            .Where(path => !File.Exists(Path.Combine(repoRoot, path)))
            .ToArray();

        TestAssert.True(
            missing.Length == 0,
            $"Flagship example projects are missing: {string.Join(", ", missing)}.");

        return Task.CompletedTask;
    }

    private static Task FlagshipExamplePrograms_DoNotImportTeaSharpCore()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Select(path => path.Replace(".csproj", "/Program.cs", StringComparison.Ordinal))
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => TeaSharpCoreImportRegex.IsMatch(File.ReadAllText(Path.Combine(repoRoot, path))))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship examples must not import TeaSharp.Core.*. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task FlagshipExamplePrograms_DoNotImportDependencyInjection()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Select(path => path.Replace(".csproj", "/Program.cs", StringComparison.Ordinal))
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => DependencyInjectionImportRegex.IsMatch(File.ReadAllText(Path.Combine(repoRoot, path))))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship examples should not depend on Microsoft.Extensions.DependencyInjection. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task FlagshipExamplePrograms_DoNotImportTeaSharpHosting()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Select(path => path.Replace(".csproj", "/Program.cs", StringComparison.Ordinal))
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => TeaSharpHostingImportRegex.IsMatch(File.ReadAllText(Path.Combine(repoRoot, path))))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship examples should not import TeaSharp.Hosting in the default public path. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task OnboardingSourceFiles_DoNotReferenceTeaSharpCore()
    {
        var repoRoot = GetRepoRoot();
        var offenders = new List<string>();

        foreach (var sourceRoot in OnboardingSourceRoots)
        {
            var absoluteRoot = Path.Combine(repoRoot, sourceRoot);
            if (!Directory.Exists(absoluteRoot))
            {
                continue;
            }

            foreach (var path in Directory.EnumerateFiles(absoluteRoot, "*.cs", SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(path);
                if (TeaSharpCoreImportRegex.IsMatch(text) || text.Contains("TeaSharp.Core.", StringComparison.Ordinal))
                {
                    offenders.Add(ToRepoRelativePath(path));
                }
            }
        }

        TestAssert.True(
            offenders.Count == 0,
            $"Onboarding source files must not reference TeaSharp.Core.*. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task FlagshipExampleProjects_DoNotReferenceTeaSharpCoreProject()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => File.ReadAllText(Path.Combine(repoRoot, path)).Contains("TeaSharp.Core.csproj", StringComparison.Ordinal))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship example projects must not reference TeaSharp.Core.csproj directly. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task PublicTeaSharpAssemblySurface_DoesNotExposeTeaSharpCoreTypes()
    {
        var publicAssembly = typeof(TeaApp).Assembly;
        var leakedMembers = new List<string>();
        var publicTypes = publicAssembly.GetExportedTypes();

        foreach (var type in publicTypes)
        {
            if (!IsOnboardingPublicType(type))
            {
                continue;
            }

            if (ContainsTeaSharpCoreType(type))
            {
                leakedMembers.Add($"{type.FullName} (type)");
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

            foreach (var constructor in type.GetConstructors(flags))
            {
                if (constructor.GetParameters().Any(parameter => ContainsTeaSharpCoreType(parameter.ParameterType)))
                {
                    leakedMembers.Add($"{type.FullName}.{constructor.Name}(...)");
                }
            }

            foreach (var method in type.GetMethods(flags).Where(method => !method.IsSpecialName))
            {
                if (ContainsTeaSharpCoreType(method.ReturnType)
                    || method.GetParameters().Any(parameter => ContainsTeaSharpCoreType(parameter.ParameterType)))
                {
                    leakedMembers.Add($"{type.FullName}.{method.Name}(...)");
                }
            }

            foreach (var property in type.GetProperties(flags))
            {
                if (ContainsTeaSharpCoreType(property.PropertyType))
                {
                    leakedMembers.Add($"{type.FullName}.{property.Name}");
                }
            }

            foreach (var @event in type.GetEvents(flags))
            {
                if (ContainsTeaSharpCoreType(@event.EventHandlerType))
                {
                    leakedMembers.Add($"{type.FullName}.{@event.Name}");
                }
            }
        }

        TestAssert.True(
            leakedMembers.Count == 0,
            $"Public TeaSharp surface must not expose TeaSharp.Core types. Offenders: {string.Join(", ", leakedMembers)}.");

        return Task.CompletedTask;
    }

    private static Task ThemeScopePublicMethods_DoNotExposeTeaSharpCoreOrHostingTypes()
    {
        var offenders = new List<string>();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        var methods = typeof(ThemeScope).GetMethods(flags);

        for (var index = 0; index < methods.Length; index++)
        {
            var method = methods[index];
            if (ContainsTeaSharpCoreType(method.ReturnType) || ContainsTeaSharpHostingType(method.ReturnType))
            {
                offenders.Add($"{nameof(ThemeScope)}.{method.Name}(...) return {method.ReturnType.Name}");
            }

            var parameters = method.GetParameters();
            for (var parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];
                if (ContainsTeaSharpCoreType(parameter.ParameterType) || ContainsTeaSharpHostingType(parameter.ParameterType))
                {
                    offenders.Add($"{nameof(ThemeScope)}.{method.Name}(...) param {parameter.Name}:{parameter.ParameterType.Name}");
                }
            }
        }

        TestAssert.True(
            offenders.Count == 0,
            $"ThemeScope must remain onboarding-safe and not expose TeaSharp.Core/TeaSharp.Hosting surface types. Offenders: {string.Join(", ", offenders)}.");

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

    private static bool ContainsTeaSharpCoreType(Type? type)
    {
        return ContainsNamespaceType(type, "TeaSharp.Core");
    }

    private static bool ContainsTeaSharpHostingType(Type? type)
    {
        return ContainsNamespaceType(type, "TeaSharp.Hosting");
    }

    private static bool ContainsNamespaceType(Type? type, string namespacePrefix)
    {
        if (type is null)
        {
            return false;
        }

        if (type.Namespace?.StartsWith(namespacePrefix, StringComparison.Ordinal) == true)
        {
            return true;
        }

        if (type.IsByRef || type.IsPointer || type.IsArray)
        {
            return ContainsTeaSharpCoreType(type.GetElementType());
        }

        if (!type.IsGenericType)
        {
            return false;
        }

        foreach (var genericArgument in type.GetGenericArguments())
        {
            if (ContainsNamespaceType(genericArgument, namespacePrefix))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsOnboardingPublicType(Type type)
    {
        var @namespace = type.Namespace ?? string.Empty;
        return @namespace.Equals("TeaSharp", StringComparison.Ordinal)
            || @namespace.StartsWith("TeaSharp.Controls", StringComparison.Ordinal)
            || @namespace.StartsWith("TeaSharp.Layout", StringComparison.Ordinal);
    }
}
