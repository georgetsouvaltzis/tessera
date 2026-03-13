using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct TextInputFrame(string Text, int CursorColumn, bool PlaceholderVisible);
