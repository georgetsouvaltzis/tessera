using TeaSharp.Components.Primitives;
using System.Reflection;

namespace TeaSharp.Tests;

internal static class ComponentRenderingTests
{
    private static readonly string[] CardLines = ["line one", "line two"];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Components_Canvas_DrawBox_RendersFrameAndTitle", Canvas_DrawBox_RendersFrameAndTitle);
        yield return new TestCase("Components_Widgets_DrawProgressBar_RendersExpectedFill", Widgets_DrawProgressBar_RendersExpectedFill);
        yield return new TestCase("Components_Widgets_DrawSparkline_MapsValuesToBlocks", Widgets_DrawSparkline_MapsValuesToBlocks);
        yield return new TestCase("Components_Widgets_DrawList_MarksSelectedRow", Widgets_DrawList_MarksSelectedRow);
        yield return new TestCase("Components_Widgets_DrawCard_RendersAccentAndBody", Widgets_DrawCard_RendersAccentAndBody);
        yield return new TestCase("Components_Widgets_DrawTable_RendersHeadersRowsAndSelection", Widgets_DrawTable_RendersHeadersRowsAndSelection);
    }

    private static Task Canvas_DrawBox_RendersFrameAndTitle()
    {
        // Arrange
        var canvas = new Canvas(20, 5);

        // Act
        canvas.DrawBox(new Rect(0, 0, 20, 5), "Panel");
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains('┌'), "Box should include top-left corner.");
        TestAssert.True(output.Contains('┘'), "Box should include bottom-right corner.");
        TestAssert.True(output.Contains(" Panel ", StringComparison.Ordinal), "Box should render title in top border.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawProgressBar_RendersExpectedFill()
    {
        // Arrange
        var canvas = new Canvas(14, 2);

        // Act
        InvokeWidgets("DrawProgressBar", canvas, new Rect(1, 0, 12, 2), 0.5, "50%");
        var lines = canvas.Render().Split('\n');

        // Assert
        TestAssert.True(
            lines[0].Contains("[█████░░░░░]", StringComparison.Ordinal),
            "Progress bar should fill half of inner slots.");
        TestAssert.True(lines[1].Contains("50%", StringComparison.Ordinal), "Progress bar should render label on second row.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawSparkline_MapsValuesToBlocks()
    {
        // Arrange
        var canvas = new Canvas(8, 1);
        var values = new[] { 0, 14, 28, 42, 57, 71, 85, 100 };

        // Act
        InvokeWidgets("DrawSparkline", canvas, new Rect(0, 0, 8, 1), values, 0, 100);
        var output = canvas.Render();

        // Assert
        TestAssert.Equal("▁▂▃▄▅▆▇█", output, "Sparkline should map rising values to ascending blocks.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawList_MarksSelectedRow()
    {
        // Arrange
        var canvas = new Canvas(18, 3);
        var items = new[] { "alpha", "beta", "gamma" };

        // Act
        InvokeWidgets("DrawList", canvas, new Rect(0, 0, 18, 3), items, 1);
        var lines = canvas.Render().Split('\n');

        // Assert
        TestAssert.True(lines[0].StartsWith("  alpha", StringComparison.Ordinal), "Non-selected row should have default prefix.");
        TestAssert.True(lines[1].StartsWith("› beta", StringComparison.Ordinal), "Selected row should have indicator prefix.");
        TestAssert.True(lines[2].StartsWith("  gamma", StringComparison.Ordinal), "Non-selected row should have default prefix.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawCard_RendersAccentAndBody()
    {
        // Arrange
        var canvas = new Canvas(24, 6);

        // Act
        InvokeWidgets("DrawCard",
            canvas,
            new Rect(0, 0, 24, 6),
            "Card",
            CardLines,
            '▌');
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Card ", StringComparison.Ordinal), "Card should render title in border.");
        TestAssert.True(output.Contains('▌'), "Card should render accent marker.");
        TestAssert.True(output.Contains("line one", StringComparison.Ordinal), "Card should render body text.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawTable_RendersHeadersRowsAndSelection()
    {
        // Arrange
        var canvas = new Canvas(34, 8);
        var headers = new[] { "Metric", "Value", "Status" };
        IReadOnlyList<IReadOnlyList<string>> rows =
        [
            new[] { "CPU", "18%", "ok" },
            new[] { "Mem", "63%", "warn" },
            new[] { "IO", "44%", "ok" },
        ];

        // Act
        InvokeWidgets("DrawTable", canvas, new Rect(0, 0, 34, 8), headers, rows, 1, "Stats", BorderStyle.SingleLine, default(Thickness));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Stats ", StringComparison.Ordinal), "Table should render title.");
        TestAssert.True(output.Contains("Metric", StringComparison.Ordinal), "Table should render header row.");
        TestAssert.True(output.Contains('┼'), "Table should render header divider intersections.");
        TestAssert.True(output.Contains("› Mem", StringComparison.Ordinal), "Selected row should include selection prefix.");
        return Task.CompletedTask;
    }

    private static void InvokeWidgets(string methodName, params object?[] arguments)
    {
        var type = typeof(Canvas).Assembly.GetType("TeaSharp.Components.Primitives.Widgets", throwOnError: true)!;
        var method = type
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .SingleOrDefault(candidate => string.Equals(candidate.Name, methodName, StringComparison.Ordinal)
                && candidate.GetParameters().Length == arguments.Length);
        TestAssert.True(method is not null, $"Widgets.{methodName} should continue to exist as an internal bridge.");
        method!.Invoke(null, arguments);
    }
}
