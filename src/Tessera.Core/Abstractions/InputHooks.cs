using Tessera.Core.Messages;

namespace Tessera.Core.Abstractions;

public readonly record struct InputHooks
{
    public Func<MouseMsg, Effect?>? OnMouse { get; init; }
}
