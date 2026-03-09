using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Messages;

public sealed record CapabilityMsg(string Name, string? Value, string Raw) : IMessage;

