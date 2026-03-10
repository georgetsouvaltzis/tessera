using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

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
        yield return new TestCase("Components_Composer_DispatchesStatefulUpdates", Composer_DispatchesStatefulUpdates);
        yield return new TestCase("Components_Composer_FocusedRoutingTargetsFocusedSlotOnly", Composer_FocusedRoutingTargetsFocusedSlotOnly);
        yield return new TestCase("Components_Composer_BroadcastRoutingUpdatesAllSlots", Composer_BroadcastRoutingUpdatesAllSlots);
        yield return new TestCase("Components_Composer_FocusFirstTargetsFirstFocusableSlot", Composer_FocusFirstTargetsFirstFocusableSlot);
        yield return new TestCase("Components_Composer_FocusNextCyclesAcrossFocusableSlots", Composer_FocusNextCyclesAcrossFocusableSlots);
        yield return new TestCase("Components_Composer_FocusPreviousCyclesBackwardAcrossFocusableSlots", Composer_FocusPreviousCyclesBackwardAcrossFocusableSlots);
        yield return new TestCase("Components_Composer_MouseClickFocusesTargetSlot", Composer_MouseClickFocusesTargetSlot);
        yield return new TestCase("Components_Composer_MouseWheelFallsBackToFocusedSlot", Composer_MouseWheelFallsBackToFocusedSlot);
    }

    private static Task LineChart_RendersPointsAndStats()
    {
        // Arrange
        var canvas = new Canvas(30, 10);
        var samples = new[] { 1.0, 2.0, 3.5, 2.4, 5.2, 4.1, 6.0 };

        // Act
        Charts.DrawLineChart(canvas, new Rect(0, 0, 30, 10), samples, title: "CPU");
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" CPU ", StringComparison.Ordinal), "Line chart should render title.");
        TestAssert.True(output.Contains("●", StringComparison.Ordinal), "Line chart should render points.");
        TestAssert.True(output.Contains("min:", StringComparison.Ordinal), "Line chart should render min/max stats.");
        return Task.CompletedTask;
    }

    private static Task BarChart_RendersLabelsAndBars()
    {
        // Arrange
        var canvas = new Canvas(30, 8);
        BarDatum[] bars =
        [
            new("ok", 80),
            new("warn", 20),
            new("crit", 10),
        ];

        // Act
        Charts.DrawBarChart(canvas, new Rect(0, 0, 30, 8), bars, title: "Status");
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Status ", StringComparison.Ordinal), "Bar chart should render title.");
        TestAssert.True(output.Contains("ok", StringComparison.Ordinal), "Bar chart should render labels.");
        TestAssert.True(output.Contains("█", StringComparison.Ordinal), "Bar chart should render filled bars.");
        return Task.CompletedTask;
    }

    private static Task LineChart_WithAxesAndLegend_RendersAxisElements()
    {
        // Arrange
        var canvas = new Canvas(34, 12);
        var samples = new[] { 20.0, 30.0, 10.0, 50.0, 40.0, 60.0 };

        // Act
        Charts.DrawLineChart(
            canvas,
            new Rect(0, 0, 34, 12),
            samples,
            title: "Latency",
            options: new LineChartOptions(
                ShowAxes: true,
                Legend: "p95",
                XLabel: "time",
                YLabel: "ms"));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains(" Latency ", StringComparison.Ordinal), "Line chart should render title with options.");
        TestAssert.True(output.Contains("└", StringComparison.Ordinal), "Line chart with axes should render axis corner.");
        TestAssert.True(output.Contains("p95", StringComparison.Ordinal), "Line chart should render legend text.");
        TestAssert.True(output.Contains("time", StringComparison.Ordinal), "Line chart should render x-axis label.");
        return Task.CompletedTask;
    }

    private static Task BarChart_WithScaleAndLegend_RendersScaleText()
    {
        // Arrange
        var canvas = new Canvas(36, 8);
        IReadOnlyList<BarDatum> bars =
        [
            new("ok", 90),
            new("warn", 35),
            new("crit", 10),
        ];

        // Act
        Charts.DrawBarChart(
            canvas,
            new Rect(0, 0, 36, 8),
            bars,
            title: "Health",
            options: new BarChartOptions(
                ShowScale: true,
                Legend: "req/s"));
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
        var component = new LineChartComponent(capacity: 4);

        // Act
        component.Append(1);
        component.Append(2);
        component.Append(3);
        component.Append(4);
        component.Append(5);

        // Assert
        TestAssert.Equal(4, component.Samples.Count, "Line chart component should keep only the latest samples.");
        TestAssert.Equal(2d, component.Samples[0], "Oldest sample should be dropped once capacity is exceeded.");
        TestAssert.Equal(5d, component.Samples[^1], "Newest sample should be retained.");
        return Task.CompletedTask;
    }

    private static Task LineChart_WithZoomAndOffset_ShiftsWindow()
    {
        // Arrange
        var samples = Enumerable.Range(0, 20).Select(i => (double)i).ToArray();
        var baseCanvas = new Canvas(32, 10);
        var zoomedCanvas = new Canvas(32, 10);

        // Act
        Charts.DrawLineChart(
            baseCanvas,
            new Rect(0, 0, 32, 10),
            samples,
            title: "Zoom",
            options: new LineChartOptions(Zoom: 1.0, Offset: 0));
        Charts.DrawLineChart(
            zoomedCanvas,
            new Rect(0, 0, 32, 10),
            samples,
            title: "Zoom",
            options: new LineChartOptions(Zoom: 2.0, Offset: 6));
        var baseline = baseCanvas.Render();
        var zoomed = zoomedCanvas.Render();

        // Assert
        TestAssert.True(baseline.Contains("min:0.0", StringComparison.Ordinal), "Baseline chart should include first sample in stats.");
        TestAssert.True(!zoomed.Contains("min:0.0", StringComparison.Ordinal), "Zoom+offset chart should shift visible window away from zero baseline.");
        TestAssert.True(zoomed.Contains("max:", StringComparison.Ordinal), "Zoom+offset chart should keep stats rendering.");
        return Task.CompletedTask;
    }

    private static Task Composer_DispatchesStatefulUpdates()
    {
        // Arrange
        var canvas = new Canvas(20, 5);
        var composer = new ComponentComposer();
        var counter = new CounterComponent();
        composer.Add(counter, new Rect(0, 0, 20, 5));

        // Act
        var changed = composer.Update(new KeyPressMsg(KeyCode.Character, "x"));
        composer.Render(canvas);
        var output = canvas.Render();

        // Assert
        TestAssert.True(changed, "Composer should surface state changes from stateful components.");
        TestAssert.True(output.Contains("count=1", StringComparison.Ordinal), "Stateful component render should reflect updated state.");
        return Task.CompletedTask;
    }

    private static Task Composer_FocusedRoutingTargetsFocusedSlotOnly()
    {
        var composer = new ComponentComposer();
        var first = new KeyProbeComponent { Focused = true };
        var second = new KeyProbeComponent();
        composer.Add(first, new Rect(0, 0, 10, 4));
        composer.Add(second, new Rect(10, 0, 10, 4));

        var changed = composer.Update(new KeyPressMsg(KeyCode.Character, "x"));

        TestAssert.True(changed, "Focused routing should report handled key updates.");
        TestAssert.Equal(1, first.KeyUpdates, "Focused slot should receive keyboard input.");
        TestAssert.Equal(0, second.KeyUpdates, "Non-focused slot should not receive keyboard input.");
        return Task.CompletedTask;
    }

    private static Task Composer_BroadcastRoutingUpdatesAllSlots()
    {
        var composer = new ComponentComposer
        {
            KeyboardRoutingMode = KeyboardRoutingMode.Broadcast,
        };
        var first = new KeyProbeComponent { Focused = true };
        var second = new KeyProbeComponent();
        composer.Add(first, new Rect(0, 0, 10, 4));
        composer.Add(second, new Rect(10, 0, 10, 4));

        var changed = composer.Update(new KeyPressMsg(KeyCode.Character, "x"));

        TestAssert.True(changed, "Broadcast routing should report handled key updates.");
        TestAssert.Equal(1, first.KeyUpdates, "First slot should receive keyboard input.");
        TestAssert.Equal(1, second.KeyUpdates, "Broadcast mode should also update non-focused slots.");
        return Task.CompletedTask;
    }

    private static Task Composer_FocusFirstTargetsFirstFocusableSlot()
    {
        var composer = new ComponentComposer();
        var first = new CounterComponent();
        var second = new KeyProbeComponent();
        composer.Add(first, new Rect(0, 0, 10, 4));
        composer.Add(second, new Rect(10, 0, 10, 4));

        var changed = composer.FocusFirst();

        TestAssert.True(changed, "FocusFirst should focus the first focusable slot.");
        TestAssert.Equal(1, composer.FocusedSlotIndex, "FocusFirst should skip non-focusable slots.");
        TestAssert.True(second.Focused, "First focusable slot should become focused.");
        return Task.CompletedTask;
    }

    private static Task Composer_FocusNextCyclesAcrossFocusableSlots()
    {
        var composer = new ComponentComposer();
        var first = new KeyProbeComponent { Focused = true };
        var second = new CounterComponent();
        var third = new KeyProbeComponent();
        composer.Add(first, new Rect(0, 0, 10, 4));
        composer.Add(second, new Rect(10, 0, 10, 4));
        composer.Add(third, new Rect(20, 0, 10, 4));

        var changed = composer.FocusNext();

        TestAssert.True(changed, "FocusNext should advance focus.");
        TestAssert.True(!first.Focused, "Current focused slot should lose focus.");
        TestAssert.True(third.Focused, "FocusNext should skip non-focusable slots.");
        TestAssert.Equal(2, composer.FocusedSlotIndex, "Focused slot index should move to the next focusable slot.");
        return Task.CompletedTask;
    }

    private static Task Composer_FocusPreviousCyclesBackwardAcrossFocusableSlots()
    {
        var composer = new ComponentComposer();
        var first = new KeyProbeComponent();
        var second = new CounterComponent();
        var third = new KeyProbeComponent { Focused = true };
        composer.Add(first, new Rect(0, 0, 10, 4));
        composer.Add(second, new Rect(10, 0, 10, 4));
        composer.Add(third, new Rect(20, 0, 10, 4));

        var changed = composer.FocusPrevious();

        TestAssert.True(changed, "FocusPrevious should move focus backward.");
        TestAssert.True(first.Focused, "FocusPrevious should wrap to the previous focusable slot.");
        TestAssert.True(!third.Focused, "Previous focused slot should lose focus.");
        TestAssert.Equal(0, composer.FocusedSlotIndex, "Focused slot index should wrap backward.");
        return Task.CompletedTask;
    }

    private static Task Composer_MouseClickFocusesTargetSlot()
    {
        var composer = new ComponentComposer();
        var first = new MouseProbeComponent { Focused = true };
        var second = new MouseProbeComponent { Focused = false };
        composer.Add(first, new Rect(0, 0, 10, 4));
        composer.Add(second, new Rect(10, 0, 10, 4));

        var changed = composer.Update(new MouseClickMsg(MouseButton.Left, 12, 1));

        TestAssert.True(changed, "Mouse click should update focus and route event.");
        TestAssert.True(!first.Focused, "First slot should lose focus.");
        TestAssert.True(second.Focused, "Clicked slot should gain focus.");
        TestAssert.Equal(1, second.MouseUpdates, "Clicked slot should receive mouse message.");
        return Task.CompletedTask;
    }

    private static Task Composer_MouseWheelFallsBackToFocusedSlot()
    {
        var composer = new ComponentComposer();
        var focused = new MouseProbeComponent { Focused = true };
        composer.Add(focused, new Rect(0, 0, 10, 4));

        var changed = composer.Update(new MouseWheelMsg(MouseButton.WheelDown, 200, 200));

        TestAssert.True(changed, "Wheel outside bounds should still route to focused slot.");
        TestAssert.Equal(1, focused.MouseUpdates, "Focused slot should receive wheel event.");
        return Task.CompletedTask;
    }

    private sealed class CounterComponent : IStatefulComponent
    {
        private int _count;

        public bool Update(IMessage message)
        {
            if (message is KeyPressMsg)
            {
                _count++;
                return true;
            }

            return false;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.DrawBox(rect, "Counter");
            var body = rect.Inset(1, 1);
            canvas.WriteText(body.X, body.Y, $"count={_count}", body.Width);
        }
    }

    private sealed class KeyProbeComponent : IStatefulComponent, IFocusableComponent
    {
        public bool Focused { get; set; }

        public int KeyUpdates { get; private set; }

        public bool Update(IMessage message)
        {
            if (message is not KeyPressMsg)
            {
                return false;
            }

            KeyUpdates++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, KeyUpdates.ToString(), rect.Width);
        }
    }

    private sealed class MouseProbeComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
    {
        public bool Focused { get; set; }

        public int MouseUpdates { get; private set; }

        public bool Update(IMessage message) => false;

        public bool UpdateMouse(MouseMsg message, Rect bounds)
        {
            MouseUpdates++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, Focused ? "focused" : "idle", rect.Width);
        }
    }
}
