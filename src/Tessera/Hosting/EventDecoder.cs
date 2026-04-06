using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Wraps the built-in input decoder for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class EventDecoder : IEventDecoder
{
    private readonly global::Tessera.Core.Input.EventDecoder _inner = new();

    public EventDecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired) =>
        _inner.Decode(buffer, timeoutExpired).ToHosting();
}
