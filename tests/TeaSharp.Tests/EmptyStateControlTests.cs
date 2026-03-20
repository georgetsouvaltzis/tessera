using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class EmptyStateControlTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "Controls_EmptyState_RendersTitleBodyHintAndAction",
            RendersTitleBodyHintAndAction);
        yield return new TestCase(
            "Controls_EmptyState_KeyboardActivation_EnterAndSpaceWhenFocused",
            KeyboardActivation_EnterAndSpaceWhenFocused);
        yield return new TestCase(
            "Controls_EmptyState_PointerHoverAndClickInBounds_ActivatesAction",
            PointerHoverAndClickInBounds_ActivatesAction);
        yield return new TestCase(
            "Controls_EmptyState_StateAndActionStyles_AppliedDeterministically",
            StateAndActionStyles_AppliedDeterministically);
        yield return new TestCase(
            "Controls_EmptyState_Disabled_PreventsActivationAndAppliesDisabledStyle",
            Disabled_PreventsActivationAndAppliesDisabledStyle);
    }

    private static Task RendersTitleBodyHintAndAction()
    {
        var control = new EmptyState
        {
            Border = BorderStyle.None,
            Title = "No projects",
            Body = "Create one\nor import.",
            Hint = "Press N to create.",
            ActionLabel = "Create project",
        };
        var canvas = new Canvas(44, 8);

        control.Render(canvas, new Rect(0, 0, 44, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("No projects", StringComparison.Ordinal), "EmptyState should render title.");
        TestAssert.True(output.Contains("Create one", StringComparison.Ordinal), "EmptyState should render body line one.");
        TestAssert.True(output.Contains("or import.", StringComparison.Ordinal), "EmptyState should render body line two.");
        TestAssert.True(output.Contains("Press N to create.", StringComparison.Ordinal), "EmptyState should render hint text.");
        TestAssert.True(output.Contains("[Create project]", StringComparison.Ordinal), "EmptyState should render action label.");
        return Task.CompletedTask;
    }

    private static Task KeyboardActivation_EnterAndSpaceWhenFocused()
    {
        var control = new EmptyState
        {
            IsFocused = true,
            ActionLabel = "Retry",
        };
        var activationCount = 0;
        control.ActionInvoked += (_, _) => activationCount++;

        var enterHandled = control.Handle(new KeyPressed(Key.Enter));
        var spaceHandled = control.Handle(new KeyPressed(Key.Character, " "));
        var unfocusedHandled = new EmptyState
        {
            IsFocused = false,
            ActionLabel = "Retry",
        }.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(enterHandled, "Focused EmptyState should activate on enter.");
        TestAssert.True(spaceHandled, "Focused EmptyState should activate on space.");
        TestAssert.Equal(2, activationCount, "Enter and space should each invoke EmptyState action.");
        TestAssert.True(!unfocusedHandled, "Unfocused EmptyState should ignore keyboard activation.");
        return Task.CompletedTask;
    }

    private static Task PointerHoverAndClickInBounds_ActivatesAction()
    {
        var control = new EmptyState
        {
            Border = BorderStyle.None,
            Hint = "Hover row",
            ActionLabel = "Open wizard",
            HoveredStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
        };
        var activationCount = 0;
        control.ActionInvoked += (_, _) => activationCount++;
        var bounds = new Rect(0, 0, 32, 6);

        var hoverHandled = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), bounds);
        var hoveredCanvas = new Canvas(32, 6, CanvasTextMode.GraphemeAware);
        control.Render(hoveredCanvas, bounds);
        var hoveredOutput = hoveredCanvas.Render();

        var pressHandled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var leaveHandled = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 40, 9), bounds);
        var leaveCanvas = new Canvas(32, 6, CanvasTextMode.GraphemeAware);
        control.Render(leaveCanvas, bounds);
        var leaveOutput = leaveCanvas.Render();

        TestAssert.True(hoverHandled, "Pointer motion inside bounds should mark EmptyState as hovered.");
        TestAssert.True(hoveredOutput.Contains("38;2;31;32;33", StringComparison.Ordinal), "Hovered pointer state should apply hovered style.");
        TestAssert.True(pressHandled, "Pointer press inside bounds should activate EmptyState action.");
        TestAssert.Equal(1, activationCount, "Pointer press should invoke action exactly once.");
        TestAssert.True(leaveHandled, "Pointer motion outside bounds should clear hover state.");
        TestAssert.True(!leaveOutput.Contains("38;2;31;32;33", StringComparison.Ordinal), "Leaving bounds should clear hovered style.");
        return Task.CompletedTask;
    }

    private static Task StateAndActionStyles_AppliedDeterministically()
    {
        var control = new EmptyState
        {
            Border = BorderStyle.None,
            IsFocused = true,
            Title = "No data",
            Body = "Add your first item.",
            Hint = "Hint row",
            ActionLabel = "Create",
            DefaultStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(1, 2, 3)),
            FocusedStyle = TeaStyle.Empty.WithItalic(),
            HoveredStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            ActionStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)).WithUnderline(),
        };
        var bounds = new Rect(0, 0, 40, 8);
        var hoverChanged = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), bounds);
        var canvas = new Canvas(40, 8, CanvasTextMode.GraphemeAware);

        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(hoverChanged, "Pointer motion should update EmptyState hover state.");
        TestAssert.True(control.IsHovered, "Pointer motion should leave EmptyState in hovered state.");
        TestAssert.True(output.Contains("48;2;1;2;3", StringComparison.Ordinal), "Default style should be present in rendered output.");
        TestAssert.True(output.Contains(";3m", StringComparison.Ordinal), "Focused style should be present in rendered output.");
        TestAssert.True(output.Contains("38;2;7;8;9", StringComparison.Ordinal), "Hovered style should be present in rendered output.");
        TestAssert.True(output.Contains("38;2;10;11;12", StringComparison.Ordinal), "Action style should be present in rendered output.");
        return Task.CompletedTask;
    }

    private static Task Disabled_PreventsActivationAndAppliesDisabledStyle()
    {
        var control = new EmptyState
        {
            Border = BorderStyle.None,
            IsFocused = true,
            IsDisabled = true,
            ActionLabel = "Create",
            DisabledStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(90, 91, 92)),
        };
        var keyboardHandled = control.Handle(new KeyPressed(Key.Enter));
        var pointerHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1),
            new Rect(0, 0, 30, 6));
        var canvas = new Canvas(30, 6, CanvasTextMode.GraphemeAware);

        control.Render(canvas, new Rect(0, 0, 30, 6));
        var output = canvas.Render();

        TestAssert.True(!keyboardHandled, "Disabled EmptyState should ignore keyboard activation.");
        TestAssert.True(!pointerHandled, "Disabled EmptyState should ignore pointer activation.");
        TestAssert.True(output.Contains("38;2;90;91;92", StringComparison.Ordinal), "Disabled style should be present in rendered output.");
        return Task.CompletedTask;
    }
}

[TestFixture]
[NonParallelizable]
public sealed class EmptyStateControlNUnitAdapter
{
    public static IEnumerable<TestCaseData> Cases()
    {
        foreach (var testCase in EmptyStateControlTests.Cases())
        {
            yield return new TestCaseData(testCase).SetName(testCase.Name);
        }
    }

    [TestCaseSource(nameof(Cases))]
    public Task Execute(TestCase testCase)
    {
        return testCase.Execute();
    }
}
