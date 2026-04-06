using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

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
            "Controls_EmptyState_Styles_ApplyDefaultFocusedHoveredDisabledAndAction",
            Styles_ApplyDefaultFocusedHoveredDisabledAndAction);
        yield return new TestCase(
            "Controls_EmptyState_Disabled_PreventsKeyboardAndPointerActivation",
            Disabled_PreventsKeyboardAndPointerActivation);
    }

    private static Task RendersTitleBodyHintAndAction()
    {
        var control = new EmptyState
        {
            Title = "No projects",
            Body = "Create one\nor import.",
            Hint = "Press N to create.",
            ActionLabel = "Create project",
            ShowAction = true,
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
            ShowAction = true,
            ActionLabel = "Retry",
        };
        var activationCount = 0;
        control.ActionInvoked += (_, _) => activationCount++;

        var enterHandled = control.Handle(new KeyPressed(Key.Enter));
        var spaceHandled = control.Handle(new KeyPressed(Key.Character, " "));
        var unfocusedHandled = new EmptyState
        {
            IsFocused = false,
            ShowAction = true,
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
            ShowAction = true,
            ActionLabel = "Open wizard",
        };
        var activationCount = 0;
        control.Activated += (_, _) => activationCount++;
        var bounds = new Rect(0, 0, 32, 6);

        var hoverHandled = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), bounds);
        var hoveredAfterMotion = control.IsHovered;
        var pressHandled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var leaveHandled = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 40, 9), bounds);

        TestAssert.True(hoverHandled, "Pointer motion inside bounds should mark EmptyState as hovered.");
        TestAssert.True(hoveredAfterMotion, "Pointer motion inside bounds should set IsHovered.");
        TestAssert.True(pressHandled, "Pointer press inside bounds should activate EmptyState action.");
        TestAssert.Equal(1, activationCount, "Pointer press should invoke action exactly once.");
        TestAssert.True(leaveHandled, "Pointer motion outside bounds should clear hover state.");
        TestAssert.True(!control.IsHovered, "Pointer motion outside bounds should clear IsHovered.");
        return Task.CompletedTask;
    }

    private static Task Styles_ApplyDefaultFocusedHoveredDisabledAndAction()
    {
        var control = new EmptyState
        {
            IsFocused = true,
            IsDisabled = true,
            Title = "No data",
            Body = "Add your first item.",
            Hint = "Hint row",
            ShowAction = true,
            ActionLabel = "Create",
            DefaultStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(1, 2, 3)),
            FocusedStyle = TesseraStyle.Empty.WithItalic(),
            HoveredStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            DisabledStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 91, 92)),
            ActionStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)).WithUnderline(),
        };
        var bounds = new Rect(0, 0, 40, 8);
        var hoverChanged = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), bounds);
        var canvas = new Canvas(40, 8, CanvasTextMode.GraphemeAware);

        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(hoverChanged, "Pointer motion should update EmptyState hover state.");
        TestAssert.True(output.Contains("48;2;1;2;3", StringComparison.Ordinal), "Default style should be present in rendered output.");
        TestAssert.True(output.Contains(";3m", StringComparison.Ordinal), "Focused style should be present in rendered output.");
        TestAssert.True(output.Contains("38;2;7;8;9", StringComparison.Ordinal), "Hovered style should be present in rendered output.");
        TestAssert.True(output.Contains("38;2;90;91;92", StringComparison.Ordinal), "Disabled style should be present in rendered output.");
        TestAssert.True(output.Contains("38;2;10;11;12", StringComparison.Ordinal), "Action style should be present in rendered output.");
        return Task.CompletedTask;
    }

    private static Task Disabled_PreventsKeyboardAndPointerActivation()
    {
        var control = new EmptyState
        {
            IsFocused = true,
            IsDisabled = true,
            ShowAction = true,
            ActionLabel = "Create",
        };
        var activationCount = 0;
        control.ActionInvoked += (_, _) => activationCount++;

        var keyboardHandled = control.Handle(new KeyPressed(Key.Enter));
        var pointerHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1),
            new Rect(0, 0, 30, 6));

        TestAssert.True(!keyboardHandled, "Disabled EmptyState should ignore keyboard activation.");
        TestAssert.True(!pointerHandled, "Disabled EmptyState should ignore pointer activation.");
        TestAssert.Equal(0, activationCount, "Disabled EmptyState should not invoke action.");
        return Task.CompletedTask;
    }
}
