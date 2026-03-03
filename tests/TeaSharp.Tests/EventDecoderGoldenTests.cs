using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class EventDecoderGoldenTests
{
    private static readonly EventDecoder Decoder = new();

    public static Task RunAsync()
    {
        AssertKey(Decode("\u001b[A"), KeyCode.Up);
        AssertKey(Decode("\u001b[1;5A"), KeyCode.Up, KeyModifiers.Ctrl);
        AssertKey(Decode("\u001b[3;5~"), KeyCode.Delete, KeyModifiers.Ctrl);
        AssertKey(Decode("\u001b[5~"), KeyCode.PageUp);
        AssertKey(Decode("\u001b[6~"), KeyCode.PageDown);
        AssertKey(Decode("\u001bOH"), KeyCode.Home);
        AssertKey(Decode("\u001bOF"), KeyCode.End);
        AssertKey(Decode("\u001bk"), KeyCode.Character, KeyModifiers.Alt, "k");

        AssertMessageType<PasteStartMsg>(Decode("\u001b[200~"));
        AssertMessageType<PasteEndMsg>(Decode("\u001b[201~"));

        var resize = Decode("\u001b[8;24;80t");
        AssertConsumed(resize, 10);
        if (resize.Message is not WindowSizeMsg { Width: 80, Height: 24 })
        {
            throw new InvalidOperationException("Expected WindowSizeMsg(80,24).");
        }

        var oscBel = Decoder.Decode([0x1B, (byte)']', (byte)'0', (byte)';', (byte)'t', (byte)'i', (byte)'t', (byte)'l', (byte)'e', 0x07], timeoutExpired: false);
        AssertConsumed(oscBel, 10);
        AssertNoMessage(oscBel);

        var oscSt = Decoder.Decode([0x1B, (byte)']', (byte)'2', (byte)';', (byte)'x', 0x1B, (byte)'\\'], timeoutExpired: false);
        AssertConsumed(oscSt, 7);
        AssertNoMessage(oscSt);

        var partial = Decode("\u001b[1;", timeoutExpired: false);
        if (!partial.NeedMoreData || partial.Consumed != 0)
        {
            throw new InvalidOperationException("Expected partial CSI to request more data.");
        }

        var timedOutPartial = Decode("\u001b[1;", timeoutExpired: true);
        AssertConsumed(timedOutPartial, 1);
        AssertKey(timedOutPartial, KeyCode.Escape);

        var unknown = Decode("\u001b[999~");
        AssertMessageType<UnknownInputMsg>(unknown);

        return Task.CompletedTask;
    }

    private static DecodeResult Decode(string sequence, bool timeoutExpired = false)
    {
        return Decoder.Decode(Encoding.UTF8.GetBytes(sequence), timeoutExpired);
    }

    private static void AssertMessageType<T>(DecodeResult result)
        where T : class, IMessage
    {
        if (result.Message is not T)
        {
            throw new InvalidOperationException($"Expected {typeof(T).Name} but got {result.Message?.GetType().Name ?? "null"}.");
        }
    }

    private static void AssertNoMessage(DecodeResult result)
    {
        if (result.Message is not null)
        {
            throw new InvalidOperationException($"Expected null message but got {result.Message.GetType().Name}.");
        }
    }

    private static void AssertConsumed(DecodeResult result, int expected)
    {
        if (result.Consumed != expected)
        {
            throw new InvalidOperationException($"Expected consumed={expected} but got {result.Consumed}.");
        }
    }

    private static void AssertKey(DecodeResult result, KeyCode keyCode, KeyModifiers modifiers = KeyModifiers.None, string text = "")
    {
        if (result.Message is not KeyPressMsg key)
        {
            throw new InvalidOperationException($"Expected KeyPressMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (key.Code != keyCode || key.Modifiers != modifiers || !string.Equals(key.Text, text, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Expected key(code={keyCode}, modifiers={modifiers}, text=\"{text}\") but got key(code={key.Code}, modifiers={key.Modifiers}, text=\"{key.Text}\").");
        }
    }
}
