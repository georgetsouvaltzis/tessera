using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class BorderedFlowDataRenderTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "Controls_Dialog_FocusedBorderStyleText_StylesFrameGlyphs",
            Dialog_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase(
            "Controls_Modal_FocusedBorderStyleText_StylesFrameGlyphs",
            Modal_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase(
            "Controls_KeyValueList_FocusedBorderStyleText_StylesFrameGlyphs",
            KeyValueList_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase(
            "Controls_Timeline_FocusedBorderStyleText_StylesFrameGlyphs",
            Timeline_FocusedBorderStyleText_StylesFrameGlyphs);
    }

    private static Task Dialog_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(12, 24, 36));
        var dialog = new Dialog
        {
            IsVisible = true,
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
            BodyLines = ["Are you sure?"],
        };

        var canvas = new Canvas(48, 14, CanvasTextMode.GraphemeAware);
        dialog.Render(canvas, new Rect(0, 0, 48, 14));
        var output = canvas.Render();

        var merged = borderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(merged.Render("┌"), StringComparison.Ordinal), "Dialog should render focused border glyphs with merged border styles.");
        return Task.CompletedTask;
    }

    private static Task Modal_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 55, 66));
        var modal = new Modal
        {
            IsVisible = true,
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
            BodyLines = ["Line A"],
        };

        var canvas = new Canvas(48, 14, CanvasTextMode.GraphemeAware);
        modal.Render(canvas, new Rect(0, 0, 48, 14));
        var output = canvas.Render();

        var merged = borderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(merged.Render("┌"), StringComparison.Ordinal), "Modal should render focused border glyphs with merged border styles.");
        return Task.CompletedTask;
    }

    private static Task KeyValueList_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(77, 88, 99));
        var list = new KeyValueList
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
        };
        list.SetEntries(
        [
            new KeyValueListEntry("Host", "localhost"),
            new KeyValueListEntry("Port", "5432"),
        ]);

        var canvas = new Canvas(48, 8, CanvasTextMode.GraphemeAware);
        list.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        var merged = borderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(merged.Render("┌"), StringComparison.Ordinal), "KeyValueList should render focused border glyphs with merged border styles.");
        return Task.CompletedTask;
    }

    private static Task Timeline_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var borderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(123, 98, 76));
        var timeline = new Timeline
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
        };
        timeline.SetEntries(
        [
            new TimelineEntry("a", "Started", "09:00"),
            new TimelineEntry("b", "Running", "09:05"),
        ]);

        var canvas = new Canvas(48, 8, CanvasTextMode.GraphemeAware);
        timeline.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        var merged = borderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(merged.Render("┌"), StringComparison.Ordinal), "Timeline should render focused border glyphs with merged border styles.");
        return Task.CompletedTask;
    }
}
