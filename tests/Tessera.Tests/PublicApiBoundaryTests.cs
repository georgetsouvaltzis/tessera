using System.Reflection;
using System.Text.RegularExpressions;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class PublicApiBoundaryTests
{
    private static readonly Regex TesseraCoreImportRegex =
        new(@"(?m)^\s*using\s+.*Tessera\.Core.*;", RegexOptions.Compiled);

    private static readonly Regex TesseraHostingImportRegex =
        new(@"(?m)^\s*using\s+.*Tessera\.Hosting.*;", RegexOptions.Compiled);

    private static readonly Regex DependencyInjectionImportRegex =
        new(@"(?m)^\s*using\s+.*Microsoft\.Extensions\.DependencyInjection.*;", RegexOptions.Compiled);

    private static readonly string[] FlagshipExampleProjectPaths =
    [
        "examples/DataWorkbench/DataWorkbench.csproj",
        "examples/OpsWatch/OpsWatch.csproj",
        "examples/GitConsole/GitConsole.csproj"
    ];

    private static readonly string[] CoreImportAllowList =
    [
    ];

    private static readonly string[] OnboardingSourceRoots =
    [
        "src/Tessera/Controls",
        "src/Tessera/Layout"
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "PublicApiBoundary_ExamplesDoNotImportTesseraCoreOutsideAllowList",
            Examples_DoNotImportTesseraCoreOutsideAllowList);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProjectsExist",
            FlagshipExampleProjects_Exist);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProgramsDoNotImportTesseraCore",
            FlagshipExamplePrograms_DoNotImportTesseraCore);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProgramsDoNotImportDependencyInjection",
            FlagshipExamplePrograms_DoNotImportDependencyInjection);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProgramsDoNotImportTesseraHosting",
            FlagshipExamplePrograms_DoNotImportTesseraHosting);
        yield return new TestCase(
            "PublicApiBoundary_OnboardingSourceFilesDoNotReferenceTesseraCore",
            OnboardingSourceFiles_DoNotReferenceTesseraCore);
        yield return new TestCase(
            "PublicApiBoundary_FlagshipExampleProjectsDoNotReferenceTesseraCoreProject",
            FlagshipExampleProjects_DoNotReferenceTesseraCoreProject);
        yield return new TestCase(
            "PublicApiBoundary_PublicTesseraAssemblySurfaceDoesNotExposeTesseraCoreTypes",
            PublicTesseraAssemblySurface_DoesNotExposeTesseraCoreTypes);
        yield return new TestCase(
            "PublicApiBoundary_ThemeScopePublicMethods_DoNotExposeTesseraCoreOrHostingTypes",
            ThemeScopePublicMethods_DoNotExposeTesseraCoreOrHostingTypes);
    }

    private static Task Examples_DoNotImportTesseraCoreOutsideAllowList()
    {
        var repoRoot = GetRepoRoot();
        var offenders = Directory
            .EnumerateFiles(Path.Combine(repoRoot, "examples"), "Program.cs", SearchOption.AllDirectories)
            .Where(path => !IsAllowListed(path))
            .Where(path => TesseraCoreImportRegex.IsMatch(File.ReadAllText(path)))
            .Select(ToRepoRelativePath)
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Example apps must not import Tessera.Core.* outside the allow-list. Offenders: {string.Join(", ", offenders)}.");

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

    private static Task FlagshipExamplePrograms_DoNotImportTesseraCore()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Select(path => path.Replace(".csproj", "/Program.cs", StringComparison.Ordinal))
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => TesseraCoreImportRegex.IsMatch(File.ReadAllText(Path.Combine(repoRoot, path))))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship examples must not import Tessera.Core.*. Offenders: {string.Join(", ", offenders)}.");

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

    private static Task FlagshipExamplePrograms_DoNotImportTesseraHosting()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Select(path => path.Replace(".csproj", "/Program.cs", StringComparison.Ordinal))
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => TesseraHostingImportRegex.IsMatch(File.ReadAllText(Path.Combine(repoRoot, path))))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship examples should not import Tessera.Hosting in the default public path. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task OnboardingSourceFiles_DoNotReferenceTesseraCore()
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
                if (TesseraCoreImportRegex.IsMatch(text) || text.Contains("Tessera.Core.", StringComparison.Ordinal))
                {
                    offenders.Add(ToRepoRelativePath(path));
                }
            }
        }

        TestAssert.True(
            offenders.Count == 0,
            $"Onboarding source files must not reference Tessera.Core.*. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task FlagshipExampleProjects_DoNotReferenceTesseraCoreProject()
    {
        var repoRoot = GetRepoRoot();
        var offenders = FlagshipExampleProjectPaths
            .Where(path => File.Exists(Path.Combine(repoRoot, path)))
            .Where(path => File.ReadAllText(Path.Combine(repoRoot, path))
                .Contains("Tessera.Core.csproj", StringComparison.Ordinal))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Flagship example projects must not reference Tessera.Core.csproj directly. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static Task PublicTesseraAssemblySurface_DoesNotExposeTesseraCoreTypes()
    {
        var publicAssembly = typeof(TesseraApp).Assembly;
        var leakedMembers = new List<string>();
        var publicTypes = publicAssembly.GetExportedTypes();

        foreach (var type in publicTypes)
        {
            if (!IsOnboardingPublicType(type))
            {
                continue;
            }

            if (ContainsTesseraCoreType(type))
            {
                leakedMembers.Add($"{type.FullName} (type)");
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.DeclaredOnly;

            foreach (var constructor in type.GetConstructors(flags))
            {
                if (constructor.GetParameters().Any(parameter => ContainsTesseraCoreType(parameter.ParameterType)))
                {
                    leakedMembers.Add($"{type.FullName}.{constructor.Name}(...)");
                }
            }

            foreach (var method in type.GetMethods(flags).Where(method => !method.IsSpecialName))
            {
                if (ContainsTesseraCoreType(method.ReturnType)
                    || method.GetParameters().Any(parameter => ContainsTesseraCoreType(parameter.ParameterType)))
                {
                    leakedMembers.Add($"{type.FullName}.{method.Name}(...)");
                }
            }

            foreach (var property in type.GetProperties(flags))
            {
                if (ContainsTesseraCoreType(property.PropertyType))
                {
                    leakedMembers.Add($"{type.FullName}.{property.Name}");
                }
            }

            foreach (var @event in type.GetEvents(flags))
            {
                if (ContainsTesseraCoreType(@event.EventHandlerType))
                {
                    leakedMembers.Add($"{type.FullName}.{@event.Name}");
                }
            }
        }

        TestAssert.True(
            leakedMembers.Count == 0,
            $"Public Tessera surface must not expose Tessera.Core types. Offenders: {string.Join(", ", leakedMembers)}.");

        return Task.CompletedTask;
    }

    private static Task ThemeScopePublicMethods_DoNotExposeTesseraCoreOrHostingTypes()
    {
        var offenders = new List<string>();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        var methods = typeof(ThemeScope).GetMethods(flags);

        for (var index = 0; index < methods.Length; index++)
        {
            var method = methods[index];
            if (ContainsTesseraCoreType(method.ReturnType) || ContainsTesseraHostingType(method.ReturnType))
            {
                offenders.Add($"{nameof(ThemeScope)}.{method.Name}(...) return {method.ReturnType.Name}");
            }

            var parameters = method.GetParameters();
            for (var parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];
                if (ContainsTesseraCoreType(parameter.ParameterType) ||
                    ContainsTesseraHostingType(parameter.ParameterType))
                {
                    offenders.Add(
                        $"{nameof(ThemeScope)}.{method.Name}(...) param {parameter.Name}:{parameter.ParameterType.Name}");
                }
            }
        }

        TestAssert.True(
            offenders.Count == 0,
            $"ThemeScope must remain onboarding-safe and not expose Tessera.Core/Tessera.Hosting surface types. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
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

        throw new InvalidOperationException("Could not locate the Tessera repository root.");
    }

    private static string ToRepoRelativePath(string path)
    {
        return Path.GetRelativePath(GetRepoRoot(), path).Replace(Path.DirectorySeparatorChar, '/');
    }

    private static bool ContainsTesseraCoreType(Type? type)
    {
        return ContainsNamespaceType(type, "Tessera.Core");
    }

    private static bool ContainsTesseraHostingType(Type? type)
    {
        return ContainsNamespaceType(type, "Tessera.Hosting");
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
            return ContainsTesseraCoreType(type.GetElementType());
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
        return @namespace.Equals("Tessera", StringComparison.Ordinal)
               || @namespace.StartsWith("Tessera.Controls", StringComparison.Ordinal)
               || @namespace.StartsWith("Tessera.Layout", StringComparison.Ordinal);
    }
}
