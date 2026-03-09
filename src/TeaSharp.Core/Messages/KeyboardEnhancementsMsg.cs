using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Messages;

public sealed record KeyboardEnhancementsMsg(int Flags) : IMessage
{
    public bool SupportsKeyDisambiguation() => Flags > 0;

    public bool SupportsEventTypes() => (Flags & 0b10) != 0;
}

