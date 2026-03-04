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
}
