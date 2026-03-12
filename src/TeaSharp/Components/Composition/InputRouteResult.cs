using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct InputRouteResult(bool Handled, Effect? Effect = null)
{
    public static InputRouteResult NotHandled => default;

    public static InputRouteResult HandledWithoutEffect => new(true, null);

    public static InputRouteResult FromEffect(Effect? effect) => new(true, effect);
}
