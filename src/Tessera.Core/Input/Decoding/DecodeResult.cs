using Tessera.Core.Abstractions;

namespace Tessera.Core.Input;

internal readonly record struct DecodeResult(int Consumed, IMessage? Message, bool NeedMoreData);
