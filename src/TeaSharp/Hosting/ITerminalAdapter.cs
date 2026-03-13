using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Represents the terminal adapter seam used by advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ITerminalAdapter : global::TeaSharp.Core.Terminal.ITerminalAdapter
{
}
