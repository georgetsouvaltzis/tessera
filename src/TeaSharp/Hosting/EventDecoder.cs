using System.ComponentModel;
using TeaSharp.Internal;

namespace TeaSharp.Hosting;

/// <summary>
/// Wraps the built-in input decoder for advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class EventDecoder : IEventDecoder
{
    private readonly global::TeaSharp.Core.Input.EventDecoder _inner = new();

    public EventDecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired) =>
        _inner.Decode(buffer, timeoutExpired).ToHosting();
}
