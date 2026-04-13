using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class VisualParityTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "VisualParity_ContextMenu_BorderedLongTitleWithMarker_RendersWithoutAnsiByDefault",
            ContextMenu_BorderedLongTitleWithMarker_RendersWithoutAnsiByDefault);
        yield return new TestCase(
            "VisualParity_ContextMenu_BorderedLongTitleWithMarker_AppliesBorderAndGlyphOverrides",
            ContextMenu_BorderedLongTitleWithMarker_AppliesBorderAndGlyphOverrides);
        yield return new TestCase(
            "VisualParity_CommandPalette_BorderedTitleMarkerAndPadding_RendersWithoutAnsiByDefault",
            CommandPalette_BorderedTitleMarkerAndPadding_RendersWithoutAnsiByDefault);
        yield return new TestCase(
            "VisualParity_CommandPalette_BorderedTitleMarkerAndPadding_AppliesBorderAndGlyphOverrides",
            CommandPalette_BorderedTitleMarkerAndPadding_AppliesBorderAndGlyphOverrides);
        yield return new TestCase(
            "VisualParity_MenuBar_BorderedGlyphWrappers_DoNotClipFirstAndLastItems_InMonochrome",
            MenuBar_BorderedGlyphWrappers_DoNotClipFirstAndLastItems_InMonochrome);
        yield return new TestCase(
            "VisualParity_MenuBar_BorderedGlyphWrappers_AppliesFocusedBorderStyleOverride",
            MenuBar_BorderedGlyphWrappers_AppliesFocusedBorderStyleOverride);
        yield return new TestCase(
            "VisualParity_Notifications_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault",
            Notifications_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault);
        yield return new TestCase(
            "VisualParity_Notifications_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides",
            Notifications_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides);
        yield return new TestCase(
            "VisualParity_LogView_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault",
            LogView_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault);
        yield return new TestCase(
            "VisualParity_LogView_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides",
            LogView_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides);
        yield return new TestCase(
            "VisualParity_MarkdownView_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault",
            MarkdownView_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault);
        yield return new TestCase(
            "VisualParity_MarkdownView_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides",
            MarkdownView_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides);
    }

    private static Task ContextMenu_BorderedLongTitleWithMarker_RendersWithoutAnsiByDefault()
    {
        var menu = new ContextMenu
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            ShowFocusMarker = true,
            FocusMarker = "!",
            Title = "Context Actions for Build Pipeline"
        };
        menu.SetItems(
        [
            new ContextMenuItem("open", "Open"),
            new ContextMenuItem("edit", "Edit")
        ]);
        menu.OpenAt(0, 0);

        var canvas = new Canvas(96, 10);
        menu.Render(canvas, new Rect(0, 0, 96, 10));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Context Actions for Build Pipeline !", StringComparison.Ordinal),
            "ContextMenu title and marker should remain readable when bordered.");
        TestAssert.True(output.Contains("> Open", StringComparison.Ordinal),
            "ContextMenu should render selected row marker and label in monochrome mode.");
        TestAssert.True(!ContainsAnsiEscape(output),
            "ContextMenu monochrome defaults should not emit ANSI style sequences.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_BorderedLongTitleWithMarker_AppliesBorderAndGlyphOverrides()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var baseBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(15, 55, 95));
        var menu = new ContextMenu
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            ShowFocusMarker = true,
            FocusMarker = "!",
            Title = "Context Actions for Build Pipeline",
            Glyphs = new ContextMenuGlyphSet(".", "▶", "~", ":"),
            BorderStyleText = baseBorderStyle,
            FocusedBorderStyleText = focusedBorderStyle
        };
        menu.SetItems(
        [
            new ContextMenuItem("open", "Open"),
            new ContextMenuItem("edit", "Edit")
        ]);
        menu.OpenAt(0, 0);

        var canvas = new Canvas(96, 10, CanvasTextMode.GraphemeAware);
        menu.Render(canvas, new Rect(0, 0, 96, 10));
        var output = canvas.Render();

        var mergedBorderStyle = baseBorderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains("▶:Open", StringComparison.Ordinal),
            "ContextMenu should render selected rows using override glyph markers.");
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "ContextMenu should render focused border glyphs with merged border style overrides.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_BorderedTitleMarkerAndPadding_RendersWithoutAnsiByDefault()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Padding = Thickness.All(1),
            ShowFocusMarker = true,
            FocusMarker = "!",
            Title = "Command Palette for Deployments"
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous")
        ]);
        palette.Open();
        palette.SetQueryText("de");

        var canvas = new Canvas(96, 24);
        palette.Render(canvas, new Rect(0, 0, 96, 24));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Command Palette for Deployments !", StringComparison.Ordinal),
            "CommandPalette should render full title and marker when bordered with padding.");
        TestAssert.True(output.Contains("> de", StringComparison.Ordinal),
            "CommandPalette should render query prompt and query text in monochrome mode.");
        TestAssert.True(output.Contains("> Deploy - publish release", StringComparison.Ordinal),
            "CommandPalette should render selected row text in monochrome mode.");
        TestAssert.True(!ContainsAnsiEscape(output),
            "CommandPalette monochrome defaults should not emit ANSI style sequences.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_BorderedTitleMarkerAndPadding_AppliesBorderAndGlyphOverrides()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var baseBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(100, 22, 80));
        var palette = new CommandPalette
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Padding = Thickness.All(1),
            ShowFocusMarker = true,
            FocusMarker = "!",
            Title = "Command Palette for Deployments",
            Glyphs = new CommandPaletteGlyphSet("?", ".", "*", "~", ":"),
            BorderStyleText = baseBorderStyle,
            FocusedBorderStyleText = focusedBorderStyle
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous")
        ]);
        palette.Open();
        palette.SetQueryText("de");

        var canvas = new Canvas(96, 24, CanvasTextMode.GraphemeAware);
        palette.Render(canvas, new Rect(0, 0, 96, 24));
        var output = canvas.Render();

        var mergedBorderStyle = baseBorderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains("?:de", StringComparison.Ordinal),
            "CommandPalette should render override query prompt glyphs.");
        TestAssert.True(output.Contains("*:Deploy - publish release", StringComparison.Ordinal),
            "CommandPalette should render selected row override glyphs.");
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "CommandPalette should style focused border glyphs with merged border overrides.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_BorderedGlyphWrappers_DoNotClipFirstAndLastItems_InMonochrome()
    {
        var menu = new MenuBar
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Glyphs = new MenuBarGlyphSet("<", ">", "[", "]", "(", ")", "{", "}")
        };
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("view", "View", 'v')
        ]);

        var firstCanvas = new Canvas(22, 3);
        menu.Render(firstCanvas, new Rect(0, 0, 22, 3));
        var firstOutput = firstCanvas.Render();
        TestAssert.True(firstOutput.Contains("<File{f}>", StringComparison.Ordinal),
            "MenuBar should render first selected item wrappers without clipping.");
        TestAssert.True(firstOutput.Contains("[View{v}]", StringComparison.Ordinal),
            "MenuBar should render trailing item wrappers without clipping.");
        TestAssert.True(!ContainsAnsiEscape(firstOutput),
            "MenuBar monochrome defaults should not emit ANSI style sequences.");

        menu.Handle(new KeyPressed(Key.Right));
        var lastCanvas = new Canvas(22, 3);
        menu.Render(lastCanvas, new Rect(0, 0, 22, 3));
        var lastOutput = lastCanvas.Render();
        TestAssert.True(lastOutput.Contains("[File{f}]", StringComparison.Ordinal),
            "MenuBar should keep first item fully visible when it becomes unselected.");
        TestAssert.True(lastOutput.Contains("<View{v}>", StringComparison.Ordinal),
            "MenuBar should keep last selected item fully visible when selection moves right.");
        TestAssert.True(!ContainsAnsiEscape(lastOutput),
            "MenuBar monochrome output should remain ANSI-free after selection changes.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_BorderedGlyphWrappers_AppliesFocusedBorderStyleOverride()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var baseBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(60, 10, 40));
        var menu = new MenuBar
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Glyphs = new MenuBarGlyphSet("<", ">", "[", "]", "(", ")", "{", "}"),
            BorderStyleText = baseBorderStyle,
            FocusedBorderStyleText = focusedBorderStyle
        };
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("view", "View", 'v')
        ]);

        var canvas = new Canvas(22, 3, CanvasTextMode.GraphemeAware);
        menu.Render(canvas, new Rect(0, 0, 22, 3));
        var output = canvas.Render();

        var mergedBorderStyle = baseBorderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "MenuBar should render focused border glyphs with merged border style overrides.");
        return Task.CompletedTask;
    }

    private static Task Notifications_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault()
    {
        var notifications = new Notifications
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "Notification Feed",
            FocusMarker = "!",
            ShowFocusMarker = true
        };
        notifications.Push("build finished", NotificationLevel.Success, "a");
        notifications.Push("pipeline warning", NotificationLevel.Warning, "b");

        var canvas = new Canvas(48, 8);
        notifications.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Notification Feed !", StringComparison.Ordinal),
            "Notifications should render title and focus marker by default.");
        TestAssert.True(output.Contains("pipeline warning", StringComparison.Ordinal),
            "Notifications should render item text in monochrome mode.");
        TestAssert.True(!ContainsAnsiEscape(output),
            "Notifications monochrome defaults should not emit ANSI style sequences.");
        return Task.CompletedTask;
    }

    private static Task Notifications_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var baseBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 30, 90));
        var notifications = new Notifications
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "Notification Feed",
            FocusMarker = "!",
            ShowFocusMarker = true,
            BorderStyleText = baseBorderStyle,
            FocusedBorderStyleText = focusedBorderStyle
        };
        notifications.Push("pipeline warning", NotificationLevel.Warning, "a");

        var canvas = new Canvas(48, 8, CanvasTextMode.GraphemeAware);
        notifications.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        var mergedBorderStyle = baseBorderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "Notifications should render focused border glyphs with merged border style overrides.");
        return Task.CompletedTask;
    }

    private static Task LogView_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault()
    {
        var logs = new LogView
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "Build Logs",
            FocusMarker = "!",
            ShowFocusMarker = true
        };
        logs.Append("compile started");
        logs.Append("compile finished");

        var canvas = new Canvas(48, 8);
        logs.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Build Logs !", StringComparison.Ordinal),
            "LogView should render title and focus marker by default.");
        TestAssert.True(output.Contains("compile finished", StringComparison.Ordinal),
            "LogView should render entry text in monochrome mode.");
        TestAssert.True(!ContainsAnsiEscape(output),
            "LogView monochrome defaults should not emit ANSI style sequences.");
        return Task.CompletedTask;
    }

    private static Task LogView_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var baseBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(90, 40, 10));
        var logs = new LogView
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "Build Logs",
            FocusMarker = "!",
            ShowFocusMarker = true,
            BorderStyleText = baseBorderStyle,
            FocusedBorderStyleText = focusedBorderStyle
        };
        logs.Append("compile finished");

        var canvas = new Canvas(48, 8, CanvasTextMode.GraphemeAware);
        logs.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        var mergedBorderStyle = baseBorderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "LogView should render focused border glyphs with merged border style overrides.");
        return Task.CompletedTask;
    }

    private static Task MarkdownView_BorderedTitleAndFocusMarker_RendersWithoutAnsiByDefault()
    {
        var markdown = new MarkdownView
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "README Preview",
            FocusMarker = "!",
            ShowFocusMarker = true
        };
        markdown.SetMarkdown("# heading\n- item");

        var canvas = new Canvas(48, 8);
        markdown.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("README Preview !", StringComparison.Ordinal),
            "MarkdownView should render title and focus marker by default.");
        TestAssert.True(output.Contains("# HEADING", StringComparison.Ordinal),
            "MarkdownView should render markdown content in monochrome mode.");
        TestAssert.True(!ContainsAnsiEscape(output),
            "MarkdownView monochrome defaults should not emit ANSI style sequences.");
        return Task.CompletedTask;
    }

    private static Task MarkdownView_BorderedTitleAndFocusMarker_AppliesBorderStyleOverrides()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithBold();
        var baseBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(12, 80, 30));
        var markdown = new MarkdownView
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "README Preview",
            FocusMarker = "!",
            ShowFocusMarker = true,
            BorderStyleText = baseBorderStyle,
            FocusedBorderStyleText = focusedBorderStyle
        };
        markdown.SetMarkdown("# heading");

        var canvas = new Canvas(48, 8, CanvasTextMode.GraphemeAware);
        markdown.Render(canvas, new Rect(0, 0, 48, 8));
        var output = canvas.Render();

        var mergedBorderStyle = baseBorderStyle.Merge(focusedBorderStyle);
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal),
            "MarkdownView should render focused border glyphs with merged border style overrides.");
        return Task.CompletedTask;
    }

    private static bool ContainsAnsiEscape(string value)
    {
        return value.Contains("\e[", StringComparison.Ordinal);
    }
}
