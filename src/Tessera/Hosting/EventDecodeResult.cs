using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Represents the result of decoding terminal input for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct EventDecodeResult(int Consumed, Message? Message, bool NeedMoreData);
