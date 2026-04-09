using System.Buffers;
using System.ComponentModel;
using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Input;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TerminalReader(Stream input, IEventDecoder decoder, TimeSpan escapeTimeout)
{
    private const int DefaultReadBufferSize = 4096;
    private const int FinalTimeoutDrainRetries = 4;

    public async Task StreamEventsAsync(Action<IMessage> onEvent, CancellationToken cancellationToken = default)
    {
        using var pending = new PendingByteBuffer(DefaultReadBufferSize);
        var readBuffer = ArrayPool<byte>.Shared.Rent(DefaultReadBufferSize);

        try
        {
            var state = new PasteState();
            Task<int> readTask = ReadNextAsync(input, readBuffer, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                int read;

                if (pending.Count > 0)
                {
                    try
                    {
                        read = await readTask.WaitAsync(escapeTimeout, cancellationToken).ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                        Drain(pending, onEvent, state, timeoutExpired: true);
                        continue;
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }
                else
                {
                    try
                    {
                        read = await readTask.WaitAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }

                if (read <= 0)
                {
                    break;
                }

                pending.Append(readBuffer.AsSpan(0, read));
                Drain(pending, onEvent, state, timeoutExpired: false);
                readTask = ReadNextAsync(input, readBuffer, cancellationToken);
            }

            if (pending.Count > 0)
            {
                await Task.Delay(escapeTimeout, cancellationToken).ConfigureAwait(false);
                var attempt = 0;
                while (attempt < FinalTimeoutDrainRetries && pending.Count > 0)
                {
                    var before = pending.Count;
                    Drain(pending, onEvent, state, timeoutExpired: true);
                    if (pending.Count < before)
                    {
                        attempt = -1;
                    }

                    attempt++;
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(readBuffer);
        }
    }

    private void Drain(PendingByteBuffer pending, Action<IMessage> onEvent, PasteState state, bool timeoutExpired)
    {
        while (pending.Count > 0)
        {
            var result = decoder.Decode(pending.AsSpan(), timeoutExpired);
            if (result.NeedMoreData || result.Consumed <= 0)
            {
                return;
            }

            if (result.Message is not null)
            {
                if (result.Message is PasteStartMsg)
                {
                    state.IsInPaste = true;
                    state.Buffer.Clear();
                    onEvent(result.Message);
                }
                else if (result.Message is PasteEndMsg)
                {
                    if (state.IsInPaste)
                    {
                        onEvent(new PasteMsg(state.Buffer.ToString()));
                        state.Buffer.Clear();
                        state.IsInPaste = false;
                    }

                    onEvent(result.Message);
                }
                else if (state.IsInPaste)
                {
                    AppendPasteFragment(state.Buffer, result.Message);
                }
                else
                {
                    onEvent(result.Message);
                }
            }

            pending.Consume(result.Consumed);
        }
    }

    private static Task<int> ReadNextAsync(Stream input, byte[] readBuffer, CancellationToken cancellationToken)
    {
        return input.ReadAsync(readBuffer.AsMemory(0, DefaultReadBufferSize), cancellationToken).AsTask();
    }

    private static void AppendPasteFragment(StringBuilder buffer, IMessage message)
    {
        if (message is not KeyPressMsg keyPress)
        {
            return;
        }

        if (keyPress.Code == KeyCode.Character)
        {
            buffer.Append(keyPress.Text);
            return;
        }

        if (keyPress.Code == KeyCode.Enter)
        {
            buffer.Append('\n');
            return;
        }

        if (keyPress.Code == KeyCode.Tab)
        {
            buffer.Append('\t');
            return;
        }

        if (keyPress.Code == KeyCode.Backspace)
        {
            buffer.Append('\b');
        }
    }

    private sealed class PasteState
    {
        public bool IsInPaste { get; set; }

        public StringBuilder Buffer { get; } = new();
    }

    private sealed class PendingByteBuffer : IDisposable
    {
        private readonly ArrayPool<byte> _pool;
        private byte[] _buffer;
        private int _start;
        private int _end;

        public PendingByteBuffer(int initialCapacity)
        {
            _pool = ArrayPool<byte>.Shared;
            _buffer = _pool.Rent(Math.Max(initialCapacity, 64));
            _start = 0;
            _end = 0;
        }

        public int Count => _end - _start;

        public ReadOnlySpan<byte> AsSpan()
        {
            return _buffer.AsSpan(_start, Count);
        }

        public void Append(ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty)
            {
                return;
            }

            EnsureAppendCapacity(bytes.Length);
            bytes.CopyTo(_buffer.AsSpan(_end));
            _end += bytes.Length;
        }

        public void Consume(int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count >= Count)
            {
                _start = 0;
                _end = 0;
                return;
            }

            _start += count;

            if (_start >= _buffer.Length / 2)
            {
                Compact();
            }
        }

        private void EnsureAppendCapacity(int appendCount)
        {
            var currentCount = Count;
            var required = currentCount + appendCount;
            if (required <= _buffer.Length)
            {
                if (_end + appendCount > _buffer.Length)
                {
                    Compact();
                }

                return;
            }

            var newSize = _buffer.Length;
            while (newSize < required)
            {
                newSize *= 2;
            }

            var next = _pool.Rent(newSize);
            if (currentCount > 0)
            {
                _buffer.AsSpan(_start, currentCount).CopyTo(next);
            }

            var previous = _buffer;
            _buffer = next;
            _pool.Return(previous);
            _start = 0;
            _end = currentCount;
        }

        private void Compact()
        {
            var currentCount = Count;
            if (_start == 0 || currentCount == 0)
            {
                if (currentCount == 0)
                {
                    _start = 0;
                    _end = 0;
                }

                return;
            }

            _buffer.AsSpan(_start, currentCount).CopyTo(_buffer);
            _start = 0;
            _end = currentCount;
        }

        public void Dispose()
        {
            _pool.Return(_buffer);
            _buffer = [];
            _start = 0;
            _end = 0;
        }
    }
}
