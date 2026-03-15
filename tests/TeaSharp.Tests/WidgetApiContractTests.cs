using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;

namespace TeaSharp.Tests;

internal static class WidgetApiContractTests
{
    private static readonly string[] InternalizedWidgetTypes =
    [
        "TeaSharp.Widgets.IWidgetKeyMap",
        "TeaSharp.Widgets.HelpView",
        "TeaSharp.Widgets.KeyBinding",
        "TeaSharp.Widgets.TextInputKeyMap",
        "TeaSharp.Widgets.ListKeyMap",
        "TeaSharp.Widgets.ViewportKeyMap",
        "TeaSharp.Widgets.TextInputFrame",
        "TeaSharp.Widgets.TextInputModel",
        "TeaSharp.Widgets.TextInputUpdateResult",
        "TeaSharp.Widgets.ListRow`1",
        "TeaSharp.Widgets.ListModel`1",
        "TeaSharp.Widgets.ViewportModel",
        "TeaSharp.Components.Interaction.WidgetInteractionProfile",
        "TeaSharp.Components.Styling.WidgetVisualState",
        "TeaSharp.Components.Styling.WidgetStatePalette",
        "TeaSharp.Components.Styling.WidgetStateAppearance",
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("WidgetApi_InternalizedWidgetTypes_AreNotPublic", InternalizedWidgetTypes_AreNotPublic);
    }

    private static Task InternalizedWidgetTypes_AreNotPublic()
    {
        var assembly = typeof(Screen).Assembly;

        foreach (var typeName in InternalizedWidgetTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public.");
        }

        return Task.CompletedTask;
    }
}
