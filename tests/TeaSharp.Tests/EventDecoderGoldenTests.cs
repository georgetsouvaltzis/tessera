using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class EventDecoderGoldenTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Decoder_ArrowAndNavigationKeys_Parse", ArrowAndNavigationSequences_ParseExpectedKeys);
        yield return new TestCase("Decoder_PasteBoundaryMarkers_Parse", PasteBoundaryMarkers_ParseExpectedMessages);
        yield return new TestCase("Decoder_FocusMarkers_Parse", FocusMarkers_ParseExpectedMessages);
        yield return new TestCase("Decoder_WindowResizeSequence_Parses", WindowResizeSequence_ParsesExpectedSize);
        yield return new TestCase("Decoder_MouseSequences_Parse", MouseSequences_ParseExpectedMessages);
        yield return new TestCase("Decoder_OscSequences_AreConsumedWithoutMessages", OscSequences_AreConsumedWithoutMessages);
        yield return new TestCase("Decoder_PartialCsi_RequestsMoreDataUntilTimeout", PartialCsi_RequestsMoreDataUntilTimeout);
        yield return new TestCase("Decoder_UnknownSequence_ProducesUnknownMessage", UnknownSequence_ProducesUnknownMessage);
    }

    private static Task ArrowAndNavigationSequences_ParseExpectedKeys()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var up = Decode(decoder, "\u001b[A");
        var ctrlUp = Decode(decoder, "\u001b[1;5A");
        var ctrlDelete = Decode(decoder, "\u001b[3;5~");
        var pageUp = Decode(decoder, "\u001b[5~");
        var pageDown = Decode(decoder, "\u001b[6~");
        var home = Decode(decoder, "\u001bOH");
        var end = Decode(decoder, "\u001bOF");
        var altK = Decode(decoder, "\u001bk");

        // Assert
        AssertKey(up, KeyCode.Up);
        AssertKey(ctrlUp, KeyCode.Up, KeyModifiers.Ctrl);
        AssertKey(ctrlDelete, KeyCode.Delete, KeyModifiers.Ctrl);
        AssertKey(pageUp, KeyCode.PageUp);
        AssertKey(pageDown, KeyCode.PageDown);
        AssertKey(home, KeyCode.Home);
        AssertKey(end, KeyCode.End);
        AssertKey(altK, KeyCode.Character, KeyModifiers.Alt, "k");
        return Task.CompletedTask;
    }

    private static Task PasteBoundaryMarkers_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var pasteStart = Decode(decoder, "\u001b[200~");
        var pasteEnd = Decode(decoder, "\u001b[201~");

        // Assert
        AssertMessageType<PasteStartMsg>(pasteStart);
        AssertMessageType<PasteEndMsg>(pasteEnd);
        return Task.CompletedTask;
    }

    private static Task FocusMarkers_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var focusIn = Decode(decoder, "\u001b[I");
        var focusOut = Decode(decoder, "\u001b[O");

        // Assert
        AssertMessageType<FocusInMsg>(focusIn);
        AssertMessageType<FocusOutMsg>(focusOut);
        return Task.CompletedTask;
    }

    private static Task WindowResizeSequence_ParsesExpectedSize()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var resize = Decode(decoder, "\u001b[8;24;80t");

        // Assert
        AssertConsumed(resize, 10);
        if (resize.Message is not WindowSizeMsg { Width: 80, Height: 24 })
        {
            throw new InvalidOperationException("Expected WindowSizeMsg(80,24).");
        }

        return Task.CompletedTask;
    }

    private static Task MouseSequences_ParseExpectedMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var sgrPress = Decode(decoder, "\u001b[<0;11;7M");
        var sgrRelease = Decode(decoder, "\u001b[<0;11;7m");
        var sgrMotion = Decode(decoder, "\u001b[<35;11;7M");
        var sgrWheel = Decode(decoder, "\u001b[<65;11;7M");

        var x10Bytes = new byte[] { 0x1B, (byte)'[', (byte)'M', (byte)' ', (byte)'+', (byte)'&' };
        var x10Press = decoder.Decode(x10Bytes, timeoutExpired: false);

        // Assert
        AssertMouse(sgrPress, MouseEventType.Press, MouseButton.Left, 10, 6, KeyModifiers.None);
        AssertMouse(sgrRelease, MouseEventType.Release, MouseButton.Left, 10, 6, KeyModifiers.None);
        AssertMouse(sgrMotion, MouseEventType.Motion, MouseButton.None, 10, 6, KeyModifiers.None);
        AssertMouse(sgrWheel, MouseEventType.Wheel, MouseButton.WheelDown, 10, 6, KeyModifiers.None);
        AssertMouse(x10Press, MouseEventType.Press, MouseButton.Left, 10, 5, KeyModifiers.None);

        return Task.CompletedTask;
    }

    private static Task OscSequences_AreConsumedWithoutMessages()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var oscBel = decoder.Decode(
            [0x1B, (byte)']', (byte)'0', (byte)';', (byte)'t', (byte)'i', (byte)'t', (byte)'l', (byte)'e', 0x07],
            timeoutExpired: false);

        var oscSt = decoder.Decode(
            [0x1B, (byte)']', (byte)'2', (byte)';', (byte)'x', 0x1B, (byte)'\\'],
            timeoutExpired: false);

        // Assert
        AssertConsumed(oscBel, 10);
        AssertNoMessage(oscBel);
        AssertConsumed(oscSt, 7);
        AssertNoMessage(oscSt);
        return Task.CompletedTask;
    }

    private static Task PartialCsi_RequestsMoreDataUntilTimeout()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var partial = Decode(decoder, "\u001b[1;", timeoutExpired: false);
        var timedOutPartial = Decode(decoder, "\u001b[1;", timeoutExpired: true);

        // Assert
        TestAssert.True(partial.NeedMoreData, "Partial CSI should request more data.");
        AssertConsumed(partial, 0);
        AssertConsumed(timedOutPartial, 1);
        AssertKey(timedOutPartial, KeyCode.Escape);
        return Task.CompletedTask;
    }

    private static Task UnknownSequence_ProducesUnknownMessage()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var unknown = Decode(decoder, "\u001b[999~");

        // Assert
        AssertMessageType<UnknownInputMsg>(unknown);
        return Task.CompletedTask;
    }

    private static DecodeResult Decode(EventDecoder decoder, string sequence, bool timeoutExpired = false)
    {
        return decoder.Decode(Encoding.UTF8.GetBytes(sequence), timeoutExpired);
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

    private static void AssertMouse(
        DecodeResult result,
        MouseEventType eventType,
        MouseButton button,
        int x,
        int y,
        KeyModifiers modifiers)
    {
        if (result.Message is not MouseMsg mouse)
        {
            throw new InvalidOperationException($"Expected MouseMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (mouse.EventType != eventType
            || mouse.Button != button
            || mouse.X != x
            || mouse.Y != y
            || mouse.Modifiers != modifiers)
        {
            throw new InvalidOperationException(
                $"Expected mouse(type={eventType}, button={button}, x={x}, y={y}, modifiers={modifiers}) " +
                $"but got mouse(type={mouse.EventType}, button={mouse.Button}, x={mouse.X}, y={mouse.Y}, modifiers={mouse.Modifiers}).");
        }
    }
}
