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
