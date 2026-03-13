using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Messages;
using TeaSharp.Internal;

namespace TeaSharp.Controls;

internal static class ControlForwarder
{
    public static bool Forward(ICanvasComponent component, Message message)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(message);

        return component is IStatefulComponent stateful
            && stateful.Update(TeaMessageAdapter.ToCore(message));
    }

    public static bool Forward(ICanvasComponent component, Message message, Rect bounds)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(message);

        return component is IMouseStatefulComponent mouseStateful
            && message is PointerInput
            && mouseStateful.UpdateMouse((MouseMsg)TeaMessageAdapter.ToCore(message), bounds);
    }
}
