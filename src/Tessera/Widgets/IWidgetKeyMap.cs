using System.ComponentModel;

namespace Tessera.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IWidgetKeyMap
{
    IReadOnlyList<KeyBinding> HelpBindings { get; }
}
