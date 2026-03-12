using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public enum KeyboardRoutingMode
{
    FocusedOnly = 0,
    Broadcast = 1,
}
