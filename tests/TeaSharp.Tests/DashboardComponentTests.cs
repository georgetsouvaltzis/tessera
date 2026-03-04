using TeaSharp.Components;

namespace TeaSharp.Tests;

internal static class DashboardComponentTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Components_Canvas_GraphemeAware_RendersWideAndCombiningText", Canvas_GraphemeAware_RendersWideAndCombiningText);
        yield return new TestCase("Components_Gauge_RendersValueAndLabel", Gauge_RendersValueAndLabel);
        yield return new TestCase("Components_StatsCard_RendersEntries", StatsCard_RendersEntries);
        yield return new TestCase("Components_MiniLog_RespectsCapacityAndShowsLatest", MiniLog_RespectsCapacityAndShowsLatest);
    }

    private static Task Canvas_GraphemeAware_RendersWideAndCombiningText()
    {
        // Arrange
        var canvas = new Canvas(6, 1, CanvasTextMode.GraphemeAware);

        // Act
        canvas.WriteText(0, 0, "A😀e\u0301B", 6);
        var output = canvas.Render();

        // Assert
        TestAssert.Equal("A😀e\u0301B ", output, "Grapheme-aware mode should preserve wide and combining text while keeping layout width.");
        return Task.CompletedTask;
    }

    private static Task Gauge_RendersValueAndLabel()
    {
        // Arrange
        var canvas = new Canvas(26, 5);
        var gauge = new GaugeComponent
        {
            Title = "Load",
            MinValue = 0,
            MaxValue = 100,
            Value = 66,
            Label = "66%",
        };

        // Act
        gauge.Render(canvas, new Rect(0, 0, 26, 5));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Load ", StringComparison.Ordinal), "Gauge should render title.");
        TestAssert.True(output.Contains("66%", StringComparison.Ordinal), "Gauge should render label.");
        TestAssert.True(output.Contains("█", StringComparison.Ordinal), "Gauge should render filled bar cells.");
        return Task.CompletedTask;
    }

    private static Task StatsCard_RendersEntries()
    {
        // Arrange
        var canvas = new Canvas(34, 7);
        var stats = new StatsCardComponent
        {
            Title = "Stats",
        };
        stats.SetItems(
        [
            new StatsCardItem("raw", "yes"),
            new StatsCardItem("mouse", "yes"),
            new StatsCardItem("paste", "no"),
        ]);

        // Act
        stats.Render(canvas, new Rect(0, 0, 34, 7));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Stats ", StringComparison.Ordinal), "Stats card should render title.");
        TestAssert.True(output.Contains("raw", StringComparison.Ordinal), "Stats card should include item labels.");
        TestAssert.True(output.Contains("yes", StringComparison.Ordinal), "Stats card should include item values.");
        return Task.CompletedTask;
    }

    private static Task MiniLog_RespectsCapacityAndShowsLatest()
    {
        // Arrange
        var canvas = new Canvas(30, 6);
        var log = new MiniLogComponent(capacity: 3)
        {
            Title = "Live Event",
        };
        log.Append("one");
        log.Append("two");
        log.Append("three");
        log.Append("four");

        // Act
        log.Render(canvas, new Rect(0, 0, 30, 6));
        var output = canvas.Render();

        // Assert
        TestAssert.Equal(3, log.Entries.Count, "Mini log should keep bounded history.");
        TestAssert.True(output.Contains("Live Event", StringComparison.Ordinal), "Mini log should render title.");
        TestAssert.True(output.Contains("two", StringComparison.Ordinal), "Mini log should retain recent lines.");
        TestAssert.True(output.Contains("four", StringComparison.Ordinal), "Mini log should render latest line.");
        return Task.CompletedTask;
    }
}
