using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal enum ScreenLayer
{
    Base = 0,
    Overlay = 100,
    Toast = 200,
    Modal = 300,
    Palette = 400,
}
