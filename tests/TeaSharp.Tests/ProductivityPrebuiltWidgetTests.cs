using TeaSharp.Components;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class ProductivityPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Productivity_MenuBarComponent_ActivatesShortcut", MenuBarComponent_ActivatesShortcut);
        yield return new TestCase("Productivity_ContextMenuComponent_ExecutesAndCloses", ContextMenuComponent_ExecutesAndCloses);
        yield return new TestCase("Productivity_NumberInputComponent_AdjustsAndSubmits", NumberInputComponent_AdjustsAndSubmits);
        yield return new TestCase("Productivity_DatePickerComponent_MovesDate", DatePickerComponent_MovesDate);
        yield return new TestCase("Productivity_TimePickerComponent_AdjustsField", TimePickerComponent_AdjustsField);
        yield return new TestCase("Productivity_MarkdownViewerComponent_RendersMarkdown", MarkdownViewerComponent_RendersMarkdown);
    }

    private static Task MenuBarComponent_ActivatesShortcut()
    {
        var menu = new MenuBarComponent
        {
            Focused = true,
        };
        menu.SetItems(
        [
            new MenuBarItem("file", "File", 'f'),
            new MenuBarItem("edit", "Edit", 'e'),
            new MenuBarItem("help", "Help", 'h'),
        ]);

        menu.Update(new KeyPressMsg(KeyCode.Character, "e"));
        menu.Update(new KeyPressMsg(KeyCode.Character, "h"));
        var activationVersion = menu.ActivationVersion;
        menu.Update(new KeyPressMsg(KeyCode.Enter));
        menu.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("help", menu.LastActivatedItemId ?? string.Empty, "Menu bar should prioritize shortcut activation over navigation aliases.");
        TestAssert.True(menu.ActivationVersion == activationVersion + 2, "Menu bar should count repeated activations on the same selected item.");
        return Task.CompletedTask;
    }

    private static Task ContextMenuComponent_ExecutesAndCloses()
    {
        var menu = new ContextMenuComponent
        {
            Focused = true,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(4, 2);
        menu.Update(new KeyPressMsg(KeyCode.Down));
        menu.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu should execute selected action.");
        TestAssert.True(!menu.Visible, "Context menu should close after execute.");
        return Task.CompletedTask;
    }

    private static Task NumberInputComponent_AdjustsAndSubmits()
    {
        var input = new NumberInputComponent
        {
            Focused = true,
            Min = 0,
            Max = 10,
            Step = 2,
        };
        input.SetValue(2);
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
        var picker = new DatePickerComponent
        {
            Focused = true,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));
        picker.Update(new KeyPressMsg(KeyCode.Right));
        picker.Update(new KeyPressMsg(KeyCode.Down));

        TestAssert.Equal(new DateOnly(2026, 3, 16), picker.SelectedDate, "Date picker should move day and week correctly.");
        return Task.CompletedTask;
    }

    private static Task TimePickerComponent_AdjustsField()
    {
        var picker = new TimePickerComponent
        {
            Focused = true,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));
        picker.Update(new KeyPressMsg(KeyCode.Right));
        picker.Update(new KeyPressMsg(KeyCode.Up));

        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker should adjust minute field.");
        return Task.CompletedTask;
    }

    private static Task MarkdownViewerComponent_RendersMarkdown()
    {
        var viewer = new MarkdownViewerComponent
        {
            ShowBorder = false,
        };
        viewer.SetMarkdown("# title\n- one\n```\ncode\n```");
        var canvas = new Canvas(40, 8);

        viewer.Render(canvas, new Rect(0, 0, 40, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("# TITLE", StringComparison.Ordinal), "Markdown viewer should render heading.");
        TestAssert.True(output.Contains("• one", StringComparison.Ordinal), "Markdown viewer should render bullets.");
        TestAssert.True(output.Contains("code", StringComparison.Ordinal), "Markdown viewer should render code block content.");
        return Task.CompletedTask;
    }
}
