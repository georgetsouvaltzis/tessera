using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
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
        yield return new TestCase("TerminalReader_EscapeThenDelayedChar_EmitsEscapeThenChar", EscapeThenDelayedChar_EmitsEscapeThenChar);
        yield return new TestCase("TerminalReader_EscapeThenImmediateChar_EmitsAltChar", EscapeThenImmediateChar_EmitsAltChar);
        yield return new TestCase("TerminalReader_CancelledWhileReadPending_Exits", CancelledWhileReadPending_Exits);
    }

    private static async Task BracketedPaste_AggregatesContent()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("\u001b[200~hello\nworld\u001b[201~"));
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

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
        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

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
        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        // Assert
        TestAssert.Equal(1, events.Count, "Trailing ESC should emit one key event after timeout.");
        TestAssert.True(
            events[0] is KeyPressMsg { Code: KeyCode.Escape },
            "Trailing ESC should decode as Escape key.");
    }

    private static async Task EscapeThenDelayedChar_EmitsEscapeThenChar()
    {
        // Arrange
        var stream = new TimedChunkReadStream(
            [(new byte[] { 0x1B }, 0), (Encoding.UTF8.GetBytes("i"), 35)]);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        // Assert
        TestAssert.Equal(2, events.Count, "Delayed post-ESC input should produce Escape and then plain character.");
        TestAssert.True(events[0] is KeyPressMsg { Code: KeyCode.Escape }, "First message should be Escape.");
        TestAssert.True(
            events[1] is KeyPressMsg { Code: KeyCode.Character, Text: "i", Modifiers: KeyModifiers.None },
            "Second message should be plain 'i'.");
    }

    private static async Task EscapeThenImmediateChar_EmitsAltChar()
    {
        // Arrange
        var stream = new TimedChunkReadStream(
            [(new byte[] { 0x1B }, 0), (Encoding.UTF8.GetBytes("i"), 0)]);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        // Act
        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        // Assert
        TestAssert.Equal(1, events.Count, "Immediate post-ESC input should decode as one Alt-modified character.");
        TestAssert.True(
            events[0] is KeyPressMsg { Code: KeyCode.Character, Text: "i", Modifiers: KeyModifiers.Alt },
            "Message should be alt+i.");
    }

    private static async Task CancelledWhileReadPending_Exits()
    {
        // Arrange
        using var stream = new BlockingReadStream();
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));

        // Act
        var run = reader.StreamEventsAsync(_ => { }, cts.Token);
        await run.WaitAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        // Assert
        TestAssert.True(run.IsCompletedSuccessfully, "Reader should exit when cancellation is requested even if read is still pending.");
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

    private sealed class TimedChunkReadStream((byte[] Data, int DelayMs)[] chunks) : Stream
    {
        private int _chunkIndex;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_chunkIndex >= chunks.Length)
            {
                return 0;
            }

            var (data, delayMs) = chunks[_chunkIndex++];
            if (delayMs > 0)
            {
                await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
            }

            var length = Math.Min(buffer.Length, data.Length);
            data.AsSpan(0, length).CopyTo(buffer.Span);
            return length;
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

    private sealed class BlockingReadStream : Stream
    {
        private readonly TaskCompletionSource<int> _pendingRead = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            _ = buffer;
            _ = cancellationToken;
            return new ValueTask<int>(_pendingRead.Task);
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
