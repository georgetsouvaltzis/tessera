using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class KanbanBoardControlTests
{
    [Test]
    public void ControlsKanbanBoardRendersLanesAndSelectedCardMarker()
    {
        var todo = new KanbanLane("Todo");
        todo.AddCard(new KanbanCard("Investigate"));
        todo.AddCard(new KanbanCard("Patch"));
        var done = new KanbanLane("Done");
        done.AddCard(new KanbanCard("Deploy"));

        var control = new KanbanBoard
        {
            Title = "Ops",
        };
        control.SetLanes([todo, done]);
        var canvas = new Canvas(48, 10);

        control.Render(canvas, new Rect(0, 0, 48, 10));
        var output = canvas.Render();

        TestAssert.True(output.Contains(" Ops ", StringComparison.Ordinal), "Kanban board should render title.");
        TestAssert.True(output.Contains("Todo", StringComparison.Ordinal), "Kanban board should render first lane header.");
        TestAssert.True(output.Contains("Done", StringComparison.Ordinal), "Kanban board should render second lane header.");
        TestAssert.True(output.Contains('▸'), "Kanban board should render selected card marker.");
    }

    [Test]
    public void ControlsKanbanBoardKeyboardNavigationUpdatesSelectionAndRaisesEvent()
    {
        var todo = new KanbanLane("Todo");
        todo.AddCard(new KanbanCard("A"));
        todo.AddCard(new KanbanCard("B"));
        var doing = new KanbanLane("Doing");
        doing.AddCard(new KanbanCard("C"));
        doing.AddCard(new KanbanCard("D"));

        var control = new KanbanBoard
        {
            IsFocused = true,
        };
        control.SetLanes([todo, doing]);
        var changes = 0;
        control.SelectionChanged += (_, _) => changes++;

        var rightHandled = control.Handle(new KeyPressed(Key.Right));
        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var leftHandled = control.Handle(new KeyPressed(Key.Left));

        TestAssert.True(rightHandled, "Right key should move lane selection.");
        TestAssert.True(downHandled, "Down key should move card selection.");
        TestAssert.True(leftHandled, "Left key should move lane selection back.");
        TestAssert.Equal(0, control.SelectedLaneIndex, "Lane selection should return to first lane.");
        TestAssert.Equal(1, control.SelectedCardIndex, "Card index should persist when switching lanes.");
        TestAssert.Equal(3, changes, "Each move should raise selection changed event.");
    }

    [Test]
    public void ControlsKanbanBoardPointerHoverAndStateStylesRenderExpectedAnsi()
    {
        var lane = new KanbanLane("Todo");
        lane.AddCard(new KanbanCard("A"));
        lane.AddCard(new KanbanCard("B")
        {
            IsDisabled = true,
            HasError = true,
        });

        var control = new KanbanBoard
        {
            IsFocused = true,
            Border = BorderStyle.None,
            SelectedCardStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(1, 2, 3)),
            FocusedCardStyle = TesseraStyle.Empty.WithItalic(),
            HoveredCardStyle = TesseraStyle.Empty.WithUnderline(),
            DisabledCardStyle = TesseraStyle.Empty.WithDim(),
            ErrorCardStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 91, 92)),
        };
        control.SetLanes([lane]);
        var bounds = new Rect(0, 0, 36, 8);

        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 2), bounds);
        _ = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 2), bounds);
        var canvas = new Canvas(36, 8, CanvasTextMode.GraphemeAware);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.Equal(1, control.SelectedCardIndex, "Pointer press should select hovered card.");
        TestAssert.True(output.Contains("48;2;1;2;3", StringComparison.Ordinal), "Selected card style should be rendered.");
        TestAssert.True(output.Contains("38;2;90;91;92", StringComparison.Ordinal), "Error card style should be rendered.");
        TestAssert.True(
            output.Contains(";4;", StringComparison.Ordinal) || output.Contains("[4m", StringComparison.Ordinal),
            "Hovered card style should be rendered.");
    }

    [Test]
    public void ControlsKanbanBoardDefaultRenderIsDeterministicAndMonochrome()
    {
        var lane = new KanbanLane("Lane");
        lane.AddCard(new KanbanCard("One"));
        lane.AddCard(new KanbanCard("Two"));

        var control = new KanbanBoard();
        control.SetLanes([lane]);
        var bounds = new Rect(0, 0, 32, 8);
        var firstCanvas = new Canvas(32, 8);
        var secondCanvas = new Canvas(32, 8);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "Kanban board render should be deterministic.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default Kanban board should render monochrome output.");
    }
}
