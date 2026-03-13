using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Internal;

namespace TeaSharp.Controls;

/// <summary>
/// Represents the base type for custom TeaSharp controls.
/// </summary>
/// <remarks>
/// Derive from this type when built-in controls are not enough. Normal apps work with controls directly, while
/// TeaSharp adapts them to the underlying composition/runtime engine behind the scenes.
/// </remarks>
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

    /// <summary>
    /// Gets or sets a value indicating whether the control currently owns focus.
    /// </summary>
    public virtual bool IsFocused { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control should ignore interaction.
    /// </summary>
    public virtual bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control should remain interactive but not mutate its value.
    /// </summary>
    public virtual bool IsReadOnly { get; set; }

    /// <summary>
    /// Requests focus for the next composition pass.
    /// </summary>
    /// <remarks>
    /// Focus requests are one-shot. If multiple controls request focus during the same pass, the most recent
    /// request wins.
    /// </remarks>
    public void RequestFocus()
    {
        _focusRequestPending = true;
        _focusRequestOrder = Interlocked.Increment(ref s_focusRequestCounter);
    }

    /// <summary>
    /// Renders the control into the provided canvas bounds.
    /// </summary>
    /// <param name="canvas">The target canvas.</param>
    /// <param name="rect">The bounds assigned to the control.</param>
    public abstract void Render(Canvas canvas, Rect rect);

    /// <summary>
    /// Handles a message that does not depend on pointer bounds.
    /// </summary>
    /// <param name="message">The message to process.</param>
    /// <returns><see langword="true"/> when the message was handled; otherwise, <see langword="false"/>.</returns>
    public virtual bool Handle(Message message)
    {
        return false;
    }

    /// <summary>
    /// Handles a message with the current control bounds.
    /// </summary>
    /// <param name="message">The message to process.</param>
    /// <param name="bounds">The current control bounds.</param>
    /// <returns><see langword="true"/> when the message was handled; otherwise, <see langword="false"/>.</returns>
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
