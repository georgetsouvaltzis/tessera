using System.ComponentModel;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct TextInputFrame(string Text, int CursorColumn, bool PlaceholderVisible);
