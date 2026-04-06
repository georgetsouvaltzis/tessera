using System.ComponentModel;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct TextInputUpdateResult(bool Changed, bool Submitted);
