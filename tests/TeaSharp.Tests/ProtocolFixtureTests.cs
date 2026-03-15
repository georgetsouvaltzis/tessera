using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class ProtocolFixtureTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("ProtocolFixture_Ghostty_ModifyOtherKeys_DecodesModifiers", Ghostty_ModifyOtherKeys_DecodesModifiers);
        yield return new TestCase("ProtocolFixture_ITerm2_CsiU_DecodesModifierCombos", ITerm2_CsiU_DecodesModifierCombos);
        yield return new TestCase("ProtocolFixture_Tmux_CsiCursorModifiers_Decode", Tmux_CsiCursorModifiers_Decode);
        yield return new TestCase("ProtocolFixture_Xterm_FunctionKeys_Decode", Xterm_FunctionKeys_Decode);
        yield return new TestCase("ProtocolFixture_Kitty_CsiUCombos_Decode", Kitty_CsiUCombos_Decode);
        yield return new TestCase("ProtocolFixture_WezTerm_ModifyOtherKeys_Decode", WezTerm_ModifyOtherKeys_Decode);
        yield return new TestCase("ProtocolFixture_Konsole_MetaCursorAndCsiU_Decode", Konsole_MetaCursorAndCsiU_Decode);
        yield return new TestCase("ProtocolFixture_Urxvt1015_Mouse_Decode", Urxvt1015_Mouse_Decode);
        yield return new TestCase("ProtocolFixture_Alacritty_AltFallback_Decode", Alacritty_AltFallback_Decode);
        yield return new TestCase("ProtocolFixture_TerminalReader_FocusPasteRoundTrip", TerminalReader_FocusPasteRoundTrip);
        yield return new TestCase("ProtocolFixture_AppleTerminal_AltFallback_Decodes", AppleTerminal_AltFallback_Decodes);
    }

    private static Task Ghostty_ModifyOtherKeys_DecodesModifiers()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var altCtrlSemicolon = Decode(decoder, "\u001b[27;7;59~");
        var shiftAltCtrlJ = Decode(decoder, "\u001b[27;8;106~");

        // Assert
        AssertKey(altCtrlSemicolon, KeyCode.Character, KeyModifiers.Alt | KeyModifiers.Ctrl, ";");
        AssertKey(shiftAltCtrlJ, KeyCode.Character, KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Ctrl, "j");
        return Task.CompletedTask;
    }

    private static Task ITerm2_CsiU_DecodesModifierCombos()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var ctrlShiftTab = Decode(decoder, "\u001b[9;6u");
        var altB = Decode(decoder, "\u001b[98;3u");
        var altBackspace = Decode(decoder, "\u001b[127;3u");

        // Assert
        AssertKey(ctrlShiftTab, KeyCode.Tab, KeyModifiers.Shift | KeyModifiers.Ctrl);
        AssertKey(altB, KeyCode.Character, KeyModifiers.Alt, "b");
        AssertKey(altBackspace, KeyCode.Backspace, KeyModifiers.Alt);
        return Task.CompletedTask;
    }

    private static Task Tmux_CsiCursorModifiers_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var altUp = Decode(decoder, "\u001b[1;3A");
        var shiftCtrlLeft = Decode(decoder, "\u001b[1;6D");
        var ctrlRight = Decode(decoder, "\u001b[1;5C");

        // Assert
        AssertKey(altUp, KeyCode.Up, KeyModifiers.Alt);
        AssertKey(shiftCtrlLeft, KeyCode.Left, KeyModifiers.Shift | KeyModifiers.Ctrl);
        AssertKey(ctrlRight, KeyCode.Right, KeyModifiers.Ctrl);
        return Task.CompletedTask;
    }

    private static Task Xterm_FunctionKeys_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var ss3F1 = Decode(decoder, "\u001bOP");
        var ss3F4 = Decode(decoder, "\u001bOS");
        var csiF8 = Decode(decoder, "\u001b[19~");
        var shiftF11 = Decode(decoder, "\u001b[23;2~");
        var ctrlAltF12 = Decode(decoder, "\u001b[24;7~");

        // Assert
        AssertKey(ss3F1, KeyCode.F1);
        AssertKey(ss3F4, KeyCode.F4);
        AssertKey(csiF8, KeyCode.F8);
        AssertKey(shiftF11, KeyCode.F11, KeyModifiers.Shift);
        AssertKey(ctrlAltF12, KeyCode.F12, KeyModifiers.Alt | KeyModifiers.Ctrl);
        return Task.CompletedTask;
    }

    private static Task Kitty_CsiUCombos_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var ctrlEnter = Decode(decoder, "\u001b[13;5u");
        var shiftAltTab = Decode(decoder, "\u001b[9;4u");
        var ctrlAltBackspace = Decode(decoder, "\u001b[127;7u");
        var repeatCtrlK = Decode(decoder, "\u001b[107;5;2u");
        var releaseCtrlK = Decode(decoder, "\u001b[107;5:3u");

        // Assert
        AssertKey(ctrlEnter, KeyCode.Enter, KeyModifiers.Ctrl);
        AssertKey(shiftAltTab, KeyCode.Tab, KeyModifiers.Shift | KeyModifiers.Alt);
        AssertKey(ctrlAltBackspace, KeyCode.Backspace, KeyModifiers.Alt | KeyModifiers.Ctrl);
        AssertKey(repeatCtrlK, KeyCode.Character, KeyModifiers.Ctrl, "k", isRepeat: true);
        AssertKeyRelease(releaseCtrlK, KeyCode.Character, KeyModifiers.Ctrl, "k");
        return Task.CompletedTask;
    }

    private static Task WezTerm_ModifyOtherKeys_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var altSlash = Decode(decoder, "\u001b[27;3;47~");
        var ctrlSemicolon = Decode(decoder, "\u001b[27;5;59~");
        var shiftCtrlK = Decode(decoder, "\u001b[27;6;107~");

        // Assert
        AssertKey(altSlash, KeyCode.Character, KeyModifiers.Alt, "/");
        AssertKey(ctrlSemicolon, KeyCode.Character, KeyModifiers.Ctrl, ";");
        AssertKey(shiftCtrlK, KeyCode.Character, KeyModifiers.Shift | KeyModifiers.Ctrl, "k");
        return Task.CompletedTask;
    }

    private static Task Konsole_MetaCursorAndCsiU_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var metaLeft = Decode(decoder, "\u001b[1;9D");
        var metaB = Decode(decoder, "\u001b[98;9u");
        var ctrlMetaB = Decode(decoder, "\u001b[98;13u");

        // Assert
        AssertKey(metaLeft, KeyCode.Left, KeyModifiers.Meta);
        AssertKey(metaB, KeyCode.Character, KeyModifiers.Meta, "b");
        AssertKey(ctrlMetaB, KeyCode.Character, KeyModifiers.Ctrl | KeyModifiers.Meta, "b");
        return Task.CompletedTask;
    }

    private static Task Urxvt1015_Mouse_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var press = Decode(decoder, "\u001b[0;11;7M");
        var motion = Decode(decoder, "\u001b[35;11;7M");

        // Assert
        AssertMouse<MouseClickMsg>(press, MouseEventType.Press, MouseButton.Left, 10, 6, KeyModifiers.None);
        AssertMouse<MouseMotionMsg>(motion, MouseEventType.Motion, MouseButton.None, 10, 6, KeyModifiers.None);
        return Task.CompletedTask;
    }

    private static Task Alacritty_AltFallback_Decode()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var altLeft = decoder.Decode([0x1B, 0x1B, (byte)'[', (byte)'D'], timeoutExpired: false);
        var altRight = decoder.Decode([0x1B, 0x1B, (byte)'[', (byte)'C'], timeoutExpired: false);
        var altEnter = decoder.Decode([0x1B, 0x0D], timeoutExpired: false);
        var backTab = Decode(decoder, "\u001b[Z");

        // Assert
        AssertKey(altLeft, KeyCode.Left, KeyModifiers.Alt);
        AssertKey(altRight, KeyCode.Right, KeyModifiers.Alt);
        AssertKey(altEnter, KeyCode.Enter, KeyModifiers.Alt);
        AssertKey(backTab, KeyCode.Tab, KeyModifiers.Shift);
        return Task.CompletedTask;
    }

    private static async Task TerminalReader_FocusPasteRoundTrip()
    {
        // Arrange
        var payload = Encoding.UTF8.GetBytes("\u001b[I\u001b[200~hello\nγειά\u001b[201~\u001b[O");
        var stream = new ChunkedReadStream(payload, maxChunkSize: 2);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        // Assert
        TestAssert.Equal(5, events.Count, "Fixture stream should decode focus + paste roundtrip.");
        TestAssert.True(events[0] is FocusInMsg, "First fixture event should be focus-in.");
        TestAssert.True(events[1] is PasteStartMsg, "Second fixture event should be paste-start.");
        TestAssert.True(events[2] is PasteMsg { Content: "hello\nγειά" }, "Third fixture event should aggregate paste payload.");
        TestAssert.True(events[3] is PasteEndMsg, "Fourth fixture event should be paste-end.");
        TestAssert.True(events[4] is FocusOutMsg, "Fifth fixture event should be focus-out.");
    }

    private static Task AppleTerminal_AltFallback_Decodes()
    {
        // Arrange
        var decoder = new EventDecoder();

        // Act
        var altBackspace = decoder.Decode(new byte[] { 0x1B, 0x7F }, timeoutExpired: false);
        var altTab = decoder.Decode(new byte[] { 0x1B, 0x09 }, timeoutExpired: false);
        var focusIn = Decode(decoder, "\u001b[I");

        // Assert
        AssertKey(altBackspace, KeyCode.Backspace, KeyModifiers.Alt);
        AssertKey(altTab, KeyCode.Tab, KeyModifiers.Alt);
        TestAssert.True(focusIn.Message is FocusInMsg, "Focus-in marker should decode consistently in fallback fixtures.");
        return Task.CompletedTask;
    }

    private static DecodeResult Decode(EventDecoder decoder, string sequence)
    {
        return decoder.Decode(Encoding.UTF8.GetBytes(sequence), timeoutExpired: false);
    }

    private static void AssertKey(
        DecodeResult result,
        KeyCode code,
        KeyModifiers modifiers = KeyModifiers.None,
        string text = "",
        bool isRepeat = false)
    {
        if (result.Message is not KeyPressMsg key)
        {
            throw new InvalidOperationException($"Expected KeyPressMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (key.Code != code
            || key.Modifiers != modifiers
            || !string.Equals(key.Text, text, StringComparison.Ordinal)
            || key.IsRepeat != isRepeat)
        {
            throw new InvalidOperationException(
                $"Expected key(code={code}, modifiers={modifiers}, text=\"{text}\", repeat={isRepeat}) " +
                $"but got key(code={key.Code}, modifiers={key.Modifiers}, text=\"{key.Text}\", repeat={key.IsRepeat}).");
        }
    }

    private static void AssertKeyRelease(
        DecodeResult result,
        KeyCode code,
        KeyModifiers modifiers = KeyModifiers.None,
        string text = "")
    {
        if (result.Message is not KeyReleaseMsg key)
        {
            throw new InvalidOperationException($"Expected KeyReleaseMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (key.Code != code || key.Modifiers != modifiers || !string.Equals(key.Text, text, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Expected release(code={code}, modifiers={modifiers}, text=\"{text}\") " +
                $"but got release(code={key.Code}, modifiers={key.Modifiers}, text=\"{key.Text}\").");
        }
    }

    private static void AssertMouse<TMouse>(
        DecodeResult result,
        MouseEventType eventType,
        MouseButton button,
        int x,
        int y,
        KeyModifiers modifiers)
        where TMouse : MouseMsg
    {
        if (result.Message is not TMouse mouse)
        {
            throw new InvalidOperationException($"Expected {typeof(TMouse).Name} but got {result.Message?.GetType().Name ?? "null"}.");
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

    private sealed class ChunkedReadStream(byte[] payload, int maxChunkSize) : Stream
    {
        private int _position;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => payload.Length;

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position >= payload.Length)
            {
                return 0;
            }

            var available = payload.Length - _position;
            var chunk = Math.Min(Math.Min(maxChunkSize, count), available);
            Array.Copy(payload, _position, buffer, offset, chunk);
            _position += chunk;
            return chunk;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_position >= payload.Length)
            {
                return ValueTask.FromResult(0);
            }

            var available = payload.Length - _position;
            var chunk = Math.Min(Math.Min(maxChunkSize, buffer.Length), available);
            payload.AsSpan(_position, chunk).CopyTo(buffer.Span);
            _position += chunk;
            return ValueTask.FromResult(chunk);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
