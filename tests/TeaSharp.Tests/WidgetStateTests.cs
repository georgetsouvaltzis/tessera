using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Tests;

internal static class WidgetStateTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Widgets_KeyBinding_MatchesCtrlChord", KeyBinding_MatchesCtrlChord);
        yield return new TestCase("Widgets_HelpView_RenderCompact_WrapsByWidth", HelpView_RenderCompact_WrapsByWidth);
        yield return new TestCase("Widgets_Viewport_ScrollAndHorizontalOffset", Viewport_ScrollAndHorizontalOffset);
        yield return new TestCase("Widgets_Viewport_WrapMode_SoftWrapsRows", Viewport_WrapMode_SoftWrapsRows);
        yield return new TestCase("Widgets_TextInput_EditWordAndSubmit", TextInput_EditWordAndSubmit);
        yield return new TestCase("Widgets_TextInput_SelectAllThenPaste_ReplacesValue", TextInput_SelectAllThenPaste_ReplacesValue);
        yield return new TestCase("Widgets_List_FilterAndPaging_MaintainSelection", List_FilterAndPaging_MaintainSelection);
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
        viewport.Update(new KeyPressMsg(KeyCode.Down));
        var lines = viewport.RenderLines();

        // Assert
        TestAssert.Equal("23456789", horizontal[0], "Horizontal scroll should shift viewport content.");
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
        viewport.Update(new KeyPressMsg(KeyCode.Down));
        var second = viewport.RenderLines();

        // Assert
        TestAssert.Equal("abcd", first[0], "Wrap mode should render first visual segment.");
        TestAssert.Equal("efgh", first[1], "Wrap mode should render second visual segment.");
        TestAssert.Equal("efgh", second[0], "Vertical scroll should move through wrapped visual rows.");
        TestAssert.Equal("ijkl", second[1], "Vertical scroll should reveal last wrapped segment.");
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
}
