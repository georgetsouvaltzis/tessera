using TeaSharp.Components.Primitives;
namespace TeaSharp.Components.Composition;

public enum InputScopeKind
{
    System = 0,
    Modal = 100,
    Palette = 200,
    Command = 300,
    FocusedRegion = 400,
    Global = 500,
}
