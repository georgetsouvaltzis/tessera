using TeaSharp.Components;

namespace TeaSharp.Tests;

internal static class ComponentRenderingTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Components_Canvas_DrawBox_RendersFrameAndTitle", Canvas_DrawBox_RendersFrameAndTitle);
        yield return new TestCase("Components_Widgets_DrawProgressBar_RendersExpectedFill", Widgets_DrawProgressBar_RendersExpectedFill);
        yield return new TestCase("Components_Widgets_DrawSparkline_MapsValuesToBlocks", Widgets_DrawSparkline_MapsValuesToBlocks);
        yield return new TestCase("Components_Widgets_DrawList_MarksSelectedRow", Widgets_DrawList_MarksSelectedRow);
    }

    private static Task Canvas_DrawBox_RendersFrameAndTitle()
    {
        // Arrange
        var canvas = new Canvas(20, 5);

        // Act
        canvas.DrawBox(new Rect(0, 0, 20, 5), "Panel");
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains("┌", StringComparison.Ordinal), "Box should include top-left corner.");
        TestAssert.True(output.Contains("┘", StringComparison.Ordinal), "Box should include bottom-right corner.");
        TestAssert.True(output.Contains(" Panel ", StringComparison.Ordinal), "Box should render title in top border.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawProgressBar_RendersExpectedFill()
    {
        // Arrange
        var canvas = new Canvas(14, 2);

        // Act
        Widgets.DrawProgressBar(canvas, new Rect(1, 0, 12, 2), 0.5, "50%");
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
        Widgets.DrawSparkline(canvas, new Rect(0, 0, 8, 1), values, minValue: 0, maxValue: 100);
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
        Widgets.DrawList(canvas, new Rect(0, 0, 18, 3), items, selectedIndex: 1);
        var lines = canvas.Render().Split('\n');

        // Assert
        TestAssert.True(lines[0].StartsWith("  alpha", StringComparison.Ordinal), "Non-selected row should have default prefix.");
        TestAssert.True(lines[1].StartsWith("› beta", StringComparison.Ordinal), "Selected row should have indicator prefix.");
        TestAssert.True(lines[2].StartsWith("  gamma", StringComparison.Ordinal), "Non-selected row should have default prefix.");
        return Task.CompletedTask;
    }
}
