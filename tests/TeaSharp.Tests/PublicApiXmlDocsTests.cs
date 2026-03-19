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
        yield return new TestCase(
            "PublicApiXmlDocs_EntryControlsAndLayouts_HaveSummaries",
            EntryControlsAndLayouts_HaveSummaries);
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
            "M:TeaSharp.TeaApp.Post(TeaSharp.Message)",
            "M:TeaSharp.TeaApp.Update(TeaSharp.Message)",
            "M:TeaSharp.TeaApp.Build(TeaSharp.ScreenContext)",
            "M:TeaSharp.Screen.Build(System.Action{TeaSharp.Layout.WindowBuilder})",
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

    private static Task EntryControlsAndLayouts_HaveSummaries()
    {
        string[] memberNames =
        [
            "T:TeaSharp.Controls.Control",
            "T:TeaSharp.Controls.Accordion",
            "T:TeaSharp.Controls.AccordionSection",
            "T:TeaSharp.Controls.Badge",
            "T:TeaSharp.Controls.BadgeTone",
            "T:TeaSharp.Controls.BarChart",
            "T:TeaSharp.Controls.BarPoint",
            "T:TeaSharp.Controls.BarChartOptions",
            "T:TeaSharp.Controls.Button",
            "T:TeaSharp.Controls.Breadcrumb",
            "T:TeaSharp.Controls.BreadcrumbItem",
            "T:TeaSharp.Controls.BreadcrumbSelectionChangedEventArgs",
            "T:TeaSharp.Controls.CommandBar",
            "T:TeaSharp.Controls.CommandBarItem",
            "T:TeaSharp.Controls.CommandBarItemActivatedEventArgs",
            "T:TeaSharp.Controls.CommandPalette",
            "T:TeaSharp.Controls.CommandPaletteItem",
            "T:TeaSharp.Controls.CommandPaletteItemExecutedEventArgs",
            "T:TeaSharp.Controls.ContextMenu",
            "T:TeaSharp.Controls.ContextMenuItem",
            "T:TeaSharp.Controls.ContextMenuItemExecutedEventArgs",
            "T:TeaSharp.Controls.TextInput",
            "T:TeaSharp.Controls.TextInputSubmittedEventArgs",
            "T:TeaSharp.Controls.TextInputCancelledEventArgs",
            "T:TeaSharp.Controls.TextArea",
            "T:TeaSharp.Controls.Choice",
            "T:TeaSharp.Controls.SelectionChangedEventArgs",
            "T:TeaSharp.Controls.ComboBox",
            "T:TeaSharp.Controls.DatePicker",
            "T:TeaSharp.Controls.DateChangedEventArgs",
            "T:TeaSharp.Controls.Dialog",
            "T:TeaSharp.Controls.DialogResult",
            "T:TeaSharp.Controls.DiffView",
            "T:TeaSharp.Controls.DiffLineEntry",
            "T:TeaSharp.Controls.DiffLineKind",
            "T:TeaSharp.Controls.DiffViewMode",
            "T:TeaSharp.Controls.Gauge",
            "T:TeaSharp.Controls.ListView`1",
            "T:TeaSharp.Controls.ListSelectionChangedEventArgs`1",
            "T:TeaSharp.Controls.Label",
            "T:TeaSharp.Controls.LineChart",
            "T:TeaSharp.Controls.LineChartOptions",
            "T:TeaSharp.Controls.LogView",
            "T:TeaSharp.Controls.MarkdownView",
            "T:TeaSharp.Controls.MenuBar",
            "T:TeaSharp.Controls.MenuItem",
            "T:TeaSharp.Controls.MenuItemActivatedEventArgs",
            "T:TeaSharp.Controls.MiniLog",
            "T:TeaSharp.Controls.Modal",
            "T:TeaSharp.Controls.MultiSelect",
            "T:TeaSharp.Controls.NotificationLevel",
            "T:TeaSharp.Controls.Notifications",
            "T:TeaSharp.Controls.NumberInput",
            "T:TeaSharp.Controls.NumberInputSubmittedEventArgs",
            "T:TeaSharp.Controls.ProgressBar",
            "T:TeaSharp.Controls.Paginator",
            "T:TeaSharp.Controls.PageChangedEventArgs",
            "T:TeaSharp.Controls.PropertyGrid",
            "T:TeaSharp.Controls.PropertyGridProperty",
            "T:TeaSharp.Controls.PropertyGridSelectionChangedEventArgs",
            "T:TeaSharp.Controls.RadioGroup",
            "T:TeaSharp.Controls.SearchBox",
            "T:TeaSharp.Controls.SearchBoxQueryChangedEventArgs",
            "T:TeaSharp.Controls.SearchBoxNavigationRequestedEventArgs",
            "T:TeaSharp.Controls.SearchNavigationDirection",
            "T:TeaSharp.Controls.Slider",
            "T:TeaSharp.Controls.Spinner",
            "T:TeaSharp.Controls.StatItem",
            "T:TeaSharp.Controls.StatsCard",
            "T:TeaSharp.Controls.StatusBar",
            "T:TeaSharp.Controls.Table",
            "T:TeaSharp.Controls.Tabs",
            "T:TeaSharp.Controls.TimeField",
            "T:TeaSharp.Controls.TimePicker",
            "T:TeaSharp.Controls.TimeValueChangedEventArgs",
            "T:TeaSharp.Controls.Toolbar",
            "T:TeaSharp.Controls.ToolbarItem",
            "T:TeaSharp.Controls.ToolbarSelectionChangedEventArgs",
            "T:TeaSharp.Controls.Toggle",
            "T:TeaSharp.Controls.TreeItem",
            "T:TeaSharp.Controls.TreeView",
            "T:TeaSharp.Layout.LayoutSlot",
            "T:TeaSharp.Layout.WindowBuilder",
            "T:TeaSharp.Layout.ContentBuilder",
            "T:TeaSharp.Layout.StackBuilder",
            "T:TeaSharp.Layout.PanelBuilder",
            "T:TeaSharp.Layout.CenterLayout",
            "T:TeaSharp.Layout.PanelLayout",
            "T:TeaSharp.Layout.WindowLayout",
            "T:TeaSharp.Layout.RowLayout",
            "T:TeaSharp.Layout.ColumnLayout",
            "T:TeaSharp.Components.Primitives.Canvas",
            "T:TeaSharp.Thickness",
            "T:TeaSharp.BorderStyle",
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
