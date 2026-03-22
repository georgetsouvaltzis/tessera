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
            "T:TeaSharp.Controls.DataGrid",
            "T:TeaSharp.Controls.DataGridColumn",
            "T:TeaSharp.Controls.DataGridSortDirection",
            "T:TeaSharp.Controls.DataGridSortRequestedEventArgs",
            "T:TeaSharp.Controls.Dialog",
            "T:TeaSharp.Controls.DialogResult",
            "T:TeaSharp.Controls.DiffView",
            "T:TeaSharp.Controls.DiffLineEntry",
            "T:TeaSharp.Controls.DiffLineKind",
            "T:TeaSharp.Controls.DiffViewMode",
            "T:TeaSharp.Controls.FileExplorer",
            "T:TeaSharp.Controls.FileExplorerItem",
            "T:TeaSharp.Controls.FileExplorerSelectionChangedEventArgs",
            "T:TeaSharp.Controls.FuzzyFinder",
            "T:TeaSharp.Controls.FuzzyFinderItem",
            "T:TeaSharp.Controls.FuzzyFinderSelectionChangedEventArgs",
            "T:TeaSharp.Controls.FuzzyFinderItemSelectedEventArgs",
            "T:TeaSharp.Controls.Gauge",
            "T:TeaSharp.Controls.KeyValueList",
            "T:TeaSharp.Controls.KeyValueListEntry",
            "T:TeaSharp.Controls.KeyValueListSelectionChangedEventArgs",
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
            "T:TeaSharp.Controls.Stepper",
            "T:TeaSharp.Controls.StepperStep",
            "T:TeaSharp.Controls.StepperCurrentStepChangedEventArgs",
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
            "T:TeaSharp.Controls.Timeline",
            "T:TeaSharp.Controls.TimelineEntry",
            "T:TeaSharp.Controls.TimelineSelectionChangedEventArgs",
            "T:TeaSharp.Controls.ToastCenter",
            "T:TeaSharp.Controls.ToastItem",
            "T:TeaSharp.Controls.Toolbar",
            "T:TeaSharp.Controls.ToolbarItem",
            "T:TeaSharp.Controls.ToolbarSelectionChangedEventArgs",
            "T:TeaSharp.Controls.Toggle",
            "T:TeaSharp.Controls.TreeTable",
            "T:TeaSharp.Controls.TreeTableNode",
            "T:TeaSharp.Controls.TreeTableSelectionChangedEventArgs",
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

    private static Task PlottingErgonomicsApis_HaveSummaries()
    {
        string[] memberNames =
        [
            "T:TeaSharp.Controls.LinePlot",
            "T:TeaSharp.Controls.LineSeries",
            "T:TeaSharp.Controls.LineSeriesScaleMode",
            "T:TeaSharp.Controls.Sparkline",
            "T:TeaSharp.Controls.ScatterPlot",
            "M:TeaSharp.Controls.LinePlot.ConfigureAxes(System.Boolean,System.String,System.String,System.String)",
            "M:TeaSharp.Controls.LinePlot.ConfigureGrid(System.Boolean)",
            "M:TeaSharp.Controls.LinePlot.ConfigureLegend(System.Boolean)",
            "M:TeaSharp.Controls.LineSeries.TrimToLast(System.Int32)",
            "M:TeaSharp.Controls.ScatterPlot.TrimToLast(System.Int32)",
            "M:TeaSharp.Controls.Sparkline.TrimToLast(System.Int32)",
            "P:TeaSharp.Controls.LineSeries.Capacity",
            "P:TeaSharp.Controls.ScatterPlot.Capacity",
            "P:TeaSharp.Controls.LineSeries.ScaleMode",
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
            "T:TeaSharp.Controls.DashboardGrid",
            "T:TeaSharp.Controls.DashboardTile",
            "M:TeaSharp.Controls.DashboardGrid.SetTiles(System.Collections.Generic.IEnumerable{TeaSharp.Controls.DashboardTile})",
            "M:TeaSharp.Controls.DashboardGrid.SetSelectedIndex(System.Int32)",
            "T:TeaSharp.Controls.BulletChart",
            "T:TeaSharp.Controls.BulletRange",
            "T:TeaSharp.Controls.BulletRangeKind",
            "M:TeaSharp.Controls.BulletChart.SetRanges(System.Collections.Generic.IEnumerable{TeaSharp.Controls.BulletRange})",
            "M:TeaSharp.Controls.BulletChart.SetValue(System.Double)",
            "M:TeaSharp.Controls.BulletChart.SetTarget(System.Double)",
            "T:TeaSharp.Controls.HealthBoard",
            "T:TeaSharp.Controls.HealthService",
            "T:TeaSharp.Controls.HealthServiceSeverity",
            "M:TeaSharp.Controls.HealthBoard.SetServices(System.Collections.Generic.IEnumerable{TeaSharp.Controls.HealthService})",
            "M:TeaSharp.Controls.HealthBoard.SetSelectedIndex(System.Int32)",
            "T:TeaSharp.Controls.SideNavRail",
            "T:TeaSharp.Controls.NavItem",
            "T:TeaSharp.Controls.SideNavRailGlyphSet",
            "T:TeaSharp.Controls.SideNavRailSelectionChangedEventArgs",
            "T:TeaSharp.Controls.SideNavRailActivatedEventArgs",
            "M:TeaSharp.Controls.SideNavRail.SetItems(System.Collections.Generic.IEnumerable{TeaSharp.Controls.NavItem})",
            "M:TeaSharp.Controls.SideNavRail.SetSelectedIndex(System.Int32)",
            "T:TeaSharp.Controls.ResizablePaneGroup",
            "T:TeaSharp.Controls.PaneSpec",
            "M:TeaSharp.Controls.ResizablePaneGroup.SetPanes(System.Collections.Generic.IEnumerable{TeaSharp.Controls.PaneSpec})",
            "M:TeaSharp.Controls.ResizablePaneGroup.SetSelectedPaneIndex(System.Int32)",
            "T:TeaSharp.Controls.JumpList",
            "T:TeaSharp.Controls.JumpListItem",
            "T:TeaSharp.Controls.JumpListGlyphSet",
            "T:TeaSharp.Controls.JumpListActivatedEventArgs",
            "M:TeaSharp.Controls.JumpList.SetItems(System.Collections.Generic.IEnumerable{TeaSharp.Controls.JumpListItem})",
            "M:TeaSharp.Controls.JumpList.SetSelectedIndex(System.Int32)",
            "T:TeaSharp.Controls.AutocompleteInput",
            "T:TeaSharp.Controls.AutocompleteInputGlyphSet",
            "T:TeaSharp.Controls.AutocompleteInputSuggestionCommittedEventArgs",
            "M:TeaSharp.Controls.AutocompleteInput.SetSuggestions(System.Collections.Generic.IEnumerable{System.String})",
            "M:TeaSharp.Controls.AutocompleteInput.SetSelectedSuggestionIndex(System.Int32)",
            "T:TeaSharp.Controls.QuickOpenOverlay",
            "T:TeaSharp.Controls.QuickOpenItem",
            "T:TeaSharp.Controls.QuickOpenOverlayGlyphSet",
            "T:TeaSharp.Controls.QuickOpenOverlaySubmittedEventArgs",
            "M:TeaSharp.Controls.QuickOpenOverlay.SetItems(System.Collections.Generic.IEnumerable{TeaSharp.Controls.QuickOpenItem})",
            "M:TeaSharp.Controls.QuickOpenOverlay.SetSelectedIndex(System.Int32)",
            "M:TeaSharp.Controls.QuickOpenOverlay.Open",
            "M:TeaSharp.Controls.QuickOpenOverlay.Close",
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
            "P:TeaSharp.Controls.StatsCard.Border",
            "P:TeaSharp.Controls.StatsCard.Padding",
            "P:TeaSharp.Controls.StatsCard.BorderStyleText",
            "P:TeaSharp.Controls.StatsCard.FocusedBorderStyleText",
            "P:TeaSharp.Controls.Table.SelectedRowIndex",
            "P:TeaSharp.Controls.Table.SelectedRow",
            "E:TeaSharp.Controls.Table.SelectionChanged",
            "M:TeaSharp.Controls.Table.TryGetSelectedRow(System.Collections.Generic.IReadOnlyList{System.String}@)",
            "M:TeaSharp.Controls.ListView`1.SetSelectedIndex(System.Int32)",
            "T:TeaSharp.Styles.ThemeScope",
            "M:TeaSharp.Styles.ThemeScope.Apply(TeaSharp.Styles.TeaTheme,TeaSharp.Controls.Control[])",
            "M:TeaSharp.Styles.ThemeScope.Apply(TeaSharp.Styles.TeaTheme,System.Collections.Generic.IEnumerable{TeaSharp.Controls.Control})",
            "P:TeaSharp.Controls.LinePlot.Series",
            "M:TeaSharp.Controls.LinePlot.SetSeries(System.Collections.Generic.IEnumerable{TeaSharp.Controls.LineSeries})",
            "M:TeaSharp.Controls.LinePlot.AddSeries(TeaSharp.Controls.LineSeries)",
            "M:TeaSharp.Controls.LinePlot.AppendSample(System.String,System.Double)",
            "M:TeaSharp.Controls.LineSeries.SetSamples(System.Collections.Generic.IEnumerable{System.Double})",
            "M:TeaSharp.Controls.LineSeries.Append(System.Double)",
            "M:TeaSharp.Controls.ScatterPlot.SetPoints(System.Collections.Generic.IEnumerable{TeaSharp.Controls.ScatterPlotPoint})",
            "M:TeaSharp.Controls.ScatterPlot.Append(TeaSharp.Controls.ScatterPlotPoint)",
            "M:TeaSharp.Controls.Sparkline.SetSamples(System.Collections.Generic.IEnumerable{System.Double})",
            "M:TeaSharp.Controls.Sparkline.Append(System.Double)",
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
