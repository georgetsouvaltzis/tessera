using System.Xml.Linq;

namespace Tessera.Tests;

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
        yield return new TestCase(
            "PublicApiXmlDocs_PlottingErgonomicsApis_HaveSummaries",
            PlottingErgonomicsApis_HaveSummaries);
        yield return new TestCase(
            "PublicApiXmlDocs_DashboardAndOverlayApis_HaveSummaries",
            DashboardAndOverlayApis_HaveSummaries);
        yield return new TestCase(
            "PublicApiXmlDocs_RecentSurfaceArea_HasSummaries",
            RecentSurfaceArea_HasSummaries);
    }

    private static Task RootTypes_HaveSummaries()
    {
        string[] memberNames =
        [
            "T:Tessera.TesseraApp",
            "T:Tessera.TesseraApplication",
            "T:Tessera.TesseraApplicationBuilder",
            "T:Tessera.TesseraRuntimeOptions",
            "T:Tessera.Screen",
            "T:Tessera.ScreenContext",
            "T:Tessera.ScreenOptions",
            "T:Tessera.Message",
            "T:Tessera.TesseraEffect",
            "T:Tessera.TesseraEffects"
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
            "T:Tessera.TesseraApp",
            "T:Tessera.Screen",
            "T:Tessera.TesseraRuntimeOptions",
            "T:Tessera.ScreenOptions",
            "T:Tessera.Message",
            "T:Tessera.TesseraEffect"
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
            "M:Tessera.TesseraApp.Initialize",
            "M:Tessera.TesseraApp.Post(Tessera.Message)",
            "M:Tessera.TesseraApp.Update(Tessera.Message)",
            "M:Tessera.TesseraApp.Build(Tessera.ScreenContext)",
            "M:Tessera.Screen.Build(System.Action{Tessera.Layout.WindowBuilder})",
            "M:Tessera.TesseraApplication.CreateBuilder",
            "M:Tessera.TesseraApplication.CreateApplication(Tessera.TesseraApp,Tessera.TesseraRuntimeOptions)",
            "M:Tessera.TesseraApplication.RunAsync(Tessera.TesseraApp,Tessera.TesseraRuntimeOptions,System.Threading.CancellationToken)",
            "M:Tessera.TesseraApplication.RunAsync(System.Threading.CancellationToken)",
            "M:Tessera.TesseraApplicationBuilder.UseApp``1",
            "M:Tessera.TesseraApplicationBuilder.ConfigureRuntime(System.Action{Tessera.TesseraRuntimeOptions})",
            "M:Tessera.Screen.From(Tessera.Controls.Control)",
            "M:Tessera.ScreenContext.CreateCanvas(Tessera.Components.Primitives.CanvasTextMode)",
            "M:Tessera.TesseraEffects.Emit(Tessera.Message)",
            "M:Tessera.TesseraEffects.Tick(System.TimeSpan,System.Func{System.DateTimeOffset,Tessera.Message})"
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
            "T:Tessera.Controls.Control",
            "T:Tessera.Controls.Accordion",
            "T:Tessera.Controls.AccordionSection",
            "T:Tessera.Controls.Badge",
            "T:Tessera.Controls.BadgeTone",
            "T:Tessera.Controls.BarChart",
            "T:Tessera.Controls.BarPoint",
            "T:Tessera.Controls.BarChartOptions",
            "T:Tessera.Controls.Button",
            "T:Tessera.Controls.Breadcrumb",
            "T:Tessera.Controls.BreadcrumbItem",
            "T:Tessera.Controls.BreadcrumbSelectionChangedEventArgs",
            "T:Tessera.Controls.CommandBar",
            "T:Tessera.Controls.CommandBarItem",
            "T:Tessera.Controls.CommandBarItemActivatedEventArgs",
            "T:Tessera.Controls.CommandPalette",
            "T:Tessera.Controls.CommandPaletteItem",
            "T:Tessera.Controls.CommandPaletteItemExecutedEventArgs",
            "T:Tessera.Controls.ContextMenu",
            "T:Tessera.Controls.ContextMenuItem",
            "T:Tessera.Controls.ContextMenuItemExecutedEventArgs",
            "T:Tessera.Controls.TextInput",
            "T:Tessera.Controls.TextInputSubmittedEventArgs",
            "T:Tessera.Controls.TextInputCancelledEventArgs",
            "T:Tessera.Controls.TextArea",
            "T:Tessera.Controls.Choice",
            "T:Tessera.Controls.SelectionChangedEventArgs",
            "T:Tessera.Controls.ComboBox",
            "T:Tessera.Controls.DatePicker",
            "T:Tessera.Controls.DateChangedEventArgs",
            "T:Tessera.Controls.DataGrid",
            "T:Tessera.Controls.DataGridColumn",
            "T:Tessera.Controls.DataGridSortDirection",
            "T:Tessera.Controls.DataGridSortRequestedEventArgs",
            "T:Tessera.Controls.Dialog",
            "T:Tessera.Controls.DialogResult",
            "T:Tessera.Controls.DiffView",
            "T:Tessera.Controls.DiffLineEntry",
            "T:Tessera.Controls.DiffLineKind",
            "T:Tessera.Controls.DiffViewMode",
            "T:Tessera.Controls.FileExplorer",
            "T:Tessera.Controls.FileExplorerItem",
            "T:Tessera.Controls.FileExplorerSelectionChangedEventArgs",
            "T:Tessera.Controls.FuzzyFinder",
            "T:Tessera.Controls.FuzzyFinderItem",
            "T:Tessera.Controls.FuzzyFinderSelectionChangedEventArgs",
            "T:Tessera.Controls.FuzzyFinderItemSelectedEventArgs",
            "T:Tessera.Controls.Gauge",
            "T:Tessera.Controls.KeyValueList",
            "T:Tessera.Controls.KeyValueListEntry",
            "T:Tessera.Controls.KeyValueListSelectionChangedEventArgs",
            "T:Tessera.Controls.ListView`1",
            "T:Tessera.Controls.ListSelectionChangedEventArgs`1",
            "T:Tessera.Controls.Label",
            "T:Tessera.Controls.LineChart",
            "T:Tessera.Controls.LineChartOptions",
            "T:Tessera.Controls.LogView",
            "T:Tessera.Controls.MarkdownView",
            "T:Tessera.Controls.MenuBar",
            "T:Tessera.Controls.MenuItem",
            "T:Tessera.Controls.MenuItemActivatedEventArgs",
            "T:Tessera.Controls.MiniLog",
            "T:Tessera.Controls.Modal",
            "T:Tessera.Controls.MultiSelect",
            "T:Tessera.Controls.NotificationLevel",
            "T:Tessera.Controls.Notifications",
            "T:Tessera.Controls.NumberInput",
            "T:Tessera.Controls.NumberInputSubmittedEventArgs",
            "T:Tessera.Controls.ProgressBar",
            "T:Tessera.Controls.Paginator",
            "T:Tessera.Controls.PageChangedEventArgs",
            "T:Tessera.Controls.PropertyGrid",
            "T:Tessera.Controls.PropertyGridProperty",
            "T:Tessera.Controls.PropertyGridSelectionChangedEventArgs",
            "T:Tessera.Controls.RadioGroup",
            "T:Tessera.Controls.SearchBox",
            "T:Tessera.Controls.SearchBoxQueryChangedEventArgs",
            "T:Tessera.Controls.SearchBoxNavigationRequestedEventArgs",
            "T:Tessera.Controls.SearchNavigationDirection",
            "T:Tessera.Controls.Stepper",
            "T:Tessera.Controls.StepperStep",
            "T:Tessera.Controls.StepperCurrentStepChangedEventArgs",
            "T:Tessera.Controls.Slider",
            "T:Tessera.Controls.Spinner",
            "T:Tessera.Controls.StatItem",
            "T:Tessera.Controls.StatsCard",
            "T:Tessera.Controls.StatusBar",
            "T:Tessera.Controls.Table",
            "T:Tessera.Controls.Tabs",
            "T:Tessera.Controls.TimeField",
            "T:Tessera.Controls.TimePicker",
            "T:Tessera.Controls.TimeValueChangedEventArgs",
            "T:Tessera.Controls.Timeline",
            "T:Tessera.Controls.TimelineEntry",
            "T:Tessera.Controls.TimelineSelectionChangedEventArgs",
            "T:Tessera.Controls.ToastCenter",
            "T:Tessera.Controls.ToastItem",
            "T:Tessera.Controls.Toolbar",
            "T:Tessera.Controls.ToolbarItem",
            "T:Tessera.Controls.ToolbarSelectionChangedEventArgs",
            "T:Tessera.Controls.Toggle",
            "T:Tessera.Controls.TreeTable",
            "T:Tessera.Controls.TreeTableNode",
            "T:Tessera.Controls.TreeTableSelectionChangedEventArgs",
            "T:Tessera.Controls.TreeItem",
            "T:Tessera.Controls.TreeView",
            "T:Tessera.Layout.LayoutSlot",
            "T:Tessera.Layout.WindowBuilder",
            "T:Tessera.Layout.ContentBuilder",
            "T:Tessera.Layout.StackBuilder",
            "T:Tessera.Layout.PanelBuilder",
            "T:Tessera.Layout.CenterLayout",
            "T:Tessera.Layout.PanelLayout",
            "T:Tessera.Layout.WindowLayout",
            "T:Tessera.Layout.RowLayout",
            "T:Tessera.Layout.ColumnLayout",
            "T:Tessera.Components.Primitives.Canvas",
            "T:Tessera.Thickness",
            "T:Tessera.BorderStyle"
        ];

        var docs = LoadDocumentation();
        foreach (var memberName in memberNames)
        {
            AssertTagHasContent(docs, memberName, "summary");
        }

        return Task.CompletedTask;
    }

    private static Task PlottingErgonomicsApis_HaveSummaries()
    {
        string[] memberNames =
        [
            "T:Tessera.Controls.LinePlot",
            "T:Tessera.Controls.LineSeries",
            "T:Tessera.Controls.LineSeriesScaleMode",
            "T:Tessera.Controls.Sparkline",
            "T:Tessera.Controls.ScatterPlot",
            "M:Tessera.Controls.LinePlot.ConfigureAxes(System.Boolean,System.String,System.String,System.String)",
            "M:Tessera.Controls.LinePlot.ConfigureGrid(System.Boolean)",
            "M:Tessera.Controls.LinePlot.ConfigureLegend(System.Boolean)",
            "M:Tessera.Controls.LineSeries.TrimToLast(System.Int32)",
            "M:Tessera.Controls.ScatterPlot.TrimToLast(System.Int32)",
            "M:Tessera.Controls.Sparkline.TrimToLast(System.Int32)",
            "P:Tessera.Controls.LineSeries.Capacity",
            "P:Tessera.Controls.ScatterPlot.Capacity",
            "P:Tessera.Controls.LineSeries.ScaleMode"
        ];

        var docs = LoadDocumentation();
        foreach (var memberName in memberNames)
        {
            AssertTagHasContent(docs, memberName, "summary");
        }

        return Task.CompletedTask;
    }

    private static Task DashboardAndOverlayApis_HaveSummaries()
    {
        string[] memberNames =
        [
            "T:Tessera.Controls.DashboardGrid",
            "T:Tessera.Controls.DashboardTile",
            "M:Tessera.Controls.DashboardGrid.SetTiles(System.Collections.Generic.IEnumerable{Tessera.Controls.DashboardTile})",
            "M:Tessera.Controls.DashboardGrid.SetSelectedIndex(System.Int32)",
            "T:Tessera.Controls.BulletChart",
            "T:Tessera.Controls.BulletRange",
            "T:Tessera.Controls.BulletRangeKind",
            "M:Tessera.Controls.BulletChart.SetRanges(System.Collections.Generic.IEnumerable{Tessera.Controls.BulletRange})",
            "M:Tessera.Controls.BulletChart.SetValue(System.Double)",
            "M:Tessera.Controls.BulletChart.SetTarget(System.Double)",
            "T:Tessera.Controls.HealthBoard",
            "T:Tessera.Controls.HealthService",
            "T:Tessera.Controls.HealthServiceSeverity",
            "M:Tessera.Controls.HealthBoard.SetServices(System.Collections.Generic.IEnumerable{Tessera.Controls.HealthService})",
            "M:Tessera.Controls.HealthBoard.SetSelectedIndex(System.Int32)",
            "T:Tessera.Controls.SideNavRail",
            "T:Tessera.Controls.NavItem",
            "T:Tessera.Controls.SideNavRailGlyphSet",
            "T:Tessera.Controls.SideNavRailSelectionChangedEventArgs",
            "T:Tessera.Controls.SideNavRailActivatedEventArgs",
            "M:Tessera.Controls.SideNavRail.SetItems(System.Collections.Generic.IEnumerable{Tessera.Controls.NavItem})",
            "M:Tessera.Controls.SideNavRail.SetSelectedIndex(System.Int32)",
            "T:Tessera.Controls.ResizablePaneGroup",
            "T:Tessera.Controls.PaneSpec",
            "M:Tessera.Controls.ResizablePaneGroup.SetPanes(System.Collections.Generic.IEnumerable{Tessera.Controls.PaneSpec})",
            "M:Tessera.Controls.ResizablePaneGroup.SetSelectedPaneIndex(System.Int32)",
            "T:Tessera.Controls.JumpList",
            "T:Tessera.Controls.JumpListItem",
            "T:Tessera.Controls.JumpListGlyphSet",
            "T:Tessera.Controls.JumpListActivatedEventArgs",
            "M:Tessera.Controls.JumpList.SetItems(System.Collections.Generic.IEnumerable{Tessera.Controls.JumpListItem})",
            "M:Tessera.Controls.JumpList.SetSelectedIndex(System.Int32)",
            "T:Tessera.Controls.AutocompleteInput",
            "T:Tessera.Controls.AutocompleteInputGlyphSet",
            "T:Tessera.Controls.AutocompleteInputSuggestionCommittedEventArgs",
            "M:Tessera.Controls.AutocompleteInput.SetSuggestions(System.Collections.Generic.IEnumerable{System.String})",
            "M:Tessera.Controls.AutocompleteInput.SetSelectedSuggestionIndex(System.Int32)",
            "T:Tessera.Controls.QuickOpenOverlay",
            "T:Tessera.Controls.QuickOpenItem",
            "T:Tessera.Controls.QuickOpenOverlayGlyphSet",
            "T:Tessera.Controls.QuickOpenOverlaySubmittedEventArgs",
            "M:Tessera.Controls.QuickOpenOverlay.SetItems(System.Collections.Generic.IEnumerable{Tessera.Controls.QuickOpenItem})",
            "M:Tessera.Controls.QuickOpenOverlay.SetSelectedIndex(System.Int32)",
            "M:Tessera.Controls.QuickOpenOverlay.Open",
            "M:Tessera.Controls.QuickOpenOverlay.Close"
        ];

        var docs = LoadDocumentation();
        foreach (var memberName in memberNames)
        {
            AssertTagHasContent(docs, memberName, "summary");
        }

        return Task.CompletedTask;
    }

    private static Task RecentSurfaceArea_HasSummaries()
    {
        string[] memberNames =
        [
            "P:Tessera.Controls.StatsCard.Border",
            "P:Tessera.Controls.StatsCard.Padding",
            "P:Tessera.Controls.StatsCard.BorderStyleText",
            "P:Tessera.Controls.StatsCard.FocusedBorderStyleText",
            "P:Tessera.Controls.Table.SelectedRowIndex",
            "P:Tessera.Controls.Table.SelectedRow",
            "E:Tessera.Controls.Table.SelectionChanged",
            "M:Tessera.Controls.Table.TryGetSelectedRow(System.Collections.Generic.IReadOnlyList{System.String}@)",
            "M:Tessera.Controls.ListView`1.SetSelectedIndex(System.Int32)",
            "P:Tessera.Controls.ResizablePaneGroup.SelectedIndex",
            "P:Tessera.Controls.ResizablePaneGroup.SelectedItem",
            "M:Tessera.Controls.ResizablePaneGroup.SetSelectedIndex(System.Int32)",
            "P:Tessera.Controls.ResizablePaneGroup.TitleStyle",
            "P:Tessera.Controls.ResizablePaneGroup.FocusedTitleStyle",
            "P:Tessera.Controls.DashboardGrid.TitleStyle",
            "P:Tessera.Controls.DashboardGrid.FocusedTitleStyle",
            "T:Tessera.Styles.ThemeScope",
            "M:Tessera.Styles.ThemeScope.Apply(Tessera.Styles.TesseraTheme,Tessera.Controls.Control[])",
            "M:Tessera.Styles.ThemeScope.Apply(Tessera.Styles.TesseraTheme,System.Collections.Generic.IEnumerable{Tessera.Controls.Control})",
            "P:Tessera.Controls.LinePlot.Series",
            "M:Tessera.Controls.LinePlot.SetSeries(System.Collections.Generic.IEnumerable{Tessera.Controls.LineSeries})",
            "M:Tessera.Controls.LinePlot.AddSeries(Tessera.Controls.LineSeries)",
            "M:Tessera.Controls.LinePlot.AppendSample(System.String,System.Double)",
            "M:Tessera.Controls.LineSeries.SetSamples(System.Collections.Generic.IEnumerable{System.Double})",
            "M:Tessera.Controls.LineSeries.Append(System.Double)",
            "M:Tessera.Controls.ScatterPlot.SetPoints(System.Collections.Generic.IEnumerable{Tessera.Controls.ScatterPlotPoint})",
            "M:Tessera.Controls.ScatterPlot.Append(Tessera.Controls.ScatterPlotPoint)",
            "M:Tessera.Controls.Sparkline.SetSamples(System.Collections.Generic.IEnumerable{System.Double})",
            "M:Tessera.Controls.Sparkline.Append(System.Double)",
            "P:Tessera.Controls.Spinner.Frames",
            "M:Tessera.Controls.Spinner.SetFrames(System.Collections.Generic.IEnumerable{System.String})"
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
        var assemblyPath = typeof(TesseraApp).Assembly.Location;
        var xmlPath = Path.ChangeExtension(assemblyPath, ".xml");

        TestAssert.True(File.Exists(xmlPath), $"Expected XML documentation file at {xmlPath}.");
        return XDocument.Load(xmlPath);
    }

    private static void AssertTagHasContent(XDocument docs, string memberName, string tagName)
    {
        var member = docs.Root?
            .Element("members")?
            .Elements("member")
            .SingleOrDefault(element =>
                string.Equals((string?)element.Attribute("name"), memberName, StringComparison.Ordinal));

        TestAssert.True(member is not null, $"Expected XML documentation member {memberName}.");

        var tag = member!.Element(tagName);
        var content = tag?.Value?.Trim();

        TestAssert.True(!string.IsNullOrWhiteSpace(content),
            $"{memberName} should include a non-empty <{tagName}> tag.");
    }
}
