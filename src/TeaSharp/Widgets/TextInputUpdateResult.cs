using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct TextInputUpdateResult(bool Changed, bool Submitted);
