using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Messages;

public sealed record QuitMsg : IMessage;

public sealed record InterruptMsg : IMessage;

public sealed record WindowSizeMsg(int Width, int Height) : IMessage;

public sealed record TickMsg(DateTimeOffset Timestamp) : IMessage;

public sealed record FocusInMsg : IMessage;

public sealed record FocusOutMsg : IMessage;

public sealed record PasteStartMsg : IMessage;

public sealed record PasteEndMsg : IMessage;

public sealed record PasteMsg(string Content) : IMessage;

public enum ModeReportState
{
    Unknown = 0,
    Set = 1,
    Reset = 2,
    PermanentlySet = 3,
    PermanentlyReset = 4,
}

public sealed record ModeReportMsg(int Mode, ModeReportState State) : IMessage;

public sealed record UnknownInputMsg(string Raw) : IMessage;

public sealed record CommandErrorMsg(Exception Exception) : IMessage;

public sealed record BatchMsg(IReadOnlyList<Command> Commands) : IMessage;

public sealed record SequenceMsg(IReadOnlyList<Command> Commands) : IMessage;
