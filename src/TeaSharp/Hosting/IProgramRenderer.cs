using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Represents the renderer seam used by advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IProgramRenderer : global::TeaSharp.Core.Rendering.IProgramRenderer
{
}
