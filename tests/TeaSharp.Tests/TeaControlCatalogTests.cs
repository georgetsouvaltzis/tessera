using System.ComponentModel;
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
        typeof(Dialog),
        typeof(StatusBar),
        typeof(Tabs),
        typeof(ListView<string>),
        typeof(Table),
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
        typeof(DialogComponent),
        typeof(DialogOptions),
        typeof(StatusBarComponent),
        typeof(StatusBarOptions),
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
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TeaControlCatalog_NewControlTypes_RemainDiscoverable",
            NewControlTypes_RemainDiscoverable);
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
