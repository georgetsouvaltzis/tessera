using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using System.Reflection;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Widgets;

namespace TeaSharp.Tests;

internal static class ShowcaseInteractionTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Showcase_Colon_EntersCommandModeAndFocusesInput", Colon_EntersCommandModeAndFocusesInput);
        yield return new TestCase("Showcase_ShiftSemicolon_EntersCommandMode", ShiftSemicolon_EntersCommandMode);
        yield return new TestCase("Showcase_ColonTwice_FocusesCommandInput", ColonTwice_FocusesCommandInput);
        yield return new TestCase("Showcase_Escape_ExitsCommandMode", Escape_ExitsCommandMode);
        yield return new TestCase("Showcase_Escape_OneShortcutBurst_DoesNotSwitchPage", EscapeBurst_DoesNotSwitchPage);
        yield return new TestCase("Showcase_PlainS_InCommandMode_StaysInput", PlainS_InCommandMode_StaysInput);
        yield return new TestCase("Showcase_PlainQ_InCommandMode_StaysInputAndDoesNotQuit", PlainQ_InCommandMode_StaysInputAndDoesNotQuit);
        yield return new TestCase("Showcase_PlainDigits_InCommandMode_StayInInputAndDoNotSwitchPage", PlainDigits_InCommandMode_StayInInputAndDoNotSwitchPage);
        yield return new TestCase("Showcase_CommandTabSwitch_ChangesShowcaseTab", CommandTabSwitch_ChangesShowcaseTab);
        yield return new TestCase("Showcase_QuestionMark_TogglesHelpMode", QuestionMark_TogglesHelpMode);
        yield return new TestCase("Showcase_ShiftSlash_TogglesHelpMode", ShiftSlash_TogglesHelpMode);
        yield return new TestCase("Showcase_PlainSlash_TogglesHelpMode", PlainSlash_TogglesHelpMode);
        yield return new TestCase("Showcase_ThemeSelection_ChangesRenderedPalette", ThemeSelection_ChangesRenderedPalette);
        yield return new TestCase("Showcase_CtrlS_TogglesStress", CtrlS_TogglesStress);
        yield return new TestCase("Showcase_PaneNavigation_RequiresShowcaseFocus", PaneNavigation_RequiresShowcaseFocus);
        yield return new TestCase("Showcase_UppercaseP_CyclesPaneBackward", UppercaseP_CyclesPaneBackward);
        yield return new TestCase("Showcase_UnhandledKeyInShowcaseFocus_DoesNotTypeCommandInput", UnhandledKeyInShowcaseFocus_DoesNotTypeCommandInput);
    }

    private static async Task Colon_EntersCommandModeAndFocusesInput()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);

        // Act
        PressPlain(model, ":");
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.True(view.Contains("mode=cmd", StringComparison.Ordinal), "Colon should enter command mode.");
        TestAssert.True(view.Contains("focus=command", StringComparison.Ordinal), "Single colon should focus command input.");
    }

    private static async Task Escape_ExitsCommandMode()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);
        PressPlain(model, ":");

        // Act
        PressEscape(model);
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.True(view.Contains("mode=nav", StringComparison.Ordinal), "Escape should exit command mode.");
        TestAssert.True(view.Contains("focus=showcase", StringComparison.Ordinal), "Escape should restore prior focus.");
    }

    private static async Task ColonTwice_FocusesCommandInput()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);

        // Act
        PressPlain(model, ":");
        PressPlain(model, ":");
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.True(view.Contains("mode=cmd", StringComparison.Ordinal), "Command mode should stay enabled.");
        TestAssert.True(view.Contains("focus=command", StringComparison.Ordinal), "Second colon should focus command input.");
    }

    private static async Task ShiftSemicolon_EntersCommandMode()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);

        // Act
        model.Update(new KeyPressMsg(KeyCode.Character, ";", KeyModifiers.Shift));
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.True(view.Contains("mode=cmd", StringComparison.Ordinal), "Shift+semicolon should enter command mode.");
    }

    private static async Task EscapeBurst_DoesNotSwitchPage()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);

        // Act
        PressEscape(model);
        PressPlain(model, "1");
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.True(view.Contains("page=showcase", StringComparison.Ordinal), "Escape shortcut burst should not trigger page switch.");
    }

    private static async Task PlainS_InCommandMode_StaysInput()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        PressPlain(model, ":");
        PressPlain(model, ":");

        // Act
        PressPlain(model, "s");
        var stressMode = StressMode(model);
        var input = CommandInput(model);

        // Assert
        TestAssert.True(!stressMode, "Plain 's' should not toggle stress mode.");
        TestAssert.Equal("s", input.Value, "Plain 's' should route into command input.");
    }

    private static async Task PlainQ_InCommandMode_StaysInputAndDoesNotQuit()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        PressPlain(model, ":");
        PressPlain(model, ":");

        // Act
        var result = model.Update(new KeyPressMsg(KeyCode.Character, "q"));
        var input = CommandInput(model);
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.True(result is null, "Plain 'q' in command mode should not emit quit command.");
        TestAssert.Equal("q", input.Value, "Plain 'q' should route into command input.");
        TestAssert.True(view.Contains("mode=cmd", StringComparison.Ordinal), "Command mode should remain active.");
    }

    private static async Task PlainDigits_InCommandMode_StayInInputAndDoNotSwitchPage()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        PressPlain(model, ":");
        var before = model.Render().Frame.Content;

        // Act
        PressPlain(model, "1");
        PressPlain(model, "2");
        PressPlain(model, "3");
        var after = model.Render().Frame.Content;
        var input = CommandInput(model);

        // Assert
        TestAssert.True(before.Contains("page=showcase", StringComparison.Ordinal), "Setup should begin on showcase page.");
        TestAssert.True(after.Contains("page=showcase", StringComparison.Ordinal), "Digits in command mode should not switch pages.");
        TestAssert.True(after.Contains("mode=cmd", StringComparison.Ordinal), "Command mode should stay active.");
        TestAssert.Equal("123", input.Value, "Digits should route into command input.");
    }

    private static async Task CommandTabSwitch_ChangesShowcaseTab()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        PressPlain(model, ":");
        var input = CommandInput(model);

        // Act
        foreach (var ch in "tab 2")
        {
            PressPlain(model, ch.ToString());
        }

        model.Update(new KeyPressMsg(KeyCode.Enter));
        var view = model.Render().Frame.Content;

        // Assert
        TestAssert.Equal(string.Empty, input.Value, "Submitted command should clear command input.");
        TestAssert.True(view.Contains("tab=2", StringComparison.Ordinal), "Command 'tab 2' should switch to showcase tab 2.");
        TestAssert.True(view.Contains("mode=cmd", StringComparison.Ordinal), "Command mode should remain active after submission.");
    }

    private static async Task ShiftSlash_TogglesHelpMode()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        var before = ShowFullHelp(model);

        // Act
        model.Update(new KeyPressMsg(KeyCode.Character, "/", KeyModifiers.Shift));
        var after = ShowFullHelp(model);

        // Assert
        TestAssert.True(before != after, "Shift+/ should toggle workspace help mode.");
    }

    private static async Task QuestionMark_TogglesHelpMode()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        var before = ShowFullHelp(model);

        // Act
        model.Update(new KeyPressMsg(KeyCode.Character, "?"));
        var after = ShowFullHelp(model);

        // Assert
        TestAssert.True(before != after, "'?' should toggle workspace help mode.");
    }

    private static async Task PlainSlash_TogglesHelpMode()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        var before = ShowFullHelp(model);

        // Act
        model.Update(new KeyPressMsg(KeyCode.Character, "/"));
        var after = ShowFullHelp(model);

        // Assert
        TestAssert.True(before != after, "Plain '/' should toggle workspace help mode.");
    }

    private static async Task ThemeSelection_ChangesRenderedPalette()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);
        model.Update(new KeyPressMsg(KeyCode.Right)); // data
        model.Update(new KeyPressMsg(KeyCode.Right)); // forms
        PressPlain(model, "p");
        PressPlain(model, "p"); // forms theme pane
        var before = HeaderStylePrefix(model.Render().Frame.Content);

        // Act
        PressPlain(model, "r");
        var after = HeaderStylePrefix(model.Render().Frame.Content);

        // Assert
        TestAssert.True(
            !string.Equals(before, after, StringComparison.Ordinal),
            "Theme change should alter rendered header palette.");
    }

    private static async Task CtrlS_TogglesStress()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);

        // Act
        model.Update(new KeyPressMsg(KeyCode.Character, "s", KeyModifiers.Ctrl));
        var stressMode = StressMode(model);

        // Assert
        TestAssert.True(stressMode, "Ctrl+S should toggle stress mode on.");
    }

    private static async Task PaneNavigation_RequiresShowcaseFocus()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        var beforeWithoutFocus = ShowcasePaneToken(model.Render().Frame.Content);

        // Act
        PressPlain(model, "p");
        var afterWithoutFocus = ShowcasePaneToken(model.Render().Frame.Content);

        // Assert
        TestAssert.Equal(beforeWithoutFocus, afterWithoutFocus, "Pane should not move while focus is outside showcase pane.");

        // Act
        FocusShowcasePane(model);
        var beforeFocused = ShowcasePaneToken(model.Render().Frame.Content);
        PressPlain(model, "p");
        var afterFocused = ShowcasePaneToken(model.Render().Frame.Content);

        // Assert
        TestAssert.True(
            !string.Equals(beforeFocused, afterFocused, StringComparison.Ordinal),
            "Pane should move when showcase pane has focus.");
    }

    private static async Task UppercaseP_CyclesPaneBackward()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);
        var before = ShowcasePaneToken(model.Render().Frame.Content);

        // Act
        model.Update(new KeyPressMsg(KeyCode.Character, "P"));
        var after = ShowcasePaneToken(model.Render().Frame.Content);

        // Assert
        TestAssert.True(
            !string.Equals(before, after, StringComparison.Ordinal),
            "Uppercase P should cycle showcase pane backward.");
    }

    private static async Task UnhandledKeyInShowcaseFocus_DoesNotTypeCommandInput()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);
        var input = CommandInput(model);
        input.Clear();

        // Act
        PressPlain(model, "x");

        // Assert
        TestAssert.Equal(string.Empty, input.Value, "Unhandled showcase keys should not leak into command input.");
    }

    private static void GoToShowcase(CounterModel model)
    {
        PressPlain(model, "3");
    }

    private static void FocusShowcasePane(CounterModel model)
    {
        model.Update(new KeyPressMsg(KeyCode.Tab));
    }

    private static void PressEscape(CounterModel model, bool isRepeat = false)
    {
        model.Update(new KeyPressMsg(KeyCode.Escape, IsRepeat: isRepeat));
    }

    private static void PressPlain(CounterModel model, string text)
    {
        model.Update(new KeyPressMsg(KeyCode.Character, text));
    }

    private static string ShowcasePaneToken(string content)
    {
        const string marker = "pane=";
        var idx = content.IndexOf(marker, StringComparison.Ordinal);
        if (idx < 0)
        {
            return string.Empty;
        }

        idx += marker.Length;
        var end = idx;
        while (end < content.Length && !char.IsWhiteSpace(content[end]))
        {
            end++;
        }

        return content[idx..end];
    }

    private static string HeaderStylePrefix(string content)
    {
        const string title = "TeaSharp Capability Showcase";
        var index = content.IndexOf(title, StringComparison.Ordinal);
        if (index <= 0)
        {
            return string.Empty;
        }

        return content[..index];
    }

    private static bool StressMode(CounterModel model)
    {
        return (bool)(GetPrivateField(nameof(CounterModel), model, "_stressMode") ?? false);
    }

    private static bool ShowFullHelp(CounterModel model)
    {
        return (bool)(GetPrivateField(nameof(CounterModel), model, "_showFullHelp") ?? false);
    }

    private static TextInputModel CommandInput(CounterModel model)
    {
        var value = GetPrivateField(nameof(CounterModel), model, "_commandInput");
        return value as TextInputModel
            ?? throw new InvalidOperationException("CounterModel._commandInput field was not found.");
    }

    private static object? GetPrivateField(string ownerName, object instance, string fieldName)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field is null)
        {
            throw new InvalidOperationException($"{ownerName}.{fieldName} field is missing.");
        }

        return field.GetValue(instance);
    }
}
