using System.Xml.Linq;

namespace TeaSharp.Tests;

internal static class PublicApiXmlDocsTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "PublicApiXmlDocs_RootTypes_HaveSummaries",
            RootTypes_HaveSummaries);
        yield return new TestCase(
            "PublicApiXmlDocs_MentalModelTypes_HaveRemarks",
            MentalModelTypes_HaveRemarks);
        yield return new TestCase(
            "PublicApiXmlDocs_KeyMembers_HaveSummaries",
            KeyMembers_HaveSummaries);
    }

    private static Task RootTypes_HaveSummaries()
    {
        string[] memberNames =
        [
            "T:TeaSharp.TeaApp",
            "T:TeaSharp.Tea",
            "T:TeaSharp.TeaApplication",
            "T:TeaSharp.TeaApplicationBuilder",
            "T:TeaSharp.TeaRuntimeOptions",
            "T:TeaSharp.Screen",
            "T:TeaSharp.ScreenContext",
            "T:TeaSharp.ScreenOptions",
            "T:TeaSharp.Message",
            "T:TeaSharp.TeaEffect",
            "T:TeaSharp.TeaEffects",
        ];

        var docs = LoadDocumentation();
        foreach (var memberName in memberNames)
        {
            AssertTagHasContent(docs, memberName, "summary");
        }

        return Task.CompletedTask;
    }

    private static Task MentalModelTypes_HaveRemarks()
    {
        string[] memberNames =
        [
            "T:TeaSharp.TeaApp",
            "T:TeaSharp.Tea",
            "T:TeaSharp.Screen",
            "T:TeaSharp.TeaRuntimeOptions",
            "T:TeaSharp.ScreenOptions",
            "T:TeaSharp.Message",
            "T:TeaSharp.TeaEffect",
        ];

        var docs = LoadDocumentation();
        foreach (var memberName in memberNames)
        {
            AssertTagHasContent(docs, memberName, "remarks");
        }

        return Task.CompletedTask;
    }

    private static Task KeyMembers_HaveSummaries()
    {
        string[] memberNames =
        [
            "M:TeaSharp.TeaApp.Initialize",
            "M:TeaSharp.TeaApp.Update(TeaSharp.Message)",
            "M:TeaSharp.TeaApp.Build(TeaSharp.ScreenContext)",
            "M:TeaSharp.TeaApplication.RunAsync(System.Threading.CancellationToken)",
            "M:TeaSharp.TeaApplicationBuilder.UseApp``1",
            "M:TeaSharp.TeaApplicationBuilder.ConfigureRuntime(System.Action{TeaSharp.TeaRuntimeOptions})",
            "M:TeaSharp.Screen.From(TeaSharp.Controls.Control)",
            "M:TeaSharp.ScreenContext.CreateCanvas(TeaSharp.Components.Primitives.CanvasTextMode)",
            "M:TeaSharp.TeaEffects.Emit(TeaSharp.Message)",
            "M:TeaSharp.TeaEffects.Tick(System.TimeSpan,System.Func{System.DateTimeOffset,TeaSharp.Message})",
        ];

        var docs = LoadDocumentation();
        foreach (var memberName in memberNames)
        {
            AssertTagHasContent(docs, memberName, "summary");
        }

        return Task.CompletedTask;
    }

    private static XDocument LoadDocumentation()
    {
        var assemblyPath = typeof(TeaApp).Assembly.Location;
        var xmlPath = Path.ChangeExtension(assemblyPath, ".xml");

        TestAssert.True(File.Exists(xmlPath), $"Expected XML documentation file at {xmlPath}.");
        return XDocument.Load(xmlPath);
    }

    private static void AssertTagHasContent(XDocument docs, string memberName, string tagName)
    {
        var member = docs.Root?
            .Element("members")?
            .Elements("member")
            .SingleOrDefault(element => string.Equals((string?)element.Attribute("name"), memberName, StringComparison.Ordinal));

        TestAssert.True(member is not null, $"Expected XML documentation member {memberName}.");

        var tag = member!.Element(tagName);
        var content = tag?.Value?.Trim();

        TestAssert.True(!string.IsNullOrWhiteSpace(content), $"{memberName} should include a non-empty <{tagName}> tag.");
    }
}
