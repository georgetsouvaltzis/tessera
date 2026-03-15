using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Input;

internal readonly record struct DecodeResult(int Consumed, IMessage? Message, bool NeedMoreData);
