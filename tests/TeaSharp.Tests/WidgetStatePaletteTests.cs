using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class WidgetStatePaletteTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("WidgetStatePalette_InheritsParentStateAppearance", InheritsParentStateAppearance);
        yield return new TestCase("WidgetStatePalette_ChildOverridesParentStateAppearance", ChildOverridesParentStateAppearance);
        yield return new TestCase("WidgetStatePalette_RejectsParentCycles", RejectsParentCycles);
    }

    private static Task InheritsParentStateAppearance()
    {
        var parent = new WidgetStatePalette();
        parent[WidgetVisualState.Selected] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            Prefix = "P:",
        };

        var child = new WidgetStatePalette
        {
            Parent = parent,
        };

        var rendered = child.Render("item", WidgetVisualState.Selected);

        TestAssert.True(rendered.Contains("P:item", StringComparison.Ordinal), "Child palette should inherit selected prefix from parent.");
        TestAssert.True(rendered.Contains("38;5;12", StringComparison.Ordinal), "Child palette should inherit selected color from parent.");
        return Task.CompletedTask;
    }

    private static Task ChildOverridesParentStateAppearance()
    {
        var parent = new WidgetStatePalette();
        parent[WidgetVisualState.Selected] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            Prefix = "P:",
        };

        var child = new WidgetStatePalette
        {
            Parent = parent,
        };
        child[WidgetVisualState.Selected] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            Prefix = "C:",
        };

        var rendered = child.Render("item", WidgetVisualState.Selected);

        TestAssert.True(rendered.Contains("C:item", StringComparison.Ordinal), "Child palette should override selected prefix.");
        TestAssert.True(rendered.Contains("38;5;10", StringComparison.Ordinal), "Child palette should override selected color.");
        return Task.CompletedTask;
    }

    private static Task RejectsParentCycles()
    {
        var a = new WidgetStatePalette();
        var b = new WidgetStatePalette
        {
            Parent = a,
        };

        var threw = false;
        try
        {
            a.Parent = b;
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        TestAssert.True(threw, "Palette should reject cyclical parent chains.");
        return Task.CompletedTask;
    }
}
