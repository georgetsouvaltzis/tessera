using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Messages;

public sealed record ModeReportMsg(int Mode, ModeReportState State) : IMessage;

