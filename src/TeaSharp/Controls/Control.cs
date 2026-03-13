using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Internal;

namespace TeaSharp.Controls;

public abstract class Control
{
    private static long s_focusRequestCounter;
    private readonly ControlComponentAdapter _componentAdapter;
    private bool _focusRequestPending;
    private long _focusRequestOrder;

    protected Control()
    {
        _componentAdapter = new ControlComponentAdapter(this);
    }

    public virtual bool IsFocused { get; set; }

    public virtual bool IsDisabled { get; set; }

    public virtual bool IsReadOnly { get; set; }

    public void RequestFocus()
    {
        _focusRequestPending = true;
        _focusRequestOrder = Interlocked.Increment(ref s_focusRequestCounter);
    }

    public abstract void Render(Canvas canvas, Rect rect);

    public virtual bool Handle(Message message)
    {
        return false;
    }

    public virtual bool Handle(Message message, Rect bounds)
    {
        return Handle(message);
    }

    internal ICanvasComponent Component => _componentAdapter;

    private sealed class ControlComponentAdapter(Control owner) : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent, IFocusRequestSource
    {
        public bool IsFocused
        {
            get => owner.IsFocused;
            set
            {
                owner.IsFocused = value;
                if (value)
                {
                    owner._focusRequestPending = false;
                }
            }
        }

        public void Render(Canvas canvas, Rect rect)
        {
            owner.Render(canvas, rect);
        }

        public bool Update(IMessage message)
        {
            if (owner.IsDisabled)
            {
                return false;
            }

            return owner.Handle(TeaMessageAdapter.ToPublic(message));
        }

        public bool UpdateMouse(MouseMsg message, Rect bounds)
        {
            if (owner.IsDisabled)
            {
                return false;
            }

            return owner.Handle(TeaMessageAdapter.ToPublic(message), bounds);
        }

        public bool TryConsumeFocusRequest(out long order)
        {
            var requested = owner._focusRequestPending;
            owner._focusRequestPending = false;
            order = owner._focusRequestOrder;
            return requested;
        }
    }
}
