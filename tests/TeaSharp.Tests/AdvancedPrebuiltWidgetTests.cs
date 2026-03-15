using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class AdvancedPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Controls_Badge_RendersLabel", Badge_RendersLabel);
        yield return new TestCase("Controls_Toggle_TogglesValue", Toggle_TogglesValue);
        yield return new TestCase("Controls_Toggle_MouseClickTogglesValue", Toggle_MouseClickTogglesValue);
        yield return new TestCase("Controls_Slider_AdjustsValue", Slider_AdjustsValue);
        yield return new TestCase("Controls_Slider_MouseClickSetsValue", Slider_MouseClickSetsValue);
        yield return new TestCase("Controls_Slider_DragUpdatesValue", Slider_DragUpdatesValue);
        yield return new TestCase("Controls_Spinner_AdvancesFrame", Spinner_AdvancesFrame);
        yield return new TestCase("Controls_Spinner_MouseClickTogglesRunning", Spinner_MouseClickTogglesRunning);
        yield return new TestCase("Controls_Spinner_MouseWheelAdvancesFrame", Spinner_MouseWheelAdvancesFrame);
        yield return new TestCase("Controls_Toggle_MouseWheelSetsValue", Toggle_MouseWheelSetsValue);
        yield return new TestCase("Advanced_TreeViewComponent_TogglesExpansion", TreeViewComponent_TogglesExpansion);
        yield return new TestCase("Advanced_TreeViewComponent_MouseClickSelectsVisibleNode", TreeViewComponent_MouseClickSelectsVisibleNode);
        yield return new TestCase("Advanced_NotificationCenterComponent_DismissesEntries", NotificationCenterComponent_DismissesEntries);
        yield return new TestCase("Advanced_NotificationCenterComponent_MouseWheelMovesSelection", NotificationCenterComponent_MouseWheelMovesSelection);
    }

    private static Task Badge_RendersLabel()
    {
        var badge = new Badge
        {
            Text = "hot",
            Tone = BadgeTone.Warning,
        };
        var canvas = new Canvas(20, 1);

        badge.Render(canvas, new Rect(0, 0, 20, 1));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[hot]", StringComparison.Ordinal), "Badge should render bracketed text.");
        return Task.CompletedTask;
    }

    private static Task Toggle_TogglesValue()
    {
        var toggle = new Toggle
        {
            IsFocused = true,
        };

        toggle.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(toggle.Value, "Toggle should flip to on after enter.");
        toggle.Handle(new KeyPressed(Key.Left));
        TestAssert.True(!toggle.Value, "Toggle should flip to off after left.");
        return Task.CompletedTask;
    }

    private static Task Toggle_MouseClickTogglesValue()
    {
        var toggle = new Toggle
        {
            Border = BorderStyle.None,
        };

        var changed = toggle.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0), new Rect(0, 0, 10, 1));

        TestAssert.True(changed, "Toggle mouse click should report state change.");
        TestAssert.True(toggle.Value, "Toggle mouse click should enable value.");
        return Task.CompletedTask;
    }

    private static Task Slider_AdjustsValue()
    {
        var slider = new Slider
        {
            IsFocused = true,
            Min = 0,
            Max = 10,
            Step = 2,
        };

        slider.SetValue(4);
        slider.Handle(new KeyPressed(Key.Right));
        slider.Handle(new KeyPressed(Key.Right));
        slider.Handle(new KeyPressed(Key.Right));

        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider should clamp at max.");
        slider.Handle(new KeyPressed(Key.Left));
        TestAssert.True(Math.Abs(slider.Value - 8) < 0.0001, "Slider should decrement by step.");
        return Task.CompletedTask;
    }

    private static Task Slider_MouseClickSetsValue()
    {
        var slider = new Slider
        {
            Border = BorderStyle.None,
            Min = 0,
            Max = 10,
            Step = 1,
        };

        var changed = slider.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 19, 1), new Rect(0, 0, 20, 2));

        TestAssert.True(changed, "Slider mouse click should update slider value.");
        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider click at far-right should move value to max.");
        return Task.CompletedTask;
    }

    private static Task Slider_DragUpdatesValue()
    {
        var slider = new Slider
        {
            Border = BorderStyle.None,
            Min = 0,
            Max = 10,
            Step = 1,
        };

        var bounds = new Rect(0, 0, 20, 2);
        slider.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var changed = slider.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.Left, 19, 1), bounds);
        slider.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, 19, 1), bounds);

        TestAssert.True(changed, "Slider drag should update slider value.");
        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider drag to far-right should move value to max.");
        return Task.CompletedTask;
    }

    private static Task Spinner_AdvancesFrame()
    {
        var spinner = new Spinner
        {
            IsFocused = true,
        };
        var canvasBefore = new Canvas(20, 3);
        spinner.Render(canvasBefore, new Rect(0, 0, 20, 3));
        var before = canvasBefore.Render();

        spinner.Handle(new KeyPressed(Key.Right));
        var canvasAfter = new Canvas(20, 3);
        spinner.Render(canvasAfter, new Rect(0, 0, 20, 3));
        var after = canvasAfter.Render();

        TestAssert.True(!string.Equals(before, after, StringComparison.Ordinal), "Spinner should advance when running.");
        spinner.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(!spinner.Running, "Spinner should stop when toggled.");
        return Task.CompletedTask;
    }

    private static Task Spinner_MouseClickTogglesRunning()
    {
        var spinner = new Spinner
        {
            Border = BorderStyle.None,
        };

        var changed = spinner.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0), new Rect(0, 0, 16, 1));

        TestAssert.True(changed, "Spinner click should toggle running state.");
        TestAssert.True(!spinner.Running, "Spinner click should stop the spinner.");
        return Task.CompletedTask;
    }

    private static Task Spinner_MouseWheelAdvancesFrame()
    {
        var spinner = new Spinner
        {
            Border = BorderStyle.None,
        };
        var beforeCanvas = new Canvas(16, 1);
        spinner.Render(beforeCanvas, new Rect(0, 0, 16, 1));
        var before = beforeCanvas.Render();

        var changed = spinner.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, 0, 0), new Rect(0, 0, 16, 1));
        var afterCanvas = new Canvas(16, 1);
        spinner.Render(afterCanvas, new Rect(0, 0, 16, 1));
        var after = afterCanvas.Render();

        TestAssert.True(changed, "Spinner wheel should advance frame while running.");
        TestAssert.True(!string.Equals(before, after, StringComparison.Ordinal), "Spinner wheel should move frame index.");
        return Task.CompletedTask;
    }

    private static Task Toggle_MouseWheelSetsValue()
    {
        var toggle = new Toggle
        {
            Border = BorderStyle.None,
        };

        var changedOn = toggle.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelUp, 0, 0), new Rect(0, 0, 10, 1));
        var changedOff = toggle.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, 0, 0), new Rect(0, 0, 10, 1));

        TestAssert.True(changedOn, "Toggle wheel-up should change value.");
        TestAssert.True(changedOff, "Toggle wheel-down should change value.");
        TestAssert.True(!toggle.Value, "Toggle wheel-down should disable value.");
        return Task.CompletedTask;
    }

    private static Task TreeViewComponent_TogglesExpansion()
    {
        var tree = new TreeViewComponent
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        tree.SetRoots(
        [
            new TreeItemNode("root", "Root",
            [
                new TreeItemNode("child", "Child"),
            ]),
        ]);
        var canvas = new Canvas(40, 5);

        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var expanded = canvas.Render();
        TestAssert.True(expanded.Contains("Child", StringComparison.Ordinal), "Tree should render child when expanded.");

        tree.Update(new KeyPressMsg(KeyCode.Enter));
        canvas.Clear();
        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var collapsed = canvas.Render();
        TestAssert.True(!collapsed.Contains("Child", StringComparison.Ordinal), "Tree should hide child when collapsed.");
        return Task.CompletedTask;
    }

    private static Task TreeViewComponent_MouseClickSelectsVisibleNode()
    {
        var tree = new TreeViewComponent
        {
            Border = BorderStyle.None,
        };
        tree.SetRoots(
        [
            new TreeItemNode("root", "Root",
            [
                new TreeItemNode("child", "Child"),
            ]),
        ]);

        var changed = tree.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 1), new Rect(0, 0, 30, 4));

        TestAssert.True(changed, "Tree click should update selected node.");
        TestAssert.Equal("child", tree.SelectedNodeId ?? string.Empty, "Tree click should select visible row under pointer.");
        return Task.CompletedTask;
    }

    private static Task NotificationCenterComponent_DismissesEntries()
    {
        var center = new NotificationCenterComponent
        {
            IsFocused = true,
        };
        center.Push("hello", NotificationSeverity.Info, id: "a");
        center.Push("oops", NotificationSeverity.Error, id: "b");

        center.Update(new KeyPressMsg(KeyCode.Down));
        center.Update(new KeyPressMsg(KeyCode.Character, "d"));

        TestAssert.Equal(1, center.Entries.Count, "Notification center should dismiss selected entry.");
        TestAssert.Equal("a", center.Entries[0].Id, "Remaining entry should be the non-selected one.");
        return Task.CompletedTask;
    }

    private static Task NotificationCenterComponent_MouseWheelMovesSelection()
    {
        var center = new NotificationCenterComponent
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        center.Push("first", NotificationSeverity.Info, id: "a");
        center.Push("second", NotificationSeverity.Info, id: "b");
        center.Push("third", NotificationSeverity.Info, id: "c");

        var changed = center.UpdateMouse(new MouseWheelMsg(MouseButton.WheelUp, 0, 1), new Rect(0, 0, 32, 6));
        center.Update(new KeyPressMsg(KeyCode.Character, "d"));

        TestAssert.True(changed, "Notification center wheel should move selected entry.");
        TestAssert.Equal(2, center.Entries.Count, "Dismiss should remove wheel-selected entry.");
        TestAssert.True(center.Entries.Any(entry => entry.Id == "c"), "Newest entry should remain after moving selection up.");
        TestAssert.True(center.Entries.Any(entry => entry.Id == "a"), "Oldest entry should remain after removing middle entry.");
        return Task.CompletedTask;
    }
}
