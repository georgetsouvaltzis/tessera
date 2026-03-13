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
using System.Reflection;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Tests;

internal static class CompositionApiContractTests
{
    private static readonly string[] InternalizedCompositionTypes =
    [
        "TeaSharp.Components.Composition.ScreenComposer",
        "TeaSharp.Components.Composition.ComponentComposer",
        "TeaSharp.Components.Composition.InputRouter",
        "TeaSharp.Components.Composition.InteractiveScreenModel",
        "TeaSharp.Components.Composition.DialogWorkflow",
        "TeaSharp.Components.Composition.InputScope",
        "TeaSharp.Components.Composition.InputScopeBehavior",
        "TeaSharp.Components.Composition.InputScopeKind",
        "TeaSharp.Components.Composition.InputRouteResult",
        "TeaSharp.Components.Composition.ScreenFocusChain",
        "TeaSharp.Components.Composition.ScreenFocusSnapshot",
        "TeaSharp.Components.Composition.ScreenFrameLayout",
        "TeaSharp.Components.Composition.ScreenLayer",
        "TeaSharp.Components.Composition.KeyboardRoutingMode",
        "TeaSharp.Components.Composition.MasterDetailScreen",
        "TeaSharp.Components.Composition.DashboardScreen",
        "TeaSharp.Components.Composition.FormScreen",
        "TeaSharp.Components.Composition.ComponentSlot",
        "TeaSharp.Components.Composition.ScreenRegion",
        "TeaSharp.Components.Composition.ScreenRegionKey",
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CompositionApi_InternalizedCompositionTypes_AreNotPublic", InternalizedCompositionTypes_AreNotPublic);
        yield return new TestCase("CompositionApi_RemainingInteropTypes_AreMarkedAdvanced", RemainingInteropTypes_AreMarkedAdvanced);
    }

    private static Task InternalizedCompositionTypes_AreNotPublic()
    {
        var assembly = typeof(Screen).Assembly;

        foreach (var typeName in InternalizedCompositionTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public.");
        }

        return Task.CompletedTask;
    }

    private static Task RemainingInteropTypes_AreMarkedAdvanced()
    {
        Type[] advancedTypes =
        [
            typeof(TeaSharp.Components.UiKit.Layout),
        ];

        foreach (var type in advancedTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(type, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null, $"{type.Name} should remain explicitly marked as advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced, $"{type.Name} should stay hidden from default discovery.");
        }

        return Task.CompletedTask;
    }
}
