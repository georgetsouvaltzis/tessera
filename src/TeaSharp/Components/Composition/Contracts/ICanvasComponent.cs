using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}
