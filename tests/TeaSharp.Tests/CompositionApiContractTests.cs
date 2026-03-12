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
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class CompositionApiContractTests
{
    private static readonly Type[] AdvancedCompositionTypes =
    [
        typeof(ScreenComposer),
        typeof(InputRouter),
        typeof(InteractiveScreenModel),
        typeof(ScreenRegionKey),
        typeof(DialogWorkflow),
        typeof(InputScope),
        typeof(InputScopeBehavior),
        typeof(InputScopeKind),
        typeof(InputRouteResult),
        typeof(ScreenFocusChain),
        typeof(ScreenFocusSnapshot),
        typeof(ScreenFrameLayout),
        typeof(ScreenLayer),
        typeof(KeyboardRoutingMode),
        typeof(MasterDetailScreen),
        typeof(DashboardScreen),
        typeof(FormScreen),
        typeof(TeaSharp.Components.UiKit.Layout),
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CompositionApi_ComponentComposer_IsMarkedAdvanced", ComponentComposer_IsMarkedAdvanced);
        yield return new TestCase("CompositionApi_AdvancedCompositionTypes_AreMarkedAdvanced", AdvancedCompositionTypes_AreMarkedAdvanced);
        yield return new TestCase("CompositionApi_ScreenComposer_StringOverloads_AreMarkedAdvanced", ScreenComposer_StringOverloads_AreMarkedAdvanced);
    }

    private static Task ComponentComposer_IsMarkedAdvanced()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(ComponentComposer),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, "ComponentComposer should be explicitly marked as an advanced composition surface.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            "ComponentComposer should be hidden from default discovery paths.");
        return Task.CompletedTask;
    }

    private static Task AdvancedCompositionTypes_AreMarkedAdvanced()
    {
        foreach (var type in AdvancedCompositionTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                type,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is not null, $"{type.Name} should be explicitly marked as advanced.");
            TestAssert.True(
                attribute!.State == EditorBrowsableState.Advanced,
                $"{type.Name} should be hidden from the default discoverable API.");
        }

        return Task.CompletedTask;
    }

    private static Task ScreenComposer_StringOverloads_AreMarkedAdvanced()
    {
        MethodInfo?[] methods =
        [
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddRegion), [typeof(string), typeof(Rect), typeof(Action<Canvas, Rect>), typeof(Func<IMessage, bool>), typeof(Func<MouseMsg, Rect, bool>), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddComponent), [typeof(string), typeof(Rect), typeof(ICanvasComponent), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddOverlayRegion), [typeof(string), typeof(Rect), typeof(Action<Canvas, Rect>), typeof(Func<IMessage, bool>), typeof(Func<MouseMsg, Rect, bool>), typeof(bool), typeof(bool), typeof(bool), typeof(ScreenLayer), typeof(Action)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddOverlayComponent), [typeof(string), typeof(Rect), typeof(ICanvasComponent), typeof(bool?), typeof(bool), typeof(bool), typeof(ScreenLayer), typeof(Action)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddModalComponent), [typeof(string), typeof(Rect), typeof(ICanvasComponent), typeof(bool?), typeof(Action)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddPaletteComponent), [typeof(string), typeof(Rect), typeof(ICanvasComponent), typeof(bool?), typeof(Action)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.AddToastOverlay), [typeof(string), typeof(Rect), typeof(ICanvasComponent)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.SetFocus), [typeof(string)]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.TryGetBounds), [typeof(string), typeof(Rect).MakeByRefType()]),
            typeof(ScreenComposer).GetMethod(nameof(ScreenComposer.CompleteFrame), [typeof(string)]),
        ];

        foreach (var method in methods)
        {
            TestAssert.True(method is not null, "Expected ScreenComposer string overload to exist for compatibility.");

            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                method!,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is not null, $"{method!.Name} string overload should be explicitly marked as advanced.");
            TestAssert.True(
                attribute!.State == EditorBrowsableState.Advanced,
                $"{method!.Name} string overload should be hidden from default discovery.");
        }

        return Task.CompletedTask;
    }
}
