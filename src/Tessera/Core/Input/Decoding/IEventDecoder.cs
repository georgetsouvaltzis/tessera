using System.ComponentModel;

namespace Tessera.Core.Input.Decoding;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IEventDecoder
{
    DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
