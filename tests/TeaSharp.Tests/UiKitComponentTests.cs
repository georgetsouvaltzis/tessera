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
using UiLayout = TeaSharp.Components.UiKit.Layout;

namespace TeaSharp.Tests;

internal static class UiKitComponentTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("UiKit_Canvas_DrawBox_BorderStyles_RenderExpectedCorners", Canvas_DrawBox_BorderStyles_RenderExpectedCorners);
        yield return new TestCase("UiKit_Layout_SplitVertical_StaysWithinBounds", Layout_SplitVertical_StaysWithinBounds);
        yield return new TestCase("UiKit_Layout_Grid_DistributesRemainderAcrossCells", Layout_Grid_DistributesRemainderAcrossCells);
        yield return new TestCase("UiKit_Widgets_DrawStatusBar_PlacesLeftAndRightText", Widgets_DrawStatusBar_PlacesLeftAndRightText);
        yield return new TestCase("UiKit_Widgets_DrawStatusBar_UsesThemeFill", Widgets_DrawStatusBar_UsesThemeFill);
        yield return new TestCase("UiKit_SortableTableComponent_UpdatesSortAndPaging", SortableTableComponent_UpdatesSortAndPaging);
        yield return new TestCase("UiKit_SortableTableComponent_VirtualizationWindow_RendersSlice", SortableTableComponent_VirtualizationWindow_RendersSlice);
        yield return new TestCase("UiKit_SortableTableComponent_MouseClickSelectsVisibleRow", SortableTableComponent_MouseClickSelectsVisibleRow);
        yield return new TestCase("UiKit_FormComponents_RespondToInput", FormComponents_RespondToInput);
        yield return new TestCase("UiKit_ModalComponent_VisibleStateControlsRendering", ModalComponent_VisibleStateControlsRendering);
        yield return new TestCase("UiKit_ModalComponent_BackdropOccludesUnderlyingContent", ModalComponent_BackdropOccludesUnderlyingContent);
        yield return new TestCase("UiKit_ModalComponent_BackdropOccludesUnderlyingContent_GraphemeAwareCanvas", ModalComponent_BackdropOccludesUnderlyingContent_GraphemeAwareCanvas);
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

    private static Task Layout_SplitVertical_StaysWithinBounds()
    {
        // Arrange
        var rect = new Rect(0, 0, 10, 4);

        // Act
        var (first, second) = UiLayout.SplitVertical(rect, firstWidth: 50, minFirst: 8, minSecond: 8);

        // Assert
        TestAssert.Equal(8, first.Width, "SplitVertical should clamp first segment to a safe width.");
        TestAssert.Equal(2, second.Width, "SplitVertical should preserve remaining width for second segment.");
        TestAssert.Equal(10, first.Width + second.Width, "SplitVertical should keep total width unchanged.");
        return Task.CompletedTask;
    }

    private static Task Layout_Grid_DistributesRemainderAcrossCells()
    {
        // Arrange
        var rect = new Rect(0, 0, 5, 3);

        // Act
        var cells = UiLayout.Grid(rect, rows: 2, columns: 2);

        // Assert
        TestAssert.Equal(4, cells.Length, "Grid should create one cell per row/column combination.");
        TestAssert.Equal(3, cells[0].Width, "Grid should place width remainder in early columns.");
        TestAssert.Equal(2, cells[1].Width, "Grid should keep remaining columns at base width.");
        TestAssert.Equal(2, cells[0].Height, "Grid should place height remainder in early rows.");
        TestAssert.Equal(1, cells[2].Height, "Grid should keep remaining rows at base height.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawStatusBar_PlacesLeftAndRightText()
    {
        // Arrange
        var canvas = new Canvas(24, 1);

        // Act
        UiWidgets.DrawStatusBar(canvas, new Rect(0, 0, 24, 1), "left", "right");
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.StartsWith("left", StringComparison.Ordinal), "Status bar should place left text at row start.");
        TestAssert.True(output.EndsWith("right", StringComparison.Ordinal), "Status bar should align right text to row end.");
        return Task.CompletedTask;
    }

    private static Task Widgets_DrawStatusBar_UsesThemeFill()
    {
        // Arrange
        var canvas = new Canvas(16, 1);

        // Act
        UiWidgets.DrawStatusBar(canvas, new Rect(0, 0, 16, 1), "L", "R", new UiTheme(StatusFill: '.'));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains('.'), "Status bar should use theme fill character.");
        return Task.CompletedTask;
    }

    private static Task SortableTableComponent_UpdatesSortAndPaging()
    {
        // Arrange
        var table = new SortableTableComponent(["Metric", "Value"]) { PageSize = 2, Title = "Sample" };
        table.SetRows(
        [
            ["cpu", "33"],
            ["mem", "60"],
            ["io", "18"],
            ["latency", "22"],
            ["errors", "1"],
        ]);

        // Act
        table.Update(new KeyPressMsg(KeyCode.Character, "]"));
        table.Update(new KeyPressMsg(KeyCode.Character, "c"));
        table.Update(new KeyPressMsg(KeyCode.Character, "s"));
        var canvas = new Canvas(40, 8);
        table.Render(canvas, new Rect(0, 0, 40, 8));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains("p2/3", StringComparison.Ordinal), "Table should update to second page when next-page key is pressed.");
        TestAssert.True(output.Contains("sort:Value desc", StringComparison.Ordinal), "Table should switch sort column and direction from hotkeys.");
        return Task.CompletedTask;
    }

    private static Task SortableTableComponent_VirtualizationWindow_RendersSlice()
    {
        // Arrange
        var table = new SortableTableComponent(["Metric", "Value"])
        {
            EnableVirtualization = true,
            Title = "Virtual",
        };
        table.SetRows(
        [
            ["a", "1"],
            ["b", "2"],
            ["c", "3"],
            ["d", "4"],
            ["e", "5"],
        ]);
        table.SetVirtualWindow(startIndex: 2, windowSize: 2);

        // Act
        var canvas = new Canvas(32, 8);
        table.Render(canvas, new Rect(0, 0, 32, 8));
        var output = canvas.Render();

        // Assert
        TestAssert.True(output.Contains("v3+2", StringComparison.Ordinal), "Virtualized table title should report active window.");
        TestAssert.True(output.Contains('c'), "Virtualized slice should include starting row.");
        TestAssert.True(output.Contains('d'), "Virtualized slice should include following row.");
        return Task.CompletedTask;
    }

    private static Task SortableTableComponent_MouseClickSelectsVisibleRow()
    {
        // Arrange
        var table = new SortableTableComponent(["Metric", "Value"])
        {
            Border = BorderStyle.None,
            Title = "Sample",
        };
        table.SetRows(
        [
            ["cpu", "33"],
            ["mem", "60"],
            ["io", "18"],
        ]);
        var bounds = new Rect(0, 0, 40, 6);

        // Act
        var changed = table.UpdateMouse(new MouseClickMsg(MouseButton.Left, 2, 5), bounds);
        var canvas = new Canvas(40, 6);
        table.Render(canvas, bounds);
        var output = canvas.Render();

        // Assert
        TestAssert.True(changed, "Mouse click on table row should update visible selection.");
        TestAssert.True(output.Contains("› mem", StringComparison.Ordinal), "Clicked row should render with selected marker.");
        return Task.CompletedTask;
    }

    private static Task FormComponents_RespondToInput()
    {
        // Arrange
        var checklist = new CheckboxListComponent();
        checklist.SetItems([("focus", true), ("mouse", false)]);

        var radio = new RadioGroupComponent();
        radio.SetItems(["a", "b", "c"]);

        var select = new SelectComponent();
        select.SetItems(["compact", "cozy"]);

        // Act
        checklist.Update(new KeyPressMsg(KeyCode.Down));
        checklist.Update(new KeyPressMsg(KeyCode.Enter));
        radio.Update(new KeyPressMsg(KeyCode.Right));
        select.Update(new KeyPressMsg(KeyCode.Right));

        // Assert
        TestAssert.True(checklist.Items[1].Checked, "Checklist enter key should toggle selected item.");
        TestAssert.Equal(1, radio.SelectedIndex, "Radio group should advance selection on right arrow.");
        TestAssert.Equal(1, select.SelectedIndex, "Select component should advance selection on right arrow.");
        return Task.CompletedTask;
    }

    private static Task ModalComponent_VisibleStateControlsRendering()
    {
        // Arrange
        var hiddenCanvas = new Canvas(30, 10);
        var shownCanvas = new Canvas(30, 10);
        var modal = new ModalComponent(new ModalOptions(
            Title: "Help",
            BodyLines: ["line one", "line two"],
            Theme: new UiTheme(ModalBackdropFill: ':')));

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

    private static Task ModalComponent_BackdropOccludesUnderlyingContent()
    {
        // Arrange
        var canvas = new Canvas(40, 12);
        canvas.WriteText(0, 0, "UNDERLAY-TEXT", 40);
        canvas.DrawBox(new Rect(0, 1, 40, 10), "underlay");

        var modal = new ModalComponent(new ModalOptions(
            IsVisible: true,
            Title: "Dialog",
            BodyLines: ["confirm action"],
            Theme: new UiTheme(ModalBackdropFill: ':')));

        // Act
        modal.Render(canvas, new Rect(0, 0, 40, 12));
        var output = canvas.Render();

        // Assert
        TestAssert.True(!output.Contains("UNDERLAY-TEXT", StringComparison.Ordinal), "Modal backdrop should hide pre-rendered base content.");
        TestAssert.True(!output.Contains("underlay", StringComparison.Ordinal), "Modal backdrop should hide underlay frame/title.");
        TestAssert.True(output.Contains(" Dialog ", StringComparison.Ordinal), "Modal title should be rendered above backdrop.");
        return Task.CompletedTask;
    }

    private static Task ModalComponent_BackdropOccludesUnderlyingContent_GraphemeAwareCanvas()
    {
        // Arrange
        var canvas = new Canvas(40, 12, CanvasTextMode.GraphemeAware);
        canvas.WriteText(0, 0, "UNDERLAY-TEXT", 40);
        canvas.DrawBox(new Rect(0, 1, 40, 10), "underlay");

        var modal = new ModalComponent(new ModalOptions(
            IsVisible: true,
            Title: "Dialog",
            BodyLines: ["confirm action"],
            Theme: new UiTheme(ModalBackdropFill: ':')));

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
