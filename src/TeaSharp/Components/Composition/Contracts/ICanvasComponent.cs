using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Components;

public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}

