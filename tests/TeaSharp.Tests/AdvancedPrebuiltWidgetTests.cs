using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class AdvancedPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Advanced_BadgeComponent_RendersLabel", BadgeComponent_RendersLabel);
        yield return new TestCase("Advanced_ToggleSwitchComponent_TogglesValue", ToggleSwitchComponent_TogglesValue);
        yield return new TestCase("Advanced_ToggleSwitchComponent_MouseClickTogglesValue", ToggleSwitchComponent_MouseClickTogglesValue);
        yield return new TestCase("Advanced_SliderComponent_AdjustsValue", SliderComponent_AdjustsValue);
        yield return new TestCase("Advanced_SliderComponent_MouseClickSetsValue", SliderComponent_MouseClickSetsValue);
        yield return new TestCase("Advanced_SpinnerComponent_AdvancesFrame", SpinnerComponent_AdvancesFrame);
        yield return new TestCase("Advanced_SpinnerComponent_MouseWheelAdvancesFrame", SpinnerComponent_MouseWheelAdvancesFrame);
        yield return new TestCase("Advanced_CommandPaletteComponent_FiltersAndExecutes", CommandPaletteComponent_FiltersAndExecutes);
        yield return new TestCase("Advanced_CommandPaletteComponent_TryConsumeExecution_IsSingleUse", CommandPaletteComponent_TryConsumeExecution_IsSingleUse);
        yield return new TestCase("Advanced_CommandPaletteComponent_MouseClickExecutesSelection", CommandPaletteComponent_MouseClickExecutesSelection);
        yield return new TestCase("Advanced_CommandPaletteComponent_ExposesQueryAccessors", CommandPaletteComponent_ExposesQueryAccessors);
        yield return new TestCase("Advanced_TreeViewComponent_TogglesExpansion", TreeViewComponent_TogglesExpansion);
        yield return new TestCase("Advanced_TreeViewComponent_MouseClickSelectsVisibleNode", TreeViewComponent_MouseClickSelectsVisibleNode);
        yield return new TestCase("Advanced_NotificationCenterComponent_DismissesEntries", NotificationCenterComponent_DismissesEntries);
        yield return new TestCase("Advanced_NotificationCenterComponent_MouseWheelMovesSelection", NotificationCenterComponent_MouseWheelMovesSelection);
    }

    private static Task BadgeComponent_RendersLabel()
    {
        var badge = new BadgeComponent
        {
            Text = "hot",
            State = WidgetVisualState.Warning,
        };
        var canvas = new Canvas(20, 1);

        badge.Render(canvas, new Rect(0, 0, 20, 1));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[hot]", StringComparison.Ordinal), "Badge should render bracketed text.");
        return Task.CompletedTask;
    }

    private static Task ToggleSwitchComponent_TogglesValue()
    {
        var toggle = new ToggleSwitchComponent
        {
            Focused = true,
        };

        toggle.Update(new KeyPressMsg(KeyCode.Enter));
        TestAssert.True(toggle.Value, "Toggle should flip to on after enter.");
        toggle.Update(new KeyPressMsg(KeyCode.Left));
        TestAssert.True(!toggle.Value, "Toggle should flip to off after left.");
        return Task.CompletedTask;
    }

    private static Task ToggleSwitchComponent_MouseClickTogglesValue()
    {
        var toggle = new ToggleSwitchComponent
        {
            Border = BorderStyle.None,
        };

        var changed = toggle.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 0), new Rect(0, 0, 10, 1));

        TestAssert.True(changed, "Toggle mouse click should report state change.");
        TestAssert.True(toggle.Value, "Toggle mouse click should enable value.");
        return Task.CompletedTask;
    }

    private static Task SliderComponent_AdjustsValue()
    {
        var slider = new SliderComponent
        {
            Focused = true,
            Min = 0,
            Max = 10,
            Step = 2,
        };

        slider.SetValue(4);
        slider.Update(new KeyPressMsg(KeyCode.Right));
        slider.Update(new KeyPressMsg(KeyCode.Right));
        slider.Update(new KeyPressMsg(KeyCode.Right));

        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider should clamp at max.");
        slider.Update(new KeyPressMsg(KeyCode.Left));
        TestAssert.True(Math.Abs(slider.Value - 8) < 0.0001, "Slider should decrement by step.");
        return Task.CompletedTask;
    }

    private static Task SliderComponent_MouseClickSetsValue()
    {
        var slider = new SliderComponent
        {
            Border = BorderStyle.None,
            Min = 0,
            Max = 10,
            Step = 1,
        };

        var changed = slider.UpdateMouse(new MouseClickMsg(MouseButton.Left, 19, 1), new Rect(0, 0, 20, 2));

        TestAssert.True(changed, "Slider mouse click should update slider value.");
        TestAssert.True(Math.Abs(slider.Value - 10) < 0.0001, "Slider click at far-right should move value to max.");
        return Task.CompletedTask;
    }

    private static Task SpinnerComponent_AdvancesFrame()
    {
        var spinner = new SpinnerComponent
        {
            Focused = true,
        };

        var before = spinner.FrameIndex;
        spinner.Update(new KeyPressMsg(KeyCode.Right));
        TestAssert.True(spinner.FrameIndex != before, "Spinner should advance when running.");
        spinner.Update(new KeyPressMsg(KeyCode.Enter));
        TestAssert.True(!spinner.Running, "Spinner should stop when toggled.");
        return Task.CompletedTask;
    }

    private static Task SpinnerComponent_MouseWheelAdvancesFrame()
    {
        var spinner = new SpinnerComponent
        {
            Border = BorderStyle.None,
        };

        var before = spinner.FrameIndex;
        var changed = spinner.UpdateMouse(new MouseWheelMsg(MouseButton.WheelDown, 0, 0), new Rect(0, 0, 16, 1));

        TestAssert.True(changed, "Spinner wheel should advance frame while running.");
        TestAssert.True(spinner.FrameIndex != before, "Spinner wheel should move frame index.");
        return Task.CompletedTask;
    }

    private static Task CommandPaletteComponent_FiltersAndExecutes()
    {
        var palette = new CommandPaletteComponent
        {
            Focused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.Update(new KeyPressMsg(KeyCode.Character, "p", KeyModifiers.Ctrl));
        palette.Update(new KeyPressMsg(KeyCode.Character, "r"));
        palette.Update(new KeyPressMsg(KeyCode.Character, "o"));
        palette.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("rollback", palette.LastExecutedItemId ?? string.Empty, "Command palette should execute filtered item.");
        TestAssert.True(!palette.IsOpen, "Palette should close after execute.");
        return Task.CompletedTask;
    }

    private static Task CommandPaletteComponent_MouseClickExecutesSelection()
    {
        var palette = new CommandPaletteComponent
        {
            Focused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);
        palette.Open();

        var changed = palette.UpdateMouse(new MouseClickMsg(MouseButton.Left, 12, 5), new Rect(0, 0, 60, 20));

        TestAssert.True(changed, "Command palette click should execute selected command.");
        TestAssert.Equal("deploy", palette.LastExecutedItemId ?? string.Empty, "Palette click should execute clicked row.");
        TestAssert.True(!palette.IsOpen, "Palette should close after click execute.");
        return Task.CompletedTask;
    }

    private static Task CommandPaletteComponent_TryConsumeExecution_IsSingleUse()
    {
        var palette = new CommandPaletteComponent
        {
            Focused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.Update(new KeyPressMsg(KeyCode.Character, "p", KeyModifiers.Ctrl));
        palette.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(palette.TryConsumeExecution(out var itemId), "Command palette should expose one-shot execution consumption.");
        TestAssert.Equal("deploy", itemId, "Command palette should consume the executed item id.");
        TestAssert.True(!palette.TryConsumeExecution(out _), "Command palette should not report the same execution twice.");
        return Task.CompletedTask;
    }

    private static Task CommandPaletteComponent_ExposesQueryAccessors()
    {
        var palette = new CommandPaletteComponent();
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.SetQueryText("roll");

        TestAssert.Equal("roll", palette.QueryText, "Command palette should expose the current query text without requiring the raw text-input model.");

        palette.ClearQuery();

        TestAssert.Equal(string.Empty, palette.QueryText, "Command palette should clear the query through the component-level API.");
        return Task.CompletedTask;
    }

    private static Task TreeViewComponent_TogglesExpansion()
    {
        var tree = new TreeViewComponent
        {
            Focused = true,
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
            Focused = true,
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
            Focused = true,
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
