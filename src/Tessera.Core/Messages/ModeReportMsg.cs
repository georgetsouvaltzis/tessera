using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

public sealed record ModeReportMsg(int Mode, ModeReportState State) : IMessage;

