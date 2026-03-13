using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal enum InputScopeBehavior
{
    ContinueWhenUnhandled = 0,
    CaptureWhileActive = 1,
}
