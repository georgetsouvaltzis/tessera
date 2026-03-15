using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.Text;
using NUnit.Framework;
using TeaSharp;
using TeaSharp.Hosting;
using TeaSharp.TestFixtures;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class DecoderDrivenWorkspaceUxTests
{
    [Test]
    public void CsiArrowUpSequenceIncrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001b[A");

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: 1"));
    }

    [Test]
    public void CsiArrowDownSequenceDecrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001b[B");

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: -1"));
    }

    [Test]
    public void Utf8QSequenceReturnsQuitCommand()
    {
        var model = new CounterFixtureModel();

        var result = ApplyDecoded(model, "q");

        Assert.That(result, Is.EqualTo(TeaSharp.Core.Commands.Effects.Quit));
    }

    [Test]
    public void Ss3ArrowUpSequenceAlsoIncrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001bOA");

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: 1"));
    }

    private static Effect? ApplyDecoded(CounterFixtureModel model, string sequence)
    {
        var decoder = new EventDecoder();
        var bytes = Encoding.UTF8.GetBytes(sequence);
        var index = 0;
        Effect? last = null;

        while (index < bytes.Length)
        {
            var result = decoder.Decode(bytes.AsSpan(index), timeoutExpired: false);
            if (result.Consumed == 0)
            {
                result = decoder.Decode(bytes.AsSpan(index), timeoutExpired: true);
            }

            if (result.Consumed == 0)
            {
                break;
            }

            index += result.Consumed;
            if (result.Message is not null)
            {
                last = model.Update(ToFixtureMessage(result.Message));
            }
        }

        return last;
    }

    private static KeyPressMsg ToFixtureMessage(Message message)
    {
        return message switch
        {
            KeyPressed key => new KeyPressMsg(
                key.Key switch
                {
                    Key.Character => KeyCode.Character,
                    Key.Enter => KeyCode.Enter,
                    Key.Tab => KeyCode.Tab,
                    Key.Escape => KeyCode.Escape,
                    Key.Backspace => KeyCode.Backspace,
                    Key.Up => KeyCode.Up,
                    Key.Down => KeyCode.Down,
                    Key.Left => KeyCode.Left,
                    Key.Right => KeyCode.Right,
                    Key.Home => KeyCode.Home,
                    Key.End => KeyCode.End,
                    Key.PageUp => KeyCode.PageUp,
                    Key.PageDown => KeyCode.PageDown,
                    Key.Insert => KeyCode.Insert,
                    Key.Delete => KeyCode.Delete,
                    Key.F1 => KeyCode.F1,
                    Key.F2 => KeyCode.F2,
                    Key.F3 => KeyCode.F3,
                    Key.F4 => KeyCode.F4,
                    Key.F5 => KeyCode.F5,
                    Key.F6 => KeyCode.F6,
                    Key.F7 => KeyCode.F7,
                    Key.F8 => KeyCode.F8,
                    Key.F9 => KeyCode.F9,
                    Key.F10 => KeyCode.F10,
                    Key.F11 => KeyCode.F11,
                    Key.F12 => KeyCode.F12,
                    _ => KeyCode.Unknown,
                },
                key.Text,
                key.Modifiers switch
                {
                    ModifierKeys.Shift => KeyModifiers.Shift,
                    ModifierKeys.Alt => KeyModifiers.Alt,
                    ModifierKeys.Ctrl => KeyModifiers.Ctrl,
                    ModifierKeys.Meta => KeyModifiers.Meta,
                    ModifierKeys.Shift | ModifierKeys.Alt => KeyModifiers.Shift | KeyModifiers.Alt,
                    ModifierKeys.Shift | ModifierKeys.Ctrl => KeyModifiers.Shift | KeyModifiers.Ctrl,
                    ModifierKeys.Shift | ModifierKeys.Meta => KeyModifiers.Shift | KeyModifiers.Meta,
                    ModifierKeys.Alt | ModifierKeys.Ctrl => KeyModifiers.Alt | KeyModifiers.Ctrl,
                    ModifierKeys.Alt | ModifierKeys.Meta => KeyModifiers.Alt | KeyModifiers.Meta,
                    ModifierKeys.Ctrl | ModifierKeys.Meta => KeyModifiers.Ctrl | KeyModifiers.Meta,
                    ModifierKeys.Shift | ModifierKeys.Alt | ModifierKeys.Ctrl => KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Ctrl,
                    ModifierKeys.Shift | ModifierKeys.Alt | ModifierKeys.Meta => KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Meta,
                    ModifierKeys.Shift | ModifierKeys.Ctrl | ModifierKeys.Meta => KeyModifiers.Shift | KeyModifiers.Ctrl | KeyModifiers.Meta,
                    ModifierKeys.Alt | ModifierKeys.Ctrl | ModifierKeys.Meta => KeyModifiers.Alt | KeyModifiers.Ctrl | KeyModifiers.Meta,
                    ModifierKeys.Shift | ModifierKeys.Alt | ModifierKeys.Ctrl | ModifierKeys.Meta => KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Ctrl | KeyModifiers.Meta,
                    _ => KeyModifiers.None,
                },
                key.IsRepeat),
            _ => throw new InvalidOperationException($"Unsupported decoded message for fixture model: {message.GetType().Name}"),
        };
    }
}
