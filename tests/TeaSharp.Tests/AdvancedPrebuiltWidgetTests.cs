using TeaSharp.Components;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class AdvancedPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Advanced_BadgeComponent_RendersLabel", BadgeComponent_RendersLabel);
        yield return new TestCase("Advanced_ToggleSwitchComponent_TogglesValue", ToggleSwitchComponent_TogglesValue);
        yield return new TestCase("Advanced_SliderComponent_AdjustsValue", SliderComponent_AdjustsValue);
        yield return new TestCase("Advanced_SpinnerComponent_AdvancesFrame", SpinnerComponent_AdvancesFrame);
        yield return new TestCase("Advanced_CommandPaletteComponent_FiltersAndExecutes", CommandPaletteComponent_FiltersAndExecutes);
        yield return new TestCase("Advanced_TreeViewComponent_TogglesExpansion", TreeViewComponent_TogglesExpansion);
        yield return new TestCase("Advanced_NotificationCenterComponent_DismissesEntries", NotificationCenterComponent_DismissesEntries);
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
        palette.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("rollback", palette.LastExecutedItemId ?? string.Empty, "Command palette should execute filtered item.");
        TestAssert.True(!palette.IsOpen, "Palette should close after execute.");
        return Task.CompletedTask;
    }

    private static Task TreeViewComponent_TogglesExpansion()
    {
        var tree = new TreeViewComponent
        {
            Focused = true,
            ShowBorder = false,
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
}
