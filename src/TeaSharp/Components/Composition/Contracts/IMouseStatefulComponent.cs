using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IMouseStatefulComponent : ICanvasComponent
{
    bool UpdateMouse(MouseMsg message, Rect bounds);
}
