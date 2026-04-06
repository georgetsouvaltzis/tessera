using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tessera.Controls;
using Tessera.Layout;

namespace Tessera.Tests;

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

    private static readonly string[] InternalizedCompositionTypes = [];

    private static readonly string[] RemovedCompositionTypes =
    [
        "Tessera.Components.Composition.ScreenComposer",
        "Tessera.Components.Composition.ScreenFocusChain",
        "Tessera.Components.Composition.ScreenFocusSnapshot",
        "Tessera.Components.Composition.ScreenFrameLayout",
        "Tessera.Components.Composition.ScreenRegion",
        "Tessera.Components.Composition.ScreenRegionKey",
        "Tessera.Components.Composition.ComponentSlot",
        "Tessera.Components.Composition.ScreenLayer",
        "Tessera.Components.Prebuilt.LayoutFlow",
        "Tessera.Components.Prebuilt.LayoutContainerOptions",
        "Tessera.Components.Prebuilt.LayoutContainerComponent",
        "Tessera.Components.Prebuilt.PrebuiltCatalog",
        "Tessera.Components.UiKit.Layout",
        "Tessera.Components.UiKit.SelectComponent",
        "Tessera.Components.UiKit.TimelineEntry",
        "Tessera.Components.UiKit.ToastCenterComponent",
        "Tessera.Components.UiKit.ToastMessage",
        "Tessera.Components.UiKit.ToastSeverity",
        "Tessera.Components.UiKit.TreeNode",
        "Tessera.Components.UiKit.ViewportClass",
        "Tessera.Components.UiKit.UiTheme",
        "Tessera.Components.UiKit.UiWidgets",
        "Tessera.Layout.Stack",
        "Tessera.Layout.Split",
        "Tessera.Layout.Panel",
        "Tessera.Layout.Dock",
        "Tessera.Layout.Overlay",
        "Tessera.Layout.Center",
        "Tessera.Layout.Slot",
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CompositionApi_RootLayoutTypes_RemainDiscoverable", RootLayoutTypes_RemainDiscoverable);
        yield return new TestCase("CompositionApi_KeyLayoutTypes_SupportObjectInitializerAssembly", KeyLayoutTypes_SupportObjectInitializerAssembly);
        yield return new TestCase("CompositionApi_LayoutLength_SupportsImplicitFixedIntegers", LayoutLength_SupportsImplicitFixedIntegers);
        yield return new TestCase("CompositionApi_RowAndColumnSizingHelpers_AreMarkedAdvanced", RowAndColumnSizingHelpers_AreMarkedAdvanced);
        yield return new TestCase("CompositionApi_ScreenFromLayoutNode_IsMarkedAdvanced", ScreenFromLayoutNode_IsMarkedAdvanced);
        yield return new TestCase("CompositionApi_InternalizedCompositionTypes_AreNotPublic", InternalizedCompositionTypes_AreNotPublic);
        yield return new TestCase("CompositionApi_RemovedCompositionTypes_AreAbsent", RemovedCompositionTypes_AreAbsent);
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

    private static Task KeyLayoutTypes_SupportObjectInitializerAssembly()
    {
        AssertObjectInitializerShape(typeof(CenterLayout));
        AssertObjectInitializerShape(typeof(PanelLayout));
        AssertObjectInitializerShape(typeof(LayoutSlot), "Length");
        return Task.CompletedTask;
    }

    private static Task LayoutLength_SupportsImplicitFixedIntegers()
    {
        var slot = new LayoutSlot
        {
            Content = new Label(),
            Length = 6,
        };

        TestAssert.True(slot.Length.Kind == LayoutLengthKind.Fixed, "Implicit integer conversion should produce a fixed layout length.");
        TestAssert.Equal(6, slot.Length.Value, "Implicit integer conversion should preserve the requested fixed size.");
        return Task.CompletedTask;
    }

    private static Task RowAndColumnSizingHelpers_AreMarkedAdvanced()
    {
        AssertMarkedAdvanced(typeof(RowLayout), nameof(RowLayout.AddAuto), [typeof(LayoutNode), typeof(Thickness)]);
        AssertMarkedAdvanced(typeof(RowLayout), nameof(RowLayout.AddFixed), [typeof(LayoutNode), typeof(int), typeof(Thickness)]);
        AssertMarkedAdvanced(typeof(RowLayout), nameof(RowLayout.AddFill), [typeof(LayoutNode), typeof(Thickness)]);
        AssertMarkedAdvanced(typeof(RowLayout), nameof(RowLayout.AddWeighted), [typeof(LayoutNode), typeof(int), typeof(Thickness)]);

        AssertMarkedAdvanced(typeof(ColumnLayout), nameof(ColumnLayout.AddAuto), [typeof(LayoutNode), typeof(Thickness)]);
        AssertMarkedAdvanced(typeof(ColumnLayout), nameof(ColumnLayout.AddFixed), [typeof(LayoutNode), typeof(int), typeof(Thickness)]);
        AssertMarkedAdvanced(typeof(ColumnLayout), nameof(ColumnLayout.AddFill), [typeof(LayoutNode), typeof(Thickness)]);
        AssertMarkedAdvanced(typeof(ColumnLayout), nameof(ColumnLayout.AddWeighted), [typeof(LayoutNode), typeof(int), typeof(Thickness)]);
        return Task.CompletedTask;
    }

    private static Task ScreenFromLayoutNode_IsMarkedAdvanced()
    {
        AssertMarkedAdvanced(typeof(Screen), nameof(Screen.From), [typeof(LayoutNode)]);
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

    private static Task RemovedCompositionTypes_AreAbsent()
    {
        var assembly = typeof(Screen).Assembly;

        foreach (var typeName in RemovedCompositionTypes)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is null, $"{typeName} should be removed once the root path owns the behavior directly.");
        }

        return Task.CompletedTask;
    }

    private static void AssertObjectInitializerShape(Type type, params string[] requiredProperties)
    {
        var constructor = type.GetConstructor(Type.EmptyTypes);
        TestAssert.True(constructor is not null, $"{type.Name} should expose a parameterless constructor for object-initializer assembly.");

        foreach (var propertyName in EnumerateRequiredPropertyNames(requiredProperties))
        {
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            TestAssert.True(property is not null, $"{type.Name} should expose a public {propertyName} property.");
            TestAssert.True(property!.SetMethod is not null, $"{type.Name}.{propertyName} should be settable during object initialization.");

            var requiredAttribute = (RequiredMemberAttribute?)Attribute.GetCustomAttribute(property, typeof(RequiredMemberAttribute));
            TestAssert.True(requiredAttribute is not null, $"{type.Name}.{propertyName} should stay required for valid object-initializer assembly.");
        }
    }

    private static void AssertMarkedAdvanced(Type type, string name, Type[] parameters)
    {
        var method = type.GetMethod(name, parameters);
        TestAssert.True(method is not null, $"{type.Name}.{name} should exist.");
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(method!, typeof(EditorBrowsableAttribute));
        TestAssert.True(attribute is not null, $"{type.Name}.{name} should be marked advanced.");
        TestAssert.True(attribute!.State == EditorBrowsableState.Advanced, $"{type.Name}.{name} should stay out of default discovery.");
    }

    private static IEnumerable<string> EnumerateRequiredPropertyNames(IEnumerable<string> requiredProperties)
    {
        yield return "Content";

        foreach (var propertyName in requiredProperties)
        {
            yield return propertyName;
        }
    }
}
