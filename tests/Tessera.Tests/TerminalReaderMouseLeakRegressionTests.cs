using NUnit.Framework;
using Tessera.Core.Abstractions;
using Tessera.Core.Input;
using Tessera.Core.Messages;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TerminalReaderMouseLeakRegressionTests
{
    [Test]
    public async Task TerminalReaderMouseLeakRegressionSplitEscBracketAcrossTimeoutDoesNotEmitCharacterFragments()
    {
        var stream = new TimedChunkReadStream(
        [
            ("\e["u8.ToArray(), 0),
            ("<32;83;7M\e["u8.ToArray(), 35),
            ("<0;84;7M"u8.ToArray(), 35)
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

    [Test]
    public async Task
        TerminalReaderMouseLeakRegressionEscapeThenDelayedCsiMouseAcrossTimeoutDoesNotEmitCharacterFragments()
    {
        var stream = new TimedChunkReadStream(
        [
            ("\e"u8.ToArray(), 0),
            ("[<32;83;7M"u8.ToArray(), 35),
            ("\e"u8.ToArray(), 35),
            ("[<0;84;7M"u8.ToArray(), 35)
        ]);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        Assert.That(events.Count, Is.EqualTo(2),
            "Split ESC then delayed SGR reports should decode to two mouse events.");
        Assert.That(
            events[0] is MouseMotionMsg { Button: MouseButton.Left, X: 82, Y: 6 },
            Is.True,
            "First decoded event should be motion.");
        Assert.That(
            events[1] is MouseClickMsg { Button: MouseButton.Left, X: 83, Y: 6 },
            Is.True,
            "Second decoded event should be click.");

        AssertNoLeak(events);
    }

    [Test]
    public async Task TerminalReaderMouseLeakRegressionSplitX10AcrossTimeoutDoesNotLeakAndStaysZeroBased()
    {
        var stream = new TimedChunkReadStream(
        [
            ([0x1B, (byte)'[', (byte)'M', (byte)' '], 0),
            ("!!"u8.ToArray(), 35)
        ]);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        Assert.That(events.Count, Is.EqualTo(1), "Split X10 report should decode as a single mouse event.");
        Assert.That(
            events[0] is MouseClickMsg { Button: MouseButton.Left, X: 0, Y: 0 },
            Is.True,
            "Split X10 top-left press should remain zero-based and not leak.");
        AssertNoLeak(events);
    }

    [Test]
    public async Task TerminalReaderMouseLeakRegressionSplitX10MotionAcrossTimeoutDoesNotEmitPress()
    {
        var stream = new TimedChunkReadStream(
        [
            ([0x1B, (byte)'[', (byte)'M', (byte)'C'], 0),
            ("!!"u8.ToArray(), 35)
        ]);
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        await reader.StreamEventsAsync(events.Add, CancellationToken.None);

        Assert.That(events.Count, Is.EqualTo(1), "Split X10 motion should decode as a single mouse event.");
        Assert.That(
            events[0] is MouseMotionMsg { Button: MouseButton.None, X: 0, Y: 0 },
            Is.True,
            "Split X10 motion should stay motion and preserve zero-based coordinates.");
        Assert.That(
            events.Any(static message => message is MouseClickMsg),
            Is.False,
            "Motion-only X10 sequence must not generate synthetic click events.");
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

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer,
            CancellationToken cancellationToken = default)
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
