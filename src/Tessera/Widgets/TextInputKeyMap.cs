using System.ComponentModel;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TextInputKeyMap : IWidgetKeyMap
{
    public static TextInputKeyMap Default { get; } = new();

    public KeyBinding Submit { get; } = new("enter", "submit", "enter");
    public KeyBinding Left { get; } = new("left", "cursor left", "left");
    public KeyBinding Right { get; } = new("right", "cursor right", "right");
    public KeyBinding Home { get; } = new("home", "line start", "home");
    public KeyBinding End { get; } = new("end", "line end", "end");
    public KeyBinding WordLeft { get; } = new("alt+left|alt+b", "word left", "alt+left", "ctrl+left", "alt+b", "meta+b");
    public KeyBinding WordRight { get; } = new("alt+right|alt+f", "word right", "alt+right", "ctrl+right", "alt+f", "meta+f");
    public KeyBinding DeleteBackward { get; } = new("backspace", "delete left", "backspace");
    public KeyBinding DeleteForward { get; } = new("delete", "delete right", "delete");
    public KeyBinding DeleteWordBackward { get; } = new("alt+backspace|alt+h", "delete word left", "alt+backspace", "ctrl+backspace", "alt+h", "meta+h");
    public KeyBinding DeleteWordForward { get; } = new("alt+delete|alt+d", "delete word right", "alt+delete", "ctrl+delete", "alt+d", "meta+d");
    public KeyBinding SelectAll { get; } = new("ctrl+a", "select all", "ctrl+a");

    public IReadOnlyList<KeyBinding> HelpBindings =>
    [
        Submit,
        Left,
        Right,
        WordLeft,
        WordRight,
        DeleteBackward,
        DeleteForward,
        SelectAll,
    ];
}
