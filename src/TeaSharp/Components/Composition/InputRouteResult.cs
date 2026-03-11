using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;

namespace TeaSharp.Components.Composition;

public readonly record struct InputRouteResult(bool Handled, Effect? Effect = null)
{
    public static InputRouteResult NotHandled => default;

    public static InputRouteResult HandledWithoutEffect => new(true, null);

    public static InputRouteResult FromEffect(Effect? effect) => new(true, effect);
}
