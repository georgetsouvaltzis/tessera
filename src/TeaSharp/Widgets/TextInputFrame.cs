using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct TextInputFrame(string Text, int CursorColumn, bool PlaceholderVisible);
