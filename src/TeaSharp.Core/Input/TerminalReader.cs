using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Input;

public sealed class TerminalReader(Stream input, EventDecoder decoder, TimeSpan escapeTimeout)
{
    private const int DefaultReadBufferSize = 4096;

    public async Task StreamEventsAsync(CancellationToken cancellationToken, Action<IMessage> onEvent)
    {
        var pending = new List<byte>(DefaultReadBufferSize);
        var readBuffer = new byte[DefaultReadBufferSize];

        while (!cancellationToken.IsCancellationRequested)
        {
            var read = await input.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), cancellationToken)
                .ConfigureAwait(false);

            if (read <= 0)
            {
                break;
            }

            pending.AddRange(readBuffer.AsSpan(0, read).ToArray());
            Drain(pending, onEvent, timeoutExpired: false);
        }

        if (pending.Count > 0)
        {
            await Task.Delay(escapeTimeout, cancellationToken).ConfigureAwait(false);
            Drain(pending, onEvent, timeoutExpired: true);
        }
    }

    private void Drain(List<byte> pending, Action<IMessage> onEvent, bool timeoutExpired)
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
                onEvent(result.Message);
            }

            pending.RemoveRange(0, result.Consumed);
        }
    }
}
