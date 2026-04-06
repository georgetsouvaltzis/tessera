using System.ComponentModel;

namespace Tessera.Core.Input;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IEventDecoder
{
    DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
