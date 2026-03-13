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
using System.Runtime.CompilerServices;
using TeaSharp.Core.Abstractions;
using TeaSharp.Layout;

namespace TeaSharp.Tests;

internal static class CompositionApiContractTests
{
    private static readonly Type[] RootLayoutTypes =
    [
        typeof(WindowLayout),
        typeof(RowLayout),
        typeof(ColumnLayout),
        typeof(PanelLayout),
        typeof(CenterLayout),
        typeof(LayoutSlot),
    ];

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
        "TeaSharp.Components.Prebuilt.LayoutFlow",
        "TeaSharp.Components.Prebuilt.LayoutContainerOptions",
        "TeaSharp.Components.Prebuilt.LayoutContainerComponent",
        "TeaSharp.Components.UiKit.Layout",
        "TeaSharp.Components.UiKit.SelectComponent",
        "TeaSharp.Components.UiKit.SortableTableComponent",
        "TeaSharp.Components.UiKit.TimelineEntry",
        "TeaSharp.Components.UiKit.ToastCenterComponent",
        "TeaSharp.Components.UiKit.ToastMessage",
        "TeaSharp.Components.UiKit.ToastSeverity",
        "TeaSharp.Components.UiKit.TreeNode",
        "TeaSharp.Components.UiKit.UiTheme",
        "TeaSharp.Components.UiKit.UiWidgets",
        "TeaSharp.Components.UiKit.ViewportClass",
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CompositionApi_RootLayoutTypes_RemainDiscoverable", RootLayoutTypes_RemainDiscoverable);
        yield return new TestCase("CompositionApi_CenterAndPanelLayouts_SupportObjectInitializerAssembly", CenterAndPanelLayouts_SupportObjectInitializerAssembly);
        yield return new TestCase("CompositionApi_InternalizedCompositionTypes_AreNotPublic", InternalizedCompositionTypes_AreNotPublic);
    }

    private static Task RootLayoutTypes_RemainDiscoverable()
    {
        foreach (var type in RootLayoutTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                type,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is null, $"{type.Name} should remain on the default discoverable layout path.");
        }

        return Task.CompletedTask;
    }

    private static Task CenterAndPanelLayouts_SupportObjectInitializerAssembly()
    {
        AssertObjectInitializerShape(typeof(CenterLayout));
        AssertObjectInitializerShape(typeof(PanelLayout));
        return Task.CompletedTask;
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

    private static void AssertObjectInitializerShape(Type type)
    {
        var constructor = type.GetConstructor(Type.EmptyTypes);
        TestAssert.True(constructor is not null, $"{type.Name} should expose a parameterless constructor for object-initializer assembly.");

        var property = type.GetProperty("Content", BindingFlags.Public | BindingFlags.Instance);
        TestAssert.True(property is not null, $"{type.Name} should expose a public Content property.");
        TestAssert.True(property!.SetMethod is not null, $"{type.Name}.Content should be settable during object initialization.");

        var requiredAttribute = (RequiredMemberAttribute?)Attribute.GetCustomAttribute(property, typeof(RequiredMemberAttribute));
        TestAssert.True(requiredAttribute is not null, $"{type.Name}.Content should stay required for valid object-initializer assembly.");
    }
}
