using System.Text;
using NUnit.Framework;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TerminalReaderMouseLeakRegressionTests
{
    [Test]
    public async Task TerminalReader_MouseLeakRegression_SplitEscBracketAcrossTimeout_DoesNotEmitCharacterFragments()
    {
        var stream = new TimedChunkReadStream(
        [
            (Encoding.UTF8.GetBytes("\u001b["), 0),
            (Encoding.UTF8.GetBytes("<32;83;7M\u001b["), 35),
            (Encoding.UTF8.GetBytes("<0;84;7M"), 35),
        ]);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        Assert.That(events.Count, Is.EqualTo(2), "Split/repeated SGR mouse chunks should decode to two mouse events.");
        Assert.That(
            events[0] is MouseMotionMsg { Button: MouseButton.Left, X: 82, Y: 6 },
            Is.True,
            "First decoded event should be the SGR motion report.");
        Assert.That(
            events[1] is MouseClickMsg { Button: MouseButton.Left, X: 83, Y: 6 },
            Is.True,
            "Second decoded event should be the SGR press report.");

        AssertNoLeak(events);
    }

    private static void AssertNoLeak(List<IMessage> events)
    {
        foreach (var message in events)
        {
            if (message is KeyPressMsg { Code: KeyCode.Character or KeyCode.Escape })
            {
                Assert.Fail("Split SGR mouse chunks should not leak into character/escape events.");
            }
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

            var chunk = chunks[_chunkIndex++];
            if (chunk.DelayMs > 0)
            {
                await Task.Delay(chunk.DelayMs, cancellationToken).ConfigureAwait(false);
            }

            if (chunk.Data.Length > buffer.Length)
            {
                throw new InvalidOperationException("Chunk payload exceeds requested buffer length.");
            }

            chunk.Data.CopyTo(buffer);
            return chunk.Data.Length;
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
