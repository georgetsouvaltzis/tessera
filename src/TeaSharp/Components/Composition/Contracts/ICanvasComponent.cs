using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components.Composition;

public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}

