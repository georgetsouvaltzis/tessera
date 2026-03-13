using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal enum KeyboardRoutingMode
{
    FocusedOnly = 0,
    Broadcast = 1,
}
