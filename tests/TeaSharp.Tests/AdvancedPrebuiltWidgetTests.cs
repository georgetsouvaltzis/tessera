using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

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
        yield return new TestCase("Controls_TreeView_TogglesExpansion", TreeView_TogglesExpansion);
        yield return new TestCase("Controls_TreeView_MouseClickSelectsVisibleNode", TreeView_MouseClickSelectsVisibleNode);
        yield return new TestCase("Controls_Notifications_DismissesEntries", Notifications_DismissesEntries);
        yield return new TestCase("Controls_Notifications_MouseWheelMovesSelection", Notifications_MouseWheelMovesSelection);
        yield return new TestCase("Controls_ToastCenter_KeyboardNavigationAndDismiss", ToastCenter_KeyboardNavigationAndDismiss);
        yield return new TestCase("Controls_ToastCenter_PointerSelectsAndDismissesRow", ToastCenter_PointerSelectsAndDismissesRow);
        yield return new TestCase("Controls_ToastCenter_StyleHooksAndTimeoutMetadata", ToastCenter_StyleHooksAndTimeoutMetadata);
        yield return new TestCase("Controls_Toolbar_KeyboardNavigationUpdatesSelection", Toolbar_KeyboardNavigationUpdatesSelection);
        yield return new TestCase("Controls_Toolbar_MouseClickSelectsItem", Toolbar_MouseClickSelectsItem);
        yield return new TestCase("Controls_Toolbar_SelectionChangedEvent_ReportsTransition", Toolbar_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_Toolbar_RendersTitleAndSelectedLabel", Toolbar_RendersTitleAndSelectedLabel);
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

    private static Task TreeView_TogglesExpansion()
    {
        var tree = new TreeView
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        tree.SetItems(
        [
            new TreeItem("root", "Root",
            [
                new TreeItem("child", "Child"),
            ]),
        ]);
        var canvas = new Canvas(40, 5);

        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var expanded = canvas.Render();
        TestAssert.True(expanded.Contains("Child", StringComparison.Ordinal), "Tree should render child when expanded.");

        tree.Handle(new KeyPressed(Key.Enter));
        canvas.Clear();
        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var collapsed = canvas.Render();
        TestAssert.True(!collapsed.Contains("Child", StringComparison.Ordinal), "Tree should hide child when collapsed.");
        return Task.CompletedTask;
    }

    private static Task TreeView_MouseClickSelectsVisibleNode()
    {
        var tree = new TreeView
        {
            Border = BorderStyle.None,
        };
        tree.SetItems(
        [
            new TreeItem("root", "Root",
            [
                new TreeItem("child", "Child"),
            ]),
        ]);

        var changed = tree.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 1), new Rect(0, 0, 30, 4));

        TestAssert.True(changed, "Tree click should update selected node.");
        TestAssert.Equal("child", tree.SelectedId ?? string.Empty, "Tree click should select visible row under pointer.");
        return Task.CompletedTask;
    }

    private static Task Notifications_DismissesEntries()
    {
        var center = new Notifications
        {
            IsFocused = true,
        };
        center.Push("hello", NotificationLevel.Info, id: "a");
        center.Push("oops", NotificationLevel.Error, id: "b");

        center.Handle(new KeyPressed(Key.Down));
        center.Handle(new KeyPressed(Key.Character, "d"));

        TestAssert.Equal(1, center.Count, "Notification center should dismiss selected entry.");
        var canvas = new Canvas(48, 4);
        center.Render(canvas, new Rect(0, 0, 48, 4));
        var output = canvas.Render();
        TestAssert.True(output.Contains("hello", StringComparison.Ordinal), "Remaining entry should still render.");
        TestAssert.True(!output.Contains("oops", StringComparison.Ordinal), "Dismissed entry should no longer render.");
        return Task.CompletedTask;
    }

    private static Task Notifications_MouseWheelMovesSelection()
    {
        var center = new Notifications
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        center.Push("first", NotificationLevel.Info, id: "a");
        center.Push("second", NotificationLevel.Info, id: "b");
        center.Push("third", NotificationLevel.Info, id: "c");

        var changed = center.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelUp, 0, 1), new Rect(0, 0, 32, 6));
        center.Handle(new KeyPressed(Key.Character, "d"));

        TestAssert.True(changed, "Notification center wheel should move selected entry.");
        TestAssert.Equal(2, center.Count, "Dismiss should remove wheel-selected entry.");
        var canvas = new Canvas(48, 6);
        center.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();
        TestAssert.True(output.Contains("third", StringComparison.Ordinal), "Newest entry should remain after moving selection up.");
        TestAssert.True(output.Contains("first", StringComparison.Ordinal), "Oldest entry should remain after removing middle entry.");
        TestAssert.True(!output.Contains("second", StringComparison.Ordinal), "Wheel-selected entry should be removed.");
        return Task.CompletedTask;
    }

    private static Task ToastCenter_KeyboardNavigationAndDismiss()
    {
        var center = new ToastCenter
        {
            IsFocused = true,
            Border = BorderStyle.None,
            VisibleCapacity = 3,
            MaxItems = 3,
            AutoDismissExpired = false,
        };
        center.Push("first", NotificationLevel.Info, id: "a", timeout: null);
        center.Push("second", NotificationLevel.Warning, id: "b", timeout: null);
        center.Push("third", NotificationLevel.Error, id: "c", timeout: null);
        center.Push("fourth", NotificationLevel.Success, id: "d", timeout: null);

        TestAssert.Equal(3, center.Count, "Toast center should trim queue to max item count.");
        TestAssert.Equal("b", center.Items[0].Id, "Oldest toast should be dropped when max queue size is reached.");

        center.Handle(new KeyPressed(Key.Up));
        TestAssert.Equal("c", center.SelectedItem?.Id ?? string.Empty, "Up key should move toast selection.");

        var dismissed = center.Handle(new KeyPressed(Key.Delete));
        TestAssert.True(dismissed, "Delete key should dismiss selected toast.");
        TestAssert.Equal(2, center.Count, "Delete should remove one toast.");
        TestAssert.Equal("d", center.SelectedItem?.Id ?? string.Empty, "Selection should remain stable after dismissal.");
        return Task.CompletedTask;
    }

    private static Task ToastCenter_PointerSelectsAndDismissesRow()
    {
        var center = new ToastCenter
        {
            Border = BorderStyle.None,
            AutoDismissExpired = false,
        };
        center.Push("alpha", NotificationLevel.Info, id: "a", timeout: null);
        center.Push("beta", NotificationLevel.Warning, id: "b", timeout: null);
        center.Push("gamma", NotificationLevel.Error, id: "c", timeout: null);

        var bounds = new Rect(0, 0, 40, 4);
        var selected = center.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var dismissed = center.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Right, 1, 1), bounds);

        TestAssert.True(selected, "Pointer left-click should select the hit row.");
        TestAssert.True(dismissed, "Pointer right-click should dismiss the hit row.");
        TestAssert.Equal(2, center.Count, "Pointer dismiss should remove one toast.");
        TestAssert.Equal("c", center.SelectedItem?.Id ?? string.Empty, "Selection should move to nearest remaining toast.");
        return Task.CompletedTask;
    }

    private static Task ToastCenter_StyleHooksAndTimeoutMetadata()
    {
        var center = new ToastCenter
        {
            IsFocused = true,
            Title = "Toasts",
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            AutoDismissExpired = false,
            ItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            SelectedItemStyle = TeaStyle.Empty.WithBold(),
            MutedItemStyle = TeaStyle.Empty.WithDim(),
            WarningItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightYellow),
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
        };
        center.Push("muted", NotificationLevel.Info, id: "m", timeout: null);
        center.SetMuted("m");
        center.Push("expired", NotificationLevel.Info, id: "x", timeout: TimeSpan.Zero);
        center.Push("warning", NotificationLevel.Warning, id: "w", timeout: null);

        var removed = center.DismissExpired(DateTimeOffset.UtcNow);
        TestAssert.Equal(1, removed, "DismissExpired should remove timeout-expired toasts.");

        var canvas = new Canvas(48, 6);
        center.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Toasts !", StringComparison.Ordinal), "Focused title should include focus marker.");
        TestAssert.True(output.Contains("\u001b[4;38;5;13m", StringComparison.Ordinal), "Focused title style should render.");
        TestAssert.True(output.Contains("\u001b[1;38;5;11m", StringComparison.Ordinal), "Selected warning style should render.");
        TestAssert.True(output.Contains("\u001b[2;38;5;14m", StringComparison.Ordinal), "Muted row style should render.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_KeyboardNavigationUpdatesSelection()
    {
        var toolbar = new Toolbar
        {
            IsFocused = true,
        };
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
            new ToolbarItem("save", "Save"),
        ]);

        toolbar.Handle(new KeyPressed(Key.Right));
        toolbar.Handle(new KeyPressed(Key.End));
        var unchangedAtEnd = toolbar.Handle(new KeyPressed(Key.Right));
        toolbar.Handle(new KeyPressed(Key.Home));

        TestAssert.True(!unchangedAtEnd, "Toolbar should clamp navigation at the end.");
        TestAssert.Equal(0, toolbar.SelectedIndex, "Toolbar Home key should move selection back to the first item.");
        TestAssert.Equal("new", toolbar.SelectedItem?.Id ?? string.Empty, "Toolbar should expose selected item after keyboard navigation.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_MouseClickSelectsItem()
    {
        var toolbar = new Toolbar();
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
            new ToolbarItem("save", "Save"),
        ]);

        var changed = toolbar.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 9, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Toolbar click should select the hit item.");
        TestAssert.Equal(1, toolbar.SelectedIndex, "Toolbar click should select the second item from the hit location.");
        TestAssert.Equal("open", toolbar.SelectedItem?.Id ?? string.Empty, "Toolbar click should expose selected item id.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_SelectionChangedEvent_ReportsTransition()
    {
        var toolbar = new Toolbar
        {
            IsFocused = true,
        };
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
            new ToolbarItem("save", "Save"),
        ]);
        ToolbarSelectionChangedEventArgs? args = null;
        toolbar.SelectionChanged += (_, eventArgs) => args = eventArgs;

        toolbar.Handle(new KeyPressed(Key.Right));

        TestAssert.True(args is not null, "Toolbar should raise selection changed when selected item changes.");
        TestAssert.Equal(0, args!.PreviousIndex, "Toolbar event should report previous index.");
        TestAssert.Equal(1, args.SelectedIndex, "Toolbar event should report selected index.");
        TestAssert.Equal("new", args.PreviousItem?.Id ?? string.Empty, "Toolbar event should report previous item.");
        TestAssert.Equal("open", args.SelectedItem?.Id ?? string.Empty, "Toolbar event should report selected item.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_RendersTitleAndSelectedLabel()
    {
        var toolbar = new Toolbar
        {
            IsFocused = true,
            Title = "Main",
        };
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
        ]);

        var canvas = new Canvas(40, 1);
        toolbar.Render(canvas, new Rect(0, 0, 40, 1));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Main *", StringComparison.Ordinal), "Toolbar should render focused title marker.");
        TestAssert.True(output.Contains("[New]", StringComparison.Ordinal), "Toolbar should render selected item with a bracket marker.");
        return Task.CompletedTask;
    }
}
