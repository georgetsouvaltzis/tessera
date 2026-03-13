using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal enum InputScopeKind
{
    System = 0,
    Modal = 100,
    Palette = 200,
    CommandBar = 300,
    FocusedRegion = 400,
    Global = 500,
}
