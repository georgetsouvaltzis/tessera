using System.ComponentModel;
using TeaSharp.Components;
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
    ];

    public static IEnumerable<TestCase> Cases()
    {
        foreach (var (name, type) in AdvancedWidgetTypes)
        {
            yield return new TestCase(
                $"WidgetApi_{name}_IsMarkedAdvanced",
                () => AssertMarkedAdvanced(type));
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

    private static Task KeyBinding_RemainsDiscoverable()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(KeyBinding),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is null, "KeyBinding should remain discoverable until higher-level key customization options exist.");
        return Task.CompletedTask;
    }
}
