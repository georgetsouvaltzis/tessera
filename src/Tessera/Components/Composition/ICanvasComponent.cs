using Tessera.Components.Primitives;
using System.ComponentModel;

namespace Tessera.Components.Composition;

/// <summary>
/// Defines the canvas component contract.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ICanvasComponent
{
    /// <summary>
    /// Renders the component into the supplied canvas bounds.
    /// </summary>
    /// <param name="canvas">The target canvas.</param>
    /// <param name="rect">The target bounds.</param>
    void Render(Canvas canvas, Rect rect);
}
