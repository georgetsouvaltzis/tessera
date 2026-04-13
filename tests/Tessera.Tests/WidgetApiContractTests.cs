namespace Tessera.Tests;

internal static class WidgetApiContractTests
{
    private static readonly string[] InternalizedWidgetTypes =
    [
        "Tessera.Widgets.IWidgetKeyMap",
        "Tessera.Widgets.HelpView",
        "Tessera.Widgets.KeyBinding",
        "Tessera.Widgets.TextInputKeyMap",
        "Tessera.Widgets.ListKeyMap",
        "Tessera.Widgets.ViewportKeyMap",
        "Tessera.Widgets.TextInputFrame",
        "Tessera.Widgets.TextInputModel",
        "Tessera.Widgets.TextInputUpdateResult",
        "Tessera.Widgets.ListRow`1",
        "Tessera.Widgets.ListModel`1",
        "Tessera.Widgets.ViewportModel",
        "Tessera.Components.Styling.WidgetVisualState",
        "Tessera.Components.Styling.WidgetStatePalette",
        "Tessera.Components.Styling.WidgetStateAppearance"
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("WidgetApi_InternalizedWidgetTypes_AreNotPublic",
            InternalizedWidgetTypes_AreNotPublic);
    }

    private static Task InternalizedWidgetTypes_AreNotPublic()
    {
        var assembly = typeof(Screen).Assembly;

        foreach (var typeName in InternalizedWidgetTypes)
        {
            var type = assembly.GetType(typeName, false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public.");
        }

        return Task.CompletedTask;
    }
}
