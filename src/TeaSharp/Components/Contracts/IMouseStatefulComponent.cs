using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public interface IMouseStatefulComponent : ICanvasComponent
{
    bool UpdateMouse(MouseMsg message, Rect bounds);
}

