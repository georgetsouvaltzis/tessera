using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class InteractionPointerParityDataInputTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "Controls_PointerParity_DataInput_MultiSelect_MouseClickSelectsAndToggles",
            MultiSelectMouseClickSelectsAndToggles);
        yield return new TestCase(
            "Controls_PointerParity_DataInput_MultiSelect_MouseHoverAppliesHoveredStyle",
            MultiSelectMouseHoverAppliesHoveredStyle);
        yield return new TestCase(
            "Controls_PointerParity_DataInput_RadioGroup_MouseClickSelectsAndRaisesEvent",
            RadioGroupMouseClickSelectsAndRaisesEvent);
        yield return new TestCase(
            "Controls_PointerParity_DataInput_RadioGroup_MouseHoverAppliesHoveredStyle",
            RadioGroupMouseHoverAppliesHoveredStyle);
        yield return new TestCase(
            "Controls_PointerParity_DataInput_DataGrid_MouseHoverAppliesHoveredRowAndCellStyles",
            DataGridMouseHoverAppliesHoveredRowAndCellStyles);
    }

    private static Task MultiSelectMouseClickSelectsAndToggles()
    {
        var control = new MultiSelect { IsFocused = true };
        control.SetItems(["alpha", "beta", "gamma"]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            new Rect(0, 0, 28, 6));

        TestAssert.True(handled, "Mouse click should be handled.");
        TestAssert.Equal(1, control.SelectedIndex, "Mouse click should select clicked row.");
        TestAssert.True(control.CheckedItems.Contains("beta", StringComparer.Ordinal),
            "Mouse click should toggle clicked checklist row.");
        return Task.CompletedTask;
    }

    private static Task MultiSelectMouseHoverAppliesHoveredStyle()
    {
        var hoveredStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(17, 34, 51));
        var control = new MultiSelect { HoveredItemStyle = hoveredStyle };
        control.SetItems(["alpha", "beta", "gamma"]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2),
            new Rect(0, 0, 28, 6));
        var canvas = new Canvas(28, 6, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, 28, 6));
        var output = canvas.Render();

        TestAssert.True(handled, "Mouse motion should update hover state.");
        TestAssert.True(output.Contains("38;2;17;34;51", StringComparison.Ordinal),
            "Hovered row should apply configured hovered style.");
        return Task.CompletedTask;
    }

    private static Task RadioGroupMouseClickSelectsAndRaisesEvent()
    {
        var control = new RadioGroup { IsFocused = true };
        control.SetItems(["low", "high"]);
        SelectionChangedEventArgs? args = null;
        control.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            new Rect(0, 0, 28, 5));

        TestAssert.True(handled, "Mouse click should be handled.");
        TestAssert.Equal(1, control.SelectedIndex, "Mouse click should select clicked radio row.");
        TestAssert.Equal("high", control.SelectedItem, "Mouse click should expose selected item.");
        TestAssert.True(args is not null, "Mouse click should raise selection event on change.");
        TestAssert.Equal(0, TestAssert.NotNull(args).PreviousIndex, "Selection event should expose previous index.");
        TestAssert.Equal(1, args.SelectedIndex, "Selection event should expose new index.");
        return Task.CompletedTask;
    }

    private static Task RadioGroupMouseHoverAppliesHoveredStyle()
    {
        var hoveredStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(61, 92, 123));
        var control = new RadioGroup { HoveredItemStyle = hoveredStyle };
        control.SetItems(["low", "high"]);

        var handled = control.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2),
            new Rect(0, 0, 28, 5));
        var canvas = new Canvas(28, 5, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, 28, 5));
        var output = canvas.Render();

        TestAssert.True(handled, "Mouse motion should update hover state.");
        TestAssert.True(output.Contains("38;2;61;92;123", StringComparison.Ordinal),
            "Hovered row should apply configured hovered style.");
        return Task.CompletedTask;
    }

    private static Task DataGridMouseHoverAppliesHoveredRowAndCellStyles()
    {
        var hoveredRowStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(10, 20, 30));
        var hoveredCellStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60));
        var grid = new DataGrid
        {
            Border = BorderStyle.None,
            HoveredRowStyle = hoveredRowStyle,
            HoveredCellStyle = hoveredCellStyle
        };
        grid.SetColumns(
        [
            new DataGridColumn("name", "Name"),
            new DataGridColumn("state", "State")
        ]);
        grid.SetRows(
        [
            ["alpha", "open"],
            ["beta", "done"]
        ]);

        var handled = grid.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2),
            new Rect(0, 0, 32, 4));
        var canvas = new Canvas(32, 4, CanvasTextMode.GraphemeAware);
        grid.Render(canvas, new Rect(0, 0, 32, 4));
        var output = canvas.Render();

        TestAssert.True(handled, "Mouse motion should update DataGrid hover state.");
        TestAssert.True(output.Contains("48;2;10;20;30", StringComparison.Ordinal),
            "Hovered row style should be rendered.");
        TestAssert.True(output.Contains("38;2;40;50;60", StringComparison.Ordinal),
            "Hovered cell style should be rendered.");
        return Task.CompletedTask;
    }
}

[TestFixture]
[NonParallelizable]
public sealed class InteractionPointerParityDataInputNUnitAdapter
{
    public static IEnumerable<TestCaseData> Cases()
    {
        foreach (var testCase in InteractionPointerParityDataInputTests.Cases())
        {
            yield return new TestCaseData(testCase).SetName(testCase.Name);
        }
    }

    [TestCaseSource(nameof(Cases))]
    public async Task Execute(TestCase testCase)
    {
        Assert.That(testCase, Is.Not.Null);
        await testCase.Execute();
    }
}
