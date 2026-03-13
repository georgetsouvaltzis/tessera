using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct TextInputUpdateResult(bool Changed, bool Submitted);
