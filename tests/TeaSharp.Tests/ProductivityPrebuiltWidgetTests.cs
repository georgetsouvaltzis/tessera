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
        yield return new TestCase("Productivity_NumberInputComponent_AdjustsAndSubmits", NumberInputComponent_AdjustsAndSubmits);
        yield return new TestCase("Productivity_NumberInputComponent_SubmittedEvent_ReportsValue", NumberInputComponent_SubmittedEvent_ReportsValue);
        yield return new TestCase("Productivity_NumberInputComponent_TryConsumeSubmit_IsSingleUse", NumberInputComponent_TryConsumeSubmit_IsSingleUse);
        yield return new TestCase("Productivity_DatePickerComponent_MovesDate", DatePickerComponent_MovesDate);
        yield return new TestCase("Productivity_DatePickerComponent_DateChangedEvent_ReportsTransition", DatePickerComponent_DateChangedEvent_ReportsTransition);
        yield return new TestCase("Productivity_DatePickerComponent_MouseClickSelectsDate", DatePickerComponent_MouseClickSelectsDate);
        yield return new TestCase("Productivity_TimePickerComponent_AdjustsField", TimePickerComponent_AdjustsField);
        yield return new TestCase("Productivity_TimePickerComponent_ValueChangedEvent_ReportsTransition", TimePickerComponent_ValueChangedEvent_ReportsTransition);
        yield return new TestCase("Productivity_TimePickerComponent_MouseWheelAdjustsField", TimePickerComponent_MouseWheelAdjustsField);
        yield return new TestCase("Productivity_MarkdownViewerComponent_RendersMarkdown", MarkdownViewerComponent_RendersMarkdown);
    }

    private static Task NumberInputComponent_AdjustsAndSubmits()
    {
        var input = new NumberInputComponent(new NumberInputOptions(
            IsFocused: true,
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

    private static Task NumberInputComponent_TryConsumeSubmit_IsSingleUse()
    {
        var input = new NumberInputComponent(new NumberInputOptions(
            IsFocused: true,
            InitialValue: 3));

        input.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(input.TryConsumeSubmit(out var submitted), "Number input should expose one-shot submit consumption.");
        TestAssert.True(Math.Abs(submitted - 3) < 0.0001, "Number input should consume the submitted numeric value.");
        TestAssert.True(!input.TryConsumeSubmit(out _), "Number input should not report the same submit twice.");
        return Task.CompletedTask;
    }

    private static Task NumberInputComponent_SubmittedEvent_ReportsValue()
    {
        var input = new NumberInputComponent(new NumberInputOptions(
            IsFocused: true,
            InitialValue: 3));
        double submitted = -1;
        input.Submitted += (_, args) => submitted = args.Value;

        input.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(Math.Abs(submitted - 3) < 0.0001, "Number input submit event should expose the submitted numeric value.");
        return Task.CompletedTask;
    }

    private static Task DatePickerComponent_MovesDate()
    {
        var picker = new DatePickerComponent(new DatePickerOptions(
            IsFocused: true,
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

    private static Task DatePickerComponent_DateChangedEvent_ReportsTransition()
    {
        var picker = new DatePickerComponent(new DatePickerOptions(
            IsFocused: true,
            InitialDate: new DateOnly(2026, 3, 8)));
        DateChangedEventArgs? args = null;
        picker.DateChanged += (_, eventArgs) => args = eventArgs;

        picker.Update(new KeyPressMsg(KeyCode.Right));

        TestAssert.True(args is not null, "Date picker should raise date changed when the selected date changes.");
        TestAssert.Equal(new DateOnly(2026, 3, 8), args!.PreviousDate, "Date picker event should expose the previous date.");
        TestAssert.Equal(new DateOnly(2026, 3, 9), args.SelectedDate, "Date picker event should expose the selected date.");
        return Task.CompletedTask;
    }

    private static Task TimePickerComponent_AdjustsField()
    {
        var picker = new TimePickerComponent(new TimePickerOptions(
            IsFocused: true,
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

    private static Task TimePickerComponent_ValueChangedEvent_ReportsTransition()
    {
        var picker = new TimePickerComponent(new TimePickerOptions(
            IsFocused: true,
            MinuteStep: 5,
            InitialValue: new TimeOnly(10, 0, 0)));
        TimeValueChangedEventArgs? args = null;
        picker.ValueChanged += (_, eventArgs) => args = eventArgs;

        picker.Update(new KeyPressMsg(KeyCode.Right));
        picker.Update(new KeyPressMsg(KeyCode.Up));

        TestAssert.True(args is not null, "Time picker should raise value changed when the selected time changes.");
        TestAssert.Equal(new TimeOnly(10, 0, 0), args!.PreviousValue, "Time picker event should expose the previous value.");
        TestAssert.Equal(new TimeOnly(10, 5, 0), args.Value, "Time picker event should expose the current value.");
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
