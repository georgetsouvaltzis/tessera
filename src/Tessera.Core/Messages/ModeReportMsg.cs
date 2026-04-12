using Tessera.Core.Abstractions;

namespace Tessera.Core.Messages;

/// <summary>
/// Carries the state of a terminal mode report.
/// </summary>
/// <param name="Mode">The reported DEC mode number.</param>
/// <param name="State">The reported mode state.</param>
public sealed record ModeReportMsg(int Mode, ModeReportState State) : IMessage;
