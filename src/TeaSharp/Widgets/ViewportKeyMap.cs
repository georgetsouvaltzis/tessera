using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ViewportKeyMap : IWidgetKeyMap
{
    public static ViewportKeyMap Default { get; } = new();

    public KeyBinding Up { get; } = new("up/k", "scroll up", "up", "k");
    public KeyBinding Down { get; } = new("down/j", "scroll down", "down", "j");
    public KeyBinding PageUp { get; } = new("pgup", "page up", "pageup");
    public KeyBinding PageDown { get; } = new("pgdn", "page down", "pagedown");
    public KeyBinding Home { get; } = new("home", "top", "home");
    public KeyBinding End { get; } = new("end", "bottom", "end");
    public KeyBinding Left { get; } = new("left/h", "scroll left", "left", "h");
    public KeyBinding Right { get; } = new("right/l", "scroll right", "right", "l");

    public IReadOnlyList<KeyBinding> HelpBindings =>
    [
        Up,
        Down,
        PageUp,
        PageDown,
        Left,
        Right,
        Home,
        End,
    ];
}
