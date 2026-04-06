using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Represents the input decoder seam used by advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IEventDecoder
{
    EventDecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
