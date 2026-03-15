using System.ComponentModel;

namespace TeaSharp.Core.Input;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IEventDecoder
{
    DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
