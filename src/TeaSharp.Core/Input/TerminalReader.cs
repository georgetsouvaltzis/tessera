using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Input;

public sealed class TerminalReader(Stream input, EventDecoder decoder, TimeSpan escapeTimeout)
{
    private const int DefaultReadBufferSize = 4096;

    public async Task StreamEventsAsync(CancellationToken cancellationToken, Action<IMessage> onEvent)
    {
        var pending = new List<byte>(DefaultReadBufferSize);
        var readBuffer = new byte[DefaultReadBufferSize];
        var state = new PasteState();

        while (!cancellationToken.IsCancellationRequested)
        {
            var read = await input.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), cancellationToken)
                .ConfigureAwait(false);

            if (read <= 0)
            {
                break;
            }

            pending.AddRange(readBuffer.AsSpan(0, read).ToArray());
            Drain(pending, onEvent, state, timeoutExpired: false);
        }

        if (pending.Count > 0)
        {
            await Task.Delay(escapeTimeout, cancellationToken).ConfigureAwait(false);
            Drain(pending, onEvent, state, timeoutExpired: true);
        }
    }

    private void Drain(List<byte> pending, Action<IMessage> onEvent, PasteState state, bool timeoutExpired)
    {
        while (pending.Count > 0)
        {
            var result = decoder.Decode(pending.ToArray().AsSpan(), timeoutExpired);
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

            pending.RemoveRange(0, result.Consumed);
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
}
