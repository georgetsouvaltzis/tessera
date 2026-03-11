using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Tests;

internal static class WidgetApiContractTests
{
    private static readonly (string Name, Type Type)[] AdvancedWidgetTypes =
    [
        ("IWidgetKeyMap", typeof(IWidgetKeyMap)),
        ("TextInputKeyMap", typeof(TextInputKeyMap)),
        ("ListKeyMap", typeof(ListKeyMap)),
        ("ViewportKeyMap", typeof(ViewportKeyMap)),
        ("TextInputModel", typeof(TextInputModel)),
        ("ListModel", typeof(ListModel<string>)),
        ("ViewportModel", typeof(ViewportModel)),
        ("WidgetInteractionProfile", typeof(WidgetInteractionProfile)),
        ("WidgetStatePalette", typeof(WidgetStatePalette)),
        ("WidgetStateAppearance", typeof(WidgetStateAppearance)),
    ];

    private static readonly (string Name, Type Type, string PropertyName)[] AdvancedWidgetProperties =
    [
        ("TextInputComponent.KeyMap", typeof(TextInputComponent), nameof(TextInputComponent.KeyMap)),
        ("TextAreaComponent.InputKeyMap", typeof(TextAreaComponent), nameof(TextAreaComponent.InputKeyMap)),
        ("TextAreaComponent.ViewportKeyMap", typeof(TextAreaComponent), nameof(TextAreaComponent.ViewportKeyMap)),
        ("ListComponent.KeyMap", typeof(ListComponent<string>), nameof(ListComponent<string>.KeyMap)),
        ("DropdownComponent.InteractionProfile", typeof(DropdownComponent), nameof(DropdownComponent.InteractionProfile)),
        ("ComboboxComponent.InputKeyMap", typeof(ComboboxComponent), nameof(ComboboxComponent.InputKeyMap)),
        ("NumberInputComponent.InputKeyMap", typeof(NumberInputComponent), nameof(NumberInputComponent.InputKeyMap)),
        ("DatePickerComponent.InteractionProfile", typeof(DatePickerComponent), nameof(DatePickerComponent.InteractionProfile)),
        ("TimePickerComponent.InteractionProfile", typeof(TimePickerComponent), nameof(TimePickerComponent.InteractionProfile)),
        ("MarkdownViewerComponent.ViewportKeyMap", typeof(MarkdownViewerComponent), nameof(MarkdownViewerComponent.ViewportKeyMap)),
        ("LogViewerComponent.ViewportKeyMap", typeof(LogViewerComponent), nameof(LogViewerComponent.ViewportKeyMap)),
        ("MenuBarComponent.InteractionProfile", typeof(MenuBarComponent), nameof(MenuBarComponent.InteractionProfile)),
        ("ContextMenuComponent.InteractionProfile", typeof(ContextMenuComponent), nameof(ContextMenuComponent.InteractionProfile)),
    ];

    public static IEnumerable<TestCase> Cases()
    {
        foreach (var (name, type) in AdvancedWidgetTypes)
        {
            yield return new TestCase(
                $"WidgetApi_{name}_IsMarkedAdvanced",
                () => AssertMarkedAdvanced(type));
        }

        foreach (var (name, type, propertyName) in AdvancedWidgetProperties)
        {
            yield return new TestCase(
                $"WidgetApi_{name}_IsMarkedAdvanced",
                () => AssertPropertyMarkedAdvanced(type, propertyName));
        }

        yield return new TestCase(
            "WidgetApi_CommandPaletteQuery_IsMarkedAdvanced",
            CommandPaletteQuery_IsMarkedAdvanced);

        yield return new TestCase(
            "WidgetApi_KeyBinding_RemainsDiscoverable",
            KeyBinding_RemainsDiscoverable);
    }

    private static Task AssertMarkedAdvanced(Type type)
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            type,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, $"{type.Name} should be explicitly marked as advanced widget infrastructure.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            $"{type.Name} should be hidden from default API discovery.");
        return Task.CompletedTask;
    }

    private static Task CommandPaletteQuery_IsMarkedAdvanced()
    {
        var property = typeof(CommandPaletteComponent).GetProperty(nameof(CommandPaletteComponent.Query));
        TestAssert.True(property is not null, "CommandPaletteComponent.Query should exist.");

        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            property!,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, "CommandPaletteComponent.Query should be explicitly marked as advanced.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            "CommandPaletteComponent.Query should be hidden from default API discovery.");
        return Task.CompletedTask;
    }

    private static Task AssertPropertyMarkedAdvanced(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        TestAssert.True(property is not null, $"{type.Name}.{propertyName} should exist.");

        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            property!,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, $"{type.Name}.{propertyName} should be explicitly marked as advanced.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            $"{type.Name}.{propertyName} should be hidden from default API discovery.");
        return Task.CompletedTask;
    }

    private static Task KeyBinding_RemainsDiscoverable()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(KeyBinding),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is null, "KeyBinding should remain discoverable until higher-level key customization options exist.");
        return Task.CompletedTask;
    }
}
