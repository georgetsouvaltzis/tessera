using Tessera.Components.Primitives;
using Tessera.Controls;
using System.Globalization;

namespace Tessera.Tests;

internal static class ChartComponentTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Charts_LineChart_RendersPointsAndStats", LineChart_RendersPointsAndStats);
        yield return new TestCase("Charts_LineChart_WithAxesAndLegend_RendersAxisElements", LineChart_WithAxesAndLegend_RendersAxisElements);
        yield return new TestCase("Charts_BarChart_RendersLabelsAndBars", BarChart_RendersLabelsAndBars);
        yield return new TestCase("Charts_BarChart_WithScaleAndLegend_RendersScaleText", BarChart_WithScaleAndLegend_RendersScaleText);
        yield return new TestCase("Charts_LineChartComponent_HonorsCapacity", LineChartComponent_HonorsCapacity);
        yield return new TestCase("Charts_LineChart_WithZoomAndOffset_ShiftsWindow", LineChart_WithZoomAndOffset_ShiftsWindow);
    }

    private static Task LineChart_RendersPointsAndStats()
    {
        // Arrange
        var canvas = new Canvas(30, 10);
        var samples = new[] { 1.0, 2.0, 3.5, 2.4, 5.2, 4.1, 6.0 };
        var chart = new LineChart
        {
            Title = "CPU",
        };
        chart.SetSamples(samples);

        // Act
        chart.Render(canvas, new Rect(0, 0, 30, 10));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" CPU ", StringComparison.Ordinal), "Line chart should render title.");
        TestAssert.True(output.Contains('●'), "Line chart should render points.");
        TestAssert.True(output.Contains("min:", StringComparison.Ordinal), "Line chart should render min/max stats.");
        return Task.CompletedTask;
    }

    private static Task BarChart_RendersLabelsAndBars()
    {
        // Arrange
        var canvas = new Canvas(30, 8);
        BarPoint[] bars =
        [
            new("ok", 80),
            new("warn", 20),
            new("crit", 10),
        ];
        var chart = new BarChart
        {
            Title = "Status",
        };
        chart.SetBars(bars);

        // Act
        chart.Render(canvas, new Rect(0, 0, 30, 8));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Status ", StringComparison.Ordinal), "Bar chart should render title.");
        TestAssert.True(output.Contains("ok", StringComparison.Ordinal), "Bar chart should render labels.");
        TestAssert.True(output.Contains('█'), "Bar chart should render filled bars.");
        return Task.CompletedTask;
    }

    private static Task LineChart_WithAxesAndLegend_RendersAxisElements()
    {
        // Arrange
        var canvas = new Canvas(34, 12);
        var samples = new[] { 20.0, 30.0, 10.0, 50.0, 40.0, 60.0 };
        var chart = new LineChart
        {
            Title = "Latency",
            Options = new LineChartOptions(
                ShowAxes: true,
                Legend: "p95",
                XLabel: "time",
                YLabel: "ms"),
        };
        chart.SetSamples(samples);

        // Act
        chart.Render(canvas, new Rect(0, 0, 34, 12));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Latency ", StringComparison.Ordinal), "Line chart should render title with options.");
        TestAssert.True(output.Contains('└'), "Line chart with axes should render axis corner.");
        TestAssert.True(output.Contains("p95", StringComparison.Ordinal), "Line chart should render legend text.");
        TestAssert.True(output.Contains("time", StringComparison.Ordinal), "Line chart should render x-axis label.");
        return Task.CompletedTask;
    }

    private static Task BarChart_WithScaleAndLegend_RendersScaleText()
    {
        // Arrange
        var canvas = new Canvas(36, 8);
        IReadOnlyList<BarPoint> bars =
        [
            new("ok", 90),
            new("warn", 35),
            new("crit", 10),
        ];
        var chart = new BarChart
        {
            Title = "Health",
            Options = new BarChartOptions(
                ShowScale: true,
                Legend: "req/s"),
        };
        chart.SetBars(bars);

        // Act
        chart.Render(canvas, new Rect(0, 0, 36, 8));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Health ", StringComparison.Ordinal), "Bar chart should render title.");
        TestAssert.True(output.Contains("req/s", StringComparison.Ordinal), "Bar chart should render legend.");
        TestAssert.True(output.Contains("0..", StringComparison.Ordinal), "Bar chart should render scale range text.");
        return Task.CompletedTask;
    }

    private static Task LineChartComponent_HonorsCapacity()
    {
        // Arrange
        var chart = new LineChart(capacity: 4);

        // Act
        chart.Append(1);
        chart.Append(2);
        chart.Append(3);
        chart.Append(4);
        chart.Append(5);

        // Assert
        TestAssert.Equal(4, chart.Samples.Count, "Line chart should keep only the latest samples.");
        TestAssert.Equal(2d, chart.Samples[0], "Oldest sample should be dropped once capacity is exceeded.");
        TestAssert.Equal(5d, chart.Samples[^1], "Newest sample should be retained.");
        return Task.CompletedTask;
    }

    private static Task LineChart_WithZoomAndOffset_ShiftsWindow()
    {
        // Arrange
        var samples = Enumerable.Range(0, 20).Select(i => (double)i).ToArray();
        var baseCanvas = new Canvas(32, 10);
        var zoomedCanvas = new Canvas(32, 10);
        var baselineChart = new LineChart
        {
            Title = "Zoom",
            Options = new LineChartOptions(),
            Zoom = 1.0,
            Offset = 0,
        };
        baselineChart.SetSamples(samples);
        var shiftedChart = new LineChart
        {
            Title = "Zoom",
            Options = new LineChartOptions(),
            Zoom = 2.0,
            Offset = 6,
        };
        shiftedChart.SetSamples(samples);

        // Act
        baselineChart.Render(baseCanvas, new Rect(0, 0, 32, 10));
        shiftedChart.Render(zoomedCanvas, new Rect(0, 0, 32, 10));
        var baseline = baseCanvas.Render();
        var zoomed = zoomedCanvas.Render();

        // Assert
        TestAssert.True(baseline.Contains("min:0.0", StringComparison.Ordinal), "Baseline chart should include first sample in stats.");
        TestAssert.True(!zoomed.Contains("min:0.0", StringComparison.Ordinal), "Zoom+offset chart should shift visible window away from zero baseline.");
        TestAssert.True(zoomed.Contains("max:", StringComparison.Ordinal), "Zoom+offset chart should keep stats rendering.");
        return Task.CompletedTask;
    }

}
