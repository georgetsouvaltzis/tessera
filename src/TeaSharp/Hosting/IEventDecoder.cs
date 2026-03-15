using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Represents the input decoder seam used by advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IEventDecoder
{
    EventDecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
