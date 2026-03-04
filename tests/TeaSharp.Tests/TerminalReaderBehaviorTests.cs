using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class TerminalReaderBehaviorTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("TerminalReader_BracketedPaste_AggregatesContent", BracketedPaste_AggregatesContent);
        yield return new TestCase("TerminalReader_ChunkedStream_DecodesMixedSequences", ChunkedStream_DecodesMixedSequences);
        yield return new TestCase("TerminalReader_TrailingEscape_EmitsEscapeAfterTimeout", TrailingEscape_EmitsEscapeAfterTimeout);
    }

    private static async Task BracketedPaste_AggregatesContent()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("\u001b[200~hello\nworld\u001b[201~"));
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(CancellationToken.None, events.Add);

        // Assert
        TestAssert.Equal(3, events.Count, "Reader should emit exactly three messages for bracketed paste");
        TestAssert.True(events[0] is PasteStartMsg, "First message should be PasteStartMsg.");
        TestAssert.True(events[1] is PasteMsg { Content: "hello\nworld" }, "Second message should be aggregated PasteMsg.");
        TestAssert.True(events[2] is PasteEndMsg, "Third message should be PasteEndMsg.");
    }

    private static async Task ChunkedStream_DecodesMixedSequences()
    {
        // Arrange
        var payload = Encoding.UTF8.GetBytes("\u001b[A\u001b[200~ab\ncd\u001b[201~\u001b[<26;4;6M");
        var stream = new ChunkedReadStream(payload, maxChunkSize: 1);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(CancellationToken.None, events.Add);

        // Assert
        TestAssert.Equal(5, events.Count, "Chunked stream should decode all expected messages.");
        TestAssert.True(events[0] is KeyPressMsg { Code: KeyCode.Up }, "First message should be Up key.");
        TestAssert.True(events[1] is PasteStartMsg, "Second message should be PasteStartMsg.");
        TestAssert.True(events[2] is PasteMsg { Content: "ab\ncd" }, "Third message should be aggregated PasteMsg.");
        TestAssert.True(events[3] is PasteEndMsg, "Fourth message should be PasteEndMsg.");
        TestAssert.True(
            events[4] is MouseClickMsg
            {
                Button: MouseButton.Right,
                X: 3,
                Y: 5,
                Modifiers: KeyModifiers.Ctrl | KeyModifiers.Alt,
            },
            "Fifth message should be Alt+Ctrl right-click mouse message.");
    }

    private static async Task TrailingEscape_EmitsEscapeAfterTimeout()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 0x1B });
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(1));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(CancellationToken.None, events.Add);

        // Assert
        TestAssert.Equal(1, events.Count, "Trailing ESC should emit one key event after timeout.");
        TestAssert.True(
            events[0] is KeyPressMsg { Code: KeyCode.Escape },
            "Trailing ESC should decode as Escape key.");
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
