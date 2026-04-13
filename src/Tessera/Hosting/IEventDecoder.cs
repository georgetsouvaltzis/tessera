using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Represents the input decoder seam used by advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IEventDecoder
{
    /// <summary>
    ///     Attempts to decode terminal input bytes into a runtime event.
    /// </summary>
    /// <param name="buffer">The unread input buffer.</param>
    /// <param name="timeoutExpired">Whether the caller's read timeout expired.</param>
    /// <returns>The decode result for the provided buffer.</returns>
    EventDecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
