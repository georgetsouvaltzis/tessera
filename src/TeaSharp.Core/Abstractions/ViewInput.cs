using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Abstractions;

public readonly record struct ViewInput
{
    public Func<MouseMsg, Command?>? OnMouse { get; init; }
}
