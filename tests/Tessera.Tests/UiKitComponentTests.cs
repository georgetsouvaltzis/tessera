using Tessera.Components.Composition;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Controls;
using Tessera.Core.Messages;

namespace Tessera.Tests;

internal static class UiKitComponentTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("UiKit_Canvas_DrawBox_BorderStyles_RenderExpectedCorners", Canvas_DrawBox_BorderStyles_RenderExpectedCorners);
        yield return new TestCase("Controls_StatusBar_PlacesLeftAndRightText", StatusBar_PlacesLeftAndRightText);
        yield return new TestCase("Controls_StatusBar_UsesFillCharacter", StatusBar_UsesFillCharacter);
        yield return new TestCase("Controls_FormComponents_RespondToInput", FormComponents_RespondToInput);
        yield return new TestCase("Controls_Modal_VisibleStateControlsRendering", Modal_VisibleStateControlsRendering);
        yield return new TestCase("Controls_Modal_BackdropOccludesUnderlyingContent", Modal_BackdropOccludesUnderlyingContent);
        yield return new TestCase("Controls_Modal_BackdropOccludesUnderlyingContent_GraphemeAwareCanvas", Modal_BackdropOccludesUnderlyingContent_GraphemeAwareCanvas);
    }

    private static Task Canvas_DrawBox_BorderStyles_RenderExpectedCorners()
    {
        // Arrange
        var rounded = new Canvas(12, 4);
        var ascii = new Canvas(12, 4);

        // Act
        rounded.DrawBox(new Rect(0, 0, 12, 4), "Rounded", BorderStyle.Rounded);
        ascii.DrawBox(new Rect(0, 0, 12, 4), "Ascii", BorderStyle.Ascii);
        var roundedOutput = rounded.Render();
        var asciiOutput = ascii.Render();

        // Assert
        TestAssert.True(roundedOutput.Contains('╭'), "Rounded border should render rounded top-left corner.");
        TestAssert.True(roundedOutput.Contains('╯'), "Rounded border should render rounded bottom-right corner.");
        TestAssert.True(asciiOutput.Contains('+'), "ASCII border should render plus corners.");
        return Task.CompletedTask;
    }

    private static Task StatusBar_PlacesLeftAndRightText()
    {
        // Arrange
        var statusBar = new StatusBar
        {
            LeftText = "left",
            RightText = "right",
        };
        var canvas = new Canvas(24, 1);

        // Act
        statusBar.Render(canvas, new Rect(0, 0, 24, 1));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.StartsWith("left", StringComparison.Ordinal), "Status bar should place left text at row start.");
        TestAssert.True(output.EndsWith("right", StringComparison.Ordinal), "Status bar should align right text to row end.");
        return Task.CompletedTask;
    }

    private static Task StatusBar_UsesFillCharacter()
    {
        // Arrange
        var statusBar = new StatusBar
        {
            LeftText = "L",
            RightText = "R",
            Fill = '.',
        };
        var canvas = new Canvas(16, 1);

        // Act
        statusBar.Render(canvas, new Rect(0, 0, 16, 1));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains('.'), "Status bar should use theme fill character.");
        return Task.CompletedTask;
    }

    private static Task FormComponents_RespondToInput()
    {
        // Arrange
        var checklist = new MultiSelect
        {
            IsFocused = true,
        };
        checklist.SetItems([("focus", true), ("mouse", false)]);

        var radio = new RadioGroup
        {
            IsFocused = true,
        };
        radio.SetItems(["a", "b", "c"]);

        // Act
        checklist.Handle(new KeyPressed(Key.Down));
        checklist.Handle(new KeyPressed(Key.Enter));
        radio.Handle(new KeyPressed(Key.Right));

        // Assert
        TestAssert.True(checklist.CheckedItems.Contains("mouse", StringComparer.Ordinal), "Checklist enter key should toggle selected item.");
        TestAssert.Equal(1, radio.SelectedIndex, "Radio group should advance selection on right arrow.");
        return Task.CompletedTask;
    }

    private static Task Modal_VisibleStateControlsRendering()
    {
        // Arrange
        var hiddenCanvas = new Canvas(30, 10);
        var shownCanvas = new Canvas(30, 10);
        var modal = new Modal
        {
            Title = "Help",
            BodyLines = ["line one", "line two"],
            BackdropFill = ':',
        };

        // Act
        modal.IsVisible = false;
        modal.Render(hiddenCanvas, new Rect(0, 0, 30, 10));
        var hidden = hiddenCanvas.Render();

        modal.IsVisible = true;
        modal.Render(shownCanvas, new Rect(0, 0, 30, 10));
        var shown = shownCanvas.Render();

        // Assert
        TestAssert.True(!hidden.Contains("line one", StringComparison.Ordinal), "Hidden modal should not draw modal content.");
        TestAssert.True(shown.Contains(" Help ", StringComparison.Ordinal), "IsVisible modal should render title.");
        TestAssert.True(shown.Contains("line one", StringComparison.Ordinal), "IsVisible modal should render body lines.");
        TestAssert.True(shown.Contains(':'), "IsVisible modal should apply themed backdrop fill.");
        return Task.CompletedTask;
    }

    private static Task Modal_BackdropOccludesUnderlyingContent()
    {
        // Arrange
        var canvas = new Canvas(40, 12);
        canvas.WriteText(0, 0, "UNDERLAY-TEXT", 40);
        canvas.DrawBox(new Rect(0, 1, 40, 10), "underlay");

        var modal = new Modal
        {
            IsVisible = true,
            Title = "Dialog",
            BodyLines = ["confirm action"],
            BackdropFill = ':',
        };

        // Act
        modal.Render(canvas, new Rect(0, 0, 40, 12));
        var output = canvas.Render();

        // Assert
        TestAssert.True(!output.Contains("UNDERLAY-TEXT", StringComparison.Ordinal), "Modal backdrop should hide pre-rendered base content.");
        TestAssert.True(!output.Contains("underlay", StringComparison.Ordinal), "Modal backdrop should hide underlay frame/title.");
        TestAssert.True(output.Contains(" Dialog ", StringComparison.Ordinal), "Modal title should be rendered above backdrop.");
        return Task.CompletedTask;
    }

    private static Task Modal_BackdropOccludesUnderlyingContent_GraphemeAwareCanvas()
    {
        // Arrange
        var canvas = new Canvas(40, 12, CanvasTextMode.GraphemeAware);
        canvas.WriteText(0, 0, "UNDERLAY-TEXT", 40);
        canvas.DrawBox(new Rect(0, 1, 40, 10), "underlay");

        var modal = new Modal
        {
            IsVisible = true,
            Title = "Dialog",
            BodyLines = ["confirm action"],
            BackdropFill = ':',
        };

        // Act
        modal.Render(canvas, new Rect(0, 0, 40, 12));
        var output = canvas.Render();

        // Assert
        TestAssert.True(!output.Contains("UNDERLAY-TEXT", StringComparison.Ordinal), "Modal backdrop should hide pre-rendered base content on grapheme-aware canvases.");
        TestAssert.True(!output.Contains("underlay", StringComparison.Ordinal), "Modal backdrop should hide underlay frame/title on grapheme-aware canvases.");
        TestAssert.True(output.Contains(" Dialog ", StringComparison.Ordinal), "Modal title should still render on grapheme-aware canvases.");
        TestAssert.True(output.Contains("::", StringComparison.Ordinal), "Modal backdrop should render a solid visible fill.");
        return Task.CompletedTask;
    }
}
