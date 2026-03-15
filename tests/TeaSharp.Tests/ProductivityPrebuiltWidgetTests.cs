using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

internal static class ProductivityPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Controls_NumberInput_AdjustsAndSubmits", NumberInput_AdjustsAndSubmits);
        yield return new TestCase("Controls_NumberInput_SubmittedEvent_ReportsValue", NumberInput_SubmittedEvent_ReportsValue);
        yield return new TestCase("Controls_NumberInput_TryConsumeSubmission_IsSingleUse", NumberInput_TryConsumeSubmission_IsSingleUse);
        yield return new TestCase("Controls_DatePicker_MovesDate", DatePicker_MovesDate);
        yield return new TestCase("Controls_DatePicker_DateChangedEvent_ReportsTransition", DatePicker_DateChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_DatePicker_MouseClickSelectsDate", DatePicker_MouseClickSelectsDate);
        yield return new TestCase("Controls_TimePicker_AdjustsField", TimePicker_AdjustsField);
        yield return new TestCase("Controls_TimePicker_ValueChangedEvent_ReportsTransition", TimePicker_ValueChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_TimePicker_MouseWheelAdjustsField", TimePicker_MouseWheelAdjustsField);
        yield return new TestCase("Controls_MarkdownView_RendersMarkdown", MarkdownView_RendersMarkdown);
    }

    private static Task NumberInput_AdjustsAndSubmits()
    {
        var input = new NumberInput
        {
            IsFocused = true,
            Min = 0,
            Max = 10,
            Step = 2,
        };
        input.SetValue(2);
        input.Handle(new KeyPressed(Key.Up));
        input.Handle(new KeyPressed(Key.Up));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(Math.Abs(input.Value - 6) < 0.0001, "Number input should adjust value by step.");
        TestAssert.True(input.LastSubmittedValue.HasValue, "Number input should track submitted value.");

        input.SetValue(4);
        input.Handle(new KeyPressed(Key.Character, "1"));
        input.Handle(new KeyPressed(Key.Character, "2"));
        input.Handle(new KeyPressed(Key.Character, "."));
        input.Handle(new KeyPressed(Key.Character, "5"));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(Math.Abs(input.Value - 10) < 0.0001, "Number input should parse decimal text entry and clamp to range.");
        return Task.CompletedTask;
    }

    private static Task NumberInput_TryConsumeSubmission_IsSingleUse()
    {
        var input = new NumberInput
        {
            IsFocused = true,
        };
        input.SetValue(3);

        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(input.TryConsumeSubmission(out var submitted), "Number input should expose one-shot submit consumption.");
        TestAssert.True(Math.Abs(submitted - 3) < 0.0001, "Number input should consume the submitted numeric value.");
        TestAssert.True(!input.TryConsumeSubmission(out _), "Number input should not report the same submit twice.");
        return Task.CompletedTask;
    }

    private static Task NumberInput_SubmittedEvent_ReportsValue()
    {
        var input = new NumberInput
        {
            IsFocused = true,
        };
        input.SetValue(3);
        double submitted = -1;
        input.Submitted += (_, args) => submitted = args.Value;

        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(Math.Abs(submitted - 3) < 0.0001, "Number input submit event should expose the submitted numeric value.");
        return Task.CompletedTask;
    }

    private static Task DatePicker_MovesDate()
    {
        var picker = new DatePicker
        {
            IsFocused = true,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));
        picker.Handle(new KeyPressed(Key.Right));
        picker.Handle(new KeyPressed(Key.Down));

        TestAssert.Equal(new DateOnly(2026, 3, 16), picker.SelectedDate, "Date picker should move day and week correctly.");
        return Task.CompletedTask;
    }

    private static Task DatePicker_MouseClickSelectsDate()
    {
        var picker = new DatePicker
        {
            Border = BorderStyle.None,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));

        var changed = picker.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 4), new Rect(0, 0, 24, 10));

        TestAssert.True(changed, "Date picker click should select day under pointer.");
        TestAssert.Equal(new DateOnly(2026, 3, 9), picker.SelectedDate, "Date picker click should select correct calendar date.");
        return Task.CompletedTask;
    }

    private static Task DatePicker_DateChangedEvent_ReportsTransition()
    {
        var picker = new DatePicker
        {
            IsFocused = true,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));
        DateChangedEventArgs? args = null;
        picker.DateChanged += (_, eventArgs) => args = eventArgs;

        picker.Handle(new KeyPressed(Key.Right));

        TestAssert.True(args is not null, "Date picker should raise date changed when the selected date changes.");
        TestAssert.Equal(new DateOnly(2026, 3, 8), args!.PreviousDate, "Date picker event should expose the previous date.");
        TestAssert.Equal(new DateOnly(2026, 3, 9), args.SelectedDate, "Date picker event should expose the selected date.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_AdjustsField()
    {
        var picker = new TimePicker
        {
            IsFocused = true,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));
        picker.Handle(new KeyPressed(Key.Right));
        picker.Handle(new KeyPressed(Key.Up));

        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker should adjust minute field.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_MouseWheelAdjustsField()
    {
        var picker = new TimePicker
        {
            Border = BorderStyle.None,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));

        picker.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 0), new Rect(0, 0, 12, 1));
        var changed = picker.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelUp, 3, 0), new Rect(0, 0, 12, 1));

        TestAssert.True(changed, "Time picker wheel should adjust hovered/active field.");
        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker wheel should increase minute field by configured step.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_ValueChangedEvent_ReportsTransition()
    {
        var picker = new TimePicker
        {
            IsFocused = true,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));
        TimeValueChangedEventArgs? args = null;
        picker.ValueChanged += (_, eventArgs) => args = eventArgs;

        picker.Handle(new KeyPressed(Key.Right));
        picker.Handle(new KeyPressed(Key.Up));

        TestAssert.True(args is not null, "Time picker should raise value changed when the selected time changes.");
        TestAssert.Equal(new TimeOnly(10, 0, 0), args!.PreviousValue, "Time picker event should expose the previous value.");
        TestAssert.Equal(new TimeOnly(10, 5, 0), args.Value, "Time picker event should expose the current value.");
        return Task.CompletedTask;
    }

    private static Task MarkdownView_RendersMarkdown()
    {
        var viewer = new MarkdownView
        {
            Border = BorderStyle.None,
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
