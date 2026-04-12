using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Wraps the built-in input decoder for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class EventDecoder : IEventDecoder
{
    private readonly global::Tessera.Core.Input.Decoding.EventDecoder _inner = new();

    /// <summary>
    /// Executes decode.
    /// </summary>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="timeoutExpired">The timeout expired value.</param>
    /// <returns><see langword="true" /> when decode succeeds.</returns>
    public EventDecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired) =>
        _inner.Decode(buffer, timeoutExpired).ToHosting();
}
