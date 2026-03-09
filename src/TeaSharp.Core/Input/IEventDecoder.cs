namespace TeaSharp.Core.Input;

public interface IEventDecoder
{
    DecodeResult Decode(ReadOnlySpan<byte> buffer, bool timeoutExpired);
}
