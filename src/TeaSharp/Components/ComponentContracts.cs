using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}

public interface IStatefulComponent : ICanvasComponent
{
    bool Update(IMessage message);
}

public interface IMouseStatefulComponent : ICanvasComponent
{
    bool UpdateMouse(MouseMsg message, Rect bounds);
}

public interface IFocusableComponent : ICanvasComponent
{
    bool Focused { get; set; }
}

public interface IInteractiveComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent;

public readonly record struct ComponentSlot(ICanvasComponent Component, Rect Bounds);
