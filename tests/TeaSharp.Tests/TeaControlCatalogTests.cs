using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

internal static class TeaControlCatalogTests
{
    private static readonly Type[] NewControlTypes =
    [
        typeof(Label),
        typeof(Badge),
        typeof(BadgeTone),
        typeof(Button),
        typeof(Breadcrumb),
        typeof(BreadcrumbItem),
        typeof(CommandBar),
        typeof(CommandBarItem),
        typeof(CommandBarItemActivatedEventArgs),
        typeof(Accordion),
        typeof(TeaSharp.Controls.AccordionSection),
        typeof(TextInput),
        typeof(TextArea),
        typeof(Choice),
        typeof(ComboBox),
        typeof(CommandPalette),
        typeof(TeaSharp.Controls.CommandPaletteItem),
        typeof(Dialog),
        typeof(DiffView),
        typeof(DiffLineEntry),
        typeof(DiffLineKind),
        typeof(DiffViewMode),
        typeof(ContextMenu),
        typeof(TeaSharp.Controls.ContextMenuItem),
        typeof(LogView),
        typeof(Modal),
        typeof(NotificationLevel),
        typeof(Notifications),
        typeof(PropertyGrid),
        typeof(PropertyGridProperty),
        typeof(PropertyGridSelectionChangedEventArgs),
        typeof(ProgressBar),
        typeof(Paginator),
        typeof(SearchBox),
        typeof(SearchBoxQueryChangedEventArgs),
        typeof(SearchBoxNavigationRequestedEventArgs),
        typeof(SearchNavigationDirection),
        typeof(Slider),
        typeof(Spinner),
        typeof(StatusBar),
        typeof(Tabs),
        typeof(Toolbar),
        typeof(ToolbarItem),
        typeof(ToolbarSelectionChangedEventArgs),
        typeof(ListView<string>),
        typeof(Table),
        typeof(Toggle),
        typeof(TreeItem),
        typeof(TreeView),
        typeof(MenuBar),
        typeof(MenuItem),
        typeof(NumberInput),
        typeof(DatePicker),
        typeof(TimePicker),
        typeof(MarkdownView),
        typeof(MultiSelect),
        typeof(RadioGroup),
        typeof(TimeField),
        typeof(BarPoint),
        typeof(BarChart),
        typeof(LineChart),
        typeof(Gauge),
        typeof(MiniLog),
        typeof(StatItem),
        typeof(StatsCard),
    ];

    private static readonly string[] InternalizedLegacyPrebuiltTypes = [];

    private static readonly string[] RemovedLegacyPrebuiltTypes =
    [
        "TeaSharp.Components.Prebuilt.TextBlockComponent",
        "TeaSharp.Components.Prebuilt.TextBlockOptions",
        "TeaSharp.Components.Prebuilt.ButtonComponent",
        "TeaSharp.Components.Prebuilt.ButtonOptions",
        "TeaSharp.Components.Prebuilt.TextInputComponent",
        "TeaSharp.Components.Prebuilt.TextInputOptions",
        "TeaSharp.Components.Prebuilt.TextInputSubmittedEventArgs",
        "TeaSharp.Components.Prebuilt.TextInputCancelledEventArgs",
        "TeaSharp.Components.Prebuilt.TextAreaComponent",
        "TeaSharp.Components.Prebuilt.TextAreaOptions",
        "TeaSharp.Components.Prebuilt.StatusBarComponent",
        "TeaSharp.Components.Prebuilt.StatusBarOptions",
        "TeaSharp.Components.Prebuilt.DropdownComponent",
        "TeaSharp.Components.Prebuilt.DropdownOptions",
        "TeaSharp.Components.Prebuilt.ComboboxComponent",
        "TeaSharp.Components.Prebuilt.ComboboxOptions",
        "TeaSharp.Components.Prebuilt.OptionSelectionChangedEventArgs",
        "TeaSharp.Components.Prebuilt.ListComponent`1",
        "TeaSharp.Components.Prebuilt.ListOptions`1",
        "TeaSharp.Components.Prebuilt.ListSelectionChangedEventArgs`1",
        "TeaSharp.Components.Prebuilt.DialogComponent",
        "TeaSharp.Components.Prebuilt.DialogOptions",
        "TeaSharp.Components.Prebuilt.DialogResult",
        "TeaSharp.Components.Prebuilt.ProgressBarComponent",
        "TeaSharp.Components.Prebuilt.ProgressBarOptions",
        "TeaSharp.Components.Prebuilt.LogViewerComponent",
        "TeaSharp.Components.Prebuilt.LogViewerOptions",
        "TeaSharp.Components.Prebuilt.TableComponent",
        "TeaSharp.Components.Prebuilt.TableOptions",
    ];

