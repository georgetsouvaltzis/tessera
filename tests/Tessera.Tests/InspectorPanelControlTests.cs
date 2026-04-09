using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class InspectorPanelControlTests
{
    [Test]
    public void ControlsInspectorPanelRendersSectionsAndRows()
    {
        var general = new InspectorSection("General", isExpanded: true);
        general.AddField("Name", "Tessera");
        general.AddDetail("interactive mode");
        var metrics = new InspectorSection("Metrics", isExpanded: false);
        metrics.AddField("Rows", "24");

        var control = new InspectorPanel
        {
            Border = BorderStyle.None,
        };
        control.SetSections([general, metrics]);

        var output = Render(control, 50, 8);

        TestAssert.True(output.Contains("▾ General", StringComparison.Ordinal), "Expanded section should render expanded marker.");
        TestAssert.True(output.Contains("Name", StringComparison.Ordinal), "Field key should render.");
        TestAssert.True(output.Contains("Tessera", StringComparison.Ordinal), "Field value should render.");
        TestAssert.True(output.Contains("interactive mode", StringComparison.Ordinal), "Detail row should render.");
        TestAssert.True(output.Contains("▸ Metrics", StringComparison.Ordinal), "Collapsed section should render collapsed marker.");
    }

    [Test]
    public void ControlsInspectorPanelKeyboardNavigationAndToggleSection()
    {
        var section = new InspectorSection("Build", isExpanded: false);
        section.AddField("Target", "Release");
        var control = new InspectorPanel
        {
            Border = BorderStyle.None,
            IsFocused = true,
        };
        control.SetSections([section]);

        var toggledOpen = control.Handle(new KeyPressed(Key.Enter));
        var movedToField = control.Handle(new KeyPressed(Key.Down));
        var toggledClosed = control.Handle(new KeyPressed(Key.Left));
        var output = Render(control, 48, 6);

        TestAssert.True(toggledOpen, "Enter on section header should toggle section.");
        TestAssert.True(movedToField, "Down should move selection to first field row.");
        TestAssert.True(toggledClosed, "Left on field row should collapse parent section.");
        TestAssert.Equal(0, control.SelectedRowIndex, "Selection should clamp to section header after collapse.");
        TestAssert.True(!output.Contains("Target", StringComparison.Ordinal), "Collapsed section should hide field rows.");
    }

    [Test]
    public void ControlsInspectorPanelPointerSelectionAndToggleSection()
    {
        var section = new InspectorSection("Env", isExpanded: false);
        section.AddField("Region", "eu-west");
        var control = new InspectorPanel
        {
            Border = BorderStyle.None,
        };
        control.SetSections([section]);
        var bounds = new Rect(0, 0, 40, 6);

        var headerPress = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 1, Y: 0), bounds);
        var rowPress = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 2, Y: 1), bounds);

        TestAssert.True(headerPress, "Header press should toggle section.");
        TestAssert.True(rowPress, "Press on first field row should select it.");
        TestAssert.Equal(1, control.SelectedRowIndex, "Pointer should select field row after expansion.");
    }

    [Test]
    public void ControlsInspectorPanelFocusedStylesEmitAnsi()
    {
        var section = new InspectorSection("Runtime", isExpanded: true);
        section.AddField("Status", "ok");
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(111, 101, 91));
        var selectedStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(7, 17, 27));
        var control = new InspectorPanel
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            FocusedBorderStyleText = borderStyle,
            SelectedRowStyle = selectedStyle,
        };
        control.SetSections([section]);

        var output = Render(control, 52, 8, CanvasTextMode.GraphemeAware);

        TestAssert.True(output.Contains(borderStyle.Render("┌"), StringComparison.Ordinal), "Focused border style should apply to border glyphs.");
        TestAssert.True(output.Contains("48;2;7;17;27", StringComparison.Ordinal), "Selected row style should emit ANSI background sequence.");
    }

    [Test]
    public void ControlsInspectorPanelDefaultRenderIsDeterministicAndMonochrome()
    {
        var section = new InspectorSection("Config", isExpanded: true);
        section.AddField("Mode", "safe");
        var control = new InspectorPanel
        {
            Border = BorderStyle.None,
        };
        control.SetSections([section]);

        var first = Render(control, 44, 6);
        var second = Render(control, 44, 6);

        TestAssert.Equal(first, second, "InspectorPanel should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default InspectorPanel output should remain monochrome.");
    }

    private static string Render(InspectorPanel control, int width, int height, CanvasTextMode mode = CanvasTextMode.Fast)
    {
        var canvas = new Canvas(width, height, mode);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
