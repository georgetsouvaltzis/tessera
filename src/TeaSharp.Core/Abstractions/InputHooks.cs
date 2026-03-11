using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Abstractions;

public readonly record struct InputHooks
{
    public Func<MouseMsg, Effect?>? OnMouse { get; init; }
}
