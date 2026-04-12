using Tessera.Core.Abstractions;
using Tessera.Core.Terminal;

namespace Tessera.Core.Messages;

/// <summary>
/// Reports the detected terminal color profile.
/// </summary>
/// <param name="Profile">The reported terminal color profile.</param>
public sealed record ColorProfileMsg(TerminalColorProfile Profile) : IMessage;
