using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

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
            "InteractionPointerParity_Choice_CustomBorderGlyphAndFocusMarker_RenderStable",
            Choice_CustomBorderGlyphAndFocusMarker_RenderStable);
        yield return new TestCase(
            "InteractionPointerParity_ComboBox_CustomBorderGlyphAndFocusMarker_RenderStable",
            ComboBox_CustomBorderGlyphAndFocusMarker_RenderStable);
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
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(220, 121, 20));
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
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(200, 80, 40));
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

    private static Task Choice_CustomBorderGlyphAndFocusMarker_RenderStable()
    {
        var focusedBorder = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(180, 120, 30));
        var control = new Choice
        {
            Border = BorderStyle.SingleLine,
            Title = "Choice",
            IsFocused = true,
            FocusMarker = "◆",
            ShowFocusMarker = true,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 40, 40)),
            FocusedBorderStyleText = focusedBorder,
            Glyphs = new DropdownGlyphSet("v", "^", ">", "+"),
        };
        control.SetItems(["alpha", "beta"]);
        _ = control.Handle(new KeyPressed(Key.Down));

        var first = Render(control, 40, 8, CanvasTextMode.GraphemeAware);
        var second = Render(control, 40, 8, CanvasTextMode.GraphemeAware);

        TestAssert.True(first.Contains("Choice ◆", StringComparison.Ordinal), "Choice title should render custom focus marker.");
        TestAssert.True(first.Contains("^ alpha", StringComparison.Ordinal), "Choice field should render custom expanded indicator.");
        TestAssert.True(first.Contains(">+ alpha", StringComparison.Ordinal), "Choice options should render custom highlighted and selected markers.");
        TestAssert.True(first.Contains(focusedBorder.Render("┌"), StringComparison.Ordinal), "Choice focused border style should render on border glyphs.");
        TestAssert.Equal(first, second, "Choice custom border/glyph render should be deterministic.");
        return Task.CompletedTask;
    }

    private static Task ComboBox_CustomBorderGlyphAndFocusMarker_RenderStable()
    {
        var focusedBorder = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(120, 180, 50));
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(210, 90, 70));
        var control = new ComboBox
        {
            Border = BorderStyle.SingleLine,
            Title = "Combo",
            IsFocused = true,
            FocusMarker = "◆",
            ShowFocusMarker = true,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 40, 40)),
            FocusedBorderStyleText = focusedBorder,
            HoveredValueStyle = hoverStyle,
            Glyphs = new DropdownGlyphSet("v", "^", ">", "*"),
        };
        control.SetItems(["alpha", "beta"]);
        _ = control.Handle(new KeyPressed(Key.Down));
        _ = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Down));
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), new Rect(0, 0, 40, 8));

        var first = Render(control, 40, 8, CanvasTextMode.GraphemeAware);
        var second = Render(control, 40, 8, CanvasTextMode.GraphemeAware);

        TestAssert.True(first.Contains("Combo ◆", StringComparison.Ordinal), "ComboBox title should render custom focus marker.");
        TestAssert.True(first.Contains('^'), "ComboBox field should render custom expanded indicator.");
        TestAssert.True(first.Contains(">* alpha", StringComparison.Ordinal), "ComboBox options should render custom highlighted and selected markers.");
        TestAssert.True(first.Contains(focusedBorder.Render("┌"), StringComparison.Ordinal), "ComboBox focused border style should render on border glyphs.");
        TestAssert.True(first.Contains(hoverStyle.ToEscapeSequence(), StringComparison.Ordinal), "ComboBox hovered field style should render SGR sequence.");
        TestAssert.Equal(first, second, "ComboBox custom border/glyph render should be deterministic.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_HoveredRowStyle_IsRendered()
    {
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(180, 30, 210));
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
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(70, 140, 230));
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
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 210, 130));
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
        var hoverStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(240, 180, 30));
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

    private static string Render(Control control, int width, int height, CanvasTextMode mode = CanvasTextMode.Fast)
    {
        var canvas = new Canvas(width, height, mode);
        var bounds = new Rect(0, 0, width, height);
        control.Render(canvas, bounds);
        return canvas.Render();
    }
}
