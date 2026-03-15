using System.ComponentModel;

namespace TeaSharp.Hosting;

/// <summary>
/// Represents the result of decoding terminal input for advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct EventDecodeResult(int Consumed, Message? Message, bool NeedMoreData);
