using Tessera.Components.Primitives;
using System.ComponentModel;

namespace Tessera.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ICanvasComponent
{
    void Render(Canvas canvas, Rect rect);
}
