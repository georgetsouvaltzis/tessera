using Tessera.Core.Abstractions;

namespace Tessera.Core.Input.Decoding;

internal readonly record struct DecodeResult(int Consumed, IMessage? Message, bool NeedMoreData);
