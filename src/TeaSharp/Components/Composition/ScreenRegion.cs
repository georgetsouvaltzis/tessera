using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public sealed class ScreenRegion
{
    private readonly Action<Canvas, Rect> _render;
    private readonly Func<IMessage, bool>? _update;
    private readonly Func<MouseMsg, Rect, bool>? _updateMouse;
    private readonly IFocusableComponent? _focusTarget;
    private readonly Action? _onFocus;

    internal ScreenRegion(
        string id,
        Rect bounds,
        Action<Canvas, Rect> render,
        Func<IMessage, bool>? update,
        Func<MouseMsg, Rect, bool>? updateMouse,
        bool focusable,
        bool focusOnClick,
        bool interceptsPointer,
        int layer,
        IFocusableComponent? focusTarget,
        Action? onFocus)
    {
        Id = id;
        Bounds = bounds;
        _render = render;
        _update = update;
        _updateMouse = updateMouse;
        Focusable = focusable;
        FocusOnClick = focusOnClick;
        InterceptsPointer = interceptsPointer;
        Layer = layer;
        _focusTarget = focusTarget;
        _onFocus = onFocus;
    }

    public string Id { get; }

    public Rect Bounds { get; }

    public bool Focusable { get; }

    public bool FocusOnClick { get; }

    public bool InterceptsPointer { get; }

    public int Layer { get; }

    public void Render(Canvas canvas)
    {
        _render(canvas, Bounds);
    }

    public bool Update(IMessage message)
    {
        return _update?.Invoke(message) ?? false;
    }

    public bool UpdateMouse(MouseMsg message)
    {
        return _updateMouse?.Invoke(message, Bounds) ?? false;
    }

    internal void ApplyFocus(bool focused, bool invokeFocus)
    {
        if (_focusTarget is not null)
        {
            _focusTarget.Focused = focused;
        }

        if (focused && invokeFocus)
        {
            _onFocus?.Invoke();
        }
    }
}
