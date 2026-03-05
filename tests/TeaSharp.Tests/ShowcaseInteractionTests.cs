using System.Reflection;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Widgets;

namespace TeaSharp.Tests;

internal static class ShowcaseInteractionTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Showcase_Escape_TogglesCommandModeOnce", Escape_TogglesCommandModeOnce);
        yield return new TestCase("Showcase_Escape_OneShortcutBurst_DoesNotSwitchPage", EscapeBurst_DoesNotSwitchPage);
        yield return new TestCase("Showcase_PlainS_InCommandInput_DoesNotToggleStress", PlainS_InCommandInput_DoesNotToggleStress);
        yield return new TestCase("Showcase_CtrlS_TogglesStress", CtrlS_TogglesStress);
        yield return new TestCase("Showcase_PaneNavigation_RequiresShowcaseFocus", PaneNavigation_RequiresShowcaseFocus);
    }

    private static async Task Escape_TogglesCommandModeOnce()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);

        // Act
        PressEscape(model);
        var once = model.View().Content;
        PressEscape(model, isRepeat: true);
        var repeated = model.View().Content;

        // Assert
        TestAssert.True(once.Contains("mode=cmd", StringComparison.Ordinal), "Escape should toggle showcase mode to command.");
        TestAssert.True(repeated.Contains("mode=cmd", StringComparison.Ordinal), "Repeated escape should not toggle mode twice.");
        TestAssert.True(repeated.Contains("page=showcase", StringComparison.Ordinal), "Escape handling should stay on showcase page.");
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
        var view = model.View().Content;

        // Assert
        TestAssert.True(view.Contains("page=showcase", StringComparison.Ordinal), "Escape shortcut burst should not trigger page switch.");
    }

    private static async Task PlainS_InCommandInput_DoesNotToggleStress()
    {
        // Arrange
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusCommandInput(model);

        // Act
        PressPlain(model, "s");
        var stressMode = StressMode(model);
        var input = CommandInput(model);

        // Assert
        TestAssert.True(!stressMode, "Plain 's' should not toggle stress mode.");
        TestAssert.Equal("s", input.Value, "Plain 's' should route into command input.");
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
        var beforeWithoutFocus = ShowcasePaneToken(model.View().Content);

        // Act
        PressPlain(model, "p");
        var afterWithoutFocus = ShowcasePaneToken(model.View().Content);

        // Assert
        TestAssert.Equal(beforeWithoutFocus, afterWithoutFocus, "Pane should not move while focus is outside showcase pane.");

        // Act
        FocusShowcasePane(model);
        var beforeFocused = ShowcasePaneToken(model.View().Content);
        PressPlain(model, "p");
        var afterFocused = ShowcasePaneToken(model.View().Content);

        // Assert
        TestAssert.True(
            !string.Equals(beforeFocused, afterFocused, StringComparison.Ordinal),
            "Pane should move when showcase pane has focus.");
    }

    private static void GoToShowcase(CounterModel model)
    {
        PressPlain(model, "3");
    }

    private static void FocusShowcasePane(CounterModel model)
    {
        model.Update(new KeyPressMsg(KeyCode.Tab));
    }

    private static void FocusCommandInput(CounterModel model)
    {
        model.Update(new KeyPressMsg(KeyCode.Tab));
        model.Update(new KeyPressMsg(KeyCode.Tab));
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

    private static bool StressMode(CounterModel model)
    {
        return (bool)(GetPrivateField(nameof(CounterModel), model, "_stressMode") ?? false);
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
