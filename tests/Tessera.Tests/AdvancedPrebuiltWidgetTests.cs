using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class AdvancedPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Controls_Badge_RendersLabel", Badge_RendersLabel);
        yield return new TestCase("Controls_Toggle_TogglesValue", Toggle_TogglesValue);
        yield return new TestCase("Controls_Toggle_MouseClickTogglesValue", Toggle_MouseClickTogglesValue);
        yield return new TestCase("Controls_Toggle_FocusedBorderStyleText_StylesFrameGlyphs",
            Toggle_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Slider_AdjustsValue", Slider_AdjustsValue);
        yield return new TestCase("Controls_Slider_MouseClickSetsValue", Slider_MouseClickSetsValue);
        yield return new TestCase("Controls_Slider_DragUpdatesValue", Slider_DragUpdatesValue);
        yield return new TestCase("Controls_Slider_FocusedBorderStyleText_StylesFrameGlyphs",
            Slider_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Spinner_AdvancesFrame", Spinner_AdvancesFrame);
        yield return new TestCase("Controls_Spinner_SetFrames_SwapsFamiliesDuringRun",
            Spinner_SetFrames_SwapsFamiliesDuringRun);
        yield return new TestCase("Controls_Spinner_SetFrames_RejectsEmptyFamilies",
            Spinner_SetFrames_RejectsEmptyFamilies);
        yield return new TestCase("Controls_Spinner_MouseClickTogglesRunning", Spinner_MouseClickTogglesRunning);
        yield return new TestCase("Controls_Spinner_MouseWheelAdvancesFrame", Spinner_MouseWheelAdvancesFrame);
        yield return new TestCase("Controls_Spinner_FocusedBorderStyleText_StylesFrameGlyphs",
            Spinner_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Toggle_MouseWheelSetsValue", Toggle_MouseWheelSetsValue);
        yield return new TestCase("Controls_TreeView_TogglesExpansion", TreeView_TogglesExpansion);
        yield return new TestCase("Controls_TreeView_MouseClickSelectsVisibleNode",
            TreeView_MouseClickSelectsVisibleNode);
        yield return new TestCase("Controls_TreeView_CustomGlyphSet_RendersCustomMarkers",
            TreeView_CustomGlyphSet_RendersCustomMarkers);
        yield return new TestCase("Controls_TreeView_FocusedBorderStyleText_StylesFrameGlyphs",
            TreeView_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Notifications_DismissesEntries", Notifications_DismissesEntries);
        yield return new TestCase("Controls_Notifications_MouseWheelMovesSelection",
            Notifications_MouseWheelMovesSelection);
        yield return new TestCase("Controls_ToastCenter_KeyboardNavigationAndDismiss",
            ToastCenter_KeyboardNavigationAndDismiss);
        yield return new TestCase("Controls_ToastCenter_PointerSelectsAndDismissesRow",
            ToastCenter_PointerSelectsAndDismissesRow);
        yield return new TestCase("Controls_ToastCenter_StyleHooksAndTimeoutMetadata",
            ToastCenter_StyleHooksAndTimeoutMetadata);
        yield return new TestCase("Controls_Toolbar_KeyboardNavigationUpdatesSelection",
            Toolbar_KeyboardNavigationUpdatesSelection);
        yield return new TestCase("Controls_Toolbar_MouseClickSelectsItem", Toolbar_MouseClickSelectsItem);
        yield return new TestCase("Controls_Toolbar_SelectionChangedEvent_ReportsTransition",
            Toolbar_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_Toolbar_RendersTitleAndSelectedLabel",
            Toolbar_RendersTitleAndSelectedLabel);
        yield return new TestCase("Controls_TreeTable_KeyboardNavigationAndExpansion",
            TreeTable_KeyboardNavigationAndExpansion);
        yield return new TestCase("Controls_TreeTable_PointerClickSelectsVisibleRow",
            TreeTable_PointerClickSelectsVisibleRow);
        yield return new TestCase("Controls_TreeTable_SelectionChangedEvent_ReportsTransition",
            TreeTable_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_TreeTable_RendersHeadersRowsAndStyles",
            TreeTable_RendersHeadersRowsAndStyles);
        yield return new TestCase("Controls_Timeline_KeyboardNavigationTracksSelection",
            Timeline_KeyboardNavigationTracksSelection);
        yield return new TestCase("Controls_Timeline_PointerClickSelectsRow", Timeline_PointerClickSelectsRow);
        yield return new TestCase("Controls_Timeline_SelectionChangedEvent_ReportsTransition",
            Timeline_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_Timeline_RendersTitleAndStyleHooks", Timeline_RendersTitleAndStyleHooks);
    }

    private static Task Badge_RendersLabel()
    {
        var badge = new Badge { Text = "hot", Tone = BadgeTone.Warning };
        var canvas = new Canvas(20, 1);

        badge.Render(canvas, new Rect(0, 0, 20, 1));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[hot]", StringComparison.Ordinal), "Badge should render bracketed text.");
        return Task.CompletedTask;
    }

    private static Task Toggle_TogglesValue()
    {
        var toggle = new Toggle { IsFocused = true };

        toggle.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(toggle.Value, "Toggle should flip to on after enter.");
        toggle.Handle(new KeyPressed(Key.Left));
        TestAssert.True(!toggle.Value, "Toggle should flip to off after left.");
        return Task.CompletedTask;
    }

    private static Task Toggle_MouseClickTogglesValue()
    {
        var toggle = new Toggle { Border = BorderStyle.None };

        var changed = toggle.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0),
            new Rect(0, 0, 10, 1));

        TestAssert.True(changed, "Toggle mouse click should report state change.");
        TestAssert.True(toggle.Value, "Toggle mouse click should enable value.");
        return Task.CompletedTask;
    }

    private static Task Toggle_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(102, 65, 122));
        var toggle = new Toggle
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = string.Empty,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(17, 18, 19)),
            FocusedBorderStyleText = focusedBorderStyle
        };
        var canvas = new Canvas(24, 4, CanvasTextMode.GraphemeAware);

        toggle.Render(canvas, new Rect(0, 0, 24, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "Toggle should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task Slider_AdjustsValue()
    {
        var slider = new Slider { IsFocused = true, Min = 0, Max = 10, Step = 2 };

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
        var slider = new Slider { Border = BorderStyle.None, Min = 0, Max = 10, Step = 1 };

        var changed = slider.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 19, 1),
            new Rect(0, 0, 20, 2));

        TestAssert.True(changed, "Slider mouse click should update slider value.");
        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider click at far-right should move value to max.");
        return Task.CompletedTask;
    }

    private static Task Slider_DragUpdatesValue()
    {
        var slider = new Slider { Border = BorderStyle.None, Min = 0, Max = 10, Step = 1 };

        var bounds = new Rect(0, 0, 20, 2);
        slider.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var changed = slider.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.Left, 19, 1), bounds);
        slider.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, 19, 1), bounds);

        TestAssert.True(changed, "Slider drag should update slider value.");
        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider drag to far-right should move value to max.");
        return Task.CompletedTask;
    }

    private static Task Slider_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(49, 111, 124));
        var slider = new Slider
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = string.Empty,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 20, 20)),
            FocusedBorderStyleText = focusedBorderStyle
        };
        var canvas = new Canvas(28, 5, CanvasTextMode.GraphemeAware);

        slider.Render(canvas, new Rect(0, 0, 28, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "Slider should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task Spinner_AdvancesFrame()
    {
        var spinner = new Spinner { IsFocused = true };
        var canvasBefore = new Canvas(20, 3);
        spinner.Render(canvasBefore, new Rect(0, 0, 20, 3));
        var before = canvasBefore.Render();

        spinner.Handle(new KeyPressed(Key.Right));
        var canvasAfter = new Canvas(20, 3);
        spinner.Render(canvasAfter, new Rect(0, 0, 20, 3));
        var after = canvasAfter.Render();

        TestAssert.True(!string.Equals(before, after, StringComparison.Ordinal),
            "Spinner should advance when running.");
        spinner.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(!spinner.Running, "Spinner should stop when toggled.");
        return Task.CompletedTask;
    }

    private static Task Spinner_SetFrames_SwapsFamiliesDuringRun()
    {
        var spinner = new Spinner { Border = BorderStyle.None, Label = "syncing", IsFocused = true };

        spinner.Advance();
        spinner.SetFrames(["⠁", "⠂", "⠄"]);

        var swappedCanvas = new Canvas(24, 1, CanvasTextMode.GraphemeAware);
        spinner.Render(swappedCanvas, new Rect(0, 0, 24, 1));
        var swapped = swappedCanvas.Render();

        spinner.Advance();
        var advancedCanvas = new Canvas(24, 1, CanvasTextMode.GraphemeAware);
        spinner.Render(advancedCanvas, new Rect(0, 0, 24, 1));
        var advanced = advancedCanvas.Render();

        TestAssert.Equal(3, spinner.Frames.Count, "Spinner should expose the replaced frame family.");
        TestAssert.True(swapped.Contains("⠂ syncing", StringComparison.Ordinal),
            "Spinner should render the swapped family at the current animation index.");
        TestAssert.True(advanced.Contains("⠄ syncing", StringComparison.Ordinal),
            "Spinner should continue advancing within the swapped family.");
        return Task.CompletedTask;
    }

    private static Task Spinner_SetFrames_RejectsEmptyFamilies()
    {
        var spinner = new Spinner();

        _ = Assert.Throws<ArgumentException>(() => spinner.SetFrames([]),
            "Spinner should reject empty frame families.");
        _ = Assert.Throws<ArgumentException>(() => spinner.SetFrames(["ok", string.Empty]),
            "Spinner should reject empty frame entries.");
        return Task.CompletedTask;
    }

    private static Task Spinner_MouseClickTogglesRunning()
    {
        var spinner = new Spinner { Border = BorderStyle.None };

        var changed = spinner.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0),
            new Rect(0, 0, 16, 1));

        TestAssert.True(changed, "Spinner click should toggle running state.");
        TestAssert.True(!spinner.Running, "Spinner click should stop the spinner.");
        return Task.CompletedTask;
    }

    private static Task Spinner_MouseWheelAdvancesFrame()
    {
        var spinner = new Spinner { Border = BorderStyle.None };
        var beforeCanvas = new Canvas(16, 1);
        spinner.Render(beforeCanvas, new Rect(0, 0, 16, 1));
        var before = beforeCanvas.Render();

        var changed = spinner.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, 0, 0),
            new Rect(0, 0, 16, 1));
        var afterCanvas = new Canvas(16, 1);
        spinner.Render(afterCanvas, new Rect(0, 0, 16, 1));
        var after = afterCanvas.Render();

        TestAssert.True(changed, "Spinner wheel should advance frame while running.");
        TestAssert.True(!string.Equals(before, after, StringComparison.Ordinal),
            "Spinner wheel should move frame index.");
        return Task.CompletedTask;
    }

    private static Task Spinner_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(130, 84, 46));
        var spinner = new Spinner
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = string.Empty,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(22, 22, 22)),
            FocusedBorderStyleText = focusedBorderStyle
        };
        var canvas = new Canvas(24, 4, CanvasTextMode.GraphemeAware);

        spinner.Render(canvas, new Rect(0, 0, 24, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "Spinner should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task Toggle_MouseWheelSetsValue()
    {
        var toggle = new Toggle { Border = BorderStyle.None };

        var changedOn = toggle.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelUp, 0, 0),
            new Rect(0, 0, 10, 1));
        var changedOff = toggle.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, 0, 0),
            new Rect(0, 0, 10, 1));

        TestAssert.True(changedOn, "Toggle wheel-up should change value.");
        TestAssert.True(changedOff, "Toggle wheel-down should change value.");
        TestAssert.True(!toggle.Value, "Toggle wheel-down should disable value.");
        return Task.CompletedTask;
    }

    private static Task TreeView_TogglesExpansion()
    {
        var tree = new TreeView { IsFocused = true, Border = BorderStyle.None };
        tree.SetItems(
        [
            new TreeItem("root", "Root",
            [
                new TreeItem("child", "Child")
            ])
        ]);
        var canvas = new Canvas(40, 5);

        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var expanded = canvas.Render();
        TestAssert.True(expanded.Contains("Child", StringComparison.Ordinal),
            "Tree should render child when expanded.");

        tree.Handle(new KeyPressed(Key.Enter));
        canvas.Clear();
        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var collapsed = canvas.Render();
        TestAssert.True(!collapsed.Contains("Child", StringComparison.Ordinal),
            "Tree should hide child when collapsed.");
        return Task.CompletedTask;
    }

    private static Task TreeView_MouseClickSelectsVisibleNode()
    {
        var tree = new TreeView { Border = BorderStyle.None };
        tree.SetItems(
        [
            new TreeItem("root", "Root",
            [
                new TreeItem("child", "Child")
            ])
        ]);

        var changed = tree.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 1),
            new Rect(0, 0, 30, 4));

        TestAssert.True(changed, "Tree click should update selected node.");
        TestAssert.Equal("child", tree.SelectedId ?? string.Empty,
            "Tree click should select visible row under pointer.");
        return Task.CompletedTask;
    }

    private static Task TreeView_CustomGlyphSet_RendersCustomMarkers()
    {
        var tree = new TreeView
        {
            IsFocused = true,
            Border = BorderStyle.None,
            Glyphs = new TreeViewGlyphSet("v", ">", "*")
        };
        tree.SetItems(
        [
            new TreeItem("root", "Root",
            [
                new TreeItem("child", "Child")
            ])
        ]);
        var canvas = new Canvas(40, 5);

        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var expanded = canvas.Render();
        TestAssert.True(expanded.Contains("v Root", StringComparison.Ordinal),
            "Tree should render custom expanded branch marker.");
        TestAssert.True(expanded.Contains("* Child", StringComparison.Ordinal),
            "Tree should render custom leaf marker.");

        tree.Handle(new KeyPressed(Key.Enter));
        canvas.Clear();
        tree.Render(canvas, new Rect(0, 0, 40, 5));
        var collapsed = canvas.Render();
        TestAssert.True(collapsed.Contains("> > Root", StringComparison.Ordinal),
            "Tree should render custom collapsed branch marker.");
        return Task.CompletedTask;
    }

    private static Task TreeView_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(91, 52, 33));
        var tree = new TreeView
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = string.Empty,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            FocusedBorderStyleText = focusedBorderStyle
        };
        tree.SetItems([new TreeItem("root", "Root")]);
        var canvas = new Canvas(24, 5, CanvasTextMode.GraphemeAware);

        tree.Render(canvas, new Rect(0, 0, 24, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "TreeView should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task Notifications_DismissesEntries()
    {
        var center = new Notifications { IsFocused = true };
        center.Push("hello", NotificationLevel.Info, "a");
        center.Push("oops", NotificationLevel.Error, "b");

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
        var center = new Notifications { IsFocused = true, Border = BorderStyle.None };
        center.Push("first", NotificationLevel.Info, "a");
        center.Push("second", NotificationLevel.Info, "b");
        center.Push("third", NotificationLevel.Info, "c");

        var changed = center.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelUp, 0, 1),
            new Rect(0, 0, 32, 6));
        center.Handle(new KeyPressed(Key.Character, "d"));

        TestAssert.True(changed, "Notification center wheel should move selected entry.");
        TestAssert.Equal(2, center.Count, "Dismiss should remove wheel-selected entry.");
        var canvas = new Canvas(48, 6);
        center.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();
        TestAssert.True(output.Contains("third", StringComparison.Ordinal),
            "Newest entry should remain after moving selection up.");
        TestAssert.True(output.Contains("first", StringComparison.Ordinal),
            "Oldest entry should remain after removing middle entry.");
        TestAssert.True(!output.Contains("second", StringComparison.Ordinal),
            "Wheel-selected entry should be removed.");
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
            AutoDismissExpired = false
        };
        center.Push("first", NotificationLevel.Info, "a");
        center.Push("second", NotificationLevel.Warning, "b");
        center.Push("third", NotificationLevel.Error, "c");
        center.Push("fourth", NotificationLevel.Success, "d");

        TestAssert.Equal(3, center.Count, "Toast center should trim queue to max item count.");
        TestAssert.Equal("b", center.Items[0].Id, "Oldest toast should be dropped when max queue size is reached.");

        center.Handle(new KeyPressed(Key.Up));
        TestAssert.Equal("c", center.SelectedItem?.Id ?? string.Empty, "Up key should move toast selection.");

        var dismissed = center.Handle(new KeyPressed(Key.Delete));
        TestAssert.True(dismissed, "Delete key should dismiss selected toast.");
        TestAssert.Equal(2, center.Count, "Delete should remove one toast.");
        TestAssert.Equal("d", center.SelectedItem?.Id ?? string.Empty,
            "Selection should remain stable after dismissal.");
        return Task.CompletedTask;
    }

    private static Task ToastCenter_PointerSelectsAndDismissesRow()
    {
        var center = new ToastCenter { Border = BorderStyle.None, AutoDismissExpired = false };
        center.Push("alpha", NotificationLevel.Info, "a");
        center.Push("beta", NotificationLevel.Warning, "b");
        center.Push("gamma", NotificationLevel.Error, "c");

        var bounds = new Rect(0, 0, 40, 4);
        var selected = center.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var dismissed = center.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Right, 1, 1), bounds);

        TestAssert.True(selected, "Pointer left-click should select the hit row.");
        TestAssert.True(dismissed, "Pointer right-click should dismiss the hit row.");
        TestAssert.Equal(2, center.Count, "Pointer dismiss should remove one toast.");
        TestAssert.Equal("c", center.SelectedItem?.Id ?? string.Empty,
            "Selection should move to nearest remaining toast.");
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
            ItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            SelectedItemStyle = TesseraStyle.Empty.WithBold(),
            MutedItemStyle = TesseraStyle.Empty.WithDim(),
            WarningItemStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightYellow),
            FocusedTitleStyle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta)
        };
        center.Push("muted", NotificationLevel.Info, "m");
        center.SetMuted("m");
        center.Push("expired", NotificationLevel.Info, "x", TimeSpan.Zero);
        center.Push("warning", NotificationLevel.Warning, "w");

        var removed = center.DismissExpired(DateTimeOffset.UtcNow);
        TestAssert.Equal(1, removed, "DismissExpired should remove timeout-expired toasts.");

        var canvas = new Canvas(48, 6);
        center.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Toasts !", StringComparison.Ordinal),
            "Focused title should include focus marker.");
        TestAssert.True(output.Contains("\e[4;38;5;13m", StringComparison.Ordinal),
            "Focused title style should render.");
        TestAssert.True(output.Contains("\e[1;38;5;11m", StringComparison.Ordinal),
            "Selected warning style should render.");
        TestAssert.True(output.Contains("\e[2;38;5;14m", StringComparison.Ordinal),
            "Muted row style should render.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_KeyboardNavigationUpdatesSelection()
    {
        var toolbar = new Toolbar { IsFocused = true };
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
            new ToolbarItem("save", "Save")
        ]);

        toolbar.Handle(new KeyPressed(Key.Right));
        toolbar.Handle(new KeyPressed(Key.End));
        var unchangedAtEnd = toolbar.Handle(new KeyPressed(Key.Right));
        toolbar.Handle(new KeyPressed(Key.Home));

        TestAssert.True(!unchangedAtEnd, "Toolbar should clamp navigation at the end.");
        TestAssert.Equal(0, toolbar.SelectedIndex, "Toolbar Home key should move selection back to the first item.");
        TestAssert.Equal("new", toolbar.SelectedItem?.Id ?? string.Empty,
            "Toolbar should expose selected item after keyboard navigation.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_MouseClickSelectsItem()
    {
        var toolbar = new Toolbar();
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
            new ToolbarItem("save", "Save")
        ]);

        var changed = toolbar.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 9, 0),
            new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Toolbar click should select the hit item.");
        TestAssert.Equal(1, toolbar.SelectedIndex,
            "Toolbar click should select the second item from the hit location.");
        TestAssert.Equal("open", toolbar.SelectedItem?.Id ?? string.Empty,
            "Toolbar click should expose selected item id.");
        return Task.CompletedTask;
    }

    private static Task Toolbar_SelectionChangedEvent_ReportsTransition()
    {
        var toolbar = new Toolbar { IsFocused = true };
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open"),
            new ToolbarItem("save", "Save")
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
        var toolbar = new Toolbar { IsFocused = true, Title = "Main" };
        toolbar.SetItems(
        [
            new ToolbarItem("new", "New"),
            new ToolbarItem("open", "Open")
        ]);

        var canvas = new Canvas(40, 1);
        toolbar.Render(canvas, new Rect(0, 0, 40, 1));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Main *", StringComparison.Ordinal),
            "Toolbar should render focused title marker.");
        TestAssert.True(output.Contains("[New]", StringComparison.Ordinal),
            "Toolbar should render selected item with a bracket marker.");
        return Task.CompletedTask;
    }

    private static Task TreeTable_KeyboardNavigationAndExpansion()
    {
        var src = new TreeTableNode("src", "src", ["dir", "folder"],
        [
            new TreeTableNode("core", "Tessera.Core.csproj", ["12 KB", "file"])
        ])
        { IsExpanded = false };

        var table = new TreeTable("Name", "Size", "Kind") { IsFocused = true, Border = BorderStyle.None };
        table.SetItems(
        [
            src,
            new TreeTableNode("readme", "README.md", ["4 KB", "file"])
        ]);

        var expanded = table.Handle(new KeyPressed(Key.Right));
        TestAssert.True(expanded, "TreeTable Right key should expand selected collapsed branch.");

        var intoChild = table.Handle(new KeyPressed(Key.Right));
        TestAssert.True(intoChild, "TreeTable Right key should move into first child when branch is expanded.");
        TestAssert.Equal("core", table.SelectedItem?.Id ?? string.Empty,
            "TreeTable should select first child after moving into branch.");

        var backToParent = table.Handle(new KeyPressed(Key.Left));
        TestAssert.True(backToParent, "TreeTable Left key should move selection to parent row.");
        TestAssert.Equal("src", table.SelectedItem?.Id ?? string.Empty,
            "TreeTable should select parent row after Left.");

        var collapsed = table.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(collapsed, "TreeTable Enter should collapse selected branch.");
        TestAssert.True(!(table.SelectedItem?.IsExpanded ?? true),
            "TreeTable selected branch should be collapsed after Enter.");

        var down = table.Handle(new KeyPressed(Key.Down));
        TestAssert.True(down, "TreeTable Down key should move selection to next visible row.");
        TestAssert.Equal("readme", table.SelectedItem?.Id ?? string.Empty,
            "TreeTable Down should skip collapsed children.");

        var up = table.Handle(new KeyPressed(Key.Up));
        TestAssert.True(up, "TreeTable Up key should move selection back to previous row.");
        TestAssert.Equal("src", table.SelectedItem?.Id ?? string.Empty,
            "TreeTable Up should return selection to branch row.");

        var expandedAgain = table.Handle(new KeyPressed(Key.Right));
        TestAssert.True(expandedAgain, "TreeTable Right key should expand collapsed branch.");
        TestAssert.True(table.SelectedItem?.IsExpanded ?? false,
            "TreeTable selected branch should be expanded after Right.");
        return Task.CompletedTask;
    }

    private static Task TreeTable_PointerClickSelectsVisibleRow()
    {
        var table = new TreeTable("Name", "Size", "Kind") { Border = BorderStyle.None };
        table.SetItems(
        [
            new TreeTableNode("src", "src", ["dir", "folder"],
            [
                new TreeTableNode("core", "Tessera.Core.csproj", ["12 KB", "file"])
            ]),
            new TreeTableNode("readme", "README.md", ["4 KB", "file"])
        ]);

        var changed = table.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 3),
            new Rect(0, 0, 64, 8));

        TestAssert.True(changed, "TreeTable pointer press should select clicked row.");
        TestAssert.Equal("readme", table.SelectedItem?.Id ?? string.Empty,
            "TreeTable pointer press should map row to selected item.");
        return Task.CompletedTask;
    }

    private static Task TreeTable_SelectionChangedEvent_ReportsTransition()
    {
        var table = new TreeTable("Name", "Value") { IsFocused = true, Border = BorderStyle.None };
        table.SetItems(
        [
            new TreeTableNode("a", "Alpha", ["1"]),
            new TreeTableNode("b", "Beta", ["2"])
        ]);
        TreeTableSelectionChangedEventArgs? args = null;
        table.SelectionChanged += (_, eventArgs) => args = eventArgs;

        table.Handle(new KeyPressed(Key.Down));

        TestAssert.True(args is not null, "TreeTable should raise selection-changed event when selection moves.");
        TestAssert.Equal(0, args!.PreviousIndex, "TreeTable event should expose previous selected index.");
        TestAssert.Equal(1, args.SelectedIndex, "TreeTable event should expose current selected index.");
        TestAssert.Equal("a", args.PreviousItem?.Id ?? string.Empty,
            "TreeTable event should expose previous selected item.");
        TestAssert.Equal("b", args.SelectedItem?.Id ?? string.Empty,
            "TreeTable event should expose current selected item.");
        return Task.CompletedTask;
    }

    private static Task TreeTable_RendersHeadersRowsAndStyles()
    {
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightBlue);
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var mergedBorderStyle = borderStyle.Merge(focusedBorderStyle);
        var table = new TreeTable("Name", "Size", "Kind")
        {
            Border = BorderStyle.SingleLine,
            IsFocused = true,
            FocusMarker = "!",
            HeaderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            BranchRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            LeafRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            SelectedRowStyle = TesseraStyle.Empty.WithBold(),
            MutedRowStyle = TesseraStyle.Empty.WithDim(),
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
            ColumnSeparatorText = " • ",
            SelectedRowMarker = ">>",
            UnselectedRowMarker = "..",
            ExpandedBranchMarker = "v",
            CollapsedBranchMarker = ">",
            LeafMarker = "*"
        };
        table.SetItems(
        [
            new TreeTableNode("src", "src", ["dir", "folder"],
            [
                new TreeTableNode("program", "Program.cs", ["12 KB", "file"])
            ]),
            new TreeTableNode("readme", "README.md", ["4 KB", "file"])
        ]);

        table.Handle(new KeyPressed(Key.Down));

        var canvas = new Canvas(80, 8, CanvasTextMode.GraphemeAware);

        table.Render(canvas, new Rect(0, 0, 80, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Tree Table !", StringComparison.Ordinal),
            "TreeTable should render focused title marker.");
        TestAssert.True(output.Contains("Name • Size • Kind", StringComparison.Ordinal),
            "TreeTable should render custom header separator.");
        TestAssert.True(output.Contains(">>   * Program.cs • 12 KB • file", StringComparison.Ordinal),
            "TreeTable should render selected row and custom leaf marker.");
        TestAssert.True(output.Contains(".. v src • dir • folder", StringComparison.Ordinal),
            "TreeTable should render custom expanded branch marker.");
        TestAssert.True(output.Contains(".. * README.md • 4 KB • file", StringComparison.Ordinal),
            "TreeTable should render additional leaf rows with custom marker.");
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "TreeTable should style focused border glyphs.");
        TestAssert.True(output.Contains("\e[38;5;12m", StringComparison.Ordinal),
            "TreeTable header style should render ANSI color.");
        TestAssert.True(output.Contains("\e[38;5;14m", StringComparison.Ordinal),
            "TreeTable branch row style should render ANSI color.");
        TestAssert.True(output.Contains("\e[38;5;10m", StringComparison.Ordinal),
            "TreeTable leaf row style should render ANSI color.");

        table.IsDisabled = true;
        canvas.Clear();
        table.Render(canvas, new Rect(0, 0, 80, 8));
        output = canvas.Render();
        TestAssert.True(output.Contains("\e[2;", StringComparison.Ordinal),
            "TreeTable disabled rows should include muted styling.");
        return Task.CompletedTask;
    }

    private static Task Timeline_KeyboardNavigationTracksSelection()
    {
        var timeline = new Timeline { IsFocused = true, Border = BorderStyle.None };
        timeline.SetEntries(
        [
            new TimelineEntry("a", "Started", "09:00"),
            new TimelineEntry("b", "Queued", "09:05"),
            new TimelineEntry("c", "Running", "09:10"),
            new TimelineEntry("d", "Done", "09:15")
        ]);

        timeline.Handle(new KeyPressed(Key.Down));
        timeline.Handle(new KeyPressed(Key.PageDown));
        timeline.Handle(new KeyPressed(Key.End));
        timeline.Handle(new KeyPressed(Key.Up));
        timeline.Handle(new KeyPressed(Key.Home));

        TestAssert.Equal("a", timeline.SelectedItem?.Id ?? string.Empty,
            "Timeline Home key should move selection to first entry.");
        TestAssert.True(timeline.Select(3), "Timeline Select should allow direct selection.");
        TestAssert.Equal("d", timeline.SelectedItem?.Id ?? string.Empty,
            "Timeline Select should update selected entry.");
        return Task.CompletedTask;
    }

    private static Task Timeline_PointerClickSelectsRow()
    {
        var timeline = new Timeline { Border = BorderStyle.None };
        timeline.SetEntries(
        [
            new TimelineEntry("a", "Started", "09:00"),
            new TimelineEntry("b", "Queued", "09:05"),
            new TimelineEntry("c", "Running", "09:10")
        ]);

        var changed = timeline.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1),
            new Rect(0, 0, 40, 4));

        TestAssert.True(changed, "Timeline pointer press should select clicked row.");
        TestAssert.Equal("b", timeline.SelectedItem?.Id ?? string.Empty,
            "Timeline pointer selection should map row to timeline entry.");
        return Task.CompletedTask;
    }

    private static Task Timeline_SelectionChangedEvent_ReportsTransition()
    {
        var timeline = new Timeline { IsFocused = true, Border = BorderStyle.None };
        timeline.SetEntries(
        [
            new TimelineEntry("a", "Started", "09:00"),
            new TimelineEntry("b", "Queued", "09:05")
        ]);
        TimelineSelectionChangedEventArgs? args = null;
        timeline.SelectionChanged += (_, eventArgs) => args = eventArgs;

        timeline.Handle(new KeyPressed(Key.Down));

        TestAssert.True(args is not null, "Timeline should raise selection changed when selection moves.");
        TestAssert.Equal(0, args!.PreviousIndex, "Timeline event should expose previous index.");
        TestAssert.Equal(1, args.SelectedIndex, "Timeline event should expose selected index.");
        TestAssert.Equal("a", args.PreviousItem?.Id ?? string.Empty, "Timeline event should expose previous item.");
        TestAssert.Equal("b", args.SelectedItem?.Id ?? string.Empty, "Timeline event should expose selected item.");
        return Task.CompletedTask;
    }

    private static Task Timeline_RendersTitleAndStyleHooks()
    {
        var timeline = new Timeline
        {
            Title = "Timeline",
            IsFocused = true,
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            FocusedTitleStyle = TesseraStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            TimestampStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            LabelStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            ContentStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            SeparatorStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightYellow),
            SelectedRowStyle = TesseraStyle.Empty.WithBold(),
            MutedStyle = TesseraStyle.Empty.WithDim()
        };
        timeline.SetEntries(
        [
            new TimelineEntry("a", "Started", "09:00", status: "ok", isMuted: true),
            new TimelineEntry("b", "Running", "09:05", "Worker A")
        ]);
        timeline.Select(1);
        var canvas = new Canvas(160, 6);

        timeline.Render(canvas, new Rect(0, 0, 160, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Timeline !", StringComparison.Ordinal),
            "Timeline should render focused title marker.");
        TestAssert.True(output.Contains("\e[4;38;5;13m", StringComparison.Ordinal),
            "Timeline should render focused title style.");
        var hasTimestampColor = output.Contains("\e[38;5;12m", StringComparison.Ordinal)
                                || output.Contains(";5;12m", StringComparison.Ordinal)
                                || output.Contains("\e[94m", StringComparison.Ordinal);
        TestAssert.True(hasTimestampColor, "Timeline should render timestamp style.");
        var hasSeparatorColor = output.Contains("\e[38;5;11m", StringComparison.Ordinal)
                                || output.Contains(";5;11m", StringComparison.Ordinal)
                                || output.Contains("\e[93m", StringComparison.Ordinal)
                                || output.Contains("\e[33m", StringComparison.Ordinal);
        TestAssert.True(hasSeparatorColor, "Timeline should render separator/status style.");
        TestAssert.True(output.Contains("Worker A", StringComparison.Ordinal),
            "Timeline should render entry content text.");
        TestAssert.True(output.Contains("\e[2;", StringComparison.Ordinal),
            "Timeline muted rows should include dim style.");
        return Task.CompletedTask;
    }
}
