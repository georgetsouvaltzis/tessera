using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Messages;

public sealed record EffectErrorMsg(Exception Exception) : IMessage;
