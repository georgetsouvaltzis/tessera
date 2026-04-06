using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class ControlStyleHooksTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "Controls_StatusBar_StyleHooks_EmitSgrFragments",
            StatusBar_StyleHooks_EmitSgrFragments);
        yield return new TestCase(
            "Controls_ListView_StyleHooks_EmitSgrFragments",
            ListView_StyleHooks_EmitSgrFragments);
        yield return new TestCase(
            "Controls_ListView_TitleMarkerAndStyleOverrides_EmitExpectedFragments",
            ListView_TitleMarkerAndStyleOverrides_EmitExpectedFragments);
        yield return new TestCase(
            "Controls_Button_StyleHooks_EmitSgrFragments",
            Button_StyleHooks_EmitSgrFragments);
        yield return new TestCase(
            "Controls_TextInput_StyleHooks_EmitSgrFragments",
            TextInput_StyleHooks_EmitSgrFragments);
        yield return new TestCase(
            "Controls_TextInput_TitleMarkerAndStyleOverrides_EmitExpectedFragments",
            TextInput_TitleMarkerAndStyleOverrides_EmitExpectedFragments);
        yield return new TestCase(
            "Controls_Table_TitleMarkerAndStyleOverrides_EmitExpectedFragments",
            Table_TitleMarkerAndStyleOverrides_EmitExpectedFragments);
        yield return new TestCase(
            "Controls_Tabs_TitleMarkerAndStyleOverrides_EmitExpectedFragments",
            Tabs_TitleMarkerAndStyleOverrides_EmitExpectedFragments);
    }

    private static Task StatusBar_StyleHooks_EmitSgrFragments()
    {
        var statusBar = new StatusBar
        {
            Fill = '.',
            LeftText = "left",
            RightText = "right",
            FillStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            LeftTextStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
            RightTextStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightYellow),
        };

        var output = Render(statusBar, width: 30, height: 1);

        AssertContains(output, "\u001b[38;5;12m");
        AssertContains(output, "\u001b[1;38;5;10m");
        AssertContains(output, "\u001b[4;38;5;11m");
        return Task.CompletedTask;
    }

    private static Task ListView_StyleHooks_EmitSgrFragments()
    {
        var list = new ListView<string>(static value => value)
        {
            Border = BorderStyle.None,
            DefaultRowStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            HoveredRowStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightYellow),
            SelectedRowStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen),
        };

        list.SetItems(["alpha", "beta", "gamma"]);
        list.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, X: 1, Y: 1),
            new Rect(0, 0, 24, 3));

        var output = Render(list, width: 24, height: 3);

        AssertContains(output, "\u001b[1;38;5;10m");
        AssertContains(output, "\u001b[4;38;5;11m");
        AssertContains(output, "\u001b[38;5;14m");
        return Task.CompletedTask;
    }

    private static Task ListView_TitleMarkerAndStyleOverrides_EmitExpectedFragments()
    {
        var list = new ListView<string>(static value => value)
        {
            Title = "Projects",
            Border = BorderStyle.SingleLine,
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
        };
        list.SetItems(["alpha"]);

        var focused = Render(list, width: 32, height: 4);
        AssertContains(focused, "Projects !");
        AssertContains(focused, "\u001b[4;38;5;13m");

        list.ShowFocusMarker = false;
        var withoutMarker = Render(list, width: 32, height: 4);
        AssertNotContains(withoutMarker, "Projects !");

        list.IsFocused = false;
        var unfocused = Render(list, width: 32, height: 4);
        AssertContains(unfocused, "\u001b[38;5;14m");
        return Task.CompletedTask;
    }

    private static Task Button_StyleHooks_EmitSgrFragments()
    {
        var button = new Button
        {
            Text = "Run",
            LabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            FocusedLabelStyle = TeaStyle.Empty.WithUnderline(),
            DisabledLabelStyle = TeaStyle.Empty.WithDim(),
            PressedLabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightMagenta),
        };

        button.IsFocused = true;
        var focused = Render(button, width: 16, height: 1);
        AssertContains(focused, "\u001b[4;38;5;14m");

        button.IsFocused = false;
        button.IsDisabled = true;
        var disabled = Render(button, width: 16, height: 1);
        AssertContains(disabled, "\u001b[2;38;5;14m");

        button.IsDisabled = false;
        button.IsFocused = true;
        _ = button.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 1, Y: 0),
            new Rect(0, 0, 16, 1));

        var pressed = Render(button, width: 16, height: 1);
        AssertContains(pressed, "\u001b[4;38;5;13m");
        return Task.CompletedTask;
    }

    private static Task TextInput_StyleHooks_EmitSgrFragments()
    {
        var input = new TextInput
        {
            Border = BorderStyle.SingleLine,
            IsFocused = true,
            Placeholder = "name",
            ValueTextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            PlaceholderTextStyle = TeaStyle.Empty.WithDim().WithForeground(AnsiColor.BrightYellow),
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
        };

        var placeholder = Render(input, width: 22, height: 3);
        AssertContains(placeholder, "\u001b[4;38;5;13m");
        AssertContains(placeholder, "\u001b[2;38;5;11m");

        input.SetValue("abc");
        var value = Render(input, width: 22, height: 3);
        AssertContains(value, "\u001b[38;5;10m");
        return Task.CompletedTask;
    }

    private static Task TextInput_TitleMarkerAndStyleOverrides_EmitExpectedFragments()
    {
        var input = new TextInput
        {
            Border = BorderStyle.SingleLine,
            IsFocused = true,
            Title = "Search",
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
        };

        var focused = Render(input, width: 24, height: 3);
        AssertContains(focused, "Search !");
        AssertContains(focused, "\u001b[4;38;5;13m");

        input.ShowFocusMarker = false;
        var withoutMarker = Render(input, width: 24, height: 3);
        AssertNotContains(withoutMarker, "Search !");

        input.IsFocused = false;
        var unfocused = Render(input, width: 24, height: 3);
        AssertContains(unfocused, "\u001b[38;5;14m");
        return Task.CompletedTask;
    }

    private static Task Table_TitleMarkerAndStyleOverrides_EmitExpectedFragments()
    {
        var table = new Table("Name", "Status")
        {
            Title = "Tasks",
            Border = BorderStyle.SingleLine,
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
        };
        table.SetRows(
        [
            ["A", "Open"],
            ["B", "Done"],
        ]);

        var focused = Render(table, width: 48, height: 8);
        AssertContains(focused, "Tasks !");
        AssertContains(focused, "\u001b[4;38;5;13m");

        table.ShowFocusMarker = false;
        var withoutMarker = Render(table, width: 48, height: 8);
        AssertNotContains(withoutMarker, "Tasks !");

        table.IsFocused = false;
        var unfocused = Render(table, width: 48, height: 8);
        AssertContains(unfocused, "\u001b[38;5;14m");
        return Task.CompletedTask;
    }

    private static Task Tabs_TitleMarkerAndStyleOverrides_EmitExpectedFragments()
    {
        var tabs = new Tabs("Home", "Build", "Deploy")
        {
            IsFocused = true,
            Title = "Main",
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            TitleStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
        };

        var focused = Render(tabs, width: 48, height: 1);
        AssertContains(focused, "Main !");
        AssertContains(focused, "\u001b[4;38;5;13m");

        tabs.ShowFocusMarker = false;
        var withoutMarker = Render(tabs, width: 48, height: 1);
        AssertNotContains(withoutMarker, "Main !");

        tabs.IsFocused = false;
        var unfocused = Render(tabs, width: 48, height: 1);
        AssertContains(unfocused, "\u001b[38;5;14m");
        return Task.CompletedTask;
    }

    private static string Render(Control control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private static void AssertContains(string actual, string expectedFragment)
    {
        if (!actual.Contains(expectedFragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Expected output to contain '{Escape(expectedFragment)}'.");
        }
    }

    private static void AssertNotContains(string actual, string unexpectedFragment)
    {
        if (actual.Contains(unexpectedFragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Expected output to not contain '{Escape(unexpectedFragment)}'.");
        }
    }

    private static string Escape(string text)
    {
        return text
            .Replace("\u001b", "\\u001b", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }
}
