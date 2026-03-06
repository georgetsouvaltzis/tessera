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

    private static async Task TerminalReader_FocusPasteRoundTrip()
    {
        // Arrange
        var payload = Encoding.UTF8.GetBytes("\u001b[I\u001b[200~hello\nγειά\u001b[201~\u001b[O");
        var stream = new ChunkedReadStream(payload, maxChunkSize: 2);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(CancellationToken.None, events.Add);

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

    private static void AssertKey(DecodeResult result, KeyCode code, KeyModifiers modifiers = KeyModifiers.None, string text = "")
    {
        if (result.Message is not KeyPressMsg key)
        {
            throw new InvalidOperationException($"Expected KeyPressMsg but got {result.Message?.GetType().Name ?? "null"}.");
        }

        if (key.Code != code || key.Modifiers != modifiers || !string.Equals(key.Text, text, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Expected key(code={code}, modifiers={modifiers}, text=\"{text}\") but got key(code={key.Code}, modifiers={key.Modifiers}, text=\"{key.Text}\").");
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
