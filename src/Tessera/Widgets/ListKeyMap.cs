using System.ComponentModel;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ListKeyMap : IWidgetKeyMap
{
    public static ListKeyMap Default { get; } = new();

    public KeyBinding Up { get; } = new("up/k", "move up", "up", "k");
    public KeyBinding Down { get; } = new("down/j", "move down", "down", "j");
    public KeyBinding PageUp { get; } = new("pgup", "page up", "pageup");
    public KeyBinding PageDown { get; } = new("pgdn", "page down", "pagedown");
    public KeyBinding Home { get; } = new("home", "first item", "home");
    public KeyBinding End { get; } = new("end", "last item", "end");

    public IReadOnlyList<KeyBinding> HelpBindings =>
    [
        Up,
        Down,
        PageUp,
        PageDown,
        Home,
        End
    ];
}
