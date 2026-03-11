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

internal static class ProductivityPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Productivity_MenuBarComponent_ActivatesShortcut", MenuBarComponent_ActivatesShortcut);
        yield return new TestCase("Productivity_MenuBarComponent_MouseClickActivatesItem", MenuBarComponent_MouseClickActivatesItem);
        yield return new TestCase("Productivity_MenuBarComponent_ParamsSetterReplacesItems", MenuBarComponent_ParamsSetterReplacesItems);
        yield return new TestCase("Productivity_ContextMenuComponent_ExecutesAndCloses", ContextMenuComponent_ExecutesAndCloses);
        yield return new TestCase("Productivity_ContextMenuComponent_MouseClickExecutesAndCloses", ContextMenuComponent_MouseClickExecutesAndCloses);
        yield return new TestCase("Productivity_ContextMenuComponent_MouseReleaseExecutesAndCloses", ContextMenuComponent_MouseReleaseExecutesAndCloses);
        yield return new TestCase("Productivity_ContextMenuComponent_ParamsSetterReplacesItems", ContextMenuComponent_ParamsSetterReplacesItems);
        yield return new TestCase("Productivity_NumberInputComponent_AdjustsAndSubmits", NumberInputComponent_AdjustsAndSubmits);
        yield return new TestCase("Productivity_DatePickerComponent_MovesDate", DatePickerComponent_MovesDate);
        yield return new TestCase("Productivity_DatePickerComponent_MouseClickSelectsDate", DatePickerComponent_MouseClickSelectsDate);
        yield return new TestCase("Productivity_TimePickerComponent_AdjustsField", TimePickerComponent_AdjustsField);
        yield return new TestCase("Productivity_TimePickerComponent_MouseWheelAdjustsField", TimePickerComponent_MouseWheelAdjustsField);
        yield return new TestCase("Productivity_MarkdownViewerComponent_RendersMarkdown", MarkdownViewerComponent_RendersMarkdown);
    }

    private static Task MenuBarComponent_ActivatesShortcut()
    {
        var menu = new MenuBarComponent(new MenuBarOptions(
            Items:
            [
                new MenuBarItem("file", "File", 'f'),
                new MenuBarItem("edit", "Edit", 'e'),
                new MenuBarItem("help", "Help", 'h'),
            ],
            Focused: true));

        menu.Update(new KeyPressMsg(KeyCode.Character, "e"));
        menu.Update(new KeyPressMsg(KeyCode.Character, "h"));
        var activationVersion = menu.ActivationVersion;
        menu.Update(new KeyPressMsg(KeyCode.Enter));
        menu.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("help", menu.LastActivatedItemId ?? string.Empty, "Menu bar should prioritize shortcut activation over navigation aliases.");
        TestAssert.True(menu.ActivationVersion == activationVersion + 2, "Menu bar should count repeated activations on the same selected item.");
        return Task.CompletedTask;
    }

    private static Task MenuBarComponent_MouseClickActivatesItem()
    {
        var menu = new MenuBarComponent();
        menu.SetItems(
        [
            new MenuBarItem("file", "File", 'f'),
            new MenuBarItem("edit", "Edit", 'e'),
            new MenuBarItem("help", "Help", 'h'),
        ]);

        var changed = menu.UpdateMouse(new MouseClickMsg(MouseButton.Left, 12, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Menu mouse click should trigger selection/activation.");
        TestAssert.Equal("edit", menu.LastActivatedItemId ?? string.Empty, "Menu mouse click should activate clicked item.");
        return Task.CompletedTask;
    }

    private static Task ContextMenuComponent_ExecutesAndCloses()
    {
        var menu = new ContextMenuComponent(new ContextMenuOptions(
            Items:
            [
                new ContextMenuItem("copy", "Copy"),
                new ContextMenuItem("paste", "Paste"),
            ],
            Focused: true));
        menu.OpenAt(4, 2);
        menu.Update(new KeyPressMsg(KeyCode.Down));
        menu.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu should execute selected action.");
        TestAssert.True(!menu.Visible, "Context menu should close after execute.");
        return Task.CompletedTask;
    }

    private static Task ContextMenuComponent_MouseClickExecutesAndCloses()
    {
        var menu = new ContextMenuComponent(new ContextMenuOptions(
            Items:
            [
                new ContextMenuItem("copy", "Copy"),
                new ContextMenuItem("paste", "Paste"),
            ],
            Border: BorderStyle.None));
        menu.OpenAt(0, 0);

        var changed = menu.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 1), new Rect(0, 0, 20, 6));

        TestAssert.True(changed, "Context menu click should execute row action.");
        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu click should execute clicked item.");
        TestAssert.True(!menu.Visible, "Context menu should close after mouse execute.");
        return Task.CompletedTask;
    }

    private static Task ContextMenuComponent_MouseReleaseExecutesAndCloses()
    {
        var menu = new ContextMenuComponent(new ContextMenuOptions(
            Items:
            [
                new ContextMenuItem("copy", "Copy"),
                new ContextMenuItem("paste", "Paste"),
            ],
            Border: BorderStyle.None));
        menu.OpenAt(0, 0);

        var changed = menu.UpdateMouse(new MouseReleaseMsg(MouseButton.None, 0, 1), new Rect(0, 0, 20, 6));

        TestAssert.True(changed, "Context menu mouse release should execute row action.");
        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu release should execute hovered item.");
        TestAssert.True(!menu.Visible, "Context menu should close after mouse release execute.");
        return Task.CompletedTask;
    }

    private static Task MenuBarComponent_ParamsSetterReplacesItems()
    {
        var menu = new MenuBarComponent();

        menu.SetItems(
            new MenuBarItem("file", "File", 'f'),
            new MenuBarItem("help", "Help", 'h'));

        TestAssert.Equal(2, menu.Items.Count, "Params-based menu bar setup should populate items.");
        TestAssert.Equal("file", menu.Items[0].Id, "Params-based menu bar setup should keep item order.");
        return Task.CompletedTask;
    }

    private static Task ContextMenuComponent_ParamsSetterReplacesItems()
    {
        var menu = new ContextMenuComponent();

        menu.SetItems(
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"));

        TestAssert.Equal(2, menu.Items.Count, "Params-based context menu setup should populate items.");
        TestAssert.Equal("paste", menu.Items[1].Id, "Params-based context menu setup should keep item order.");
        return Task.CompletedTask;
    }

    private static Task NumberInputComponent_AdjustsAndSubmits()
    {
        var input = new NumberInputComponent(new NumberInputOptions(
            Focused: true,
            Min: 0,
            Max: 10,
            Step: 2,
            InitialValue: 2));
        input.Update(new KeyPressMsg(KeyCode.Up));
        input.Update(new KeyPressMsg(KeyCode.Up));
        input.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(Math.Abs(input.Value - 6) < 0.0001, "Number input should adjust value by step.");
        TestAssert.True(input.LastSubmittedValue.HasValue, "Number input should track submitted value.");

        input.SetValue(4);
        input.Update(new KeyPressMsg(KeyCode.Character, "1"));
        input.Update(new KeyPressMsg(KeyCode.Character, "2"));
        input.Update(new KeyPressMsg(KeyCode.Character, "."));
        input.Update(new KeyPressMsg(KeyCode.Character, "5"));
        input.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(Math.Abs(input.Value - 10) < 0.0001, "Number input should parse decimal text entry and clamp to range.");
        return Task.CompletedTask;
    }

    private static Task DatePickerComponent_MovesDate()
    {
        var picker = new DatePickerComponent(new DatePickerOptions(
            Focused: true,
            InitialDate: new DateOnly(2026, 3, 8)));
        picker.Update(new KeyPressMsg(KeyCode.Right));
        picker.Update(new KeyPressMsg(KeyCode.Down));

        TestAssert.Equal(new DateOnly(2026, 3, 16), picker.SelectedDate, "Date picker should move day and week correctly.");
        return Task.CompletedTask;
    }

    private static Task DatePickerComponent_MouseClickSelectsDate()
    {
        var picker = new DatePickerComponent(new DatePickerOptions(
            Border: BorderStyle.None,
            InitialDate: new DateOnly(2026, 3, 8)));

        var changed = picker.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 4), new Rect(0, 0, 24, 10));

        TestAssert.True(changed, "Date picker click should select day under pointer.");
        TestAssert.Equal(new DateOnly(2026, 3, 9), picker.SelectedDate, "Date picker click should select correct calendar date.");
        return Task.CompletedTask;
    }

    private static Task TimePickerComponent_AdjustsField()
    {
        var picker = new TimePickerComponent(new TimePickerOptions(
            Focused: true,
            MinuteStep: 5,
            InitialValue: new TimeOnly(10, 0, 0)));
        picker.Update(new KeyPressMsg(KeyCode.Right));
        picker.Update(new KeyPressMsg(KeyCode.Up));

        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker should adjust minute field.");
        return Task.CompletedTask;
    }

    private static Task TimePickerComponent_MouseWheelAdjustsField()
    {
        var picker = new TimePickerComponent(new TimePickerOptions(
            Border: BorderStyle.None,
            MinuteStep: 5,
            InitialValue: new TimeOnly(10, 0, 0)));

        picker.UpdateMouse(new MouseClickMsg(MouseButton.Left, 3, 0), new Rect(0, 0, 12, 1));
        var changed = picker.UpdateMouse(new MouseWheelMsg(MouseButton.WheelUp, 3, 0), new Rect(0, 0, 12, 1));

        TestAssert.True(changed, "Time picker wheel should adjust hovered/active field.");
        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker wheel should increase minute field by configured step.");
        return Task.CompletedTask;
    }

    private static Task MarkdownViewerComponent_RendersMarkdown()
    {
        var viewer = new MarkdownViewerComponent(new MarkdownViewerOptions(
            Border: BorderStyle.None,
            InitialMarkdown: "# title\n- one\n```\ncode\n```"));
        var canvas = new Canvas(40, 8);

        viewer.Render(canvas, new Rect(0, 0, 40, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("# TITLE", StringComparison.Ordinal), "Markdown viewer should render heading.");
        TestAssert.True(output.Contains("• one", StringComparison.Ordinal), "Markdown viewer should render bullets.");
        TestAssert.True(output.Contains("code", StringComparison.Ordinal), "Markdown viewer should render code block content.");
        return Task.CompletedTask;
    }
}
