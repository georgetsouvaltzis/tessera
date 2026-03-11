using TeaSharp.Core.Abstractions;

namespace TeaSharp.Components.Composition;

public readonly record struct InputRouteResult(bool Handled, Command? Command = null)
{
    public static InputRouteResult NotHandled => default;

    public static InputRouteResult HandledWithoutCommand => new(true, null);

    public static InputRouteResult FromCommand(Command? command) => new(true, command);
}
