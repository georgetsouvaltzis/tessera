using System.Text.RegularExpressions;

namespace TeaSharp.Tests;

internal static class PublicOnboardingExamplesTests
{
    private static readonly Regex TeaSharpCoreImportRegex = new(@"(?m)^\s*using\s+.*TeaSharp\.Core.*;", RegexOptions.Compiled);
    private static readonly Regex TeaSharpHostingImportRegex = new(@"(?m)^\s*using\s+.*TeaSharp\.Hosting.*;", RegexOptions.Compiled);
    private static readonly Regex DependencyInjectionImportRegex = new(@"(?m)^\s*using\s+.*Microsoft\.Extensions\.DependencyInjection.*;", RegexOptions.Compiled);

    private static readonly string[] OnboardingExampleProjectPaths =
    [
        "examples/HelloWorld/HelloWorld.csproj",
        "examples/CounterForm/CounterForm.csproj",
        "examples/WorkspaceApp/WorkspaceApp.csproj",
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "PublicOnboardingExamples_ProjectsExist",
            Projects_Exist);
        yield return new TestCase(
            "PublicOnboardingExamples_ProgramsDoNotImportTeaSharpCore",
            Programs_DoNotImportTeaSharpCore);
        yield return new TestCase(
            "PublicOnboardingExamples_ProgramsDoNotImportDependencyInjection",
            Programs_DoNotImportDependencyInjection);
        yield return new TestCase(
            "PublicOnboardingExamples_ProgramsDoNotImportTeaSharpHosting",
            Programs_DoNotImportTeaSharpHosting);
        yield return new TestCase(
            "PublicOnboardingExamples_ProjectsDoNotReferenceTeaSharpCoreProject",
            Projects_DoNotReferenceTeaSharpCoreProject);
    }

    private static Task Projects_Exist()
    {
        var repoRoot = GetRepoRoot();
        var missing = OnboardingExampleProjectPaths
            .Where(path => !File.Exists(Path.Combine(repoRoot, path)))
            .ToArray();

        TestAssert.True(
            missing.Length == 0,
            $"Onboarding example projects are missing: {string.Join(", ", missing)}.");

        return Task.CompletedTask;
    }

    private static Task Programs_DoNotImportTeaSharpCore()
    {
        AssertProgramsDoNotMatch(
            TeaSharpCoreImportRegex,
            "Onboarding examples must not import TeaSharp.Core.*.");
        return Task.CompletedTask;
    }

    private static Task Programs_DoNotImportDependencyInjection()
    {
        AssertProgramsDoNotMatch(
            DependencyInjectionImportRegex,
            "Onboarding examples should not depend on Microsoft.Extensions.DependencyInjection.");
        return Task.CompletedTask;
    }

    private static Task Programs_DoNotImportTeaSharpHosting()
    {
        AssertProgramsDoNotMatch(
            TeaSharpHostingImportRegex,
            "Onboarding examples should not import TeaSharp.Hosting in the default public path.");
        return Task.CompletedTask;
    }

    private static Task Projects_DoNotReferenceTeaSharpCoreProject()
    {
        var repoRoot = GetRepoRoot();
        var offenders = OnboardingExampleProjectPaths
            .Where(path => File.ReadAllText(Path.Combine(repoRoot, path)).Contains("TeaSharp.Core.csproj", StringComparison.Ordinal))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"Onboarding example projects must not reference TeaSharp.Core.csproj directly. Offenders: {string.Join(", ", offenders)}.");

        return Task.CompletedTask;
    }

    private static void AssertProgramsDoNotMatch(Regex regex, string message)
    {
        var repoRoot = GetRepoRoot();
        var offenders = OnboardingExampleProjectPaths
            .Select(path => path.Replace(".csproj", "/Program.cs", StringComparison.Ordinal))
            .Where(path => regex.IsMatch(File.ReadAllText(Path.Combine(repoRoot, path))))
            .ToArray();

        TestAssert.True(
            offenders.Length == 0,
            $"{message} Offenders: {string.Join(", ", offenders)}.");
    }

    private static string GetRepoRoot()
    {
        var start = Path.GetDirectoryName(typeof(PublicOnboardingExamplesTests).Assembly.Location)
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
