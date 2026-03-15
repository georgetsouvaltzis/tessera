using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class ConsoleTerminalAdapterHelperTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("ConsoleTerminal_KeyMapper_MapsCtrlLetterToCharacterKey", KeyMapper_MapsCtrlLetterToCharacterKey);
        yield return new TestCase("ConsoleTerminal_PasteBurstBuffer_CollapsesBurstIntoPaste", PasteBurstBuffer_CollapsesBurstIntoPaste);
    }

    private static Task KeyMapper_MapsCtrlLetterToCharacterKey()
    {
        var key = new ConsoleKeyInfo('\u0001', ConsoleKey.A, shift: false, alt: false, control: true);

        var message = ConsoleKeyMessageMapper.Map(key) as KeyPressMsg;

        TestAssert.True(message is not null, "Control-letter should map to a key press message.");
        TestAssert.True(message!.Code == KeyCode.Character, "Control-letter should stay on character key code.");
        TestAssert.Equal("a", message.Text, "Control-letter should normalize to lower-case character text.");
        TestAssert.True(message.Modifiers == KeyModifiers.Ctrl, "Control-letter should keep ctrl modifier.");
        return Task.CompletedTask;
    }

    private static Task PasteBurstBuffer_CollapsesBurstIntoPaste()
    {
        var buffer = new ConsolePasteBurstBuffer();
        var emitted = new List<IMessage>();
        Action<IMessage> emit = message => emitted.Add(message);

        foreach (var ch in "alpha beta")
        {
            buffer.TryBuffer(new KeyPressMsg(KeyCode.Character, ch.ToString()), emit);
        }

        buffer.TryBuffer(new KeyPressMsg(KeyCode.Enter), emit);
        foreach (var ch in "gamma")
        {
            buffer.TryBuffer(new KeyPressMsg(KeyCode.Character, ch.ToString()), emit);
        }

        buffer.Flush(emit);

        TestAssert.Equal(1, emitted.Count, "Burst should collapse into a single paste message.");
        if (emitted[0] is not PasteMsg paste)
        {
            throw new InvalidOperationException("Expected PasteMsg from burst flush.");
        }

        TestAssert.True(paste.Content.Contains("alpha beta\ngamma", StringComparison.Ordinal), "Paste content should preserve characters and newline.");
        return Task.CompletedTask;
    }
}
