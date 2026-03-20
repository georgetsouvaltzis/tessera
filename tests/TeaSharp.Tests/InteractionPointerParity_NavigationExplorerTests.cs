using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class InteractionPointerParity_NavigationExplorerTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "InteractionPointerParity_Choice_FieldHoverStyle_IsRendered",
            Choice_FieldHoverStyle_IsRendered);
        yield return new TestCase(
            "InteractionPointerParity_ComboBox_FieldHoverStyle_IsRendered",
            ComboBox_FieldHoverStyle_IsRendered);
        yield return new TestCase(
            "InteractionPointerParity_FuzzyFinder_HoveredRowStyle_IsRendered",
            FuzzyFinder_HoveredRowStyle_IsRendered);
        yield return new TestCase(
            "InteractionPointerParity_FileExplorer_HoveredRowStyle_IsRendered",
            FileExplorer_HoveredRowStyle_IsRendered);
        yield return new TestCase(
            "InteractionPointerParity_TreeTable_HoveredRowStyle_IsRendered",
            TreeTable_HoveredRowStyle_IsRendered);
        yield return new TestCase(
            "InteractionPointerParity_Table_HoveredRowStyle_IsRendered",
            Table_HoveredRowStyle_IsRendered);
    }

    private static Task Choice_FieldHoverStyle_IsRendered()
    {
        var hoverStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(220, 121, 20));
        var control = new Choice
        {
            Border = BorderStyle.None,
            HoveredValueStyle = hoverStyle,
        };
        control.SetItems(["alpha", "beta"]);

        var bounds = new Rect(0, 0, 32, 6);
        var changed = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 0, 0), bounds);
        var canvas = new Canvas(32, 6);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(changed, "Choice pointer motion over field should update hover state.");
        TestAssert.True(output.Contains('▾'), "Choice should render the field indicator.");
        TestAssert.True(output.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "Choice field hover style should render SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task ComboBox_FieldHoverStyle_IsRendered()
    {
        var hoverStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(200, 80, 40));
        var control = new ComboBox
        {
            Border = BorderStyle.None,
            HoveredValueStyle = hoverStyle,
        };
        control.SetItems(["alpha", "beta"]);

        var bounds = new Rect(0, 0, 32, 6);
        var changed = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 0, 0), bounds);
        var canvas = new Canvas(32, 6);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(changed, "ComboBox pointer motion over field should update hover state.");
        TestAssert.True(output.Contains('▾'), "ComboBox should render the field indicator.");
        TestAssert.True(output.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "ComboBox field hover style should render SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_HoveredRowStyle_IsRendered()
    {
        var hoverStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(180, 30, 210));
        var control = new FuzzyFinder
        {
            Border = BorderStyle.None,
            HoveredItemStyle = hoverStyle,
        };
        control.SetItems(["one", "two", "three"]);

        var bounds = new Rect(0, 0, 40, 6);
        var changed = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2), bounds);
        var canvas = new Canvas(40, 6);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(changed, "FuzzyFinder pointer motion should update hovered row.");
        TestAssert.True(output.Contains("two", StringComparison.Ordinal), "FuzzyFinder should render hovered row label.");
        TestAssert.True(output.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "FuzzyFinder hovered row style should render SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task FileExplorer_HoveredRowStyle_IsRendered()
    {
        var hoverStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(70, 140, 230));
        var control = new FileExplorer
        {
            Border = BorderStyle.None,
            HoveredStyle = hoverStyle,
        };
        control.SetItems(
        [
            new FileExplorerItem("src", isDirectory: true, path: "/src"),
            new FileExplorerItem("README.md", isDirectory: false, path: "/README.md"),
        ]);

        var bounds = new Rect(0, 0, 48, 6);
        var changed = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), bounds);
        var canvas = new Canvas(48, 6);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(changed, "FileExplorer pointer motion should update hovered row.");
        TestAssert.True(output.Contains("README.md", StringComparison.Ordinal), "FileExplorer should render hovered row label.");
        TestAssert.True(output.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "FileExplorer hovered row style should render SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task TreeTable_HoveredRowStyle_IsRendered()
    {
        var hoverStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(90, 210, 130));
        var control = new TreeTable("Name")
        {
            Border = BorderStyle.None,
            HoveredRowStyle = hoverStyle,
        };
        control.SetItems(
        [
            new TreeTableNode("alpha", "alpha"),
            new TreeTableNode("beta", "beta"),
        ]);

        var bounds = new Rect(0, 0, 40, 6);
        var changed = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 2), bounds);
        var canvas = new Canvas(40, 6);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(changed, "TreeTable pointer motion should update hovered row.");
        TestAssert.True(output.Contains("beta", StringComparison.Ordinal), "TreeTable should render hovered row label.");
        TestAssert.True(output.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "TreeTable hovered row style should render SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task Table_HoveredRowStyle_IsRendered()
    {
        var hoverStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(240, 180, 30));
        var control = new Table("Name", "State")
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            HoveredRowStyle = hoverStyle,
        };
        control.SetRows(
        [
            ["svc-a", "ok"],
            ["svc-b", "warn"],
        ]);

        var bounds = new Rect(0, 0, 48, 7);
        var changed = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 3), bounds);
        var canvas = new Canvas(48, 7);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(changed, "Table pointer motion should update hovered row.");
        TestAssert.True(output.Contains("svc-b", StringComparison.Ordinal), "Table should render hovered row label.");
        TestAssert.True(output.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "Table hovered row style should render SGR sequence.");
        return Task.CompletedTask;
    }
}
