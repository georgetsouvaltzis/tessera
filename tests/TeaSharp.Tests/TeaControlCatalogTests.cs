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
}
