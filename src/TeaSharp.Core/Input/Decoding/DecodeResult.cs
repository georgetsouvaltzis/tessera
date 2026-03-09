using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Input;

public readonly record struct DecodeResult(int Consumed, IMessage? Message, bool NeedMoreData);
