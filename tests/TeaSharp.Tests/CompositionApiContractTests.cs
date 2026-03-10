using System.ComponentModel;
using TeaSharp.Components;

namespace TeaSharp.Tests;

internal static class CompositionApiContractTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("CompositionApi_ComponentComposer_IsMarkedAdvanced", ComponentComposer_IsMarkedAdvanced);
        yield return new TestCase("CompositionApi_ScreenComposer_RemainsDefaultSurface", ScreenComposer_RemainsDefaultSurface);
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

    private static Task ScreenComposer_RemainsDefaultSurface()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(ScreenComposer),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is null, "ScreenComposer should remain the default discoverable composition surface.");
        return Task.CompletedTask;
    }
}
