using Tessera.Components.Composition;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Core.Messages;
using Tessera.Widgets;
using Tessera.Widgets.Internal;

namespace Tessera.Tests;

internal static class WidgetStateTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Widgets_KeyBinding_MatchesCtrlChord", KeyBinding_MatchesCtrlChord);
        yield return new TestCase("Widgets_HelpView_RenderCompact_WrapsByWidth", HelpView_RenderCompact_WrapsByWidth);
        yield return new TestCase("Widgets_HelpView_RenderColumns_UsesExpandedLayout", HelpView_RenderColumns_UsesExpandedLayout);
        yield return new TestCase("Widgets_Viewport_ScrollAndHorizontalOffset", Viewport_ScrollAndHorizontalOffset);
        yield return new TestCase("Widgets_Viewport_WrapMode_SoftWrapsRows", Viewport_WrapMode_SoftWrapsRows);
        yield return new TestCase("Widgets_Viewport_NoDecoration_NoClip_ReusesOriginalLineReferences", Viewport_NoDecoration_NoClip_ReusesOriginalLineReferences);
        yield return new TestCase("Widgets_Viewport_AppendRawLine_PreservesExistingRowsAndAddsNewTailRow", Viewport_AppendRawLine_PreservesExistingRowsAndAddsNewTailRow);
        yield return new TestCase("Widgets_Viewport_NoDecoration_HorizontalOffsetBeyondLineRendersEmpty", Viewport_NoDecoration_HorizontalOffsetBeyondLineRendersEmpty);
        yield return new TestCase("Widgets_Viewport_GutterAndHighlight_RenderDecorations", Viewport_GutterAndHighlight_RenderDecorations);
        yield return new TestCase("Widgets_Viewport_HighlightWithoutGutter_PreservesMarkerPrefix", Viewport_HighlightWithoutGutter_PreservesMarkerPrefix);
        yield return new TestCase("Widgets_Viewport_LineNumberPrefix_ClipsWhenViewportNarrow", Viewport_LineNumberPrefix_ClipsWhenViewportNarrow);
        yield return new TestCase("Widgets_TextInput_EditWordAndSubmit", TextInput_EditWordAndSubmit);
        yield return new TestCase("Widgets_TextInput_AltBindings_WorkForWordOps", TextInput_AltBindings_WorkForWordOps);
        yield return new TestCase("Widgets_TextInput_Multiline_EnterAndVerticalNavigation", TextInput_Multiline_EnterAndVerticalNavigation);
        yield return new TestCase("Widgets_TextInput_SelectAllThenPaste_ReplacesValue", TextInput_SelectAllThenPaste_ReplacesValue);
        yield return new TestCase("Widgets_List_FilterAndPaging_MaintainSelection", List_FilterAndPaging_MaintainSelection);
        yield return new TestCase("Widgets_List_AsyncLoaders_ApplyFilterAndSelection", List_AsyncLoaders_ApplyFilterAndSelection);
        yield return new TestCase("Widgets_List_ReloadAsync_CancelsStaleLoad", List_ReloadAsync_CancelsStaleLoad);
    }

    private static Task KeyBinding_MatchesCtrlChord()
    {
        // Arrange
        var binding = new KeyBinding("ctrl+c", "quit", "ctrl+c");
        var key = new KeyPressMsg(KeyCode.Character, "c", KeyModifiers.Ctrl);

        // Act
        var matched = binding.Matches(key);

        // Assert
        TestAssert.True(matched, "Binding should match normalized ctrl chord.");
        return Task.CompletedTask;
    }

    private static Task HelpView_RenderCompact_WrapsByWidth()
    {
        // Arrange
        var bindings = new[]
        {
            new KeyBinding("up/k", "move up"),
            new KeyBinding("down/j", "move down"),
            new KeyBinding("q", "quit"),
        };

        // Act
        var help = HelpView.RenderCompact(bindings, maxWidth: 24);

        // Assert
        TestAssert.True(help.Contains('\n'), "Help output should wrap when width is constrained.");
        TestAssert.True(help.Contains("up/k move up", StringComparison.Ordinal), "Help output should include first binding.");
        return Task.CompletedTask;
    }

    private static Task HelpView_RenderColumns_UsesExpandedLayout()
    {
        // Arrange
        var bindings = new[]
        {
            new KeyBinding("up/k", "move up"),
            new KeyBinding("down/j", "move down"),
            new KeyBinding("q", "quit"),
            new KeyBinding("?", "help"),
        };

        // Act
        var help = HelpView.RenderColumns(bindings, maxWidth: 56);

        // Assert
        var lines = help.Split('\n');
        TestAssert.True(lines.Length >= 2, "Expanded help should wrap to multiple rows.");
        TestAssert.True(lines[0].Contains("up/k move up", StringComparison.Ordinal), "First row should include first binding.");
        TestAssert.True(lines[0].Contains("q quit", StringComparison.Ordinal), "First row should include second column binding.");
        return Task.CompletedTask;
    }

    private static Task Viewport_ScrollAndHorizontalOffset()
    {
        // Arrange
        var viewport = new ViewportModel();
        viewport.Resize(width: 8, height: 2);
        viewport.SetWrap(false);
        viewport.SetContent("0123456789\nabcdefghij\nklmnopqrst");

        // Act
        viewport.Update(new KeyPressMsg(KeyCode.Right));
        var horizontal = viewport.RenderLines();
        var firstLineAfterHorizontalScroll = horizontal[0];
        viewport.Update(new KeyPressMsg(KeyCode.Down));
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.Equal("23456789", firstLineAfterHorizontalScroll, "Horizontal scroll should shift viewport content.");
        TestAssert.Equal("cdefghij", lines[0], "Vertical scroll should move viewport to the next row.");
        TestAssert.Equal("mnopqrst", lines[1], "Viewport should render following row with same horizontal offset.");
        return Task.CompletedTask;
    }

    private static Task Viewport_WrapMode_SoftWrapsRows()
    {
        // Arrange
        var viewport = new ViewportModel();
        viewport.Resize(width: 4, height: 2);
        viewport.SetWrap(true);
        viewport.SetContent("abcdefghijkl");

        // Act
        var first = viewport.RenderLines();
        var firstLine = first[0];
        var secondLine = first[1];
        viewport.Update(new KeyPressMsg(KeyCode.Down));
        var second = viewport.RenderLines();

        // Assert
        TestAssert.Equal("abcd", firstLine, "Wrap mode should render first visual segment.");
        TestAssert.Equal("efgh", secondLine, "Wrap mode should render second visual segment.");
        TestAssert.Equal("efgh", second[0], "Vertical scroll should move through wrapped visual rows.");
        TestAssert.Equal("ijkl", second[1], "Vertical scroll should reveal last wrapped segment.");
        return Task.CompletedTask;
    }

    private static Task Viewport_NoDecoration_NoClip_ReusesOriginalLineReferences()
    {
        // Arrange
        var viewport = new ViewportModel();
        viewport.Resize(width: 24, height: 2);
        viewport.SetWrap(false);
        var first = "alpha";
        var second = "beta";
        viewport.SetLines([first, second]);

        // Act
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.True(object.ReferenceEquals(first, lines[0]), "No-decoration path should reuse existing line instance when clipping is not required.");
        TestAssert.True(object.ReferenceEquals(second, lines[1]), "No-decoration path should reuse existing line instance for subsequent rows when clipping is not required.");
        return Task.CompletedTask;
    }

    private static Task Viewport_NoDecoration_HorizontalOffsetBeyondLineRendersEmpty()
    {
        // Act
        var lines = ViewportRenderer.RenderLines(
            ["abc"],
            width: 6,
            height: 1,
            xOffset: 10,
            yOffset: 0,
            wrap: false,
            showLineNumbers: false,
            highlightVisualLine: null);

        // Assert
        TestAssert.Equal(string.Empty, lines[0], "No-decoration path should return empty output when horizontal offset exceeds line length.");
        return Task.CompletedTask;
    }

    private static Task Viewport_AppendRawLine_PreservesExistingRowsAndAddsNewTailRow()
    {
        // Arrange
        var viewport = new ViewportModel();
        viewport.Resize(width: 16, height: 3);
        viewport.SetWrap(false);
        viewport.SetLines(["alpha", "beta"]);
        _ = viewport.RenderLines();

        // Act
        viewport.AppendRawLine("gamma");
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.Equal("alpha", lines[0], "Append should preserve existing first row content.");
        TestAssert.Equal("beta", lines[1], "Append should preserve existing second row content.");
        TestAssert.Equal("gamma", lines[2], "Append should expose new tail row without requiring a full reset.");
        return Task.CompletedTask;
    }

    private static Task Viewport_GutterAndHighlight_RenderDecorations()
    {
        // Arrange
        var viewport = new ViewportModel
        {
            ShowLineNumbers = true,
            HighlightVisualLine = 1,
        };
        viewport.Resize(width: 12, height: 2);
        viewport.SetContent("alpha\nbeta\ngamma");

        // Act
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.True(lines[0].StartsWith(" 1 ", StringComparison.Ordinal), "Viewport line numbers should prefix each visual row.");
        TestAssert.True(lines[1].StartsWith(" 2> ", StringComparison.Ordinal), "Highlighted visual row should include marker.");
        return Task.CompletedTask;
    }

    private static Task Viewport_HighlightWithoutGutter_PreservesMarkerPrefix()
    {
        // Arrange
        var viewport = new ViewportModel
        {
            ShowLineNumbers = false,
            HighlightVisualLine = 0,
        };
        viewport.Resize(width: 4, height: 1);
        viewport.SetWrap(false);
        viewport.SetContent("abcdef");

        // Act
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.Equal("> abcd", lines[0], "Highlight marker should prefix clipped content even when viewport width is fully consumed.");
        return Task.CompletedTask;
    }

    private static Task Viewport_LineNumberPrefix_ClipsWhenViewportNarrow()
    {
        // Arrange
        var viewport = new ViewportModel
        {
            ShowLineNumbers = true,
        };
        viewport.Resize(width: 2, height: 1);
        viewport.SetContent("alpha");

        // Act
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.Equal(" 1", lines[0], "When width is smaller than gutter prefix, viewport should render clipped prefix only.");
        return Task.CompletedTask;
    }

    private static Task TextInput_EditWordAndSubmit()
    {
        // Arrange
        var input = new TextInputModel();
        var keyMap = TextInputKeyMap.Default;

        // Act
        foreach (var ch in "hello world")
        {
            input.Update(new KeyPressMsg(KeyCode.Character, ch.ToString()));
        }

        input.Update(new KeyPressMsg(KeyCode.Left, Modifiers: KeyModifiers.Alt), keyMap);
        input.Update(new KeyPressMsg(KeyCode.Backspace, Modifiers: KeyModifiers.Alt), keyMap);
        var submitted = input.Update(new KeyPressMsg(KeyCode.Enter), keyMap);

        // Assert
        TestAssert.Equal("world", input.Value, "Word-level backward delete should remove prior word and separator.");
        TestAssert.True(submitted.Submitted, "Enter key should mark text input submission.");
        return Task.CompletedTask;
    }

    private static Task TextInput_SelectAllThenPaste_ReplacesValue()
    {
        // Arrange
        var input = new TextInputModel();
        input.SetValue("abcdef");

        // Act
        input.Update(new KeyPressMsg(KeyCode.Character, "a", KeyModifiers.Ctrl));
        input.Update(new PasteMsg("z"));

        // Assert
        TestAssert.Equal("z", input.Value, "Pasting with active selection should replace selected text.");
        return Task.CompletedTask;
    }

    private static Task TextInput_AltBindings_WorkForWordOps()
    {
        // Arrange
        var input = new TextInputModel();
        var keyMap = TextInputKeyMap.Default;
        input.SetValue("alpha beta gamma");
        input.Update(new KeyPressMsg(KeyCode.End), keyMap);

        // Act
        input.Update(new KeyPressMsg(KeyCode.Character, "b", KeyModifiers.Alt), keyMap);
        input.Update(new KeyPressMsg(KeyCode.Backspace, Modifiers: KeyModifiers.Alt), keyMap);
        input.Update(new KeyPressMsg(KeyCode.Character, "d", KeyModifiers.Alt), keyMap);

        // Assert
        TestAssert.Equal("alpha ", input.Value, "Alt+b, alt+backspace and alt+d should navigate/delete by word.");
        return Task.CompletedTask;
    }

    private static Task TextInput_Multiline_EnterAndVerticalNavigation()
    {
        // Arrange
        var input = new TextInputModel
        {
            Multiline = true,
        };
        var keyMap = TextInputKeyMap.Default;
        input.SetValue("alpha");
        input.Update(new KeyPressMsg(KeyCode.End), keyMap);

        // Act
        var submit = input.Update(new KeyPressMsg(KeyCode.Enter), keyMap);
        foreach (var ch in "beta")
        {
            input.Update(new KeyPressMsg(KeyCode.Character, ch.ToString()), keyMap);
        }

        input.Update(new KeyPressMsg(KeyCode.Up), keyMap);
        input.Update(new KeyPressMsg(KeyCode.Character, "!"), keyMap);

        // Assert
        TestAssert.True(!submit.Submitted, "Enter should insert newline in multiline mode.");
        TestAssert.Equal("alph!a\nbeta", input.Value, "Vertical navigation should preserve current column while moving between lines.");
        return Task.CompletedTask;
    }

    private static Task List_FilterAndPaging_MaintainSelection()
    {
        // Arrange
        var items = new[]
        {
            "alpha", "beta", "gamma", "delta", "epsilon", "zeta", "eta", "theta",
        };
        var list = new ListModel<string>(items, item => item)
        {
            PageSize = 3,
        };

        // Act
        list.Update(new KeyPressMsg(KeyCode.Down));
        list.Update(new KeyPressMsg(KeyCode.Down));
        list.Update(new KeyPressMsg(KeyCode.Down));
        list.Update(new KeyPressMsg(KeyCode.Down));
        var rowsAfterScroll = list.VisibleRows();

        list.SetFilter("ta");
        var rowsAfterFilter = list.VisibleRows();

        // Assert
        TestAssert.Equal(2, rowsAfterScroll[0].Index, "Paging should shift first visible row as selection moves downward.");
        TestAssert.True(rowsAfterFilter.Count > 0, "Filtered list should keep matching rows.");
        TestAssert.True(rowsAfterFilter[0].Item.Contains("ta", StringComparison.Ordinal), "Filtered rows should match filter text.");
        return Task.CompletedTask;
    }

    private static async Task List_AsyncLoaders_ApplyFilterAndSelection()
    {
        // Arrange
        var list = new ListModel<string>([], item => item)
        {
            PageSize = 2,
        };
        list.SetFilter("ta");

        // Act
        await list.SetItemsAsync(Stream("alpha", "beta", "gamma"));
        var initialRows = list.VisibleRows();
        var appended = await list.AppendItemsAsync(Stream("delta", "theta"));
        var rows = list.VisibleRows();

        // Assert
        TestAssert.Equal(1, initialRows.Count, "Initial async set should keep filtered matches.");
        TestAssert.Equal("beta", initialRows[0].Item, "Initial filtered item should match expected order.");
        TestAssert.Equal(2, appended, "Async append should report number of appended items.");
        TestAssert.Equal(3, list.Count, "Filter should include appended matching rows.");
        TestAssert.Equal("beta", rows[0].Item, "Selection should remain stable after async append.");
        return;

        static async IAsyncEnumerable<string> Stream(params string[] values)
        {
            foreach (var value in values)
            {
                await Task.Yield();
                yield return value;
            }
        }
    }

    private static async Task List_ReloadAsync_CancelsStaleLoad()
    {
        // Arrange
        var list = new ListModel<string>([], item => item);

        // Act
        var first = ReloadAsync(list, token => Slow("old-a", "old-b", token));
        await Task.Delay(5);
        var second = ReloadAsync(list, token => Fast("new-a", "new-b", "new-c", token));
        await Task.WhenAll(first, second);
        var rows = list.VisibleRows();

        // Assert
        TestAssert.Equal(3, list.Count, "Latest tracked load should win and set new item set.");
        TestAssert.True(rows[0].Item.StartsWith("new-", StringComparison.Ordinal), "Stale load should not overwrite latest results.");
        return;

        static async IAsyncEnumerable<string> Slow(string a, string b, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token)
        {
            await Task.Delay(20, token);
            token.ThrowIfCancellationRequested();
            yield return a;
            await Task.Delay(20, token);
            token.ThrowIfCancellationRequested();
            yield return b;
        }

        static async IAsyncEnumerable<string> Fast(string a, string b, string c, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken token)
        {
            await Task.Delay(1, token);
            token.ThrowIfCancellationRequested();
            yield return a;
            yield return b;
            yield return c;
        }

        static Task ReloadAsync(ListModel<string> listModel, Func<CancellationToken, IAsyncEnumerable<string>> loader)
        {
            return listModel.ReloadAsync(loader).AsTask();
        }
    }
}
