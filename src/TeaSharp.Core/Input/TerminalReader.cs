using System.ComponentModel;
using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Input;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class TerminalReader(Stream input, IEventDecoder decoder, TimeSpan escapeTimeout)
{
    private const int DefaultReadBufferSize = 4096;

    public async Task StreamEventsAsync(CancellationToken cancellationToken, Action<IMessage> onEvent)
    {
        var pending = new PendingByteBuffer(DefaultReadBufferSize);
        var readBuffer = new byte[DefaultReadBufferSize];
        var state = new PasteState();
        var readTask = input.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), cancellationToken).AsTask();

        while (!cancellationToken.IsCancellationRequested)
        {
            if (pending.Count > 0)
            {
                var completed = await Task.WhenAny(
                        readTask,
                        Task.Delay(escapeTimeout, cancellationToken))
                    .ConfigureAwait(false);
                if (!ReferenceEquals(completed, readTask))
                {
                    Drain(pending, onEvent, state, timeoutExpired: true);
                    continue;
                }
            }

            int read;
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

            if (read <= 0)
            {
                break;
            }

            pending.Append(readBuffer.AsSpan(0, read));
            Drain(pending, onEvent, state, timeoutExpired: false);
            readTask = input.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), cancellationToken).AsTask();
        }

        if (pending.Count > 0)
        {
            await Task.Delay(escapeTimeout, cancellationToken).ConfigureAwait(false);
            Drain(pending, onEvent, state, timeoutExpired: true);
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

    private sealed class PendingByteBuffer
    {
        private byte[] _buffer;
        private int _start;
        private int _end;

        public PendingByteBuffer(int initialCapacity)
        {
            _buffer = new byte[Math.Max(initialCapacity, 64)];
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

            var next = new byte[newSize];
            if (currentCount > 0)
            {
                _buffer.AsSpan(_start, currentCount).CopyTo(next);
            }

            _buffer = next;
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
    }
}