    private static readonly string[] InternalizedLegacyPromotedTypes = [];

    private static readonly string[] RemovedLegacyPromotedTypes =
    [
        "TeaSharp.Components.Productivity.MenuBarComponent",
        "TeaSharp.Components.Productivity.MenuBarItem",
        "TeaSharp.Components.Productivity.MenuBarItemActivatedEventArgs",
        "TeaSharp.Components.Productivity.MenuBarOptions",
        "TeaSharp.Components.Productivity.ContextMenuComponent",
        "TeaSharp.Components.Productivity.ContextMenuItem",
        "TeaSharp.Components.Productivity.ContextMenuItemExecutedEventArgs",
        "TeaSharp.Components.Productivity.ContextMenuOptions",
        "TeaSharp.Components.Advanced.CommandPaletteComponent",
        "TeaSharp.Components.Advanced.CommandPaletteItem",
        "TeaSharp.Components.Advanced.CommandPaletteItemExecutedEventArgs",
        "TeaSharp.Components.Advanced.BadgeComponent",
        "TeaSharp.Components.Advanced.NotificationCenterComponent",
        "TeaSharp.Components.Advanced.NotificationEntry",
        "TeaSharp.Components.Advanced.NotificationSeverity",
        "TeaSharp.Components.Advanced.SliderComponent",
        "TeaSharp.Components.Advanced.SpinnerComponent",
        "TeaSharp.Components.Advanced.TreeItemNode",
        "TeaSharp.Components.Advanced.TreeViewComponent",
        "TeaSharp.Components.Advanced.ToggleSwitchComponent",
        "TeaSharp.Components.Productivity.DateChangedEventArgs",
        "TeaSharp.Components.Productivity.DatePickerComponent",
        "TeaSharp.Components.Productivity.DatePickerOptions",
        "TeaSharp.Components.Productivity.MarkdownViewerComponent",
        "TeaSharp.Components.Productivity.MarkdownViewerOptions",
        "TeaSharp.Components.Productivity.NumberInputComponent",
        "TeaSharp.Components.Productivity.NumberInputOptions",
        "TeaSharp.Components.Productivity.NumberInputSubmittedEventArgs",
        "TeaSharp.Components.Productivity.TimePickerComponent",
        "TeaSharp.Components.Productivity.TimePickerField",
        "TeaSharp.Components.Productivity.TimePickerOptions",
        "TeaSharp.Components.Productivity.TimeValueChangedEventArgs",
        "TeaSharp.Components.UiKit.AccordionComponent",
        "TeaSharp.Components.UiKit.AccordionSection",
        "TeaSharp.Components.UiKit.CheckboxListComponent",
        "TeaSharp.Components.UiKit.ModalComponent",
        "TeaSharp.Components.UiKit.ModalOptions",
        "TeaSharp.Components.UiKit.RadioGroupComponent",
        "TeaSharp.Components.UiKit.SortableTableComponent",
        "TeaSharp.Components.UiKit.TabSelectionChangedEventArgs",
        "TeaSharp.Components.UiKit.TabsComponent",
        "TeaSharp.Components.UiKit.TabsOptions",
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TeaControlCatalog_NewControlTypes_RemainDiscoverable",
            NewControlTypes_RemainDiscoverable);
        yield return new TestCase(
            "TeaControlCatalog_RootPollingMethods_AreMarkedAdvanced",
            RootPollingMethods_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaControlCatalog_InternalizedLegacyPrebuiltTypes_AreNotPublic",
            InternalizedLegacyPrebuiltTypes_AreNotPublic);
        yield return new TestCase(
            "TeaControlCatalog_RemovedLegacyPrebuiltTypes_AreAbsent",
            RemovedLegacyPrebuiltTypes_AreAbsent);
        yield return new TestCase(
            "TeaControlCatalog_InternalizedLegacyPromotedTypes_AreNotPublic",
            InternalizedLegacyPromotedTypes_AreNotPublic);
        yield return new TestCase(
            "TeaControlCatalog_RemovedLegacyPromotedTypes_AreAbsent",
            RemovedLegacyPromotedTypes_AreAbsent);
        yield return new TestCase(
            "TeaControlCatalog_PromotedFormControls_WorkThroughRootWrappers",
            PromotedFormControls_WorkThroughRootWrappers);
        yield return new TestCase(
            "TeaControlCatalog_TimePicker_UsesRootTimeFieldType",
            TimePicker_UsesRootTimeFieldType);
        yield return new TestCase(
            "TeaControlCatalog_PromotedAdvancedControls_WorkThroughRootWrappers",
            PromotedAdvancedControls_WorkThroughRootWrappers);
        yield return new TestCase(
            "TeaControlCatalog_PromotedDashboardAndChartControls_WorkThroughRootWrappers",
            PromotedDashboardAndChartControls_WorkThroughRootWrappers);
    }

    private static Task NewControlTypes_RemainDiscoverable()
    {
        foreach (var type in NewControlTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                type,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is null, $"{type.Name} should remain on the default discoverable control path.");
        }

        return Task.CompletedTask;
    }

    private static Task RootPollingMethods_AreMarkedAdvanced()
    {
        var methods =
            new (Type Type, string Name, Type[] Parameters)[]
            {
                (typeof(Button), nameof(Button.TryConsumeActivation), Type.EmptyTypes),
                (typeof(TextInput), nameof(TextInput.TryConsumeSubmission), [typeof(string).MakeByRefType()]),
                (typeof(TextInput), nameof(TextInput.TryConsumeCancellation), [typeof(string).MakeByRefType()]),
                (typeof(Dialog), nameof(Dialog.TryConsumeResult), [typeof(TeaSharp.Controls.DialogResult).MakeByRefType()]),
                (typeof(MenuBar), nameof(MenuBar.TryConsumeActivation), [typeof(string).MakeByRefType()]),
            };

        foreach (var (type, name, parameters) in methods)
        {
            var method = type.GetMethod(name, parameters);
            TestAssert.True(method is not null, $"{type.Name}.{name} should exist for advanced callers.");
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(method!, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null, $"{type.Name}.{name} should be marked advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced, $"{type.Name}.{name} should be hidden from the default path.");
        }

        return Task.CompletedTask;
    }

    private static Task InternalizedLegacyPrebuiltTypes_AreNotPublic()
    {
        var assembly = typeof(Label).Assembly;

        foreach (var typeName in InternalizedLegacyPrebuiltTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public once a root wrapper exists.");
        }

        return Task.CompletedTask;
    }

    private static Task RemovedLegacyPrebuiltTypes_AreAbsent()
    {
        var assembly = typeof(Label).Assembly;

        foreach (var typeName in RemovedLegacyPrebuiltTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is null, $"{typeName} should be removed once the root control owns the implementation directly.");
        }

        return Task.CompletedTask;
    }

    private static Task InternalizedLegacyPromotedTypes_AreNotPublic()
    {
        var assembly = typeof(Label).Assembly;

        foreach (var typeName in InternalizedLegacyPromotedTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public once a root wrapper exists.");
        }

        return Task.CompletedTask;
    }

    private static Task RemovedLegacyPromotedTypes_AreAbsent()
    {
        var assembly = typeof(Label).Assembly;

        foreach (var typeName in RemovedLegacyPromotedTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is null, $"{typeName} should be removed once the root wrapper owns the implementation directly.");
        }

        return Task.CompletedTask;
    }

    private static Task PromotedFormControls_WorkThroughRootWrappers()
    {
        var number = new NumberInput
        {
            IsFocused = true,
        };
        double? submittedNumber = null;
        number.Submitted += (_, args) => submittedNumber = args.Value;
        number.Handle(new KeyPressed(Key.Character, "4"));
        number.Handle(new KeyPressed(Key.Enter));

        var date = new DatePicker
        {
            IsFocused = true,
        };
        var initialDate = date.SelectedDate;
        date.Handle(new KeyPressed(Key.Right));
        date.Handle(new KeyPressed(Key.Enter));

        var time = new TimePicker
        {
            IsFocused = true,
        };
        time.SetValue(new TimeOnly(10, 0, 0));
        time.Handle(new KeyPressed(Key.Up));
        time.Handle(new KeyPressed(Key.Enter));

        var multi = new MultiSelect
        {
            IsFocused = true,
        };
        multi.SetItems(["Docs", "Tests"]);
        multi.Handle(new KeyPressed(Key.Enter));
        multi.Handle(new KeyPressed(Key.Down));
        multi.Handle(new KeyPressed(Key.Enter));

        var radio = new RadioGroup
        {
            IsFocused = true,
        };
        radio.SetItems(["Low", "High"]);
        radio.Handle(new KeyPressed(Key.Right));

        var markdown = new MarkdownView
        {
            IsFocused = true,
        };
        markdown.SetMarkdown("# Help\nUse arrows.");

        TestAssert.Equal(4d, submittedNumber ?? -1d, "NumberInput should submit values through the root wrapper.");
        TestAssert.Equal(initialDate.AddDays(1), date.SelectedDate, "DatePicker should move the selected date through the root wrapper.");
        TestAssert.True(date.LastCommittedDate == date.SelectedDate, "DatePicker should expose the committed date through the root wrapper.");
        TestAssert.True(time.Value.Hour == 11, "TimePicker should adjust the active field through the root wrapper.");
        TestAssert.True(time.LastCommittedTime == time.Value, "TimePicker should expose the committed time through the root wrapper.");
        TestAssert.Equal(2, multi.CheckedItems.Count, "MultiSelect should toggle checked items through the root wrapper.");
        TestAssert.Equal("High", radio.SelectedItem, "RadioGroup should move selection through the root wrapper.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_UsesRootTimeFieldType()
    {
        var property = typeof(TimePicker).GetProperty(nameof(TimePicker.ActiveField));

        TestAssert.True(property is not null, "TimePicker.ActiveField should exist.");
        TestAssert.True(property!.PropertyType == typeof(TimeField), "TimePicker.ActiveField should stay on the root control contract.");
        return Task.CompletedTask;
    }

    private static Task PromotedAdvancedControls_WorkThroughRootWrappers()
    {
        var badge = new Badge
        {
            Text = "hot",
            Tone = BadgeTone.Warning,
        };
        var badgeCanvas = new Canvas(16, 1);
        badge.Render(badgeCanvas, new Rect(0, 0, 16, 1));

        var accordion = new Accordion
        {
            Title = "Sections",
            IsFocused = true,
        };
        accordion.SetSections(
        [
            new TeaSharp.Controls.AccordionSection("Overview", ["alpha"]),
            new TeaSharp.Controls.AccordionSection("Deploy", ["ship it"]),
        ]);
        accordion.MoveNext();
        accordion.ToggleSelected();
        var accordionCanvas = new Canvas(24, 6);
        accordion.Render(accordionCanvas, new Rect(0, 0, 24, 6));

        var modal = new Modal
        {
            Title = "Dialog",
            IsVisible = true,
            BackdropFill = ':',
        };
        modal.SetBodyLines(["deploy ready"]);
        var modalCanvas = new Canvas(30, 10);
        modal.Render(modalCanvas, new Rect(0, 0, 30, 10));

        var menu = new ContextMenu
        {
            IsFocused = true,
        };
        string? contextItemId = null;
        menu.ItemExecuted += (_, args) => contextItemId = args.ItemId;
        menu.SetItems(
        [
            new TeaSharp.Controls.ContextMenuItem("copy", "Copy"),
            new TeaSharp.Controls.ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(2, 2);
        menu.Handle(new KeyPressed(Key.Enter));

        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        string? commandItemId = null;
        palette.ItemExecuted += (_, args) => commandItemId = args.ItemId;
        palette.SetItems(
        [
            new TeaSharp.Controls.CommandPaletteItem("deploy", "Deploy", "publish release"),
            new TeaSharp.Controls.CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);
        palette.Open();
        palette.QueryText = "roll";
        palette.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(badgeCanvas.Render().Contains("[hot]", StringComparison.Ordinal), "Badge should render through the root wrapper.");
        TestAssert.Equal(1, accordion.SelectedIndex, "Accordion should move selection through the root wrapper.");
        TestAssert.True(accordionCanvas.Render().Contains("ship it", StringComparison.Ordinal), "Accordion should expand sections through the root wrapper.");
        TestAssert.True(modalCanvas.Render().Contains("deploy ready", StringComparison.Ordinal), "Modal should render body lines through the root wrapper.");
        TestAssert.True(modalCanvas.Render().Contains("::", StringComparison.Ordinal), "Modal should expose backdrop fill through the root wrapper.");
        TestAssert.True(contextItemId == "copy", "ContextMenu should execute items through the root wrapper.");
        TestAssert.True(!menu.IsVisible, "ContextMenu should close after executing through the root wrapper.");
        TestAssert.True(commandItemId == "rollback", "CommandPalette should filter and execute through the root wrapper.");
        TestAssert.True(!palette.IsVisible, "CommandPalette should close after executing through the root wrapper.");
        return Task.CompletedTask;
    }

    private static Task PromotedDashboardAndChartControls_WorkThroughRootWrappers()
    {
        var bars = new BarChart
        {
            Title = "Bars",
        };
        bars.SetBars([new BarPoint("cpu", 42), new BarPoint("mem", 73)]);
        bars.SetValue("cpu", 55);
        var barCanvas = new Canvas(24, 8);
        bars.Render(barCanvas, new Rect(0, 0, 24, 8));

        var line = new LineChart(capacity: 4)
        {
            Title = "Line",
        };
        line.SetSamples([1, 2, 3, 4, 5]);
        line.ZoomIn();
        line.Pan(1);
        var lineCanvas = new Canvas(24, 8);
        line.Render(lineCanvas, new Rect(0, 0, 24, 8));

        var gauge = new Gauge
        {
            Title = "Gauge",
            Value = 72,
            MaxValue = 100,
            Label = "72%",
        };
        var gaugeCanvas = new Canvas(24, 5);
        gauge.Render(gaugeCanvas, new Rect(0, 0, 24, 5));

        var log = new MiniLog(capacity: 3)
        {
            Title = "Log",
        };
        log.Append("one");
        log.Append("two");
        log.Append("three");
        log.Append("four");
        var logCanvas = new Canvas(24, 6);
        log.Render(logCanvas, new Rect(0, 0, 24, 6));

        var stats = new StatsCard
        {
            Title = "Stats",
        };
        stats.SetItems([new StatItem("raw", "yes"), new StatItem("mouse", "yes")]);
        stats.SetValue("paste", "no");
        var statsCanvas = new Canvas(24, 6);
        stats.Render(statsCanvas, new Rect(0, 0, 24, 6));

        TestAssert.Equal(55d, bars.Bars[0].Value, "BarChart should update named bar values through the root wrapper.");
        TestAssert.True(barCanvas.Render().Contains("Bars", StringComparison.Ordinal), "BarChart should render through the root wrapper.");
        TestAssert.Equal(4, line.Samples.Count, "LineChart should honor capacity through the root wrapper.");
        TestAssert.True(line.Zoom > 1.0, "LineChart should zoom through the root wrapper.");
        TestAssert.True(line.Offset == 1, "LineChart should pan through the root wrapper.");
        TestAssert.True(lineCanvas.Render().Contains("Line", StringComparison.Ordinal), "LineChart should render through the root wrapper.");
        TestAssert.True(gaugeCanvas.Render().Contains("72%", StringComparison.Ordinal), "Gauge should render labels through the root wrapper.");
        TestAssert.Equal(3, log.Entries.Count, "MiniLog should honor capacity through the root wrapper.");
        TestAssert.True(logCanvas.Render().Contains("four", StringComparison.Ordinal), "MiniLog should render appended entries through the root wrapper.");
        TestAssert.Equal(3, stats.Items.Count, "StatsCard should store values through the root wrapper.");
        TestAssert.True(statsCanvas.Render().Contains("paste", StringComparison.Ordinal), "StatsCard should render values through the root wrapper.");
        return Task.CompletedTask;
    }
}
