using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Internal;

namespace TeaSharp.Controls;

public abstract class Control : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    public virtual bool IsFocused { get; set; }

    public virtual bool IsDisabled { get; set; }

    public virtual bool IsReadOnly { get; set; }

    public abstract void Render(Canvas canvas, Rect rect);

    public virtual bool Handle(Message message)
    {
        return false;
    }

    public virtual bool Handle(Message message, Rect bounds)
    {
        return Handle(message);
    }

    protected static bool Forward(ICanvasComponent component, Message message)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(message);

        return component is IStatefulComponent stateful
            && stateful.Update(TeaMessageAdapter.ToCore(message));
    }

    protected static bool Forward(ICanvasComponent component, Message message, Rect bounds)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(message);

        return component is IMouseStatefulComponent mouseStateful
            && message is PointerInput
            && mouseStateful.UpdateMouse((MouseMsg)TeaMessageAdapter.ToCore(message), bounds);
    }

    bool IStatefulComponent.Update(IMessage message)
    {
        if (IsDisabled)
        {
            return false;
        }

        return Handle(TeaMessageAdapter.ToPublic(message));
    }

    bool IMouseStatefulComponent.UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (IsDisabled)
        {
            return false;
        }

        return Handle(TeaMessageAdapter.ToPublic(message), bounds);
    }
}
