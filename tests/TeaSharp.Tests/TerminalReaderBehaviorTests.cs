using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class TerminalReaderBehaviorTests
{
    public static async Task RunAsync()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("\u001b[200~hello\nworld\u001b[201~"));
        var reader = new TerminalReader(stream, new EventDecoder(), TimeSpan.FromMilliseconds(10));
        var events = new List<IMessage>();

        await reader.StreamEventsAsync(CancellationToken.None, events.Add);

        if (events.Count != 3)
        {
            throw new InvalidOperationException($"Expected 3 messages, got {events.Count}.");
        }

        if (events[0] is not PasteStartMsg)
        {
            throw new InvalidOperationException("Expected first message to be PasteStartMsg.");
        }

        if (events[1] is not PasteMsg { Content: "hello\nworld" })
        {
            throw new InvalidOperationException("Expected second message to be PasteMsg with full aggregated content.");
        }

        if (events[2] is not PasteEndMsg)
        {
            throw new InvalidOperationException("Expected third message to be PasteEndMsg.");
        }
    }
}
