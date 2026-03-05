using System.Reflection;
using NUnit.Framework;
using TeaSharp.Components;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Widgets;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class WorkspaceUxIntegrationTests
{
    [Test]
    public async Task ColonEntersCommandModeWithoutChangingFocus()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);

        PressPlain(model, ":");

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=cmd"));
        Assert.That(view, Does.Contain("focus=actions"));
    }

    [Test]
    public async Task ColonTwiceFocusesCommandInput()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);

        PressPlain(model, ":");
        PressPlain(model, ":");

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=cmd"));
        Assert.That(view, Does.Contain("focus=command"));
    }

    [Test]
    public async Task ShiftSemicolonAlsoEntersCommandMode()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);

        model.Update(new KeyPressMsg(KeyCode.Character, ";", KeyModifiers.Shift));

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=cmd"));
    }

    [Test]
    public async Task EscapeExitsCommandModeAndRestoresPriorFocus()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);
        PressPlain(model, ":");

        PressEscape(model);

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=nav"));
        Assert.That(view, Does.Contain("focus=showcase"));
        Assert.That(view, Does.Contain("page=showcase"));
    }

    [Test]
    public async Task UppercasePAndShiftPBothCyclePaneBackward()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);

        var before = ShowcasePaneToken(model.View().Content);
        model.Update(new KeyPressMsg(KeyCode.Character, "P"));
        var afterUpper = ShowcasePaneToken(model.View().Content);
        model.Update(new KeyPressMsg(KeyCode.Character, "p", KeyModifiers.Shift));
        var afterShift = ShowcasePaneToken(model.View().Content);

        Assert.That(afterUpper, Is.Not.EqualTo(before));
        Assert.That(afterShift, Is.Not.EqualTo(afterUpper));
    }

    [Test]
    public async Task ShowcaseHotkeys_ModifyToastAndModalOnlyInCommandMode()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);

        // nav mode: no single-letter side effects
        PressPlain(model, "t");
        PressPlain(model, "m");
        Assert.That(ToastCount(model), Is.EqualTo(0));
        Assert.That(Modal(model).Visible, Is.False);

        // command mode + showcase focus: hotkeys active
        PressPlain(model, ":");
        PressPlain(model, "t");
        PressPlain(model, "m");

        Assert.That(ToastCount(model), Is.EqualTo(1));
        Assert.That(Modal(model).Visible, Is.True);
    }

    [Test]
    public async Task PlainSInCommandInput_DoesNotToggleStress()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        PressPlain(model, ":");
        PressPlain(model, ":");

        PressPlain(model, "s");

        Assert.That(StressMode(model), Is.False);
        Assert.That(CommandInput(model).Value, Is.EqualTo("s"));
    }

    [Test]
    public async Task UnhandledShowcaseKey_DoesNotWriteIntoCommandInput()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);
        GoToShowcase(model);
        FocusShowcasePane(model);
        CommandInput(model).Clear();

        PressPlain(model, "x");

        Assert.That(CommandInput(model).Value, Is.Empty);
    }

    private static void GoToShowcase(CounterModel model)
    {
        PressPlain(model, "3");
    }

    private static void FocusShowcasePane(CounterModel model)
    {
        model.Update(new KeyPressMsg(KeyCode.Tab));
    }

    private static void PressEscape(CounterModel model)
    {
        model.Update(new KeyPressMsg(KeyCode.Escape));
    }

    private static void PressPlain(CounterModel model, string text)
    {
        model.Update(new KeyPressMsg(KeyCode.Character, text));
    }

    private static string ShowcasePaneToken(string content)
    {
        const string marker = "pane=";
        var index = content.IndexOf(marker, StringComparison.Ordinal);
        if (index < 0)
        {
            return string.Empty;
        }

        index += marker.Length;
        var end = index;
        while (end < content.Length && !char.IsWhiteSpace(content[end]))
        {
            end++;
        }

        return content[index..end];
    }

    private static bool StressMode(CounterModel model)
    {
        return (bool)(GetPrivateField(model, "_stressMode") ?? false);
    }

    private static TextInputModel CommandInput(CounterModel model)
    {
        return (TextInputModel?)GetPrivateField(model, "_commandInput")
            ?? throw new InvalidOperationException("CounterModel._commandInput missing.");
    }

    private static ModalComponent Modal(CounterModel model)
    {
        return (ModalComponent?)GetPrivateField(model, "_showcaseModal")
            ?? throw new InvalidOperationException("CounterModel._showcaseModal missing.");
    }

    private static int ToastCount(CounterModel model)
    {
        var center = (ToastCenterComponent?)GetPrivateField(model, "_showcaseToasts")
            ?? throw new InvalidOperationException("CounterModel._showcaseToasts missing.");
        var toasts = center.GetType().GetField("_toasts", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(center);
        return toasts switch
        {
            System.Collections.ICollection collection => collection.Count,
            _ => 0,
        };
    }

    private static object? GetPrivateField(object instance, string fieldName)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field?.GetValue(instance);
    }
}
