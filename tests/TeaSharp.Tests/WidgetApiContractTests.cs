using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Tests;

internal static class WidgetApiContractTests
{
    private static readonly (string Name, Type Type)[] AdvancedWidgetTypes =
    [
        ("IWidgetKeyMap", typeof(IWidgetKeyMap)),
        ("HelpView", typeof(HelpView)),
        ("KeyBinding", typeof(KeyBinding)),
        ("TextInputKeyMap", typeof(TextInputKeyMap)),
        ("ListKeyMap", typeof(ListKeyMap)),
        ("ViewportKeyMap", typeof(ViewportKeyMap)),
        ("TextInputFrame", typeof(TextInputFrame)),
        ("TextInputModel", typeof(TextInputModel)),
        ("TextInputUpdateResult", typeof(TextInputUpdateResult)),
        ("ListRow", typeof(ListRow<string>)),
        ("ListModel", typeof(ListModel<string>)),
        ("ViewportModel", typeof(ViewportModel)),
        ("WidgetInteractionProfile", typeof(WidgetInteractionProfile)),
        ("WidgetVisualState", typeof(WidgetVisualState)),
        ("WidgetStatePalette", typeof(WidgetStatePalette)),
        ("WidgetStateAppearance", typeof(WidgetStateAppearance)),
    ];

    public static IEnumerable<TestCase> Cases()
    {
        foreach (var (name, type) in AdvancedWidgetTypes)
        {
            yield return new TestCase(
                $"WidgetApi_{name}_IsMarkedAdvanced",
                () => AssertMarkedAdvanced(type));
        }
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
}
