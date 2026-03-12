using System.ComponentModel;
using TeaSharp.Components.Advanced;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.UiKit;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

internal static class TeaControlCatalogTests
{
    private static readonly Type[] NewControlTypes =
    [
        typeof(Label),
        typeof(Button),
        typeof(TextInput),
        typeof(TextArea),
        typeof(Choice),
        typeof(ComboBox),
        typeof(Dialog),
        typeof(LogView),
        typeof(NotificationLevel),
        typeof(Notifications),
        typeof(ProgressBar),
        typeof(Slider),
        typeof(Spinner),
        typeof(StatusBar),
        typeof(Tabs),
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
    ];

    private static readonly Type[] LegacyPromotedTypes =
    [
        typeof(TextBlockComponent),
        typeof(TextBlockOptions),
        typeof(ButtonComponent),
        typeof(ButtonOptions),
        typeof(TextInputComponent),
        typeof(TextInputOptions),
        typeof(global::TeaSharp.Components.Prebuilt.TextInputSubmittedEventArgs),
        typeof(global::TeaSharp.Components.Prebuilt.TextInputCancelledEventArgs),
        typeof(TextAreaComponent),
        typeof(TextAreaOptions),
        typeof(DropdownComponent),
        typeof(DropdownOptions),
        typeof(ComboboxComponent),
        typeof(ComboboxOptions),
        typeof(DialogComponent),
        typeof(DialogOptions),
        typeof(ProgressBarComponent),
        typeof(ProgressBarOptions),
        typeof(StatusBarComponent),
        typeof(StatusBarOptions),
        typeof(LogViewerComponent),
        typeof(LogViewerOptions),
        typeof(TabsComponent),
        typeof(TabsOptions),
        typeof(TabSelectionChangedEventArgs),
        typeof(ListComponent<string>),
        typeof(ListOptions<string>),
        typeof(global::TeaSharp.Components.Prebuilt.ListSelectionChangedEventArgs<string>),
        typeof(TableComponent),
        typeof(TableOptions),
        typeof(MenuBarComponent),
        typeof(MenuBarOptions),
        typeof(MenuBarItem),
        typeof(MenuBarItemActivatedEventArgs),
        typeof(ToggleSwitchComponent),
        typeof(SliderComponent),
        typeof(SpinnerComponent),
        typeof(TreeViewComponent),
        typeof(NotificationCenterComponent),
        typeof(NumberInputComponent),
        typeof(NumberInputOptions),
        typeof(global::TeaSharp.Components.Productivity.NumberInputSubmittedEventArgs),
        typeof(DatePickerComponent),
        typeof(DatePickerOptions),
        typeof(global::TeaSharp.Components.Productivity.DateChangedEventArgs),
        typeof(TimePickerComponent),
        typeof(TimePickerOptions),
        typeof(global::TeaSharp.Components.Productivity.TimeValueChangedEventArgs),
        typeof(global::TeaSharp.Components.Productivity.TimePickerField),
        typeof(MarkdownViewerComponent),
        typeof(MarkdownViewerOptions),
        typeof(CheckboxListComponent),
        typeof(RadioGroupComponent),
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
            "TeaControlCatalog_LegacyPromotedTypes_AreMarkedAdvanced",
            LegacyPromotedTypes_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaControlCatalog_PromotedFormControls_WorkThroughRootWrappers",
            PromotedFormControls_WorkThroughRootWrappers);
        yield return new TestCase(
            "TeaControlCatalog_TimePicker_UsesRootTimeFieldType",
            TimePicker_UsesRootTimeFieldType);
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

    private static Task LegacyPromotedTypes_AreMarkedAdvanced()
    {
        foreach (var type in LegacyPromotedTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                type,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is not null, $"{type.Name} should be explicitly marked as advanced.");
            TestAssert.True(
                attribute!.State == EditorBrowsableState.Advanced,
                $"{type.Name} should be hidden from default discovery now that a root-level control exists.");
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
}
