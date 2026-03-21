using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class PaletteEditorControlTests
{
    [Test]
    public void Controls_PaletteEditor_RendersGridPreviewAndHex()
    {
        var control = new PaletteEditor
        {
            Title = string.Empty,
            ColumnCount = 2,
            ShowHexCode = true,
            ShowPreviewBlock = true,
        };
        control.SetSwatches(
        [
            new PaletteSwatch("Mauve", "#CBA6F7"),
            new PaletteSwatch("Blue", "#89B4FA"),
            new PaletteSwatch("Teal", "#94E2D5"),
            new PaletteSwatch("Peach", "#FAB387"),
        ]);

        var output = Render(control, width: 64, height: 4);

        Assert.That(output.Contains("Mauve #CBA6F7", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Blue #89B4FA", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Teal #94E2D5", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("██", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_PaletteEditor_KeyboardAndPointerSelection_RaisesSelectionChanged()
    {
        var control = new PaletteEditor
        {
            Title = string.Empty,
            ColumnCount = 2,
            IsFocused = true,
        };
        control.SetSwatches(CreateSwatches());

        var raised = 0;
        PaletteSelectionChangedEventArgs? latest = null;
        control.SelectionChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var clickHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 40, 0),
            new Rect(0, 0, 64, 4));

        Assert.That(downHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(1));
        Assert.That(raised, Is.GreaterThanOrEqualTo(2));
        Assert.That(latest?.SelectedSwatch?.Name, Is.EqualTo("Blue"));
    }

    [Test]
    public void Controls_PaletteEditor_StyleHooks_RenderAnsi()
    {
        var control = new PaletteEditor
        {
            Title = string.Empty,
            IsFocused = true,
            ColumnCount = 2,
            SwatchStyle = TeaStyle.Empty.WithItalic(),
            HoveredSwatchStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(7, 8, 9)),
            SelectedSwatchStyle = TeaStyle.Empty.WithBold(),
            FocusedSelectedSwatchStyle = TeaStyle.Empty.WithUnderline(),
            PreviewSwatchStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 22, 33)),
        };
        control.SetSwatches(CreateSwatches());
        _ = control.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 40, 0),
            new Rect(0, 0, 64, 4));

        var output = Render(control, width: 64, height: 4);
        Assert.That(output.Contains("38;2;11;22;33", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;7;8;9", StringComparison.Ordinal), Is.True);
        Assert.That(
            output.Contains("[1;", StringComparison.Ordinal)
            || output.Contains(";1;", StringComparison.Ordinal)
            || output.Contains("[1m", StringComparison.Ordinal),
            Is.True);
        Assert.That(output.Contains(";4;", StringComparison.Ordinal) || output.Contains("[4m", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_PaletteEditor_DefaultRender_IsDeterministicAndMonochrome()
    {
        var control = new PaletteEditor
        {
            Title = string.Empty,
            ColumnCount = 2,
        };
        control.SetSwatches(CreateSwatches());

        var first = Render(control, width: 64, height: 4);
        var second = Render(control, width: 64, height: 4);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    [Test]
    public void Controls_PaletteEditor_EmptyState_RendersConfiguredText()
    {
        var control = new PaletteEditor
        {
            Title = string.Empty,
            EmptyText = "(palette empty)",
        };

        var output = Render(control, width: 40, height: 2);

        Assert.That(output.Contains("(palette empty)", StringComparison.Ordinal), Is.True);
    }

    private static IReadOnlyList<PaletteSwatch> CreateSwatches()
    {
        return
        [
            new PaletteSwatch("Mauve", "#CBA6F7"),
            new PaletteSwatch("Blue", "#89B4FA"),
            new PaletteSwatch("Teal", "#94E2D5"),
            new PaletteSwatch("Peach", "#FAB387"),
        ];
    }

    private static string Render(PaletteEditor control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
