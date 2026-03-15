using TeaSharp.Components.Primitives;
using TeaSharp.Layout;

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
    private bool _focusRequestPending;
    private long _focusRequestOrder;

    protected Control()
    {
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

    internal virtual LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new LayoutMeasurement(availableBounds.Width, availableBounds.Height);
    }

    internal virtual bool CanFocus => true;

    internal void ApplyFocus(bool focused)
    {
        IsFocused = focused;
        if (focused)
        {
            _focusRequestPending = false;
        }
    }

    internal bool TryConsumeFocusRequest(out long order)
    {
        var requested = _focusRequestPending;
        _focusRequestPending = false;
        order = _focusRequestOrder;
        return requested;
    }
}
